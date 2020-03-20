using NPOI.HSSF.UserModel;
using NPOI.HSSF.UserModel.Contrib;
using NPOI.HSSF.Util;

using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Infinium.Modules.Permits
{
    public class Permits
    {
        DataTable AllDispatchFrontsWeightDT;
        DataTable AllDispatchDecorWeightDT;
        DataTable AllMainOrdersFrontsWeightDT;
        DataTable AllMainOrdersDecorWeightDT;
        DataTable AllMegaBatchNumbersDT;
        DataTable DispatchInfoDT;
        DataTable RealDispTimeDT;
        DataTable PermitsDatesDT;

        DataTable UnloadsDT;
        DataTable MDispatchDT;
        DataTable ZDispatchDT;
        DataTable PermitsDT;
        DataTable PermitDetailsDT;
        DataTable UsersDT;

        BindingSource UnloadsBS;
        BindingSource MDispatchBS;
        BindingSource ZDispatchBS;
        BindingSource PermitsBS;
        BindingSource UsersBS;
        BindingSource PermitsDatesBS;

        public Permits()
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
            PermitsDatesDT = new DataTable();

            AllDispatchFrontsWeightDT = new DataTable();
            AllDispatchDecorWeightDT = new DataTable();
            AllMainOrdersFrontsWeightDT = new DataTable();
            AllMainOrdersDecorWeightDT = new DataTable();
            AllMegaBatchNumbersDT = new DataTable();
            DispatchInfoDT = new DataTable();
            RealDispTimeDT = new DataTable();

            MDispatchDT = new DataTable();
            MDispatchDT.Columns.Add(new DataColumn(("DispPackagesCount"), System.Type.GetType("System.String")));
            MDispatchDT.Columns.Add(new DataColumn(("Weight"), System.Type.GetType("System.Decimal")));
            MDispatchDT.Columns.Add(new DataColumn(("DispatchStatus"), System.Type.GetType("System.String")));
            MDispatchDT.Columns.Add(new DataColumn(("RealDispDateTime"), System.Type.GetType("System.DateTime")));
            PermitsDT = new DataTable();
            UsersDT = new DataTable();
            ZDispatchDT = MDispatchDT.Clone();
            UnloadsDT = new DataTable();

            MDispatchBS = new BindingSource();
            ZDispatchBS = new BindingSource();
            PermitsBS = new BindingSource();
            UsersBS = new BindingSource();
            UnloadsBS = new BindingSource();
            PermitsDatesBS = new BindingSource();
        }

        private void Fill()
        {
            string SelectCommand = "SELECT UserID, Name, ShortName FROM Users  WHERE Fired <> 1 ORDER BY Name";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDT);
            }
            SelectCommand = "SELECT TOP 0 * FROM Permits";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(PermitsDT);
            }
            SelectCommand = "SELECT TOP 0 * FROM PermitDetails";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                PermitDetailsDT = new DataTable();
                DA.Fill(PermitDetailsDT);
            }
            SelectCommand = @"SELECT TOP 0 Dispatch.*, infiniu2_marketingreference.dbo.Clients.ClientName, 
				ConfirmExpUser.ShortName AS ConfirmExpUser, ConfirmDispUser.ShortName AS ConfirmDispUser FROM Dispatch
				LEFT JOIN infiniu2_users.dbo.Users AS ConfirmexpUser ON Dispatch.ConfirmExpUserID = ConfirmExpUser.UserID
				LEFT JOIN infiniu2_users.dbo.Users AS ConfirmDispUser ON Dispatch.ConfirmDispUserID = ConfirmDispUser.UserID
				INNER JOIN infiniu2_marketingreference.dbo.Clients ON Dispatch.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MDispatchDT);
            }
            SelectCommand = @"SELECT TOP 0 Dispatch.*, infiniu2_zovreference.dbo.Clients.ClientName, 
				ConfirmExpUser.ShortName AS ConfirmExpUser, ConfirmDispUser.ShortName AS ConfirmDispUser FROM Dispatch
				LEFT JOIN infiniu2_users.dbo.Users AS ConfirmexpUser ON Dispatch.ConfirmExpUserID = ConfirmExpUser.UserID
				LEFT JOIN infiniu2_users.dbo.Users AS ConfirmDispUser ON Dispatch.ConfirmDispUserID = ConfirmDispUser.UserID
				INNER JOIN infiniu2_zovreference.dbo.Clients ON Dispatch.ClientID = infiniu2_zovreference.dbo.Clients.ClientID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(ZDispatchDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM Unloads";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(UnloadsDT);
            }
            SelectCommand = "SELECT TOP 0 CreateDate FROM Permits ORDER BY CreateDate";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(PermitsDatesDT);
                PermitsDatesDT.Columns.Add(new DataColumn(("WeekNumber"), System.Type.GetType("System.String")));
            }
        }

        private void Binding()
        {
            MDispatchBS.DataSource = MDispatchDT;
            ZDispatchBS.DataSource = ZDispatchDT;
            PermitsBS.DataSource = PermitsDT;
            UsersBS.DataSource = UsersDT;
            UnloadsBS.DataSource = UnloadsDT;
            PermitsDatesBS.DataSource = PermitsDatesDT;
        }

        public bool HasPermits
        {
            get { return PermitsDT.Rows.Count > 0; }
        }

        public object CurrentCreateDate
        {
            get
            {
                if (PermitsDatesBS.Count == 0 || ((DataRowView)PermitsDatesBS.Current).Row["CreateDate"] == DBNull.Value)
                    return DBNull.Value;
                else
                    return ((DataRowView)PermitsDatesBS.Current).Row["CreateDate"];
            }
        }

        public BindingSource MDispatchList
        {
            get { return MDispatchBS; }
        }

        public BindingSource ZDispatchList
        {
            get { return ZDispatchBS; }
        }

        public BindingSource UnloadsList
        {
            get { return UnloadsBS; }
        }

        public BindingSource PermitsList
        {
            get { return PermitsBS; }
        }

        public BindingSource PermitsDatesList
        {
            get { return PermitsDatesBS; }
        }

        public DataGridViewComboBoxColumn CreateUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn()
                {
                    Name = "CreateUserColumn",
                    HeaderText = "Создал",
                    DataPropertyName = "UserID",
                    DataSource = new DataView(UsersDT),
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

        public DataGridViewComboBoxColumn SignUserColumn
        {
            get
            {
                DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn()
                {
                    Name = "SignUserColumn",
                    HeaderText = "Утвердил",
                    DataPropertyName = "SignUserID",
                    DataSource = new DataView(UsersDT),
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

        private int GetWeekNumber(DateTime dtPassed)
        {
            System.Globalization.CultureInfo ciCurr = System.Globalization.CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekNum;
        }

        private void FillWeekNumber()
        {
            for (int i = 0; i < PermitsDatesDT.Rows.Count; i++)
            {
                PermitsDatesDT.Rows[i]["WeekNumber"] = GetWeekNumber(Convert.ToDateTime(PermitsDatesDT.Rows[i]["CreateDate"])) + " к.н.";
            }
        }

        public void ClearZDispatch()
        {
            ZDispatchDT.Clear();
        }

        public void ClearMDispatch()
        {
            MDispatchDT.Clear();
        }

        public void ClearUnloads()
        {
            UnloadsDT.Clear();
        }

        public void ClearPermits()
        {
            PermitsDT.Clear();
            PermitDetailsDT.Clear();
        }

        public void SavePermits()
        {
            string SelectCommand = "SELECT TOP 0 * FROM Permits";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(PermitsDT);
                }
            }

            SelectCommand = "SELECT TOP 0 * FROM PermitDetails";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(PermitDetailsDT);
                }
            }
        }

        public void UpdatePermitsDates(DateTime Date)
        {
            string SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), CreateDate, 104) AS CreateDate FROM Permits" +
                " WHERE DATEPART(month, CreateDate) = DATEPART(month, '" + Date.ToString("yyyy-MM-dd") +
                "') AND DATEPART(year, CreateDate) = DATEPART(year, '" + Date.ToString("yyyy-MM-dd") + "')" +
                " ORDER BY CreateDate DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                PermitsDatesDT.Clear();
                DA.Fill(PermitsDatesDT);
            }
            FillWeekNumber();
        }

        public void FilterPermitsByCreateDate(DateTime Date)
        {
            string SelectCommand = "SELECT * FROM Permits WHERE CAST(CreateDate AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(PermitsDT);
            }
            SelectCommand = "SELECT * FROM PermitDetails WHERE PermitID IN (SELECT PermitID FROM Permits WHERE CAST(CreateDate AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(PermitDetailsDT);
            }
        }

        public void FilterPermitsBySignedDate(DateTime Date)
        {
            string SelectCommand = "SELECT * FROM Permits WHERE CAST(SignedDate AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(PermitsDT);
            }
        }

        public void NewPermit(bool bVisitor, string Name, string Purpose)
        {
            DataRow NewRow = PermitsDT.NewRow();
            NewRow["CreateDate"] = Security.GetCurrentDate();
            NewRow["UserID"] = Security.CurrentUserID;
            NewRow["Visitor"] = bVisitor;
            if (Name.Length > 0)
                NewRow["Name"] = Name;
            if (Purpose.Length > 0)
                NewRow["Purpose"] = Purpose;
            PermitsDT.Rows.Add(NewRow);
        }

        public void RemovePermit(int PermitID)
        {
            DataRow[] rows = PermitDetailsDT.Select("PermitID=" + PermitID);
            for (int i = rows.Count() - 1; i >= 0; i--)
                rows[i].Delete();

            rows = PermitsDT.Select("PermitID=" + PermitID);
            if (rows.Count() == 0)
                return;
            rows[0].Delete();
        }

        public void BindPermitToMarketingDispatch(int PermitID, int[] Dispatches)
        {
            for (int i = 0; i < Dispatches.Count(); i++)
            {
                DataRow NewRow = PermitDetailsDT.NewRow();
                NewRow["CreateDateTime"] = Security.GetCurrentDate();
                NewRow["CreateUserID"] = Security.CurrentUserID;
                NewRow["PermitID"] = PermitID;
                NewRow["DispatchID"] = Dispatches[i];
                PermitDetailsDT.Rows.Add(NewRow);
            }

            //DataRow[] rows = PermitsDT.Select("PermitID=" + PermitID);
            //if (rows.Count() == 0)
            //	return;
            //rows[0]["MarketingDispatchID"] = MarketingDispatchID;
        }

        public void BindPermitToZOVDispatch(int PermitID, DateTime ZOVDispatchDate)
        {
            DataRow[] rows = PermitsDT.Select("PermitID=" + PermitID);
            if (rows.Count() == 0)
                return;
            rows[0]["ZOVDispatchDate"] = ZOVDispatchDate;
        }

        public void BindPermitToUnload(int PermitID, int UnloadID)
        {
            DataRow[] rows = PermitsDT.Select("PermitID=" + PermitID);
            if (rows.Count() == 0)
                return;
            rows[0]["UnloadID"] = UnloadID;
        }

        public void SignPermit(int PermitID)
        {
            DataRow[] rows = PermitsDT.Select("PermitID=" + PermitID);
            if (rows.Count() == 0)
                return;
            rows[0]["SignUserID"] = Security.CurrentUserID;
            rows[0]["SignedDate"] = Security.GetCurrentDate();
        }

        public void SecurityCheck(int PermitID, bool Checked)
        {
            DataRow[] rows = PermitsDT.Select("PermitID=" + PermitID);
            if (rows.Count() == 0)
                return;
            rows[0]["SecurityChecked"] = Checked;
            rows[0]["SecurityCheckDate"] = Security.GetCurrentDate();
        }

        public void UnloadInformation(int UnloadID, ref string UserName, ref string ResponsibleUserName,
            ref object UnloadDateTime)
        {
            string SelectCommand = @"SELECT UserName, infiniu2_users.dbo.Users.ShortName AS ResponsibleUserName, UnloadDateTime FROM Unloads
				LEFT JOIN infiniu2_users.dbo.Users ON Unloads.ResponsibleUserID = Users.UserID
				WHERE UnloadID = " + UnloadID;
            string OrdersConnectionString = ConnectionStrings.LightConnectionString;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, OrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["UserName"] != DBNull.Value)
                            UserName = DT.Rows[0]["UserName"].ToString();
                        if (DT.Rows[0]["ResponsibleUserName"] != DBNull.Value)
                            ResponsibleUserName = DT.Rows[0]["ResponsibleUserName"].ToString();

                        UnloadDateTime = DT.Rows[0]["UnloadDateTime"];
                    }
                }
            }
        }

        public void GetMMainOrdersInfo(int DispatchID)
        {
            string SelectCommand = @"SELECT Packages.DispatchID, FrontsOrders.MainOrderID, (Square*PackageDetails.Count/FrontsOrders.Count) As Square, (FrontsOrders.Weight*PackageDetails.Count/FrontsOrders.Count) AS Weight
					FROM PackageDetails INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
					INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 0 
					AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE DispatchID=" + DispatchID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                AllMainOrdersFrontsWeightDT.Clear();
                DA.Fill(AllMainOrdersFrontsWeightDT);
            }
            SelectCommand = @"SELECT Packages.DispatchID, DecorOrders.MainOrderID, (DecorOrders.Weight*PackageDetails.Count/DecorOrders.Count) AS Weight
					FROM PackageDetails INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID
					INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 1 
					AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE DispatchID=" + DispatchID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                AllMainOrdersDecorWeightDT.Clear();
                DA.Fill(AllMainOrdersDecorWeightDT);
            }

            SelectCommand = @"SELECT Packages.DispatchID, (Square*PackageDetails.Count/FrontsOrders.Count) As Square, (FrontsOrders.Weight*PackageDetails.Count/FrontsOrders.Count) AS Weight
					FROM PackageDetails INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
					INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 0 
					AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE DispatchID=" + DispatchID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                AllDispatchFrontsWeightDT.Clear();
                DA.Fill(AllDispatchFrontsWeightDT);
            }
            SelectCommand = @"SELECT Packages.DispatchID, (DecorOrders.Weight*PackageDetails.Count/DecorOrders.Count) AS Weight
					FROM PackageDetails INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID
					INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 1 
					AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE DispatchID=" + DispatchID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                AllDispatchDecorWeightDT.Clear();
                DA.Fill(AllDispatchDecorWeightDT);
            }
            SelectCommand = @"SELECT PackageID, PackageStatusID, DispatchID
					FROM Packages WHERE Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE DispatchID=" + DispatchID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DispatchInfoDT.Clear();
                DA.Fill(DispatchInfoDT);
            }
            SelectCommand = @"SELECT Packages.DispatchID, MAX(DispatchDateTime) AS DispatchDateTime
					FROM Packages WHERE Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE DispatchID=" + DispatchID + ") GROUP BY DispatchID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                RealDispTimeDT.Clear();
                DA.Fill(RealDispTimeDT);
            }
        }

        public void GetZMainOrdersInfo(DateTime PrepareDispatchDateTime)
        {
            string SelectCommand = @"SELECT Packages.DispatchID, FrontsOrders.MainOrderID, (Square*PackageDetails.Count/FrontsOrders.Count) As Square, (FrontsOrders.Weight*PackageDetails.Count/FrontsOrders.Count) AS Weight
					FROM PackageDetails INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
					INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 0 
					AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
					'" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                AllMainOrdersFrontsWeightDT.Clear();
                DA.Fill(AllMainOrdersFrontsWeightDT);
            }
            SelectCommand = @"SELECT Packages.DispatchID, DecorOrders.MainOrderID, (DecorOrders.Weight*PackageDetails.Count/DecorOrders.Count) AS Weight
					FROM PackageDetails INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID
					INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 1 
					AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
					'" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                AllMainOrdersDecorWeightDT.Clear();
                DA.Fill(AllMainOrdersDecorWeightDT);
            }

            SelectCommand = @"SELECT Packages.DispatchID, (Square*PackageDetails.Count/FrontsOrders.Count) As Square, (FrontsOrders.Weight*PackageDetails.Count/FrontsOrders.Count) AS Weight
					FROM PackageDetails INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
					INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 0 
					AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
					'" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                AllDispatchFrontsWeightDT.Clear();
                DA.Fill(AllDispatchFrontsWeightDT);
            }
            SelectCommand = @"SELECT Packages.DispatchID, (DecorOrders.Weight*PackageDetails.Count/DecorOrders.Count) AS Weight
					FROM PackageDetails INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID
					INNER JOIN  Packages ON PackageDetails.PackageID = Packages.PackageID AND Packages.ProductType = 1 
					AND Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
					'" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                AllDispatchDecorWeightDT.Clear();
                DA.Fill(AllDispatchDecorWeightDT);
            }
            SelectCommand = @"SELECT PackageID, PackageStatusID, DispatchID
					FROM Packages WHERE Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
					'" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "')";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DispatchInfoDT.Clear();
                DA.Fill(DispatchInfoDT);
            }
            SelectCommand = @"SELECT Packages.DispatchID, MAX(DispatchDateTime) AS DispatchDateTime
					FROM Packages WHERE Packages.DispatchID IN (SELECT DispatchID FROM Dispatch WHERE CAST(PrepareDispatchDateTime AS Date) = 
					'" + PrepareDispatchDateTime.ToString("yyyy-MM-dd") + "') GROUP By DispatchID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                RealDispTimeDT.Clear();
                DA.Fill(RealDispTimeDT);
            }
        }

        private decimal GetWeight(int DispatchID)
        {
            decimal Weight = 0;
            DataRow[] rows = AllDispatchFrontsWeightDT.Select("DispatchID=" + DispatchID);
            foreach (DataRow item in rows)
            {
                Weight += Convert.ToDecimal(item["Weight"]) + Convert.ToDecimal(item["Square"]) * Convert.ToDecimal(0.7);
            }
            rows = AllDispatchDecorWeightDT.Select("DispatchID=" + DispatchID);
            foreach (DataRow item in rows)
            {
                Weight += Convert.ToDecimal(item["Weight"]);
            }
            Weight = Decimal.Round(Weight, 2, MidpointRounding.AwayFromZero);

            return Weight;
        }

        public object GetRealDispDateTime(int DispatchID)
        {
            object RealDispDateTime = DBNull.Value;
            DataRow[] rows = RealDispTimeDT.Select("DispatchID=" + DispatchID);

            if (rows.Count() > 0 && rows[0]["DispatchDateTime"] != DBNull.Value)
                RealDispDateTime = rows[0]["DispatchDateTime"];

            return RealDispDateTime;
        }

        private void GetDispPackagesInfo(int DispatchID, ref int DispPackagesCount, ref int PackagesCount)
        {
            DataRow[] rows1 = DispatchInfoDT.Select("DispatchID=" + DispatchID);
            DataRow[] rows2 = DispatchInfoDT.Select("DispatchID=" + DispatchID + " AND PackageStatusID=3");

            if (rows2.Count() > 0)
            {
                DispPackagesCount = rows2.Count();
            }
            if (rows1.Count() > 0)
            {
                PackagesCount = rows1.Count();
            }
        }

        private void FillMDispPackagesInfo()
        {

        }

        private void FillZDispPackagesInfo()
        {
            bool IsDispConfirm = false;
            bool IsExpConfirm = false;
            int DispPackagesCount = 0;
            int PackagesCount = 0;
            string Status = string.Empty;
            object RealDispDateTime = DBNull.Value;

            for (int i = 0; i < ZDispatchDT.Rows.Count; i++)
            {
                DispPackagesCount = 0;
                PackagesCount = 0;
                if (ZDispatchDT.Rows[i]["ConfirmExpDateTime"] != DBNull.Value)
                    IsExpConfirm = true;
                else
                    IsExpConfirm = false;
                if (ZDispatchDT.Rows[i]["ConfirmDispDateTime"] != DBNull.Value)
                    IsDispConfirm = true;
                else
                    IsDispConfirm = false;
                GetDispPackagesInfo(Convert.ToInt32(ZDispatchDT.Rows[i]["DispatchID"]), ref DispPackagesCount, ref PackagesCount);
                if (PackagesCount > 0)
                {
                    if (IsExpConfirm)
                    {
                        Status = "Утверждена к эксп-ции";
                        if (IsDispConfirm)
                        {
                            Status = "Утверждена к отгрузке";
                            if (DispPackagesCount > 0 && PackagesCount == DispPackagesCount)
                                Status = "Отгружена";
                        }
                    }
                    else
                        Status = "Ожидает утверждения к эксп-ции";

                }
                else
                {
                    Status = "Отгрузка пуста";
                }

                if (PackagesCount == DispPackagesCount)
                {
                    RealDispDateTime = GetRealDispDateTime(Convert.ToInt32(ZDispatchDT.Rows[i]["DispatchID"]));
                    ZDispatchDT.Rows[i]["RealDispDateTime"] = RealDispDateTime;
                }
                ZDispatchDT.Rows[i]["DispatchStatus"] = Status;
                ZDispatchDT.Rows[i]["DispPackagesCount"] = DispPackagesCount + " / " + PackagesCount;
                ZDispatchDT.Rows[i]["Weight"] = GetWeight(Convert.ToInt32(ZDispatchDT.Rows[i]["DispatchID"]));
            }
        }

        public void GetMDispatch(int PermitID)
        {
            string SelectCommand = @"SELECT Dispatch.*, infiniu2_marketingreference.dbo.Clients.ClientName, 
				ConfirmExpUser.ShortName AS ConfirmExpUser, ConfirmDispUser.ShortName AS ConfirmDispUser FROM Dispatch
				LEFT JOIN infiniu2_users.dbo.Users AS ConfirmexpUser ON Dispatch.ConfirmExpUserID = ConfirmExpUser.UserID
				LEFT JOIN infiniu2_users.dbo.Users AS ConfirmDispUser ON Dispatch.ConfirmDispUserID = ConfirmDispUser.UserID
				INNER JOIN infiniu2_marketingreference.dbo.Clients ON Dispatch.ClientID = infiniu2_marketingreference.dbo.Clients.ClientID
				WHERE DispatchID IN (SELECT DispatchID FROM infiniu2_light.dbo.PermitDetails WHERE infiniu2_light.dbo.PermitDetails.PermitID = " + PermitID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                MDispatchDT.Clear();
                DA.Fill(MDispatchDT);
                for (int i = 0; i < MDispatchDT.Rows.Count; i++)
                {
                    GetMMainOrdersInfo(Convert.ToInt32(MDispatchDT.Rows[i]["DispatchID"]));
                    bool IsDispConfirm = false;
                    bool IsExpConfirm = false;
                    int DispPackagesCount = 0;
                    int PackagesCount = 0;
                    string Status = string.Empty;
                    object RealDispDateTime = DBNull.Value;

                    DispPackagesCount = 0;
                    PackagesCount = 0;
                    if (MDispatchDT.Rows[i]["ConfirmExpDateTime"] != DBNull.Value)
                        IsExpConfirm = true;
                    else
                        IsExpConfirm = false;
                    if (MDispatchDT.Rows[i]["ConfirmDispDateTime"] != DBNull.Value)
                        IsDispConfirm = true;
                    else
                        IsDispConfirm = false;
                    GetDispPackagesInfo(Convert.ToInt32(MDispatchDT.Rows[i]["DispatchID"]), ref DispPackagesCount, ref PackagesCount);
                    if (PackagesCount > 0)
                    {
                        if (IsExpConfirm)
                        {
                            Status = "Утверждена к эксп-ции";
                            if (IsDispConfirm)
                            {
                                Status = "Утверждена к отгрузке";
                                if (DispPackagesCount > 0 && PackagesCount == DispPackagesCount)
                                    Status = "Отгружена";
                            }
                        }
                        else
                            Status = "Ожидает утверждения к эксп-ции";

                    }
                    else
                    {
                        Status = "Отгрузка пуста";
                    }

                    if (PackagesCount == DispPackagesCount)
                    {
                        RealDispDateTime = GetRealDispDateTime(Convert.ToInt32(MDispatchDT.Rows[i]["DispatchID"]));
                        MDispatchDT.Rows[i]["RealDispDateTime"] = RealDispDateTime;
                    }
                    MDispatchDT.Rows[i]["DispatchStatus"] = Status;
                    MDispatchDT.Rows[i]["DispPackagesCount"] = DispPackagesCount + " / " + PackagesCount;
                    MDispatchDT.Rows[i]["Weight"] = GetWeight(Convert.ToInt32(MDispatchDT.Rows[i]["DispatchID"]));
                }
            }
        }

        public void GetZDispatch(DateTime ZOVDispatchDate)
        {
            string SelectCommand = @"SELECT Dispatch.*, infiniu2_zovreference.dbo.Clients.ClientName, 
				ConfirmExpUser.ShortName AS ConfirmExpUser, ConfirmDispUser.ShortName AS ConfirmDispUser FROM Dispatch
				LEFT JOIN infiniu2_users.dbo.Users AS ConfirmexpUser ON Dispatch.ConfirmExpUserID = ConfirmExpUser.UserID
				LEFT JOIN infiniu2_users.dbo.Users AS ConfirmDispUser ON Dispatch.ConfirmDispUserID = ConfirmDispUser.UserID
				INNER JOIN infiniu2_zovreference.dbo.Clients ON Dispatch.ClientID = infiniu2_zovreference.dbo.Clients.ClientID
				WHERE CAST(PrepareDispatchDateTime AS Date) = '" + ZOVDispatchDate.ToString("yyyy-MM-dd") + "'";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                ZDispatchDT.Clear();
                DA.Fill(ZDispatchDT);
                //for (int i = 0; i < ZDispatchDT.Rows.Count; i++)
                //    ZDispatchDT.Rows[i]["Weight"] = GetWeight(true, true, Convert.ToInt32(ZDispatchDT.Rows[i]["DispatchID"]));
            }
            GetZMainOrdersInfo(ZOVDispatchDate);
            FillZDispPackagesInfo();
        }

        public void GetUnoads(int UnloadID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Unloads WHERE UnloadID = " + UnloadID,
                ConnectionStrings.LightConnectionString))
            {
                UnloadsDT.Clear();
                DA.Fill(UnloadsDT);
            }
        }

        public void MoveToCreateDate(DateTime CreateDate)
        {
            PermitsDatesBS.Position = PermitsDatesBS.Find("CreateDate", CreateDate);
        }

        public void MoveToPermit(int PermitID)
        {
            PermitsBS.Position = PermitsBS.Find("PermitID", PermitID);
        }
    }




    public class NewPermit
    {
        public bool bCreatePermit;
        public int AddresseeID;
        public string CreateUserName;
        public string VisitorName;
        public string VisitMission;
        public string AddresseeName;
        public DateTime Validity;
    }


    public class VisitorsPermits
    {
        int iVisitorPermitID = 0;
        string AddresseeName = string.Empty;
        string InputDeniedUserName = string.Empty;
        string OutputAllowedUserName = string.Empty;
        string OutputDeniedUserName = string.Empty;
        string CreateUserName = string.Empty;
        string PrintUserName = string.Empty;
        string AgreedUserName = string.Empty;
        string AprovedUserName = string.Empty;

        DataTable dtFilterMenu;
        DataTable dtPermits;
        DataTable dtPermitsDates;
        DataTable dtRolePermissions;
        DataTable dtUsers;
        BindingSource bsPermits;
        BindingSource bsPermitsDates;
        BindingSource bsAddressees;
        SqlDataAdapter daPermits;
        SqlCommandBuilder cbPermits;

        public int CurrentVisitorPermitID
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["VisitorPermitID"] == DBNull.Value)
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
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["VisitorName"] == DBNull.Value)
                    return string.Empty;
                else
                    return ((DataRowView)bsPermits.Current).Row["VisitorName"].ToString();
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
        public string CurrentAddresseeName
        {
            get { return AddresseeName; }
        }
        public string CurrentInputDeniedUserName
        {
            get { return InputDeniedUserName; }
        }
        public string CurrentOutputAllowedUserName
        {
            get { return OutputAllowedUserName; }
        }
        public string CurrentOutputDeniedUserName
        {
            get { return OutputDeniedUserName; }
        }
        public string CurrentCreateUserName
        {
            get { return CreateUserName; }
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
            get { return AgreedUserName; }
        }
        public string CurrentAprovedUserName
        {
            get { return AprovedUserName; }
        }
        public bool CurrentInputEnable
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["InputEnable"] == DBNull.Value)
                    return false;
                else
                    return Convert.ToBoolean(((DataRowView)bsPermits.Current).Row["InputEnable"]);
            }
        }
        public bool CurrentInputDone
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["InputTime"] == DBNull.Value)
                    return false;
                else
                    return true;
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
        public object CurrentInputDeniedTime
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["InputDeniedTime"] == DBNull.Value)
                    return DBNull.Value;
                else
                    return ((DataRowView)bsPermits.Current).Row["InputDeniedTime"];
            }
        }
        public object CurrentInputTime
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["InputTime"] == DBNull.Value)
                    return DBNull.Value;
                else
                    return ((DataRowView)bsPermits.Current).Row["InputTime"];
            }
        }
        public object CurrentOutputAllowedTime
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["OutputAllowedTime"] == DBNull.Value)
                    return DBNull.Value;
                else
                    return ((DataRowView)bsPermits.Current).Row["OutputAllowedTime"];
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
        public object CurrentApprovedTime
        {
            get
            {
                if (bsPermits.Count == 0 || ((DataRowView)bsPermits.Current).Row["ApprovedTime"] == DBNull.Value)
                    return DBNull.Value;
                else
                    return ((DataRowView)bsPermits.Current).Row["ApprovedTime"];
            }
        }
        public DataTable DtFilterMenu
        {
            get { return dtFilterMenu; }
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
        public BindingSource BsAddressees
        {
            get { return bsAddressees; }
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

        public VisitorsPermits()
        {

        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
        }

        public void CreateFilterTable(bool ApprovedRole)
        {
            dtFilterMenu = new DataTable();
            dtFilterMenu.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            dtFilterMenu.Columns.Add(new DataColumn("Count", Type.GetType("System.Int32")));
            dtFilterMenu.Columns.Add(new DataColumn("Image", Type.GetType("System.Byte[]")));

            {
                DataRow NewRow = dtFilterMenu.NewRow();
                NewRow["Name"] = "Мои";
                NewRow["Count"] = 0;
                MemoryStream ms = new MemoryStream();
                Properties.Resources.DocumentsMenuUser.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                NewRow["Image"] = ms.ToArray();
                dtFilterMenu.Rows.Add(NewRow);
                ms.Dispose();
            }

            {
                DataRow NewRow = dtFilterMenu.NewRow();
                NewRow["Name"] = "Все пропуска";
                NewRow["Count"] = 0;
                MemoryStream ms = new MemoryStream();
                Properties.Resources.DocumentsMenuAllDocs.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                NewRow["Image"] = ms.ToArray();
                dtFilterMenu.Rows.Add(NewRow);
                ms.Dispose();
            }
            if (ApprovedRole)
            {
                {
                    DataRow NewRow = dtFilterMenu.NewRow();
                    NewRow["Name"] = "На утверждение";
                    NewRow["Count"] = 0;
                    MemoryStream ms = new MemoryStream();
                    Properties.Resources.DocumentsMenuConfirms.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    NewRow["Image"] = ms.ToArray();
                    dtFilterMenu.Rows.Add(NewRow);
                    ms.Dispose();
                }
            }
            {
                DataRow NewRow = dtFilterMenu.NewRow();
                NewRow["Name"] = "Поиск";
                NewRow["Count"] = 0;
                MemoryStream ms = new MemoryStream();
                Properties.Resources.DocumentsSearch.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                NewRow["Image"] = ms.ToArray();
                dtFilterMenu.Rows.Add(NewRow);
                ms.Dispose();
            }
        }

        private void Create()
        {
            dtPermits = new DataTable();
            dtPermitsDates = new DataTable();
            dtRolePermissions = new DataTable();
            dtUsers = new DataTable();
            bsPermits = new BindingSource();
            bsPermitsDates = new BindingSource();
            bsAddressees = new BindingSource();
        }

        private void Fill()
        {
            string SelectCommand = "SELECT TOP 0 * FROM VisitorsPermits WHERE PermitEnable=1";
            daPermits = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString);
            cbPermits = new SqlCommandBuilder(daPermits);
            daPermits.Fill(dtPermits);
            dtPermits.Columns.Add(new DataColumn(("Overdued"), System.Type.GetType("System.Boolean")));
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            //{
            //    DA.Fill(dtPermits);
            //}
            SelectCommand = "SELECT TOP 0 Validity AS VisitDateTime FROM VisitorsPermits ORDER BY Validity";
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
            bsAddressees.DataSource = new DataView(dtUsers);
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

        public void ClearPermits()
        {
            AddresseeName = string.Empty;
            InputDeniedUserName = string.Empty;
            OutputAllowedUserName = string.Empty;
            OutputDeniedUserName = string.Empty;
            CreateUserName = string.Empty;
            PrintUserName = string.Empty;
            AgreedUserName = string.Empty;
            AprovedUserName = string.Empty;
            dtPermits.Clear();
        }

        public void ClearPermitDates()
        {
            dtPermitsDates.Clear();
        }

        public void SavePermits()
        {
            //string SelectCommand = "SELECT TOP 0 * FROM VisitorsPermits";
            //using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            //{
            //    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
            //    {
            //        DA.Update(dtPermits);
            //    }
            //}
            daPermits.Update(dtPermits);
        }

        public void UpdatePermitsDates(DateTime Date, bool ShowClosePermits,
            bool MyPermits, bool bImCreated, bool bImAddressee, bool bImAgreed, bool bImAproved,
            bool AllPermits, bool bNew, bool bActive, bool bOverdued, bool bClose,
            bool ForApproval)
        {
            string SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                " WHERE PermitEnable=1 AND DATEPART(month, Validity) = DATEPART(month, '" + Date.ToString("yyyy-MM-dd") +
                "') AND DATEPART(year, Validity) = DATEPART(year, '" + Date.ToString("yyyy-MM-dd") + "')" +
                " ORDER BY Validity DESC";
            if (MyPermits)
            {
                if (!ShowClosePermits)
                {
                    if (bImCreated)
                        SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND CreateUserID=" + Security.CurrentUserID + " AND DATEPART(month, Validity) = DATEPART(month, '" + Date.ToString("yyyy-MM-dd") +
                        "') AND DATEPART(year, Validity) = DATEPART(year, '" + Date.ToString("yyyy-MM-dd") + "')" +
                        " ORDER BY Validity DESC";
                    if (bImAddressee)
                        SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND AddresseeID=" + Security.CurrentUserID + " AND DATEPART(month, Validity) = DATEPART(month, '" + Date.ToString("yyyy-MM-dd") +
                        "') AND DATEPART(year, Validity) = DATEPART(year, '" + Date.ToString("yyyy-MM-dd") + "')" +
                        " ORDER BY Validity DESC";
                    if (bImAgreed)
                        SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND AgreedUserID=" + Security.CurrentUserID + " AND DATEPART(month, Validity) = DATEPART(month, '" + Date.ToString("yyyy-MM-dd") +
                        "') AND DATEPART(year, Validity) = DATEPART(year, '" + Date.ToString("yyyy-MM-dd") + "')" +
                        " ORDER BY Validity DESC";
                    if (bImAproved)
                        SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND ApprovedUserID=" + Security.CurrentUserID + " AND DATEPART(month, Validity) = DATEPART(month, '" + Date.ToString("yyyy-MM-dd") +
                        "') AND DATEPART(year, Validity) = DATEPART(year, '" + Date.ToString("yyyy-MM-dd") + "')" +
                        " ORDER BY Validity DESC";
                }
                else
                {
                    if (bImCreated)
                        SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND OutputDone=0 AND CreateUserID=" + Security.CurrentUserID + " AND DATEPART(month, Validity) = DATEPART(month, '" + Date.ToString("yyyy-MM-dd") +
                        "') AND DATEPART(year, Validity) = DATEPART(year, '" + Date.ToString("yyyy-MM-dd") + "')" +
                        " ORDER BY Validity DESC";
                    if (bImAddressee)
                        SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND OutputDone=0 AND AddresseeID=" + Security.CurrentUserID + " AND DATEPART(month, Validity) = DATEPART(month, '" + Date.ToString("yyyy-MM-dd") +
                        "') AND DATEPART(year, Validity) = DATEPART(year, '" + Date.ToString("yyyy-MM-dd") + "')" +
                        " ORDER BY Validity DESC";
                    if (bImAgreed)
                        SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND OutputDone=0 AND AgreedUserID=" + Security.CurrentUserID + " AND DATEPART(month, Validity) = DATEPART(month, '" + Date.ToString("yyyy-MM-dd") +
                        "') AND DATEPART(year, Validity) = DATEPART(year, '" + Date.ToString("yyyy-MM-dd") + "')" +
                        " ORDER BY Validity DESC";
                    if (bImAproved)
                        SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND OutputDone=0 AND ApprovedUserID=" + Security.CurrentUserID + " AND DATEPART(month, Validity) = DATEPART(month, '" + Date.ToString("yyyy-MM-dd") +
                        "') AND DATEPART(year, Validity) = DATEPART(year, '" + Date.ToString("yyyy-MM-dd") + "')" +
                        " ORDER BY Validity DESC";
                }
            }
            if (AllPermits)
            {
                if (bNew)
                    SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                    " WHERE PermitEnable=1 AND InputDone=0 AND OutputDone=0" +
                    " ORDER BY Validity DESC";
                if (bActive)
                    SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                    " WHERE PermitEnable=1 AND InputDone=1 AND OutputDone=0" +
                    " ORDER BY Validity DESC";
                if (bOverdued)
                    SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                    " WHERE PermitEnable=1 AND InputDone=0 AND OutputDone=0 AND CAST(Validity AS DATE) < CAST(GETDATE() AS DATE)" +
                    " ORDER BY Validity DESC";
                if (bClose)
                    SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                    " WHERE PermitEnable=1 AND InputDone=1 AND OutputDone=1 AND DATEPART(month, Validity) = DATEPART(month, '" + Date.ToString("yyyy-MM-dd") +
                    "') AND DATEPART(year, Validity) = DATEPART(year, '" + Date.ToString("yyyy-MM-dd") + "')" +
                    " ORDER BY Validity DESC";
            }
            if (ForApproval)
            {
                SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                    " WHERE PermitEnable=1 AND InputDone=0 AND ApprovedTime IS NULL" +
                    " ORDER BY Validity DESC";
            }
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

        public void UpdatePermitsDatesByPerson(DateTime Date, bool SearchVisitor, string VisitorName,
            bool SearchAddressee, string AddresseeName)
        {
            string SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                " WHERE PermitEnable=1 AND DATEPART(month, Validity) = DATEPART(month, '" + Date.ToString("yyyy-MM-dd") +
                "') AND DATEPART(year, Validity) = DATEPART(year, '" + Date.ToString("yyyy-MM-dd") + "')" +
                " ORDER BY Validity DESC";
            if (SearchVisitor)
            {
                SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                " WHERE PermitEnable=1 AND VisitorName LIKE '%" + VisitorName + "%'" +
                " ORDER BY Validity DESC";
            }
            if (SearchAddressee)
            {
                SelectCommand = "SELECT DISTINCT CONVERT(VARCHAR(10), Validity, 104) AS VisitDateTime, Validity FROM VisitorsPermits" +
                " WHERE PermitEnable=1 AND AddresseeName LIKE '%" + AddresseeName + "%'" +
                " ORDER BY Validity DESC";
            }
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

        public void FilterPermits(DateTime Date, bool ShowClosePermits,
            bool MyPermits, bool bImCreated, bool bImAddressee, bool bImAgreed, bool bImAproved,
            bool AllPermits, bool bNew, bool bActive, bool bOverdued, bool bClose,
            bool ForApproval)
        {
            string SelectCommand = "SELECT * FROM VisitorsPermits WHERE PermitEnable=1 AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";

            if (MyPermits)
            {
                if (!ShowClosePermits)
                {
                    if (bImCreated)
                        SelectCommand = "SELECT * FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND CreateUserID=" + Security.CurrentUserID + " AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
                    if (bImAddressee)
                        SelectCommand = "SELECT * FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND AddresseeID=" + Security.CurrentUserID + " AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
                    if (bImAgreed)
                        SelectCommand = "SELECT * FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND AgreedUserID=" + Security.CurrentUserID + " AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
                    if (bImAproved)
                        SelectCommand = "SELECT * FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND ApprovedUserID=" + Security.CurrentUserID + " AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
                }
                else
                {
                    if (bImCreated)
                        SelectCommand = "SELECT * FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND OutputDone=0 AND CreateUserID=" + Security.CurrentUserID + " AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
                    if (bImAddressee)
                        SelectCommand = "SELECT * FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND OutputDone=0 AND AddresseeID=" + Security.CurrentUserID + " AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
                    if (bImAgreed)
                        SelectCommand = "SELECT * FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND OutputDone=0 AND AgreedUserID=" + Security.CurrentUserID + " AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
                    if (bImAproved)
                        SelectCommand = "SELECT * FROM VisitorsPermits" +
                        " WHERE PermitEnable=1 AND OutputDone=0 AND ApprovedUserID=" + Security.CurrentUserID + " AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
                }
            }
            if (AllPermits)
            {
                if (bNew)
                    SelectCommand = "SELECT * FROM VisitorsPermits" +
                    " WHERE PermitEnable=1 AND InputDone=0 AND OutputDone=0 AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
                if (bActive)
                    SelectCommand = "SELECT * FROM VisitorsPermits" +
                    " WHERE PermitEnable=1 AND InputDone=1 AND OutputDone=0 AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
                if (bOverdued)
                    SelectCommand = "SELECT * FROM VisitorsPermits" +
                    " WHERE PermitEnable=1 AND InputDone=0 AND OutputDone=0 AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
                if (bClose)
                    SelectCommand = "SELECT * FROM VisitorsPermits" +
                    " WHERE PermitEnable=1 AND InputDone=1 AND OutputDone=1 AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
            }
            if (ForApproval)
            {
                SelectCommand = "SELECT * FROM VisitorsPermits" +
                " WHERE PermitEnable=1 AND InputDone=0 AND ApprovedTime IS NULL AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
            }

            daPermits.SelectCommand.CommandText = SelectCommand;
            dtPermits.Clear();
            daPermits.Fill(dtPermits);
            bsPermits.MoveFirst();
            SetOverdued();
        }

        public void FilterPermitsByPerson(DateTime Date, bool SearchVisitor, string VisitorName,
            bool SearchAddressee, string AddresseeName)
        {
            string SelectCommand = "SELECT * FROM VisitorsPermits WHERE PermitEnable=1 AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";

            if (SearchVisitor)
            {
                SelectCommand = "SELECT * FROM VisitorsPermits" +
                " WHERE PermitEnable=1 AND VisitorName LIKE '%" + VisitorName + "%' AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
            }
            if (SearchAddressee)
            {
                SelectCommand = "SELECT * FROM VisitorsPermits" +
                " WHERE PermitEnable=1 AND AddresseeName LIKE '%" + AddresseeName + "%' AND CAST(Validity AS DATE) = '" + Date.ToString("yyyy-MM-dd") + "'";
            }

            daPermits.SelectCommand.CommandText = SelectCommand;
            dtPermits.Clear();
            daPermits.Fill(dtPermits);
            bsPermits.MoveFirst();
            SetOverdued();
        }

        private void SetOverdued()
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            for (int i = 0; i < dtPermits.Rows.Count; i++)
            {
                dtPermits.Rows[i]["Overdued"] = false;
                if (dtPermits.Rows[i]["Validity"] == DBNull.Value)
                    continue;
                DateTime Validity = Convert.ToDateTime(dtPermits.Rows[i]["Validity"]);
                if (Validity.Date < CurrentDate.Date)
                    dtPermits.Rows[i]["Overdued"] = true;
            }
        }

        public void ScanPermit(int PermitID)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM ScanPermits",
                    ConnectionStrings.LightConnectionString))
                {
                    using (new SqlCommandBuilder(DA))
                    {
                        DA.Fill(DT);
                        DataRow NewRow = DT.NewRow();
                        NewRow["ScanDateTime"] = Security.GetCurrentDate();
                        NewRow["ScanUserID"] = Security.CurrentUserID;
                        NewRow["PermitID"] = PermitID;
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                    }
                }
            }

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM Permits WHERE PermitID=" + PermitID,
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

        public void ScanVisitorPermit(int PermitID, int ScanType)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM ScanVisitorPermits",
                    ConnectionStrings.LightConnectionString))
                {
                    using (new SqlCommandBuilder(DA))
                    {
                        DA.Fill(DT);
                        DataRow NewRow = DT.NewRow();
                        NewRow["ScanDateTime"] = Security.GetCurrentDate();
                        NewRow["ScanUserID"] = Security.CurrentUserID;
                        NewRow["PermitID"] = PermitID;
                        NewRow["ScanType"] = ScanType;
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                    }
                }
            }
        }

        public void ScanUnload(int UnloadID, bool OutObject)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM ScanUnloads",
                    ConnectionStrings.LightConnectionString))
                {
                    using (new SqlCommandBuilder(DA))
                    {
                        DA.Fill(DT);
                        DataRow NewRow = DT.NewRow();
                        NewRow["ScanDateTime"] = Security.GetCurrentDate();
                        NewRow["ScanUserID"] = Security.CurrentUserID;
                        NewRow["UnloadID"] = UnloadID;
                        if (OutObject)
                            NewRow["ScanType"] = 1;
                        else
                            NewRow["ScanType"] = 0;
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                    }
                }
            }

            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM Unloads WHERE UnloadID=" + UnloadID,
                    ConnectionStrings.LightConnectionString))
                {
                    using (new SqlCommandBuilder(DA))
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            if (OutObject)
                                DT.Rows[0]["ReturnObject"] = true;
                            else
                                DT.Rows[0]["OutObject"] = true;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void ScanMachinesPermit(int MachinePermitID)
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

        public void NewPermit(string VisitorName, string VisitMission, int AddresseeID, string AddresseeName, DateTime Validity)
        {
            DataRow NewRow = dtPermits.NewRow();
            NewRow["CreateTime"] = Security.GetCurrentDate();
            NewRow["CreateUserID"] = Security.CurrentUserID;
            NewRow["VisitorName"] = VisitorName;
            NewRow["VisitMission"] = VisitMission;
            NewRow["AddresseeID"] = AddresseeID;
            //if (AddresseeName.Length > 0)
            NewRow["AddresseeName"] = AddresseeName;
            NewRow["Validity"] = Validity;
            dtPermits.Rows.Add(NewRow);
        }

        public void GetUsersInformation(int AddresseeID, string sAddresseeName, int InputDeniedUserID, int OutputAllowedUserID, int OutputDeniedUserID, int CreateUserID, int PrintUserID, int AgreedUserID, int ApprovedUserID, int DeleteUserID)
        {
            if (AddresseeID == 0)
                AddresseeName = sAddresseeName;
            else
                AddresseeName = GetUserName(AddresseeID);
            InputDeniedUserName = GetUserName(InputDeniedUserID);
            OutputAllowedUserName = GetUserName(OutputAllowedUserID);
            OutputDeniedUserName = GetUserName(OutputDeniedUserID);
            CreateUserName = GetUserName(CreateUserID);
            PrintUserName = GetUserName(PrintUserID);
            AgreedUserName = GetUserName(AgreedUserID);
            AprovedUserName = GetUserName(ApprovedUserID);
        }

        public string GetUserName(int UserID)
        {
            string Name = string.Empty;
            DataRow[] rows = dtUsers.Select("UserID=" + UserID);
            if (rows.Count() > 0)
                Name = rows[0]["ShortName"].ToString();

            return Name;
        }

        public void RemovePermit(int VisitorPermitID)
        {
            DataRow[] rows = dtPermits.Select("VisitorPermitID=" + VisitorPermitID);
            if (rows.Count() == 0 || rows[0]["DeleteTime"] != DBNull.Value)
                return;
            rows[0]["PermitEnable"] = false;
            rows[0]["DeleteUserID"] = Security.CurrentUserID;
            rows[0]["DeleteTime"] = Security.GetCurrentDate();
        }

        public void AgreePermit(int VisitorPermitID)
        {
            DataRow[] rows = dtPermits.Select("VisitorPermitID=" + VisitorPermitID);
            if (rows.Count() == 0 || rows[0]["AgreedTime"] != DBNull.Value)
                return;
            rows[0]["AgreedUserID"] = Security.CurrentUserID;
            rows[0]["AgreedTime"] = Security.GetCurrentDate();
            AllowOutput(VisitorPermitID);

        }

        public void AprovePermit(int VisitorPermitID)
        {
            DataRow[] rows = dtPermits.Select("VisitorPermitID=" + VisitorPermitID);
            if (rows.Count() == 0 || rows[0]["ApprovedTime"] != DBNull.Value)
                return;
            if (rows[0]["AgreedTime"] == DBNull.Value)
            {
                rows[0]["AgreedUserID"] = Security.CurrentUserID;
                rows[0]["AgreedTime"] = Security.GetCurrentDate();
            }
            rows[0]["ApprovedUserID"] = Security.CurrentUserID;
            rows[0]["ApprovedTime"] = Security.GetCurrentDate();
            rows[0]["InputEnable"] = 1;
            AllowOutput(VisitorPermitID);
        }

        public void PrintPermit(int VisitorPermitID)
        {
            DataRow[] rows = dtPermits.Select("VisitorPermitID=" + VisitorPermitID);
            if (rows.Count() == 0)
                return;
            rows[0]["PrintUserID"] = Security.CurrentUserID;
            rows[0]["PrintTime"] = Security.GetCurrentDate();
        }

        public void AllowInput(int VisitorPermitID)
        {
            DataRow[] rows = dtPermits.Select("VisitorPermitID=" + VisitorPermitID);
            if (rows.Count() == 0)
                return;
            rows[0]["InputDeniedUserID"] = DBNull.Value;
            rows[0]["InputEnable"] = true;
            rows[0]["InputDeniedTime"] = DBNull.Value;
        }

        public void DenyInput(int VisitorPermitID)
        {
            DataRow[] rows = dtPermits.Select("VisitorPermitID=" + VisitorPermitID);
            if (rows.Count() == 0)
                return;
            rows[0]["InputDeniedUserID"] = Security.CurrentUserID;
            rows[0]["InputEnable"] = false;
            rows[0]["InputDeniedTime"] = Security.GetCurrentDate();
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
            public string InputTime;
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
                InputTime = string.Empty;
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

        public struct ScanedMachinesPermitsInfo
        {
            public bool OutputDone;
            public bool OutputEnable;

            public int OutputDeniedUserID;
            public int CreateUserID;
            public int PrintUserID;
            public int AgreedUserID;


            public string Name;
            public string VisitMission;
            public string Validity;
            public string OutputDeniedTime;
            public string OutputDeniedUserName;
            public string OutputTime;
            public string CreateTime;
            public string CreateUserName;
            public string PrintTime;
            public string PrintUserName;
            public string AgreedTime;
            public string AgreedUserName;
            public void Clear()
            {
                OutputDone = false;
                OutputEnable = false;

                OutputDeniedUserID = 0;
                CreateUserID = 0;
                PrintUserID = 0;
                AgreedUserID = 0;

                Name = string.Empty;
                VisitMission = string.Empty;
                Validity = string.Empty;
                OutputDeniedTime = string.Empty;
                OutputDeniedUserName = string.Empty;
                OutputTime = string.Empty;
                CreateTime = string.Empty;
                CreateUserName = string.Empty;
                PrintTime = string.Empty;
                PrintUserName = string.Empty;
                AgreedTime = string.Empty;
                AgreedUserName = string.Empty;
            }
        }

        public bool IsPermitExist(int VisitorPermitID)
        {
            string SelectCommand = "SELECT * FROM VisitorsPermits WHERE VisitorPermitID=" + VisitorPermitID;
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

        public bool IsDispatchPermitExist(int PermitID)
        {
            string SelectCommand = "SELECT * FROM PermitDetails WHERE PermitID=" + PermitID;
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

        public bool IsUnloadExist(int UnloadID)
        {
            string SelectCommand = "SELECT * FROM Unloads WHERE UnloadID=" + UnloadID;
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

        public bool IsMachinesPermitExist(int MachinePermitID)
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

        public ScanedPermitsInfo InputDone(int VisitorPermitID)
        {
            ScanedPermitsInfo Struct = new ScanedPermitsInfo();
            Struct.Clear();

            string SelectCommand = "SELECT * FROM VisitorsPermits WHERE VisitorPermitID=" + VisitorPermitID;
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

                            if (!Struct.InputEnable)
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
                                Struct.Validity = Convert.ToDateTime(DT.Rows[0]["Validity"]).ToString("dd.MM.yyyy");
                            if (DT.Rows[0]["InputDeniedTime"] != DBNull.Value)
                                Struct.InputDeniedTime = Convert.ToDateTime(DT.Rows[0]["InputDeniedTime"]).ToString("dd.MM.yyyy HH:mm");
                            if (DT.Rows[0]["InputTime"] != DBNull.Value)
                                Struct.InputTime = Convert.ToDateTime(DT.Rows[0]["InputTime"]).ToString("dd.MM.yyyy HH:mm");
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

                            if (!Struct.InputDone)
                            {
                                DT.Rows[0]["InputDone"] = true;
                                DT.Rows[0]["InputTime"] = Security.GetCurrentDate();
                                DA.Update(DT);
                            }
                        }
                    }
                }
            }

            return Struct;
        }

        public ScanedPermitsInfo OutputDone(int VisitorPermitID)
        {
            ScanedPermitsInfo Struct = new ScanedPermitsInfo();
            Struct.Clear();

            string SelectCommand = "SELECT * FROM VisitorsPermits WHERE VisitorPermitID=" + VisitorPermitID;
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
                            if (DT.Rows[0]["InputTime"] != DBNull.Value)
                                Struct.InputTime = Convert.ToDateTime(DT.Rows[0]["InputTime"]).ToString("dd.MM.yyyy HH:mm");
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

        public ScanedMachinesPermitsInfo MachinesPermitDone(int MachinePermitID)
        {
            ScanedMachinesPermitsInfo Struct = new ScanedMachinesPermitsInfo();
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
                            Struct.OutputEnable = Convert.ToBoolean(DT.Rows[0]["OutputEnable"]);
                            Struct.OutputDone = Convert.ToBoolean(DT.Rows[0]["OutputDone"]);

                            if (!Struct.OutputEnable)
                            {
                                return Struct;
                            }


                            if (DT.Rows[0]["OutputDeniedUserID"] != DBNull.Value)
                                Struct.OutputDeniedUserID = Convert.ToInt32(DT.Rows[0]["OutputDeniedUserID"]);
                            if (DT.Rows[0]["CreateUserID"] != DBNull.Value)
                                Struct.CreateUserID = Convert.ToInt32(DT.Rows[0]["CreateUserID"]);
                            if (DT.Rows[0]["PrintUserID"] != DBNull.Value)
                                Struct.PrintUserID = Convert.ToInt32(DT.Rows[0]["PrintUserID"]);
                            if (DT.Rows[0]["AgreedUserID"] != DBNull.Value)
                                Struct.AgreedUserID = Convert.ToInt32(DT.Rows[0]["AgreedUserID"]);

                            if (DT.Rows[0]["Name"] != DBNull.Value)
                                Struct.Name = DT.Rows[0]["Name"].ToString();
                            if (DT.Rows[0]["VisitMission"] != DBNull.Value)
                                Struct.VisitMission = DT.Rows[0]["VisitMission"].ToString();
                            if (DT.Rows[0]["Validity"] != DBNull.Value)
                                Struct.Validity = Convert.ToDateTime(DT.Rows[0]["Validity"]).ToString("dd.MM.yyyy HH:mm");
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

                            Struct.OutputDeniedUserName = GetUserName(Struct.OutputDeniedUserID);
                            Struct.CreateUserName = GetUserName(Struct.CreateUserID);
                            Struct.PrintUserName = GetUserName(Struct.PrintUserID);
                            Struct.AgreedUserName = GetUserName(Struct.AgreedUserID);

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

        public void AllowOutput(int VisitorPermitID)
        {
            DataRow[] rows = dtPermits.Select("VisitorPermitID=" + VisitorPermitID);
            if (rows.Count() == 0)
                return;
            rows[0]["OutputAllowedUserID"] = Security.CurrentUserID;
            rows[0]["OutputAllowedTime"] = Security.GetCurrentDate();
            rows[0]["OutputDeniedUserID"] = DBNull.Value;
            rows[0]["OutputEnable"] = true;
            rows[0]["OutputDeniedTime"] = DBNull.Value;
        }

        public void DenyOutput(int VisitorPermitID)
        {
            DataRow[] rows = dtPermits.Select("VisitorPermitID=" + VisitorPermitID);
            if (rows.Count() == 0)
                return;
            rows[0]["OutputAllowedUserID"] = DBNull.Value;
            rows[0]["OutputAllowedTime"] = DBNull.Value;
            rows[0]["OutputDeniedUserID"] = Security.CurrentUserID;
            rows[0]["OutputEnable"] = false;
            rows[0]["OutputDeniedTime"] = Security.GetCurrentDate();
        }

        //public void OutputDone(int VisitorPermitID)
        //{
        //    DataRow[] rows = dtPermits.Select("VisitorPermitID=" + VisitorPermitID);
        //    if (rows.Count() == 0)
        //        return;
        //    rows[0]["OutputDone"] = true;
        //    rows[0]["OutputTime"] = Security.GetCurrentDate();
        //}

        public void MoveToVisitDateTime(DateTime VisitDateTime)
        {
            bsPermitsDates.Position = bsPermitsDates.Find("VisitDateTime", VisitDateTime);
        }

        public void MoveToPermit(int VisitorPermitID)
        {
            bsPermits.Position = bsPermits.Find("VisitorPermitID", VisitorPermitID);
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT RoleID, UserID FROM UserRoles WHERE RoleID=" + RoleID, ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        int UserID = Convert.ToInt32(DT.Rows[i]["UserID"]);
                        InfiniumMessages.SendMessage("Необходимо утвердить пропуск", UserID);
                    }
                }
            }
        }

        DataTable PackagesDT;
        private void FillPackages(int PermitID)
        {
            PackagesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(
                @"SELECT MainOrders.MegaOrderID, MegaOrders.OrderNumber, Packages.ProductType, Packages.FactoryID, Packages.MainOrderID, Packages.PackageID, Packages.PackageStatusID
				FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID
				INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
				WHERE Packages.DispatchID IN (SELECT DispatchID FROM infiniu2_light.dbo.PermitDetails WHERE PermitID=" + PermitID + ")", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(PackagesDT);
            }
        }

        private int[] GetOrderNumbers()
        {
            DataTable DT = new DataTable();

            using (DataView DV = new DataView(PackagesDT))
            {
                DT = DV.ToTable(true, new string[] { "OrderNumber" });
            }

            int[] rows = new int[DT.Rows.Count];

            for (int i = 0; i < DT.Rows.Count; i++)
                rows[i] = Convert.ToInt32(DT.Rows[i]["OrderNumber"]);
            DT.Dispose();
            return rows;
        }

        private int GetDispPackagesCount()
        {
            DataRow[] rows = PackagesDT.Select("PackageStatusID = 3");
            return rows.Count();
        }

        private int GetPackagesCount()
        {
            DataRow[] rows = PackagesDT.Select();
            return rows.Count();
        }

        private decimal GetSquare(int FactoryID, int PermitID)
        {
            decimal Square = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.Count AS PackageDetailsCount, FrontsOrders.Count AS FrontsOrdersCount, FrontsOrders.Square, FrontsOrders.Weight FROM PackageDetails 
					INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
					WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0 AND FactoryID = " + FactoryID +
                    " AND Packages.DispatchID IN (SELECT DispatchID FROM infiniu2_light.dbo.PermitDetails WHERE PermitID=" + PermitID + "))",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        if (DT.Rows[i]["Square"] != DBNull.Value && DT.Rows[i]["PackageDetailsCount"] != DBNull.Value && DT.Rows[i]["FrontsOrdersCount"] != DBNull.Value)
                            Square += Convert.ToDecimal(DT.Rows[i]["Square"]) * Convert.ToDecimal(DT.Rows[i]["PackageDetailsCount"]) / Convert.ToDecimal(DT.Rows[i]["FrontsOrdersCount"]);
                    }
                    Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);
                }
            }
            return Square;
        }

        private decimal GetWeight(int FactoryID, int PermitID)
        {
            decimal PackWeight = 0;
            decimal Weight = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.Count AS PackageDetailsCount, FrontsOrders.Count AS FrontsOrdersCount, FrontsOrders.Square, FrontsOrders.Weight FROM PackageDetails 
					INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID
					WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0 AND FactoryID = " + FactoryID +
                    " AND Packages.DispatchID IN (SELECT DispatchID FROM infiniu2_light.dbo.PermitDetails WHERE PermitID=" + PermitID + "))",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        if (DT.Rows[i]["Square"] != DBNull.Value && DT.Rows[i]["PackageDetailsCount"] != DBNull.Value && DT.Rows[i]["FrontsOrdersCount"] != DBNull.Value)
                            PackWeight += Convert.ToDecimal(0.7) * Convert.ToDecimal(DT.Rows[i]["Square"]) * Convert.ToDecimal(DT.Rows[i]["PackageDetailsCount"]) / Convert.ToDecimal(DT.Rows[i]["FrontsOrdersCount"]);
                        if (DT.Rows[i]["Weight"] != DBNull.Value && DT.Rows[i]["PackageDetailsCount"] != DBNull.Value && DT.Rows[i]["FrontsOrdersCount"] != DBNull.Value)
                            Weight += Convert.ToDecimal(DT.Rows[i]["Weight"]) * Convert.ToDecimal(DT.Rows[i]["PackageDetailsCount"]) / Convert.ToDecimal(DT.Rows[i]["FrontsOrdersCount"]);
                    }
                    Weight = PackWeight + Weight;
                }
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PackageDetails.Count AS PackageDetailsCount, DecorOrders.Count AS DecorOrdersCount, DecorOrders.Weight FROM PackageDetails 
					INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID
					WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 1 AND FactoryID = " + FactoryID +
                    " AND Packages.DispatchID IN (SELECT DispatchID FROM infiniu2_light.dbo.PermitDetails WHERE PermitID=" + PermitID + "))",
                    ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DT.Clear();
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        if (DT.Rows[i]["Weight"] != DBNull.Value && DT.Rows[i]["PackageDetailsCount"] != DBNull.Value && DT.Rows[i]["DecorOrdersCount"] != DBNull.Value)
                            Weight += Convert.ToDecimal(DT.Rows[i]["Weight"]) * Convert.ToDecimal(DT.Rows[i]["PackageDetailsCount"]) / Convert.ToDecimal(DT.Rows[i]["DecorOrdersCount"]);
                    }
                }
                Weight = Decimal.Round(Weight, 2, MidpointRounding.AwayFromZero);
            }

            return Weight;
        }

        private void GetConfirmDispDateTime(int PermitID, ref object ConfirmDispDateTime, ref object ConfirmDispUserID)
        {
            string SelectCommand = @"SELECT ConfirmDispDateTime, ConfirmDispUserID
				FROM Dispatch WHERE DispatchID IN (SELECT DispatchID FROM infiniu2_light.dbo.PermitDetails WHERE PermitID=" + PermitID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["ConfirmDispDateTime"] != DBNull.Value)
                            ConfirmDispDateTime = DT.Rows[0]["ConfirmDispDateTime"];
                        if (DT.Rows[0]["ConfirmDispUserID"] != DBNull.Value)
                            ConfirmDispUserID = DT.Rows[0]["ConfirmDispUserID"];
                    }
                }
            }
        }

        private void GetRealDispDateTime(int PermitID, ref object RealDispDateTime, ref object DispUserID)
        {
            string SelectCommand = @"SELECT MAX(DispatchDateTime) AS DispatchDateTime, DispUserID
				FROM Packages WHERE DispatchID IN (SELECT DispatchID FROM infiniu2_light.dbo.PermitDetails WHERE PermitID=" + PermitID + ") GROUP BY DispUserID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["DispatchDateTime"] != DBNull.Value)
                            RealDispDateTime = DT.Rows[0]["DispatchDateTime"];
                        if (DT.Rows[0]["DispUserID"] != DBNull.Value)
                            DispUserID = DT.Rows[0]["DispUserID"];
                    }
                }
            }
        }

        private string GetClientname(int PermitID)
        {
            string ClientName = string.Empty;
            string SelectCommand = @"SELECT Clients.ClientName FROM Dispatch 
				INNER JOIN infiniu2_marketingreference.dbo.Clients AS Clients ON Dispatch.ClientID = Clients.ClientID
				WHERE DispatchID IN (SELECT DispatchID FROM infiniu2_light.dbo.PermitDetails WHERE PermitID=" + PermitID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        if (DT.Rows[0]["ClientName"] != DBNull.Value)
                            ClientName = DT.Rows[0]["ClientName"].ToString();
                    }
                }
            }
            return ClientName;
        }

        public string GetDispatchInfo(int PermitID)
        {
            FillPackages(PermitID);
            string ClientName = GetClientname(PermitID);
            object ConfirmDispDateTime = DBNull.Value;
            object ConfirmDispUserID = DBNull.Value;
            object RealDispDateTime = DBNull.Value;
            object RealDispUserID = DBNull.Value;

            GetConfirmDispDateTime(PermitID, ref ConfirmDispDateTime, ref ConfirmDispUserID);
            GetRealDispDateTime(PermitID, ref RealDispDateTime, ref RealDispUserID);

            StringBuilder BarcodeNumber = new StringBuilder();

            int[] OrderNumbers = GetOrderNumbers();
            string OrderNumber = string.Empty;
            if (OrderNumbers.Count() > 0)
            {
                for (int i = 0; i < OrderNumbers.Count(); i++)
                    OrderNumber += OrderNumbers[i] + ", ";
                OrderNumber = OrderNumber.Substring(0, OrderNumber.Length - 2);
            }

            if (ConfirmDispDateTime != DBNull.Value)
            {
                BarcodeNumber.Append("Отгрузка разрешена: " + GetUserName(Convert.ToInt32(ConfirmDispUserID)) + " " + Convert.ToDateTime(ConfirmDispDateTime).ToString("dd.MM.yyyy HH:mm"));
                BarcodeNumber.Append(Environment.NewLine);
            }
            if (RealDispDateTime != DBNull.Value)
            {
                BarcodeNumber.Append("Отгрузка произведена: " + GetUserName(Convert.ToInt32(RealDispUserID)) + " " + Convert.ToDateTime(RealDispDateTime).ToString("dd.MM.yyyy HH:mm"));
                BarcodeNumber.Append(Environment.NewLine);
            }

            BarcodeNumber.Append("Клиент: " + ClientName);
            BarcodeNumber.Append(Environment.NewLine);
            BarcodeNumber.Append("Заказы: " + OrderNumber);
            BarcodeNumber.Append(Environment.NewLine);

            decimal Weight = GetWeight(1, PermitID) + GetWeight(2, PermitID);
            decimal TotalFrontsSquare = GetSquare(1, PermitID) + GetSquare(2, PermitID);

            BarcodeNumber.Append("Площадь: " + TotalFrontsSquare + " м.кв.");
            BarcodeNumber.Append(Environment.NewLine);
            BarcodeNumber.Append("Вес: " + Weight + " кг");
            BarcodeNumber.Append(Environment.NewLine);

            int PackedPackages = GetDispPackagesCount();
            int Packages = GetPackagesCount();
            BarcodeNumber.Append("Упаковок: " + PackedPackages + "/" + Packages);
            BarcodeNumber.Append(Environment.NewLine);

            return BarcodeNumber.ToString();
        }

        public string GetUnloadInfo(int UnloadID, ref bool OutObject)
        {
            StringBuilder BarcodeNumber = new StringBuilder();
            using (SqlDataAdapter DA = new SqlDataAdapter(
                @"SELECT * FROM Unloads WHERE UnloadID=" + UnloadID, ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        OutObject = Convert.ToBoolean(DT.Rows[0]["OutObject"]);
                        string UserName = DT.Rows[0]["UserName"].ToString();
                        int ResponsibleUserID = Convert.ToInt32(DT.Rows[0]["ResponsibleUserID"]);
                        bool NeedReturnObject = Convert.ToBoolean(DT.Rows[0]["NeedReturnObject"]);
                        //string ResponsibleUser = GetUserName(ResponsibleUserID);

                        BarcodeNumber.Append(UserName);
                        BarcodeNumber.Append(Environment.NewLine);

                        DataTable GoodsDT = new DataTable();

                        using (SqlDataAdapter sDA = new SqlDataAdapter(
                            @"SELECT Goods.*, M.Measure FROM Goods INNER JOIN infiniu2_catalog.dbo.Measures AS M ON Goods.MeasureID=M.MeasureID WHERE UnloadID=" + UnloadID, ConnectionStrings.LightConnectionString))
                        {
                            if (sDA.Fill(GoodsDT) > 0)
                            {
                                string SubjectName = GoodsDT.Rows[0]["SubjectName"].ToString();
                                string Measure = GoodsDT.Rows[0]["Measure"].ToString();
                                int Count = Convert.ToInt32(GoodsDT.Rows[0]["Count"]);

                                BarcodeNumber.Append(SubjectName + " " + Count + " " + Measure);
                                BarcodeNumber.Append(Environment.NewLine);
                            }
                        }
                        if (NeedReturnObject)
                        {
                            DateTime UnloadDateTime = Convert.ToDateTime(DT.Rows[0]["UnloadDateTime"]);
                            BarcodeNumber.Append("С возвратом до " + UnloadDateTime.ToString("dd.MM.yyyy"));
                            BarcodeNumber.Append(Environment.NewLine);
                        }
                    }
                    else
                    {
                        return "Пропуска не существует";
                    }
                }
            }
            return BarcodeNumber.ToString();
        }
    }

    public struct PrintPermitsInfo
    {
        public string BarcodeNumber;
        public string VisitorName;
        public string VisitMission;
        public string AddresseeName;
        public string CreateUserName;
        public string PrintUserName;
        public string AgreedUserName;
        public string AprovedUserName;

        public string InputTime;
        public string CreateTime;
        public string PrintTime;
        public string AgreedTime;
        public string ApprovedTime;
    }

    public class PrintMachinesPermits
    {
        Infinium.Modules.Packages.Marketing.Barcode Barcode;
        public PrintDocument PD;

        public int PaperHeight = 488;
        public int PaperWidth = 794;

        public int CurrentLabelNumber = 0;

        public int PrintedCount = 0;

        public bool Printed = false;

        SolidBrush FontBrush;

        Font HeaderFont;
        Font OrdinaryFont;

        Pen Pen;

        Image ZTTPS;
        Image ZTProfil;
        Image STB;
        Image RST;

        public ArrayList LabelInfo;

        public PrintMachinesPermits()
        {
            Barcode = new Modules.Packages.Marketing.Barcode();

            InitializeFonts();
            InitializePrinter();

            ZTTPS = new Bitmap(Properties.Resources.ZOVTPS);
            ZTProfil = new Bitmap(Properties.Resources.ZOVPROFIL);
            STB = new Bitmap(Properties.Resources.STB);
            RST = new Bitmap(Properties.Resources.RST);

            LabelInfo = new System.Collections.ArrayList();
        }

        private void InitializePrinter()
        {
            PD = new PrintDocument();
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth, PaperHeight);
            PD.DefaultPageSettings.Landscape = true;
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Left = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
        }

        private void InitializeFonts()
        {
            FontBrush = new System.Drawing.SolidBrush(Color.Black);
            HeaderFont = new Font("Calibri", 12.0f, FontStyle.Regular);
            OrdinaryFont = new Font("Calibri", 12.0f, FontStyle.Regular);
            Pen = new Pen(FontBrush)
            {
                Width = 1
            };
        }

        public void ClearLabelInfo()
        {
            CurrentLabelNumber = 0;
            PrintedCount = 0;
            LabelInfo.Clear();
            GC.Collect();
        }

        public void AddLabelInfo(ref PrintPermitsInfo LabelInfoItem)
        {
            LabelInfo.Add(LabelInfoItem);
        }

        public void PrintPage(object sender, PrintPageEventArgs ev)
        {
            if (PrintedCount >= LabelInfo.Count)
                return;
            else
                PrintedCount++;
            ev.Graphics.Clear(Color.White);
            ev.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            int Y = 0;

            //ev.Graphics.DrawLine(Pen, 235, 315, 235, 385);

            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).VisitorName.Length > 0)
            {
                ev.Graphics.DrawString("Авто: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).VisitorName, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).VisitMission.Length > 0)
            {
                ev.Graphics.DrawString("Цель: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).VisitMission, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).AddresseeName.Length > 0)
            {
                ev.Graphics.DrawString("К кому: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).AddresseeName, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).CreateTime.Length > 0)
            {
                ev.Graphics.DrawString("Пропуск создан: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).CreateTime +
                    " " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).CreateUserName, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).AgreedTime.Length > 0)
            {
                ev.Graphics.DrawString("Пропуск утвержден: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).AgreedTime +
                    " " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).AgreedUserName, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).ApprovedTime.Length > 0)
            {
                ev.Graphics.DrawString("Пропуск утвержден: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).ApprovedTime +
                    " " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).AprovedUserName, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).PrintTime.Length > 0)
            {
                ev.Graphics.DrawString("Пропуск распечатан: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).PrintTime +
                    " " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).PrintUserName, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).InputTime.Length > 0)
            {
                ev.Graphics.DrawString("Вход выполнен: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).InputTime, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.Marketing.Barcode.BarcodeLength.Short, 35, ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 0, Y);
            Barcode.DrawBarcodeText(Modules.Packages.Marketing.Barcode.BarcodeLength.Short, ev.Graphics, ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 4, Y + 35);

            if (CurrentLabelNumber == LabelInfo.Count - 1 || PrintedCount >= LabelInfo.Count)
                ev.HasMorePages = false;

            if (CurrentLabelNumber < LabelInfo.Count - 1 && PrintedCount < LabelInfo.Count)
            {
                ev.HasMorePages = true;
                CurrentLabelNumber++;
            }
        }

        public void Print()
        {
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth + 40, PaperHeight);
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
            PD.DefaultPageSettings.Margins.Left = 0;

            if (!Printed)
            {
                Printed = true;
                PD.PrintPage += new PrintPageEventHandler(PrintPage);
            }

            PD.Print();
        }
    }


    public class PrintVisitorPermits
    {
        Infinium.Modules.Packages.Marketing.Barcode Barcode;
        public PrintDocument PD;

        public int PaperHeight = 488;
        public int PaperWidth = 794;

        public int CurrentLabelNumber = 0;

        public int PrintedCount = 0;

        public bool Printed = false;

        SolidBrush FontBrush;

        Font HeaderFont;
        Font OrdinaryFont;

        Pen Pen;

        Image ZTTPS;
        Image ZTProfil;
        Image STB;
        Image RST;

        public ArrayList LabelInfo;

        public PrintVisitorPermits()
        {
            Barcode = new Modules.Packages.Marketing.Barcode();

            InitializeFonts();
            InitializePrinter();

            ZTTPS = new Bitmap(Properties.Resources.ZOVTPS);
            ZTProfil = new Bitmap(Properties.Resources.ZOVPROFIL);
            STB = new Bitmap(Properties.Resources.STB);
            RST = new Bitmap(Properties.Resources.RST);

            LabelInfo = new System.Collections.ArrayList();
        }

        private void InitializePrinter()
        {
            PD = new PrintDocument();
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth, PaperHeight);
            PD.DefaultPageSettings.Landscape = true;
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Left = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
        }

        private void InitializeFonts()
        {
            FontBrush = new System.Drawing.SolidBrush(Color.Black);
            HeaderFont = new Font("Calibri", 12.0f, FontStyle.Regular);
            OrdinaryFont = new Font("Calibri", 12.0f, FontStyle.Regular);
            Pen = new Pen(FontBrush)
            {
                Width = 1
            };
        }

        public void ClearLabelInfo()
        {
            CurrentLabelNumber = 0;
            PrintedCount = 0;
            LabelInfo.Clear();
            GC.Collect();
        }

        public void AddLabelInfo(ref PrintPermitsInfo LabelInfoItem)
        {
            LabelInfo.Add(LabelInfoItem);
        }

        public void PrintPage(object sender, PrintPageEventArgs ev)
        {
            if (PrintedCount >= LabelInfo.Count)
                return;
            else
                PrintedCount++;
            ev.Graphics.Clear(Color.White);
            ev.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            int Y = 0;

            //ev.Graphics.DrawLine(Pen, 235, 315, 235, 385);

            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).VisitorName.Length > 0)
            {
                ev.Graphics.DrawString("Посетитель: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).VisitorName, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).VisitMission.Length > 0)
            {
                ev.Graphics.DrawString("Цель: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).VisitMission, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).AddresseeName.Length > 0)
            {
                ev.Graphics.DrawString("К кому: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).AddresseeName, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).CreateTime.Length > 0)
            {
                ev.Graphics.DrawString("Пропуск создан: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).CreateTime +
                    " " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).CreateUserName, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).AgreedTime.Length > 0)
            {
                ev.Graphics.DrawString("Пропуск согласован: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).AgreedTime +
                    " " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).AgreedUserName, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).ApprovedTime.Length > 0)
            {
                ev.Graphics.DrawString("Пропуск утвержден: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).ApprovedTime +
                    " " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).AprovedUserName, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).PrintTime.Length > 0)
            {
                ev.Graphics.DrawString("Пропуск распечатан: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).PrintTime +
                    " " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).PrintUserName, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            if (((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).InputTime.Length > 0)
            {
                ev.Graphics.DrawString("Вход выполнен: " + ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).InputTime, OrdinaryFont, FontBrush, 0, Y);
                Y += 20;
            }
            ev.Graphics.DrawImage(Barcode.GetBarcode(Modules.Packages.Marketing.Barcode.BarcodeLength.Short, 35, ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 0, Y);
            Barcode.DrawBarcodeText(Modules.Packages.Marketing.Barcode.BarcodeLength.Short, ev.Graphics, ((PrintPermitsInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 4, Y + 35);

            if (CurrentLabelNumber == LabelInfo.Count - 1 || PrintedCount >= LabelInfo.Count)
                ev.HasMorePages = false;

            if (CurrentLabelNumber < LabelInfo.Count - 1 && PrintedCount < LabelInfo.Count)
            {
                ev.HasMorePages = true;
                CurrentLabelNumber++;
            }
        }

        public void Print()
        {
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth + 40, PaperHeight);
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
            PD.DefaultPageSettings.Margins.Left = 0;

            if (!Printed)
            {
                Printed = true;
                PD.PrintPage += new PrintPageEventHandler(PrintPage);
            }

            PD.Print();
        }
    }


    public class HistoryScanPermits
    {
        private DataTable _scanInputPermitsDt;
        private DataTable _scanOutputPermitsDt;
        private DataTable _scanMachinesPermitsDt;
        private DataTable _scanDispatchPermitsDt;
        private DataTable _scanUnloadsDt;
        private DataTable _rolePermissionsDt;
        private DataTable _usersDt;
        private DataTable _scanUsersDt;

        public BindingSource ScanInputPermitsBs;
        public BindingSource ScanOutputPermitsBs;
        public BindingSource ScanMachinesPermitsBs;
        public BindingSource ScanDispatchPermitsBs;
        public BindingSource ScanUnloadsPermitsBs;
        public BindingSource ScanUsersBs;

        public HistoryScanPermits()
        {
            Create();
            Fill();
            Binding();
        }

        private void Create()
        {
            _scanInputPermitsDt = new DataTable();
            _scanOutputPermitsDt = new DataTable();
            _scanMachinesPermitsDt = new DataTable();
            _scanDispatchPermitsDt = new DataTable();
            _scanUnloadsDt = new DataTable();
            _usersDt = new DataTable();
            _scanUsersDt = new DataTable();
            _rolePermissionsDt = new DataTable();
        }

        private void Fill()
        {
            var selectCommand = @"SELECT S.*, V.VisitorName, V.VisitMission, V.AddresseeName, V.InputTime FROM ScanVisitorPermits AS S
INNER JOIN VisitorsPermits AS V ON S.PermitID=V.VisitorPermitID WHERE CAST(ScanDateTime AS DATE) >= '" + DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") + "' AND ScanType =0 ORDER BY ScanVisitorPermitID DESC";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                da.Fill(_scanInputPermitsDt);
            }
            selectCommand = @"SELECT S.*, V.VisitorName, V.VisitMission, V.AddresseeName, V.OutputTime FROM ScanVisitorPermits AS S
INNER JOIN VisitorsPermits AS V ON S.PermitID=V.VisitorPermitID WHERE CAST(ScanDateTime AS DATE) >= '" + DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") + "' AND ScanType=1 ORDER BY ScanVisitorPermitID DESC";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                da.Fill(_scanOutputPermitsDt);
            }
            selectCommand = @"SELECT S.*, V.Name, V.VisitMission, V.OutputTime FROM ScanMachinesPermits AS S
INNER JOIN MachinesPermits AS V ON S.MachinePermitID=V.MachinePermitID WHERE CAST(ScanDateTime AS DATE) >= '" + DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") + "' ORDER BY ScanMachinePermitID DESC";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                da.Fill(_scanMachinesPermitsDt);
            }
            selectCommand = @"SELECT * FROM ScanPermits WHERE CAST(ScanDateTime AS DATE) >= '" + DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") + "' ORDER BY ScanPermitID DESC";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                da.Fill(_scanDispatchPermitsDt);
            }
            selectCommand = @"SELECT S.*, G.SubjectName, G.Count, M.Measure FROM ScanUnloads AS S
INNER JOIN Goods AS G ON S.UnloadID=G.UnloadID
INNER JOIN infiniu2_catalog.dbo.Measures AS M ON G.MeasureID=M.MeasureID WHERE CAST(ScanDateTime AS DATE) >= '" + DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") + "' ORDER BY ScanUnloadID DESC";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                da.Fill(_scanUnloadsDt);
                _scanUnloadsDt.Columns.Add(new DataColumn("Subject", Type.GetType("System.String")));
                for (int i = 0; i < _scanUnloadsDt.Rows.Count; i++)
                {
                    _scanUnloadsDt.Rows[i]["Subject"] = _scanUnloadsDt.Rows[i]["SubjectName"] + " "
                        + _scanUnloadsDt.Rows[i]["Count"] + _scanUnloadsDt.Rows[i]["Measure"];
                }
            }
            selectCommand = @"SELECT UserID, Name, ShortName FROM Users ORDER BY Name";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.UsersConnectionString))
            {
                da.Fill(_usersDt);
                _usersDt.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
                for (var i = 0; i < _usersDt.Rows.Count; i++)
                    _usersDt.Rows[i]["Check"] = false;
            }
            //			selectCommand = @"SELECT DISTINCT ScanUserID, U.Name FROM ScanPermits 
            //INNER JOIN infiniu2_users.dbo.Users AS U ON ScanPermits.ScanUserID=U.UserID WHERE CAST(ScanDateTime AS DATE) >= '" + DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") + "' ORDER BY U.Name";
            //			using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            //			{
            //				da.Fill(_scanUsersDt);
            //				_scanUsersDt.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
            //				for (var i = 0; i < _scanUsersDt.Rows.Count; i++)
            //					_scanUsersDt.Rows[i]["Check"] = false;
            //			}

            _scanUsersDt.Columns.Add(new DataColumn("ScanUserID", Type.GetType("System.Int64")));
            _scanUsersDt.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            _scanUsersDt.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));

            GetScanUsers(_scanInputPermitsDt);
            GetScanUsers(_scanOutputPermitsDt);
            GetScanUsers(_scanMachinesPermitsDt);
            GetScanUsers(_scanDispatchPermitsDt);
            GetScanUsers(_scanUnloadsDt);

        }

        private void GetScanUsers(DataTable _table)
        {
            for (int i = 0; i < _table.Rows.Count; i++)
            {
                int ScanUserID = Convert.ToInt32(_table.Rows[i]["ScanUserID"]);
                DataRow[] rows = _scanUsersDt.Select("ScanUserID=" + ScanUserID);
                if (rows.Count() == 0)
                {
                    DataRow NewRow = _scanUsersDt.NewRow();
                    NewRow["ScanUserID"] = ScanUserID;
                    NewRow["Name"] = GetUserName(ScanUserID);
                    NewRow["Check"] = false;
                    _scanUsersDt.Rows.Add(NewRow);
                }
            }
        }

        public string GetUserName(int UserID)
        {
            string Name = string.Empty;
            DataRow[] rows = _usersDt.Select("UserID=" + UserID);
            if (rows.Count() > 0)
                Name = rows[0]["ShortName"].ToString();

            return Name;
        }

        private void Binding()
        {
            ScanUsersBs = new BindingSource { DataSource = _scanUsersDt };

            ScanInputPermitsBs = new BindingSource { DataSource = _scanInputPermitsDt };
            ScanOutputPermitsBs = new BindingSource { DataSource = _scanOutputPermitsDt };
            ScanMachinesPermitsBs = new BindingSource { DataSource = _scanMachinesPermitsDt };
            ScanDispatchPermitsBs = new BindingSource { DataSource = _scanDispatchPermitsDt };
            ScanUnloadsPermitsBs = new BindingSource { DataSource = _scanUnloadsDt };
        }

        public DataGridViewComboBoxColumn ScanUserNameColumn
        {
            get
            {
                var column = new DataGridViewComboBoxColumn
                {
                    HeaderText = @"Сканировал",
                    Name = "ScanUserName",
                    DataPropertyName = "ScanUserID",
                    DataSource = new DataView(_usersDt),
                    ValueMember = "UserID",
                    DisplayMember = "Name",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return column;
            }
        }

        public void UpdateDispatchScans(bool bUsers, bool bDate, DateTime from, DateTime to)
        {
            var filter = "";
            if (bUsers)
            {
                if (SelectedUsers.Count > 0)
                {
                    if (filter.Length > 0)
                        filter +=
                            " AND ScanUserID IN (" +
                            string.Join(",", SelectedUsers.OfType<Int32>().ToArray()) + ")";
                    else
                        filter =
                            " WHERE ScanUserID IN (" +
                            string.Join(",", SelectedUsers.OfType<Int32>().ToArray()) + ")";
                }
                else
                    filter = " WHERE ScanPermitID =-1";
            }
            if (bDate)
            {
                if (filter.Length > 0)
                    filter += " AND CAST(ScanDateTime AS DATE) >= '" + from.ToString("yyyy-MM-dd") +
                              "' AND CAST(ScanDateTime AS DATE) <= '" + to.ToString("yyyy-MM-dd") + "' ";
                else
                    filter = " WHERE CAST(ScanDateTime AS DATE) >= '" + from.ToString("yyyy-MM-dd") +
                             "' AND CAST(ScanDateTime AS DATE) <= '" + to.ToString("yyyy-MM-dd") + "' ";
            }

            var selectCommand = @"SELECT * FROM ScanPermits " + filter + " ORDER BY ScanPermitID DESC";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                _scanDispatchPermitsDt.Clear();
                da.Fill(_scanDispatchPermitsDt);
            }

            DataTable DT = new DataTable();

            using (DataView DV = new DataView(_scanDispatchPermitsDt))
            {
                DT = DV.ToTable(true, new string[] { "ScanUserID" });
            }
            filter = "";
            if (bUsers)
            {
                if (SelectedUsers.Count > 0)
                {
                    if (filter.Length > 0)
                        filter +=
                            " AND ScanUserID IN (" +
                            string.Join(",", SelectedUsers.OfType<Int32>().ToArray()) + ")";
                    else
                        filter =
                            " WHERE ScanUserID IN (" +
                            string.Join(",", SelectedUsers.OfType<Int32>().ToArray()) + ")";
                }
                else
                    filter = " WHERE ScanVisitorPermitID =-1";
            }
            if (bDate)
            {
                if (filter.Length > 0)
                    filter += " AND CAST(ScanDateTime AS DATE) >= '" + from.ToString("yyyy-MM-dd") +
                              "' AND CAST(ScanDateTime AS DATE) <= '" + to.ToString("yyyy-MM-dd") + "' ";
                else
                    filter = " WHERE CAST(ScanDateTime AS DATE) >= '" + from.ToString("yyyy-MM-dd") +
                             "' AND CAST(ScanDateTime AS DATE) <= '" + to.ToString("yyyy-MM-dd") + "' ";
            }

            if (filter.Length > 0)
                selectCommand = @"SELECT S.*, V.VisitorName, V.VisitMission, V.AddresseeName, V.InputTime FROM ScanVisitorPermits AS S
INNER JOIN VisitorsPermits AS V ON S.PermitID=V.VisitorPermitID " + filter + " AND ScanType=0 ORDER BY ScanVisitorPermitID DESC";
            else
                selectCommand = @"SELECT S.*, V.VisitorName, V.VisitMission, V.AddresseeName, V.InputTime FROM ScanVisitorPermits AS S
INNER JOIN VisitorsPermits AS V ON S.PermitID=V.VisitorPermitID WHERE ScanType=0 ORDER BY ScanVisitorPermitID DESC";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                _scanInputPermitsDt.Clear();
                da.Fill(_scanInputPermitsDt);
            }

            if (filter.Length > 0)
                selectCommand = @"SELECT S.*, V.VisitorName, V.VisitMission, V.AddresseeName, V.OutputTime FROM ScanVisitorPermits AS S
