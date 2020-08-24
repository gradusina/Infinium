using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;


namespace Infinium
{
    public class TablesManager
    {
        private static DataTable FrontsAllConfigDT;
        private static DataTable FrontsConfigDT;
        private static DataTable DecorAllConfigDT;
        private static DataTable DecorConfigDT;
        private static DataTable TechStoreDT;
        public static DataTable UsersDataTable;
        public static DataTable UsersPhotoDataTable;
        public static DataTable DepartmentsDataTable;
        public static DataTable PositionsDataTable;
        public static DataTable ModulesDataTable;

        public static DataTable FrontsConfigDataTableAll
        {
            get
            {
                if (FrontsAllConfigDT == null)
                {
                    FrontsAllConfigDT = new DataTable();
                    using (
                        SqlDataAdapter DA =
                            new SqlDataAdapter(
                                "SELECT * FROM FrontsConfig WHERE AccountingName IS NOT NULL AND InvNumber IS NOT NULL",
                                ConnectionStrings.CatalogConnectionString))
                    {
                        DA.Fill(FrontsAllConfigDT);
                    }
                }
                if (FrontsAllConfigDT.Columns.Contains("Excluzive"))
                    FrontsAllConfigDT.Columns.Remove("Excluzive");
                return FrontsAllConfigDT;
            }
        }

        public static DataTable FrontsConfigDataTable
        {
            get
            {
                if (FrontsConfigDT == null)
                {
                    FrontsConfigDT = new DataTable();
                    using (
                        SqlDataAdapter DA =
                            new SqlDataAdapter(
                                "SELECT * FROM FrontsConfig WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL",
                                ConnectionStrings.CatalogConnectionString))
                    {
                        DA.Fill(FrontsConfigDT);
                    }
                }
                if (FrontsConfigDT.Columns.Contains("Excluzive"))
                    FrontsConfigDT.Columns.Remove("Excluzive");
                return FrontsConfigDT;
            }
        }

        public static DataTable DecorConfigDataTableAll
        {
            get
            {
                if (DecorAllConfigDT == null)
                {
                    DecorAllConfigDT = new DataTable();
                    using (
                        SqlDataAdapter DA =
                            new SqlDataAdapter(
                                "SELECT * FROM DecorConfig WHERE AccountingName IS NOT NULL AND InvNumber IS NOT NULL",
                                ConnectionStrings.CatalogConnectionString))
                    {
                        DA.Fill(DecorAllConfigDT);
                    }
                }
                if (DecorAllConfigDT.Columns.Contains("Excluzive"))
                    DecorAllConfigDT.Columns.Remove("Excluzive");
                return DecorAllConfigDT;
            }
        }

        public static DataTable DecorConfigDataTable
        {
            get
            {
                if (DecorConfigDT == null)
                {
                    DecorConfigDT = new DataTable();
                    using (
                        SqlDataAdapter DA =
                            new SqlDataAdapter(
                                "SELECT * FROM DecorConfig WHERE Enabled = 1 AND AccountingName IS NOT NULL AND InvNumber IS NOT NULL",
                                ConnectionStrings.CatalogConnectionString))
                    {
                        DA.Fill(DecorConfigDT);
                    }
                }
                if (DecorConfigDT.Columns.Contains("Excluzive"))
                    DecorConfigDT.Columns.Remove("Excluzive");
                return DecorConfigDT;
            }
        }

        public static DataTable TechStoreDataTable
        {
            get
            {
                if (TechStoreDT == null)
                {
                    TechStoreDT = new DataTable();
                    using (
                        SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM TechStore",
                            ConnectionStrings.CatalogConnectionString))
                    {
                        DA.Fill(TechStoreDT);
                    }
                }
                return TechStoreDT;
            }
        }

        public TablesManager()
        {
            UsersDataTable = new DataTable();
            FillUsersDataTable();
            UsersPhotoDataTable = new DataTable();
            FillUsersPhotoDataTable();
            DepartmentsDataTable = new DataTable();
            FillDepartmentsDataTable();
            PositionsDataTable = new DataTable();
            FillPositionsDataTable();
            ModulesDataTable = new DataTable();
            FillModulesDataTable();
        }