INNER JOIN VisitorsPermits AS V ON S.PermitID=V.VisitorPermitID " + filter + " AND ScanType=1 ORDER BY ScanVisitorPermitID DESC";
            else
                selectCommand = @"SELECT S.*, V.VisitorName, V.VisitMission, V.AddresseeName, V.OutputTime FROM ScanVisitorPermits AS S
INNER JOIN VisitorsPermits AS V ON S.PermitID=V.VisitorPermitID WHERE ScanType=1 ORDER BY ScanVisitorPermitID DESC";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                _scanOutputPermitsDt.Clear();
                da.Fill(_scanOutputPermitsDt);
            }

            if (filter.Length > 0)
                selectCommand = @"SELECT S.*, V.Name, V.VisitMission, V.OutputTime FROM ScanMachinesPermits AS S
INNER JOIN MachinesPermits AS V ON S.MachinePermitID=V.MachinePermitID " + filter + " ORDER BY ScanMachinePermitID DESC";
            else
                selectCommand = @"SELECT S.*, V.Name, V.VisitMission, V.OutputTime FROM ScanMachinesPermits AS S
INNER JOIN MachinesPermits AS V ON S.MachinePermitID=V.MachinePermitID ORDER BY ScanMachinePermitID DESC";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                _scanMachinesPermitsDt.Clear();
                da.Fill(_scanMachinesPermitsDt);
            }

            filter = "";
            if (bUsers)
            {
                if (SelectedUsers.Count > 0)
                {
                    if (filter.Length > 0)
                        filter +=
                            " AND ScanUserID IN (" +
                            string.Join(",", SelectedUsers.OfType<Int32>().ToArray()) + ")";
                    else
                        filter =
                            " WHERE ScanUserID IN (" +
                            string.Join(",", SelectedUsers.OfType<Int32>().ToArray()) + ")";
                }
                else
                    filter = " WHERE ScanUnloadID =-1";
            }
            if (bDate)
            {
                if (filter.Length > 0)
                    filter += " AND CAST(ScanDateTime AS DATE) >= '" + from.ToString("yyyy-MM-dd") +
                              "' AND CAST(ScanDateTime AS DATE) <= '" + to.ToString("yyyy-MM-dd") + "' ";
                else
                    filter = " WHERE CAST(ScanDateTime AS DATE) >= '" + from.ToString("yyyy-MM-dd") +
                             "' AND CAST(ScanDateTime AS DATE) <= '" + to.ToString("yyyy-MM-dd") + "' ";
            }

            selectCommand = @"SELECT S.*, G.SubjectName, G.Count, M.Measure FROM ScanUnloads AS S
INNER JOIN Goods AS G ON S.UnloadID=G.UnloadID
INNER JOIN infiniu2_catalog.dbo.Measures AS M ON G.MeasureID=M.MeasureID " + filter + " ORDER BY ScanUnloadID DESC";
            using (var da = new SqlDataAdapter(selectCommand, ConnectionStrings.LightConnectionString))
            {
                _scanUnloadsDt.Clear();
                da.Fill(_scanUnloadsDt);
                for (int i = 0; i < _scanUnloadsDt.Rows.Count; i++)
                {
                    _scanUnloadsDt.Rows[i]["Subject"] = _scanUnloadsDt.Rows[i]["SubjectName"] + " "
                        + _scanUnloadsDt.Rows[i]["Count"] + _scanUnloadsDt.Rows[i]["Measure"];
                }
            }

            _scanUsersDt.Clear();
            GetScanUsers(_scanInputPermitsDt);
            GetScanUsers(_scanOutputPermitsDt);
            GetScanUsers(_scanMachinesPermitsDt);
            GetScanUsers(_scanDispatchPermitsDt);
            GetScanUsers(_scanUnloadsDt);
        }

        public void MoveToPosition(int plannedWorkId)
        {
            ScanInputPermitsBs.Position = ScanInputPermitsBs.Find("PlannedWorkID", plannedWorkId);
        }

        public ArrayList SelectedUsers
        {
            get
            {
                var users = new ArrayList();

                for (var i = 0; i < _scanUsersDt.Rows.Count; i++)
                {
                    if (!Convert.ToBoolean(_scanUsersDt.Rows[i]["Check"]))
                        continue;

                    users.Add(Convert.ToInt32(_scanUsersDt.Rows[i]["ScanUserID"]));
                }

                return users;
            }
        }

        public void SelectAllUsers(bool check)
        {
            for (var i = 0; i < _scanUsersDt.Rows.Count; i++)
            {
                _scanUsersDt.Rows[i]["Check"] = check;
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

    }

    public class HistoryDispatch
    {
        private DataTable FrontsDataTable = null;
        private DataTable FrameColorsDataTable = null;
        private DataTable TechnoProfilesDataTable = null;
        private DataTable PatinaDataTable = null;
        private DataTable PatinaRALDataTable = null;
        private DataTable InsetTypesDataTable = null;
        private DataTable InsetColorsDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;

        DataTable DispMainOrdersDT = null;

        DataTable MainOrdersDT;
        DataTable PackagesDT;
        DataTable FrontsOrdersDT;
        DataTable DecorOrdersDT;

        BindingSource MainOrdersBS;
        BindingSource PackagesBS;
        BindingSource FrontsOrdersBS;
        BindingSource DecorOrdersBS;

        public HistoryDispatch()
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
            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();

            DispMainOrdersDT = new DataTable();

            MainOrdersDT = new DataTable();
            PackagesDT = new DataTable();
            FrontsOrdersDT = new DataTable();
            DecorOrdersDT = new DataTable();

            MainOrdersBS = new BindingSource();
            PackagesBS = new BindingSource();
            FrontsOrdersBS = new BindingSource();
            DecorOrdersBS = new BindingSource();
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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColors WHERE GroupID IN (2,3,4,5,6,21,22)", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow[] rows = FrameColorsDataTable.Select("ColorID=" + Convert.ToInt32(DT.Rows[i]["InsetColorID"]));
                        if (rows.Count() == 0)
                        {
                            DataRow NewRow = FrameColorsDataTable.NewRow();
                            NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                            NewRow["ColorName"] = DT.Rows[i]["InsetColorName"].ToString();
                            FrameColorsDataTable.Rows.Add(NewRow);
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
            string SelectCommand = @"SELECT TOP 0 M.MainOrderID, C.ClientName, M.Weight, M.ProfilPackCount, M.TPSPackCount, M.Notes, M.FactoryID FROM MainOrders AS M
INNER JOIN MegaOrders ON M.MegaOrderID=MegaOrders.MegaOrderID
INNER JOIN infiniu2_marketingreference.dbo.Clients AS C ON MegaOrders.ClientID=C.ClientID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(MainOrdersDT);
            }
            MainOrdersDT.Columns.Add(new DataColumn("ProfilDispatchedCount", Type.GetType("System.String")));
            MainOrdersDT.Columns.Add(new DataColumn("ProfilDispPercentage", Type.GetType("System.Int32")));

            MainOrdersDT.Columns.Add(new DataColumn("TPSDispatchedCount", Type.GetType("System.String")));
            MainOrdersDT.Columns.Add(new DataColumn("TPSDispPercentage", Type.GetType("System.Int32")));

            SelectCommand = @"SELECT TOP 0 Packages.PackNumber, Packages.ProductType, Packages.MainOrderID, PackageStatus, FactoryName, Packages.PackingDateTime,
				Packages.StorageDateTime, Packages.ExpeditionDateTime, Packages.DispatchDateTime, Packages.PackageID, Packages.DispatchID, Packages.TrayID FROM Packages
				INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID
				INNER JOIN infiniu2_catalog.dbo.PackageStatuses ON Packages.PackageStatusID = infiniu2_catalog.dbo.PackageStatuses.PackageStatusID
				ORDER BY PackNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(PackagesDT);
            }

            SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1))) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
				INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1 ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }
            SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
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

            SelectCommand = @"SELECT TOP 0  FrontsOrdersID, MainOrderID, FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, Count, Square, Notes FROM FrontsOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDT);
            }
            SelectCommand = @"SELECT TOP 0 DecorOrderID, ProductID, DecorID, ColorID, PatinaID, Height, Length, Width, Count, Notes
				FROM DecorOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDT);
            }
        }

        private void Binding()
        {
            MainOrdersBS.DataSource = MainOrdersDT;
            PackagesBS.DataSource = PackagesDT;
            FrontsOrdersBS.DataSource = FrontsOrdersDT;
            DecorOrdersBS.DataSource = DecorOrdersDT;
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
                    DataSource = new DataView(InsetTypesDataTable),
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
                    DataSource = new DataView(InsetColorsDataTable),
                    ValueMember = "InsetColorID",
                    DisplayMember = "InsetColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return Column;
            }
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

                    DataSource = new DataView(FrameColorsDataTable),
                    ValueMember = "ColorID",
                    DisplayMember = "ColorName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                return ColorsColumn;
            }
        }

        public DataGridViewComboBoxColumn DecorPatinaColumn
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

        public string PatinaDisplayName(int PatinaID)
        {
            DataRow[] rows = PatinaDataTable.Select("PatinaID = " + PatinaID);
            if (rows.Count() > 0)
                return rows[0]["DisplayName"].ToString();
            return string.Empty;
        }

        public void FillMainPercentageColumn()
        {
            int MainOrderProfilDispCount = 0;
            int MainOrderProfilAllCount = 0;

            int ProfilDispPercentage = 0;

            decimal ProfilDispProgressVal = 0;

            int MainOrderTPSDispCount = 0;
            int MainOrderTPSAllCount = 0;

            int TPSDispPercentage = 0;

            decimal TPSDispProgressVal = 0;

            decimal d3 = 0;
            decimal d6 = 0;

            for (int i = 0; i < MainOrdersDT.Rows.Count; i++)
            {
                int MainOrderID = Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]);

                MainOrderProfilDispCount = GetMainOrderDispCount(Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]), 1);
                MainOrderProfilAllCount = Convert.ToInt32(MainOrdersDT.Rows[i]["ProfilPackCount"]);

                MainOrderTPSDispCount = GetMainOrderDispCount(Convert.ToInt32(MainOrdersDT.Rows[i]["MainOrderID"]), 2);
                MainOrderTPSAllCount = Convert.ToInt32(MainOrdersDT.Rows[i]["TPSPackCount"]);

                if (MainOrderTPSAllCount == 0 && MainOrderTPSDispCount > 0)
                    MessageBox.Show("Внутрення ошибка Infininum (деление на ноль). Сообщите администратору");

                if (MainOrderProfilAllCount == 0 && MainOrderProfilDispCount > 0)
                    MessageBox.Show("Внутрення ошибка Infininum (деление на ноль). Сообщите администратору");

                ProfilDispProgressVal = 0;

                TPSDispProgressVal = 0;

                if (MainOrderProfilAllCount > 0)
                    ProfilDispProgressVal = Convert.ToDecimal(Convert.ToDecimal(MainOrderProfilDispCount) / Convert.ToDecimal(MainOrderProfilAllCount));

                if (MainOrderTPSAllCount > 0)
                    TPSDispProgressVal = Convert.ToDecimal(Convert.ToDecimal(MainOrderTPSDispCount) / Convert.ToDecimal(MainOrderTPSAllCount));

                d3 = ProfilDispProgressVal * 100;
                d6 = TPSDispProgressVal * 100;

                ProfilDispPercentage = Convert.ToInt32(Math.Truncate(d3));

                TPSDispPercentage = Convert.ToInt32(Math.Truncate(d6));

                MainOrdersDT.Rows[i]["ProfilDispatchedCount"] = MainOrderProfilDispCount + " / " + MainOrderProfilAllCount;
                MainOrdersDT.Rows[i]["ProfilDispPercentage"] = ProfilDispPercentage;

                MainOrdersDT.Rows[i]["TPSDispatchedCount"] = MainOrderTPSDispCount + " / " + MainOrderTPSAllCount;
                MainOrdersDT.Rows[i]["TPSDispPercentage"] = TPSDispPercentage;
            }
        }

        private int GetMainOrderDispCount(int MainOrderID, int FactoryID)
        {
            int DispCount = 0;
            DataRow[] Rows = DispMainOrdersDT.Select("MainOrderID = " + MainOrderID + " AND FactoryID = " + FactoryID);

            if (Rows.Count() > 0)
                DispCount = Convert.ToInt32(Rows[0]["Count"]);

            return DispCount;
        }

        public BindingSource MainOrdersList
        {
            get { return MainOrdersBS; }
        }

        public BindingSource PackagesList
        {
            get { return PackagesBS; }
        }

        public BindingSource FrontsOrdersList
        {
            get { return FrontsOrdersBS; }
        }

        public BindingSource DecorOrdersList
        {
            get { return DecorOrdersBS; }
        }

        public void ClearPackages()
        {
            PackagesDT.Clear();
        }

        public void ClearFrontsOrders()
        {
            FrontsOrdersDT.Clear();
        }

        public void ClearDecorOrders()
        {
            DecorOrdersDT.Clear();
        }

        public void FilterMainOrders(int PermitID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT COUNT(PackageID) AS Count, MainOrderID, Packages.FactoryID FROM Packages WHERE PackageStatusID = 3" +
                " AND MainOrderID IN (SELECT MainOrderID FROM Packages WHERE DispatchID IN (SELECT DispatchID FROM infiniu2_light.dbo.PermitDetails WHERE PermitID = " + PermitID + "))" +
                " GROUP BY MainOrderID, Packages.FactoryID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispMainOrdersDT);
            }

            string SelectCommand = @"SELECT M.MainOrderID, C.ClientName, M.Weight, M.ProfilPackCount, M.TPSPackCount, M.Notes, M.FactoryID FROM MainOrders AS M