        public static void FillUsersDataTable()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Users  WHERE Fired <> 1 ORDER BY Name", ConnectionStrings.UsersConnectionString))
            {
                UsersDataTable.Clear();
                DA.Fill(UsersDataTable);
            }
        }

        public static void FillUsersPhotoDataTable()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM UsersPhoto", ConnectionStrings.UsersConnectionString))
            {
                UsersPhotoDataTable.Clear();
                DA.Fill(UsersPhotoDataTable);
            }
        }

        public static void FillDepartmentsDataTable()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Departments ORDER BY DepartmentName", ConnectionStrings.LightConnectionString))
            {
                DepartmentsDataTable.Clear();
                DA.Fill(DepartmentsDataTable);
            }
        }

        public static void FillPositionsDataTable()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Positions ORDER BY Position", ConnectionStrings.LightConnectionString))
            {
                PositionsDataTable.Clear();
                DA.Fill(PositionsDataTable);
            }
        }

        public static void FillModulesDataTable()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Modules", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(ModulesDataTable);
            }
        }
    }



    public class Security
    {
        private string UsersConnectionString = null;

        public DataTable UsersDataTable = null;
        private DataTable AccessDataTable = null;
        private DataTable CompParamsDataTable = null;

        public BindingSource UsersBindingSource = null;

        public SqlDataAdapter UsersDataAdapter = null;
        public SqlCommandBuilder UsersCommandBuilder = null;

        public static DateTime CurrentEnterDateEnter;

        public static bool IsConding = false;
        public static int CurrentUserID = -1;
        public static string DBFSaveFilePath;
        public static string MuttlementSaveFilePath;
        public static string CurrentUserShortName;
        public static string CurrentUserName;
        public static string CurrentUserRName;
        public static int CurrentModuleID = -1;
        public static DateTime CurrentModuleEnterDateTime;
        public static int CurrentLoginJournalID = -1;

        public string UsersBindingSourceDisplayMember;
        public string UsersBindingSourceValueMember;

        public static bool PriceAccess = false;

        public static Color GridsBackColor = Color.White;

        ManagementObjectSearcher searcher;

        ManagementObjectCollection mObject;

        public Security()
        {
            UsersConnectionString = ConnectionStrings.UsersConnectionString;
        }

        private void Create()
        {
            UsersDataTable = new DataTable();
            AccessDataTable = new DataTable();
            UsersDataTable = new DataTable();

            UsersBindingSource = new BindingSource();

            CompParamsDataTable = new DataTable()
            {
                TableName = "ComputerParams"
            };
            CompParamsDataTable.Columns.Add(new DataColumn("Domain", Type.GetType("System.String")));
            CompParamsDataTable.Columns.Add(new DataColumn("ComputerName", Type.GetType("System.String")));
            CompParamsDataTable.Columns.Add(new DataColumn("LoginName", Type.GetType("System.String")));
            CompParamsDataTable.Columns.Add(new DataColumn("Manufacturer", Type.GetType("System.String")));
            CompParamsDataTable.Columns.Add(new DataColumn("Model", Type.GetType("System.String")));
            CompParamsDataTable.Columns.Add(new DataColumn("ProcessorName", Type.GetType("System.String")));
            CompParamsDataTable.Columns.Add(new DataColumn("ProcessorCores", Type.GetType("System.String")));
            CompParamsDataTable.Columns.Add(new DataColumn("TotalRAM", Type.GetType("System.String")));
            CompParamsDataTable.Columns.Add(new DataColumn("OSName", Type.GetType("System.String")));
            CompParamsDataTable.Columns.Add(new DataColumn("OSVersion", Type.GetType("System.String")));
            CompParamsDataTable.Columns.Add(new DataColumn("OSPlatform", Type.GetType("System.String")));
        }

        private bool Fill()
        {
            string error = string.Empty;
            try
            {
                UsersDataAdapter = new SqlDataAdapter("SELECT UserID, Name, Coding, PriceAccess, Password, AuthorizationCode FROM Users WHERE Fired <> 1 ORDER BY Name", UsersConnectionString);
                UsersDataAdapter.Fill(UsersDataTable);
                UsersCommandBuilder = new SqlCommandBuilder(UsersDataAdapter);
            }
            catch (SqlException ex)
            {
                error = ex.Message;

                return false;
            }

            return true;
        }

        private void Binding()
        {
            UsersBindingSource.DataSource = UsersDataTable;
            UsersBindingSourceDisplayMember = "Name";
            UsersBindingSourceValueMember = "UserID";
        }

        public bool Initialize()
        {
            Create();
            bool bError = Fill();
            Binding();

            return bError;
        }

        public static bool IsAccessGrant(string ModuleButtonName)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ModuleID, SecurityAccess FROM Modules WHERE ModuleButtonName = '" + ModuleButtonName + "'",
                ConnectionStrings.UsersConnectionString))
            {
                using (DataTable ModulesDataTable = new DataTable())
                {
                    DA.Fill(ModulesDataTable);

                    if (Convert.ToBoolean(ModulesDataTable.Rows[0]["SecurityAccess"]) == false)
                        return true;

                    int ModuleID = Convert.ToInt32(ModulesDataTable.Rows[0]["ModuleID"]);

                    using (SqlDataAdapter aDA = new SqlDataAdapter("SELECT * FROM ModulesAccess WHERE UserID = " + Security.CurrentUserID +
                                                                   " AND ModuleID = " + ModuleID, ConnectionStrings.UsersConnectionString))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            return aDA.Fill(DT) > 0;
                        }
                    }

                }
            }


        }

        //public string GetMuttlementSaveFilePath()
        //{
        //    if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium") == false)//not exists
        //    {
        //        return string.Empty;
        //    }

        //    if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/muttlementsave.txt") == true)
        //    {
        //        using (System.IO.StreamReader sr = new System.IO.StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/muttlementsave.txt"))
        //        {
        //            string secondLine = string.Empty;
        //            if (File.ReadLines(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/muttlementsave.txt").Count() > 1)
        //                secondLine = File.ReadLines(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/muttlementsave.txt").Skip(1).First();

        //            return secondLine;
        //        }
        //    }
        //    else
        //        return string.Empty;
        //}

        //public static void SetMuttlementSaveFilePath(string Path)
        //{
        //    if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium") == false)//not exists
        //    {
        //        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium");
        //    }
        //    string[] fileLines = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/muttlementsave.txt");
        //    for (int i = 1; i < fileLines.Count(); i++)
        //    {
        //        fileLines[i] = String.Empty; // deleting
        //        File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/muttlementsave.txt", fileLines);
        //    }
        //    string[] file = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/muttlementsave.txt");
        //    List<string> list = new List<string>(file);
        //    list.RemoveAll(x => x == string.Empty);
        //    File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/muttlementsave.txt", list);

        //    using (System.IO.StreamWriter sr = new System.IO.StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/muttlementsave.txt", true))
        //    {
        //        sr.Write(Path);
        //    }
        //}

        public string GetDBFSaveFilePath()
        {
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium") == false)//not exists
            {
                return string.Empty;
            }

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/DBFsave.txt") == true)
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/DBFsave.txt"))
                {
                    return sr.ReadToEnd();
                }
            }
            else
                return string.Empty;
        }

        public static void SetDBFSaveFilePath(string Path)
        {
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium") == false)//not exists
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium");
            }

            using (System.IO.StreamWriter sr = new System.IO.StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/DBFsave.txt"))
            {
                sr.Write(Path);
            }
        }

        public string GetMuttlementSaveFilePath()
        {
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium") == false)//not exists
            {
                return string.Empty;
            }

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/muttlementsave.txt") == true)
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/muttlementsave.txt"))
                {
                    return sr.ReadToEnd();
                }
            }
            else
                return string.Empty;
        }

        public static void SetMuttlementSaveFilePath(string Path)
        {
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium") == false)//not exists
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium");
            }

            using (System.IO.StreamWriter sr = new System.IO.StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/muttlementsave.txt"))
            {
                sr.Write(Path);
            }
        }

        public int GetCurrentUserLogin()
        {
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium") == false)//not exists
            {
                return -1;
            }

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/currentuser.txt") == true)
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/currentuser.txt"))
                {
                    return (Convert.ToInt32(sr.ReadToEnd()));
                }
            }
            else
                return -1;
        }

        public void SetCurrentUserLogin(int UserID)
        {
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium") == false)//not exists
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium");
            }

            using (System.IO.StreamWriter sr = new System.IO.StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/currentuser.txt"))
            {
                sr.Write(UserID);
            }
        }

        //public int GetCurrentUserLogin()
        //{
        //    if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium") == false)//not exists
        //    {
        //        return -1;
        //    }

        //    if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/currentuser.txt") == true)
        //    {
        //        using (System.IO.StreamReader sr = new System.IO.StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/currentuser.txt"))
        //        {
        //            string firstLine = "0";
        //            if (File.ReadLines(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/currentuser.txt").Count() > 0)
        //                firstLine = File.ReadLines(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/currentuser.txt").First();
        //            return (Convert.ToInt32(firstLine));
        //        }
        //    }
        //    else
        //        return -1;
        //}

        //public void SetCurrentUserLogin(int UserID)
        //{
        //    if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium") == false)//not exists
        //    {
        //        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium");
        //    }

        //    using (System.IO.StreamWriter sr = new System.IO.StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/currentuser.txt"))
        //    {
        //        sr.Write(UserID);
        //    }
        //}

        public void SetForciblyOffline(int UserID, bool b)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, ForciblyOffline FROM Users WHERE UserID = " + UserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        try
                        {
                            if (DA.Fill(DT) == 0)
                                return;
                        }
                        catch
                        {
                            return;
                        }

                        foreach (DataRow Row in DT.Rows)
                        {
                            Row["ForciblyOffline"] = b;
                            //SetOffDateTime(Convert.ToInt32(Row["UserID"]), Security.GetCurrentDate());
                        }

                        try
                        {
                            DA.Update(DT);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        public static void EnterInModule(string FormName)
        {
            if (NotifyForm.bShowed)
                NotifyForm.bCloseNotify = true;

            DataTable ModulesDataTable = new DataTable();

            int ModuleID = GetModuleIDFromName(FormName);

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM ModulesJournal", ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DataRow NewRow = DT.NewRow();
                        NewRow["LoginJournalID"] = CurrentLoginJournalID;
                        NewRow["ModuleID"] = ModuleID;
                        CurrentModuleID = ModuleID;
                        NewRow["UserID"] = CurrentUserID;
                        CurrentModuleEnterDateTime = GetCurrentDate();
                        NewRow["DateEnter"] = CurrentModuleEnterDateTime;
                        DT.Rows.Add(NewRow);

                        DA.Update(DT);
                    }
                }
            }
            ModulesDataTable.Dispose();
        }

        public static void ExitFromModule(string FormName)
        {
            int ModuleID = GetModuleIDFromName(FormName);

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ModulesJournal WHERE UserID = " + CurrentUserID + " AND ModuleID = " + ModuleID + " ORDER BY ModulesJournalID DESC", ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count == 0)
                        {

                            return;
                        }

                        DT.Rows[0]["DateExit"] = GetCurrentDate();

                        DA.Update(DT);
                    }
                }
            }
        }

        public static int GetModuleIDFromName(string FormName)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ModuleID FROM Modules WHERE FormName = '" + FormName + "'", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return Convert.ToInt32(DT.Rows[0]["ModuleID"]);
                }
            }
        }

        private string GetUserName(int UserID)
        {
            DataRow[] Rows = UsersDataTable.Select("UserID = " + UserID);
            return Rows[0]["Name"].ToString();
        }

        private void GetPriceAccess(int UserID)
        {
            DataRow[] Rows = UsersDataTable.Select("UserID = " + UserID);
            PriceAccess = Convert.ToBoolean(Rows[0]["PriceAccess"]);
        }

        private static string GetMD5(string text)
        {
            using (MD5 Hasher = MD5.Create())
            {
                byte[] data = Hasher.ComputeHash(Encoding.Default.GetBytes(text));

                StringBuilder sBuilder = new StringBuilder();

                //преобразование в HEX
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

        public int Enter(int UserID, string Password)
        {
            DataRow[] Rows = UsersDataTable.Select("UserID = " + UserID);
            string passMD5 = GetMD5(Password);

            if (Rows[0]["Password"].ToString() == passMD5)
            {
                if (IsUserOnline(UserID))
                {
                    return -1;
                }

                DBFSaveFilePath = GetDBFSaveFilePath();
                MuttlementSaveFilePath = GetMuttlementSaveFilePath();
                CurrentUserID = UserID;
                CurrentUserShortName = GetUserShortNameByID(UserID);
                CurrentUserName = GetUserName(UserID);
                CurrentUserRName = GetUserName();
                GetPriceAccess(UserID);

                GetStandardFormBackColor();

                return 1;
            }

            return 0;
        }

        public static bool IsUserOnline(int UserID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Online, OnlineRefreshDateTime, GetDate() AS Date FROM USERS WHERE UserID = " + UserID + " AND Online = 1", ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                        {
                            return false;
                        }
                        try
                        {
                            if (Convert.ToDateTime(DT.Rows[0]["OnlineRefreshDateTime"]).AddSeconds(8) < Convert.ToDateTime(DT.Rows[0]["Date"]))
                            {
                                DT.Rows[0]["Online"] = false;

                                DA.Update(DT);

                                return false;
                            }
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show(ex.Message + " \r\nIsUserOnline SqlError в проверке статуса Online");
                            return false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + " \r\nIsUserOnline Ошибка в проверке статуса Online");
                            return false;
                        }

                        return true;
                    }
                }
            }
        }

        public static DateTime GetCurrentDate()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return Convert.ToDateTime(DT.Rows[0][0]);
                }
            }
        }

        //public static DateTime GetCurrentDate(ref double time)
        //{
        //    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //    sw.Start();
        //    double t = 0;
        //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.UsersConnectionString))
        //    {
        //        using (DataTable DT = new DataTable())
        //        {
        //            DA.Fill(DT);

        //            sw.Stop();
        //            t = sw.Elapsed.TotalMilliseconds;
        //            time += t;
        //            return Convert.ToDateTime(DT.Rows[0][0]);
        //        }
        //    }
        //}

        public void CreateJournalRecord()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM LoginJournal", UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        CurrentEnterDateEnter = GetCurrentDate();

                        DataRow Row = DT.NewRow();
                        Row["UserID"] = CurrentUserID;
                        Row["DateEnter"] = CurrentEnterDateEnter;

                        SetCompParams();

                        using (StringWriter SW = new StringWriter())
                        {
                            CompParamsDataTable.WriteXml(SW);
                            Row["ComputerParams"] = SW.ToString();
                        }

                        DT.Rows.Add(Row);


                        DA.Update(DT);
                    }
                }
            }

            if (File.Exists(Application.StartupPath + @"\Infinium_Update(new).exe"))
            {
                File.Delete(Application.StartupPath + @"\Infinium_Update.exe");
                File.Move(Application.StartupPath + @"\Infinium_Update(new).exe", Application.StartupPath + @"\Infinium_Update.exe");
            }

            CurrentLoginJournalID = GetCurrentLoginJournalID(CurrentEnterDateEnter);

            UserOnOffLine(true);
        }

        private int GetCurrentLoginJournalID(DateTime Date)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT LoginJournalID FROM LoginJournal WHERE UserID = " + CurrentUserID + " AND DateEnter = '" + Date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'", UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        return Convert.ToInt32(DT.Rows[0]["LoginJournalID"]);
                    else
                        return -1;
                }
            }
        }

        public void CloseJournalRecord()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM LoginJournal WHERE UserID = " +
                                                          CurrentUserID +
                                                          " AND DateEnter = '" + CurrentEnterDateEnter.ToString("yyyy-MM-dd HH:mm:ss") + "'", UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        try
                        {
                            DA.Fill(DT);
                        }
                        catch
                        {
                            return;
                        }
                        if (DT.Rows.Count == 0)
                        {
                            return;
                        }

                        DT.Rows[0]["DateExit"] = GetCurrentDate();

                        DA.Update(DT);
                    }
                }
            }

            UserOnOffLine(false);
        }

        public void UserOnOffLine(bool bOnline)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Online FROM Users WHERE UserID = " + CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DT.Rows[0]["Online"] = bOnline;

                        DA.Update(DT);
                    }
                }
            }
        }

        public bool CheckPass(string Pass)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Password FROM Users WHERE UserID = " + CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows[0]["Password"].ToString() == GetMD5(Pass))
                            return true;
                    }
                }
            }

            return false;
        }

        public void CheckConding(int UserID)
        {
            using (
                SqlDataAdapter DA =
                    new SqlDataAdapter("SELECT UserID, Conding FROM Users WHERE UserID = " + UserID,
                        ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && Convert.ToBoolean(DT.Rows[0]["Conding"]))
                        IsConding = true;
                }
            }
        }

        public static bool CheckAuth(string Pass)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, AuthorizationCode FROM Users WHERE UserID = " + CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (Pass.Length == 0 && DT.Rows[0]["AuthorizationCode"] == DBNull.Value)
                            return true;

                        if (DT.Rows[0]["AuthorizationCode"].ToString() == GetMD5(Pass))
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool CheckAuthNull()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, AuthorizationCode FROM Users WHERE UserID = " + CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows[0]["AuthorizationCode"] == DBNull.Value)
                            return true;
                    }
                }
            }

            return false;
        }

        public void ChangePassword(string OldPass, string NewPass)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Password FROM Users WHERE UserID = " + CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DT.Rows[0]["Password"] = GetMD5(NewPass);

                        DA.Update(DT);
                    }
                }
            }
        }

        public void ChangeAuth(string NewPass)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, AuthorizationCode FROM Users WHERE UserID = " + CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DT.Rows[0]["AuthorizationCode"] = GetMD5(NewPass);

                        DA.Update(DT);
                    }
                }
            }
        }

        public string GetUserName()
        {
            string OriginalUserName = UsersDataTable.Select("UserID = " + CurrentUserID)[0]["Name"].ToString();
            string ResultUserName = "";

            for (int i = 0; i < OriginalUserName.Length; i++)
            {
                if (OriginalUserName[i] == ' ')
                {
                    ResultUserName = OriginalUserName.Substring(0, i) + "\r\n" + OriginalUserName.Substring(i + 1, (OriginalUserName.Length - (i + 1)));
                    break;
                }
            }

            return ResultUserName;
        }

        public static string GetUserNameByID(int UserID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name FROM Users WHERE UserID = " + UserID, ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return DT.Rows[0]["Name"].ToString();
                }
            }
        }

        public static string GetUserShortNameByID(int UserID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, ShortName FROM Users WHERE UserID = " + UserID, ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return DT.Rows[0]["ShortName"].ToString();
                }
            }
        }

        public static Color GetStandardFormBackColor()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, GridsBackColor FROM USERS WHERE UserID = " + Security.CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    GridsBackColor = GetColorFromString(DT.Rows[0]["GridsBackColor"].ToString());

                    return GridsBackColor;
                }
            }
        }

        private static Color GetColorFromString(string cColor)
        {
            string temp = "";

            int R = 255;
            int G = 255;
            int B = 255;

            int c = 0;

            for (int i = 0; i < cColor.Length; i++)
            {
                if (i == cColor.Length - 1)
                {
                    temp += cColor[i];
                    B = Convert.ToInt32(temp);
                    break;
                }

                if (cColor[i] != ';')
                    temp += cColor[i];
                else
                {
                    if (c == 0)
                        R = Convert.ToInt32(temp);
                    if (c == 1)
                        G = Convert.ToInt32(temp);

                    c++;
                    temp = "";
                }
            }

            return Color.FromArgb(R, G, B);
        }

        public static void SetStandardFormBackColor(Color Color)
        {
            GridsBackColor = Color;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, GridsBackColor FROM USERS WHERE UserID = " + Security.CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DT.Rows[0]["GridsBackColor"] = Color.R.ToString() + ";" + Color.G.ToString() + ";" + Color.B.ToString();

                        DA.Update(DT);
                    }
                }
            }
        }

        public void SetCompParams()
        {
            if (searcher != null)
                searcher.Dispose();

            searcher = new ManagementObjectSearcher("root\\CIMV2",
                "Select domain, Name, UserName, Manufacturer, Model, TotalPhysicalMemory FROM Win32_ComputerSystem");

            DataRow NewRow = CompParamsDataTable.NewRow();
            NewRow["Domain"] = GetParam("Domain");
            NewRow["ComputerName"] = GetParam("Name");
            NewRow["LoginName"] = GetParam("UserName");
            NewRow["Manufacturer"] = GetParam("Manufacturer");
            NewRow["Model"] = GetParam("Model");
            NewRow["TotalRAM"] = Convert.ToInt64(GetParam("TotalPhysicalMemory")) / 1024 / 1024;

            searcher.Dispose();
            searcher = new ManagementObjectSearcher("root\\CIMV2",
                "Select Name, NumberOfCores, CurrentClockSpeed FROM WIN32_Processor");

            NewRow["ProcessorName"] = GetParam("Name");
            NewRow["ProcessorCores"] = GetParam("NumberOfCores") + " x " + GetParam("CurrentClockSpeed") + " MHz";

            Microsoft.VisualBasic.Devices.ComputerInfo CI = new Microsoft.VisualBasic.Devices.ComputerInfo();
            NewRow["OSName"] = CI.OSFullName;
            NewRow["OSPlatform"] = CI.OSPlatform;
            NewRow["OSVersion"] = CI.OSVersion;

            CompParamsDataTable.Rows.Add(NewRow);
        }

        private string GetParam(string param)
        {
            String processorID = "";
            try
            {

                mObject = searcher.Get();

                if (mObject.Count == 0)
                    return "-";

                foreach (ManagementObject obj in mObject)
                {
                    processorID = obj[param].ToString();
                }

            }
            catch
            {
                return "-";
            }

            return processorID;
        }

        public static bool IsConnect()
        {
            SqlConnection Connection = new SqlConnection(ConnectionStrings.LightConnectionString);

            try
            {
                Connection.Open();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }





    public class OnLineControl
    {
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }


        public OnLineControl()
        {


        }
        public static void KillProcess()
        {
            string name = "Infinium";
            System.Diagnostics.Process[] CurrentProcesses = System.Diagnostics.Process.GetProcesses();
            ArrayList InfiniumProcesses = new ArrayList();
            foreach (System.Diagnostics.Process item in CurrentProcesses)
                if (item.ProcessName.ToLower().Contains(name.ToLower()))
                {
                    InfiniumProcesses.Add(item);
                }
            if (InfiniumProcesses.Count > 1)
            {
                DateTime StartTime = ((System.Diagnostics.Process)InfiniumProcesses[0]).StartTime;
                if (StartTime > ((System.Diagnostics.Process)InfiniumProcesses[1]).StartTime)
                    ((System.Diagnostics.Process)InfiniumProcesses[1]).Kill();
                else
                    ((System.Diagnostics.Process)InfiniumProcesses[0]).Kill();
            }
        }

        public static void IamOnline(int ModuleID, bool TopMost)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Online, OnlineRefreshDateTime, TopModule, IdleTime, TopMost, GetDate() AS DateTime FROM Users WHERE ForciblyOffline=0 AND UserID = " + Security.CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                        }
                        else
                        {
                            SetForciblyOffline(false);
                            SetOffline();
                        }

                        string s = "";

                        DT.Rows[0]["Online"] = true;
                        DT.Rows[0]["OnlineRefreshDateTime"] = DT.Rows[0]["DateTime"];
                        DT.Rows[0]["TopModule"] = ModuleID;
                        DT.Rows[0]["IdleTime"] = OnLineControl.GetIdleTime(ref s);
                        DT.Rows[0]["TopMost"] = TopMost;

                        try
                        {
                            DA.Update(DT);
                        }
                        catch
                        {

                        }
                    }
                }
            }

            //SetDateExitToNULL(Security.CurrentUserID);
        }

        public static void SetOffline()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Online FROM Users WHERE Online = 'TRUE' AND ((DATEADD(second, 8, OnlineRefreshDateTime) < GetDate())) OR OnlineRefreshDateTime IS NULL", ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        try
                        {
                            if (DA.Fill(DT) == 0)
                                return;
                        }
                        catch
                        {
                            return;
                        }

                        foreach (DataRow Row in DT.Rows)
                        {
                            Row["Online"] = false;
                            //SetOffDateTime(Convert.ToInt32(Row["UserID"]), Security.GetCurrentDate());
                        }

                        try
                        {
                            DA.Update(DT);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        public static void SetForciblyOffline(bool b)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, ForciblyOffline FROM Users WHERE UserID = " + Security.CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        try
                        {
                            if (DA.Fill(DT) == 0)
                                return;
                        }
                        catch
                        {
                            return;
                        }

                        foreach (DataRow Row in DT.Rows)
                        {
                            Row["ForciblyOffline"] = b;
                            //SetOffDateTime(Convert.ToInt32(Row["UserID"]), Security.GetCurrentDate());
                        }

                        try
                        {
                            DA.Update(DT);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        public static void SetOfflineClient()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, Online FROM Clients WHERE Online = 'TRUE' AND ((DATEADD(second, 20, OnlineRefreshDateTime) < GetDate())) OR OnlineRefreshDateTime IS NULL", ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        try
                        {
                            if (DA.Fill(DT) == 0)
                                return;
                        }
                        catch
                        {
                            return;
                        }

                        foreach (DataRow Row in DT.Rows)
                        {
                            Row["Online"] = false;
                        }

                        try
                        {
                            DA.Update(DT);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        public static void SetOfflineManager()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ManagerID, Online FROM Managers WHERE Online = 'TRUE' AND ((DATEADD(second, 20, OnlineRefreshDateTime) < GetDate())) OR OnlineRefreshDateTime IS NULL", ConnectionStrings.ZOVReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        try
                        {
                            if (DA.Fill(DT) == 0)
                                return;
                        }
                        catch
                        {
                            return;
                        }

                        foreach (DataRow Row in DT.Rows)
                        {
                            Row["Online"] = false;
                        }

                        try
                        {
                            DA.Update(DT);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }


        public static int GetIdleTime(ref string LastInputTime)
        {
            int systemUptime = Environment.TickCount;
            int LastInputTicks = 0;
            int IdleTicks = 0;

            LASTINPUTINFO LastInputInfo = new LASTINPUTINFO();
            LastInputInfo.cbSize = (uint)Marshal.SizeOf(LastInputInfo);
            LastInputInfo.dwTime = 0;

            if (GetLastInputInfo(ref LastInputInfo))
            {
                LastInputTicks = (int)LastInputInfo.dwTime;
                IdleTicks = systemUptime - LastInputTicks;
            }

            LastInputTime = (DateTime.Now - TimeSpan.FromMilliseconds(IdleTicks)).ToString();

            return IdleTicks / 1000;

            //label3.Text = "At second " + Convert.ToString(LastInputTicks / 1000);

            //label3.Text = "At " + new DateTime((DateTime.Now.Ticks - LastInputTicks)).ToString();

            //label2.Text = DateTime.Now.Ticks
        }

        //public static void SetOffDateTime(int UserID, object ExitDateTime)
        //{
        //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 LoginJournalID, UserID, DateExit FROM LoginJournal WHERE DateExit IS NULL ORDER BY LoginJournalID DESC", ConnectionStrings.UsersConnectionString))
        //    {
        //        using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
        //        {
        //            using (DataTable DT = new DataTable())
        //            {
        //                if (DA.Fill(DT) == 0)
        //                    return;

        //                DT.Rows[0]["DateExit"] = ExitDateTime;

        //                DA.Update(DT);
        //            }
        //        }
        //    }
        //}

        //public static void SetDateExitToNULL(int UserID)
        //{
        //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 LoginJournalID, UserID, DateExit FROM LoginJournal WHERE DateEnter = '"+
        //                                                    Security.CurrentEnterDateEnter.ToString("yyyy-MM-dd HH:mm:ss") + ".000" + 
        //                                                    "' AND DateExit IS NOT NULL ORDER BY LoginJournalID DESC", ConnectionStrings.UsersConnectionString))
        //    {                                                                                                                                                                                                                           
        //        using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
        //        {
        //            using (DataTable DT = new DataTable())
        //            {
        //                if (DA.Fill(DT) == 0)
        //                    return;

        //                DT.Rows[0]["DateExit"] = DBNull.Value;

        //                DA.Update(DT);
        //            }
        //        }
        //    }
        //}
    }





    public class DatabaseConfigsManager
    {
        public static bool Animation;

        private string sConnectionString = null;

        public DatabaseConfigsManager()
        {

        }

        public void ReadAnimationFlag(String FileName)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(FileName))
            {
                Animation = Convert.ToBoolean(sr.ReadToEnd());
            }
        }

        public string ReadConfig(String FileName)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(FileName))
            {
                sConnectionString = sr.ReadToEnd();
            }

            return sConnectionString;
        }

        public string ReadConfig(String FileName, int BytesToRead, int StartByte)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(FileName))
            {
                string s = sr.ReadToEnd();

                sConnectionString = s.Substring(StartByte, BytesToRead);
            }

            return sConnectionString;
        }
    }





    public class UserProfile
    {
        DataTable dtRolePermissions;
        DataTable UsersPositionsDT;
        public DataTable UsersDataTable;
        public BindingSource UsersBindingSource;
        public DataTable PositionsDataTable;
        public BindingSource PositionsBindingSource;

        public struct Contacts
        {
            public string PersonalMobilePhone;
            public string WorkMobilePhone;
            public string WorkStatPhone;
            public string WorkExtPhone;
            public string Skype;
            public string Email;
            public string ICQ;
            public bool NeedSpam;
        }

        public struct PersonalInform
        {
            public string BirthDate;
            //public int PositionID;
            public DataTable ProfilPositionsDT;
            public DataTable TPSPositionsDT;
            public int DepartmentID;
            public string Education;
            public string EducationPlace;
            public string Language;
            public bool DriveA;
            public bool DriveB;
            public bool DriveC;
            public bool DriveD;
            public bool DriveE;
            public string CombatArm;
            public string MilitaryRank;
        }

        public struct InfiniumSettings
        {
            public int InfinumBackColorIndex;
            public int InfiniumTilesStyleIndex;
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
        public UserProfile()
        {
            dtRolePermissions = new DataTable();
            UsersDataTable = new DataTable();
            UsersBindingSource = new BindingSource();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name FROM Users  WHERE Fired <> 1 ORDER BY Name", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDataTable);
            }

            UsersBindingSource.DataSource = UsersDataTable;

            PositionsDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Positions ORDER BY Position", ConnectionStrings.LightConnectionString))
            {
                DA.Fill(PositionsDataTable);
            }

            UsersPositionsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT DISTINCT UserID, FactoryID, DepartmentID, PositionID, Rate FROM StaffList", ConnectionStrings.LightConnectionString))
            {
                DA.Fill(UsersPositionsDT);
            }

            PositionsBindingSource = new BindingSource()
            {
                DataSource = PositionsDataTable
            };
        }

        public void SaveUsersPhotoToFile()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Users", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return;

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        int userId = Convert.ToInt32(DT.Rows[i]["UserID"]);
                        if (DT.Rows[i]["Photo"] == DBNull.Value)
                            continue;
                        byte[] b = (byte[])DT.Rows[i]["Photo"];
                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            PictureBox pb = new PictureBox { Image = Image.FromStream(ms) };
                            SetUserPhoto(pb.Image, userId);
                            pb.Image.Save(@"D:\UserPhoto\" + userId + ".jpg", ImageFormat.Jpeg);
                        }
                    }
                }
            }
        }

        public int GetUserByName(string sName)
        {
            return Convert.ToInt32(UsersDataTable.Select("Name = '" + sName + "'")[0]["UserID"]);
        }

        public string PhoneFormatToNumberFormat(string PhoneFormatString)
        {
            string result = "";

            foreach (char c in PhoneFormatString)
            {
                if (c != ' ')
                    result += c;
            }

            return result;
        }

        public void SetContacts(Contacts Contacts)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, AccessToken, PersonalMobilePhone, WorkMobilePhone, WorkStatPhone, WorkExtPhone, Skype, Email, ICQ, NeedSpam FROM Users WHERE UserID = " + Security.CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DT.Rows[0]["PersonalMobilePhone"] = Contacts.PersonalMobilePhone;
                        DT.Rows[0]["WorkMobilePhone"] = Contacts.WorkMobilePhone;
                        DT.Rows[0]["WorkStatPhone"] = Contacts.WorkStatPhone;
                        DT.Rows[0]["WorkExtPhone"] = Contacts.WorkExtPhone;
                        DT.Rows[0]["Skype"] = Contacts.Skype;
                        DT.Rows[0]["Email"] = Contacts.Email;
                        DT.Rows[0]["ICQ"] = Contacts.ICQ;
                        DT.Rows[0]["NeedSpam"] = Contacts.NeedSpam;
                        DT.Rows[0]["AccessToken"] = GenAccessToken(Security.CurrentUserID);

                        DA.Update(DT);
                    }
                }
            }
        }

        public Contacts GetContacts()
        {
            Contacts Contacts = new Contacts();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, PersonalMobilePhone, WorkMobilePhone, WorkStatPhone, WorkExtPhone, Skype, Email, ICQ, NeedSpam FROM Users WHERE UserID = " + Security.CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        Contacts.PersonalMobilePhone = DT.Rows[0]["PersonalMobilePhone"].ToString();
                        Contacts.WorkMobilePhone = DT.Rows[0]["WorkMobilePhone"].ToString();
                        Contacts.WorkStatPhone = DT.Rows[0]["WorkStatPhone"].ToString();
                        Contacts.WorkExtPhone = DT.Rows[0]["WorkExtPhone"].ToString();
                        Contacts.Skype = DT.Rows[0]["Skype"].ToString();
                        Contacts.Email = DT.Rows[0]["Email"].ToString();
                        Contacts.ICQ = DT.Rows[0]["ICQ"].ToString();
                        Contacts.NeedSpam = Convert.ToBoolean(DT.Rows[0]["NeedSpam"]);

                        return Contacts;
                    }
                }
            }
        }

        private string GetMD5(string text)
        {
            using (System.Security.Cryptography.MD5 Hasher = System.Security.Cryptography.MD5.Create())
            {
                byte[] data = Hasher.ComputeHash(Encoding.Default.GetBytes(text));

                StringBuilder sBuilder = new StringBuilder();

                //преобразование в HEX
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

        public string GenAccessToken(int UserID)
        {
            string AccessToken = string.Empty;
            string Email = GetEmail(UserID);
            if (Email.Length > 0)
            {
                Guid g = Guid.NewGuid();
                string GuidString = Convert.ToBase64String(g.ToByteArray());
                AccessToken = GetMD5(Email) + GetMD5(GuidString);
            }
            return AccessToken;
        }

        private string GetEmail(int UserID)
        {
            string Email = string.Empty;
            DataRow[] Rows = UsersDataTable.Select("UserID = " + UserID);
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Email FROM Users WHERE UserID=" + UserID,
                ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0 && DT.Rows[0]["Email"] != DBNull.Value)
                        Email = DT.Rows[0]["Email"].ToString();
                }
            }

            return Email;
        }

        public static string GetDepartmentName(int DepartmentID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DepartmentName FROM Departments WHERE DepartmentID = " + DepartmentID, ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        return DT.Rows[0]["DepartmentName"].ToString();
                    return string.Empty;
                }
            }
        }

        public DataTable GetUsersPositions(int FactoryID, int UserID)
        {
            DataTable dt = new DataTable();
            using (DataView DV = new DataView(UsersPositionsDT, "UserID=" + UserID + " AND FactoryID=" + FactoryID, string.Empty, DataViewRowState.CurrentRows))
            {
                dt = DV.ToTable(true, new string[] { "DepartmentID", "PositionID", "Rate" });
            }
            return dt;
        }

        public PersonalInform GetPersonalInform()
        {
            PersonalInform PersonalInform = new PersonalInform();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, BirthDate, PositionID, DepartmentID, Education, EducationPlace, Language, DriveA, DriveB, DriveC, DriveD, DriveE, CombatArm, MilitaryRank FROM Users WHERE UserID = " + Security.CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows[0]["BirthDate"].ToString().Length > 0)
                            PersonalInform.BirthDate = Convert.ToDateTime(DT.Rows[0]["BirthDate"]).ToString("dd.MM.yyyy");
                        else
                            PersonalInform.BirthDate = "";
                        //PersonalInform.PositionID = Convert.ToInt32(DT.Rows[0]["PositionID"]);
                        PersonalInform.ProfilPositionsDT = GetUsersPositions(1, Security.CurrentUserID);
                        PersonalInform.TPSPositionsDT = GetUsersPositions(2, Security.CurrentUserID);
                        PersonalInform.DepartmentID = Convert.ToInt32(DT.Rows[0]["DepartmentID"]);
                        PersonalInform.Education = DT.Rows[0]["Education"].ToString();
                        PersonalInform.EducationPlace = DT.Rows[0]["EducationPlace"].ToString();
                        PersonalInform.Language = DT.Rows[0]["Language"].ToString();
                        PersonalInform.DriveA = Convert.ToBoolean(DT.Rows[0]["DriveA"]);
                        PersonalInform.DriveB = Convert.ToBoolean(DT.Rows[0]["DriveB"]);
                        PersonalInform.DriveC = Convert.ToBoolean(DT.Rows[0]["DriveC"]);
                        PersonalInform.DriveD = Convert.ToBoolean(DT.Rows[0]["DriveD"]);
                        PersonalInform.DriveE = Convert.ToBoolean(DT.Rows[0]["DriveE"]);
                        PersonalInform.CombatArm = DT.Rows[0]["CombatArm"].ToString();
                        PersonalInform.MilitaryRank = DT.Rows[0]["MilitaryRank"].ToString();

                        return PersonalInform;
                    }
                }
            }
        }

        public InfiniumSettings GetInfiniumSettings()
        {
            InfiniumSettings InfiniumSettings = new InfiniumSettings();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, InfinumBackColorIndex, InfiniumTilesStyleIndex FROM Users WHERE UserID = " + Security.CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        InfiniumSettings.InfinumBackColorIndex = Convert.ToInt32(DT.Rows[0]["InfinumBackColorIndex"]);
                        InfiniumSettings.InfiniumTilesStyleIndex = Convert.ToInt32(DT.Rows[0]["InfiniumTilesStyleIndex"]);

                        return InfiniumSettings;
                    }
                }
            }
        }

        public void SetInfiniumSettings(InfiniumSettings InfiniumSettings)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, InfinumBackColorIndex, InfiniumTilesStyleIndex FROM Users WHERE UserID = " + Security.CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DT.Rows[0]["InfinumBackColorIndex"] = InfiniumSettings.InfinumBackColorIndex;
                        DT.Rows[0]["InfiniumTilesStyleIndex"] = InfiniumSettings.InfiniumTilesStyleIndex;

                        DA.Update(DT);
                    }
                }
            }
        }

        public void SetPersonalInform(PersonalInform PersonalInform)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, BirthDate, PositionID, Education, EducationPlace, Language, DriveA, DriveB, DriveC, DriveD, DriveE, CombatArm, MilitaryRank FROM Users WHERE UserID = " + Security.CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (PersonalInform.BirthDate != null)
                            DT.Rows[0]["BirthDate"] = PersonalInform.BirthDate;
                        else
                            DT.Rows[0]["BirthDate"] = DBNull.Value;

                        //DT.Rows[0]["PositionID"] = PersonalInform.PositionID;

                        if (PersonalInform.Education.Length > 0)
                            DT.Rows[0]["Education"] = PersonalInform.Education;
                        else
                            DT.Rows[0]["Education"] = DBNull.Value;

                        if (PersonalInform.EducationPlace.Length > 0)
                            DT.Rows[0]["EducationPlace"] = PersonalInform.EducationPlace;
                        else
                            DT.Rows[0]["EducationPlace"] = DBNull.Value;

                        DT.Rows[0]["Language"] = PersonalInform.Language;
                        DT.Rows[0]["DriveA"] = PersonalInform.DriveA;
                        DT.Rows[0]["DriveB"] = PersonalInform.DriveB;
                        DT.Rows[0]["DriveC"] = PersonalInform.DriveC;
                        DT.Rows[0]["DriveD"] = PersonalInform.DriveD;
                        DT.Rows[0]["DriveE"] = PersonalInform.DriveE;
                        DT.Rows[0]["CombatArm"] = PersonalInform.CombatArm;
                        DT.Rows[0]["MilitaryRank"] = PersonalInform.MilitaryRank;

                        DA.Update(DT);
                    }
                }
            }
        }




        FileManager FM = new FileManager();
        //for usersprofiles only

        public Contacts GetContacts(int UserID)
        {
            Contacts Contacts = new Contacts();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, PersonalMobilePhone, WorkMobilePhone, WorkStatPhone, WorkExtPhone, Skype, Email, ICQ, NeedSpam FROM Users WHERE UserID = " + UserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        Contacts.PersonalMobilePhone = DT.Rows[0]["PersonalMobilePhone"].ToString();
                        Contacts.WorkMobilePhone = DT.Rows[0]["WorkMobilePhone"].ToString();
                        Contacts.WorkStatPhone = DT.Rows[0]["WorkStatPhone"].ToString();
                        Contacts.WorkExtPhone = DT.Rows[0]["WorkExtPhone"].ToString();
                        Contacts.Skype = DT.Rows[0]["Skype"].ToString();
                        Contacts.Email = DT.Rows[0]["Email"].ToString();
                        Contacts.ICQ = DT.Rows[0]["ICQ"].ToString();
                        Contacts.NeedSpam = Convert.ToBoolean(DT.Rows[0]["NeedSpam"]);

                        return Contacts;
                    }
                }
            }
        }

        public PersonalInform GetPersonalInform(int UserID)
        {
            PersonalInform PersonalInform = new PersonalInform();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, BirthDate, PositionID, DepartmentID, Education, EducationPlace, Language, DriveA, DriveB, DriveC, DriveD, DriveE, CombatArm, MilitaryRank FROM Users WHERE UserID = " + UserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows[0]["BirthDate"].ToString().Length > 0)
                            PersonalInform.BirthDate = Convert.ToDateTime(DT.Rows[0]["BirthDate"]).ToString("dd.MM.yyyy");
                        else
                            PersonalInform.BirthDate = "";
                        //PersonalInform.PositionID = Convert.ToInt32(DT.Rows[0]["PositionID"]);
                        PersonalInform.ProfilPositionsDT = GetUsersPositions(1, UserID);
                        PersonalInform.TPSPositionsDT = GetUsersPositions(2, UserID);
                        PersonalInform.DepartmentID = Convert.ToInt32(DT.Rows[0]["DepartmentID"]);
                        PersonalInform.Education = DT.Rows[0]["Education"].ToString();
                        PersonalInform.EducationPlace = DT.Rows[0]["EducationPlace"].ToString();
                        PersonalInform.Language = DT.Rows[0]["Language"].ToString();
                        PersonalInform.DriveA = Convert.ToBoolean(DT.Rows[0]["DriveA"]);
                        PersonalInform.DriveB = Convert.ToBoolean(DT.Rows[0]["DriveB"]);
                        PersonalInform.DriveC = Convert.ToBoolean(DT.Rows[0]["DriveC"]);
                        PersonalInform.DriveD = Convert.ToBoolean(DT.Rows[0]["DriveD"]);
                        PersonalInform.DriveE = Convert.ToBoolean(DT.Rows[0]["DriveE"]);
                        PersonalInform.CombatArm = DT.Rows[0]["CombatArm"].ToString();
                        PersonalInform.MilitaryRank = DT.Rows[0]["MilitaryRank"].ToString();

                        return PersonalInform;
                    }
                }
            }
        }

        public void SetPersonalInform(int UserID, PersonalInform PersonalInform)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, BirthDate, PositionID, Education, EducationPlace, Language, DriveA, DriveB, DriveC, DriveD, DriveE, CombatArm, MilitaryRank FROM Users WHERE UserID = " + UserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (PersonalInform.BirthDate != null)
                            DT.Rows[0]["BirthDate"] = PersonalInform.BirthDate;
                        else
                            DT.Rows[0]["BirthDate"] = DBNull.Value;

                        //DT.Rows[0]["PositionID"] = PersonalInform.PositionID;

                        if (PersonalInform.Education.Length > 0)
                            DT.Rows[0]["Education"] = PersonalInform.Education;
                        else
                            DT.Rows[0]["Education"] = DBNull.Value;

                        if (PersonalInform.EducationPlace.Length > 0)
                            DT.Rows[0]["EducationPlace"] = PersonalInform.EducationPlace;
                        else
                            DT.Rows[0]["EducationPlace"] = DBNull.Value;

                        DT.Rows[0]["Language"] = PersonalInform.Language;
                        DT.Rows[0]["DriveA"] = PersonalInform.DriveA;
                        DT.Rows[0]["DriveB"] = PersonalInform.DriveB;
                        DT.Rows[0]["DriveC"] = PersonalInform.DriveC;
                        DT.Rows[0]["DriveD"] = PersonalInform.DriveD;
                        DT.Rows[0]["DriveE"] = PersonalInform.DriveE;
                        DT.Rows[0]["CombatArm"] = PersonalInform.CombatArm;
                        DT.Rows[0]["MilitaryRank"] = PersonalInform.MilitaryRank;

                        DA.Update(DT);
                    }
                }
            }
        }

        public void dd()
        {

            using (SqlDataAdapter da = new SqlDataAdapter("SELECT UserID, AccessToken FROM Users  WHERE (Email IS NOT NULL) AND ({ fn LENGTH(Email) } > 0) AND Fired <> 1 ORDER BY Name", ConnectionStrings.UsersConnectionString))
            {
                using (new SqlCommandBuilder(da))
                {
                    using (DataTable dt = new DataTable())
                    {
                        da.Fill(dt);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            int userId = Convert.ToInt32(dt.Rows[i]["UserID"]);
                            dt.Rows[i]["AccessToken"] = GenAccessToken(userId);
                        }
                        da.Update(dt);
                    }
                }
            }
        }

        public void SetContacts(int UserID, Contacts Contacts)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, AccessToken, PersonalMobilePhone, WorkMobilePhone, WorkStatPhone, WorkExtPhone, Skype, Email, ICQ, NeedSpam FROM Users WHERE UserID = " + UserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DT.Rows[0]["PersonalMobilePhone"] = Contacts.PersonalMobilePhone;
                        DT.Rows[0]["WorkMobilePhone"] = Contacts.WorkMobilePhone;
                        DT.Rows[0]["WorkStatPhone"] = Contacts.WorkStatPhone;
                        DT.Rows[0]["WorkExtPhone"] = Contacts.WorkExtPhone;
                        DT.Rows[0]["Skype"] = Contacts.Skype;
                        DT.Rows[0]["Email"] = Contacts.Email;
                        DT.Rows[0]["ICQ"] = Contacts.ICQ;
                        DT.Rows[0]["NeedSpam"] = Contacts.NeedSpam;
                        DT.Rows[0]["AccessToken"] = GenAccessToken(UserID);

                        DA.Update(DT);
                    }
                }
            }
        }



        public string GetUserName(int UserID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name FROM Users WHERE UserID = " + UserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        return DT.Rows[0]["Name"].ToString();
                    }
                }
            }
        }

        public Image GetUserPhoto(int UserID)
        {
            DataRow[] Rows = TablesManager.UsersPhotoDataTable.Select("UserID = " + UserID);
            if (!Rows.Any())
                return null;
            if (Rows[0]["Photo"] == DBNull.Value)
                return null;
            if (FM.FileExist(Configs.DocumentsZOVTPSPath + FileManager.GetPath("UsersPhoto") + "/" + Rows[0]["Photo"], Configs.FTPType))
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(
                        FM.ReadFile(Configs.DocumentsZOVTPSPath + FileManager.GetPath("UsersPhoto") + "/" + Rows[0]["Photo"],
                        Convert.ToInt64(Rows[0]["FileSize"]), Configs.FTPType)))
                    {
                        return Image.FromStream(ms);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Или здесь " + ex.Message);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void SetUserPhoto(Image Photo, int UserID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM UsersPhoto WHERE UserID = " + UserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                        string sDestFolder = Configs.DocumentsZOVTPSPath + FileManager.GetPath("UsersPhoto");
                        if (DA.Fill(DT) > 0)
                        {
                            if (Photo != null)
                            {
                                string sFileName = UserID + ".jpg";

                                Int64 FileSize = 0;
                                using (var ms = new MemoryStream())
                                {
                                    Photo.Save(ms, ImageFormat.Jpeg);
                                    FileSize = ms.Length;
                                }
                                Photo.Save(tempFolder + "/" + sFileName, ImageFormat.Jpeg);
                                FM.UploadFile(tempFolder + "/" + sFileName,
                                    sDestFolder + "/" + sFileName, Configs.FTPType);
                                DT.Rows[0]["FileSize"] = FileSize;
                            }
                            else
                                DT.Rows[0]["Photo"] = DBNull.Value;
                        }
                        else
                        {
                            Int64 FileSize = 0;
                            using (var ms = new MemoryStream())
                            {
                                Photo.Save(ms, ImageFormat.Jpeg);
                                FileSize = ms.Length;
                            }

                            string sFileName = UserID + ".jpg";
                            Photo.Save(tempFolder + "/" + sFileName, ImageFormat.Jpeg);
                            FM.UploadFile(tempFolder + "/" + sFileName,
                                sDestFolder + "/" + sFileName, Configs.FTPType);
                            DataRow NewRow = DT.NewRow();
                            NewRow["UserID"] = UserID;
                            NewRow["Photo"] = UserID + ".jpg";
                            NewRow["FileSize"] = FileSize;
                            DT.Rows.Add(NewRow);
                        }

                        DA.Update(DT);
                    }
                }
            }

            TablesManager.FillUsersPhotoDataTable();
        }
        /////////////////////////////////////////////////////////////////////////////////////

        public bool IsUserClientManager(int UserID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsManagers WHERE UserID = " + UserID,
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    return DA.Fill(DT) > 0;
                }
            }
        }

        public void DeleteClientManager(int UserID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsManagers WHERE UserID = " + UserID,
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0].Delete();
                            DA.Update(DT);
                        }
                    }
                }
            }
        }
        public void SaveClientManager(int UserID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM ClientsManagers WHERE UserID=" + UserID,
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (DT.Rows.Count == 0)
                        {
                            string Name = GetUserName(UserID);
                            string fullName = Name;
                            string firstName = Name;
                            string lastName = string.Empty;
                            string middleName = string.Empty;

                            var names = fullName.Split(' ');
                            if (names.Count() == 1)
                                lastName = names[0];
                            if (names.Count() == 2)
                            {
                                lastName = names[0];
                                firstName = names[1];
                            }
                            if (names.Count() == 3)
                            {
                                lastName = names[0];
                                firstName = names[1];
                                middleName = names[2];
                            }

                            string ShortName = Name;
                            if (Name.IndexOf(' ') > -1)
                            {
                                if (lastName.Length > 0)
                                    ShortName = lastName;
                                if (firstName.Length > 0)
                                    ShortName += " " + firstName.Substring(0, 1) + ".";
                                if (middleName.Length > 0)
                                    ShortName += middleName.Substring(0, 1) + ".";
                            }
                            DataRow NewRow = DT.NewRow();
                            NewRow["Name"] = lastName + " " + firstName;
                            NewRow["ShortName"] = ShortName;
                            NewRow["UserID"] = UserID;
                            NewRow["OnlineRefreshDateTime"] = DateTime.Now;
                            NewRow["Password"] = "4ba29b9f9e5732ed33761840f4ba6c53";
                            DT.Rows.Add(NewRow);

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public string GetUserName()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name FROM Users WHERE UserID = " + Security.CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        return DT.Rows[0]["Name"].ToString();
                    }
                }
            }
        }

        public static Image GetUserPhoto()
        {
            DataRow[] Rows = TablesManager.UsersPhotoDataTable.Select("UserID = " + Security.CurrentUserID);
            if (!Rows.Any())
                return null;
            if (Rows[0]["Photo"] == DBNull.Value)
                return null;
            FileManager FM1 = new FileManager();
            if (FM1.FileExist(Configs.DocumentsZOVTPSPath + FileManager.GetPath("UsersPhoto") + "/" + Rows[0]["Photo"], Configs.FTPType))
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(
                        FM1.ReadFile(Configs.DocumentsZOVTPSPath + FileManager.GetPath("UsersPhoto") + "/" + Rows[0]["Photo"],
                        Convert.ToInt64(Rows[0]["FileSize"]), Configs.FTPType)))
                    {
                        return Image.FromStream(ms);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка здесь " + ex.Message);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public void SetUserPhoto(Image Photo)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM UsersPhoto WHERE UserID = " + Security.CurrentUserID, ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
                        string sDestFolder = Configs.DocumentsZOVTPSPath + FileManager.GetPath("UsersPhoto");
                        if (DA.Fill(DT) > 0)
                        {
                            if (Photo != null)
                            {
                                string sFileName = Security.CurrentUserID + ".jpg";

                                Int64 FileSize = 0;
                                using (var ms = new MemoryStream())
                                {
                                    Photo.Save(ms, ImageFormat.Jpeg);
                                    FileSize = ms.Length;
                                }
                                Photo.Save(tempFolder + "/" + sFileName, ImageFormat.Jpeg);
                                FM.UploadFile(tempFolder + "/" + sFileName,
                                    sDestFolder + "/" + sFileName, Configs.FTPType);
                                DT.Rows[0]["FileSize"] = FileSize;
                            }
                            else
                                DT.Rows[0]["Photo"] = DBNull.Value;
                        }
                        else
                        {
                            Int64 FileSize = 0;
                            using (var ms = new MemoryStream())
                            {
                                Photo.Save(ms, ImageFormat.Jpeg);
                                FileSize = ms.Length;
                            }

                            string sFileName = Security.CurrentUserID + ".jpg";
                            Photo.Save(tempFolder + "/" + sFileName, ImageFormat.Jpeg);
                            FM.UploadFile(tempFolder + "/" + sFileName,
                                sDestFolder + "/" + sFileName, Configs.FTPType);
                            DataRow NewRow = DT.NewRow();
                            NewRow["UserID"] = Security.CurrentUserID;
                            NewRow["Photo"] = sFileName;
                            NewRow["FileSize"] = FileSize;
                            DT.Rows.Add(NewRow);
                        }

                        DA.Update(DT);
                    }
                }
            }

            TablesManager.FillUsersPhotoDataTable();
        }

        public void UpdateUsersDataTable()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Users  WHERE Fired <> 1 ORDER BY Name", ConnectionStrings.UsersConnectionString))
            {
                UsersDataTable.Clear();
                DA.Fill(UsersDataTable);
            }
        }

    }






    class LightCrypto
    {
        public static string Encrypt(string ToEncrypt, bool useHasing, string Password)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(ToEncrypt);
            //System.Configuration.AppSettingsReader settingsReader = new     AppSettingsReader();
            if (useHasing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Password));
                hashmd5.Clear();
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(Password);
            }
            TripleDESCryptoServiceProvider tDes = new TripleDESCryptoServiceProvider()
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = tDes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tDes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cypherString, bool useHasing, string Password)
        {
            byte[] keyArray;
            byte[] toDecryptArray = Convert.FromBase64String(cypherString);
            //byte[] toEncryptArray = Convert.FromBase64String(cypherString);
            //System.Configuration.AppSettingsReader settingReader = new     AppSettingsReader();
            if (useHasing)
            {
                MD5CryptoServiceProvider hashmd = new MD5CryptoServiceProvider();
                keyArray = hashmd.ComputeHash(UTF8Encoding.UTF8.GetBytes(Password));
                hashmd.Clear();
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(Password);
            }
            TripleDESCryptoServiceProvider tDes = new TripleDESCryptoServiceProvider()
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = tDes.CreateDecryptor();
            try
            {
                byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);

                tDes.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray, 0, resultArray.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }




    public static class CheckOrdersStatus
    {
        public struct PackageStatues
        {
            /// <summary>
            /// 0 - нет упаковок
            /// 1 - частично
            /// 2 - все
            /// </summary>
            public int ProfilNotPacked;

            public int ProfilPacked;
            public int ProfilStore;
            public int ProfilExp;
            public int ProfilDisp;

            public int TPSNotPacked;
            public int TPSPacked;
            public int TPSStore;
            public int TPSExp;
            public int TPSDisp;

            public bool FullDisp;
            public object DispDate;

            public void ClearStatuses()
            {
                ProfilNotPacked = 0;
                ProfilPacked = 0;
                ProfilStore = 0;
                ProfilExp = 0;
                ProfilDisp = 0;

                TPSNotPacked = 0;
                TPSPacked = 0;
                TPSStore = 0;
                TPSExp = 0;
                TPSDisp = 0;
                FullDisp = false;
                DispDate = DBNull.Value;
            }
        }

        static private ArrayList GetMegaOrdersInDispatch(ArrayList DispatchIDs)
        {
            string OrdersConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            ArrayList MegaOrders = new ArrayList();

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MegaOrderID FROM MegaOrders
				WHERE MegaOrderID IN (SELECT DISTINCT MegaOrderID FROM MainOrders
				WHERE MainOrderID IN (SELECT DISTINCT MainOrderID FROM Packages WHERE DispatchID IN (" +
                                                          string.Join(",", DispatchIDs.OfType<Int32>().ToArray()) +
                                                          ")))",
                OrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0 && DT.Rows[0]["MegaOrderID"] != DBNull.Value)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                            MegaOrders.Add(Convert.ToInt32(DT.Rows[i]["MegaOrderID"]));
                    }
                }
            }

            return MegaOrders;
        }

        static private ArrayList GetMegaOrdersInTray(int TrayID)
        {
            string OrdersConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            ArrayList MegaOrders = new ArrayList();

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MegaOrderID FROM MegaOrders
				WHERE AgreementStatusID = 2 AND MegaOrderID IN (SELECT DISTINCT MegaOrderID FROM MainOrders
				WHERE MainOrderID IN (SELECT DISTINCT MainOrderID FROM Packages WHERE TrayID = " + TrayID + "))",
                OrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0 && DT.Rows[0]["MegaOrderID"] != DBNull.Value)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                            MegaOrders.Add(Convert.ToInt32(DT.Rows[i]["MegaOrderID"]));
                    }
                }
            }

            return MegaOrders;
        }

        static private int GetMainOrderPackCount(bool Marketing, int MainOrderID, int FactoryID)
        {
            string OrdersConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            if (!Marketing)
                OrdersConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            int Count = 0;
            DataTable FrontsDT = new DataTable();
            DataTable DecorDT = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackNumber FROM PackageDetails WHERE PackageID IN " +
                                                          "(SELECT PackageID FROM Packages WHERE MainOrderID = " +
                                                          MainOrderID +
                                                          " AND ProductType = 0" + " AND FactoryID=" + FactoryID + ")",
                OrdersConnectionString))
            {
                DA.Fill(FrontsDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackNumber FROM PackageDetails WHERE PackageID IN " +
                                                          "(SELECT PackageID FROM Packages WHERE MainOrderID = " +
                                                          MainOrderID +
                                                          " AND ProductType = 1" + " AND FactoryID=" + FactoryID + ")",
                OrdersConnectionString))
            {
                DA.Fill(DecorDT);
            }

            DataTable DT = new DataTable();
            DT.Columns.Add(new DataColumn("PackNumber", Type.GetType("System.Int32")));

            foreach (DataRow Row in FrontsDT.Rows)
            {
                if (Row["PackNumber"].ToString().Length > 0)
                    DT.ImportRow(Row);
            }

            foreach (DataRow Row in DecorDT.Rows)
            {
                if (Row["PackNumber"].ToString().Length > 0)
                    DT.ImportRow(Row);
            }


            using (DataView DV = new DataView(DT.Copy()))
            {
                DT.Clear();

                DV.Sort = "PackNumber ASC";
                DT = DV.ToTable(true, new string[] {"PackNumber"});
            }

            for (int i = 0; i < DT.Rows.Count; i++)
            {
                Count++;
            }

            DT.Dispose();

            return Count;
        }

        /// <summary>
        /// вычисляет статусы упаковок для конкретного подзаказа
        /// </summary>
        /// <param name="FactoryID"></param>
        /// <param name="MainOrderID"></param>
        /// <param name="PS"></param>
        /// <returns>возвращает false, если в таблице Packages нет ни одной строки для подзаказа (ещё не распределен)</returns>
        static private bool GetPackageStatuses(bool Marketing, int FactoryID, int MainOrderID, ref PackageStatues PS)
        {
            string OrdersConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            if (!Marketing)
                OrdersConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            //int AllPackages = 0;
            int ProfilPackages = 0;
            int TPSPackages = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages" +
                                                          " WHERE MainOrderID = " + MainOrderID +
                                                          " ORDER BY DispatchDateTime",
                OrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        //общее кол-во упаковок
                        DataRow[] ProfilRows = DT.Select("FactoryID = 1");
                        if (ProfilRows.Count() > 0)
                            ProfilPackages = ProfilRows.Count();

                        DataRow[] TPSRows = DT.Select("FactoryID = 2");
                        if (TPSRows.Count() > 0)
                            TPSPackages = TPSRows.Count();

                        PS.FullDisp = false;
                        PS.DispDate = DBNull.Value;

                        #region Profil

                        if (FactoryID == 1)
                        {
                            //упаковки со статусом НЕ Упаковано
                            DataRow[] NPRows = DT.Select("PackageStatusID = 0");
                            {
                                if (NPRows.Count() > 0)
                                {
                                    if (NPRows.Count() == ProfilPackages)
                                        PS.ProfilNotPacked = 2;
                                    else
                                        PS.ProfilNotPacked = 1;
                                }
                                else
                                    PS.ProfilNotPacked = 0;
                            }

                            //упаковки со статусом Упаковано
                            DataRow[] PRows = DT.Select("PackageStatusID = 1");
                            {
                                if (PRows.Count() > 0)
                                {
                                    if (PRows.Count() == ProfilPackages)
                                        PS.ProfilPacked = 2;
                                    else
                                        PS.ProfilPacked = 1;
                                }
                                else
                                    PS.ProfilPacked = 0;
                            }

                            //упаковки со статусом Склад
                            DataRow[] SRows = DT.Select("PackageStatusID = 2");
                            {
                                if (SRows.Count() > 0)
                                {
                                    if (SRows.Count() == ProfilPackages)
                                        PS.ProfilStore = 2;
                                    else
                                        PS.ProfilStore = 1;
                                }
                                else
                                    PS.ProfilStore = 0;
                            }

                            //упаковки со статусом Экспедиция
                            DataRow[] ERows = DT.Select("PackageStatusID = 4");
                            {
                                if (ERows.Count() > 0)
                                {
                                    if (ERows.Count() == ProfilPackages)
                                        PS.ProfilExp = 2;
                                    else
                                        PS.ProfilExp = 1;
                                }
                                else
                                    PS.ProfilExp = 0;
                            }

                            //упаковки со статусом Отгружено
                            DataRow[] DRows = DT.Select("PackageStatusID = 3");
                            {
                                if (DRows.Count() > 0)
                                {
                                    if (DRows.Count() == ProfilPackages)
                                    {
                                        PS.DispDate = DRows[0]["DispatchDateTime"];
                                        PS.FullDisp = true;
                                        PS.ProfilDisp = 2;
                                    }
                                    else
                                        PS.ProfilDisp = 1;
                                }
                                else
                                    PS.ProfilDisp = 0;
                            }
                        }

                        #endregion

                        #region TPS

                        if (FactoryID == 2)
                        {
                            DataRow[] NPRows = DT.Select("PackageStatusID = 0");
                            {
                                if (NPRows.Count() > 0)
                                {
                                    if (NPRows.Count() == TPSPackages)
                                        PS.TPSNotPacked = 2;
                                    else
                                        PS.TPSNotPacked = 1;
                                }
                                else
                                    PS.TPSNotPacked = 0;
                            }

                            DataRow[] PRows = DT.Select("PackageStatusID = 1");
                            {
                                if (PRows.Count() > 0)
                                {
                                    if (PRows.Count() == TPSPackages)
                                        PS.TPSPacked = 2;
                                    else
                                        PS.TPSPacked = 1;
                                }
                                else
                                    PS.TPSPacked = 0;
                            }

                            DataRow[] SRows = DT.Select("PackageStatusID = 2");
                            {
                                if (SRows.Count() > 0)
                                {
                                    if (SRows.Count() == TPSPackages)
                                        PS.TPSStore = 2;
                                    else
                                        PS.TPSStore = 1;
                                }
                                else
                                    PS.TPSStore = 0;
                            }

                            DataRow[] ERows = DT.Select("PackageStatusID = 4");
                            {
                                if (ERows.Count() > 0)
                                {
                                    if (ERows.Count() == TPSPackages)
                                        PS.TPSExp = 2;
                                    else
                                        PS.TPSExp = 1;
                                }
                                else
                                    PS.TPSExp = 0;
                            }

                            DataRow[] DRows = DT.Select("PackageStatusID = 3");
                            {
                                if (DRows.Count() > 0)
                                {
                                    if (DRows.Count() == TPSPackages)
                                    {
                                        PS.DispDate = DRows[0]["DispatchDateTime"];
                                        PS.FullDisp = true;
                                        PS.TPSDisp = 2;
                                    }
                                    else
                                        PS.TPSDisp = 1;
                                }
                                else
                                    PS.TPSDisp = 0;
                            }
                        }

                        #endregion

                        #region Profil + TPS

                        if (FactoryID == 0)
                        {
                            //ПРОФИЛЬ
                            DataRow[] NPRows = DT.Select("PackageStatusID = 0 AND FactoryID = 1");
                            {
                                if (NPRows.Count() > 0)
                                {
                                    if (NPRows.Count() == ProfilPackages)
                                        PS.ProfilNotPacked = 2;
                                    else
                                        PS.ProfilNotPacked = 1;
                                }
                                else
                                    PS.ProfilNotPacked = 0;
                            }

                            DataRow[] PRows = DT.Select("PackageStatusID = 1 AND FactoryID = 1");
                            {
                                if (PRows.Count() > 0)
                                {
                                    if (PRows.Count() == ProfilPackages)
                                        PS.ProfilPacked = 2;
                                    else
                                        PS.ProfilPacked = 1;
                                }
                                else
                                    PS.ProfilPacked = 0;
                            }

                            DataRow[] SRows = DT.Select("PackageStatusID = 2 AND FactoryID = 1");
                            {
                                if (SRows.Count() > 0)
                                {
                                    if (SRows.Count() == ProfilPackages)
                                        PS.ProfilStore = 2;
                                    else
                                        PS.ProfilStore = 1;
                                }
                                else
                                    PS.ProfilStore = 0;
                            }

                            DataRow[] ERows = DT.Select("PackageStatusID = 4 AND FactoryID = 1");
                            {
                                if (ERows.Count() > 0)
                                {
                                    if (ERows.Count() == ProfilPackages)
                                        PS.ProfilExp = 2;
                                    else
                                        PS.ProfilExp = 1;
                                }
                                else
                                    PS.ProfilExp = 0;
                            }

                            DataRow[] DRows = DT.Select("PackageStatusID = 3 AND FactoryID = 1");
                            {
                                if (DRows.Count() > 0)
                                {
                                    if (DRows.Count() == ProfilPackages)
                                    {
                                        PS.DispDate = DRows[0]["DispatchDateTime"];
                                        PS.FullDisp = true;
                                        PS.ProfilDisp = 2;
                                    }
                                    else
                                        PS.ProfilDisp = 1;
                                }
                                else
                                    PS.ProfilDisp = 0;
                            }

                            //ТПС
                            DataRow[] NPRowsTPS = DT.Select("PackageStatusID = 0 AND FactoryID = 2");
                            {
                                if (NPRowsTPS.Count() > 0)
                                {
                                    if (NPRowsTPS.Count() == TPSPackages)
                                        PS.TPSNotPacked = 2;
                                    else
                                        PS.TPSNotPacked = 1;
                                }
                                else
                                    PS.TPSNotPacked = 0;
                            }

                            DataRow[] PRowsTPS = DT.Select("PackageStatusID = 1 AND FactoryID = 2");
                            {
                                if (PRowsTPS.Count() > 0)
                                {
                                    if (PRowsTPS.Count() == TPSPackages)
                                        PS.TPSPacked = 2;
                                    else
                                        PS.TPSPacked = 1;
                                }
                                else
                                    PS.TPSPacked = 0;
                            }

                            DataRow[] SRowsTPS = DT.Select("PackageStatusID = 2 AND FactoryID = 2");
                            {
                                if (SRowsTPS.Count() > 0)
                                {
                                    if (SRowsTPS.Count() == TPSPackages)
                                        PS.TPSStore = 2;
                                    else
                                        PS.TPSStore = 1;
                                }
                                else
                                    PS.TPSStore = 0;
                            }

                            DataRow[] ERowsTPS = DT.Select("PackageStatusID = 4 AND FactoryID = 2");
                            {
                                if (ERowsTPS.Count() > 0)
                                {
                                    if (ERowsTPS.Count() == TPSPackages)
                                        PS.TPSExp = 2;
                                    else
                                        PS.TPSExp = 1;
                                }
                                else
                                    PS.TPSExp = 0;
                            }

                            DataRow[] DRowsTPS = DT.Select("PackageStatusID = 3 AND FactoryID = 2");
                            {
                                if (DRowsTPS.Count() > 0)
                                {
                                    if (DRowsTPS.Count() == TPSPackages)
                                    {
                                        PS.DispDate = DRowsTPS[0]["DispatchDateTime"];
                                        PS.FullDisp = true;
                                        PS.TPSDisp = 2;
                                    }
                                    else
                                        PS.TPSDisp = 1;
                                }
                                else
                                    PS.TPSDisp = 0;
                            }
                        }

                        #endregion

                    }
                    else
                        return false;
                }
            }

            return true;
        }

        static public void CopySampleOrders(bool Marketing, int MainOrderID, object DispDate)
        {
            string OrdersConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            if (!Marketing)
                OrdersConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            DataTable TempDT = new DataTable();
            string SelectCommand = @"SELECT * FROM MainOrders WHERE MainOrderID = " + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, OrdersConnectionString))
            {
                DA.Fill(TempDT);
            }

            SelectCommand = @"SELECT * FROM SampleMainOrders WHERE MainOrderID = " + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, OrdersConnectionString))
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
                                DataRow[] Rows =
                                    DT.Select("MainOrderID=" + Convert.ToInt32(TempDT.Rows[i]["MainOrderID"]));
                                if (Rows.Count() > 0)
                                    continue;
                                DataRow NewRow = DT.NewRow();
                                NewRow.ItemArray = TempDT.Rows[i].ItemArray;
                                if (DispDate != DBNull.Value)
                                    NewRow["DispDate"] = DispDate;
                                DT.Rows.Add(NewRow);
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }

            TempDT.Dispose();
            TempDT = new DataTable();
            SelectCommand = @"SELECT * FROM FrontsOrders WHERE MainOrderID = " + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, OrdersConnectionString))
            {
                DA.Fill(TempDT);
            }

            SelectCommand = @"SELECT * FROM SampleFrontsOrders WHERE MainOrderID = " + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, OrdersConnectionString))
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
                                DataRow[] Rows =
                                    DT.Select("FrontsOrdersID=" + Convert.ToInt32(TempDT.Rows[i]["FrontsOrdersID"]));
                                if (Rows.Count() > 0)
                                    continue;
                                DataRow NewRow = DT.NewRow();
                                NewRow.ItemArray = TempDT.Rows[i].ItemArray;
                                DT.Rows.Add(NewRow);
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }

            TempDT.Dispose();
            TempDT = new DataTable();
            SelectCommand = @"SELECT * FROM DecorOrders WHERE MainOrderID = " + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, OrdersConnectionString))
            {
                DA.Fill(TempDT);
            }

            SelectCommand = @"SELECT * FROM SampleDecorOrders WHERE MainOrderID = " + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, OrdersConnectionString))
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
                                DataRow[] Rows =
                                    DT.Select("DecorOrderID=" + Convert.ToInt32(TempDT.Rows[i]["DecorOrderID"]));
                                if (Rows.Count() > 0)
                                    continue;
                                DataRow NewRow = DT.NewRow();
                                NewRow.ItemArray = TempDT.Rows[i].ItemArray;
                                DT.Rows.Add(NewRow);
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        static public void SetMainOrderStatusForDispatch(bool Marketing, ArrayList DispatchIDs)
        {
            string OrdersConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            if (!Marketing)
                OrdersConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            bool IsAllocPack = false;

            int FactoryID = -1;
            int ProfilProductionStatusID = 0;
            int ProfilStorageStatusID = 0;
            int ProfilExpeditionStatusID = 0;
            int ProfilDispatchStatusID = 0;
            int TPSProductionStatusID = 0;
            int TPSStorageStatusID = 0;
            int TPSExpeditionStatusID = 0;
            int TPSDispatchStatusID = 0;

            if (Marketing)
            {
                string SelectCommand = "SELECT MainOrderID, IsSample, FactoryID," +
                                       " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID, " +
                                       " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID FROM MainOrders WHERE MainOrderID IN" +
                                       " (SELECT DISTINCT MainOrderID FROM Packages WHERE DispatchID IN (" +
                                       string.Join(",", DispatchIDs.OfType<Int32>().ToArray()) + "))";
                PackageStatues PS = new PackageStatues();

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    bool IsSample = Convert.ToBoolean(DT.Rows[i]["IsSample"]);
                                    FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);
                                    ProfilProductionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]);
                                    ProfilStorageStatusID = Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]);
                                    ProfilExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]);
                                    ProfilDispatchStatusID = Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]);
                                    TPSProductionStatusID = Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]);
                                    TPSStorageStatusID = Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]);
                                    TPSExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]);
                                    TPSDispatchStatusID = Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]);
                                    PS.ClearStatuses();
                                    IsAllocPack = GetPackageStatuses(Marketing, FactoryID,
                                        Convert.ToInt32(DT.Rows[i]["MainOrderID"]), ref PS);
                                    if (!IsAllocPack)
                                        return;
                                    if (FactoryID == 1)
                                    {
                                        SetProfilMainOrderStatus(ref ProfilProductionStatusID,
                                            ref ProfilStorageStatusID,
                                            ref ProfilExpeditionStatusID, ref ProfilDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }

                                        TPSProductionStatusID = 0;
                                        TPSStorageStatusID = 0;
                                        TPSExpeditionStatusID = 0;
                                        TPSDispatchStatusID = 0;
                                    }

                                    if (FactoryID == 2)
                                    {
                                        SetTPSMainOrderStatus(ref TPSProductionStatusID, ref TPSStorageStatusID,
                                            ref TPSExpeditionStatusID, ref TPSDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }

                                        ProfilProductionStatusID = 0;
                                        ProfilStorageStatusID = 0;
                                        ProfilExpeditionStatusID = 0;
                                        ProfilDispatchStatusID = 0;
                                    }

                                    if (FactoryID == 0)
                                    {
                                        SetProfilMainOrderStatus(ref ProfilProductionStatusID,
                                            ref ProfilStorageStatusID,
                                            ref ProfilExpeditionStatusID, ref ProfilDispatchStatusID, ref PS);

                                        SetTPSMainOrderStatus(ref TPSProductionStatusID, ref TPSStorageStatusID,
                                            ref TPSExpeditionStatusID, ref TPSDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }
                                    }

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilProductionStatusID"] = ProfilProductionStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]) != 0)
                                        DT.Rows[i]["ProfilStorageStatusID"] = ProfilStorageStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilExpeditionStatusID"] = ProfilExpeditionStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]) != 0)
                                        DT.Rows[i]["ProfilDispatchStatusID"] = ProfilDispatchStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]) != 0)
                                        DT.Rows[i]["TPSProductionStatusID"] = TPSProductionStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]) != 0)
                                        DT.Rows[i]["TPSStorageStatusID"] = TPSStorageStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["TPSExpeditionStatusID"] = TPSExpeditionStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]) != 0)
                                        DT.Rows[i]["TPSDispatchStatusID"] = TPSDispatchStatusID;

                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }

                IsAllocPack = false;

                FactoryID = -1;
                ProfilProductionStatusID = 0;
                ProfilStorageStatusID = 0;
                ProfilExpeditionStatusID = 0;
                ProfilDispatchStatusID = 0;
                TPSProductionStatusID = 0;
                TPSStorageStatusID = 0;
                TPSExpeditionStatusID = 0;
                TPSDispatchStatusID = 0;

                SelectCommand = "SELECT MainOrderID, IsSample, FactoryID," +
                                " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID, " +
                                " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID FROM NewMainOrders WHERE MainOrderID IN" +
                                " (SELECT DISTINCT MainOrderID FROM Packages WHERE DispatchID IN (" +
                                string.Join(",", DispatchIDs.OfType<Int32>().ToArray()) + "))";
                PS = new PackageStatues();
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    bool IsSample = Convert.ToBoolean(DT.Rows[i]["IsSample"]);
                                    FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);
                                    ProfilProductionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]);
                                    ProfilStorageStatusID = Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]);
                                    ProfilExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]);
                                    ProfilDispatchStatusID = Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]);
                                    TPSProductionStatusID = Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]);
                                    TPSStorageStatusID = Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]);
                                    TPSExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]);
                                    TPSDispatchStatusID = Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]);
                                    PS.ClearStatuses();
                                    IsAllocPack = GetPackageStatuses(Marketing, FactoryID,
                                        Convert.ToInt32(DT.Rows[i]["MainOrderID"]), ref PS);
                                    if (!IsAllocPack)
                                        return;

                                    if (FactoryID == 1)
                                    {
                                        SetProfilMainOrderStatus(ref ProfilProductionStatusID,
                                            ref ProfilStorageStatusID,
                                            ref ProfilExpeditionStatusID, ref ProfilDispatchStatusID, ref PS);

                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }

                                        TPSProductionStatusID = 0;
                                        TPSStorageStatusID = 0;
                                        TPSExpeditionStatusID = 0;
                                        TPSDispatchStatusID = 0;
                                    }

                                    if (FactoryID == 2)
                                    {
                                        SetTPSMainOrderStatus(ref TPSProductionStatusID, ref TPSStorageStatusID,
                                            ref TPSExpeditionStatusID, ref TPSDispatchStatusID, ref PS);

                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }

                                        ProfilProductionStatusID = 0;
                                        ProfilStorageStatusID = 0;
                                        ProfilExpeditionStatusID = 0;
                                        ProfilDispatchStatusID = 0;
                                    }

                                    if (FactoryID == 0)
                                    {
                                        SetProfilMainOrderStatus(ref ProfilProductionStatusID,
                                            ref ProfilStorageStatusID,
                                            ref ProfilExpeditionStatusID, ref ProfilDispatchStatusID, ref PS);

                                        SetTPSMainOrderStatus(ref TPSProductionStatusID, ref TPSStorageStatusID,
                                            ref TPSExpeditionStatusID, ref TPSDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }
                                    }

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilProductionStatusID"] = ProfilProductionStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]) != 0)
                                        DT.Rows[i]["ProfilStorageStatusID"] = ProfilStorageStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilExpeditionStatusID"] = ProfilExpeditionStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]) != 0)
                                        DT.Rows[i]["ProfilDispatchStatusID"] = ProfilDispatchStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]) != 0)
                                        DT.Rows[i]["TPSProductionStatusID"] = TPSProductionStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]) != 0)
                                        DT.Rows[i]["TPSStorageStatusID"] = TPSStorageStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["TPSExpeditionStatusID"] = TPSExpeditionStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]) != 0)
                                        DT.Rows[i]["TPSDispatchStatusID"] = TPSDispatchStatusID;

                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }
            }
            else
            {
                string SelectCommand = "SELECT MainOrderID, IsSample, FactoryID," +
                                       " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID, " +
                                       " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID FROM MainOrders WHERE MainOrderID IN" +
                                       " (SELECT DISTINCT MainOrderID FROM Packages WHERE DispatchID IN (" +
                                       string.Join(",", DispatchIDs.OfType<Int32>().ToArray()) + "))";
                PackageStatues PS = new PackageStatues();

                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    bool IsSample = Convert.ToBoolean(DT.Rows[i]["IsSample"]);
                                    FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);
                                    ProfilProductionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]);
                                    ProfilStorageStatusID = Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]);
                                    ProfilExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]);
                                    ProfilDispatchStatusID = Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]);
                                    TPSProductionStatusID = Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]);
                                    TPSStorageStatusID = Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]);
                                    TPSExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]);
                                    TPSDispatchStatusID = Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]);
                                    PS.ClearStatuses();
                                    IsAllocPack = GetPackageStatuses(Marketing, FactoryID,
                                        Convert.ToInt32(DT.Rows[i]["MainOrderID"]), ref PS);
                                    if (!IsAllocPack)
                                        return;

                                    if (FactoryID == 1)
                                    {
                                        SetProfilMainOrderStatus(ref ProfilProductionStatusID,
                                            ref ProfilStorageStatusID,
                                            ref ProfilExpeditionStatusID, ref ProfilDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }

                                        TPSProductionStatusID = 0;
                                        TPSStorageStatusID = 0;
                                        TPSExpeditionStatusID = 0;
                                        TPSDispatchStatusID = 0;
                                    }

                                    if (FactoryID == 2)
                                    {
                                        SetTPSMainOrderStatus(ref TPSProductionStatusID, ref TPSStorageStatusID,
                                            ref TPSExpeditionStatusID, ref TPSDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }

                                        ProfilProductionStatusID = 0;
                                        ProfilStorageStatusID = 0;
                                        ProfilExpeditionStatusID = 0;
                                        ProfilDispatchStatusID = 0;
                                    }

                                    if (FactoryID == 0)
                                    {
                                        SetProfilMainOrderStatus(ref ProfilProductionStatusID,
                                            ref ProfilStorageStatusID,
                                            ref ProfilExpeditionStatusID, ref ProfilDispatchStatusID, ref PS);
                                        SetTPSMainOrderStatus(ref TPSProductionStatusID, ref TPSStorageStatusID,
                                            ref TPSExpeditionStatusID, ref TPSDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }
                                    }

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilProductionStatusID"] = ProfilProductionStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]) != 0)
                                        DT.Rows[i]["ProfilStorageStatusID"] = ProfilStorageStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilExpeditionStatusID"] = ProfilExpeditionStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]) != 0)
                                        DT.Rows[i]["ProfilDispatchStatusID"] = ProfilDispatchStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]) != 0)
                                        DT.Rows[i]["TPSProductionStatusID"] = TPSProductionStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]) != 0)
                                        DT.Rows[i]["TPSStorageStatusID"] = TPSStorageStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["TPSExpeditionStatusID"] = TPSExpeditionStatusID;

                                    if (Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]) != 0)
                                        DT.Rows[i]["TPSDispatchStatusID"] = TPSDispatchStatusID;

                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }
            }
        }

        static public void SetMainOrderStatus(bool Marketing, int MainOrderID, bool IsTray)
        {
            bool IsAllocPack = false;
            int FactoryID = -1;
            int ProfilProductionStatusID = 0;
            int ProfilStorageStatusID = 0;
            int ProfilExpeditionStatusID = 0;
            int ProfilDispatchStatusID = 0;
            int TPSProductionStatusID = 0;
            int TPSStorageStatusID = 0;
            int TPSExpeditionStatusID = 0;
            int TPSDispatchStatusID = 0;
            if (Marketing)
            {
                string SelectCommand = "SELECT MainOrderID, IsSample, FactoryID," +
                                       " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID, " +
                                       " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID FROM MainOrders WHERE MainOrderID = " +
                                       MainOrderID;
                if (IsTray)
                    SelectCommand = "SELECT MainOrderID, IsSample, FactoryID," +
                                    " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID, " +
                                    " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID FROM MainOrders WHERE MainOrderID IN" +
                                    " (SELECT DISTINCT MainOrderID FROM Packages WHERE TrayID = " + MainOrderID + ")";
                PackageStatues PS = new PackageStatues();
                using (SqlDataAdapter DA =
                    new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    bool IsSample = Convert.ToBoolean(DT.Rows[i]["IsSample"]);
                                    FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);
                                    ProfilProductionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]);
                                    ProfilStorageStatusID = Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]);
                                    ProfilExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]);
                                    ProfilDispatchStatusID = Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]);
                                    TPSProductionStatusID = Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]);
                                    TPSStorageStatusID = Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]);
                                    TPSExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]);
                                    TPSDispatchStatusID = Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]);
                                    PS.ClearStatuses();
                                    IsAllocPack = GetPackageStatuses(Marketing, FactoryID,
                                        Convert.ToInt32(DT.Rows[i]["MainOrderID"]), ref PS);
                                    if (!IsAllocPack)
                                        return;
                                    if (FactoryID == 1)
                                    {
                                        SetProfilMainOrderStatus(ref ProfilProductionStatusID,
                                            ref ProfilStorageStatusID,
                                            ref ProfilExpeditionStatusID, ref ProfilDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }

                                        TPSProductionStatusID = 0;
                                        TPSStorageStatusID = 0;
                                        TPSExpeditionStatusID = 0;
                                        TPSDispatchStatusID = 0;
                                    }

                                    if (FactoryID == 2)
                                    {
                                        SetTPSMainOrderStatus(ref TPSProductionStatusID, ref TPSStorageStatusID,
                                            ref TPSExpeditionStatusID, ref TPSDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }

                                        ProfilProductionStatusID = 0;
                                        ProfilStorageStatusID = 0;
                                        ProfilExpeditionStatusID = 0;
                                        ProfilDispatchStatusID = 0;
                                    }

                                    if (FactoryID == 0)
                                    {
                                        SetProfilMainOrderStatus(ref ProfilProductionStatusID,
                                            ref ProfilStorageStatusID,
                                            ref ProfilExpeditionStatusID, ref ProfilDispatchStatusID, ref PS);
                                        SetTPSMainOrderStatus(ref TPSProductionStatusID, ref TPSStorageStatusID,
                                            ref TPSExpeditionStatusID, ref TPSDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }
                                    }

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilProductionStatusID"] = ProfilProductionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]) != 0)
                                        DT.Rows[i]["ProfilStorageStatusID"] = ProfilStorageStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilExpeditionStatusID"] = ProfilExpeditionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]) != 0)
                                        DT.Rows[i]["ProfilDispatchStatusID"] = ProfilDispatchStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]) != 0)
                                        DT.Rows[i]["TPSProductionStatusID"] = TPSProductionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]) != 0)
                                        DT.Rows[i]["TPSStorageStatusID"] = TPSStorageStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["TPSExpeditionStatusID"] = TPSExpeditionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]) != 0)
                                        DT.Rows[i]["TPSDispatchStatusID"] = TPSDispatchStatusID;
                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }

                IsAllocPack = false;

                FactoryID = -1;
                ProfilProductionStatusID = 0;
                ProfilStorageStatusID = 0;
                ProfilExpeditionStatusID = 0;
                ProfilDispatchStatusID = 0;
                TPSProductionStatusID = 0;
                TPSStorageStatusID = 0;
                TPSExpeditionStatusID = 0;
                TPSDispatchStatusID = 0;

                SelectCommand = "SELECT MainOrderID, IsSample, FactoryID," +
                                " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID, " +
                                " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID FROM NewMainOrders WHERE MainOrderID = " +
                                MainOrderID;
                if (IsTray)
                    SelectCommand = "SELECT MainOrderID, IsSample, FactoryID," +
                                    " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID, " +
                                    " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID FROM NewMainOrders WHERE MainOrderID IN" +
                                    " (SELECT DISTINCT MainOrderID FROM Packages WHERE TrayID = " + MainOrderID + ")";
                PS = new PackageStatues();
                using (SqlDataAdapter DA =
                    new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    bool IsSample = Convert.ToBoolean(DT.Rows[i]["IsSample"]);
                                    FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);
                                    ProfilProductionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]);
                                    ProfilStorageStatusID = Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]);
                                    ProfilExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]);
                                    ProfilDispatchStatusID = Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]);
                                    TPSProductionStatusID = Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]);
                                    TPSStorageStatusID = Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]);
                                    TPSExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]);
                                    TPSDispatchStatusID = Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]);
                                    PS.ClearStatuses();
                                    IsAllocPack = GetPackageStatuses(Marketing, FactoryID,
                                        Convert.ToInt32(DT.Rows[i]["MainOrderID"]), ref PS);
                                    if (!IsAllocPack)
                                        return;
                                    if (FactoryID == 1)
                                    {
                                        SetProfilMainOrderStatus(ref ProfilProductionStatusID,
                                            ref ProfilStorageStatusID,
                                            ref ProfilExpeditionStatusID, ref ProfilDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }

                                        TPSProductionStatusID = 0;
                                        TPSStorageStatusID = 0;
                                        TPSExpeditionStatusID = 0;
                                        TPSDispatchStatusID = 0;
                                    }

                                    if (FactoryID == 2)
                                    {
                                        SetTPSMainOrderStatus(ref TPSProductionStatusID, ref TPSStorageStatusID,
                                            ref TPSExpeditionStatusID, ref TPSDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }

                                        ProfilProductionStatusID = 0;
                                        ProfilStorageStatusID = 0;
                                        ProfilExpeditionStatusID = 0;
                                        ProfilDispatchStatusID = 0;
                                    }

                                    if (FactoryID == 0)
                                    {
                                        SetProfilMainOrderStatus(ref ProfilProductionStatusID,
                                            ref ProfilStorageStatusID,
                                            ref ProfilExpeditionStatusID, ref ProfilDispatchStatusID, ref PS);
                                        SetTPSMainOrderStatus(ref TPSProductionStatusID, ref TPSStorageStatusID,
                                            ref TPSExpeditionStatusID, ref TPSDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }
                                    }

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilProductionStatusID"] = ProfilProductionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]) != 0)
                                        DT.Rows[i]["ProfilStorageStatusID"] = ProfilStorageStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilExpeditionStatusID"] = ProfilExpeditionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]) != 0)
                                        DT.Rows[i]["ProfilDispatchStatusID"] = ProfilDispatchStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]) != 0)
                                        DT.Rows[i]["TPSProductionStatusID"] = TPSProductionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]) != 0)
                                        DT.Rows[i]["TPSStorageStatusID"] = TPSStorageStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["TPSExpeditionStatusID"] = TPSExpeditionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]) != 0)
                                        DT.Rows[i]["TPSDispatchStatusID"] = TPSDispatchStatusID;
                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }
            }
            else
            {
                string SelectCommand = "SELECT MainOrderID, IsSample, FactoryID," +
                                       " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID, " +
                                       " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID FROM MainOrders WHERE MainOrderID = " +
                                       MainOrderID;
                if (IsTray)
                    SelectCommand = "SELECT MainOrderID, IsSample, FactoryID," +
                                    " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID, " +
                                    " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID FROM MainOrders WHERE MainOrderID IN" +
                                    " (SELECT DISTINCT MainOrderID FROM Packages WHERE TrayID = " + MainOrderID + ")";
                PackageStatues PS = new PackageStatues();
                using (SqlDataAdapter DA =
                    new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    bool IsSample = Convert.ToBoolean(DT.Rows[i]["IsSample"]);
                                    FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);
                                    ProfilProductionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]);
                                    ProfilStorageStatusID = Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]);
                                    ProfilExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]);
                                    ProfilDispatchStatusID = Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]);
                                    TPSProductionStatusID = Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]);
                                    TPSStorageStatusID = Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]);
                                    TPSExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]);
                                    TPSDispatchStatusID = Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]);
                                    PS.ClearStatuses();
                                    IsAllocPack = GetPackageStatuses(Marketing, FactoryID,
                                        Convert.ToInt32(DT.Rows[i]["MainOrderID"]), ref PS);
                                    if (!IsAllocPack)
                                        return;
                                    if (FactoryID == 1)
                                    {
                                        SetProfilMainOrderStatus(ref ProfilProductionStatusID,
                                            ref ProfilStorageStatusID,
                                            ref ProfilExpeditionStatusID, ref ProfilDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }

                                        TPSProductionStatusID = 0;
                                        TPSStorageStatusID = 0;
                                        TPSExpeditionStatusID = 0;
                                        TPSDispatchStatusID = 0;
                                    }

                                    if (FactoryID == 2)
                                    {
                                        SetTPSMainOrderStatus(ref TPSProductionStatusID, ref TPSStorageStatusID,
                                            ref TPSExpeditionStatusID, ref TPSDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }

                                        ProfilProductionStatusID = 0;
                                        ProfilStorageStatusID = 0;
                                        ProfilExpeditionStatusID = 0;
                                        ProfilDispatchStatusID = 0;
                                    }

                                    if (FactoryID == 0)
                                    {
                                        SetProfilMainOrderStatus(ref ProfilProductionStatusID,
                                            ref ProfilStorageStatusID,
                                            ref ProfilExpeditionStatusID, ref ProfilDispatchStatusID, ref PS);
                                        SetTPSMainOrderStatus(ref TPSProductionStatusID, ref TPSStorageStatusID,
                                            ref TPSExpeditionStatusID, ref TPSDispatchStatusID, ref PS);
                                        if (IsSample)
                                        {
                                            if (PS.FullDisp)
                                                CopySampleOrders(Marketing, Convert.ToInt32(DT.Rows[i]["MainOrderID"]),
                                                    PS.DispDate);
                                        }
                                    }

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilProductionStatusID"] = ProfilProductionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]) != 0)
                                        DT.Rows[i]["ProfilStorageStatusID"] = ProfilStorageStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilExpeditionStatusID"] = ProfilExpeditionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]) != 0)
                                        DT.Rows[i]["ProfilDispatchStatusID"] = ProfilDispatchStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]) != 0)
                                        DT.Rows[i]["TPSProductionStatusID"] = TPSProductionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]) != 0)
                                        DT.Rows[i]["TPSStorageStatusID"] = TPSStorageStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["TPSExpeditionStatusID"] = TPSExpeditionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]) != 0)
                                        DT.Rows[i]["TPSDispatchStatusID"] = TPSDispatchStatusID;
                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }
            }
        }

        private static void SetTPSMainOrderStatus(ref int TPSProductionStatusID, ref int TPSStorageStatusID,
            ref int TPSExpeditionStatusID, ref int TPSDispatchStatusID,
            ref PackageStatues PS)
        {
            if (PS.TPSNotPacked == 2 && PS.TPSPacked == 0 && PS.TPSStore == 0 && PS.TPSExp == 0 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 1;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 2 && PS.TPSStore == 0 && PS.TPSExp == 0 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 0 && PS.TPSStore == 2 && PS.TPSExp == 0 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 0 && PS.TPSStore == 0 && PS.TPSExp == 2 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 1;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 0 && PS.TPSStore == 0 && PS.TPSExp == 0 && PS.TPSDisp == 2)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 1;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 2;
                PS.FullDisp = true;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 0 && PS.TPSStore == 0 && PS.TPSExp == 1 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 1;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 2;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 0 && PS.TPSStore == 1 && PS.TPSExp == 0 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 2;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 0 && PS.TPSStore == 1 && PS.TPSExp == 1 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 0 && PS.TPSStore == 1 && PS.TPSExp == 1 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 2;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 1 && PS.TPSStore == 0 && PS.TPSExp == 0 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 2;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 1 && PS.TPSStore == 0 && PS.TPSExp == 1 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 1 && PS.TPSStore == 0 && PS.TPSExp == 1 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 2;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 1 && PS.TPSStore == 1 && PS.TPSExp == 0 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 1 && PS.TPSStore == 1 && PS.TPSExp == 0 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 2;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 1 && PS.TPSStore == 1 && PS.TPSExp == 1 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 0 && PS.TPSPacked == 1 && PS.TPSStore == 1 && PS.TPSExp == 1 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 1;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 2;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 0 && PS.TPSStore == 0 && PS.TPSExp == 0 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 1;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 2;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 0 && PS.TPSStore == 0 && PS.TPSExp == 1 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 1;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 0 && PS.TPSStore == 0 && PS.TPSExp == 1 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 1;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 2;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 0 && PS.TPSStore == 1 && PS.TPSExp == 0 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 0 && PS.TPSStore == 1 && PS.TPSExp == 0 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 2;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 0 && PS.TPSStore == 1 && PS.TPSExp == 1 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 0 && PS.TPSStore == 1 && PS.TPSExp == 1 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 2;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 1 && PS.TPSStore == 0 && PS.TPSExp == 0 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 1 && PS.TPSStore == 0 && PS.TPSExp == 0 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 2;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 1 && PS.TPSStore == 0 && PS.TPSExp == 1 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 1 && PS.TPSStore == 0 && PS.TPSExp == 1 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 2;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 1 && PS.TPSStore == 1 && PS.TPSExp == 0 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 1 && PS.TPSStore == 1 && PS.TPSExp == 0 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 1;
                TPSDispatchStatusID = 2;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 1 && PS.TPSStore == 1 && PS.TPSExp == 1 && PS.TPSDisp == 0)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 1;
            }

            if (PS.TPSNotPacked == 1 && PS.TPSPacked == 1 && PS.TPSStore == 1 && PS.TPSExp == 1 && PS.TPSDisp == 1)
            {
                TPSProductionStatusID = 2;
                TPSStorageStatusID = 2;
                TPSExpeditionStatusID = 2;
                TPSDispatchStatusID = 2;
            }
        }

        private static void SetProfilMainOrderStatus(ref int ProfilProductionStatusID, ref int ProfilStorageStatusID,
            ref int ProfilExpeditionStatusID, ref int ProfilDispatchStatusID,
            ref PackageStatues PS)
        {
            if (PS.ProfilNotPacked == 2 && PS.ProfilPacked == 0 && PS.ProfilStore == 0 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 1;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 2 && PS.ProfilStore == 0 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 0 && PS.ProfilStore == 2 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 0 && PS.ProfilStore == 0 && PS.ProfilExp == 2 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 1;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 0 && PS.ProfilStore == 0 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 2)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 1;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 2;
                PS.FullDisp = true;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 0 && PS.ProfilStore == 0 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 1;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 2;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 0 && PS.ProfilStore == 1 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 2;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 0 && PS.ProfilStore == 1 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 0 && PS.ProfilStore == 1 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 2;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 1 && PS.ProfilStore == 0 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 2;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 1 && PS.ProfilStore == 0 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 1 && PS.ProfilStore == 0 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 2;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 1 && PS.ProfilStore == 1 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 1 && PS.ProfilStore == 1 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 2;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 1 && PS.ProfilStore == 1 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 0 && PS.ProfilPacked == 1 && PS.ProfilStore == 1 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 1;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 2;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 0 && PS.ProfilStore == 0 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 1;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 2;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 0 && PS.ProfilStore == 0 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 1;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 0 && PS.ProfilStore == 0 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 1;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 2;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 0 && PS.ProfilStore == 1 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 0 && PS.ProfilStore == 1 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 2;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 0 && PS.ProfilStore == 1 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 0 && PS.ProfilStore == 1 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 2;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 1 && PS.ProfilStore == 0 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 1 && PS.ProfilStore == 0 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 2;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 1 && PS.ProfilStore == 0 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 1 && PS.ProfilStore == 0 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 2;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 1 && PS.ProfilStore == 1 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 1 && PS.ProfilStore == 1 && PS.ProfilExp == 0 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 1;
                ProfilDispatchStatusID = 2;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 1 && PS.ProfilStore == 1 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 0)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 1;
            }

            if (PS.ProfilNotPacked == 1 && PS.ProfilPacked == 1 && PS.ProfilStore == 1 && PS.ProfilExp == 1 &&
                PS.ProfilDisp == 1)
            {
                ProfilProductionStatusID = 2;
                ProfilStorageStatusID = 2;
                ProfilExpeditionStatusID = 2;
                ProfilDispatchStatusID = 2;
            }
        }

        static public void SetMainOrderDispatch(int MainOrderID)
        {
            int FactoryID = -1;
            int ProfilProductionStatusID = 0;
            int ProfilStorageStatusID = 0;
            int ProfilExpeditionStatusID = 0;
            int ProfilDispatchStatusID = 0;
            int TPSProductionStatusID = 0;
            int TPSStorageStatusID = 0;
            int TPSExpeditionStatusID = 0;
            int TPSDispatchStatusID = 0;
            int ProfilPackAllocStatusID = 0;
            int TPSPackAllocStatusID = 0;
            string SelectCommand =
                "SELECT MainOrderID, IsSample, FactoryID, ProfilPackAllocStatusID, TPSPackAllocStatusID," +
                " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID, " +
                " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID FROM MainOrders WHERE MainOrderID = " +
                MainOrderID;

            DateTime DispDate = Security.GetCurrentDate();
            //PackageStatues PS = new PackageStatues();
            using (SqlDataAdapter DA =
                new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                bool IsSample = Convert.ToBoolean(DT.Rows[i]["IsSample"]);
                                FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);

                                if (IsSample)
                                {
                                    CopySampleOrders(true, Convert.ToInt32(DT.Rows[i]["MainOrderID"]), DispDate);
                                }

                                if (FactoryID == 1)
                                {
                                    ProfilProductionStatusID = 1;
                                    ProfilStorageStatusID = 1;
                                    ProfilExpeditionStatusID = 1;
                                    ProfilDispatchStatusID = 2;
                                    TPSProductionStatusID = 0;
                                    TPSStorageStatusID = 0;
                                    TPSExpeditionStatusID = 0;
                                    TPSDispatchStatusID = 0;
                                    ProfilPackAllocStatusID = 2;
                                    TPSPackAllocStatusID = 0;
                                }

                                if (FactoryID == 2)
                                {
                                    ProfilProductionStatusID = 0;
                                    ProfilStorageStatusID = 0;
                                    ProfilExpeditionStatusID = 0;
                                    ProfilDispatchStatusID = 0;
                                    TPSProductionStatusID = 1;
                                    TPSStorageStatusID = 1;
                                    TPSExpeditionStatusID = 1;
                                    TPSDispatchStatusID = 2;
                                    ProfilPackAllocStatusID = 0;
                                    TPSPackAllocStatusID = 2;
                                }

                                if (FactoryID == 0)
                                {
                                    ProfilProductionStatusID = 1;
                                    ProfilStorageStatusID = 1;
                                    ProfilExpeditionStatusID = 1;
                                    ProfilDispatchStatusID = 2;
                                    TPSProductionStatusID = 1;
                                    TPSStorageStatusID = 1;
                                    TPSExpeditionStatusID = 1;
                                    TPSDispatchStatusID = 2;
                                    ProfilPackAllocStatusID = 2;
                                    TPSPackAllocStatusID = 2;
                                }

                                if (Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]) != 0)
                                    DT.Rows[i]["ProfilProductionStatusID"] = ProfilProductionStatusID;
                                if (Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]) != 0)
                                    DT.Rows[i]["ProfilStorageStatusID"] = ProfilStorageStatusID;
                                if (Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]) != 0)
                                    DT.Rows[i]["ProfilExpeditionStatusID"] = ProfilExpeditionStatusID;
                                if (Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]) != 0)
                                    DT.Rows[i]["ProfilDispatchStatusID"] = ProfilDispatchStatusID;
                                if (Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]) != 0)
                                    DT.Rows[i]["TPSProductionStatusID"] = TPSProductionStatusID;
                                if (Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]) != 0)
                                    DT.Rows[i]["TPSStorageStatusID"] = TPSStorageStatusID;
                                if (Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]) != 0)
                                    DT.Rows[i]["TPSExpeditionStatusID"] = TPSExpeditionStatusID;
                                if (Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]) != 0)
                                    DT.Rows[i]["TPSDispatchStatusID"] = TPSDispatchStatusID;
                                DT.Rows[i]["ProfilPackAllocStatusID"] = ProfilPackAllocStatusID;
                                DT.Rows[i]["TPSPackAllocStatusID"] = TPSPackAllocStatusID;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }

            FactoryID = -1;
            ProfilProductionStatusID = 0;
            ProfilStorageStatusID = 0;
            ProfilExpeditionStatusID = 0;
            ProfilDispatchStatusID = 0;
            TPSProductionStatusID = 0;
            TPSStorageStatusID = 0;
            TPSExpeditionStatusID = 0;
            TPSDispatchStatusID = 0;

            SelectCommand = "SELECT MainOrderID, IsSample, FactoryID, ProfilPackAllocStatusID, TPSPackAllocStatusID," +
                            " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID, " +
                            " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID FROM NewMainOrders WHERE MainOrderID = " +
                            MainOrderID;

            //PS = new PackageStatues();
            using (SqlDataAdapter DA =
                new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                bool IsSample = Convert.ToBoolean(DT.Rows[i]["IsSample"]);
                                FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);
                                if (IsSample)
                                {
                                    CopySampleOrders(true, Convert.ToInt32(DT.Rows[i]["MainOrderID"]), DispDate);
                                }

                                if (FactoryID == 1)
                                {
                                    ProfilProductionStatusID = 1;
                                    ProfilStorageStatusID = 1;
                                    ProfilExpeditionStatusID = 1;
                                    ProfilDispatchStatusID = 2;
                                    TPSProductionStatusID = 0;
                                    TPSStorageStatusID = 0;
                                    TPSExpeditionStatusID = 0;
                                    TPSDispatchStatusID = 0;
                                }

                                if (FactoryID == 2)
                                {
                                    ProfilProductionStatusID = 0;
                                    ProfilStorageStatusID = 0;
                                    ProfilExpeditionStatusID = 0;
                                    ProfilDispatchStatusID = 0;
                                    TPSProductionStatusID = 1;
                                    TPSStorageStatusID = 1;
                                    TPSExpeditionStatusID = 1;
                                    TPSDispatchStatusID = 2;
                                }

                                if (FactoryID == 0)
                                {
                                    ProfilProductionStatusID = 1;
                                    ProfilStorageStatusID = 1;
                                    ProfilExpeditionStatusID = 1;
                                    ProfilDispatchStatusID = 2;
                                    TPSProductionStatusID = 1;
                                    TPSStorageStatusID = 1;
                                    TPSExpeditionStatusID = 1;
                                    TPSDispatchStatusID = 2;
                                }

                                if (Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]) != 0)
                                    DT.Rows[i]["ProfilProductionStatusID"] = ProfilProductionStatusID;
                                if (Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]) != 0)
                                    DT.Rows[i]["ProfilStorageStatusID"] = ProfilStorageStatusID;
                                if (Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]) != 0)
                                    DT.Rows[i]["ProfilExpeditionStatusID"] = ProfilExpeditionStatusID;
                                if (Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]) != 0)
                                    DT.Rows[i]["ProfilDispatchStatusID"] = ProfilDispatchStatusID;
                                if (Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]) != 0)
                                    DT.Rows[i]["TPSProductionStatusID"] = TPSProductionStatusID;
                                if (Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]) != 0)
                                    DT.Rows[i]["TPSStorageStatusID"] = TPSStorageStatusID;
                                if (Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]) != 0)
                                    DT.Rows[i]["TPSExpeditionStatusID"] = TPSExpeditionStatusID;
                                if (Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]) != 0)
                                    DT.Rows[i]["TPSDispatchStatusID"] = TPSDispatchStatusID;
                                DT.Rows[i]["ProfilPackAllocStatusID"] = ProfilPackAllocStatusID;
                                DT.Rows[i]["TPSPackAllocStatusID"] = TPSPackAllocStatusID;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        static public void SetNewMegaOrderStatus(int MegaOrderID)
        {
            int MegaOrderStatusID = -1;
            int PS = -1;
            int TS = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT MainOrderID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID," +
                " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID" +
                " FROM NewMainOrders WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {

                    DA.Fill(DT);

                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //1) Определение статуса каждого подзаказа отдельно для профиля и тпс

                    int[] PMOST = new int[DT.Rows.Count];
                    int[] TMOST = new int[DT.Rows.Count];

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        int ProfilProductionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]);
                        int ProfilStorageStatusID = Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]);
                        int ProfilExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]);
                        int ProfilDispatchStatusID = Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]);

                        int TPSProductionStatusID = Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]);
                        int TPSStorageStatusID = Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]);
                        int TPSExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]);
                        int TPSDispatchStatusID = Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]);

                        //0	нет производства
                        //1	не был в производстве
                        //2	в производстве
                        //3	на складе
                        //4	отгружен
                        //5	на производстве
                        //6	экспедиция

                        //      Pr     St    Exp   Disp
                        //0      0 	   0     0	    0
                        //1      1 	   1     1	    1
                        //4      1 	   1     1	    2
                        //6      1 	   1     2	    1
                        //6      1 	   1     2	    2
                        //3      1 	   2     1	    1
                        //3      1 	   2     1	    2
                        //3      1 	   2     2	    1
                        //3      1 	   2     2	    2
                        //2      2 	   1     1	    1
                        //2      2 	   1     1	    2
                        //2      2 	   1     2	    1
                        //2      2 	   1     2	    2
                        //2      2 	   2     1	    1
                        //2      2 	   2     1	    2
                        //2      2 	   2     2	    1
                        //2      2 	   2     2	    2
                        //5      3 	   1     1	    1

                        //PROFIL
                        if (ProfilProductionStatusID == 0 && ProfilStorageStatusID == 0 &&
                            ProfilExpeditionStatusID == 0 && ProfilDispatchStatusID == 0)
                            PMOST[i] = 0;
                        if (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 1 &&
                            ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1)
                            PMOST[i] = 1;
                        if (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 1 &&
                            ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 2)
                            PMOST[i] = 4;
                        if ((ProfilProductionStatusID == 1 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 2))
                            PMOST[i] = 6;
                        if ((ProfilProductionStatusID == 1 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 2) ||
                            (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 2))
                            PMOST[i] = 3;
                        if ((ProfilProductionStatusID == 2 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 2) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 2) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 2) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 2))
                            PMOST[i] = 2;
                        if (ProfilProductionStatusID == 3 && ProfilStorageStatusID == 1 &&
                            ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1)
                            PMOST[i] = 5;

                        //TPS
                        if (TPSProductionStatusID == 0 && TPSStorageStatusID == 0 && TPSExpeditionStatusID == 0 &&
                            TPSDispatchStatusID == 0)
                            TMOST[i] = 0;
                        if (TPSProductionStatusID == 1 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                            TPSDispatchStatusID == 1)
                            TMOST[i] = 1;
                        if (TPSProductionStatusID == 1 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                            TPSDispatchStatusID == 2)
                            TMOST[i] = 4;
                        if ((TPSProductionStatusID == 1 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 1 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 2))
                            TMOST[i] = 6;
                        if ((TPSProductionStatusID == 1 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 1 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 2) ||
                            (TPSProductionStatusID == 1 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 1 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 2))
                            TMOST[i] = 3;
                        if ((TPSProductionStatusID == 2 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 2) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 2) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 2) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 2))
                            TMOST[i] = 2;
                        if (TPSProductionStatusID == 3 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                            TPSDispatchStatusID == 1)
                            TMOST[i] = 5;
                    }

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //2) Определение статуса заказа по фирмам

                    int No = 0;
                    int NotProd = 0;
                    int Prod = 0;
                    int OnProd = 0;
                    int Stor = 0;
                    int Exp = 0;
                    int Disp = 0;
                    int ALL = PMOST.Count();

                    //ProfilStatus
                    for (int i = 0; i < PMOST.Count(); i++)
                    {
                        if (PMOST[i] == 0)
                            No++;
                        if (PMOST[i] == 1)
                            NotProd++;
                        if (PMOST[i] == 2)
                            Prod++;
                        if (PMOST[i] == 3)
                            Stor++;
                        if (PMOST[i] == 6)
                            Exp++;
                        if (PMOST[i] == 5)
                            OnProd++;
                        if (PMOST[i] == 4)
                            Disp++;
                    }

                    if (No == ALL)
                        PS = 0;

                    if (NotProd > 0 && NotProd == ALL - No)
                        PS = 1;
                    else
                    {
                        if (Prod > 0)
                            PS = 2;
                        else
                        {
                            if (Stor > 0)
                                PS = 3;
                            else
                            {
                                if (Exp > 0)
                                    PS = 6;
                                else
                                {
                                    if (Disp > 0)
                                        PS = 4;
                                }
                            }
                        }
                    }

                    if (OnProd > 0 && OnProd == ALL - Disp - Exp - Stor - Prod - NotProd - No)
                        PS = 5;

                    No = 0;
                    NotProd = 0;
                    Prod = 0;
                    Stor = 0;
                    Disp = 0;
                    Exp = 0;
                    OnProd = 0;

                    //TPSStatus
                    for (int i = 0; i < TMOST.Count(); i++)
                    {
                        if (TMOST[i] == 0)
                            No++;
                        if (TMOST[i] == 1)
                            NotProd++;
                        if (TMOST[i] == 2)
                            Prod++;
                        if (TMOST[i] == 3)
                            Stor++;
                        if (TMOST[i] == 6)
                            Exp++;
                        if (TMOST[i] == 5)
                            OnProd++;
                        if (TMOST[i] == 4)
                            Disp++;
                    }

                    if (No == ALL)
                        TS = 0;

                    if (NotProd > 0 && NotProd == ALL - No)
                        TS = 1;
                    else
                    {
                        if (Prod > 0)
                            TS = 2;
                        else
                        {
                            if (Stor > 0)
                                TS = 3;
                            else
                            {
                                if (Exp > 0)
                                    TS = 6;
                                else
                                {
                                    if (Disp > 0)
                                        TS = 4;
                                }
                            }
                        }
                    }

                    if (OnProd > 0 && OnProd == ALL - Disp - Exp - Stor - Prod - NotProd - No)
                        TS = 5;
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////
            // 3) Определение статуса всего заказа (Profil + TPS)

            //0	Не в производстве
            //1	В производстве
            //2	На складе
            //3	Отгружен
            //4	На производстве
            //5	Экспедиция

            //           0	1	0              //           2	3	0
            //           0	1	1              //           2	3	3
            //           1	1	2              //           2	3	4
            //           1	1	3              //           2	3	5
            //           1	1	4              //           2	3	6
            //           1	1	5              //           3	4	0
            //           1	1	6              //           3	4	4
            //           1	2	0              //           4	4	5
            //           1	2	2              //           5	4	6
            //           1	2	3              //           4	5	0
            //           1	2	4              //           4	5	5
            //           1	2	5              //           5	5	6
            //           1	2	6              //           5	6	0
            //           5	6	6

            if (PS == -1)
                PS = 1;
            if (TS == -1)
                TS = 1;
            if ((PS == 1 && TS == 1) || (PS == 0 && TS == 1) || (PS == 1 && TS == 0))
                MegaOrderStatusID = 0;
            if ((PS == 3 && TS == 0) || (PS == 3 && TS == 4) || (PS == 3 && TS == 5) || (PS == 3 && TS == 6) ||
                (PS == 3 && TS == 3) ||
                (TS == 3 && PS == 0) || (TS == 3 && PS == 4) || (TS == 3 && PS == 5) || (TS == 3 && PS == 6))
                MegaOrderStatusID = 2;
            if ((PS == 4 && TS == 4) || (PS == 0 && TS == 4) || (PS == 4 && TS == 0))
                MegaOrderStatusID = 3;
            if ((PS == 1 && TS == 2) || (PS == 1 && TS == 3) || (PS == 1 && TS == 4) || (PS == 1 && TS == 5) ||
                (PS == 1 && TS == 6) ||
                (TS == 1 && PS == 2) || (TS == 1 && PS == 3) || (TS == 1 && PS == 4) || (TS == 1 && PS == 5) ||
                (TS == 1 && PS == 6) ||
                (PS == 2 && TS == 0) || (PS == 2 && TS == 3) || (PS == 2 && TS == 4) || (PS == 2 && TS == 5) ||
                (PS == 2 && TS == 6) || (PS == 2 && TS == 2) ||
                (TS == 2 && PS == 0) || (TS == 2 && PS == 3) || (TS == 2 && PS == 4) || (TS == 2 && PS == 5) ||
                (TS == 2 && PS == 6))
                MegaOrderStatusID = 1;
            if ((PS == 4 && TS == 5) || (PS == 5 && TS == 0) || (PS == 5 && TS == 5) ||
                (TS == 4 && PS == 5) || (TS == 5 && PS == 0))
                MegaOrderStatusID = 4;
            if ((PS == 4 && TS == 6) || (PS == 5 && TS == 6) || (PS == 6 && TS == 0) || (PS == 6 && TS == 6) ||
                (TS == 4 && PS == 6) || (TS == 5 && PS == 6) || (TS == 6 && PS == 0))
                MegaOrderStatusID = 5;
            if (MegaOrderStatusID == -1)
                MegaOrderStatusID = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT MegaOrderID, ProfilOrderStatusID, TPSOrderStatusID, OrderStatusID" +
                " FROM NewMegaOrders WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            DT.Rows[0]["ProfilOrderStatusID"] = PS;
                            DT.Rows[0]["TPSOrderStatusID"] = TS;
                            DT.Rows[0]["OrderStatusID"] = MegaOrderStatusID;

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        static public void SetMegaOrderStatus(int MegaOrderID)
        {
            int MegaOrderStatusID = -1;
            int PS = -1;
            int TS = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT MainOrderID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID," +
                " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID" +
                " FROM MainOrders WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {

                    DA.Fill(DT);

                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //1) Определение статуса каждого подзаказа отдельно для профиля и тпс

                    int[] PMOST = new int[DT.Rows.Count];
                    int[] TMOST = new int[DT.Rows.Count];

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        int ProfilProductionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]);
                        int ProfilStorageStatusID = Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]);
                        int ProfilExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]);
                        int ProfilDispatchStatusID = Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]);

                        int TPSProductionStatusID = Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]);
                        int TPSStorageStatusID = Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]);
                        int TPSExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]);
                        int TPSDispatchStatusID = Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]);

                        //0	нет производства
                        //1	не был в производстве
                        //2	в производстве
                        //3	на складе
                        //4	отгружен
                        //5	на производстве
                        //6	экспедиция

                        //      Pr     St    Exp   Disp
                        //0      0 	   0     0	    0
                        //1      1 	   1     1	    1
                        //4      1 	   1     1	    2
                        //6      1 	   1     2	    1
                        //6      1 	   1     2	    2
                        //3      1 	   2     1	    1
                        //3      1 	   2     1	    2
                        //3      1 	   2     2	    1
                        //3      1 	   2     2	    2
                        //2      2 	   1     1	    1
                        //2      2 	   1     1	    2
                        //2      2 	   1     2	    1
                        //2      2 	   1     2	    2
                        //2      2 	   2     1	    1
                        //2      2 	   2     1	    2
                        //2      2 	   2     2	    1
                        //2      2 	   2     2	    2
                        //5      3 	   1     1	    1

                        //PROFIL
                        if (ProfilProductionStatusID == 0 && ProfilStorageStatusID == 0 &&
                            ProfilExpeditionStatusID == 0 && ProfilDispatchStatusID == 0)
                            PMOST[i] = 0;
                        if (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 1 &&
                            ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1)
                            PMOST[i] = 1;
                        if (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 1 &&
                            ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 2)
                            PMOST[i] = 4;
                        if ((ProfilProductionStatusID == 1 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 2))
                            PMOST[i] = 6;
                        if ((ProfilProductionStatusID == 1 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 2) ||
                            (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 2))
                            PMOST[i] = 3;
                        if ((ProfilProductionStatusID == 2 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 2) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 2) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 2) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 2))
                            PMOST[i] = 2;
                        if (ProfilProductionStatusID == 3 && ProfilStorageStatusID == 1 &&
                            ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1)
                            PMOST[i] = 5;

                        //TPS
                        if (TPSProductionStatusID == 0 && TPSStorageStatusID == 0 && TPSExpeditionStatusID == 0 &&
                            TPSDispatchStatusID == 0)
                            TMOST[i] = 0;
                        if (TPSProductionStatusID == 1 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                            TPSDispatchStatusID == 1)
                            TMOST[i] = 1;
                        if (TPSProductionStatusID == 1 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                            TPSDispatchStatusID == 2)
                            TMOST[i] = 4;
                        if ((TPSProductionStatusID == 1 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 1 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 2))
                            TMOST[i] = 6;
                        if ((TPSProductionStatusID == 1 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 1 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 2) ||
                            (TPSProductionStatusID == 1 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 1 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 2))
                            TMOST[i] = 3;
                        if ((TPSProductionStatusID == 2 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 2) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 2) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 2) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 2))
                            TMOST[i] = 2;
                        if (TPSProductionStatusID == 3 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                            TPSDispatchStatusID == 1)
                            TMOST[i] = 5;
                    }

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //2) Определение статуса заказа по фирмам

                    int No = 0;
                    int NotProd = 0;
                    int Prod = 0;
                    int OnProd = 0;
                    int Stor = 0;
                    int Exp = 0;
                    int Disp = 0;
                    int ALL = PMOST.Count();

                    //ProfilStatus
                    for (int i = 0; i < PMOST.Count(); i++)
                    {
                        if (PMOST[i] == 0)
                            No++;
                        if (PMOST[i] == 1)
                            NotProd++;
                        if (PMOST[i] == 2)
                            Prod++;
                        if (PMOST[i] == 3)
                            Stor++;
                        if (PMOST[i] == 6)
                            Exp++;
                        if (PMOST[i] == 5)
                            OnProd++;
                        if (PMOST[i] == 4)
                            Disp++;
                    }

                    if (No == ALL)
                        PS = 0;

                    if (NotProd > 0 && NotProd == ALL - No)
                        PS = 1;
                    else
                    {
                        if (Prod > 0)
                            PS = 2;
                        else
                        {
                            if (Stor > 0)
                                PS = 3;
                            else
                            {
                                if (Exp > 0)
                                    PS = 6;
                                else
                                {
                                    if (Disp > 0)
                                        PS = 4;
                                }
                            }
                        }
                    }

                    if (OnProd > 0 && OnProd == ALL - Disp - Exp - Stor - Prod - NotProd - No)
                        PS = 5;

                    No = 0;
                    NotProd = 0;
                    Prod = 0;
                    Stor = 0;
                    Disp = 0;
                    Exp = 0;
                    OnProd = 0;

                    //TPSStatus
                    for (int i = 0; i < TMOST.Count(); i++)
                    {
                        if (TMOST[i] == 0)
                            No++;
                        if (TMOST[i] == 1)
                            NotProd++;
                        if (TMOST[i] == 2)
                            Prod++;
                        if (TMOST[i] == 3)
                            Stor++;
                        if (TMOST[i] == 6)
                            Exp++;
                        if (TMOST[i] == 5)
                            OnProd++;
                        if (TMOST[i] == 4)
                            Disp++;
                    }

                    if (No == ALL)
                        TS = 0;

                    if (NotProd > 0 && NotProd == ALL - No)
                        TS = 1;
                    else
                    {
                        if (Prod > 0)
                            TS = 2;
                        else
                        {
                            if (Stor > 0)
                                TS = 3;
                            else
                            {
                                if (Exp > 0)
                                    TS = 6;
                                else
                                {
                                    if (Disp > 0)
                                        TS = 4;
                                }
                            }
                        }
                    }

                    if (OnProd > 0 && OnProd == ALL - Disp - Exp - Stor - Prod - NotProd - No)
                        TS = 5;
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT MainOrderID, ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID," +
                " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID" +
                " FROM NewMainOrders WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {

                    DA.Fill(DT);

                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //1) Определение статуса каждого подзаказа отдельно для профиля и тпс

                    int[] PMOST = new int[DT.Rows.Count];
                    int[] TMOST = new int[DT.Rows.Count];

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        int ProfilProductionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]);
                        int ProfilStorageStatusID = Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]);
                        int ProfilExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]);
                        int ProfilDispatchStatusID = Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]);

                        int TPSProductionStatusID = Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]);
                        int TPSStorageStatusID = Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]);
                        int TPSExpeditionStatusID = Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]);
                        int TPSDispatchStatusID = Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]);

                        //0	нет производства
                        //1	не был в производстве
                        //2	в производстве
                        //3	на складе
                        //4	отгружен
                        //5	на производстве
                        //6	экспедиция

                        //      Pr     St    Exp   Disp
                        //0      0 	   0     0	    0
                        //1      1 	   1     1	    1
                        //4      1 	   1     1	    2
                        //6      1 	   1     2	    1
                        //6      1 	   1     2	    2
                        //3      1 	   2     1	    1
                        //3      1 	   2     1	    2
                        //3      1 	   2     2	    1
                        //3      1 	   2     2	    2
                        //2      2 	   1     1	    1
                        //2      2 	   1     1	    2
                        //2      2 	   1     2	    1
                        //2      2 	   1     2	    2
                        //2      2 	   2     1	    1
                        //2      2 	   2     1	    2
                        //2      2 	   2     2	    1
                        //2      2 	   2     2	    2
                        //5      3 	   1     1	    1

                        //PROFIL
                        if (ProfilProductionStatusID == 0 && ProfilStorageStatusID == 0 &&
                            ProfilExpeditionStatusID == 0 && ProfilDispatchStatusID == 0)
                            PMOST[i] = 0;
                        if (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 1 &&
                            ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1)
                            PMOST[i] = 1;
                        if (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 1 &&
                            ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 2)
                            PMOST[i] = 4;
                        if ((ProfilProductionStatusID == 1 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 2))
                            PMOST[i] = 6;
                        if ((ProfilProductionStatusID == 1 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 2) ||
                            (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 1 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 2))
                            PMOST[i] = 3;
                        if ((ProfilProductionStatusID == 2 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 2) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 1 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 2) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 2) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 1) ||
                            (ProfilProductionStatusID == 2 && ProfilStorageStatusID == 2 &&
                             ProfilExpeditionStatusID == 2 && ProfilDispatchStatusID == 2))
                            PMOST[i] = 2;
                        if (ProfilProductionStatusID == 3 && ProfilStorageStatusID == 1 &&
                            ProfilExpeditionStatusID == 1 && ProfilDispatchStatusID == 1)
                            PMOST[i] = 5;

                        //TPS
                        if (TPSProductionStatusID == 0 && TPSStorageStatusID == 0 && TPSExpeditionStatusID == 0 &&
                            TPSDispatchStatusID == 0)
                            TMOST[i] = 0;
                        if (TPSProductionStatusID == 1 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                            TPSDispatchStatusID == 1)
                            TMOST[i] = 1;
                        if (TPSProductionStatusID == 1 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                            TPSDispatchStatusID == 2)
                            TMOST[i] = 4;
                        if ((TPSProductionStatusID == 1 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 1 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 2))
                            TMOST[i] = 6;
                        if ((TPSProductionStatusID == 1 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 1 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 2) ||
                            (TPSProductionStatusID == 1 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 1 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 2))
                            TMOST[i] = 3;
                        if ((TPSProductionStatusID == 2 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 2) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 2) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 1 &&
                             TPSDispatchStatusID == 2) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 1) ||
                            (TPSProductionStatusID == 2 && TPSStorageStatusID == 2 && TPSExpeditionStatusID == 2 &&
                             TPSDispatchStatusID == 2))
                            TMOST[i] = 2;
                        if (TPSProductionStatusID == 3 && TPSStorageStatusID == 1 && TPSExpeditionStatusID == 1 &&
                            TPSDispatchStatusID == 1)
                            TMOST[i] = 5;
                    }

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //2) Определение статуса заказа по фирмам

                    int No = 0;
                    int NotProd = 0;
                    int Prod = 0;
                    int OnProd = 0;
                    int Stor = 0;
                    int Exp = 0;
                    int Disp = 0;
                    int ALL = PMOST.Count();

                    //ProfilStatus
                    for (int i = 0; i < PMOST.Count(); i++)
                    {
                        if (PMOST[i] == 0)
                            No++;
                        if (PMOST[i] == 1)
                            NotProd++;
                        if (PMOST[i] == 2)
                            Prod++;
                        if (PMOST[i] == 3)
                            Stor++;
                        if (PMOST[i] == 6)
                            Exp++;
                        if (PMOST[i] == 5)
                            OnProd++;
                        if (PMOST[i] == 4)
                            Disp++;
                    }

                    if (No == ALL)
                        PS = 0;

                    if (NotProd > 0 && NotProd == ALL - No)
                        PS = 1;
                    else
                    {
                        if (Prod > 0)
                            PS = 2;
                        else
                        {
                            if (Stor > 0)
                                PS = 3;
                            else
                            {
                                if (Exp > 0)
                                    PS = 6;
                                else
                                {
                                    if (Disp > 0)
                                        PS = 4;
                                }
                            }
                        }
                    }

                    if (OnProd > 0 && OnProd == ALL - Disp - Exp - Stor - Prod - NotProd - No)
                        PS = 5;

                    No = 0;
                    NotProd = 0;
                    Prod = 0;
                    Stor = 0;
                    Disp = 0;
                    Exp = 0;
                    OnProd = 0;

                    //TPSStatus
                    for (int i = 0; i < TMOST.Count(); i++)
                    {
                        if (TMOST[i] == 0)
                            No++;
                        if (TMOST[i] == 1)
                            NotProd++;
                        if (TMOST[i] == 2)
                            Prod++;
                        if (TMOST[i] == 3)
                            Stor++;
                        if (TMOST[i] == 6)
                            Exp++;
                        if (TMOST[i] == 5)
                            OnProd++;
                        if (TMOST[i] == 4)
                            Disp++;
                    }

                    if (No == ALL)
                        TS = 0;

                    if (NotProd > 0 && NotProd == ALL - No)
                        TS = 1;
                    else
                    {
                        if (Prod > 0)
                            TS = 2;
                        else
                        {
                            if (Stor > 0)
                                TS = 3;
                            else
                            {
                                if (Exp > 0)
                                    TS = 6;
                                else
                                {
                                    if (Disp > 0)
                                        TS = 4;
                                }
                            }
                        }
                    }

                    if (OnProd > 0 && OnProd == ALL - Disp - Exp - Stor - Prod - NotProd - No)
                        TS = 5;
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////
            // 3) Определение статуса всего заказа (Profil + TPS)

            //0	Не в производстве
            //1	В производстве
            //2	На складе
            //3	Отгружен
            //4	На производстве
            //5	Экспедиция

            //           0	1	0              //           2	3	0
            //           0	1	1              //           2	3	3
            //           1	1	2              //           2	3	4
            //           1	1	3              //           2	3	5
            //           1	1	4              //           2	3	6
            //           1	1	5              //           3	4	0
            //           1	1	6              //           3	4	4
            //           1	2	0              //           4	4	5
            //           1	2	2              //           5	4	6
            //           1	2	3              //           4	5	0
            //           1	2	4              //           4	5	5
            //           1	2	5              //           5	5	6
            //           1	2	6              //           5	6	0
            //           5	6	6

            if (PS == -1)
                PS = 1;
            if (TS == -1)
                TS = 1;
            if ((PS == 1 && TS == 1) || (PS == 0 && TS == 1) || (PS == 1 && TS == 0))
                MegaOrderStatusID = 0;
            if ((PS == 3 && TS == 0) || (PS == 3 && TS == 4) || (PS == 3 && TS == 5) || (PS == 3 && TS == 6) ||
                (PS == 3 && TS == 3) ||
                (TS == 3 && PS == 0) || (TS == 3 && PS == 4) || (TS == 3 && PS == 5) || (TS == 3 && PS == 6))
                MegaOrderStatusID = 2;
            if ((PS == 4 && TS == 4) || (PS == 0 && TS == 4) || (PS == 4 && TS == 0))
                MegaOrderStatusID = 3;
            if ((PS == 1 && TS == 2) || (PS == 1 && TS == 3) || (PS == 1 && TS == 4) || (PS == 1 && TS == 5) ||
                (PS == 1 && TS == 6) ||
                (TS == 1 && PS == 2) || (TS == 1 && PS == 3) || (TS == 1 && PS == 4) || (TS == 1 && PS == 5) ||
                (TS == 1 && PS == 6) ||
                (PS == 2 && TS == 0) || (PS == 2 && TS == 3) || (PS == 2 && TS == 4) || (PS == 2 && TS == 5) ||
                (PS == 2 && TS == 6) || (PS == 2 && TS == 2) ||
                (TS == 2 && PS == 0) || (TS == 2 && PS == 3) || (TS == 2 && PS == 4) || (TS == 2 && PS == 5) ||
                (TS == 2 && PS == 6))
                MegaOrderStatusID = 1;
            if ((PS == 4 && TS == 5) || (PS == 5 && TS == 0) || (PS == 5 && TS == 5) ||
                (TS == 4 && PS == 5) || (TS == 5 && PS == 0))
                MegaOrderStatusID = 4;
            if ((PS == 4 && TS == 6) || (PS == 5 && TS == 6) || (PS == 6 && TS == 0) || (PS == 6 && TS == 6) ||
                (TS == 4 && PS == 6) || (TS == 5 && PS == 6) || (TS == 6 && PS == 0))
                MegaOrderStatusID = 5;
            if (MegaOrderStatusID == -1)
                MegaOrderStatusID = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT MegaOrderID, ProfilOrderStatusID, TPSOrderStatusID, OrderStatusID" +
                " FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            DT.Rows[0]["ProfilOrderStatusID"] = PS;
                            DT.Rows[0]["TPSOrderStatusID"] = TS;
                            DT.Rows[0]["OrderStatusID"] = MegaOrderStatusID;

                            DA.Update(DT);
                        }
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT MegaOrderID, ProfilOrderStatusID, TPSOrderStatusID, OrderStatusID" +
                " FROM NewMegaOrders WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                        {
                            DT.Rows[0]["ProfilOrderStatusID"] = PS;
                            DT.Rows[0]["TPSOrderStatusID"] = TS;
                            DT.Rows[0]["OrderStatusID"] = MegaOrderStatusID;

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        static public void CheckAllocPack(bool Marketing, int MegaOrderID)
        {
            string OrdersConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            if (!Marketing)
                OrdersConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            int MainOrderID = 0;

            int FrontsOrdersCount = 0;
            int FrontsPackCount = 0;
            int DecorOrdersCount = 0;
            int DecorPackCount = 0;

            int MainProfilPackCount = 0;
            int MainTPSPackCount = 0;
            int MainFrontsPackageAllocStatus = 0;
            int MainDecorPackageAllocStatus = 0;
            int MainProfilPackAllocStatusID = 0;
            int MainTPSPackAllocStatusID = 0;

            int MegaProfilPackCount = 0;
            int MegaTPSPackCount = 0;
            int MegaProfilPackAllocStatusID = 1;
            int MegaTPSPackAllocStatusID = 1;

            DataTable MegaOrdersDT = new DataTable();
            DataTable MainOrdersDT = new DataTable();
            DataTable FrontsOrdersDT = new DataTable();
            DataTable FrontsPackagesDT = new DataTable();
            DataTable DecorOrdersDT = new DataTable();
            DataTable DecorPackagesDT = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT MainOrderID, FactoryID, Count FROM FrontsOrders WHERE MainOrderID IN " +
                " (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")",
                OrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT MainOrderID, FactoryID, Count FROM DecorOrders WHERE MainOrderID IN " +
                " (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID + ")",
                OrdersConnectionString))
            {
                DA.Fill(DecorOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT PackageDetails.PackageID, Packages.MainOrderID, Packages.FactoryID, Count FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN" +
                " (SELECT PackageID FROM Packages WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " +
                MegaOrderID + ")" +
                " AND ProductType = 0)",
                OrdersConnectionString))
            {
                DA.Fill(FrontsPackagesDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT PackageDetails.PackageID, Packages.MainOrderID, Packages.FactoryID, Count FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN" +
                " (SELECT PackageID FROM Packages WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " +
                MegaOrderID + ")" +
                " AND ProductType = 1)",
                OrdersConnectionString))
            {
                DA.Fill(DecorPackagesDT);
            }

            #region MainOrders

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT MainOrderID, FactoryID, ProfilPackAllocStatusID, TPSPackAllocStatusID," +
                " ProfilPackCount, TPSPackCount FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID,
                OrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    if (DA.Fill(MainOrdersDT) > 0)
                    {
                        for (int i = 0; i < MainOrdersDT.Rows.Count; i++)
                        {
                            MainOrderID = Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]);
                            FrontsOrdersCount = 0;
                            FrontsPackCount = 0;
                            DecorOrdersCount = 0;
                            DecorPackCount = 0;
                            MainProfilPackCount = 0;
                            MainTPSPackCount = 0;
                            MainFrontsPackageAllocStatus = 0;
                            MainDecorPackageAllocStatus = 0;
                            MainProfilPackAllocStatusID = 0;
                            MainTPSPackAllocStatusID = 0;

                            #region Profil

                            if (Convert.ToInt32(MainOrdersDT.Rows[i]["FactoryID"]) == 1)
                            {
                                DataRow[] ProfilFProws =
                                    FrontsPackagesDT.Select("FactoryID = 1 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in ProfilFProws)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        FrontsPackCount += Convert.ToInt32(row["Count"]);
                                }

                                DataRow[] ProfilFOrows =
                                    FrontsOrdersDT.Select("FactoryID = 1 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in ProfilFProws)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        FrontsOrdersCount += Convert.ToInt32(row["Count"]);
                                }

                                if (FrontsPackCount == 0)
                                    MainFrontsPackageAllocStatus = 0;
                                else
                                {
                                    if (FrontsPackCount == FrontsOrdersCount)
                                        MainFrontsPackageAllocStatus = 2;
                                    else
                                        MainFrontsPackageAllocStatus = 1;
                                }

                                DataRow[] ProfilDProws =
                                    DecorPackagesDT.Select("FactoryID = 1 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in ProfilDProws)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        DecorPackCount += Convert.ToInt32(row["Count"]);
                                }

                                DataRow[] ProfilDOrows =
                                    DecorOrdersDT.Select("FactoryID = 1 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in ProfilDOrows)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        DecorOrdersCount += Convert.ToInt32(row["Count"]);
                                }

                                if (DecorPackCount == 0)
                                    MainDecorPackageAllocStatus = 0;
                                else
                                {
                                    if (DecorPackCount == DecorOrdersCount)
                                        MainDecorPackageAllocStatus = 2;
                                    else
                                        MainDecorPackageAllocStatus = 1;
                                }

                                if (FrontsOrdersCount > 0 && DecorOrdersCount > 0)
                                {
                                    if (MainFrontsPackageAllocStatus == MainDecorPackageAllocStatus)
                                        MainProfilPackAllocStatusID = MainFrontsPackageAllocStatus;
                                    else
                                        MainProfilPackAllocStatusID = 1;
                                }
                                else
                                {
                                    if (FrontsOrdersCount > 0)
                                        MainProfilPackAllocStatusID = MainFrontsPackageAllocStatus;
                                    if (DecorOrdersCount > 0)
                                        MainProfilPackAllocStatusID = MainDecorPackageAllocStatus;
                                }

                                MainProfilPackCount = GetMainOrderPackCount(Marketing, MainOrderID, 1);
                                MainOrdersDT.Rows[i]["ProfilPackCount"] = MainProfilPackCount;
                                MainOrdersDT.Rows[i]["ProfilPackAllocStatusID"] = MainProfilPackAllocStatusID;
                            }

                            #endregion

                            #region TPS

                            if (Convert.ToInt32(MainOrdersDT.Rows[i]["FactoryID"]) == 2)
                            {
                                DataRow[] TPSFProws =
                                    FrontsPackagesDT.Select("FactoryID = 2 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in TPSFProws)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        FrontsPackCount += Convert.ToInt32(row["Count"]);
                                }

                                DataRow[] TPSFOrows =
                                    FrontsOrdersDT.Select("FactoryID = 2 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in TPSFProws)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        FrontsOrdersCount += Convert.ToInt32(row["Count"]);
                                }

                                if (FrontsPackCount == 0)
                                    MainFrontsPackageAllocStatus = 0;
                                else
                                {
                                    if (FrontsPackCount == FrontsOrdersCount)
                                        MainFrontsPackageAllocStatus = 2;
                                    else
                                        MainFrontsPackageAllocStatus = 1;
                                }

                                DataRow[] TPSDProws =
                                    DecorPackagesDT.Select("FactoryID = 2 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in TPSDProws)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        DecorPackCount += Convert.ToInt32(row["Count"]);
                                }

                                DataRow[] TPSDOrows =
                                    DecorOrdersDT.Select("FactoryID = 2 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in TPSDProws)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        DecorOrdersCount += Convert.ToInt32(row["Count"]);
                                }

                                if (DecorPackCount == 0)
                                    MainDecorPackageAllocStatus = 0;
                                else
                                {
                                    if (DecorPackCount == DecorOrdersCount)
                                        MainDecorPackageAllocStatus = 2;
                                    else
                                        MainDecorPackageAllocStatus = 1;
                                }

                                if (FrontsOrdersCount > 0 && DecorOrdersCount > 0)
                                {
                                    if (MainFrontsPackageAllocStatus == MainDecorPackageAllocStatus)
                                        MainTPSPackAllocStatusID = MainFrontsPackageAllocStatus;
                                    else
                                        MainTPSPackAllocStatusID = 1;
                                }
                                else
                                {
                                    if (FrontsOrdersCount > 0)
                                        MainTPSPackAllocStatusID = MainFrontsPackageAllocStatus;
                                    if (DecorOrdersCount > 0)
                                        MainTPSPackAllocStatusID = MainDecorPackageAllocStatus;
                                }

                                MainTPSPackCount = GetMainOrderPackCount(Marketing, MainOrderID, 2);
                                MainOrdersDT.Rows[i]["TPSPackCount"] = MainTPSPackCount;
                                MainOrdersDT.Rows[i]["TPSPackAllocStatusID"] = MainTPSPackAllocStatusID;
                            }

                            #endregion

                            #region Profil + TPS

                            if (Convert.ToInt32(MainOrdersDT.Rows[i]["FactoryID"]) == 0)
                            {
                                // ПРОФИЛЬ
                                DataRow[] ProfilFProws =
                                    FrontsPackagesDT.Select("FactoryID = 1 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in ProfilFProws)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        FrontsPackCount += Convert.ToInt32(row["Count"]);
                                }

                                DataRow[] ProfilFOrows =
                                    FrontsOrdersDT.Select("FactoryID = 1 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in ProfilFProws)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        FrontsOrdersCount += Convert.ToInt32(row["Count"]);
                                }

                                if (FrontsPackCount == 0)
                                    MainFrontsPackageAllocStatus = 0;
                                else
                                {
                                    if (FrontsPackCount == FrontsOrdersCount)
                                        MainFrontsPackageAllocStatus = 2;
                                    else
                                        MainFrontsPackageAllocStatus = 1;
                                }

                                DataRow[] ProfilDProws =
                                    DecorPackagesDT.Select("FactoryID = 1 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in ProfilDProws)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        DecorPackCount += Convert.ToInt32(row["Count"]);
                                }

                                DataRow[] ProfilDOrows =
                                    DecorOrdersDT.Select("FactoryID = 1 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in ProfilDOrows)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        DecorOrdersCount += Convert.ToInt32(row["Count"]);
                                }

                                if (DecorPackCount == 0)
                                    MainDecorPackageAllocStatus = 0;
                                else
                                {
                                    if (DecorPackCount == DecorOrdersCount)
                                        MainDecorPackageAllocStatus = 2;
                                    else
                                        MainDecorPackageAllocStatus = 1;
                                }

                                if (FrontsOrdersCount > 0 && DecorOrdersCount > 0)
                                {
                                    if (MainFrontsPackageAllocStatus == MainDecorPackageAllocStatus)
                                        MainProfilPackAllocStatusID = MainFrontsPackageAllocStatus;
                                    else
                                        MainProfilPackAllocStatusID = 1;
                                }
                                else
                                {
                                    if (FrontsOrdersCount > 0)
                                        MainProfilPackAllocStatusID = MainFrontsPackageAllocStatus;
                                    if (DecorOrdersCount > 0)
                                        MainProfilPackAllocStatusID = MainDecorPackageAllocStatus;
                                }

                                MainProfilPackCount = GetMainOrderPackCount(Marketing, MainOrderID, 1);
                                MainOrdersDT.Rows[i]["ProfilPackCount"] = MainProfilPackCount;
                                MainOrdersDT.Rows[i]["ProfilPackAllocStatusID"] = MainProfilPackAllocStatusID;

                                // ТПС
                                DataRow[] TPSFProws =
                                    FrontsPackagesDT.Select("FactoryID = 2 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in TPSFProws)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        FrontsPackCount += Convert.ToInt32(row["Count"]);
                                }

                                DataRow[] TPSFOrows =
                                    FrontsOrdersDT.Select("FactoryID = 2 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in TPSFProws)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        FrontsOrdersCount += Convert.ToInt32(row["Count"]);
                                }

                                if (FrontsPackCount == 0)
                                    MainFrontsPackageAllocStatus = 0;
                                else
                                {
                                    if (FrontsPackCount == FrontsOrdersCount)
                                        MainFrontsPackageAllocStatus = 2;
                                    else
                                        MainFrontsPackageAllocStatus = 1;
                                }

                                DataRow[] TPSDProws =
                                    DecorPackagesDT.Select("FactoryID = 2 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in TPSDProws)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        DecorPackCount += Convert.ToInt32(row["Count"]);
                                }

                                DataRow[] TPSDOrows =
                                    DecorOrdersDT.Select("FactoryID = 2 AND MainOrderID = " + MainOrderID);
                                foreach (DataRow row in TPSDProws)
                                {
                                    if (row["Count"] != DBNull.Value)
                                        DecorOrdersCount += Convert.ToInt32(row["Count"]);
                                }

                                if (DecorPackCount == 0)
                                    MainDecorPackageAllocStatus = 0;
                                else
                                {
                                    if (DecorPackCount == DecorOrdersCount)
                                        MainDecorPackageAllocStatus = 2;
                                    else
                                        MainDecorPackageAllocStatus = 1;
                                }

                                if (FrontsOrdersCount > 0 && DecorOrdersCount > 0)
                                {
                                    if (MainFrontsPackageAllocStatus == MainDecorPackageAllocStatus)
                                        MainTPSPackAllocStatusID = MainFrontsPackageAllocStatus;
                                    else
                                        MainTPSPackAllocStatusID = 1;
                                }
                                else
                                {
                                    if (FrontsOrdersCount > 0)
                                        MainTPSPackAllocStatusID = MainFrontsPackageAllocStatus;
                                    if (DecorOrdersCount > 0)
                                        MainTPSPackAllocStatusID = MainDecorPackageAllocStatus;
                                }

                                MainTPSPackCount = GetMainOrderPackCount(Marketing, MainOrderID, 2);
                                MainOrdersDT.Rows[i]["TPSPackCount"] = MainTPSPackCount;
                                MainOrdersDT.Rows[i]["TPSPackAllocStatusID"] = MainTPSPackAllocStatusID;
                            }

                            #endregion

                            MegaProfilPackCount += MainProfilPackCount;
                            MegaTPSPackCount += MainTPSPackCount;
                        }

                        DA.Update(MainOrdersDT);
                    }
                }
            }

            #endregion

            #region MegaOrders

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT MegaOrderID, ProfilPackAllocStatusID, TPSPackAllocStatusID," +
                " ProfilPackCount, TPSPackCount FROM MegaOrders WHERE MegaOrderID = " + MegaOrderID,
                OrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    if (DA.Fill(MegaOrdersDT) > 0)
                    {
                        int ProfilNotPacked = 0;
                        int ProfilPartPacked = 0;
                        int ProfilAllPacked = 0;
                        int TPSNotPacked = 0;
                        int TPSPartPacked = 0;
                        int TPSAllPacked = 0;

                        DataRow[] ProfilRows = MainOrdersDT.Select("FactoryID = 1 OR FactoryID = 0");
                        foreach (DataRow row in ProfilRows)
                        {
                            if (Convert.ToInt32(row["ProfilPackAllocStatusID"]) == 0)
                                ProfilNotPacked++;
                            if (Convert.ToInt32(row["ProfilPackAllocStatusID"]) == 1)
                                ProfilPartPacked++;
                            if (Convert.ToInt32(row["ProfilPackAllocStatusID"]) == 2)
                                ProfilAllPacked++;
                        }

                        DataRow[] TPSRows = MainOrdersDT.Select("FactoryID = 2 OR FactoryID = 0");
                        foreach (DataRow row in TPSRows)
                        {
                            if (Convert.ToInt32(row["TPSPackAllocStatusID"]) == 0)
                                TPSNotPacked++;
                            if (Convert.ToInt32(row["TPSPackAllocStatusID"]) == 1)
                                TPSPartPacked++;
                            if (Convert.ToInt32(row["TPSPackAllocStatusID"]) == 2)
                                TPSAllPacked++;
                        }

                        if (ProfilNotPacked == ProfilRows.Count())
                            MegaProfilPackAllocStatusID = 0;

                        if (ProfilAllPacked == ProfilRows.Count())
                            MegaProfilPackAllocStatusID = 2;

                        if (MegaProfilPackCount == 0)
                            MegaProfilPackAllocStatusID = 0;

                        if (TPSNotPacked == TPSRows.Count())
                            MegaTPSPackAllocStatusID = 0;

                        if (TPSAllPacked == TPSRows.Count())
                            MegaTPSPackAllocStatusID = 2;

                        if (MegaTPSPackCount == 0)
                            MegaTPSPackAllocStatusID = 0;

                        MegaOrdersDT.Rows[0]["ProfilPackAllocStatusID"] = MegaProfilPackAllocStatusID;
                        MegaOrdersDT.Rows[0]["ProfilPackCount"] = MegaProfilPackCount;
                        MegaOrdersDT.Rows[0]["TPSPackAllocStatusID"] = MegaTPSPackAllocStatusID;
                        MegaOrdersDT.Rows[0]["TPSPackCount"] = MegaTPSPackCount;

                        DA.Update(MegaOrdersDT);
                    }
                }
            }

            #endregion

            MegaOrdersDT.Dispose();
            MainOrdersDT.Dispose();
            FrontsOrdersDT.Dispose();
            DecorOrdersDT.Dispose();
            FrontsPackagesDT.Dispose();
            DecorPackagesDT.Dispose();
        }

        static private void CheckPackages(bool Marketing, int MainOrderID)
        {
            string OrdersConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            if (!Marketing)
                OrdersConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails WHERE PackageID IN" +
                                                          " (SELECT PackageID FROM Packages WHERE ProductType = 0 AND MainOrderID = " +
                                                          MainOrderID + ")" +
                                                          " AND OrderID NOT IN (SELECT FrontsOrdersID FROM FrontsOrders WHERE MainOrderID = " +
                                                          MainOrderID + ")",
                OrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            foreach (DataRow row in DT.Rows)
                            {
                                row.Delete();
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails WHERE PackageID IN" +
                                                          " (SELECT PackageID FROM Packages WHERE ProductType = 1 AND MainOrderID = " +
                                                          MainOrderID + ")" +
                                                          " AND OrderID NOT IN (SELECT DecorOrderID FROM DecorOrders WHERE MainOrderID = " +
                                                          MainOrderID + ")",
                OrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            foreach (DataRow row in DT.Rows)
                            {
                                row.Delete();
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages WHERE PackageID NOT IN" +
                                                          " (SELECT PackageID FROM PackageDetails) AND MainOrderID = " +
                                                          MainOrderID,
                OrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            foreach (DataRow row in DT.Rows)
                            {
                                row.Delete();
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        static public void SetStatusMarketingForMainOrder(int MegaOrderID, int MainOrderID)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            SetMainOrderStatus(true, MainOrderID, false);
            SetMegaOrderStatus(MegaOrderID);

            sw.Stop();
            double G = sw.Elapsed.TotalMilliseconds;
        }

        static public void SetStatusMarketingForDispatch(ArrayList DispatchIDs)
        {
            int MegaOrderID = 0;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            ArrayList MegaOrders = GetMegaOrdersInDispatch(DispatchIDs);
            if (MegaOrders.Count == 0)
                return;

            SetMainOrderStatusForDispatch(true, DispatchIDs);

            for (int i = 0; i < MegaOrders.Count; i++)
            {
                MegaOrderID = Convert.ToInt32(MegaOrders[i]);

                SetMegaOrderStatus(MegaOrderID);
            }

            sw.Stop();
            double G = sw.Elapsed.TotalMilliseconds;
        }

        static public void SetStatusZOVForDispatch(ArrayList DispatchIDs)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            SetMainOrderStatusForDispatch(false, DispatchIDs);

            sw.Stop();
            double G = sw.Elapsed.TotalMilliseconds;
        }

        static public void SetStatusMarketingForTray(int TrayID)
        {
            int MegaOrderID = 0;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            ArrayList MegaOrders = GetMegaOrdersInTray(TrayID);
            if (MegaOrders.Count == 0)
                return;

            SetMainOrderStatus(true, TrayID, true);

            for (int i = 0; i < MegaOrders.Count; i++)
            {
                MegaOrderID = Convert.ToInt32(MegaOrders[i]);

                SetMegaOrderStatus(MegaOrderID);
            }

            sw.Stop();
            double G = sw.Elapsed.TotalMilliseconds;
        }

        static public void SetStatusZOV(int MainOrderID, bool IsTray)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //Если сканируется поддон на отгрузке
            if (IsTray)
            {
                SetMainOrderStatus(false, MainOrderID, IsTray);
            }
            else
            {
                //CheckPackages(MainOrderID);
                SetMainOrderStatus(false, MainOrderID, IsTray);
                //CheckAllocPack(MegaOrderID);
            }

            sw.Stop();
            double G = sw.Elapsed.Milliseconds;
        }

        static public void DispatchMegaOrder(int MegaOrderID)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            SetMainOrderDispatch(Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                        }

                        SetMegaOrderStatus(MegaOrderID);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT MainOrderID FROM NewMainOrders WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            SetMainOrderDispatch(Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                        }

                        SetMegaOrderStatus(MegaOrderID);
                    }
                }
            }

            sw.Stop();
            double G = sw.Elapsed.Milliseconds;
        }

        static public void GG(int MegaOrderID)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            CheckPackages(true, Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                            SetMainOrderStatus(true, Convert.ToInt32(DT.Rows[i]["MainOrderID"]), false);
                        }

                        SetMegaOrderStatus(MegaOrderID);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(
                "SELECT MainOrderID FROM NewMainOrders WHERE MegaOrderID = " + MegaOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            CheckPackages(true, Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                            SetMainOrderStatus(true, Convert.ToInt32(DT.Rows[i]["MainOrderID"]), false);
                        }

                        SetMegaOrderStatus(MegaOrderID);
                    }
                }
            }

            sw.Stop();
            double G = sw.Elapsed.Milliseconds;
        }

        static public void FilterMainOrdersByMegaOrder(int MegaOrderID,
            bool NotProduction,
            bool InProduction,
            bool OnProduction,
            bool OnStorage,
            bool Dispatched,
            int FactoryID)
        {
            string FactoryFilter = string.Empty;
            string OrdersProductionStatus = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND (FactoryID = 0 OR FactoryID = " + FactoryID + ")";

            #region Orders

            if (NotProduction)
            {
                OrdersProductionStatus =
                    "((ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)" +
                    " OR (TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1))";

                if (FactoryID == 1)
                    OrdersProductionStatus =
                        "(ProfilProductionStatusID=1 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                if (FactoryID == 2)
                    OrdersProductionStatus =
                        "(TPSProductionStatusID=1 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
            }

            if (InProduction)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSProductionStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilProductionStatusID=2 OR TPSProductionStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilProductionStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSProductionStatusID=2)";
                }
            }

            if (OnProduction)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";

                    if (FactoryID == 1)
                        OrdersProductionStatus +=
                            " OR (ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus +=
                            " OR (TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilProductionStatusID=3 OR TPSProductionStatusID=3)";

                    if (FactoryID == 1)
                        OrdersProductionStatus =
                            "(ProfilProductionStatusID=3 AND ProfilStorageStatusID=1 AND ProfilDispatchStatusID=1)";
                    if (FactoryID == 2)
                        OrdersProductionStatus =
                            "(TPSProductionStatusID=3 AND TPSStorageStatusID=1 AND TPSDispatchStatusID=1)";
                }
            }

            if (OnStorage)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSStorageStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilStorageStatusID=2 OR TPSStorageStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilStorageStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSStorageStatusID=2)";
                }
            }

            if (Dispatched)
            {
                if (OrdersProductionStatus.Length > 0)
                {
                    OrdersProductionStatus += " OR (ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus += " OR (ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus += " OR (TPSDispatchStatusID=2)";
                }
                else
                {
                    OrdersProductionStatus = "(ProfilDispatchStatusID=2 OR TPSDispatchStatusID=2)";

                    if (FactoryID == 1)
                        OrdersProductionStatus = "(ProfilDispatchStatusID=2)";
                    if (FactoryID == 2)
                        OrdersProductionStatus = "(TPSDispatchStatusID=2)";
                }
            }

            if (OrdersProductionStatus.Length > 0)
                OrdersProductionStatus = " AND (" + OrdersProductionStatus + ")";

            #endregion

            string SelectionCommand = "SELECT * FROM MainOrders" +
                                      " WHERE MegaOrderID = " + MegaOrderID + FactoryFilter + OrdersProductionStatus;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        CheckPackages(false, Convert.ToInt32(DT.Rows[i]["MainOrderID"]));
                        SetMainOrderStatus(false, Convert.ToInt32(DT.Rows[i]["MainOrderID"]), false);
                    }
                }
            }
        }

        static public void SetToDispatch(DateTime date, int[] Packages, int MainOrderID)
        {
            bool HasPackages = false;
            if (Packages.Count() > 0)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(
                    "SELECT PackageID, PackageStatusID, PackingDateTime, StorageDateTime, ExpUserID, ExpeditionDateTime, DispUserID, DispatchDateTime FROM Packages" +
                    " WHERE PackageID IN (" + string.Join(",", Packages) + ")",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                HasPackages = true;
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    DT.Rows[i]["PackageStatusID"] = 3;
                                    if (DT.Rows[i]["PackingDateTime"] == DBNull.Value)
                                        DT.Rows[i]["PackingDateTime"] = date;
                                    if (DT.Rows[i]["StorageDateTime"] == DBNull.Value)
                                        DT.Rows[i]["StorageDateTime"] = date;
                                    if (DT.Rows[i]["ExpeditionDateTime"] == DBNull.Value)
                                    {
                                        DT.Rows[i]["ExpeditionDateTime"] = date;
                                        DT.Rows[i]["ExpUserID"] = 380;
                                    }

                                    if (DT.Rows[i]["DispatchDateTime"] == DBNull.Value)
                                    {
                                        DT.Rows[i]["DispatchDateTime"] = date;
                                        DT.Rows[i]["DispUserID"] = 380;
                                    }

                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }
            }

            DateTime DispDate = Security.GetCurrentDate();
            if (!HasPackages)
            {
                int ProfilProductionStatusID = 0;
                int ProfilStorageStatusID = 0;
                int ProfilExpeditionStatusID = 0;
                int ProfilDispatchStatusID = 0;
                int TPSProductionStatusID = 0;
                int TPSStorageStatusID = 0;
                int TPSExpeditionStatusID = 0;
                int TPSDispatchStatusID = 0;
                int ProfilPackAllocStatusID = 0;
                int TPSPackAllocStatusID = 0;
                string SelectCommand = "SELECT MainOrderID, IsSample, FactoryID," +
                                       " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID, ProfilPackAllocStatusID, " +
                                       " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID, TPSPackAllocStatusID FROM MainOrders WHERE MainOrderID = " +
                                       MainOrderID;

                using (SqlDataAdapter DA =
                    new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    bool IsSample = Convert.ToBoolean(DT.Rows[i]["IsSample"]);
                                    int FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);
                                    if (IsSample)
                                    {
                                        CopySampleOrders(true, Convert.ToInt32(DT.Rows[i]["MainOrderID"]), DispDate);
                                    }

                                    if (FactoryID == 1)
                                    {
                                        ProfilProductionStatusID = 1;
                                        ProfilStorageStatusID = 1;
                                        ProfilExpeditionStatusID = 1;
                                        ProfilDispatchStatusID = 2;
                                        TPSProductionStatusID = 0;
                                        TPSStorageStatusID = 0;
                                        TPSExpeditionStatusID = 0;
                                        TPSDispatchStatusID = 0;
                                        ProfilPackAllocStatusID = 2;
                                        TPSPackAllocStatusID = 0;
                                    }

                                    if (FactoryID == 2)
                                    {
                                        ProfilProductionStatusID = 0;
                                        ProfilStorageStatusID = 0;
                                        ProfilExpeditionStatusID = 0;
                                        ProfilDispatchStatusID = 0;
                                        TPSProductionStatusID = 1;
                                        TPSStorageStatusID = 1;
                                        TPSExpeditionStatusID = 1;
                                        TPSDispatchStatusID = 2;
                                        ProfilPackAllocStatusID = 0;
                                        TPSPackAllocStatusID = 2;
                                    }

                                    if (FactoryID == 0)
                                    {
                                        ProfilProductionStatusID = 1;
                                        ProfilStorageStatusID = 1;
                                        ProfilExpeditionStatusID = 1;
                                        ProfilDispatchStatusID = 2;
                                        TPSProductionStatusID = 1;
                                        TPSStorageStatusID = 1;
                                        TPSExpeditionStatusID = 1;
                                        TPSDispatchStatusID = 2;
                                        ProfilPackAllocStatusID = 2;
                                        TPSPackAllocStatusID = 2;
                                    }

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilProductionStatusID"] = ProfilProductionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]) != 0)
                                        DT.Rows[i]["ProfilStorageStatusID"] = ProfilStorageStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilExpeditionStatusID"] = ProfilExpeditionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]) != 0)
                                        DT.Rows[i]["ProfilDispatchStatusID"] = ProfilDispatchStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]) != 0)
                                        DT.Rows[i]["TPSProductionStatusID"] = TPSProductionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]) != 0)
                                        DT.Rows[i]["TPSStorageStatusID"] = TPSStorageStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["TPSExpeditionStatusID"] = TPSExpeditionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]) != 0)
                                        DT.Rows[i]["TPSDispatchStatusID"] = TPSDispatchStatusID;
                                    DT.Rows[i]["ProfilPackAllocStatusID"] = ProfilPackAllocStatusID;
                                    DT.Rows[i]["TPSPackAllocStatusID"] = TPSPackAllocStatusID;
                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }

                ProfilProductionStatusID = 0;
                ProfilStorageStatusID = 0;
                ProfilExpeditionStatusID = 0;
                ProfilDispatchStatusID = 0;
                TPSProductionStatusID = 0;
                TPSStorageStatusID = 0;
                TPSExpeditionStatusID = 0;
                TPSDispatchStatusID = 0;
                ProfilPackAllocStatusID = 0;
                TPSPackAllocStatusID = 0;
                SelectCommand = "SELECT MainOrderID, IsSample, FactoryID," +
                                " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID, ProfilPackAllocStatusID, TPSPackAllocStatusID, " +
                                " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID FROM NewMainOrders WHERE MainOrderID = " +
                                MainOrderID;

                using (SqlDataAdapter DA =
                    new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    bool IsSample = Convert.ToBoolean(DT.Rows[i]["IsSample"]);
                                    int FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);
                                    if (IsSample)
                                    {
                                        CopySampleOrders(true, Convert.ToInt32(DT.Rows[i]["MainOrderID"]), DispDate);
                                    }

                                    if (FactoryID == 1)
                                    {
                                        ProfilProductionStatusID = 1;
                                        ProfilStorageStatusID = 1;
                                        ProfilExpeditionStatusID = 1;
                                        ProfilDispatchStatusID = 2;
                                        TPSProductionStatusID = 0;
                                        TPSStorageStatusID = 0;
                                        TPSExpeditionStatusID = 0;
                                        TPSDispatchStatusID = 0;
                                        ProfilPackAllocStatusID = 2;
                                        TPSPackAllocStatusID = 0;
                                    }

                                    if (FactoryID == 2)
                                    {
                                        ProfilProductionStatusID = 0;
                                        ProfilStorageStatusID = 0;
                                        ProfilExpeditionStatusID = 0;
                                        ProfilDispatchStatusID = 0;
                                        TPSProductionStatusID = 1;
                                        TPSStorageStatusID = 1;
                                        TPSExpeditionStatusID = 1;
                                        TPSDispatchStatusID = 2;
                                        ProfilPackAllocStatusID = 0;
                                        TPSPackAllocStatusID = 2;
                                    }

                                    if (FactoryID == 0)
                                    {
                                        ProfilProductionStatusID = 1;
                                        ProfilStorageStatusID = 1;
                                        ProfilExpeditionStatusID = 1;
                                        ProfilDispatchStatusID = 2;
                                        TPSProductionStatusID = 1;
                                        TPSStorageStatusID = 1;
                                        TPSExpeditionStatusID = 1;
                                        TPSDispatchStatusID = 2;
                                        ProfilPackAllocStatusID = 2;
                                        TPSPackAllocStatusID = 2;
                                    }

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilProductionStatusID"] = ProfilProductionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]) != 0)
                                        DT.Rows[i]["ProfilStorageStatusID"] = ProfilStorageStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilExpeditionStatusID"] = ProfilExpeditionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]) != 0)
                                        DT.Rows[i]["ProfilDispatchStatusID"] = ProfilDispatchStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]) != 0)
                                        DT.Rows[i]["TPSProductionStatusID"] = TPSProductionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]) != 0)
                                        DT.Rows[i]["TPSStorageStatusID"] = TPSStorageStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["TPSExpeditionStatusID"] = TPSExpeditionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]) != 0)
                                        DT.Rows[i]["TPSDispatchStatusID"] = TPSDispatchStatusID;
                                    DT.Rows[i]["ProfilPackAllocStatusID"] = ProfilPackAllocStatusID;
                                    DT.Rows[i]["TPSPackAllocStatusID"] = TPSPackAllocStatusID;
                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }
            }
        }

        static public void SetToDispatchZOV(DateTime date, int[] Packages, int MainOrderID)
        {
            bool HasPackages = false;
            if (Packages.Count() > 0)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(
                    "SELECT PackageID, PackageStatusID, PackingDateTime, StorageDateTime, ExpUserID, ExpeditionDateTime, DispUserID, DispatchDateTime FROM Packages" +
                    " WHERE PackageID IN (" + string.Join(",", Packages) + ")",
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                HasPackages = true;
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    DT.Rows[i]["PackageStatusID"] = 3;
                                    if (DT.Rows[i]["PackingDateTime"] == DBNull.Value)
                                        DT.Rows[i]["PackingDateTime"] = date;
                                    if (DT.Rows[i]["StorageDateTime"] == DBNull.Value)
                                        DT.Rows[i]["StorageDateTime"] = date;
                                    if (DT.Rows[i]["ExpeditionDateTime"] == DBNull.Value)
                                    {
                                        DT.Rows[i]["ExpeditionDateTime"] = date;
                                        DT.Rows[i]["ExpUserID"] = 380;
                                    }

                                    if (DT.Rows[i]["DispatchDateTime"] == DBNull.Value)
                                    {
                                        DT.Rows[i]["DispatchDateTime"] = date;
                                        DT.Rows[i]["DispUserID"] = 380;
                                    }

                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }
            }

            DateTime DispDate = Security.GetCurrentDate();
            if (!HasPackages)
            {
                int ProfilProductionStatusID = 0;
                int ProfilStorageStatusID = 0;
                int ProfilExpeditionStatusID = 0;
                int ProfilDispatchStatusID = 0;
                int TPSProductionStatusID = 0;
                int TPSStorageStatusID = 0;
                int TPSExpeditionStatusID = 0;
                int TPSDispatchStatusID = 0;
                string SelectCommand = "SELECT MainOrderID, IsSample, FactoryID," +
                                       " ProfilProductionStatusID, ProfilStorageStatusID, ProfilExpeditionStatusID, ProfilDispatchStatusID, " +
                                       " TPSProductionStatusID, TPSStorageStatusID, TPSExpeditionStatusID, TPSDispatchStatusID FROM MainOrders WHERE MainOrderID = " +
                                       MainOrderID;

                using (SqlDataAdapter DA =
                    new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                for (int i = 0; i < DT.Rows.Count; i++)
                                {
                                    bool IsSample = Convert.ToBoolean(DT.Rows[i]["IsSample"]);
                                    int FactoryID = Convert.ToInt32(DT.Rows[i]["FactoryID"]);
                                    if (IsSample)
                                    {
                                        CopySampleOrders(false, Convert.ToInt32(DT.Rows[i]["MainOrderID"]), DispDate);
                                    }

                                    if (FactoryID == 1)
                                    {
                                        ProfilProductionStatusID = 1;
                                        ProfilStorageStatusID = 1;
                                        ProfilExpeditionStatusID = 1;
                                        ProfilDispatchStatusID = 2;
                                        TPSProductionStatusID = 0;
                                        TPSStorageStatusID = 0;
                                        TPSExpeditionStatusID = 0;
                                        TPSDispatchStatusID = 0;
                                    }

                                    if (FactoryID == 2)
                                    {
                                        ProfilProductionStatusID = 0;
                                        ProfilStorageStatusID = 0;
                                        ProfilExpeditionStatusID = 0;
                                        ProfilDispatchStatusID = 0;
                                        TPSProductionStatusID = 1;
                                        TPSStorageStatusID = 1;
                                        TPSExpeditionStatusID = 1;
                                        TPSDispatchStatusID = 2;
                                    }

                                    if (FactoryID == 0)
                                    {
                                        ProfilProductionStatusID = 1;
                                        ProfilStorageStatusID = 1;
                                        ProfilExpeditionStatusID = 1;
                                        ProfilDispatchStatusID = 2;
                                        TPSProductionStatusID = 1;
                                        TPSStorageStatusID = 1;
                                        TPSExpeditionStatusID = 1;
                                        TPSDispatchStatusID = 2;
                                    }

                                    if (Convert.ToInt32(DT.Rows[i]["ProfilProductionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilProductionStatusID"] = ProfilProductionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilStorageStatusID"]) != 0)
                                        DT.Rows[i]["ProfilStorageStatusID"] = ProfilStorageStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["ProfilExpeditionStatusID"] = ProfilExpeditionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["ProfilDispatchStatusID"]) != 0)
                                        DT.Rows[i]["ProfilDispatchStatusID"] = ProfilDispatchStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSProductionStatusID"]) != 0)
                                        DT.Rows[i]["TPSProductionStatusID"] = TPSProductionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSStorageStatusID"]) != 0)
                                        DT.Rows[i]["TPSStorageStatusID"] = TPSStorageStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSExpeditionStatusID"]) != 0)
                                        DT.Rows[i]["TPSExpeditionStatusID"] = TPSExpeditionStatusID;
                                    if (Convert.ToInt32(DT.Rows[i]["TPSDispatchStatusID"]) != 0)
                                        DT.Rows[i]["TPSDispatchStatusID"] = TPSDispatchStatusID;
                                }

                                DA.Update(DT);
                            }
                        }
                    }
                }
            }
        }

        static public void SetToNotPack(int[] Packages)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages" +
                                                          " WHERE PackageID IN (" + string.Join(",", Packages) + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                DT.Rows[i]["PackageStatusID"] = 0;
                                DT.Rows[i]["PackingDateTime"] = DBNull.Value;
                                DT.Rows[i]["PackUserID"] = DBNull.Value;
                                DT.Rows[i]["StorageDateTime"] = DBNull.Value;
                                DT.Rows[i]["StoreUserID"] = DBNull.Value;
                                DT.Rows[i]["ExpeditionDateTime"] = DBNull.Value;
                                DT.Rows[i]["ExpUserID"] = DBNull.Value;
                                DT.Rows[i]["DispatchDateTime"] = DBNull.Value;
                                DT.Rows[i]["DispUserID"] = DBNull.Value;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        static public void SetToPack(DateTime date, int[] Packages)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages" +
                                                          " WHERE PackageID IN (" + string.Join(",", Packages) + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                DT.Rows[i]["PackageStatusID"] = 1;

                                DT.Rows[i]["PackingDateTime"] = date;
                                DT.Rows[i]["PackUserID"] = 322;
                                DT.Rows[i]["StorageDateTime"] = DBNull.Value;
                                DT.Rows[i]["StoreUserID"] = DBNull.Value;
                                DT.Rows[i]["ExpeditionDateTime"] = DBNull.Value;
                                DT.Rows[i]["ExpUserID"] = DBNull.Value;
                                DT.Rows[i]["DispatchDateTime"] = DBNull.Value;
                                DT.Rows[i]["DispUserID"] = DBNull.Value;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        static public void SetToStorage(DateTime date, int[] Packages)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages" +
                                                          " WHERE PackageID IN (" + string.Join(",", Packages) + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                DT.Rows[i]["PackageStatusID"] = 2;
                                DT.Rows[i]["PackingDateTime"] = date;
                                DT.Rows[i]["StorageDateTime"] = date;
                                DT.Rows[i]["StoreUserID"] = 322;
                                DT.Rows[i]["ExpeditionDateTime"] = DBNull.Value;
                                DT.Rows[i]["ExpUserID"] = DBNull.Value;
                                DT.Rows[i]["DispatchDateTime"] = DBNull.Value;
                                DT.Rows[i]["DispUserID"] = DBNull.Value;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        static public void SetToExpedition(DateTime date, int[] Packages)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Packages" +
                                                          " WHERE PackageID IN (" + string.Join(",", Packages) + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                DT.Rows[i]["PackingDateTime"] = date;
                                DT.Rows[i]["StorageDateTime"] = date;


                                DT.Rows[i]["PackageStatusID"] = 4;
                                DT.Rows[i]["PackingDateTime"] = date;
                                DT.Rows[i]["StorageDateTime"] = date;
                                DT.Rows[i]["ExpeditionDateTime"] = date;
                                DT.Rows[i]["ExpUserID"] = 322;
                                DT.Rows[i]["DispatchDateTime"] = DBNull.Value;
                                DT.Rows[i]["DispUserID"] = DBNull.Value;
                            }

                            DA.Update(DT);
                        }
                    }
                }
            }
        }
    }





    public class Connection
    {
        private string sConnectionString = null;

        public Connection()
        {

        }

        private string ReadConnectionString(String FileName)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(FileName))
            {
                sConnectionString = sr.ReadToEnd();
            }

            return sConnectionString;
        }

        private string EncryptString(string inputString, int dwKeySize, string xmlString)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            int keySize = dwKeySize / 8;
            byte[] bytes = Encoding.UTF32.GetBytes(inputString);

            int maxLength = keySize - 42;
            int dataLength = bytes.Length;
            int iterations = dataLength / maxLength;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i <= iterations; i++)
            {
                byte[] tempBytes = new byte[(dataLength - maxLength * i > maxLength) ? maxLength : dataLength - maxLength * i];
                Buffer.BlockCopy(bytes, maxLength * i, tempBytes, 0, tempBytes.Length);
                byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes, true);

                Array.Reverse(encryptedBytes);

                stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            }
            return stringBuilder.ToString();
        }

        private string DecryptString(string inputString, int dwKeySize, string xmlString)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            int base64BlockSize = ((dwKeySize / 8) % 3 != 0) ? (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;
            int iterations = inputString.Length / base64BlockSize;
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(inputString.Substring(base64BlockSize * i, base64BlockSize));

                Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(encryptedBytes, true));
            }
            return Encoding.UTF32.GetString(arrayList.ToArray(Type.GetType("System.Byte")) as byte[]);
        }

        public string GetConnectionString(string FileName)
        {
            try
            {
                string EncryptedConnectionString = ReadConnectionString(FileName);
                int Index = EncryptedConnectionString.IndexOf("Password");
                string Password = EncryptedConnectionString.Substring(Index + 9, EncryptedConnectionString.Length - Index - 9);
                string privateKey = "<RSAKeyValue><Modulus>pqJ+ilhHSM5N3XGPCAYdrpFjHlvcQQoaNPiTvUHut5dIwx40olIKRjvequY8WeGb</Modulus><Exponent>AQAB</Exponent><P>2UlRsj0mJoeGxD4AtwOEn02oCEjlYifD</P><Q>xFLlKWJuoE3mb88iI8v24/7Qt1Wvc5lJ</Q><DP>s8j4sfPqpyKoHaP3z3Y3u9/zUreOJIMl</DP><DQ>YhIOy8eR/54qeLv+D+e5o1cNKCgzhwmR</DQ><InverseQ>kU9Phb5ynWsB6ZFQoAnUPAzmirRIlqDR</InverseQ><D>J1Q64ZQsXvayUg23YIFxB/6wkj3EImWroC3gjjCvYa+fjojM1XXvE/tE5t8mnnzB</D></RSAKeyValue>";
                sConnectionString = EncryptedConnectionString.Replace(Password, DecryptString(Password, 384, privateKey));
            }
            catch (ArgumentNullException)
            {

            }
            return sConnectionString;
        }

        //public string GetConnectionString(string FileName)
        //{
        //	try
        //	{
        //		string EncryptedConnectionString = ReadConnectionString(FileName);
        //		int Index = EncryptedConnectionString.IndexOf("Password");
        //		string Password = EncryptedConnectionString.Substring(Index + 9, EncryptedConnectionString.Length - Index - 9);
        //		string privateKey = "<RSAKeyValue><Modulus>pqJ+ilhHSM5N3XGPCAYdrpFjHlvcQQoaNPiTvUHut5dIwx40olIKRjvequY8WeGb</Modulus><Exponent>AQAB</Exponent><P>2UlRsj0mJoeGxD4AtwOEn02oCEjlYifD</P><Q>xFLlKWJuoE3mb88iI8v24/7Qt1Wvc5lJ</Q><DP>s8j4sfPqpyKoHaP3z3Y3u9/zUreOJIMl</DP><DQ>YhIOy8eR/54qeLv+D+e5o1cNKCgzhwmR</DQ><InverseQ>kU9Phb5ynWsB6ZFQoAnUPAzmirRIlqDR</InverseQ><D>J1Q64ZQsXvayUg23YIFxB/6wkj3EImWroC3gjjCvYa+fjojM1XXvE/tE5t8mnnzB</D></RSAKeyValue>";
        //		sConnectionString = EncryptedConnectionString.Replace(Password, DecryptString(Password, 384, privateKey));
        //	}
        //	catch (ArgumentNullException)
        //	{

        //	}
        //	return sConnectionString;
        //}
    }





    public struct ConnectionStrings
    {
        public static string CatalogConnectionString;
        public static string LightConnectionString;
        public static string MarketingOrdersConnectionString;
        public static string MarketingReferenceConnectionString;
        public static string StorageConnectionString;
        public static string UsersConnectionString;
        public static string ZOVOrdersConnectionString;
        public static string ZOVReferenceConnectionString;
    }

    public class CommonVariables
    {
        public static String CatalogConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["Infinium.Properties.Settings.CatalogConnectionString"].ConnectionString; }
        }
        public static String LightConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Infinium.Properties.Settings.LightConnectionString"].ConnectionString;
            }
        }
        public static String MarketingOrdersConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Infinium.Properties.Settings.MarketingOrdersConnectionString"].ConnectionString;
            }
        }
        public static String MarketingReferenceConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Infinium.Properties.Settings.MarketingReferenceConnectionString"].ConnectionString;
            }
        }
        public static String StorageConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Infinium.Properties.Settings.StorageConnectionString"].ConnectionString;
            }
        }
        public static String UsersConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Infinium.Properties.Settings.UsersConnectionString"].ConnectionString;
            }
        }
        public static String ZOVOrdersConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Infinium.Properties.Settings.ZOVOrdersConnectionString"].ConnectionString;
            }
        }
        public static String ZOVReferenceConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Infinium.Properties.Settings.ZOVReferenceConnectionString"].ConnectionString;
            }
        }
    }





    public struct Configs
    {
        public static string DocumentsPath;
        public static string DocumentsZOVTPSPath;
        public static string DocumentsPathHost;
        public static int FTPType;
    }





    public class FileManager
    {
        public bool bIsHostingExist = false;
        public Int64 TotalFileSize = 0;
        public Int64 Position = 0;
        public int CurrentSpeed = 0;
        private Int64 iTransData = 0;
        public bool bStopTransfer = false;

        private string ServerFTPLogin = "infinium";
        private string ServerFTPPass = "infinium";
        private string ZOVTPSHostFTPLogin = "infiniu2_infinium";
        private string ZOVTPSHostFTPPass = "vqju]nkca8ygtfibrQop";
        private string HostFTPLogin = "infiniu2_infinium";
        private string HostFTPPass = "vqju]nkca8ygtfibrQop";
        private string LocalFTPLogin = "infinium";
        private string LocalFTPPass = "infinium";
        private string ServerFTP178Login = "infinium";
        private string ServerFTP178Pass = "infinium";

        //private int bFTPDest = Configs.FTPType;//0 - server, 1 - host, 2 - local

        System.Timers.Timer Timer;

        public FileManager()
        {

            Timer = new System.Timers.Timer(300)
            {
                Enabled = false
            };
            Timer.Elapsed += Timer_Elapsed;
        }

        public static string GetIntegerWithThousands(int iValue)
        {
            if (iValue == 0)
                return "0";

            return String.Format("{0:### ### ### ### ### ###}", iValue).TrimStart();
        }

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            CurrentSpeed = Convert.ToInt32((Position - iTransData) * 2) / 1024;
            iTransData = Position;
        }

        public static string GetPath(string DocumentsType)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DocumentsPath FROM DocumentsPath WHERE DocumentsType = '" + DocumentsType + "'", ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return DT.Rows[0]["DocumentsPath"].ToString();
                }
            }
        }

        public bool DownloadFile(string sSourceFileName, string sDestFileName, Int64 FileSize, int FTPType)
        {
            FileStream writeStream = new FileStream(sDestFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

            CurrentSpeed = 0;
            iTransData = 0;
            Position = 0;
            TotalFileSize = 0;

            try
            {
                FtpWebRequest ftpClient = (FtpWebRequest)WebRequest.Create(sSourceFileName);

                if (FTPType == 4)
                    ftpClient.Credentials = new NetworkCredential(ZOVTPSHostFTPLogin, ZOVTPSHostFTPPass);
                if (FTPType == 3)
                    ftpClient.Credentials = new NetworkCredential(ServerFTP178Login, ServerFTP178Pass);
                if (FTPType == 2)
                    ftpClient.Credentials = new NetworkCredential(LocalFTPLogin, LocalFTPPass);
                if (FTPType == 1)
                    ftpClient.Credentials = new NetworkCredential(HostFTPLogin, HostFTPPass);
                if (FTPType == 0)
                    ftpClient.Credentials = new NetworkCredential(ServerFTPLogin, ServerFTPPass);

                ftpClient.UseBinary = true;
                ftpClient.KeepAlive = true;

                TotalFileSize = FileSize;

                ftpClient.Method = WebRequestMethods.Ftp.DownloadFile;
                FtpWebResponse response = (FtpWebResponse)ftpClient.GetResponse();

                //write file
                Stream responseStream = response.GetResponseStream();

                Timer.Enabled = true;

                Byte[] buffer = new Byte[4096];
                int bytesRead = responseStream.Read(buffer, 0, 1);
                Position += bytesRead;

                while (bytesRead > 0)
                {
                    if (bStopTransfer)
                    {
                        bStopTransfer = false;
                        ftpClient.Abort();
                        writeStream.Close();
                        response.Close();
                        responseStream.Close();
                        break;
                    }

                    writeStream.Write(buffer, 0, bytesRead);
                    bytesRead = responseStream.Read(buffer, 0, Convert.ToInt32(buffer.Length));
                    Position += bytesRead;
                }

                Position = TotalFileSize;
                Timer.Enabled = false;
                ftpClient.Abort();
                response.Close();
                responseStream.Close();
            }
            catch
            {
                Timer.Enabled = false;
                writeStream.Close();
                bStopTransfer = false;
                GC.Collect();
                return false;
            }

            writeStream.Close();
            return true;
        }

        public bool UploadFile(string sSourceFileName, string sDestFileName, int FTPType)
        {
            CurrentSpeed = 0;
            iTransData = 0;
            Position = 0;
            TotalFileSize = 0;

            try
            {
                FtpWebRequest ftpClient = (FtpWebRequest)WebRequest.Create(sDestFileName);

                if (FTPType == 4)
                    ftpClient.Credentials = new NetworkCredential(ZOVTPSHostFTPLogin, ZOVTPSHostFTPPass);
                if (FTPType == 3)
                    ftpClient.Credentials = new NetworkCredential(ServerFTP178Login, ServerFTP178Pass);
                if (FTPType == 2)
                    ftpClient.Credentials = new NetworkCredential(LocalFTPLogin, LocalFTPPass);
                if (FTPType == 1)
                    ftpClient.Credentials = new NetworkCredential(HostFTPLogin, HostFTPPass);
                if (FTPType == 0)
                    ftpClient.Credentials = new NetworkCredential(ServerFTPLogin, ServerFTPPass);

                ftpClient.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                ftpClient.UseBinary = true;
                ftpClient.KeepAlive = true;

                System.IO.FileInfo fi = new System.IO.FileInfo(sSourceFileName);
                ftpClient.ContentLength = fi.Length;
                TotalFileSize = fi.Length;

                byte[] buffer = new byte[4097];
                int bytes = 0;
                int total_bytes = (int)fi.Length;

                System.IO.FileStream fs = null;

                try
                {
                    fs = fi.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
                catch
                {
                    return false;
                }
                System.IO.Stream rs;
                try
                {
                    rs = ftpClient.GetRequestStream();
                }

                catch (Exception ex)
                {
                    string m = ex.Message;
                    return false;
                }
                Timer.Enabled = true;

                while (total_bytes > 0)
                {
                    if (bStopTransfer)
                    {
                        bStopTransfer = false;
                        ftpClient.Abort();
                        rs.Close();
                        fs.Close();
                        Timer.Enabled = false;
                        return false;
                    }

                    bytes = fs.Read(buffer, 0, buffer.Length);
                    rs.Write(buffer, 0, bytes);
                    total_bytes = total_bytes - bytes;
                    Position += bytes;
                }

                Timer.Enabled = false;
                fs.Close();
                rs.Close();
            }
            catch
            {
                Timer.Enabled = false;
                GC.Collect();
                return false;
            }


            return true;
        }

        public bool DeleteFile(string sFtpFileName, int FTPType)
        {
            try
            {
                FtpWebRequest ftpClient = (FtpWebRequest)WebRequest.Create(sFtpFileName);

                if (FTPType == 4)
                    ftpClient.Credentials = new NetworkCredential(ZOVTPSHostFTPLogin, ZOVTPSHostFTPPass);
                if (FTPType == 3)
                    ftpClient.Credentials = new NetworkCredential(ServerFTP178Login, ServerFTP178Pass);
                if (FTPType == 2)
                    ftpClient.Credentials = new NetworkCredential(LocalFTPLogin, LocalFTPPass);
                if (FTPType == 1)
                    ftpClient.Credentials = new NetworkCredential(HostFTPLogin, HostFTPPass);
                if (FTPType == 0)
                    ftpClient.Credentials = new NetworkCredential(ServerFTPLogin, ServerFTPPass);

                ftpClient.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;
                ftpClient.UseBinary = true;
                ftpClient.KeepAlive = true;

                FtpWebResponse response = (FtpWebResponse)ftpClient.GetResponse();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool FileExist(string sFtpFileName, int FTPType)
        {
            try
            {
                FtpWebRequest ftpClient = (FtpWebRequest)WebRequest.Create(sFtpFileName);

                if (FTPType == 4)
                    ftpClient.Credentials = new NetworkCredential(ZOVTPSHostFTPLogin, ZOVTPSHostFTPPass);
                if (FTPType == 3)
                    ftpClient.Credentials = new NetworkCredential(ServerFTP178Login, ServerFTP178Pass);
                if (FTPType == 2)
                    ftpClient.Credentials = new NetworkCredential(LocalFTPLogin, LocalFTPPass);
                if (FTPType == 1)
                    ftpClient.Credentials = new NetworkCredential(HostFTPLogin, HostFTPPass);
                if (FTPType == 0)
                    ftpClient.Credentials = new NetworkCredential(ServerFTPLogin, ServerFTPPass);

                ftpClient.Method = System.Net.WebRequestMethods.Ftp.GetFileSize;
                ftpClient.UseBinary = true;
                ftpClient.KeepAlive = true;


                try
                {
                    FtpWebResponse response = (FtpWebResponse)ftpClient.GetResponse();
                }
                catch (WebException ex)
                {
                    FtpWebResponse response = (FtpWebResponse)ex.Response;
                    if (response.StatusCode ==
                        FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool RenameFile(string sSourceFileName, string NewName, int FTPType)
        {
            try
            {
                FtpWebRequest ftpClient = (FtpWebRequest)WebRequest.Create(sSourceFileName);


                ftpClient.Method = WebRequestMethods.Ftp.Rename;
                ftpClient.RenameTo = NewName;
                ftpClient.UseBinary = true;
                ftpClient.KeepAlive = true;

                if (FTPType == 4)
                    ftpClient.Credentials = new NetworkCredential(ZOVTPSHostFTPLogin, ZOVTPSHostFTPPass);
                if (FTPType == 3)
                    ftpClient.Credentials = new NetworkCredential(ServerFTP178Login, ServerFTP178Pass);
                if (FTPType == 2)
                    ftpClient.Credentials = new NetworkCredential(LocalFTPLogin, LocalFTPPass);
                if (FTPType == 1)
                    ftpClient.Credentials = new NetworkCredential(HostFTPLogin, HostFTPPass);
                if (FTPType == 0)
                    ftpClient.Credentials = new NetworkCredential(ServerFTPLogin, ServerFTPPass);

                FtpWebResponse response = (FtpWebResponse)ftpClient.GetResponse();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public int CreateFolder(string Path, string FolderName, int FTPType)
        {
            FtpWebRequest ftpClient = (FtpWebRequest)WebRequest.Create(Path + FolderName);

            if (FTPType == 4)
                ftpClient.Credentials = new NetworkCredential(ZOVTPSHostFTPLogin, ZOVTPSHostFTPPass);
            if (FTPType == 3)
                ftpClient.Credentials = new NetworkCredential(ServerFTP178Login, ServerFTP178Pass);
            if (FTPType == 2)
                ftpClient.Credentials = new NetworkCredential(LocalFTPLogin, LocalFTPPass);
            if (FTPType == 1)
                ftpClient.Credentials = new NetworkCredential(HostFTPLogin, HostFTPPass);
            if (FTPType == 0)
                ftpClient.Credentials = new NetworkCredential(ServerFTPLogin, ServerFTPPass);

            ftpClient.Method = System.Net.WebRequestMethods.Ftp.MakeDirectory;
            ftpClient.UseBinary = false;
            ftpClient.KeepAlive = true;



            try
            {
                ftpClient.GetResponse();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }

            return 1;//directory create ok
        }

        public bool DeleteFolder(string Path, int FTPType)
        {
            FtpWebRequest ftpClient = (FtpWebRequest)WebRequest.Create(Path);

            if (FTPType == 4)
                ftpClient.Credentials = new NetworkCredential(ZOVTPSHostFTPLogin, ZOVTPSHostFTPPass);
            if (FTPType == 3)
                ftpClient.Credentials = new NetworkCredential(ServerFTP178Login, ServerFTP178Pass);
            if (FTPType == 2)
                ftpClient.Credentials = new NetworkCredential(LocalFTPLogin, LocalFTPPass);
            if (FTPType == 1)
                ftpClient.Credentials = new NetworkCredential(HostFTPLogin, HostFTPPass);
            if (FTPType == 0)
                ftpClient.Credentials = new NetworkCredential(ServerFTPLogin, ServerFTPPass);

            ftpClient.Method = System.Net.WebRequestMethods.Ftp.RemoveDirectory;
            ftpClient.UseBinary = true;
            ftpClient.KeepAlive = false;

            try
            {
                ftpClient.GetResponse();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public byte[] ReadFile(string Path, Int64 FileSize, int FTPType)
        {
            CurrentSpeed = 0;
            iTransData = 0;
            Position = 0;
            TotalFileSize = 0;

            Byte[] buffer = new Byte[2048];

            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    FtpWebRequest ftpClient = (FtpWebRequest)WebRequest.Create(Path);

                    if (FTPType == 4)
                        ftpClient.Credentials = new NetworkCredential(ZOVTPSHostFTPLogin, ZOVTPSHostFTPPass);
                    if (FTPType == 3)
                        ftpClient.Credentials = new NetworkCredential(ServerFTP178Login, ServerFTP178Pass);
                    if (FTPType == 2)
                        ftpClient.Credentials = new NetworkCredential(LocalFTPLogin, LocalFTPPass);
                    if (FTPType == 1)
                        ftpClient.Credentials = new NetworkCredential(HostFTPLogin, HostFTPPass);
                    if (FTPType == 0)
                        ftpClient.Credentials = new NetworkCredential(ServerFTPLogin, ServerFTPPass);

                    ftpClient.UseBinary = true;
                    ftpClient.KeepAlive = true;

                    TotalFileSize = FileSize;

                    ftpClient.Method = WebRequestMethods.Ftp.DownloadFile;
                    FtpWebResponse response = (FtpWebResponse)ftpClient.GetResponse();

                    //write file
                    Stream responseStream = response.GetResponseStream();

                    Timer.Enabled = true;


                    int bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                    Position += bytesRead;

                    if (bytesRead > 0)
                        ms.Write(buffer, 0, bytesRead);

                    while (bytesRead > 0)
                    {
                        if (bStopTransfer)
                        {
                            bStopTransfer = false;
                            ftpClient.Abort();
                            response.Close();
                            responseStream.Close();
                            break;
                        }

                        bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                            ms.Write(buffer, 0, bytesRead);
                        Position += bytesRead;
                    }

                    Position = TotalFileSize;
                    Timer.Enabled = false;
                    ftpClient.Abort();
                    response.Close();
                    responseStream.Close();
                    ms.Capacity = (int)ms.Length;

                    return ms.ToArray();
                }
                catch (InvalidOperationException ex)
                {
                    string m = ex.Message;
                    Timer.Enabled = false;
                    bStopTransfer = false;
                    GC.Collect();
                    return ms.ToArray();
                }
                catch (Exception ex)
                {
                    string m = ex.Message;
                    Timer.Enabled = false;
                    bStopTransfer = false;
                    GC.Collect();
                    return ms.ToArray();
                }
            }
        }
    }





    public class InfiniumTips
    {
        public static void ShowTipModal(string sText, Form ParentForm, int Left, int Top, int Time)
        {
            InfiniumTipForm F2 = new InfiniumTipForm(ParentForm.Left + Left, ParentForm.Top + Top, 2000, sText);
            F2.ShowDialog();
            F2.Focus();
        }

        public static void ShowTipModal(Form ParentForm, int LeftPercents, int TopPercents, string sText, int Time)
        {
            InfiniumTipForm F2 = new InfiniumTipForm(ParentForm, LeftPercents, TopPercents, Time, sText);
            F2.ShowDialog();
            F2 = null;
        }

        public static void ShowTip(Form ParentForm, int LeftPercents, int TopPercents, string sText, int Time)
        {
            InfiniumTipForm F2 = new InfiniumTipForm(ParentForm, LeftPercents, TopPercents, Time, sText);
            F2.Show();
            F2 = null;
        }
    }





    public class LightMessageBox
    {
        public static bool Show(ref Form TopForm, bool ShowCancelButton, string Text, string HeaderText)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            LightMessageBoxForm LightMessageBoxForm = new LightMessageBoxForm(ShowCancelButton, Text, HeaderText);

            TopForm = LightMessageBoxForm;
            LightMessageBoxForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            LightMessageBoxForm.Dispose();
            TopForm = null;

            return LightMessageBoxForm.OKCancel;
        }

        public static bool ShowClientDeleteForm(ref Form TopForm, ref bool bDeleteOrders)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            ClientDeleteForm clientDeleteForm = new ClientDeleteForm();

            TopForm = clientDeleteForm;
            clientDeleteForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            clientDeleteForm.Dispose();
            TopForm = null;

            bDeleteOrders = clientDeleteForm.bDeleteOrders;

            return clientDeleteForm.OKCancel;
        }
    }

    public interface IFirstProfilName
    {
        string GetFrontName(int i);
    }

    public interface IColorName
    {
        string GetColorName(int i);
    }

    public interface IPatinaName
    {
        string GetPatinaName(int i);
    }

    public interface IInsetTypeName
    {
        string GetInsetTypeName(int i);
    }

    public interface IInsetColorName
    {
        string GetInsetColorName(int i);
    }
    public interface IIsMarsel
    {
        bool IsMarsel3(int i);
        bool IsMarsel4(int i);
        bool IsImpost(int i);
    }

    public interface IAllFrontParameterName
    {
        string GetFrontName(int i);
        string GetFront2Name(int i);
        string GetColorName(int i);
        string GetPatinaName(int i);
        string GetInsetTypeName(int i);
        string GetInsetColorName(int i);
    }
}