INNER JOIN MegaOrders ON M.MegaOrderID=MegaOrders.MegaOrderID
INNER JOIN infiniu2_marketingreference.dbo.Clients AS C ON MegaOrders.ClientID=C.ClientID
				WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE DispatchID IN (SELECT DispatchID FROM infiniu2_light.dbo.PermitDetails WHERE PermitID = " + PermitID + "))";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                MainOrdersDT.Clear();
                DA.Fill(MainOrdersDT);
            }
            FillMainPercentageColumn();
        }

        public void FilterPackages(int PermitID, int MainOrderID)
        {
            string SelectCommand = @"SELECT Packages.PackNumber, Packages.ProductType, Packages.MainOrderID, PackageStatus, FactoryName, Packages.PackingDateTime,
				Packages.StorageDateTime, Packages.ExpeditionDateTime, Packages.DispatchDateTime, Packages.PackageID, Packages.DispatchID, Packages.TrayID FROM Packages
				INNER JOIN infiniu2_catalog.dbo.Factory ON Packages.FactoryID = infiniu2_catalog.dbo.Factory.FactoryID
				INNER JOIN infiniu2_catalog.dbo.PackageStatuses ON Packages.PackageStatusID = infiniu2_catalog.dbo.PackageStatuses.PackageStatusID
				WHERE MainOrderID=" + MainOrderID + " AND DispatchID IN (SELECT DispatchID FROM infiniu2_light.dbo.PermitDetails WHERE PermitID = " + PermitID + ") ORDER BY PackNumber";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                PackagesDT.Clear();
                DA.Fill(PackagesDT);
            }
        }

        public bool FilterFrontsOrders(int PackageID, int MainOrderID)
        {
            DataTable OriginalFrontsOrdersDT = new DataTable();
            OriginalFrontsOrdersDT = FrontsOrdersDT.Clone();

            string SelectCommand = @"SELECT  FrontsOrdersID, MainOrderID, FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, Count, Square, Notes FROM FrontsOrders
				WHERE MainOrderID = " + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails WHERE PackageID = " + PackageID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalFrontsOrdersDT.Select("FrontsOrdersID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = FrontsOrdersDT.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            NewRow["Count"] = Row["Count"];
                            FrontsOrdersDT.Rows.Add(NewRow);
                        }
                    }
                    else
                    {
                        foreach (DataRow Row in OriginalFrontsOrdersDT.Rows)
                        {
                            DataRow NewRow = FrontsOrdersDT.NewRow();
                            NewRow.ItemArray = Row.ItemArray;
                            FrontsOrdersDT.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDT.Dispose();

            return FrontsOrdersDT.Rows.Count > 0;
        }

        public bool FilterDecorOrders(int PackageID, int MainOrderID)
        {
            DataTable OriginalDecorOrdersDT = new DataTable();
            OriginalDecorOrdersDT = DecorOrdersDT.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT DecorOrderID, ProductID, DecorID, ColorID, PatinaID, Height, Length, Width, Count, Notes FROM DecorOrders
				WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails WHERE PackageID = " + PackageID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalDecorOrdersDT.Select("DecorOrderID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = DecorOrdersDT.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            NewRow["Count"] = Row["Count"];
                            DecorOrdersDT.Rows.Add(NewRow);
                        }
                    }
                    else
                    {
                        foreach (DataRow Row in OriginalDecorOrdersDT.Rows)
                        {
                            DataRow NewRow = DecorOrdersDT.NewRow();
                            NewRow.ItemArray = Row.ItemArray;
                            DecorOrdersDT.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalDecorOrdersDT.Dispose();

            return DecorOrdersDT.Rows.Count > 0;
        }

        public bool FilterFrontsOrders(int MainOrderID)
        {
            DataTable OriginalFrontsOrdersDT = new DataTable();
            OriginalFrontsOrdersDT = FrontsOrdersDT.Clone();

            string SelectCommand = @"SELECT  FrontsOrdersID, MainOrderID, FrontID, ColorID, PatinaID, InsetTypeID, InsetColorID, TechnoProfileID, TechnoColorID, TechnoInsetTypeID, TechnoInsetColorID, Height, Width, Count, Square, Notes FROM FrontsOrders
				WHERE MainOrderID = " + MainOrderID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE MainOrderID=" + MainOrderID + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalFrontsOrdersDT.Select("FrontsOrdersID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = FrontsOrdersDT.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            NewRow["Count"] = Row["Count"];
                            FrontsOrdersDT.Rows.Add(NewRow);
                        }
                    }
                    else
                    {
                        foreach (DataRow Row in OriginalFrontsOrdersDT.Rows)
                        {
                            DataRow NewRow = FrontsOrdersDT.NewRow();
                            NewRow.ItemArray = Row.ItemArray;
                            FrontsOrdersDT.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDT.Dispose();

            return FrontsOrdersDT.Rows.Count > 0;
        }

        public bool FilterDecorOrders(int MainOrderID)
        {
            DataTable OriginalDecorOrdersDT = new DataTable();
            OriginalDecorOrdersDT = DecorOrdersDT.Clone();

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT DecorOrderID, ProductID, DecorID, ColorID, PatinaID, Height, Length, Width, Count, Notes FROM DecorOrders
				WHERE MainOrderID = " + MainOrderID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE MainOrderID=" + MainOrderID + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalDecorOrdersDT.Select("DecorOrderID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = DecorOrdersDT.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            NewRow["Count"] = Row["Count"];
                            DecorOrdersDT.Rows.Add(NewRow);
                        }
                    }
                    else
                    {
                        foreach (DataRow Row in OriginalDecorOrdersDT.Rows)
                        {
                            DataRow NewRow = DecorOrdersDT.NewRow();
                            NewRow.ItemArray = Row.ItemArray;
                            DecorOrdersDT.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalDecorOrdersDT.Dispose();

            return DecorOrdersDT.Rows.Count > 0;
        }

    }

    public class PackingReport : IAllFrontParameterName, IIsMarsel
    {
        int ClientID = 0;
        public bool ColorFullName = false;
        private DataTable ClientsDataTable = null;
        private DataTable FrontsResultDataTable = null;
        private DataTable DecorResultDataTable = null;

        private DataTable FrontsOrdersDataTable = null;
        private DataTable DecorOrdersDataTable = null;

        public DataTable FrontsDataTable = null;
        public DataTable PatinaDataTable = null;
        DataTable PatinaRALDataTable = null;
        public DataTable InsetTypesDataTable = null;
        public DataTable FrameColorsDataTable = null;
        public DataTable InsetColorsDataTable = null;
        private DataTable ProductsDataTable = null;
        private DataTable DecorDataTable = null;
        private DataTable DecorParametersDataTable = null;

        HSSFWorkbook hssfworkbook;
        HSSFFont MainFont;
        HSSFCellStyle MainStyle;
        HSSFFont HeaderFont;
        HSSFCellStyle HeaderStyle;
        HSSFFont ComplaintFont;
        HSSFCellStyle ComplaintCellStyle;
        HSSFCellStyle GreyComplaintCellStyle;
        HSSFFont PackNumberFont;
        HSSFCellStyle PackNumberStyle;
        HSSFFont SimpleFont;
        HSSFCellStyle SimpleCellStyle;
        HSSFCellStyle GreyCellStyle;
        HSSFFont TotalFont;
        HSSFCellStyle TotalStyle;
        HSSFCellStyle TempStyle;

        public PackingReport()
        {
            Create();
            CreateFrontsDataTable();
            CreateDecorDataTable();

            hssfworkbook = new HSSFWorkbook();

            #region

            MainFont = hssfworkbook.CreateFont();
            MainFont.FontHeightInPoints = 12;
            MainFont.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            MainFont.FontName = "Calibri";

            MainStyle = hssfworkbook.CreateCellStyle();
            MainStyle.SetFont(MainFont);

            HeaderFont = hssfworkbook.CreateFont();
            HeaderFont.Boldweight = 8 * 256;
            HeaderFont.FontName = "Calibri";

            HeaderStyle = hssfworkbook.CreateCellStyle();
            HeaderStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.RightBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderStyle.TopBorderColor = HSSFColor.BLACK.index;
            HeaderStyle.SetFont(HeaderFont);

            ComplaintFont = hssfworkbook.CreateFont();
            ComplaintFont.Boldweight = 8 * 256;
            ComplaintFont.FontName = "Calibri";
            ComplaintFont.IsItalic = true;

            ComplaintCellStyle = hssfworkbook.CreateCellStyle();
            ComplaintCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            ComplaintCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            ComplaintCellStyle.SetFont(ComplaintFont);

            GreyComplaintCellStyle = hssfworkbook.CreateCellStyle();
            GreyComplaintCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            GreyComplaintCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            GreyComplaintCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            GreyComplaintCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            GreyComplaintCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            GreyComplaintCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            GreyComplaintCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            GreyComplaintCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            GreyComplaintCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_25_PERCENT.index;
            GreyComplaintCellStyle.FillPattern = HSSFCellStyle.SOLID_FOREGROUND;
            GreyComplaintCellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            GreyComplaintCellStyle.SetFont(ComplaintFont);

            PackNumberFont = hssfworkbook.CreateFont();
            PackNumberFont.Boldweight = 8 * 256;
            PackNumberFont.FontName = "Calibri";

            PackNumberStyle = hssfworkbook.CreateCellStyle();
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

            SimpleFont = hssfworkbook.CreateFont();
            SimpleFont.FontHeightInPoints = 8;
            SimpleFont.FontName = "Calibri";

            SimpleCellStyle = hssfworkbook.CreateCellStyle();
            SimpleCellStyle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCellStyle.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCellStyle.SetFont(SimpleFont);

            GreyCellStyle = hssfworkbook.CreateCellStyle();
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

            TotalFont = hssfworkbook.CreateFont();
            TotalFont.FontHeightInPoints = 12;
            TotalFont.FontName = "Calibri";

            TotalStyle = hssfworkbook.CreateCellStyle();
            TotalStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            TotalStyle.TopBorderColor = HSSFColor.BLACK.index;
            TotalStyle.SetFont(TotalFont);

            TempStyle = hssfworkbook.CreateCellStyle();
            TempStyle.SetFont(TotalFont);

            #endregion

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
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColors WHERE GroupID IN (2,3,4,5,6,21,22)", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow[] rows = FrameColorsDataTable.Select("ColorID=" + Convert.ToInt32(DT.Rows[i]["InsetColorID"]));
                        if (rows.Count() == 0)
                        {
                            DataRow NewRow = FrameColorsDataTable.NewRow();
                            NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                            NewRow["ColorName"] = DT.Rows[i]["InsetColorName"].ToString();
                            FrameColorsDataTable.Rows.Add(NewRow);
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

        private void Create()
        {
            FrontsDataTable = new DataTable();
            FrameColorsDataTable = new DataTable();
            PatinaDataTable = new DataTable();
            InsetTypesDataTable = new DataTable();
            InsetColorsDataTable = new DataTable();
            FrontsOrdersDataTable = new DataTable();
            DecorOrdersDataTable = new DataTable();

            FrontsResultDataTable = new DataTable();
            DecorResultDataTable = new DataTable();

            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients", ConnectionStrings.MarketingReferenceConnectionString))
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
            SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE ProductID IN (SELECT ProductID FROM DecorConfig WHERE Enabled = 1) ORDER BY ProductName ASC";
            ProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(ProductsDataTable);
            }
            DecorDataTable = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
				INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1 ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorDataTable);
            }

            DecorParametersDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorParameters", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorParametersDataTable);
            }
        }

        private void CreateFrontsDataTable()
        {
            FrontsResultDataTable = new DataTable();

            FrontsResultDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Front"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("FrameColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetType"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("InsetColor"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("TechnoInset"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Height"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Width"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("AccountingName"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("ConfirmDateTime"), System.Type.GetType("System.String")));
            FrontsResultDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));
        }

        private void CreateDecorDataTable()
        {
            DecorResultDataTable = new DataTable();

            DecorResultDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Product"), Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Color"), Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Height"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Width"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Count"), Type.GetType("System.Int32")));
            DecorResultDataTable.Columns.Add(new DataColumn(("Notes"), System.Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("AccountingName"), System.Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("ConfirmDateTime"), System.Type.GetType("System.String")));
            DecorResultDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));
        }

        public bool IsMegaComplaint(int MegaOrderID)
        {
            bool IsComplaint = false;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT IsComplaint FROM MegaOrders WHERE MegaOrderID=" +
                    MegaOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (DA.Fill(DT) == 0)
                        return IsComplaint;

                    IsComplaint = Convert.ToBoolean(DT.Rows[0]["IsComplaint"]);
                }
            }
            return IsComplaint;
        }

        public string GetMainOrderNotes(int MainOrderID)
        {
            string Notes = null;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Notes FROM MainOrders WHERE MainOrderID=" +
                    MainOrderID, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    if (DA.Fill(DT) == 0)
                        return Notes;

                    Notes = DT.Rows[0]["Notes"].ToString();
                }
            }
            return Notes;
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

        public string GetProductName(int ProductID)
        {
            DataRow[] Rows = ProductsDataTable.Select("ProductID = " + ProductID);
            return Rows[0]["ProductName"].ToString();
        }

        public string GetDecorName(int DecorID)
        {
            DataRow[] Rows = DecorDataTable.Select("DecorID = " + DecorID);
            //if (ClientID == 101)
            //    return Rows[0]["OldName"].ToString();
            //else
            return Rows[0]["Name"].ToString();
        }

        public bool HasParameter(int ProductID, string Parameter)
        {
            DataRow[] Rows = DecorParametersDataTable.Select("ProductID = " + ProductID);

            return Convert.ToBoolean(Rows[0][Parameter]);
        }

        private decimal GetSquare(DataTable DT)
        {
            decimal Square = 0;

            foreach (DataRow Row in DT.Rows)
            {
                if (Row["Width"].ToString() != "-1")
                    Square += Convert.ToDecimal(Row["Height"]) * Convert.ToDecimal(Row["Width"]) * Convert.ToDecimal(Row["Count"]) / 1000000;
            }

            return Square;
        }

        private int GetCount(DataTable DT)
        {
            int Count = 0;

            foreach (DataRow Row in DT.Rows)
            {
                Count += Convert.ToInt32(Row["Count"]);
            }

            return Count;
        }

        public DataTable PackageFrontsSequence
        {
            get
            {
                DataTable DT = new DataTable();

                using (DataView DV = new DataView(FrontsOrdersDataTable))
                {
                    DV.RowFilter = "PackNumber is not null";
                    DV.Sort = "PackNumber ASC";

                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }

                return DT;
            }
        }

        public DataTable PackageDecorSequence
        {
            get
            {
                DataTable DT = new DataTable();

                using (DataView DV = new DataView(DecorOrdersDataTable))
                {
                    DV.RowFilter = "PackNumber is not null";
                    DV.Sort = "PackNumber ASC";

                    DT = DV.ToTable(true, new string[] { "PackNumber" });
                }

                return DT;
            }
        }

        private bool FillFronts()
        {
            string Front = "";
            string FrameColor = "";
            string InsetColor = "";
            string TechnoInset = "";
            FrontsResultDataTable.Clear();

            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                DataRow NewRow = FrontsResultDataTable.NewRow();

                Front = GetFrontName(Convert.ToInt32(Row["FrontID"]));
                FrameColor = GetColorName(Convert.ToInt32(Row["ColorID"]));
                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    FrameColor = FrameColor + " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (Convert.ToInt32(Row["TechnoColorID"]) != -1 && Convert.ToInt32(Row["TechnoColorID"]) != Convert.ToInt32(Row["ColorID"]))
                    FrameColor = FrameColor + "/" + GetColorName(Convert.ToInt32(Row["TechnoColorID"]));
                if (Convert.ToInt32(Row["TechnoInsetColorID"]) != -1)
                    TechnoInset = TechnoInset + " " + GetInsetColorName(Convert.ToInt32(Row["TechnoInsetColorID"]));
                TechnoInset = GetInsetTypeName(Convert.ToInt32(Row["TechnoInsetTypeID"]));

                var InsetType = GetInsetTypeName(Convert.ToInt32(Row["InsetTypeID"]));
                var bMarsel3 = IsMarsel3(Convert.ToInt32(Row["FrontID"]));
                var bMarsel4 = IsMarsel4(Convert.ToInt32(Row["FrontID"]));
                if (bMarsel3 || bMarsel4)
                {
                    var bImpost = IsImpost(Convert.ToInt32(Row["TechnoProfileID"]));
                    if (bImpost)
                    {
                        string Front2 = GetFront2Name(Convert.ToInt32(Row["TechnoProfileID"]));
                        InsetType = InsetType + "/" + Front2;
                    }
                }
                InsetColor = GetInsetColorName(Convert.ToInt32(Row["InsetColorID"]));

                NewRow["InsetType"] = InsetType;
                NewRow["FrameColor"] = FrameColor;
                NewRow["InsetColor"] = InsetColor;
                NewRow["TechnoInset"] = TechnoInset;
                NewRow["Front"] = Front;
                NewRow["Height"] = Row["Height"];
                NewRow["Width"] = Row["Width"];
                NewRow["Count"] = Row["Count"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["PackageStatusID"] = Row["PackageStatusID"];
                NewRow["AccountingName"] = Row["AccountingName"];
                NewRow["ConfirmDateTime"] = Row["ConfirmDateTime"];
                FrontsResultDataTable.Rows.Add(NewRow);
            }
            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        private bool FillDecor()
        {
            DecorResultDataTable.Clear();

            foreach (DataRow Row in DecorOrdersDataTable.Rows)
            {
                DataRow NewRow = DecorResultDataTable.NewRow();

                NewRow["Product"] = GetProductName(Convert.ToInt32(Row["ProductID"])) + " " + GetDecorName(Convert.ToInt32(Row["DecorID"]));

                //if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Height"))
                //{
                //    if (Convert.ToInt32(Row["Height"]) != -1)
                //        NewRow["Height"] = Row["Height"];
                //    if (Convert.ToInt32(Row["Length"]) != -1)
                //        NewRow["Height"] = Row["Length"];
                //}
                //if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Length"))
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
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "Width"))
                    NewRow["Width"] = Convert.ToInt32(Row["Width"]);

                string Color = GetColorName(Convert.ToInt32(Row["ColorID"]));
                if (Convert.ToInt32(Row["PatinaID"]) != -1)
                    Color += " " + GetPatinaName(Convert.ToInt32(Row["PatinaID"]));
                if (HasParameter(Convert.ToInt32(Row["ProductID"]), "ColorID"))
                    NewRow["Color"] = Color;
                NewRow["Count"] = Row["Count"];
                NewRow["Notes"] = Row["Notes"];
                NewRow["PackNumber"] = Row["PackNumber"];
                NewRow["PackageStatusID"] = Row["PackageStatusID"];
                NewRow["AccountingName"] = Row["AccountingName"];
                NewRow["ConfirmDateTime"] = Row["ConfirmDateTime"];

                DecorResultDataTable.Rows.Add(NewRow);
            }

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        private DataTable FillPackages(int PermitID)
        {
            DataTable PackagesDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(
                @"SELECT MainOrders.MegaOrderID, MegaOrders.OrderNumber, Packages.ProductType, Packages.FactoryID, Packages.MainOrderID, Packages.PackageID, Packages.PackageStatusID
                FROM Packages INNER JOIN MainOrders ON Packages.MainOrderID = MainOrders.MainOrderID
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
                WHERE Packages.DispatchID IN (SELECT DispatchID FROM infiniu2_light.dbo.PermitDetails WHERE PermitID=" + PermitID + ")", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(PackagesDT);
            }

            DataTable DT = new DataTable();

            using (DataView DV = new DataView(PackagesDT, string.Empty, "MainOrderID", DataViewRowState.CurrentRows))
            {
                DT = DV.ToTable(true, new string[] { "MegaOrderID", "OrderNumber", "MainOrderID" });
            }

            return DT;
        }

        private bool FilterFrontsOrders(int PermitID, int MainOrderID, int FactoryID)
        {
            string FactoryFilter1 = string.Empty;
            string FactoryFilter2 = string.Empty;

            if (FactoryID != 0)
            {
                FactoryFilter1 = " AND FrontsOrders.FactoryID = " + FactoryID;
                FactoryFilter2 = " AND FactoryID = " + FactoryID;
            }

            FrontsOrdersDataTable.Clear();

            DataTable OriginalFrontsOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT FrontsOrders.*, infiniu2_catalog.dbo.FrontsConfig.AccountingName, MegaOrders.ConfirmDateTime FROM FrontsOrders 
				INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID
				INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
				INNER JOIN infiniu2_catalog.dbo.FrontsConfig ON FrontsOrders.FrontConfigID=infiniu2_catalog.dbo.FrontsConfig.FrontConfigID WHERE FrontsOrders.MainOrderID = " + MainOrderID + FactoryFilter1,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalFrontsOrdersDataTable);
            }

            FrontsOrdersDataTable = OriginalFrontsOrdersDataTable.Clone();
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            FrontsOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE Packages.DispatchID IN (SELECT DispatchID FROM infiniu2_light.dbo.PermitDetails WHERE PermitID=" + PermitID + ") AND MainOrderID = " + MainOrderID + " AND ProductType = 0" + FactoryFilter2 + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalFrontsOrdersDataTable.Select("FrontsOrdersID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = FrontsOrdersDataTable.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;
                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            NewRow["PackageStatusID"] = Row["PackageStatusID"];
                            FrontsOrdersDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalFrontsOrdersDataTable.Dispose();

            return FrontsOrdersDataTable.Rows.Count > 0;
        }

        private bool FilterDecorOrders(int PermitID, int MainOrderID, int FactoryID)
        {
            string FactoryFilter1 = string.Empty;
            string FactoryFilter2 = string.Empty;

            if (FactoryID != 0)
            {
                FactoryFilter1 = " AND DecorOrders.FactoryID = " + FactoryID;
                FactoryFilter2 = " AND FactoryID = " + FactoryID;
            }

            DecorOrdersDataTable.Clear();

            DataTable OriginalDecorOrdersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT DecorOrders.*, infiniu2_catalog.dbo.DecorConfig.AccountingName, MegaOrders.ConfirmDateTime FROM DecorOrders
				INNER JOIN MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID
				INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID
				INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID WHERE DecorOrders.MainOrderID = " + MainOrderID + FactoryFilter1,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(OriginalDecorOrdersDataTable);
            }
            DecorOrdersDataTable = OriginalDecorOrdersDataTable.Clone();
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackNumber"), System.Type.GetType("System.Int32")));
            DecorOrdersDataTable.Columns.Add(new DataColumn(("PackageStatusID"), System.Type.GetType("System.Int32")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT PackageDetails.*, PackageStatusID FROM PackageDetails" +
                " INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID" +
                " WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages" +
                " WHERE Packages.DispatchID IN (SELECT DispatchID FROM infiniu2_light.dbo.PermitDetails WHERE PermitID=" + PermitID + ") AND MainOrderID = " + MainOrderID +
                " AND ProductType = 1" + FactoryFilter2 + ")",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        foreach (DataRow Row in DT.Rows)
                        {
                            DataRow[] ORow = OriginalDecorOrdersDataTable.Select("DecorOrderID = " + Row["OrderID"]);

                            if (ORow.Count() == 0)
                                continue;

                            DataRow NewRow = DecorOrdersDataTable.NewRow();
                            NewRow.ItemArray = ORow[0].ItemArray;

                            //if (Convert.ToInt32(ORow[0]["ColorID"]) == -1)
                            //    NewRow["ColorID"] = 0;
                            //else
                            NewRow["ColorID"] = ORow[0]["ColorID"];

                            NewRow["Count"] = Row["Count"];
                            NewRow["PackNumber"] = Row["PackNumber"];
                            NewRow["PackageStatusID"] = Row["PackageStatusID"];
                            DecorOrdersDataTable.Rows.Add(NewRow);
                        }
                    }
                }
            }
            OriginalDecorOrdersDataTable.Dispose();

            return DecorOrdersDataTable.Rows.Count > 0;
        }

        private bool HasFronts(int[] MainOrderIDs, int FactoryID)
        {
            string ProductType = "ProductType = 0";

            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
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

        private bool HasDecor(int[] MainOrderIDs, int FactoryID)
        {
            string ProductType = "ProductType = 1";

            string FactoryFilter = string.Empty;

            if (FactoryID != 0)
                FactoryFilter = " AND FactoryID = " + FactoryID;

            string SelectionCommand = "SELECT PackageID FROM Packages" +
                " WHERE MainOrderID IN (" + string.Join(",", MainOrderIDs) +
                ") AND " + ProductType + FactoryFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(SelectionCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
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

        private string GetCellValue(ref HSSFSheet sheet1, int row, int col)
        {
            String value = "";

            try
            {
                HSSFCell cell = sheet1.GetRow(row - 1).GetCell(col - 1);

                if (cell.CellType != HSSFCell.CELL_TYPE_BLANK)
                {
                    switch (cell.CellType)
                    {
                        case HSSFCell.CELL_TYPE_NUMERIC:
                            // ********* Date comes here ************

                            // Numeric type
                            value = cell.NumericCellValue.ToString();

                            break;

                        case HSSFCell.CELL_TYPE_BOOLEAN:
                            // Boolean type
                            value = cell.BooleanCellValue.ToString();
                            break;

                        default:
                            // String type
                            value = cell.StringCellValue;
                            break;
                    }
                }

            }
            catch (Exception)
            {
                value = "";
            }

            return value.Trim();
        }

        public void CreateReport(int PermitID, string DispatchDate, string ClientName, int iClientID, int FactoryID)
        {
            DataTable OrderDT = new DataTable();

            string SheetName = "Ведомость Профиль+ТПС";
            ClientID = iClientID;
            if (FactoryID == 1)
                SheetName = "Ведомость Профиль";
            if (FactoryID == 2)
                SheetName = "Ведомость ТПС";

            HSSFSheet sheet1 = hssfworkbook.CreateSheet(SheetName);
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int RowIndex = 3;

            HSSFCell Cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(2), 0, "Серым цветом отмечены отсутствующие упаковки");

            DataTable DT = new DataTable();

            using (DataView DV = new DataView(OrderDT))
            {
                DT = DV.ToTable(true, new string[] { "MainOrderID" });
            }

            int[] MainOrderIDs = new int[DT.Rows.Count];

            for (int i = 0; i < DT.Rows.Count; i++)
                MainOrderIDs[i] = Convert.ToInt32(DT.Rows[i]["MainOrderID"]);
            DT.Dispose();

            if (HasFronts(MainOrderIDs, FactoryID))
            {
                CreateFrontsExcel(ref sheet1, OrderDT, PermitID, DispatchDate, ClientName, ref RowIndex, FactoryID);
            }

            if (HasDecor(MainOrderIDs, FactoryID))
            {
                CreateDecorExcel(ref sheet1, OrderDT, PermitID, DispatchDate, ClientName, RowIndex, FactoryID);
            }
        }

        private void CreateFrontsExcel(ref HSSFSheet sheet1, DataTable OrdersDT, int PermitID,
            string DispatchDate, string ClientName, ref int RowIndex, int FactoryID)
        {
            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsFronts = false;

            decimal FrontsSquare = 0;
            decimal TotalFrontsSquare = 0;
            int FrontsCount = 0;

            int DisplayIndex = 0;
            sheet1.SetColumnWidth(DisplayIndex++, 4 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 18 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 12 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 12 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 12 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 12 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 5 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 5 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 5 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 7 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 9 * 256);

            HSSFCell ConfirmCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 6, "Утверждаю...............");
            ConfirmCell.CellStyle = TempStyle;
            HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 0, "Дата и время создания документа: " + Security.GetCurrentDate().ToString("dd.MM.yyyy HH:mm"));
            CreateDateCell.CellStyle = TempStyle;

            for (int i = 0; i < OrdersDT.Rows.Count; i++)
            {
                FilterFrontsOrders(PermitID, Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]), FactoryID);

                IsFronts = FillFronts();

                if (FrontsResultDataTable.Rows.Count < 1)
                    continue;
                PackCount += PackageFrontsSequence.Rows.Count;

                FrontsSquare = GetSquare(FrontsResultDataTable);
                TotalFrontsSquare += FrontsSquare;
                FrontsCount += GetCount(FrontsResultDataTable);

                int BottomRow = FrontsResultDataTable.Rows.Count + RowIndex + 4;

                bool IsComplaint = IsMegaComplaint(Convert.ToInt32(OrdersDT.Rows[i]["MegaOrderID"]));
                MainOrderNote = GetMainOrderNotes(Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Клиент: " +
                    ClientName + " № " + Convert.ToInt32(OrdersDT.Rows[i]["OrderNumber"]) + " - " + Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }

                DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Фасад");
                cell2.CellStyle = HeaderStyle;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет профиля");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Тип наполнителя");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет наполнителя");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Техно вставка");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Выс.");
                cell8.CellStyle = HeaderStyle;
                HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Шир.");
                cell9.CellStyle = HeaderStyle;
                HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell10.CellStyle = HeaderStyle;
                HSSFCell cell12 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell12.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Бухг. наим.");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Согласовано");
                cell1.CellStyle = HeaderStyle;

                //вывод заказов фасадов
                for (int index = 0; index < PackageFrontsSequence.Rows.Count; index++)
                {
                    DataRow[] FRows = FrontsResultDataTable.Select("[PackNumber] = " + PackageFrontsSequence.Rows[index]["PackNumber"]);

                    if (FRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = FRows.Count() + TopIndex - 1;

                    for (int x = 0; x < FRows.Count(); x++)
                    {
                        if (FrontsResultDataTable.Rows.Count == 0)
                            break;

                        for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
                        {
                            if (Convert.ToInt32(FRows[x]["PackageStatusID"]) != 3)
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FRows[x][y]));

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
                                    if (IsComplaint)
                                        cell.CellStyle = GreyComplaintCellStyle;
                                    else
                                        cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    if (IsComplaint)
                                        cell.CellStyle = GreyComplaintCellStyle;
                                    else
                                        cell.CellStyle = GreyCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    if (IsComplaint)
                                        cell.CellStyle = GreyComplaintCellStyle;
                                    else
                                        cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }

                            else
                            {
                                Type t = FrontsResultDataTable.Rows[x][y].GetType();

                                if (FrontsResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToDouble(FRows[x][y]));

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
                                    if (IsComplaint)
                                        cell.CellStyle = ComplaintCellStyle;
                                    else
                                        cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(Convert.ToInt32(FRows[x][y]));
                                    if (IsComplaint)
                                        cell.CellStyle = ComplaintCellStyle;
                                    else
                                        cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(y);
                                    cell.SetCellValue(FRows[x][y].ToString());
                                    if (IsComplaint)
                                        cell.CellStyle = ComplaintCellStyle;
                                    else
                                        cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }


                        }
                        RowIndex++;
                    }

                }

                if (FrontsSquare > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(++RowIndex), 8, Decimal.Round(FrontsSquare, 3, MidpointRounding.AwayFromZero) + " м.кв.");
                    HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                    cellStyle.SetFont(PackNumberFont);
                    cell.CellStyle = cellStyle;
                }

                if (IsFronts)
                    RowIndex++;


                RowIndex++;
            }

            for (int y = 0; y < FrontsResultDataTable.Columns.Count - 1; y++)
            {
                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            TotalFrontsSquare = Decimal.Round(TotalFrontsSquare, 2, MidpointRounding.AwayFromZero);

            HSSFCell cell13 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell13.CellStyle = TotalStyle;
            HSSFCell cell14 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell14.CellStyle = TotalStyle;
            HSSFCell cell15 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 4, "Фасадов: " + FrontsCount);
            cell15.CellStyle = TotalStyle;
            HSSFCell cell16 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 6, "Квадратура: " + TotalFrontsSquare + " м.кв.");
            cell16.CellStyle = TotalStyle;
        }

        private void CreateDecorExcel(ref HSSFSheet sheet1, DataTable OrdersDT, int PermitID,
            string DispatchDate, string ClientName, int RowIndex, int FactoryID)
        {
            if (RowIndex < 1)
            {
                HSSFCell CreateDateCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(0), 0, "Дата и время создания документа: " + Security.GetCurrentDate().ToString("dd.MM.yyyy HH:mm"));
                CreateDateCell.CellStyle = TempStyle;
            }

            RowIndex++;
            RowIndex++;
            RowIndex++;

            int PackCount = 0;

            string MainOrderNote = string.Empty;

            bool IsDecor = false;

            for (int i = 0; i < OrdersDT.Rows.Count; i++)
            {
                FilterDecorOrders(PermitID, Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]), FactoryID);

                IsDecor = FillDecor();

                if (DecorResultDataTable.Rows.Count == 0)
                    continue;

                bool IsComplaint = IsMegaComplaint(Convert.ToInt32(OrdersDT.Rows[i]["MegaOrderID"]));
                MainOrderNote = GetMainOrderNotes(Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));

                PackCount += PackageDecorSequence.Rows.Count;

                int BottomRow = DecorResultDataTable.Rows.Count + RowIndex + 4;

                HSSFCell ClientCell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0,
                    "Клиент: " + ClientName + " № " + Convert.ToInt32(OrdersDT.Rows[i]["OrderNumber"]) + " - " + Convert.ToInt32(OrdersDT.Rows[i]["MainOrderID"]));
                ClientCell.CellStyle = MainStyle;

                if (DispatchDate.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Дата отгрузки: " + DispatchDate);
                    cell.CellStyle = MainStyle;
                }

                if (MainOrderNote.Length > 0)
                {
                    HSSFCell cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex++), 0, "Примечание: " + MainOrderNote);
                    cell.CellStyle = MainStyle;
                }
                int DisplayIndex = 0;
                HSSFCell cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "№");
                cell1.CellStyle = HeaderStyle;
                HSSFCell cell2 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Название");
                cell2.CellStyle = HeaderStyle;
                HSSFCell cell4 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Цвет");
                cell4.CellStyle = HeaderStyle;
                HSSFCell cell5 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Длина\\Высота");
                cell5.CellStyle = HeaderStyle;
                HSSFCell cell6 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Ширина");
                cell6.CellStyle = HeaderStyle;
                HSSFCell cell7 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Кол.");
                cell7.CellStyle = HeaderStyle;
                HSSFCell cell8 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Прим.");
                cell8.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Бухг. наим.");
                cell1.CellStyle = HeaderStyle;
                cell1 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex++, "Согласовано");
                cell1.CellStyle = HeaderStyle;

                for (int index = 0; index < PackageDecorSequence.Rows.Count; index++)
                {
                    DataRow[] DRows = DecorResultDataTable.Select("[PackNumber] = " + PackageDecorSequence.Rows[index]["PackNumber"]);
                    if (DRows.Count() == 0)
                        continue;

                    int TopIndex = RowIndex + 1;
                    int BottomIndex = DRows.Count() + TopIndex - 1;

                    for (int x = 0; x < DRows.Count(); x++)
                    {
                        for (int y = 0; y < DecorResultDataTable.Columns.Count - 1; y++)
                        {
                            int ColumnIndex = y;

                            //if (y == 0 || y == 1)
                            //{
                            //    ColumnIndex = y;
                            //}
                            //else
                            //{
                            //    ColumnIndex = y + 1;
                            //}

                            Type t = DecorResultDataTable.Rows[x][y].GetType();

                            if (Convert.ToInt32(DRows[x]["PackageStatusID"]) != 3)
                            {
                                //sheet1.CreateRow(RowIndex + 1).CreateCell(2).CellStyle = GreyCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToDouble(DRows[x][y]));

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
                                    if (IsComplaint)
                                        cell.CellStyle = GreyComplaintCellStyle;
                                    else
                                        cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    if (IsComplaint)
                                        cell.CellStyle = GreyComplaintCellStyle;
                                    else
                                        cell.CellStyle = GreyCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    if (IsComplaint)
                                        cell.CellStyle = GreyComplaintCellStyle;
                                    else
                                        cell.CellStyle = GreyCellStyle;
                                    continue;
                                }
                            }
                            else
                            {
                                //sheet1.CreateRow(RowIndex + 1).CreateCell(2).CellStyle = SimpleCellStyle;

                                if (DecorResultDataTable.Columns[y].ColumnName == "PackNumber")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    cell.CellStyle = PackNumberStyle;
                                    sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(TopIndex, 0, BottomIndex, 0));
                                    continue;
                                }

                                if (t.Name == "Decimal")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToDouble(DRows[x][y]));

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
                                    if (IsComplaint)
                                        cell.CellStyle = ComplaintCellStyle;
                                    else
                                        cell.CellStyle = SimpleCellStyle;

                                    continue;
                                }
                                if (t.Name == "Int32")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);

                                    if (DRows[x][y] == DBNull.Value)
                                        cell.SetCellValue(DRows[x][y].ToString());
                                    else
                                        cell.SetCellValue(Convert.ToInt32(DRows[x][y]));

                                    if (IsComplaint)
                                        cell.CellStyle = ComplaintCellStyle;
                                    else
                                        cell.CellStyle = SimpleCellStyle;

                                    continue;
                                }

                                if (t.Name == "String" || t.Name == "DBNull")
                                {
                                    HSSFCell cell = sheet1.CreateRow(RowIndex + 1).CreateCell(ColumnIndex);
                                    cell.SetCellValue(DRows[x][y].ToString());
                                    if (IsComplaint)
                                        cell.CellStyle = ComplaintCellStyle;
                                    else
                                        cell.CellStyle = SimpleCellStyle;
                                    continue;
                                }
                            }





                        }
                        RowIndex++;
                    }
                }
                RowIndex++;

                RowIndex++;
            }

            for (int y = 0; y < DecorResultDataTable.Columns.Count - 1; y++)
            {
                HSSFCell cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                HSSFCellStyle cellStyle = hssfworkbook.CreateCellStyle();
                cellStyle.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
                cellStyle.TopBorderColor = HSSFColor.BLACK.index;
                cell.CellStyle = cellStyle;
            }

            HSSFCell cell9 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "Итого: ");
            cell9.CellStyle = TotalStyle;
            HSSFCell cell10 = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Упаковок: " + PackCount);
            cell10.CellStyle = TotalStyle;
        }
    }
}
