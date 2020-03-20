using ComponentFactory.Krypton.Toolkit;

using Infinium.Modules.Marketing.NewOrders;

using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Infinium.Modules.Marketing.MutualSettlements
{
    public enum DocumentTypes
    {
        InvoiceExcel = 1,
        InvoiceDbf = 2,
        DispatchExcel = 3,
        DispatchDbf = 4
    }

    public struct CashReportParameters
    {
        public bool Cancel;
        public bool AllClients;
        public ArrayList Clients;
        public DateTime Date1;
        public DateTime Date2;
    }

    public class MutualSettlements
    {
        public FileManager FM = null;

        public bool NeedCreateIncome = false;

        public DataTable SubscribesDT = null;
        private DataTable FilterClientsDT = null;
        private DataTable ClientsDT = null;
        private DataTable ClientsDispatchesDT = null;
        private DataTable ClientsIncomesDT = null;
        private DataTable CurrencyTypesDT = null;
        private DataTable DiscountPaymentConditionsDT = null;
        private DataTable DocumentsToDeleteDT = null;
        private DataTable MutualSettlementsDT = null;
        private DataTable MutualSettlementOrdersDT = null;
        private DataTable CashReportDT = null;
        private DataTable ICReportDT = null;
        private DataTable VATReportDT = null;
        private DataTable NewMutualSettlementsDT = null;
        private DataTable NewClientsDispatchesDT = null;
        private DataTable AllClientsDispatchesDT = null;
        private DataTable AllClientsIncomesDT = null;
        private DataTable dtRolePermissions = null;

        public BindingSource FilterClientsBS = null;
        public BindingSource ClientsBS = null;
        public BindingSource ClientsDispatchesBS = null;
        public BindingSource ClientsIncomesBS = null;
        public BindingSource MutualSettlementsBS = null;
        public BindingSource NewMutualSettlementsBS = null;
        public BindingSource NewClientsDispatchesBS = null;

        private SqlDataAdapter SelectClientsDispatchesDA = null;
        private SqlDataAdapter UpdateClientsDispatchesDA = null;
        private SqlCommandBuilder UpdateClientsDispatchesCB = null;

        private SqlDataAdapter SelectClientsIncomesDA = null;
        private SqlDataAdapter UpdateClientsIncomesDA = null;
        private SqlCommandBuilder UpdateClientsIncomesCB = null;

        private SqlDataAdapter SelectMutualSettlementsDA = null;
        private SqlDataAdapter UpdateMutualSettlementsDA = null;
        private SqlCommandBuilder UpdateMutualSettlementsCB = null;

        private SqlDataAdapter SelectMutualSettlementOrdersDA = null;
        private SqlDataAdapter UpdateMutualSettlementOrdersDA = null;
        private SqlCommandBuilder UpdateMutualSettlementOrdersCB = null;

        public MutualSettlements()
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
            FM = new FileManager();

            dtRolePermissions = new DataTable();
            SubscribesDT = new DataTable();
            SubscribesDT.Columns.Add(new DataColumn("MutualSettlementID", Type.GetType("System.Int32")));
            SubscribesDT.Columns.Add(new DataColumn("ClientID", Type.GetType("System.Int32")));
            SubscribesDT.Columns.Add(new DataColumn("FactoryID", Type.GetType("System.Int32")));
            FilterClientsDT = new DataTable();
            ClientsDT = new DataTable();
            ClientsDispatchesDT = new DataTable();
            ClientsIncomesDT = new DataTable();
            CurrencyTypesDT = new DataTable();
            DiscountPaymentConditionsDT = new DataTable();
            DocumentsToDeleteDT = new DataTable();
            DocumentsToDeleteDT.Columns.Add(new DataColumn("DocumentType", Type.GetType("System.Int32")));
            DocumentsToDeleteDT.Columns.Add(new DataColumn("DocumentID", Type.GetType("System.Int32")));
            MutualSettlementsDT = new DataTable();
            MutualSettlementOrdersDT = new DataTable();
            NewMutualSettlementsDT = new DataTable();
            NewMutualSettlementsDT.Columns.Add(new DataColumn("MutualSettlementID", Type.GetType("System.Int32")));
            NewMutualSettlementsDT.Columns.Add(new DataColumn("ClientID", Type.GetType("System.Int32")));
            NewMutualSettlementsDT.Columns.Add(new DataColumn("ClientName", Type.GetType("System.String")));
            NewClientsDispatchesDT = new DataTable();
            NewClientsDispatchesDT.Columns.Add(new DataColumn("MutualSettlementID", Type.GetType("System.Int32")));
            NewClientsDispatchesDT.Columns.Add(new DataColumn("ClientID", Type.GetType("System.Int32")));
            NewClientsDispatchesDT.Columns.Add(new DataColumn("ClientName", Type.GetType("System.String")));
            AllClientsDispatchesDT = new DataTable();
            AllClientsIncomesDT = new DataTable();
            FilterClientsBS = new BindingSource();
            ClientsBS = new BindingSource();
            ClientsDispatchesBS = new BindingSource();
            ClientsIncomesBS = new BindingSource();
            MutualSettlementsBS = new BindingSource();
            NewMutualSettlementsBS = new BindingSource();
            NewClientsDispatchesBS = new BindingSource();
        }

        private void Fill()
        {
            CashReportDT = new DataTable();
            CashReportDT.Columns.Add(new DataColumn("ClientName", Type.GetType("System.String")));
            CashReportDT.Columns.Add(new DataColumn("CurrencyType", Type.GetType("System.String")));
            CashReportDT.Columns.Add(new DataColumn("OpeningBalance", Type.GetType("System.Decimal")));
            CashReportDT.Columns.Add(new DataColumn("TotalDispatchSum", Type.GetType("System.Decimal")));
            CashReportDT.Columns.Add(new DataColumn("TotalIncomeSum", Type.GetType("System.Decimal")));
            CashReportDT.Columns.Add(new DataColumn("ClosingBalance", Type.GetType("System.Decimal")));

            ICReportDT = new DataTable();
            ICReportDT.Columns.Add(new DataColumn("ClientName", Type.GetType("System.String")));
            ICReportDT.Columns.Add(new DataColumn("DispatchDateTime", Type.GetType("System.String")));
            ICReportDT.Columns.Add(new DataColumn("WayBill", Type.GetType("System.String")));
            ICReportDT.Columns.Add(new DataColumn("DispatchSum", Type.GetType("System.Decimal")));
            ICReportDT.Columns.Add(new DataColumn("DebtSum", Type.GetType("System.Decimal")));
            ICReportDT.Columns.Add(new DataColumn("CurrencyType", Type.GetType("System.String")));
            ICReportDT.Columns.Add(new DataColumn("Deadline", Type.GetType("System.String")));
            ICReportDT.Columns.Add(new DataColumn("Overdue", Type.GetType("System.Boolean")));
            ICReportDT.Columns.Add(new DataColumn("TotalDays", Type.GetType("System.Int32")));

            VATReportDT = new DataTable();
            VATReportDT.Columns.Add(new DataColumn("ClientName", Type.GetType("System.String")));
            VATReportDT.Columns.Add(new DataColumn("DispatchDateTime", Type.GetType("System.String")));
            VATReportDT.Columns.Add(new DataColumn("WayBill", Type.GetType("System.String")));
            VATReportDT.Columns.Add(new DataColumn("DispatchSum", Type.GetType("System.Decimal")));
            VATReportDT.Columns.Add(new DataColumn("CurrencyType", Type.GetType("System.String")));
            VATReportDT.Columns.Add(new DataColumn("Deadline", Type.GetType("System.String")));
            VATReportDT.Columns.Add(new DataColumn("Overdue", Type.GetType("System.Boolean")));
            VATReportDT.Columns.Add(new DataColumn("TotalDays", Type.GetType("System.Int32")));

            string SelectCommand = @"SELECT ClientID, ClientName, ClientsManagers.ShortName FROM Clients 
INNER JOIN ClientsManagers ON Clients.ManagerID=ClientsManagers.ManagerID WHERE Enabled=1 ORDER BY ClientName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsDT);
                DA.Fill(FilterClientsDT);
                FilterClientsDT.Columns.Add(new DataColumn("Check", Type.GetType("System.Boolean")));
                for (int i = 0; i < FilterClientsDT.Rows.Count; i++)
                    FilterClientsDT.Rows[i]["Check"] = false;
            }
            SelectCommand = @"SELECT TOP 0 * FROM ClientsDispatches";
            UpdateClientsDispatchesDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString);
            UpdateClientsDispatchesDA.Fill(ClientsDispatchesDT);
            UpdateClientsDispatchesCB = new SqlCommandBuilder(UpdateClientsDispatchesDA);
            SelectCommand = @"SELECT TOP 0 * FROM ClientsIncomes";
            UpdateClientsIncomesDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString);
            UpdateClientsIncomesDA.Fill(ClientsIncomesDT);
            UpdateClientsIncomesCB = new SqlCommandBuilder(UpdateClientsIncomesDA);
            SelectCommand = @"SELECT * FROM CurrencyTypes";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(CurrencyTypesDT);
            }
            SelectCommand = @"SELECT * FROM DiscountPaymentConditions";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(DiscountPaymentConditionsDT);
            }
            SelectCommand = @"SELECT TOP 0 * FROM MutualSettlements";
            UpdateMutualSettlementsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString);
            UpdateMutualSettlementsDA.Fill(MutualSettlementsDT);
            MutualSettlementsDT.Columns.Add(new DataColumn("OrderNumbers", Type.GetType("System.String")));
            MutualSettlementsDT.Columns.Add(new DataColumn("CompareBalance", Type.GetType("System.Int32")));
            UpdateMutualSettlementsCB = new SqlCommandBuilder(UpdateMutualSettlementsDA);
            SelectCommand = @"SELECT TOP 0 * FROM MutualSettlementOrders";
            UpdateMutualSettlementOrdersDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString);
            UpdateMutualSettlementOrdersDA.Fill(MutualSettlementOrdersDT);
            UpdateMutualSettlementOrdersCB = new SqlCommandBuilder(UpdateMutualSettlementOrdersDA);

            //            SelectCommand = @"SELECT * FROM SubscribesRecords WHERE SubscribesItemID = 22
            //                AND UserTypeID = 0 AND UserID = " + Security.CurrentUserID;
            //            using (SqlDataAdapter sDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            //            {
            //                sDA.Fill(NewsSubsRecordsDataTable);
            //            }
        }

        public bool FillNewMutualSettlements(int FactoryID)
        {
            int ClientID = -1;
            int MutualSettlementID = -1;

            NewMutualSettlementsDT.Clear();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM MutualSettlements WHERE InvoiceNumber IS NULL AND FactoryID=" + FactoryID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            ClientID = Convert.ToInt32(DT.Rows[i]["ClientID"]);
                            MutualSettlementID = Convert.ToInt32(DT.Rows[i]["MutualSettlementID"]);
                            DataRow NewRow = NewMutualSettlementsDT.NewRow();
                            NewRow["MutualSettlementID"] = MutualSettlementID;
                            NewRow["ClientID"] = ClientID;
                            NewRow["ClientName"] = GetClientName(ClientID);
                            NewMutualSettlementsDT.Rows.Add(NewRow);
                        }
                        DataTable TempDT = NewMutualSettlementsDT.Clone();
                        using (DataView DV = new DataView(NewMutualSettlementsDT.Copy()))
                        {
                            DV.Sort = "ClientName, MutualSettlementID";
                            TempDT = DV.ToTable();
                        }
                        NewMutualSettlementsDT.Clear();
                        foreach (DataRow item in TempDT.Rows)
                        {
                            DataRow NewRow = NewMutualSettlementsDT.NewRow();
                            NewRow.ItemArray = item.ItemArray;
                            NewMutualSettlementsDT.Rows.Add(NewRow);
                        }
                    }
                }
            }

            return NewMutualSettlementsDT.Rows.Count > 0;
        }

        public bool FillNewClientsDispatches(int FactoryID)
        {
            int ClientID = -1;
            int MutualSettlementID = -1;

            NewClientsDispatchesDT.Clear();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ClientsDispatches.*, MutualSettlements.ClientID FROM ClientsDispatches 
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND FactoryID=" + FactoryID + " WHERE DispatchWaybills IS NULL ", ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            ClientID = Convert.ToInt32(DT.Rows[i]["ClientID"]);
                            MutualSettlementID = Convert.ToInt32(DT.Rows[i]["MutualSettlementID"]);
                            DataRow NewRow = NewClientsDispatchesDT.NewRow();
                            NewRow["MutualSettlementID"] = MutualSettlementID;
                            NewRow["ClientID"] = ClientID;
                            NewRow["ClientName"] = GetClientName(ClientID);
                            NewClientsDispatchesDT.Rows.Add(NewRow);
                        }
                        DataTable TempDT = NewClientsDispatchesDT.Clone();
                        using (DataView DV = new DataView(NewClientsDispatchesDT.Copy()))
                        {
                            DV.Sort = "ClientName, MutualSettlementID";
                            TempDT = DV.ToTable();
                        }
                        NewClientsDispatchesDT.Clear();
                        foreach (DataRow item in TempDT.Rows)
                        {
                            DataRow NewRow = NewClientsDispatchesDT.NewRow();
                            NewRow.ItemArray = item.ItemArray;
                            NewClientsDispatchesDT.Rows.Add(NewRow);
                        }
                    }
                }
            }

            return NewClientsDispatchesDT.Rows.Count > 0;
        }

        public bool FillSubscribes()
        {
            int ClientID = -1;
            int FactoryID = -1;
            int MutualSettlementID = -1;

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TableItemID FROM SubscribesRecords WHERE UserTypeID = 0 AND SubscribesItemID = 22 AND UserID = " + Security.CurrentUserID, ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    foreach (DataRow Row in DT.Rows)
                    {
                        MutualSettlementID = Convert.ToInt32(Row["TableItemID"]);
                        using (SqlDataAdapter DA1 = new SqlDataAdapter(@"SELECT * FROM MutualSettlements WHERE MutualSettlementID=" + MutualSettlementID, ConnectionStrings.MarketingOrdersConnectionString))
                        {
                            using (DataTable DT1 = new DataTable())
                            {
                                if (DA1.Fill(DT1) > 0)
                                {
                                    ClientID = Convert.ToInt32(DT1.Rows[0]["ClientID"]);
                                    FactoryID = Convert.ToInt32(DT1.Rows[0]["FactoryID"]);
                                    DataRow NewRow = SubscribesDT.NewRow();
                                    NewRow["MutualSettlementID"] = MutualSettlementID;
                                    NewRow["ClientID"] = ClientID;
                                    NewRow["FactoryID"] = FactoryID;
                                    SubscribesDT.Rows.Add(NewRow);
                                }
                            }
                        }
                    }
                }
            }

            return SubscribesDT.Rows.Count > 0;
        }

        public void AddSubscribesRecord(int UserID, int MutualSettlementID)
        {
            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT TOP 0 * FROM SubscribesRecords", ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder uCB = new SqlCommandBuilder(uDA))
                {
                    using (DataTable uDT = new DataTable())
                    {
                        uDA.Fill(uDT);

                        DataRow cNewRow = uDT.NewRow();
                        cNewRow["TableItemID"] = MutualSettlementID;
                        cNewRow["UserID"] = UserID;
                        cNewRow["SubscribesItemID"] = 22;
                        uDT.Rows.Add(cNewRow);

                        uDA.Update(uDT);
                    }
                }
            }
        }

        public void ClearSubscribes(int MutualSettlementID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"DELETE FROM SubscribesRecords WHERE SubscribesItemID = 22 AND UserTypeID = 0 AND UserID = " + Security.CurrentUserID +
                " AND TableItemID =" + MutualSettlementID, ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                }
            }
        }

        private void Binding()
        {
            FilterClientsBS.DataSource = FilterClientsDT;
            ClientsBS.DataSource = ClientsDT;
            ClientsDispatchesBS.DataSource = ClientsDispatchesDT;
            ClientsIncomesBS.DataSource = ClientsIncomesDT;
            MutualSettlementsBS.DataSource = MutualSettlementsDT;
            NewMutualSettlementsBS.DataSource = NewMutualSettlementsDT;
            NewClientsDispatchesBS.DataSource = NewClientsDispatchesDT;
        }

        public DataGridViewComboBoxColumn ClientsColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ClientsColumn",
                    HeaderText = "Клиент",
                    DataPropertyName = "ClientID",
                    DataSource = new DataView(ClientsDT),
                    ValueMember = "ClientID",
                    DisplayMember = "ClientName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 100
                };
                return Column;
            }
        }

        public DataGridViewComboBoxColumn CurrencyTypeColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "CurrencyTypeColumn",
                    HeaderText = "Валюта",
                    DataPropertyName = "CurrencyTypeID",
                    DataSource = new DataView(CurrencyTypesDT),
                    ValueMember = "CurrencyTypeID",
                    DisplayMember = "CurrencyType",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 60
                };
                return Column;
            }
        }

        public KryptonDataGridViewDateTimePickerColumn DispatchDateTimeColumn
        {
            get
            {
                KryptonDataGridViewDateTimePickerColumn Column = new KryptonDataGridViewDateTimePickerColumn()
                {
                    CalendarTodayDate = DateTime.Now
                };
                Column.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                Column.Checked = false;
                Column.DataPropertyName = "DispatchDateTime";
                Column.HeaderText = "Дата отгрузки";
                Column.Name = "DispatchDateTimeColumn";
                Column.SortMode = DataGridViewColumnSortMode.Automatic;
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                Column.MinimumWidth = 100;
                return Column;
            }
        }

        public KryptonDataGridViewDateTimePickerColumn InvoiceDateTimeColumn
        {
            get
            {
                KryptonDataGridViewDateTimePickerColumn Column = new KryptonDataGridViewDateTimePickerColumn()
                {
                    CalendarTodayDate = DateTime.Now
                };
                Column.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                Column.Checked = false;
                Column.DataPropertyName = "InvoiceDateTime";
                Column.HeaderText = "Дата счета";
                Column.Name = "InvoiceDateTimeColumn";
                Column.SortMode = DataGridViewColumnSortMode.Automatic;
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                Column.MinimumWidth = 100;
                return Column;
            }
        }

        public KryptonDataGridViewDateTimePickerColumn IncomeDateTimeColumn
        {
            get
            {
                KryptonDataGridViewDateTimePickerColumn Column = new KryptonDataGridViewDateTimePickerColumn()
                {
                    CalendarTodayDate = DateTime.Now
                };
                Column.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                Column.Checked = false;
                Column.DataPropertyName = "IncomeDateTime";
                Column.HeaderText = "Дата поступления";
                Column.Name = "IncomeDateTimeColumn";
                Column.SortMode = DataGridViewColumnSortMode.Automatic;
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                Column.MinimumWidth = 100;
                return Column;
            }
        }

        public DataGridViewComboBoxColumn PaymentConditionColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "PaymentConditionColumn",
                    HeaderText = "Условия\r\nоплаты, %",
                    DataPropertyName = "DiscountPaymentConditionID",
                    DataSource = new DataView(DiscountPaymentConditionsDT),
                    ValueMember = "DiscountPaymentConditionID",
                    DisplayMember = "Name",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    MinimumWidth = 100
                };
                return Column;
            }
        }

        public void CheckAllClients(bool Check)
        {
            for (int i = 0; i < FilterClientsDT.Rows.Count; i++)
                FilterClientsDT.Rows[i]["Check"] = Check;
        }

        public void AddClientDispatch(int MutualSettlementID, int CurrencyTypeID)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow NewRow = ClientsDispatchesDT.NewRow();
            NewRow["DispatchSum"] = 0;
            NewRow["MutualSettlementID"] = MutualSettlementID;
            NewRow["CurrencyTypeID"] = CurrencyTypeID;
            NewRow["DispatchDateTime"] = CurrentDate;
            NewRow["CreateUserID"] = Security.CurrentUserID;
            NewRow["CreateDateTime"] = CurrentDate;
            ClientsDispatchesDT.Rows.Add(NewRow);
        }

        public int AddClientDispatch(int MutualSettlementID, int CurrencyTypeID, decimal DispatchSum)
        {
            int ClientDispatchID = -1;
            string SelectCommand = @"SELECT TOP 1 * FROM ClientsDispatches WHERE MutualSettlementID=" + MutualSettlementID + " ORDER BY ClientDispatchID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(ClientsDispatchesDT);

                    DateTime CurrentDate = Security.GetCurrentDate();
                    DataRow NewRow = ClientsDispatchesDT.NewRow();
                    NewRow["DispatchSum"] = 0;
                    NewRow["MutualSettlementID"] = MutualSettlementID;
                    NewRow["CurrencyTypeID"] = CurrencyTypeID;
                    NewRow["DispatchDateTime"] = CurrentDate;
                    NewRow["CreateUserID"] = Security.CurrentUserID;
                    NewRow["CreateDateTime"] = CurrentDate;
                    NewRow["DispatchSum"] = DispatchSum;
                    ClientsDispatchesDT.Rows.Add(NewRow);

                    DA.Update(ClientsDispatchesDT);
                    ClientsDispatchesDT.Clear();
                    DA.Fill(ClientsDispatchesDT);
                    if (ClientsDispatchesDT.Rows.Count > 0)
                        ClientDispatchID = Convert.ToInt32(ClientsDispatchesDT.Rows[0]["ClientDispatchID"]);
                }
            }
            return ClientDispatchID;
        }

        public void AddClientIncome(int MutualSettlementID, int CurrencyTypeID)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow NewRow = ClientsIncomesDT.NewRow();
            NewRow["IncomeSum"] = 0;
            NewRow["MutualSettlementID"] = MutualSettlementID;
            NewRow["CurrencyTypeID"] = CurrencyTypeID;
            NewRow["IncomeDateTime"] = CurrentDate;
            NewRow["CreateUserID"] = Security.CurrentUserID;
            NewRow["CreateDateTime"] = CurrentDate;
            ClientsIncomesDT.Rows.Add(NewRow);
        }

        public void AddClientIncome(int MutualSettlementID, int CurrencyTypeID, decimal IncomeSum)
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow NewRow = ClientsIncomesDT.NewRow();
            NewRow["IncomeSum"] = 0;
            NewRow["MutualSettlementID"] = MutualSettlementID;
            NewRow["CurrencyTypeID"] = CurrencyTypeID;
            NewRow["IncomeDateTime"] = CurrentDate;
            NewRow["IncomeSum"] = IncomeSum;
            NewRow["CreateUserID"] = Security.CurrentUserID;
            NewRow["CreateDateTime"] = CurrentDate;
            ClientsIncomesDT.Rows.Add(NewRow);
        }

        public void EditClientIncome(int MutualSettlementID)
        {
            DataRow[] rows = ClientsIncomesDT.Select("MutualSettlementID=-1");
            if (rows.Count() > 0)
            {
                rows[0]["MutualSettlementID"] = MutualSettlementID;
            }
            NeedCreateIncome = false;
        }

        public void AddMutualSettlement(int ClientID, int FactoryID, int CurrencyTypeID)
        {
            decimal OpeningBalance = 0;
            string SelectCommand = @"SELECT TOP 1 * FROM MutualSettlements WHERE ClientID=" + ClientID + " AND FactoryID=" + FactoryID + " ORDER BY MutualSettlementID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (DT.Rows.Count > 0)
                            OpeningBalance = Convert.ToDecimal(DT.Rows[0]["ClosingBalance"]);
                        //if (OpeningBalance < 0)
                        //{
                        //    NeedCreateIncome = true;
                        //    AddClientIncome(-1, 1, OpeningBalance * (-1));
                        //}
                    }
                }
            }

            DateTime CurrentDate = Security.GetCurrentDate();
            DataRow NewRow = MutualSettlementsDT.NewRow();
            NewRow["ClientID"] = ClientID;
            NewRow["FactoryID"] = FactoryID;
            NewRow["CurrencyTypeID"] = CurrencyTypeID;
            NewRow["OpeningBalance"] = OpeningBalance;
            NewRow["CompareBalance"] = 0;
            NewRow["IsSample"] = false;
            //if (!NeedCreateIncome)
            //    NewRow["OpeningBalance"] = OpeningBalance;
            //else
            //{
            //    NewRow["OpeningBalance"] = 0;
            //    NewRow["ClosingBalance"] = OpeningBalance * (-1);
            //}
            NewRow["DiscountPaymentConditionID"] = 1;
            NewRow["CreateUserID"] = Security.CurrentUserID;
            NewRow["CreateDateTime"] = CurrentDate;
            NewRow["InvoiceDateTime"] = CurrentDate;
            MutualSettlementsDT.Rows.Add(NewRow);
        }

        public void AddMutualSettlementOrder(DateTime CurrentDate, int ClientID, int MutualSettlementID, int OrderNumber)
        {
            DataRow NewRow = MutualSettlementOrdersDT.NewRow();
            NewRow["ClientID"] = ClientID;
            NewRow["MutualSettlementID"] = MutualSettlementID;
            NewRow["OrderNumber"] = OrderNumber;
            NewRow["CreateUserID"] = Security.CurrentUserID;
            NewRow["CreateDateTime"] = CurrentDate;
            MutualSettlementOrdersDT.Rows.Add(NewRow);
        }

        public int AddMutualSettlement(int ClientID, int FactoryID, int CurrencyTypeID, int DiscountPaymentConditionID,
            int DelayOfPayment, decimal TotalInvoiceSum, string Notes, bool IsSample = false)
        {
            int MutualSettlementID = -1;
            string SelectCommand = @"SELECT TOP 1 * FROM MutualSettlements WHERE ClientID=" + ClientID + " AND FactoryID=" + FactoryID + " ORDER BY MutualSettlementID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(MutualSettlementsDT);
                    decimal OpeningBalance = 0;

                    if (MutualSettlementsDT.Rows.Count > 0)
                        OpeningBalance = Convert.ToDecimal(MutualSettlementsDT.Rows[0]["ClosingBalance"]);
                    DateTime CurrentDate = Security.GetCurrentDate();
                    DataRow NewRow = MutualSettlementsDT.NewRow();
                    NewRow["ClientID"] = ClientID;
                    NewRow["IsSample"] = IsSample;
                    NewRow["FactoryID"] = FactoryID;
                    NewRow["CurrencyTypeID"] = CurrencyTypeID;
                    NewRow["OpeningBalance"] = OpeningBalance;
                    NewRow["DiscountPaymentConditionID"] = DiscountPaymentConditionID;
                    NewRow["DelayOfPayment"] = DelayOfPayment;
                    NewRow["TotalInvoiceSum"] = TotalInvoiceSum;
                    NewRow["CreateUserID"] = Security.CurrentUserID;
                    NewRow["CreateDateTime"] = CurrentDate;
                    NewRow["InvoiceDateTime"] = CurrentDate;
                    if (Notes.Length > 0)
                        NewRow["Notes"] = Notes;
                    MutualSettlementsDT.Rows.Add(NewRow);

                    DA.Update(MutualSettlementsDT);
                    MutualSettlementsDT.Clear();
                    DA.Fill(MutualSettlementsDT);
                    if (MutualSettlementsDT.Rows.Count > 0)
                        MutualSettlementID = Convert.ToInt32(MutualSettlementsDT.Rows[0]["MutualSettlementID"]);
                }
            }
            return MutualSettlementID;
        }

        public void RemoveClientsDispatches()
        {
            if (ClientsDispatchesBS.Current != null)
                ClientsDispatchesBS.RemoveCurrent();
        }

        public void RemoveClientsDispatches(int MutualSettlementID)
        {
            DataRow[] rows = ClientsDispatchesDT.Select("MutualSettlementID=" + MutualSettlementID);
            for (int i = rows.Count() - 1; i >= 0; i--)
                rows[i].Delete();
        }

        public void RemoveClientsIncomes()
        {
            if (ClientsIncomesBS.Current != null)
                ClientsIncomesBS.RemoveCurrent();
        }

        public void RemoveClientsIncomes(int MutualSettlementID)
        {
            DataRow[] rows = ClientsIncomesDT.Select("MutualSettlementID=" + MutualSettlementID);
            for (int i = rows.Count() - 1; i >= 0; i--)
                rows[i].Delete();
        }

        public void RemoveMutualSettlement()
        {
            if (MutualSettlementsBS.Current != null)
                MutualSettlementsBS.RemoveCurrent();
        }

        public void RemoveMutualSettlementOrders(int MutualSettlementID)
        {
            DataRow[] rows = MutualSettlementOrdersDT.Select("MutualSettlementID=" + MutualSettlementID);
            for (int i = rows.Count() - 1; i >= 0; i--)
                rows[i].Delete();
        }

        public void MoveToMutualSettlement(int MutualSettlementID)
        {
            MutualSettlementsBS.Position = MutualSettlementsBS.Find("MutualSettlementID", MutualSettlementID);
        }

        public void MoveToClient(int ClientID)
        {
            ClientsBS.Position = ClientsBS.Find("ClientID", ClientID);
        }

        public void GetAllClientsDispatches()
        {
            string SelectCommand = @"SELECT * FROM ClientsDispatches";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                AllClientsDispatchesDT.Clear();
                DA.Fill(AllClientsDispatchesDT);
            }
        }

        public void GetAllClientsIncomes()
        {
            string SelectCommand = @"SELECT * FROM ClientsIncomes";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                AllClientsIncomesDT.Clear();
                DA.Fill(AllClientsIncomesDT);
            }
        }

        public string ManagerName(int ClientID)
        {
            DataRow[] rows = ClientsDT.Select("ClientID = " + ClientID);
            if (rows.Count() > 0)
                return rows[0]["ShortName"].ToString();
            return string.Empty;
        }

        public void CalcClosingBalance(int ClientID, int FactoryID)
        {
            int MutualSettlementID = 0;
            decimal OpeningBalance = 0;
            decimal ClosingBalance = 0;
            decimal IncomeSum = 0;
            decimal DispatchSum = 0;
            decimal TotalIncomeSum = 0;
            decimal TotalDispatchSum = 0;

            DataTable MutDT = new DataTable();
            DataTable DispDT = new DataTable();
            DataTable IncomeDT = new DataTable();
            string SelectCommand = @"SELECT * FROM MutualSettlements WHERE ClientID=" + ClientID + " AND FactoryID=" + FactoryID + " ORDER BY InvoiceDateTime";
            using (SqlDataAdapter mDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(mDA))
                {
                    mDA.Fill(MutDT);
                    for (int i = 0; i < MutDT.Rows.Count; i++)
                    {
                        int CurrencyTypeID = Convert.ToInt32(MutDT.Rows[i]["CurrencyTypeID"]);
                        if (CurrencyTypeID == 2 || CurrencyTypeID == 3 || CurrencyTypeID == 5)
                            continue;
                        TotalIncomeSum = 0;
                        TotalDispatchSum = 0;
                        if (i == 0)
                            OpeningBalance = Convert.ToDecimal(MutDT.Rows[i]["OpeningBalance"]);
                        else
                            OpeningBalance = ClosingBalance;
                        MutualSettlementID = Convert.ToInt32(MutDT.Rows[i]["MutualSettlementID"]);
                        DataRow[] rows = AllClientsDispatchesDT.Select("MutualSettlementID=" + MutualSettlementID);
                        for (int j = 0; j < rows.Count(); j++)
                        {
                            DispatchSum = Convert.ToDecimal(rows[j]["DispatchSum"]);
                            TotalDispatchSum += DispatchSum;
                        }
                        rows = AllClientsIncomesDT.Select("MutualSettlementID=" + MutualSettlementID);
                        for (int j = 0; j < rows.Count(); j++)
                        {
                            IncomeSum = Convert.ToDecimal(rows[j]["IncomeSum"]);
                            TotalIncomeSum += IncomeSum;
                        }
                        ClosingBalance = OpeningBalance + TotalDispatchSum - TotalIncomeSum;
                        MutDT.Rows[i]["OpeningBalance"] = OpeningBalance;
                        MutDT.Rows[i]["ClosingBalance"] = ClosingBalance;
                    }
                    OpeningBalance = 0;
                    ClosingBalance = 0;
                    IncomeSum = 0;
                    DispatchSum = 0;
                    TotalIncomeSum = 0;
                    TotalDispatchSum = 0;
                    for (int i = 0; i < MutDT.Rows.Count; i++)
                    {
                        int CurrencyTypeID = Convert.ToInt32(MutDT.Rows[i]["CurrencyTypeID"]);
                        if (CurrencyTypeID == 1 || CurrencyTypeID == 3 || CurrencyTypeID == 5)
                            continue;
                        TotalIncomeSum = 0;
                        TotalDispatchSum = 0;
                        if (i == 0)
                            OpeningBalance = Convert.ToDecimal(MutDT.Rows[i]["OpeningBalance"]);
                        else
                            OpeningBalance = ClosingBalance;
                        MutualSettlementID = Convert.ToInt32(MutDT.Rows[i]["MutualSettlementID"]);
                        DataRow[] rows = AllClientsDispatchesDT.Select("MutualSettlementID=" + MutualSettlementID);
                        for (int j = 0; j < rows.Count(); j++)
                        {
                            DispatchSum = Convert.ToDecimal(rows[j]["DispatchSum"]);
                            TotalDispatchSum += DispatchSum;
                        }
                        rows = AllClientsIncomesDT.Select("MutualSettlementID=" + MutualSettlementID);
                        for (int j = 0; j < rows.Count(); j++)
                        {
                            IncomeSum = Convert.ToDecimal(rows[j]["IncomeSum"]);
                            TotalIncomeSum += IncomeSum;
                        }
                        ClosingBalance = OpeningBalance + TotalDispatchSum - TotalIncomeSum;
                        MutDT.Rows[i]["OpeningBalance"] = OpeningBalance;
                        MutDT.Rows[i]["ClosingBalance"] = ClosingBalance;
                    }
                    OpeningBalance = 0;
                    ClosingBalance = 0;
                    IncomeSum = 0;
                    DispatchSum = 0;
                    TotalIncomeSum = 0;
                    TotalDispatchSum = 0;
                    for (int i = 0; i < MutDT.Rows.Count; i++)
                    {
                        int CurrencyTypeID = Convert.ToInt32(MutDT.Rows[i]["CurrencyTypeID"]);
                        if (CurrencyTypeID == 1 || CurrencyTypeID == 2 || CurrencyTypeID == 5)
                            continue;
                        TotalIncomeSum = 0;
                        TotalDispatchSum = 0;
                        if (i == 0)
                            OpeningBalance = Convert.ToDecimal(MutDT.Rows[i]["OpeningBalance"]);
                        else
                            OpeningBalance = ClosingBalance;
                        MutualSettlementID = Convert.ToInt32(MutDT.Rows[i]["MutualSettlementID"]);
                        DataRow[] rows = AllClientsDispatchesDT.Select("MutualSettlementID=" + MutualSettlementID);
                        for (int j = 0; j < rows.Count(); j++)
                        {
                            DispatchSum = Convert.ToDecimal(rows[j]["DispatchSum"]);
                            TotalDispatchSum += DispatchSum;
                        }
                        rows = AllClientsIncomesDT.Select("MutualSettlementID=" + MutualSettlementID);
                        for (int j = 0; j < rows.Count(); j++)
                        {
                            IncomeSum = Convert.ToDecimal(rows[j]["IncomeSum"]);
                            TotalIncomeSum += IncomeSum;
                        }
                        ClosingBalance = OpeningBalance + TotalDispatchSum - TotalIncomeSum;
                        MutDT.Rows[i]["OpeningBalance"] = OpeningBalance;
                        MutDT.Rows[i]["ClosingBalance"] = ClosingBalance;
                    }
                    OpeningBalance = 0;
                    ClosingBalance = 0;
                    IncomeSum = 0;
                    DispatchSum = 0;
                    TotalIncomeSum = 0;
                    TotalDispatchSum = 0;
                    for (int i = 0; i < MutDT.Rows.Count; i++)
                    {
                        int CurrencyTypeID = Convert.ToInt32(MutDT.Rows[i]["CurrencyTypeID"]);
                        if (CurrencyTypeID == 2 || CurrencyTypeID == 3 || CurrencyTypeID == 1)
                            continue;
                        TotalIncomeSum = 0;
                        TotalDispatchSum = 0;
                        if (i == 0)
                            OpeningBalance = Convert.ToDecimal(MutDT.Rows[i]["OpeningBalance"]);
                        else
                            OpeningBalance = ClosingBalance;
                        MutualSettlementID = Convert.ToInt32(MutDT.Rows[i]["MutualSettlementID"]);
                        DataRow[] rows = AllClientsDispatchesDT.Select("MutualSettlementID=" + MutualSettlementID);
                        for (int j = 0; j < rows.Count(); j++)
                        {
                            DispatchSum = Convert.ToDecimal(rows[j]["DispatchSum"]);
                            TotalDispatchSum += DispatchSum;
                        }
                        rows = AllClientsIncomesDT.Select("MutualSettlementID=" + MutualSettlementID);
                        for (int j = 0; j < rows.Count(); j++)
                        {
                            IncomeSum = Convert.ToDecimal(rows[j]["IncomeSum"]);
                            TotalIncomeSum += IncomeSum;
                        }
                        ClosingBalance = OpeningBalance + TotalDispatchSum - TotalIncomeSum;
                        MutDT.Rows[i]["OpeningBalance"] = OpeningBalance;
                        MutDT.Rows[i]["ClosingBalance"] = ClosingBalance;
                    }
                    mDA.Update(MutDT);
                }
            }
        }

        public bool VerifyTransaction(int MutualSettlementID, decimal DispatchSum, ref decimal Discount)
        {
            bool bVerify = false;
            int DiscountPaymentConditionID = 0;
            decimal SevenDaysIncomeSum = 0;
            decimal DeadlineIncomeSum = 0;

            DataTable MutDT = new DataTable();
            DataTable DispDT = new DataTable();
            DataTable IncomeDT = new DataTable();
            string SelectCommand = @"SELECT * FROM MutualSettlements WHERE MutualSettlementID=" + MutualSettlementID;
            using (SqlDataAdapter mDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                mDA.Fill(MutDT);
                DiscountPaymentConditionID = Convert.ToInt32(MutDT.Rows[0]["DiscountPaymentConditionID"]);
                if (DiscountPaymentConditionID == 2 || DiscountPaymentConditionID == 3 || DiscountPaymentConditionID == 5)
                {
                    DateTime InvoiceDateTime = Convert.ToDateTime(MutDT.Rows[0]["InvoiceDateTime"]);
                    DateTime SevenDays = InvoiceDateTime.AddDays(7);

                    if (DiscountPaymentConditionID == 5)
                    {
                        SelectCommand = @"SELECT * FROM ClientsIncomes WHERE MutualSettlementID=" + MutualSettlementID;
                        using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                        {
                            IncomeDT.Clear();
                            DA1.Fill(IncomeDT);
                            SevenDaysIncomeSum = 0;
                            DeadlineIncomeSum = 0;
                            for (int x = 0; x < IncomeDT.Rows.Count; x++)
                            {
                                DateTime IncomeDateTime = Convert.ToDateTime(IncomeDT.Rows[x]["IncomeDateTime"]);
                                if (IncomeDateTime.Date <= SevenDays.Date)
                                    SevenDaysIncomeSum += Convert.ToDecimal(IncomeDT.Rows[x]["IncomeSum"]);
                                if (IncomeDateTime.Date <= DateTime.Now.Date)
                                    DeadlineIncomeSum += Convert.ToDecimal(IncomeDT.Rows[x]["IncomeSum"]);
                            }
                            if (SevenDaysIncomeSum == 0)
                                Discount = 0;
                            else
                            {
                                if (SevenDaysIncomeSum / DispatchSum < 0.5m)
                                    Discount = 0;
                                else
                                {
                                    if (SevenDaysIncomeSum / DispatchSum >= 1)
                                    {
                                        Discount = 8;
                                        bVerify = true;
                                    }
                                    else
                                    {
                                        if (DeadlineIncomeSum / DispatchSum >= 1)
                                        {
                                            Discount = 8;
                                            bVerify = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (DiscountPaymentConditionID == 2)
                    {
                        SelectCommand = @"SELECT * FROM ClientsIncomes WHERE MutualSettlementID=" + MutualSettlementID;
                        using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                        {
                            IncomeDT.Clear();
                            DA1.Fill(IncomeDT);
                            SevenDaysIncomeSum = 0;
                            DeadlineIncomeSum = 0;
                            for (int x = 0; x < IncomeDT.Rows.Count; x++)
                            {
                                DateTime IncomeDateTime = Convert.ToDateTime(IncomeDT.Rows[x]["IncomeDateTime"]);
                                if (IncomeDateTime.Date <= SevenDays.Date)
                                    SevenDaysIncomeSum += Convert.ToDecimal(IncomeDT.Rows[x]["IncomeSum"]);
                                if (IncomeDateTime.Date <= DateTime.Now.Date)
                                    DeadlineIncomeSum += Convert.ToDecimal(IncomeDT.Rows[x]["IncomeSum"]);
                            }
                            if (SevenDaysIncomeSum == 0)
                                Discount = 0;
                            else
                            {
                                if (SevenDaysIncomeSum / DispatchSum < 0.5m)
                                    Discount = 0;
                                else
                                {
                                    if (SevenDaysIncomeSum / DispatchSum >= 1)
                                    {
                                        Discount = 6;
                                        bVerify = true;
                                    }
                                    else
                                    {
                                        if (DeadlineIncomeSum / DispatchSum >= 1)
                                        {
                                            Discount = 6;
                                            bVerify = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (DiscountPaymentConditionID == 3)
                    {
                        SelectCommand = @"SELECT * FROM ClientsIncomes WHERE MutualSettlementID=" + MutualSettlementID;
                        using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                        {
                            IncomeDT.Clear();
                            DA1.Fill(IncomeDT);
                            SevenDaysIncomeSum = 0;
                            DeadlineIncomeSum = 0;
                            for (int x = 0; x < IncomeDT.Rows.Count; x++)
                            {
                                DateTime IncomeDateTime = Convert.ToDateTime(IncomeDT.Rows[x]["IncomeDateTime"]);
                                if (IncomeDateTime.Date <= SevenDays.Date)
                                    SevenDaysIncomeSum += Convert.ToDecimal(IncomeDT.Rows[x]["IncomeSum"]);
                                if (IncomeDateTime.Date <= DateTime.Now.Date)
                                    DeadlineIncomeSum += Convert.ToDecimal(IncomeDT.Rows[x]["IncomeSum"]);
                            }
                            if (SevenDaysIncomeSum == 0)
                                Discount = 0;
                            else
                            {
                                if (SevenDaysIncomeSum / DispatchSum < 0.5m)
                                    Discount = 0;
                                else
                                {
                                    if (SevenDaysIncomeSum / DispatchSum >= 1)
                                    {
                                        Discount = 10;
                                        bVerify = true;
                                    }
                                    else
                                    {
                                        if (DeadlineIncomeSum / DispatchSum >= 1)
                                        {
                                            Discount = 6;
                                            bVerify = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return bVerify;
        }

        public void GetClientsDispatches(int ClientID, int FactoryID, DateTime date1, DateTime date2)
        {
            string SelectCommand = @"SELECT * FROM ClientsDispatches 
                WHERE MutualSettlementID IN (SELECT MutualSettlementID FROM MutualSettlements 
                WHERE ClientID=" + ClientID + " AND FactoryID=" + FactoryID + ") ORDER BY DispatchDateTime DESC";
            ClientsDispatchesDT.Clear();
            if (SelectClientsDispatchesDA == null)
                SelectClientsDispatchesDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString);
            SelectClientsDispatchesDA.SelectCommand.CommandText = SelectCommand;
            SelectClientsDispatchesDA.Fill(ClientsDispatchesDT);
        }

        public void GetClientsIncomes(int ClientID, int FactoryID, DateTime date1, DateTime date2)
        {
            string SelectCommand = @"SELECT * FROM ClientsIncomes WHERE MutualSettlementID IN (SELECT MutualSettlementID FROM MutualSettlements WHERE ClientID=" + ClientID + " AND FactoryID=" + FactoryID + ")";
            ClientsIncomesDT.Clear();
            if (SelectClientsIncomesDA == null)
                SelectClientsIncomesDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString);
            SelectClientsIncomesDA.SelectCommand.CommandText = SelectCommand;
            SelectClientsIncomesDA.Fill(ClientsIncomesDT);
        }

        public int LastMutualSettlementID(int ClientID, int FactoryID)
        {
            int MutualSettlementID = -1;
            string SelectCommand = @"SELECT TOP 1 * FROM MutualSettlements WHERE ClientID=" + ClientID + " AND FactoryID=" + FactoryID + " ORDER BY MutualSettlementID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        MutualSettlementID = Convert.ToInt32(DT.Rows[0]["MutualSettlementID"]);
                }
            }
            return MutualSettlementID;
        }

        public void FilterMutualSettlements(bool Result0, bool Result1, bool Result2, bool Result3, int CurrencyTypeID)
        {
            string Filter = "CompareBalance=-1";
            if (Result0)
            {
                if (Filter.Length == 0)
                    Filter = "CompareBalance=0";
                else
                    Filter += " OR CompareBalance=0";
            }
            if (Result1)
            {
                if (Filter.Length == 0)
                    Filter = "CompareBalance=1";
                else
                    Filter += " OR CompareBalance=1";
            }
            if (Result2)
            {
                if (Filter.Length == 0)
                    Filter = "CompareBalance=2";
                else
                    Filter += " OR CompareBalance=2";
            }
            if (Result3)
            {
                if (Filter.Length == 0)
                    Filter = "CompareBalance=3";
                else
                    Filter += " OR CompareBalance=3";
            }
            if (Filter.Length > 0)
                Filter = "(" + Filter + ") AND CurrencyTypeID=" + CurrencyTypeID;
            MutualSettlementsBS.Filter = Filter;
            MutualSettlementsBS.MoveFirst();
        }

        public void GetMutualSettlements(int ClientID, int FactoryID, DateTime date1, DateTime date2)
        {
            string SelectCommand = @"SELECT * FROM MutualSettlements WHERE ClientID=" + ClientID + " AND FactoryID=" + FactoryID + " ORDER BY CAST(InvoiceDateTime AS date) DESC, CreateDateTime DESC";
            MutualSettlementsDT.Clear();
            if (SelectMutualSettlementsDA == null)
                SelectMutualSettlementsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString);
            SelectMutualSettlementsDA.SelectCommand.CommandText = SelectCommand;
            SelectMutualSettlementsDA.Fill(MutualSettlementsDT);
            for (int i = 0; i < MutualSettlementsDT.Rows.Count; i++)
            {
                MutualSettlementsDT.Rows[i]["OrderNumbers"] = GetOrdersNumbers(Convert.ToInt32(MutualSettlementsDT.Rows[i]["MutualSettlementID"]));
                MutualSettlementsDT.Rows[i]["CompareBalance"] = CompareBalance(Convert.ToInt32(MutualSettlementsDT.Rows[i]["MutualSettlementID"]));
            }
        }

        public void GetMutualSettlementOrders(int ClientID, int FactoryID, DateTime date1, DateTime date2)
        {
            string SelectCommand = @"SELECT * FROM MutualSettlementOrders WHERE MutualSettlementID IN (SELECT MutualSettlementID FROM MutualSettlements WHERE ClientID=" + ClientID + " AND FactoryID=" + FactoryID + ")";
            MutualSettlementOrdersDT.Clear();
            if (SelectMutualSettlementOrdersDA == null)
                SelectMutualSettlementOrdersDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString);
            SelectMutualSettlementOrdersDA.SelectCommand.CommandText = SelectCommand;
            SelectMutualSettlementOrdersDA.Fill(MutualSettlementOrdersDT);
        }

        public string GetOrdersNumbers(int MutualSettlementID)
        {
            string OrderNumbers = string.Empty;
            DataRow[] rows = MutualSettlementOrdersDT.Select("MutualSettlementID=" + MutualSettlementID);
            for (int i = 0; i < rows.Count(); i++)
                OrderNumbers += Convert.ToInt32(rows[i]["OrderNumber"]) + ",";
            if (OrderNumbers.Length > 0)
                OrderNumbers = OrderNumbers.Substring(0, OrderNumbers.Length - 1);
            return OrderNumbers;
        }

        public decimal GetDispatchSum(int MutualSettlementID)
        {
            decimal DispatchSum = 0;
            string SelectCommand = @"SELECT * FROM MutualSettlements WHERE MutualSettlementID=" + MutualSettlementID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        DispatchSum = Convert.ToDecimal(DT.Rows[0]["TotalInvoiceSum"]);
                        //DispatchSum = Decimal.Round(DispatchSum, 2, MidpointRounding.AwayFromZero);
                        return DispatchSum;
                    }
                }
            }

            return DispatchSum;
        }

        public int FindMutualSettlementID(int ClientID, int FactoryID)
        {
            int MutualSettlementID = 0;
            string SelectCommand = @"SELECT * FROM MutualSettlements WHERE ClientID=" + ClientID + " AND FactoryID=" + FactoryID + " ORDER BY MutualSettlementID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        MutualSettlementID = Convert.ToInt32(DT.Rows[0]["MutualSettlementID"]);
                        return MutualSettlementID;
                    }
                }
            }

            return MutualSettlementID;
        }

        public int FindMutualSettlementIDByOrderNumber(int ClientID, int FactoryID, int[] OrderNumbers)
        {
            int MutualSettlementID = 0;
            if (OrderNumbers.Count() == 0)
                return MutualSettlementID;
            string SelectCommand = @"SELECT * FROM MutualSettlementOrders WHERE OrderNumber IN (" + string.Join(",", OrderNumbers) +
                ") AND MutualSettlementID IN (SELECT MutualSettlementID FROM MutualSettlements WHERE ClientID=" + ClientID + " AND FactoryID=" + FactoryID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        for (int x = 0; x < OrderNumbers.Count(); x++)
                        {
                            int OrderNumber1 = OrderNumbers[x];
                            if (Convert.ToInt32(DT.Rows[i]["OrderNumber"]) == OrderNumber1)
                            {
                                MutualSettlementID = Convert.ToInt32(DT.Rows[i]["MutualSettlementID"]);
                                return MutualSettlementID;
                            }
                        }
                    }
                }
            }

            return MutualSettlementID;
        }

        public int FindMutualSettlementIDByOrderNumber(int ClientID, int FactoryID, int[] OrderNumbers, bool IsSample)
        {
            int MutualSettlementID = 0;
            if (OrderNumbers.Count() == 0)
                return MutualSettlementID;

            string SelectCommand = @"SELECT * FROM MutualSettlementOrders WHERE OrderNumber IN (" + string.Join(",", OrderNumbers) +
                ") AND MutualSettlementID IN (SELECT MutualSettlementID FROM MutualSettlements WHERE IsSample=1 AND ClientID=" + ClientID + " AND FactoryID=" + FactoryID + ")";
            if (!IsSample)
                SelectCommand = @"SELECT * FROM MutualSettlementOrders WHERE OrderNumber IN (" + string.Join(",", OrderNumbers) +
                ") AND MutualSettlementID IN (SELECT MutualSettlementID FROM MutualSettlements WHERE IsSample=0 AND ClientID=" + ClientID + " AND FactoryID=" + FactoryID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        for (int x = 0; x < OrderNumbers.Count(); x++)
                        {
                            int OrderNumber1 = OrderNumbers[x];
                            if (Convert.ToInt32(DT.Rows[i]["OrderNumber"]) == OrderNumber1)
                            {
                                MutualSettlementID = Convert.ToInt32(DT.Rows[i]["MutualSettlementID"]);
                                return MutualSettlementID;
                            }
                        }
                    }
                }
            }

            return MutualSettlementID;
        }

        public void FilterClientsDispatches(int MutualSettlementID)
        {
            ClientsDispatchesBS.Filter = "MutualSettlementID=" + MutualSettlementID;
        }

        public void FilterClientsIncomes(int MutualSettlementID)
        {
            ClientsIncomesBS.Filter = "MutualSettlementID=" + MutualSettlementID;
        }

        public void SaveClientsDispatches()
        {
            UpdateClientsDispatchesDA.Update(ClientsDispatchesDT);
        }

        public void SaveClientsIncomes()
        {
            UpdateClientsIncomesDA.Update(ClientsIncomesDT);
        }

        public void SaveMutualSettlements()
        {
            DataRow[] rows = MutualSettlementsDT.Select("MutualSettlementID IS NULL");
            for (int i = 0; i < rows.Count(); i++)
            {
                if (Convert.ToInt32(rows[i]["DiscountPaymentConditionID"]) == 1)
                    rows[i]["DelayOfPayment"] = OrdersManager.GetDelayOfPayment(Convert.ToInt32(rows[i]["ClientID"]));
                if (Convert.ToInt32(rows[i]["DiscountPaymentConditionID"]) == 2)
                    rows[i]["DelayOfPayment"] = 0;
                if (Convert.ToInt32(rows[i]["DiscountPaymentConditionID"]) == 3)
                    rows[i]["DelayOfPayment"] = 0;
            }
            DataTable dt = MutualSettlementsDT.Copy();
            dt.Columns.Remove("OrderNumbers");
            dt.Columns.Remove("CompareBalance");
            UpdateMutualSettlementsDA.Update(dt);
            dt.Dispose();
        }

        public void SaveMutualSettlementOrders()
        {
            DateTime CurrentDate = Security.GetCurrentDate();
            for (int i = MutualSettlementOrdersDT.Rows.Count - 1; i >= 0; i--)
            {
                if (MutualSettlementOrdersDT.Rows[i].RowState == DataRowState.Deleted || MutualSettlementOrdersDT.Rows[i]["MutualSettlementID"] == DBNull.Value)
                    continue;
                int MutualSettlementID = Convert.ToInt32(MutualSettlementOrdersDT.Rows[i]["MutualSettlementID"]);
                int OrderNumber = Convert.ToInt32(MutualSettlementOrdersDT.Rows[i]["OrderNumber"]);
                DataRow[] rows = MutualSettlementsDT.Select("MutualSettlementID=" + MutualSettlementID);
                for (int j = 0; j < rows.Count(); j++)
                {
                    if (rows[j]["OrderNumbers"] == DBNull.Value)
                        continue;
                    string OrderNumbers = rows[j]["OrderNumbers"].ToString();
                    int[] Numbers = ToIntArray(OrderNumbers, ',');
                    bool ToDeleteRow = true;
                    for (int x = 0; x < Numbers.Count(); x++)
                    {
                        int OrderNumber1 = Numbers[x];
                        if (OrderNumber == OrderNumber1)
                        {
                            ToDeleteRow = false;
                            break;
                        }
                    }
                    if (ToDeleteRow)
                        MutualSettlementOrdersDT.Rows[i].Delete();
                }
            }
            for (int i = 0; i < MutualSettlementsDT.Rows.Count; i++)
            {
                if (MutualSettlementsDT.Rows[i].RowState == DataRowState.Deleted || MutualSettlementsDT.Rows[i]["MutualSettlementID"] == DBNull.Value || MutualSettlementsDT.Rows[i]["OrderNumbers"] == DBNull.Value || MutualSettlementsDT.Rows[i]["OrderNumbers"].ToString().Length == 0)
                    continue;
                int ClientID = Convert.ToInt32(MutualSettlementsDT.Rows[i]["ClientID"]);
                int MutualSettlementID = Convert.ToInt32(MutualSettlementsDT.Rows[i]["MutualSettlementID"]);
                string OrderNumbers = MutualSettlementsDT.Rows[i]["OrderNumbers"].ToString();
                int[] Numbers = ToIntArray(OrderNumbers, ',');
                for (int j = 0; j < Numbers.Count(); j++)
                {
                    int OrderNumber = Numbers[j];
                    DataRow[] rows = MutualSettlementOrdersDT.Select("MutualSettlementID=" + MutualSettlementID + " AND OrderNumber=" + OrderNumber);
                    if (rows.Count() == 0)
                        AddMutualSettlementOrder(CurrentDate, ClientID, MutualSettlementID, OrderNumber);
                }
            }
            UpdateMutualSettlementOrdersDA.Update(MutualSettlementOrdersDT);
        }

        public bool AttachIncomeDocument(string FileName, string Extension, string Path, DocumentTypes DocumentType, int MutualSettlementID)
        {
            bool Ok = true;

            //write to ftp
            try
            {
                string sDestFolder = Configs.DocumentsPath + FileManager.GetPath("MutualSettlementDocuments");
                string sExtension = Extension;
                string sFileName = FileName;

                int j = 1;
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
                Ok = false;
                return false;
            }
            //write to database
            int MutualSettlementDocumentID = -1;
            string SelectCommand = @"SELECT TOP 1 * FROM MutualSettlementDocuments ORDER BY MutualSettlementDocumentID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        FileInfo fi;

                        try
                        {
                            fi = new FileInfo(Path);
                        }
                        catch
                        {
                            Ok = false;
                            return false;
                        }

                        DataRow NewRow = DT.NewRow();
                        NewRow["DocumentType"] = Convert.ToInt32(DocumentType);
                        NewRow["DocumentID"] = MutualSettlementID;
                        NewRow["FileName"] = FileName;
                        NewRow["FileSize"] = fi.Length;
                        DT.Rows.Add(NewRow);

                        DA.Update(DT);
                        DT.Clear();
                        if (DA.Fill(DT) > 0)
                            MutualSettlementDocumentID = Convert.ToInt32(DT.Rows[0]["MutualSettlementDocumentID"]);
                    }
                }
            }
            //write DocumentID to MutualSettlements
            if (MutualSettlementDocumentID != -1)
            {
                SelectCommand = @"SELECT * FROM MutualSettlements WHERE MutualSettlementID=" + MutualSettlementID;
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                if (DocumentType == DocumentTypes.InvoiceExcel)
                                {
                                    DT.Rows[0]["InvoiceExcel"] = MutualSettlementDocumentID;
                                    DT.Rows[0]["InvoiceExcelName"] = FileName;
                                }
                                if (DocumentType == DocumentTypes.InvoiceDbf)
                                {
                                    DT.Rows[0]["InvoiceDbf"] = MutualSettlementDocumentID;
                                    DT.Rows[0]["InvoiceDbfName"] = FileName;
                                }
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }

            return Ok;
        }

        public bool AttachDispatchDocument(string FileName, string Extension, string Path, DocumentTypes DocumentType, int ClientDispatchID)
        {
            bool Ok = true;

            //write to ftp
            try
            {
                string sDestFolder = Configs.DocumentsPath + FileManager.GetPath("MutualSettlementDocuments");
                string sExtension = Extension;
                string sFileName = FileName;

                int j = 1;
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
                Ok = false;
                return false;
            }
            //write to database
            int MutualSettlementDocumentID = -1;
            string SelectCommand = @"SELECT TOP 1 * FROM MutualSettlementDocuments ORDER BY MutualSettlementDocumentID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        FileInfo fi;

                        try
                        {
                            fi = new FileInfo(Path);
                        }
                        catch
                        {
                            Ok = false;
                            return false;
                        }

                        DataRow NewRow = DT.NewRow();
                        NewRow["DocumentType"] = Convert.ToInt32(DocumentType);
                        NewRow["DocumentID"] = ClientDispatchID;
                        NewRow["FileName"] = FileName;
                        NewRow["FileSize"] = fi.Length;
                        DT.Rows.Add(NewRow);

                        DA.Update(DT);
                        DT.Clear();
                        if (DA.Fill(DT) > 0)
                            MutualSettlementDocumentID = Convert.ToInt32(DT.Rows[0]["MutualSettlementDocumentID"]);
                    }
                }
            }
            //write DocumentID to MutualSettlements
            if (MutualSettlementDocumentID != -1)
            {
                SelectCommand = @"SELECT * FROM ClientsDispatches WHERE ClientDispatchID=" + ClientDispatchID;
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            if (DA.Fill(DT) > 0)
                            {
                                if (DocumentType == DocumentTypes.DispatchExcel)
                                {
                                    DT.Rows[0]["DispatchExcel"] = MutualSettlementDocumentID;
                                    DT.Rows[0]["DispatchExcelName"] = FileName;
                                }
                                if (DocumentType == DocumentTypes.DispatchDbf)
                                {
                                    DT.Rows[0]["DispatchDbf"] = MutualSettlementDocumentID;
                                    DT.Rows[0]["DispatchDbfName"] = FileName;
                                }
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }

            return Ok;
        }

        public bool AddInvoiceDocumentToDeleteTable(DocumentTypes DocumentType, int DocumentID)
        {
            DataRow[] rows = MutualSettlementsDT.Select("MutualSettlementID=" + DocumentID);
            if (rows.Count() > 0)
            {
                if (DocumentType == DocumentTypes.InvoiceExcel)
                {
                    DataRow NewRow = DocumentsToDeleteDT.NewRow();
                    NewRow["DocumentType"] = Convert.ToInt32(DocumentType);
                    NewRow["DocumentID"] = DocumentID;
                    DocumentsToDeleteDT.Rows.Add(NewRow);
                    rows[0]["InvoiceExcel"] = DBNull.Value;
                    rows[0]["InvoiceExcelName"] = DBNull.Value;
                    return true;
                }
                if (DocumentType == DocumentTypes.InvoiceDbf)
                {
                    DataRow NewRow = DocumentsToDeleteDT.NewRow();
                    NewRow["DocumentType"] = Convert.ToInt32(DocumentType);
                    NewRow["DocumentID"] = DocumentID;
                    DocumentsToDeleteDT.Rows.Add(NewRow);
                    rows[0]["InvoiceDbf"] = DBNull.Value;
                    rows[0]["InvoiceDbfName"] = DBNull.Value;
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool AddDispatchDocumentToDeleteTable(DocumentTypes DocumentType, int DocumentID)
        {
            DataRow[] rows = ClientsDispatchesDT.Select("ClientDispatchID=" + DocumentID);
            if (rows.Count() > 0)
            {
                if (DocumentType == DocumentTypes.DispatchExcel)
                {
                    DataRow NewRow = DocumentsToDeleteDT.NewRow();
                    NewRow["DocumentType"] = Convert.ToInt32(DocumentType);
                    NewRow["DocumentID"] = DocumentID;
                    DocumentsToDeleteDT.Rows.Add(NewRow);
                    rows[0]["DispatchExcel"] = DBNull.Value;
                    rows[0]["DispatchExcelName"] = DBNull.Value;
                    return true;
                }
                if (DocumentType == DocumentTypes.DispatchDbf)
                {
                    DataRow NewRow = DocumentsToDeleteDT.NewRow();
                    NewRow["DocumentType"] = Convert.ToInt32(DocumentType);
                    NewRow["DocumentID"] = DocumentID;
                    DocumentsToDeleteDT.Rows.Add(NewRow);
                    rows[0]["DispatchDbf"] = DBNull.Value;
                    rows[0]["DispatchDbfName"] = DBNull.Value;
                    return true;
                }
                return false;
            }
            return false;
        }

        public void AddDocumentsToDeleteTable(int MutualSettlementID)
        {
            string SelectCommand = @"SELECT * FROM MutualSettlementDocuments WHERE (DocumentType IN (3,4) AND DocumentID IN 
                (SELECT ClientDispatchID FROM ClientsDispatches WHERE MutualSettlementID=" + MutualSettlementID + ")) OR (DocumentType IN (1,2) AND DocumentID=" + MutualSettlementID + ")";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    for (int i = DT.Rows.Count - 1; i >= 0; i--)
                    {
                        DocumentTypes DocumentType = (DocumentTypes)DT.Rows[i]["DocumentType"];
                        int DocumentID = Convert.ToInt32(DT.Rows[i]["DocumentID"]);
                        if (DocumentType == DocumentTypes.InvoiceExcel || DocumentType == DocumentTypes.InvoiceDbf)
                            AddInvoiceDocumentToDeleteTable(DocumentType, DocumentID);
                        if (DocumentType == DocumentTypes.DispatchExcel || DocumentType == DocumentTypes.DispatchDbf)
                            AddDispatchDocumentToDeleteTable(DocumentType, DocumentID);
                    }
                }
            }
        }

        public bool DetachInocomeDocuments()
        {
            bool bOk = false;
            for (int x = DocumentsToDeleteDT.Rows.Count - 1; x >= 0; x--)
            {
                DocumentTypes DocumentType = (DocumentTypes)DocumentsToDeleteDT.Rows[x]["DocumentType"];
                if (DocumentType != DocumentTypes.InvoiceExcel && DocumentType != DocumentTypes.InvoiceDbf)
                    continue;
                int DocumentID = Convert.ToInt32(DocumentsToDeleteDT.Rows[x]["DocumentID"]);
                string SelectCommand = @"SELECT * FROM MutualSettlementDocuments WHERE DocumentType=" + Convert.ToInt32(DocumentType) + " AND DocumentID = " + DocumentID;
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);
                            for (int i = DT.Rows.Count - 1; i >= 0; i--)
                            {
                                bOk = FM.DeleteFile(Configs.DocumentsPath + FileManager.GetPath("MutualSettlementDocuments") + "/" + DT.Rows[i]["FileName"].ToString(), Configs.FTPType);
                                if (bOk)
                                {
                                    int MutualSettlementID = Convert.ToInt32(DT.Rows[i]["DocumentID"]);
                                    SelectCommand = @"SELECT * FROM MutualSettlements WHERE MutualSettlementID=" + MutualSettlementID;
                                    using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                                    {
                                        using (SqlCommandBuilder CB1 = new SqlCommandBuilder(DA1))
                                        {
                                            using (DataTable DT1 = new DataTable())
                                            {
                                                if (DA1.Fill(DT1) > 0)
                                                {
                                                    if (DocumentType == DocumentTypes.InvoiceExcel)
                                                    {
                                                        DT1.Rows[0]["InvoiceExcel"] = DBNull.Value;
                                                        DT1.Rows[0]["InvoiceExcelName"] = DBNull.Value;
                                                    }
                                                    if (DocumentType == DocumentTypes.InvoiceDbf)
                                                    {
                                                        DT1.Rows[0]["InvoiceDbf"] = DBNull.Value;
                                                        DT1.Rows[0]["InvoiceDbfName"] = DBNull.Value;
                                                    }
                                                    DA1.Update(DT1);
                                                }
                                            }
                                        }
                                    }
                                }
                                DT.Rows[i].Delete();
                            }
                            DA.Update(DT);
                        }
                    }
                }
                DocumentsToDeleteDT.Rows[x].Delete();
            }
            return bOk;
        }

        public bool DetachDispatchDocuments()
        {
            bool bOk = false;
            for (int x = DocumentsToDeleteDT.Rows.Count - 1; x >= 0; x--)
            {
                DocumentTypes DocumentType = (DocumentTypes)DocumentsToDeleteDT.Rows[x]["DocumentType"];
                if (DocumentType != DocumentTypes.DispatchExcel && DocumentType != DocumentTypes.DispatchDbf)
                    continue;
                int DocumentID = Convert.ToInt32(DocumentsToDeleteDT.Rows[x]["DocumentID"]);
                string SelectCommand = @"SELECT * FROM MutualSettlementDocuments WHERE DocumentType=" + Convert.ToInt32(DocumentType) + " AND DocumentID = " + DocumentID;
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);
                            for (int i = DT.Rows.Count - 1; i >= 0; i--)
                            {
                                bOk = FM.DeleteFile(Configs.DocumentsPath + FileManager.GetPath("MutualSettlementDocuments") + "/" + DT.Rows[i]["FileName"].ToString(), Configs.FTPType);
                                if (bOk)
                                {
                                    int ClientDispatchID = Convert.ToInt32(DT.Rows[i]["DocumentID"]);
                                    SelectCommand = @"SELECT * FROM ClientsDispatches WHERE ClientDispatchID=" + ClientDispatchID;
                                    using (SqlDataAdapter DA1 = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                                    {
                                        using (SqlCommandBuilder CB1 = new SqlCommandBuilder(DA1))
                                        {
                                            using (DataTable DT1 = new DataTable())
                                            {
                                                if (DA1.Fill(DT1) > 0)
                                                {
                                                    if (DocumentType == DocumentTypes.DispatchExcel)
                                                    {
                                                        DT1.Rows[0]["DispatchExcel"] = DBNull.Value;
                                                        DT1.Rows[0]["DispatchExcelName"] = DBNull.Value;
                                                    }
                                                    if (DocumentType == DocumentTypes.DispatchDbf)
                                                    {
                                                        DT1.Rows[0]["DispatchDbf"] = DBNull.Value;
                                                        DT1.Rows[0]["DispatchDbfName"] = DBNull.Value;
                                                    }
                                                    DA1.Update(DT1);
                                                }
                                            }
                                        }
                                    }
                                }
                                DT.Rows[i].Delete();
                            }
                            DA.Update(DT);
                        }
                    }
                }
                DocumentsToDeleteDT.Rows[x].Delete();
            }
            return bOk;
        }

        public string OpenMutualSettlementDocument(int MutualSettlementDocumentID)
        {
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");

            string FileName = "";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MutualSettlementDocuments WHERE MutualSettlementDocumentID = " + MutualSettlementDocumentID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        FileName = DT.Rows[0]["FileName"].ToString();

                    try
                    {
                        FM.DownloadFile(Configs.DocumentsPath + FileManager.GetPath("MutualSettlementDocuments") + "/" + FileName,
                            tempFolder + "\\" + FileName, Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType);
                    }
                    catch
                    {
                        return null;
                    }

                }
            }

            return tempFolder + "\\" + FileName;
        }

        public void SaveMutualSettlementDocument(int MutualSettlementDocumentID, string sDistFileName)
        {
            string FileName = "";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM MutualSettlementDocuments WHERE MutualSettlementDocumentID = " + MutualSettlementDocumentID,
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    FileName = DT.Rows[0]["FileName"].ToString();

                    try
                    {
                        FM.DownloadFile(Configs.DocumentsPath + FileManager.GetPath("MutualSettlementDocuments") + "/" + FileName,
                            sDistFileName, Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType);
                    }
                    catch
                    {
                        return;
                    }

                }
            }
        }

        #region отчет по ДС
        public DataTable BYRReportCash(int FactoryID, DateTime Date1, DateTime Date2, ref decimal TotalOpeningBalance, ref decimal TotalDispatchSum, ref decimal TotalIncomeSum, ref decimal TotalClosingBalance)
        {
            CashReportDT.Clear();
            DataTable DispDT = new DataTable();
            DataTable IncomeDT = new DataTable();
            DataTable Table = new DataTable();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID FROM ClientsDispatches 
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID + @"
                WHERE CAST (DispatchDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND ClientsDispatches.CurrencyTypeID=5 AND CAST (DispatchDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' ORDER BY ClientsDispatches.MutualSettlementID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispDT);
            }

            SelectCommand = @"SELECT ClientsIncomes.*, MutualSettlements.ClientID FROM ClientsIncomes 
                INNER JOIN MutualSettlements ON ClientsIncomes.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID + @"
                WHERE CAST (IncomeDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND ClientsIncomes.CurrencyTypeID=5 AND CAST (IncomeDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' ORDER BY ClientsIncomes.MutualSettlementID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(IncomeDT);
            }

            for (int j = 0; j < ClientsDT.Rows.Count; j++)
            {
                int ClientID = Convert.ToInt32(ClientsDT.Rows[j]["ClientID"]);
                decimal OpeningBalance = 0;
                decimal ClosingBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                decimal DispatchSum = 0;
                decimal IncomeSum = 0;

                DataRow[] drows = DispDT.Select("ClientID=" + ClientID);
                for (int i = 0; i < drows.Count(); i++)
                    DispatchSum += Convert.ToDecimal(drows[i]["DispatchSum"]);
                DataRow[] irows = IncomeDT.Select("ClientID=" + ClientID);
                for (int i = 0; i < irows.Count(); i++)
                    IncomeSum += Convert.ToDecimal(irows[i]["IncomeSum"]);

                int CurrencyTypeID = 0;
                int MinMutualSettlementID = 0;

                if (drows.Count() > 0)
                {
                    CurrencyTypeID = Convert.ToInt32(drows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(drows[0]["MutualSettlementID"]);
                }
                if (irows.Count() > 0 && MinMutualSettlementID == 0)
                {
                    CurrencyTypeID = Convert.ToInt32(irows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(irows[0]["MutualSettlementID"]);
                }
                SelectCommand = @"SELECT TOP 1 MutualSettlements.*, infiniu2_marketingreference.dbo.Clients.ClientName FROM MutualSettlements
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE MutualSettlements.ClientID=" + ClientID + " AND MutualSettlements.FactoryID=" + FactoryID + " AND CurrencyTypeID=5 ORDER BY MutualSettlementID DESC";
                using (DataTable DT = new DataTable())
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        if (DA.Fill(DT) > 0)
                            ClosingBalance = Convert.ToDecimal(DT.Rows[0]["ClosingBalance"]);
                    }
                }
                if (OpeningBalance == 0 && ClosingBalance == 0 && DispatchSum == 0 && IncomeSum == 0)
                    continue;
                OpeningBalance = ClosingBalance - DispatchSum + IncomeSum;
                DataRow NewRow = CashReportDT.NewRow();
                NewRow["CurrencyType"] = GetCurrencyType(4);
                NewRow["ClientName"] = GetClientName(ClientID);
                NewRow["OpeningBalance"] = OpeningBalance;
                NewRow["ClosingBalance"] = ClosingBalance;
                NewRow["TotalDispatchSum"] = DispatchSum;
                NewRow["TotalIncomeSum"] = IncomeSum;
                CashReportDT.Rows.Add(NewRow);
            }
            TotalOpeningBalance = 0;
            TotalDispatchSum = 0;
            TotalIncomeSum = 0;
            TotalClosingBalance = 0;
            for (int j = 0; j < CashReportDT.Rows.Count; j++)
            {
                TotalOpeningBalance += Convert.ToDecimal(CashReportDT.Rows[j]["OpeningBalance"]);
                TotalDispatchSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalDispatchSum"]);
                TotalIncomeSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalIncomeSum"]);
                TotalClosingBalance += Convert.ToDecimal(CashReportDT.Rows[j]["ClosingBalance"]);
            }
            return CashReportDT.Copy();
        }

        public DataTable RUBReportCash(int FactoryID, DateTime Date1, DateTime Date2, ref decimal TotalOpeningBalance, ref decimal TotalDispatchSum, ref decimal TotalIncomeSum, ref decimal TotalClosingBalance)
        {
            CashReportDT.Clear();
            DataTable DispDT = new DataTable();
            DataTable IncomeDT = new DataTable();
            DataTable Table = new DataTable();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID FROM ClientsDispatches 
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID + @"
                WHERE CAST (DispatchDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND ClientsDispatches.CurrencyTypeID=3 AND CAST (DispatchDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' ORDER BY ClientsDispatches.MutualSettlementID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispDT);
            }

            SelectCommand = @"SELECT ClientsIncomes.*, MutualSettlements.ClientID FROM ClientsIncomes 
                INNER JOIN MutualSettlements ON ClientsIncomes.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID + @"
                WHERE CAST (IncomeDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND ClientsIncomes.CurrencyTypeID=3 AND CAST (IncomeDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' ORDER BY ClientsIncomes.MutualSettlementID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(IncomeDT);
            }

            for (int j = 0; j < ClientsDT.Rows.Count; j++)
            {
                int ClientID = Convert.ToInt32(ClientsDT.Rows[j]["ClientID"]);
                decimal OpeningBalance = 0;
                decimal ClosingBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                decimal DispatchSum = 0;
                decimal IncomeSum = 0;

                DataRow[] drows = DispDT.Select("ClientID=" + ClientID);
                for (int i = 0; i < drows.Count(); i++)
                    DispatchSum += Convert.ToDecimal(drows[i]["DispatchSum"]);
                DataRow[] irows = IncomeDT.Select("ClientID=" + ClientID);
                for (int i = 0; i < irows.Count(); i++)
                    IncomeSum += Convert.ToDecimal(irows[i]["IncomeSum"]);

                int CurrencyTypeID = 0;
                int MinMutualSettlementID = 0;

                if (drows.Count() > 0)
                {
                    CurrencyTypeID = Convert.ToInt32(drows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(drows[0]["MutualSettlementID"]);
                }
                if (irows.Count() > 0 && MinMutualSettlementID == 0)
                {
                    CurrencyTypeID = Convert.ToInt32(irows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(irows[0]["MutualSettlementID"]);
                }
                SelectCommand = @"SELECT TOP 1 MutualSettlements.*, infiniu2_marketingreference.dbo.Clients.ClientName FROM MutualSettlements
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE MutualSettlements.ClientID=" + ClientID + " AND MutualSettlements.FactoryID=" + FactoryID + " AND CurrencyTypeID=3 ORDER BY MutualSettlementID DESC";
                using (DataTable DT = new DataTable())
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        if (DA.Fill(DT) > 0)
                            ClosingBalance = Convert.ToDecimal(DT.Rows[0]["ClosingBalance"]);
                    }
                }
                if (OpeningBalance == 0 && ClosingBalance == 0 && DispatchSum == 0 && IncomeSum == 0)
                    continue;
                OpeningBalance = ClosingBalance - DispatchSum + IncomeSum;
                DataRow NewRow = CashReportDT.NewRow();
                NewRow["CurrencyType"] = GetCurrencyType(3);
                NewRow["ClientName"] = GetClientName(ClientID);
                NewRow["OpeningBalance"] = OpeningBalance;
                NewRow["ClosingBalance"] = ClosingBalance;
                NewRow["TotalDispatchSum"] = DispatchSum;
                NewRow["TotalIncomeSum"] = IncomeSum;
                CashReportDT.Rows.Add(NewRow);
            }
            TotalOpeningBalance = 0;
            TotalDispatchSum = 0;
            TotalIncomeSum = 0;
            TotalClosingBalance = 0;
            for (int j = 0; j < CashReportDT.Rows.Count; j++)
            {
                TotalOpeningBalance += Convert.ToDecimal(CashReportDT.Rows[j]["OpeningBalance"]);
                TotalDispatchSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalDispatchSum"]);
                TotalIncomeSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalIncomeSum"]);
                TotalClosingBalance += Convert.ToDecimal(CashReportDT.Rows[j]["ClosingBalance"]);
            }
            return CashReportDT.Copy();
        }

        public DataTable EURReportCash(int FactoryID, DateTime Date1, DateTime Date2, ref decimal TotalOpeningBalance, ref decimal TotalDispatchSum, ref decimal TotalIncomeSum, ref decimal TotalClosingBalance)
        {
            CashReportDT.Clear();
            DataTable DispDT = new DataTable();
            DataTable IncomeDT = new DataTable();
            DataTable Table = new DataTable();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID FROM ClientsDispatches 
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID + @"
                WHERE CAST (DispatchDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND ClientsDispatches.CurrencyTypeID=1 AND CAST (DispatchDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' ORDER BY ClientsDispatches.MutualSettlementID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispDT);
            }

            SelectCommand = @"SELECT ClientsIncomes.*, MutualSettlements.ClientID FROM ClientsIncomes 
                INNER JOIN MutualSettlements ON ClientsIncomes.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID + @"
                WHERE CAST (IncomeDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND ClientsIncomes.CurrencyTypeID=1 AND CAST (IncomeDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' ORDER BY ClientsIncomes.MutualSettlementID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(IncomeDT);
            }

            for (int j = 0; j < ClientsDT.Rows.Count; j++)
            {
                int ClientID = Convert.ToInt32(ClientsDT.Rows[j]["ClientID"]);
                decimal OpeningBalance = 0;
                decimal ClosingBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                decimal DispatchSum = 0;
                decimal IncomeSum = 0;

                DataRow[] drows = DispDT.Select("ClientID=" + ClientID);
                for (int i = 0; i < drows.Count(); i++)
                    DispatchSum += Convert.ToDecimal(drows[i]["DispatchSum"]);
                DataRow[] irows = IncomeDT.Select("ClientID=" + ClientID);
                for (int i = 0; i < irows.Count(); i++)
                    IncomeSum += Convert.ToDecimal(irows[i]["IncomeSum"]);

                int CurrencyTypeID = 0;
                int MinMutualSettlementID = 0;

                if (drows.Count() > 0)
                {
                    CurrencyTypeID = Convert.ToInt32(drows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(drows[0]["MutualSettlementID"]);
                }
                if (irows.Count() > 0 && MinMutualSettlementID == 0)
                {
                    CurrencyTypeID = Convert.ToInt32(irows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(irows[0]["MutualSettlementID"]);
                }
                SelectCommand = @"SELECT TOP 1 MutualSettlements.*, infiniu2_marketingreference.dbo.Clients.ClientName FROM MutualSettlements
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE MutualSettlements.ClientID=" + ClientID + " AND MutualSettlements.FactoryID=" + FactoryID + " AND CurrencyTypeID=1 ORDER BY MutualSettlementID DESC";
                using (DataTable DT = new DataTable())
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        if (DA.Fill(DT) > 0)
                            ClosingBalance = Convert.ToDecimal(DT.Rows[0]["ClosingBalance"]);
                    }
                }
                if (OpeningBalance == 0 && ClosingBalance == 0 && DispatchSum == 0 && IncomeSum == 0)
                    continue;
                OpeningBalance = ClosingBalance - DispatchSum + IncomeSum;
                DataRow NewRow = CashReportDT.NewRow();
                NewRow["CurrencyType"] = GetCurrencyType(1);
                NewRow["ClientName"] = GetClientName(ClientID);
                NewRow["OpeningBalance"] = OpeningBalance;
                NewRow["ClosingBalance"] = ClosingBalance;
                NewRow["TotalDispatchSum"] = DispatchSum;
                NewRow["TotalIncomeSum"] = IncomeSum;
                CashReportDT.Rows.Add(NewRow);
            }
            TotalOpeningBalance = 0;
            TotalDispatchSum = 0;
            TotalIncomeSum = 0;
            TotalClosingBalance = 0;
            for (int j = 0; j < CashReportDT.Rows.Count; j++)
            {
                TotalOpeningBalance += Convert.ToDecimal(CashReportDT.Rows[j]["OpeningBalance"]);
                TotalDispatchSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalDispatchSum"]);
                TotalIncomeSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalIncomeSum"]);
                TotalClosingBalance += Convert.ToDecimal(CashReportDT.Rows[j]["ClosingBalance"]);
            }
            return CashReportDT.Copy();
        }

        public DataTable USDReportCash(int FactoryID, DateTime Date1, DateTime Date2, ref decimal TotalOpeningBalance, ref decimal TotalDispatchSum, ref decimal TotalIncomeSum, ref decimal TotalClosingBalance)
        {
            CashReportDT.Clear();
            DataTable DispDT = new DataTable();
            DataTable IncomeDT = new DataTable();
            DataTable Table = new DataTable();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID FROM ClientsDispatches 
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID + @"
                WHERE CAST (DispatchDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND ClientsDispatches.CurrencyTypeID=2 AND CAST (DispatchDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' ORDER BY ClientsDispatches.MutualSettlementID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispDT);
            }

            SelectCommand = @"SELECT ClientsIncomes.*, MutualSettlements.ClientID FROM ClientsIncomes 
                INNER JOIN MutualSettlements ON ClientsIncomes.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID + @"
                WHERE CAST (IncomeDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND ClientsIncomes.CurrencyTypeID=2 AND CAST (IncomeDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' ORDER BY ClientsIncomes.MutualSettlementID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(IncomeDT);
            }

            for (int j = 0; j < ClientsDT.Rows.Count; j++)
            {
                int ClientID = Convert.ToInt32(ClientsDT.Rows[j]["ClientID"]);
                decimal OpeningBalance = 0;
                decimal ClosingBalance = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                decimal DispatchSum = 0;
                decimal IncomeSum = 0;

                DataRow[] drows = DispDT.Select("ClientID=" + ClientID);
                for (int i = 0; i < drows.Count(); i++)
                    DispatchSum += Convert.ToDecimal(drows[i]["DispatchSum"]);
                DataRow[] irows = IncomeDT.Select("ClientID=" + ClientID);
                for (int i = 0; i < irows.Count(); i++)
                    IncomeSum += Convert.ToDecimal(irows[i]["IncomeSum"]);

                int CurrencyTypeID = 0;
                int MinMutualSettlementID = 0;

                if (drows.Count() > 0)
                {
                    CurrencyTypeID = Convert.ToInt32(drows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(drows[0]["MutualSettlementID"]);
                }
                if (irows.Count() > 0 && MinMutualSettlementID == 0)
                {
                    CurrencyTypeID = Convert.ToInt32(irows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(irows[0]["MutualSettlementID"]);
                }
                SelectCommand = @"SELECT TOP 1 MutualSettlements.*, infiniu2_marketingreference.dbo.Clients.ClientName FROM MutualSettlements
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE MutualSettlements.ClientID=" + ClientID + " AND MutualSettlements.FactoryID=" + FactoryID + " AND CurrencyTypeID=2 ORDER BY MutualSettlementID DESC";
                using (DataTable DT = new DataTable())
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        if (DA.Fill(DT) > 0)
                            ClosingBalance = Convert.ToDecimal(DT.Rows[0]["ClosingBalance"]);
                    }
                }
                if (OpeningBalance == 0 && ClosingBalance == 0 && DispatchSum == 0 && IncomeSum == 0)
                    continue;
                OpeningBalance = ClosingBalance - DispatchSum + IncomeSum;
                DataRow NewRow = CashReportDT.NewRow();
                NewRow["CurrencyType"] = GetCurrencyType(2);
                NewRow["ClientName"] = GetClientName(ClientID);
                NewRow["OpeningBalance"] = OpeningBalance;
                NewRow["ClosingBalance"] = ClosingBalance;
                NewRow["TotalDispatchSum"] = DispatchSum;
                NewRow["TotalIncomeSum"] = IncomeSum;
                CashReportDT.Rows.Add(NewRow);
            }
            TotalOpeningBalance = 0;
            TotalDispatchSum = 0;
            TotalIncomeSum = 0;
            TotalClosingBalance = 0;
            for (int j = 0; j < CashReportDT.Rows.Count; j++)
            {
                TotalOpeningBalance += Convert.ToDecimal(CashReportDT.Rows[j]["OpeningBalance"]);
                TotalDispatchSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalDispatchSum"]);
                TotalIncomeSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalIncomeSum"]);
                TotalClosingBalance += Convert.ToDecimal(CashReportDT.Rows[j]["ClosingBalance"]);
            }
            return CashReportDT.Copy();
        }

        public DataTable BYRReportCash(int FactoryID, ArrayList Clients, DateTime Date1, DateTime Date2, ref decimal TotalOpeningBalance, ref decimal TotalDispatchSum, ref decimal TotalIncomeSum, ref decimal TotalClosingBalance)
        {
            CashReportDT.Clear();
            for (int j = 0; j < Clients.Count; j++)
            {
                int ClientID = Convert.ToInt32(Clients[j]);
                DataTable DispDT = new DataTable();
                DataTable IncomeDT = new DataTable();
                decimal OpeningBalance = 0;
                decimal ClosingBalance = 0;
                decimal DispatchSum = 0;
                decimal IncomeSum = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                string SelectCommand = @"SELECT ClientsDispatches.* FROM ClientsDispatches 
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.ClientID=" + ClientID + @" AND MutualSettlements.FactoryID=" + FactoryID + @"
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE CAST (DispatchDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND CAST (DispatchDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' AND ClientsDispatches.CurrencyTypeID=5 ORDER BY ClientsDispatches.MutualSettlementID";
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DispDT);
                }
                SelectCommand = @"SELECT ClientsIncomes.* FROM ClientsIncomes 
                INNER JOIN MutualSettlements ON ClientsIncomes.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.ClientID=" + ClientID + @" AND MutualSettlements.FactoryID=" + FactoryID + @"
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE CAST (IncomeDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND CAST (IncomeDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' AND ClientsIncomes.CurrencyTypeID=5 ORDER BY ClientsIncomes.MutualSettlementID";
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(IncomeDT);
                }
                for (int i = 0; i < DispDT.Rows.Count; i++)
                    DispatchSum += Convert.ToDecimal(DispDT.Rows[i]["DispatchSum"]);
                for (int i = 0; i < IncomeDT.Rows.Count; i++)
                    IncomeSum += Convert.ToDecimal(IncomeDT.Rows[i]["IncomeSum"]);
                int CurrencyTypeID = 0;
                int MinMutualSettlementID = 0;
                if (DispDT.Rows.Count > 0)
                {
                    CurrencyTypeID = Convert.ToInt32(DispDT.Rows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(DispDT.Rows[0]["MutualSettlementID"]);
                }
                if (IncomeDT.Rows.Count > 0 && Convert.ToInt32(IncomeDT.Rows[0]["MutualSettlementID"]) < MinMutualSettlementID)
                {
                    CurrencyTypeID = Convert.ToInt32(IncomeDT.Rows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(IncomeDT.Rows[0]["MutualSettlementID"]);
                }
                SelectCommand = @"SELECT TOP 1 MutualSettlements.*, infiniu2_marketingreference.dbo.Clients.ClientName FROM MutualSettlements
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE MutualSettlements.ClientID=" + ClientID + " AND MutualSettlements.FactoryID=" + FactoryID + " AND MutualSettlements.CurrencyTypeID=5 ORDER BY MutualSettlementID DESC";
                using (DataTable DT = new DataTable())
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        if (DA.Fill(DT) > 0)
                            ClosingBalance = Convert.ToDecimal(DT.Rows[0]["ClosingBalance"]);
                    }
                }
                if (OpeningBalance == 0 && ClosingBalance == 0 && DispatchSum == 0 && IncomeSum == 0)
                    continue;
                OpeningBalance = ClosingBalance - DispatchSum + IncomeSum;
                DataRow NewRow = CashReportDT.NewRow();
                NewRow["CurrencyType"] = GetCurrencyType(4);
                NewRow["ClientName"] = GetClientName(ClientID);
                NewRow["OpeningBalance"] = OpeningBalance;
                NewRow["ClosingBalance"] = ClosingBalance;
                NewRow["TotalDispatchSum"] = DispatchSum;
                NewRow["TotalIncomeSum"] = IncomeSum;
                CashReportDT.Rows.Add(NewRow);
            }

            TotalOpeningBalance = 0;
            TotalDispatchSum = 0;
            TotalIncomeSum = 0;
            TotalClosingBalance = 0;
            for (int j = 0; j < CashReportDT.Rows.Count; j++)
            {
                TotalOpeningBalance += Convert.ToDecimal(CashReportDT.Rows[j]["OpeningBalance"]);
                TotalDispatchSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalDispatchSum"]);
                TotalIncomeSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalIncomeSum"]);
                TotalClosingBalance += Convert.ToDecimal(CashReportDT.Rows[j]["ClosingBalance"]);
            }
            return CashReportDT.Copy();
        }

        public DataTable RUBReportCash(int FactoryID, ArrayList Clients, DateTime Date1, DateTime Date2, ref decimal TotalOpeningBalance, ref decimal TotalDispatchSum, ref decimal TotalIncomeSum, ref decimal TotalClosingBalance)
        {
            CashReportDT.Clear();
            for (int j = 0; j < Clients.Count; j++)
            {
                int ClientID = Convert.ToInt32(Clients[j]);
                DataTable DispDT = new DataTable();
                DataTable IncomeDT = new DataTable();
                decimal OpeningBalance = 0;
                decimal ClosingBalance = 0;
                decimal DispatchSum = 0;
                decimal IncomeSum = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                string SelectCommand = @"SELECT ClientsDispatches.* FROM ClientsDispatches 
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.ClientID=" + ClientID + @" AND MutualSettlements.FactoryID=" + FactoryID + @"
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE CAST (DispatchDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND CAST (DispatchDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' AND ClientsDispatches.CurrencyTypeID=3 ORDER BY ClientsDispatches.MutualSettlementID";
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DispDT);
                }
                SelectCommand = @"SELECT ClientsIncomes.* FROM ClientsIncomes 
                INNER JOIN MutualSettlements ON ClientsIncomes.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.ClientID=" + ClientID + @" AND MutualSettlements.FactoryID=" + FactoryID + @"
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE CAST (IncomeDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND CAST (IncomeDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' AND ClientsIncomes.CurrencyTypeID=3 ORDER BY ClientsIncomes.MutualSettlementID";
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(IncomeDT);
                }
                for (int i = 0; i < DispDT.Rows.Count; i++)
                    DispatchSum += Convert.ToDecimal(DispDT.Rows[i]["DispatchSum"]);
                for (int i = 0; i < IncomeDT.Rows.Count; i++)
                    IncomeSum += Convert.ToDecimal(IncomeDT.Rows[i]["IncomeSum"]);
                int CurrencyTypeID = 0;
                int MinMutualSettlementID = 0;
                if (DispDT.Rows.Count > 0)
                {
                    CurrencyTypeID = Convert.ToInt32(DispDT.Rows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(DispDT.Rows[0]["MutualSettlementID"]);
                }
                if (IncomeDT.Rows.Count > 0 && Convert.ToInt32(IncomeDT.Rows[0]["MutualSettlementID"]) < MinMutualSettlementID)
                {
                    CurrencyTypeID = Convert.ToInt32(IncomeDT.Rows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(IncomeDT.Rows[0]["MutualSettlementID"]);
                }
                SelectCommand = @"SELECT TOP 1 MutualSettlements.*, infiniu2_marketingreference.dbo.Clients.ClientName FROM MutualSettlements
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE MutualSettlements.ClientID=" + ClientID + " AND MutualSettlements.FactoryID=" + FactoryID + " AND MutualSettlements.CurrencyTypeID=3 ORDER BY MutualSettlementID DESC";
                using (DataTable DT = new DataTable())
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        if (DA.Fill(DT) > 0)
                            ClosingBalance = Convert.ToDecimal(DT.Rows[0]["ClosingBalance"]);
                    }
                }
                if (OpeningBalance == 0 && ClosingBalance == 0 && DispatchSum == 0 && IncomeSum == 0)
                    continue;
                OpeningBalance = ClosingBalance - DispatchSum + IncomeSum;
                DataRow NewRow = CashReportDT.NewRow();
                NewRow["CurrencyType"] = GetCurrencyType(3);
                NewRow["ClientName"] = GetClientName(ClientID);
                NewRow["OpeningBalance"] = OpeningBalance;
                NewRow["ClosingBalance"] = ClosingBalance;
                NewRow["TotalDispatchSum"] = DispatchSum;
                NewRow["TotalIncomeSum"] = IncomeSum;
                CashReportDT.Rows.Add(NewRow);
            }

            TotalOpeningBalance = 0;
            TotalDispatchSum = 0;
            TotalIncomeSum = 0;
            TotalClosingBalance = 0;
            for (int j = 0; j < CashReportDT.Rows.Count; j++)
            {
                TotalOpeningBalance += Convert.ToDecimal(CashReportDT.Rows[j]["OpeningBalance"]);
                TotalDispatchSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalDispatchSum"]);
                TotalIncomeSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalIncomeSum"]);
                TotalClosingBalance += Convert.ToDecimal(CashReportDT.Rows[j]["ClosingBalance"]);
            }
            return CashReportDT.Copy();
        }

        public DataTable EURReportCash(int FactoryID, ArrayList Clients, DateTime Date1, DateTime Date2, ref decimal TotalOpeningBalance, ref decimal TotalDispatchSum, ref decimal TotalIncomeSum, ref decimal TotalClosingBalance)
        {
            CashReportDT.Clear();
            for (int j = 0; j < Clients.Count; j++)
            {
                int ClientID = Convert.ToInt32(Clients[j]);
                DataTable DispDT = new DataTable();
                DataTable IncomeDT = new DataTable();
                decimal OpeningBalance = 0;
                decimal ClosingBalance = 0;
                decimal DispatchSum = 0;
                decimal IncomeSum = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;
                string SelectCommand = @"SELECT ClientsDispatches.* FROM ClientsDispatches 
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.ClientID=" + ClientID + @" AND MutualSettlements.FactoryID=" + FactoryID + @"
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE CAST (DispatchDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND CAST (DispatchDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' AND ClientsDispatches.CurrencyTypeID=1 ORDER BY ClientsDispatches.MutualSettlementID";
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DispDT);
                }
                SelectCommand = @"SELECT ClientsIncomes.* FROM ClientsIncomes 
                INNER JOIN MutualSettlements ON ClientsIncomes.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.ClientID=" + ClientID + @" AND MutualSettlements.FactoryID=" + FactoryID + @"
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE CAST (IncomeDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND CAST (IncomeDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' AND ClientsIncomes.CurrencyTypeID=1 ORDER BY ClientsIncomes.MutualSettlementID";
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(IncomeDT);
                }
                for (int i = 0; i < DispDT.Rows.Count; i++)
                    DispatchSum += Convert.ToDecimal(DispDT.Rows[i]["DispatchSum"]);
                for (int i = 0; i < IncomeDT.Rows.Count; i++)
                    IncomeSum += Convert.ToDecimal(IncomeDT.Rows[i]["IncomeSum"]);
                int CurrencyTypeID = 0;
                int MinMutualSettlementID = 0;
                if (DispDT.Rows.Count > 0)
                {
                    CurrencyTypeID = Convert.ToInt32(DispDT.Rows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(DispDT.Rows[0]["MutualSettlementID"]);
                }
                if (IncomeDT.Rows.Count > 0 && Convert.ToInt32(IncomeDT.Rows[0]["MutualSettlementID"]) < MinMutualSettlementID)
                {
                    CurrencyTypeID = Convert.ToInt32(IncomeDT.Rows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(IncomeDT.Rows[0]["MutualSettlementID"]);
                }
                SelectCommand = @"SELECT TOP 1 MutualSettlements.*, infiniu2_marketingreference.dbo.Clients.ClientName FROM MutualSettlements
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE MutualSettlements.ClientID=" + ClientID + " AND MutualSettlements.FactoryID=" + FactoryID + " AND MutualSettlements.CurrencyTypeID=1 ORDER BY MutualSettlementID DESC";
                using (DataTable DT = new DataTable())
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        if (DA.Fill(DT) > 0)
                            ClosingBalance = Convert.ToDecimal(DT.Rows[0]["ClosingBalance"]);
                    }
                }
                if (OpeningBalance == 0 && ClosingBalance == 0 && DispatchSum == 0 && IncomeSum == 0)
                    continue;
                OpeningBalance = ClosingBalance - DispatchSum + IncomeSum;
                DataRow NewRow = CashReportDT.NewRow();
                NewRow["CurrencyType"] = GetCurrencyType(1);
                NewRow["ClientName"] = GetClientName(ClientID);
                NewRow["OpeningBalance"] = OpeningBalance;
                NewRow["ClosingBalance"] = ClosingBalance;
                NewRow["TotalDispatchSum"] = DispatchSum;
                NewRow["TotalIncomeSum"] = IncomeSum;
                CashReportDT.Rows.Add(NewRow);
            }

            TotalOpeningBalance = 0;
            TotalDispatchSum = 0;
            TotalIncomeSum = 0;
            TotalClosingBalance = 0;
            for (int j = 0; j < CashReportDT.Rows.Count; j++)
            {
                TotalOpeningBalance += Convert.ToDecimal(CashReportDT.Rows[j]["OpeningBalance"]);
                TotalDispatchSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalDispatchSum"]);
                TotalIncomeSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalIncomeSum"]);
                TotalClosingBalance += Convert.ToDecimal(CashReportDT.Rows[j]["ClosingBalance"]);
            }
            return CashReportDT.Copy();
        }

        public DataTable USDReportCash(int FactoryID, ArrayList Clients, DateTime Date1, DateTime Date2, ref decimal TotalOpeningBalance, ref decimal TotalDispatchSum, ref decimal TotalIncomeSum, ref decimal TotalClosingBalance)
        {
            CashReportDT.Clear();
            for (int j = 0; j < Clients.Count; j++)
            {
                int ClientID = Convert.ToInt32(Clients[j]);
                DataTable DispDT = new DataTable();
                DataTable IncomeDT = new DataTable();
                decimal OpeningBalance = 0;
                decimal ClosingBalance = 0;
                decimal DispatchSum = 0;
                decimal IncomeSum = 0;
                TotalDispatchSum = 0;
                TotalIncomeSum = 0;

                string SelectCommand = @"SELECT ClientsDispatches.* FROM ClientsDispatches 
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.ClientID=" + ClientID + @" AND MutualSettlements.FactoryID=" + FactoryID + @"
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE CAST (DispatchDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND CAST (DispatchDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' AND ClientsDispatches.CurrencyTypeID=2 ORDER BY ClientsDispatches.MutualSettlementID";
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DispDT);
                }
                SelectCommand = @"SELECT ClientsIncomes.* FROM ClientsIncomes 
                INNER JOIN MutualSettlements ON ClientsIncomes.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.ClientID=" + ClientID + @" AND MutualSettlements.FactoryID=" + FactoryID + @"
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE CAST (IncomeDateTime AS date)>='" + Date1.ToString("yyyy-MM-dd") + "' AND CAST (IncomeDateTime AS date)<='" + Date2.ToString("yyyy-MM-dd") + "' AND ClientsIncomes.CurrencyTypeID=2 ORDER BY ClientsIncomes.MutualSettlementID";
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(IncomeDT);
                }
                for (int i = 0; i < DispDT.Rows.Count; i++)
                    DispatchSum += Convert.ToDecimal(DispDT.Rows[i]["DispatchSum"]);
                for (int i = 0; i < IncomeDT.Rows.Count; i++)
                    IncomeSum += Convert.ToDecimal(IncomeDT.Rows[i]["IncomeSum"]);
                int CurrencyTypeID = 0;
                int MinMutualSettlementID = 0;
                if (DispDT.Rows.Count > 0)
                {
                    CurrencyTypeID = Convert.ToInt32(DispDT.Rows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(DispDT.Rows[0]["MutualSettlementID"]);
                }
                if (IncomeDT.Rows.Count > 0 && Convert.ToInt32(IncomeDT.Rows[0]["MutualSettlementID"]) < MinMutualSettlementID)
                {
                    CurrencyTypeID = Convert.ToInt32(IncomeDT.Rows[0]["CurrencyTypeID"]);
                    MinMutualSettlementID = Convert.ToInt32(IncomeDT.Rows[0]["MutualSettlementID"]);
                }
                SelectCommand = @"SELECT TOP 1 MutualSettlements.*, infiniu2_marketingreference.dbo.Clients.ClientName FROM MutualSettlements
                INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE MutualSettlements.ClientID=" + ClientID + " AND MutualSettlements.FactoryID=" + FactoryID + " AND MutualSettlements.CurrencyTypeID=2 ORDER BY MutualSettlementID DESC";
                using (DataTable DT = new DataTable())
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                    {
                        if (DA.Fill(DT) > 0)
                            ClosingBalance = Convert.ToDecimal(DT.Rows[0]["ClosingBalance"]);
                    }
                }
                if (OpeningBalance == 0 && ClosingBalance == 0 && DispatchSum == 0 && IncomeSum == 0)
                    continue;
                OpeningBalance = ClosingBalance - DispatchSum + IncomeSum;
                DataRow NewRow = CashReportDT.NewRow();
                NewRow["CurrencyType"] = GetCurrencyType(2);
                NewRow["ClientName"] = GetClientName(ClientID);
                NewRow["OpeningBalance"] = OpeningBalance;
                NewRow["ClosingBalance"] = ClosingBalance;
                NewRow["TotalDispatchSum"] = DispatchSum;
                NewRow["TotalIncomeSum"] = IncomeSum;
                CashReportDT.Rows.Add(NewRow);
            }

            TotalOpeningBalance = 0;
            TotalDispatchSum = 0;
            TotalIncomeSum = 0;
            TotalClosingBalance = 0;
            for (int j = 0; j < CashReportDT.Rows.Count; j++)
            {
                TotalOpeningBalance += Convert.ToDecimal(CashReportDT.Rows[j]["OpeningBalance"]);
                TotalDispatchSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalDispatchSum"]);
                TotalIncomeSum += Convert.ToDecimal(CashReportDT.Rows[j]["TotalIncomeSum"]);
                TotalClosingBalance += Convert.ToDecimal(CashReportDT.Rows[j]["ClosingBalance"]);
            }
            return CashReportDT.Copy();
        }

        #endregion

        #region отчет по НДС
        public DataTable BYRReportVAT(int FactoryID, ref decimal TotalDispatchSum)
        {
            VATReportDT.Clear();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID, infiniu2_marketingreference.dbo.Clients.ClientName FROM ClientsDispatches
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID +
                @"INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID AND CountryID<>1
                WHERE ClientsDispatches.CurrencyTypeID=5 AND ConfirmVAT=0 ORDER BY DispatchDateTime";
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        TotalDispatchSum += Convert.ToDecimal(DT.Rows[i]["DispatchSum"]);
                        int CurrencyTypeID = Convert.ToInt32(DT.Rows[i]["CurrencyTypeID"]);
                        DataRow NewRow = VATReportDT.NewRow();
                        NewRow["Overdue"] = false;
                        if (DT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        {
                            DateTime DispatchDateTime = Convert.ToDateTime(DT.Rows[i]["DispatchDateTime"]);
                            DateTime Deadline = DispatchDateTime.AddDays(180);
                            NewRow["DispatchDateTime"] = DispatchDateTime;
                            NewRow["Deadline"] = Deadline;
                            NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                            if (Deadline < DateTime.Now)
                                NewRow["Overdue"] = true;
                        }
                        NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                        NewRow["ClientName"] = DT.Rows[i]["ClientName"];
                        NewRow["DispatchSum"] = DT.Rows[i]["DispatchSum"];
                        NewRow["Waybill"] = DT.Rows[i]["DispatchWaybills"];
                        VATReportDT.Rows.Add(NewRow);
                    }
                }
            }
            return VATReportDT.Copy();
        }

        public DataTable RUBReportVAT(int FactoryID, ref decimal TotalDispatchSum)
        {
            VATReportDT.Clear();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID, infiniu2_marketingreference.dbo.Clients.ClientName FROM ClientsDispatches
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID +
                @"INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID AND CountryID<>1
                WHERE ClientsDispatches.CurrencyTypeID=3 AND ConfirmVAT=0 ORDER BY DispatchDateTime";
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        TotalDispatchSum += Convert.ToDecimal(DT.Rows[i]["DispatchSum"]);
                        int CurrencyTypeID = Convert.ToInt32(DT.Rows[i]["CurrencyTypeID"]);
                        DataRow NewRow = VATReportDT.NewRow();
                        NewRow["Overdue"] = false;
                        if (DT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        {
                            DateTime DispatchDateTime = Convert.ToDateTime(DT.Rows[i]["DispatchDateTime"]);
                            DateTime Deadline = DispatchDateTime.AddDays(180);
                            NewRow["DispatchDateTime"] = DispatchDateTime;
                            NewRow["Deadline"] = Deadline;
                            NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                            if (Deadline < DateTime.Now)
                                NewRow["Overdue"] = true;
                        }
                        NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                        NewRow["ClientName"] = DT.Rows[i]["ClientName"];
                        NewRow["DispatchSum"] = DT.Rows[i]["DispatchSum"];
                        NewRow["Waybill"] = DT.Rows[i]["DispatchWaybills"];
                        VATReportDT.Rows.Add(NewRow);
                    }
                }
            }
            return VATReportDT.Copy();
        }

        public DataTable EURReportVAT(int FactoryID, ref decimal TotalDispatchSum)
        {
            VATReportDT.Clear();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID, infiniu2_marketingreference.dbo.Clients.ClientName FROM ClientsDispatches
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID +
                @"INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID AND CountryID<>1
                WHERE ClientsDispatches.CurrencyTypeID=1 AND ConfirmVAT=0 ORDER BY DispatchDateTime";
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        TotalDispatchSum += Convert.ToDecimal(DT.Rows[i]["DispatchSum"]);
                        int CurrencyTypeID = Convert.ToInt32(DT.Rows[i]["CurrencyTypeID"]);
                        DataRow NewRow = VATReportDT.NewRow();
                        NewRow["Overdue"] = false;
                        if (DT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        {
                            DateTime DispatchDateTime = Convert.ToDateTime(DT.Rows[i]["DispatchDateTime"]);
                            DateTime Deadline = DispatchDateTime.AddDays(180);
                            NewRow["DispatchDateTime"] = DispatchDateTime;
                            NewRow["Deadline"] = Deadline;
                            NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                            if (Deadline < DateTime.Now)
                                NewRow["Overdue"] = true;
                        }
                        NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                        NewRow["ClientName"] = DT.Rows[i]["ClientName"];
                        NewRow["DispatchSum"] = DT.Rows[i]["DispatchSum"];
                        NewRow["Waybill"] = DT.Rows[i]["DispatchWaybills"];
                        VATReportDT.Rows.Add(NewRow);
                    }
                }
            }
            return VATReportDT.Copy();
        }

        public DataTable USDReportVAT(int FactoryID, ref decimal TotalDispatchSum)
        {
            VATReportDT.Clear();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID, infiniu2_marketingreference.dbo.Clients.ClientName FROM ClientsDispatches
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID +
                @"INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID AND CountryID<>1
                WHERE ClientsDispatches.CurrencyTypeID=2 AND ConfirmVAT=0 ORDER BY DispatchDateTime";
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        TotalDispatchSum += Convert.ToDecimal(DT.Rows[i]["DispatchSum"]);
                        int CurrencyTypeID = Convert.ToInt32(DT.Rows[i]["CurrencyTypeID"]);
                        DataRow NewRow = VATReportDT.NewRow();
                        NewRow["Overdue"] = false;
                        if (DT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        {
                            DateTime DispatchDateTime = Convert.ToDateTime(DT.Rows[i]["DispatchDateTime"]);
                            DateTime Deadline = DispatchDateTime.AddDays(180);
                            NewRow["DispatchDateTime"] = DispatchDateTime;
                            NewRow["Deadline"] = Deadline;
                            NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                            if (Deadline < DateTime.Now)
                                NewRow["Overdue"] = true;
                        }
                        NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                        NewRow["ClientName"] = DT.Rows[i]["ClientName"];
                        NewRow["DispatchSum"] = DT.Rows[i]["DispatchSum"];
                        NewRow["Waybill"] = DT.Rows[i]["DispatchWaybills"];
                        VATReportDT.Rows.Add(NewRow);
                    }
                }
            }
            return VATReportDT.Copy();
        }
        #endregion

        #region отчет по ВС
        public DataTable BYRReportIC(int FactoryID, ref decimal TotalDispatchSum)
        {
            ICReportDT.Clear();
            DataTable DispDT = new DataTable();
            DataTable IncomeDT = new DataTable();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID, infiniu2_marketingreference.dbo.Clients.DelayOfPayment, infiniu2_marketingreference.dbo.Clients.ClientName FROM ClientsDispatches
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID +
                @"INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID AND CountryID<>1
                WHERE ClientsDispatches.CurrencyTypeID=5 ORDER BY DispatchDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispDT);
            }
            SelectCommand = @"SELECT MutualSettlementID, SUM(IncomeSum) FROM ClientsIncomes WHERE ClientsIncomes.CurrencyTypeID=5 GROUP BY MutualSettlementID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(IncomeDT);
            }
            for (int i = 0; i < DispDT.Rows.Count; i++)
            {
                int MutualSettlementID = Convert.ToInt32(DispDT.Rows[i]["MutualSettlementID"]);
                decimal DispatchSum = Convert.ToDecimal(DispDT.Rows[i]["DispatchSum"]);
                DataRow[] Rows = IncomeDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (Rows.Count() > 0)
                {
                    decimal IncomeSum = Convert.ToDecimal(Rows[0]["Column1"]);
                    if (IncomeSum > 0)
                        Rows[0]["Column1"] = IncomeSum - DispatchSum;
                    if (IncomeSum - DispatchSum < 0)
                    {
                        TotalDispatchSum += DispatchSum;
                        int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                        DataRow NewRow = ICReportDT.NewRow();
                        NewRow["Overdue"] = false;
                        if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        {
                            int DelayOfPayment = 90;
                            DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                            DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                            NewRow["DispatchDateTime"] = DispatchDateTime;
                            NewRow["Deadline"] = Deadline;
                            NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                            if (Deadline < DateTime.Now)
                                NewRow["Overdue"] = true;
                        }
                        NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                        NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                        NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                        NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                        ICReportDT.Rows.Add(NewRow);
                    }
                }
                else
                {
                    TotalDispatchSum += DispatchSum;
                    int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                    DataRow NewRow = ICReportDT.NewRow();
                    NewRow["Overdue"] = false;
                    if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                    {
                        int DelayOfPayment = 90;
                        DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                        DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                        NewRow["DispatchDateTime"] = DispatchDateTime;
                        NewRow["Deadline"] = Deadline;
                        NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                        if (Deadline < DateTime.Now)
                            NewRow["Overdue"] = true;
                    }
                    NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                    NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                    NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                    NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                    ICReportDT.Rows.Add(NewRow);
                }
            }

            using (DataView DV = new DataView(ICReportDT.Copy(), string.Empty, "TotalDays", DataViewRowState.CurrentRows))
            {
                ICReportDT.Clear();
                ICReportDT = DV.ToTable();
            }

            DispDT.Dispose();
            IncomeDT.Dispose();
            return ICReportDT.Copy();
        }

        public DataTable RUBReportIC(int FactoryID, ref decimal TotalDispatchSum)
        {
            ICReportDT.Clear();
            DataTable DispDT = new DataTable();
            DataTable IncomeDT = new DataTable();
            DataTable TempDT = new DataTable();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID, infiniu2_marketingreference.dbo.Clients.DelayOfPayment, infiniu2_marketingreference.dbo.Clients.ClientName FROM ClientsDispatches
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID +
                @"INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID AND CountryID<>1
                WHERE ClientsDispatches.CurrencyTypeID=3 ORDER BY DispatchDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispDT);
            }
            SelectCommand = @"SELECT MutualSettlementID, SUM(IncomeSum) FROM ClientsIncomes WHERE ClientsIncomes.CurrencyTypeID=3 GROUP BY MutualSettlementID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(IncomeDT);
            }
            for (int i = 0; i < DispDT.Rows.Count; i++)
            {
                int MutualSettlementID = Convert.ToInt32(DispDT.Rows[i]["MutualSettlementID"]);
                decimal DispatchSum = Convert.ToDecimal(DispDT.Rows[i]["DispatchSum"]);
                DataRow[] Rows = IncomeDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (Rows.Count() > 0)
                {
                    decimal IncomeSum = Convert.ToDecimal(Rows[0]["Column1"]);
                    if (IncomeSum > 0)
                        Rows[0]["Column1"] = IncomeSum - DispatchSum;
                    if (IncomeSum - DispatchSum < 0)
                    {
                        TotalDispatchSum += DispatchSum;
                        int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                        DataRow NewRow = ICReportDT.NewRow();
                        NewRow["Overdue"] = false;
                        if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        {
                            int DelayOfPayment = 90;
                            DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                            DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                            NewRow["DispatchDateTime"] = DispatchDateTime;
                            NewRow["Deadline"] = Deadline;
                            NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                            if (Deadline < DateTime.Now)
                                NewRow["Overdue"] = true;
                        }
                        NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                        NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                        NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                        NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                        ICReportDT.Rows.Add(NewRow);
                    }
                }
                else
                {
                    TotalDispatchSum += DispatchSum;
                    int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                    DataRow NewRow = ICReportDT.NewRow();
                    NewRow["Overdue"] = false;
                    if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                    {
                        int DelayOfPayment = 90;
                        DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                        DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                        NewRow["DispatchDateTime"] = DispatchDateTime;
                        NewRow["Deadline"] = Deadline;
                        NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                        if (Deadline < DateTime.Now)
                            NewRow["Overdue"] = true;
                    }
                    NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                    NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                    NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                    NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                    ICReportDT.Rows.Add(NewRow);
                }
            }

            using (DataView DV = new DataView(ICReportDT.Copy(), string.Empty, "TotalDays", DataViewRowState.CurrentRows))
            {
                ICReportDT.Clear();
                ICReportDT = DV.ToTable();
            }

            DispDT.Dispose();
            IncomeDT.Dispose();
            return ICReportDT.Copy();
        }

        public DataTable USDReportIC(int FactoryID, ref decimal TotalDispatchSum)
        {
            ICReportDT.Clear();
            DataTable DispDT = new DataTable();
            DataTable IncomeDT = new DataTable();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID, infiniu2_marketingreference.dbo.Clients.DelayOfPayment, infiniu2_marketingreference.dbo.Clients.ClientName FROM ClientsDispatches
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID +
                @"INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID AND CountryID<>1
                WHERE ClientsDispatches.CurrencyTypeID=2 ORDER BY DispatchDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispDT);
            }
            SelectCommand = @"SELECT MutualSettlementID, SUM(IncomeSum) FROM ClientsIncomes WHERE ClientsIncomes.CurrencyTypeID=2 GROUP BY MutualSettlementID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(IncomeDT);
            }
            for (int i = 0; i < DispDT.Rows.Count; i++)
            {
                int MutualSettlementID = Convert.ToInt32(DispDT.Rows[i]["MutualSettlementID"]);
                decimal DispatchSum = Convert.ToDecimal(DispDT.Rows[i]["DispatchSum"]);
                DataRow[] Rows = IncomeDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (Rows.Count() > 0)
                {
                    decimal IncomeSum = Convert.ToDecimal(Rows[0]["Column1"]);
                    if (IncomeSum > 0)
                        Rows[0]["Column1"] = IncomeSum - DispatchSum;
                    if (IncomeSum - DispatchSum < 0)
                    {
                        TotalDispatchSum += DispatchSum;
                        int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                        DataRow NewRow = ICReportDT.NewRow();
                        NewRow["Overdue"] = false;
                        if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        {
                            int DelayOfPayment = 90;
                            DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                            DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                            NewRow["DispatchDateTime"] = DispatchDateTime;
                            NewRow["Deadline"] = Deadline;
                            NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                            if (Deadline < DateTime.Now)
                                NewRow["Overdue"] = true;
                        }
                        NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                        NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                        NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                        NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                        ICReportDT.Rows.Add(NewRow);
                    }
                }
                else
                {
                    TotalDispatchSum += DispatchSum;
                    int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                    DataRow NewRow = ICReportDT.NewRow();
                    NewRow["Overdue"] = false;
                    if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                    {
                        int DelayOfPayment = 90;
                        DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                        DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                        NewRow["DispatchDateTime"] = DispatchDateTime;
                        NewRow["Deadline"] = Deadline;
                        NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                        if (Deadline < DateTime.Now)
                            NewRow["Overdue"] = true;
                    }
                    NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                    NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                    NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                    NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                    ICReportDT.Rows.Add(NewRow);
                }
            }

            using (DataView DV = new DataView(ICReportDT.Copy(), string.Empty, "TotalDays", DataViewRowState.CurrentRows))
            {
                ICReportDT.Clear();
                ICReportDT = DV.ToTable();
            }

            DispDT.Dispose();
            IncomeDT.Dispose();
            return ICReportDT.Copy();
        }

        public DataTable EURReportIC(int FactoryID, ref decimal TotalDispatchSum)
        {
            ICReportDT.Clear();
            DataTable DispDT = new DataTable();
            DataTable IncomeDT = new DataTable();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID, infiniu2_marketingreference.dbo.Clients.DelayOfPayment, infiniu2_marketingreference.dbo.Clients.ClientName FROM ClientsDispatches
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID +
                @"INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID AND CountryID<>1
                WHERE ClientsDispatches.CurrencyTypeID=1 ORDER BY DispatchDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispDT);
            }
            SelectCommand = @"SELECT MutualSettlementID, SUM(IncomeSum) FROM ClientsIncomes WHERE ClientsIncomes.CurrencyTypeID=1 GROUP BY MutualSettlementID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(IncomeDT);
            }

            for (int i = 0; i < DispDT.Rows.Count; i++)
            {
                int MutualSettlementID = Convert.ToInt32(DispDT.Rows[i]["MutualSettlementID"]);
                decimal DispatchSum = Convert.ToDecimal(DispDT.Rows[i]["DispatchSum"]);
                DataRow[] Rows = IncomeDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (Rows.Count() > 0)
                {
                    decimal IncomeSum = Convert.ToDecimal(Rows[0]["Column1"]);
                    if (IncomeSum > 0)
                        Rows[0]["Column1"] = IncomeSum - DispatchSum;
                    if (IncomeSum - DispatchSum < 0)
                    {
                        TotalDispatchSum += DispatchSum;
                        int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                        DataRow NewRow = ICReportDT.NewRow();
                        NewRow["Overdue"] = false;
                        if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        {
                            int DelayOfPayment = 90;
                            DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                            DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                            NewRow["DispatchDateTime"] = DispatchDateTime;
                            NewRow["Deadline"] = Deadline;
                            NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                            if (Deadline < DateTime.Now)
                                NewRow["Overdue"] = true;
                        }
                        NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                        NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                        NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                        NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                        ICReportDT.Rows.Add(NewRow);
                    }
                }
                else
                {
                    int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                    DataRow NewRow = ICReportDT.NewRow();
                    NewRow["Overdue"] = false;
                    TotalDispatchSum += DispatchSum;
                    if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                    {
                        int DelayOfPayment = 90;
                        DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                        DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                        NewRow["DispatchDateTime"] = DispatchDateTime;
                        NewRow["Deadline"] = Deadline;
                        NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                        if (Deadline < DateTime.Now)
                            NewRow["Overdue"] = true;
                    }
                    NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                    NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                    NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                    NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                    ICReportDT.Rows.Add(NewRow);
                }
            }

            using (DataView DV = new DataView(ICReportDT.Copy(), string.Empty, "TotalDays", DataViewRowState.CurrentRows))
            {
                ICReportDT.Clear();
                ICReportDT = DV.ToTable();
            }

            DispDT.Dispose();
            IncomeDT.Dispose();
            return ICReportDT.Copy();
        }
        #endregion

        #region отчет по отсрочке
        public DataTable BYRReportDelayOfPayment(int FactoryID, ref decimal TotalDispatchSum)
        {
            ICReportDT.Clear();
            DataTable DispDT = new DataTable();
            DataTable IncomeDT = new DataTable();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID, infiniu2_marketingreference.dbo.Clients.DelayOfPayment, infiniu2_marketingreference.dbo.Clients.ClientName FROM ClientsDispatches
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID +
                @"INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE ClientsDispatches.CurrencyTypeID=5 ORDER BY DispatchDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispDT);
            }
            SelectCommand = @"SELECT MutualSettlementID, IncomeSum FROM ClientsIncomes WHERE ClientsIncomes.CurrencyTypeID=5 ORDER BY IncomeDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(IncomeDT);
            }
            for (int i = 0; i < DispDT.Rows.Count; i++)
            {
                int ClientID = Convert.ToInt32(DispDT.Rows[i]["ClientID"]);
                int MutualSettlementID = Convert.ToInt32(DispDT.Rows[i]["MutualSettlementID"]);
                decimal DispatchSum = Convert.ToDecimal(DispDT.Rows[i]["DispatchSum"]);
                DataRow[] Rows = IncomeDT.Select("MutualSettlementID = " + MutualSettlementID);
                decimal AllDispatchSum = 0;
                decimal AllIncomeSum = 0;
                DataRow[] dRows = DispDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (dRows.Any())
                {
                    AllDispatchSum += dRows.Sum(item => Convert.ToDecimal(item["DispatchSum"]));
                }
                DataRow[] cRows = IncomeDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (cRows.Any())
                {
                    AllIncomeSum += cRows.Sum(item => Convert.ToDecimal(item["IncomeSum"]));
                }
                if (cRows.Any())
                {
                    //decimal IncomeSum = Convert.ToDecimal(Rows[0]["Column1"]);
                    if (AllIncomeSum - AllDispatchSum < 0)
                    {
                        TotalDispatchSum += DispatchSum;
                        decimal d = DispatchSum;
                        foreach (DataRow dr in IncomeDT.Select("MutualSettlementID = " + MutualSettlementID))
                        {
                            decimal c = Convert.ToDecimal(dr["IncomeSum"]);
                            if (d <= 0)
                                break;
                            if (c == 0)
                                continue;
                            if (Convert.ToDecimal(dr["IncomeSum"]) >= d)
                            {
                                dr["IncomeSum"] = Convert.ToDecimal(dr["IncomeSum"]) - d;
                                d = 0;
                                break;
                            }
                            d -= Convert.ToDecimal(dr["IncomeSum"]);
                            dr["IncomeSum"] = 0;
                        }

                        if (d <= 0)
                            continue;
                        int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                        DataRow NewRow = ICReportDT.NewRow();
                        NewRow["Overdue"] = false;
                        if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        {
                            int DelayOfPayment = Convert.ToInt32(DispDT.Rows[i]["DelayOfPayment"]);
                            DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                            DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                            NewRow["DispatchDateTime"] = DispatchDateTime;
                            NewRow["Deadline"] = Deadline;
                            NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                            if (Deadline < DateTime.Now)
                                NewRow["Overdue"] = true;
                        }
                        NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                        NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                        NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                        NewRow["DebtSum"] = d;
                        //if ((AllIncomeSum - d) * -1 > d)
                        //    NewRow["DebtSum"] = d;
                        //else
                        //    NewRow["DebtSum"] = (AllIncomeSum - d) * -1;
                        NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                        ICReportDT.Rows.Add(NewRow);
                    }
                }
                else
                {
                    TotalDispatchSum += DispatchSum;
                    int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                    DataRow NewRow = ICReportDT.NewRow();
                    NewRow["Overdue"] = false;
                    if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                    {
                        int DelayOfPayment = Convert.ToInt32(DispDT.Rows[i]["DelayOfPayment"]);
                        DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                        DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                        NewRow["DispatchDateTime"] = DispatchDateTime;
                        NewRow["Deadline"] = Deadline;
                        NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                        if (Deadline < DateTime.Now)
                            NewRow["Overdue"] = true;
                    }
                    NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                    NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                    NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                    NewRow["DebtSum"] = DispDT.Rows[i]["DispatchSum"];
                    NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                    ICReportDT.Rows.Add(NewRow);
                }
            }

            using (DataView DV = new DataView(ICReportDT.Copy(), string.Empty, "TotalDays", DataViewRowState.CurrentRows))
            {
                ICReportDT.Clear();
                ICReportDT = DV.ToTable();
            }

            DispDT.Dispose();
            IncomeDT.Dispose();
            return ICReportDT.Copy();
        }

        public DataTable RUBReportDelayOfPayment(int FactoryID, ref decimal TotalDispatchSum)
        {
            ICReportDT.Clear();
            DataTable DispDT = new DataTable();
            DataTable IncomeDT = new DataTable();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID, infiniu2_marketingreference.dbo.Clients.DelayOfPayment, infiniu2_marketingreference.dbo.Clients.ClientName FROM ClientsDispatches
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID +
                @"INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE ClientsDispatches.CurrencyTypeID=3 ORDER BY DispatchDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispDT);
            }
            SelectCommand = @"SELECT MutualSettlementID, IncomeSum FROM ClientsIncomes WHERE ClientsIncomes.CurrencyTypeID=3 ORDER BY IncomeDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(IncomeDT);
            }
            for (int i = 0; i < DispDT.Rows.Count; i++)
            {
                int ClientID = Convert.ToInt32(DispDT.Rows[i]["ClientID"]);
                int MutualSettlementID = Convert.ToInt32(DispDT.Rows[i]["MutualSettlementID"]);
                decimal DispatchSum = Convert.ToDecimal(DispDT.Rows[i]["DispatchSum"]);
                DataRow[] Rows = IncomeDT.Select("MutualSettlementID = " + MutualSettlementID);
                decimal AllDispatchSum = 0;
                decimal AllIncomeSum = 0;
                DataRow[] dRows = DispDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (dRows.Any())
                {
                    AllDispatchSum += dRows.Sum(item => Convert.ToDecimal(item["DispatchSum"]));
                }
                DataRow[] cRows = IncomeDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (cRows.Any())
                {
                    AllIncomeSum += cRows.Sum(item => Convert.ToDecimal(item["IncomeSum"]));
                }
                if (cRows.Any())
                {
                    //decimal IncomeSum = Convert.ToDecimal(Rows[0]["Column1"]);
                    if (AllIncomeSum - AllDispatchSum < 0)
                    {
                        TotalDispatchSum += DispatchSum;
                        decimal d = DispatchSum;
                        foreach (DataRow dr in IncomeDT.Select("MutualSettlementID = " + MutualSettlementID))
                        {
                            decimal c = Convert.ToDecimal(dr["IncomeSum"]);
                            if (d <= 0)
                                break;
                            if (c == 0)
                                continue;
                            if (Convert.ToDecimal(dr["IncomeSum"]) >= d)
                            {
                                dr["IncomeSum"] = Convert.ToDecimal(dr["IncomeSum"]) - d;
                                d = 0;
                                break;
                            }
                            d -= Convert.ToDecimal(dr["IncomeSum"]);
                            dr["IncomeSum"] = 0;
                        }

                        if (d <= 0)
                            continue;
                        int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                        DataRow NewRow = ICReportDT.NewRow();
                        NewRow["Overdue"] = false;
                        if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        {
                            int DelayOfPayment = Convert.ToInt32(DispDT.Rows[i]["DelayOfPayment"]);
                            DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                            DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                            NewRow["DispatchDateTime"] = DispatchDateTime;
                            NewRow["Deadline"] = Deadline;
                            NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                            if (Deadline < DateTime.Now)
                                NewRow["Overdue"] = true;
                        }
                        NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                        NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                        NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                        NewRow["DebtSum"] = d;
                        //if ((AllIncomeSum - d) * -1 > d)
                        //    NewRow["DebtSum"] = d;
                        //else
                        //    NewRow["DebtSum"] = (AllIncomeSum - d) * -1;
                        NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                        ICReportDT.Rows.Add(NewRow);
                    }
                }
                else
                {
                    TotalDispatchSum += DispatchSum;
                    int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                    DataRow NewRow = ICReportDT.NewRow();
                    NewRow["Overdue"] = false;
                    if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                    {
                        int DelayOfPayment = Convert.ToInt32(DispDT.Rows[i]["DelayOfPayment"]);
                        DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                        DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                        NewRow["DispatchDateTime"] = DispatchDateTime;
                        NewRow["Deadline"] = Deadline;
                        NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                        if (Deadline < DateTime.Now)
                            NewRow["Overdue"] = true;
                    }
                    NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                    NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                    NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                    NewRow["DebtSum"] = DispDT.Rows[i]["DispatchSum"];
                    NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                    ICReportDT.Rows.Add(NewRow);
                }
            }

            using (DataView DV = new DataView(ICReportDT.Copy(), string.Empty, "TotalDays", DataViewRowState.CurrentRows))
            {
                ICReportDT.Clear();
                ICReportDT = DV.ToTable();
            }

            DispDT.Dispose();
            IncomeDT.Dispose();
            return ICReportDT.Copy();
        }

        public DataTable USDReportDelayOfPayment(int FactoryID, ref decimal TotalDispatchSum)
        {
            ICReportDT.Clear();
            DataTable DispDT = new DataTable();
            DataTable IncomeDT = new DataTable();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID, infiniu2_marketingreference.dbo.Clients.DelayOfPayment, infiniu2_marketingreference.dbo.Clients.ClientName FROM ClientsDispatches
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID +
                @"INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE ClientsDispatches.CurrencyTypeID=2 ORDER BY DispatchDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispDT);
            }
            SelectCommand = @"SELECT MutualSettlementID, IncomeSum FROM ClientsIncomes WHERE ClientsIncomes.CurrencyTypeID=2 ORDER BY IncomeDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(IncomeDT);
            }
            for (int i = 0; i < DispDT.Rows.Count; i++)
            {
                int ClientID = Convert.ToInt32(DispDT.Rows[i]["ClientID"]);
                int MutualSettlementID = Convert.ToInt32(DispDT.Rows[i]["MutualSettlementID"]);
                decimal DispatchSum = Convert.ToDecimal(DispDT.Rows[i]["DispatchSum"]);
                DataRow[] Rows = IncomeDT.Select("MutualSettlementID = " + MutualSettlementID);
                decimal AllDispatchSum = 0;
                decimal AllIncomeSum = 0;
                DataRow[] dRows = DispDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (dRows.Any())
                {
                    AllDispatchSum += dRows.Sum(item => Convert.ToDecimal(item["DispatchSum"]));
                }
                DataRow[] cRows = IncomeDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (cRows.Any())
                {
                    AllIncomeSum += cRows.Sum(item => Convert.ToDecimal(item["IncomeSum"]));
                }
                if (cRows.Any())
                {
                    //decimal IncomeSum = Convert.ToDecimal(Rows[0]["Column1"]);
                    if (AllIncomeSum - AllDispatchSum < 0)
                    {
                        TotalDispatchSum += DispatchSum;
                        decimal d = DispatchSum;
                        foreach (DataRow dr in IncomeDT.Select("MutualSettlementID = " + MutualSettlementID))
                        {
                            decimal c = Convert.ToDecimal(dr["IncomeSum"]);
                            if (d <= 0)
                                break;
                            if (c == 0)
                                continue;
                            if (Convert.ToDecimal(dr["IncomeSum"]) >= d)
                            {
                                dr["IncomeSum"] = Convert.ToDecimal(dr["IncomeSum"]) - d;
                                d = 0;
                                break;
                            }
                            d -= Convert.ToDecimal(dr["IncomeSum"]);
                            dr["IncomeSum"] = 0;
                        }

                        if (d <= 0)
                            continue;
                        int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                        DataRow NewRow = ICReportDT.NewRow();
                        NewRow["Overdue"] = false;
                        if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        {
                            int DelayOfPayment = Convert.ToInt32(DispDT.Rows[i]["DelayOfPayment"]);
                            DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                            DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                            NewRow["DispatchDateTime"] = DispatchDateTime;
                            NewRow["Deadline"] = Deadline;
                            NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                            if (Deadline < DateTime.Now)
                                NewRow["Overdue"] = true;
                        }
                        NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                        NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                        NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                        NewRow["DebtSum"] = d;
                        //if ((AllIncomeSum - d) * -1 > d)
                        //    NewRow["DebtSum"] = d;
                        //else
                        //    NewRow["DebtSum"] = (AllIncomeSum - d) * -1;
                        NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                        ICReportDT.Rows.Add(NewRow);
                    }
                }
                else
                {
                    TotalDispatchSum += DispatchSum;
                    int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                    DataRow NewRow = ICReportDT.NewRow();
                    NewRow["Overdue"] = false;
                    if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                    {
                        int DelayOfPayment = Convert.ToInt32(DispDT.Rows[i]["DelayOfPayment"]);
                        DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                        DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                        NewRow["DispatchDateTime"] = DispatchDateTime;
                        NewRow["Deadline"] = Deadline;
                        NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                        if (Deadline < DateTime.Now)
                            NewRow["Overdue"] = true;
                    }
                    NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                    NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                    NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                    NewRow["DebtSum"] = DispDT.Rows[i]["DispatchSum"];
                    NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                    ICReportDT.Rows.Add(NewRow);
                }
            }

            using (DataView DV = new DataView(ICReportDT.Copy(), string.Empty, "TotalDays", DataViewRowState.CurrentRows))
            {
                ICReportDT.Clear();
                ICReportDT = DV.ToTable();
            }

            DispDT.Dispose();
            IncomeDT.Dispose();
            return ICReportDT.Copy();
        }

        public DataTable EURReportDelayOfPayment(int FactoryID, ref decimal TotalDispatchSum)
        {
            ICReportDT.Clear();
            DataTable DispDT = new DataTable();
            DataTable IncomeDT = new DataTable();
            string SelectCommand = @"SELECT ClientsDispatches.*, MutualSettlements.ClientID, infiniu2_marketingreference.dbo.Clients.DelayOfPayment, infiniu2_marketingreference.dbo.Clients.ClientName FROM ClientsDispatches
                INNER JOIN MutualSettlements ON ClientsDispatches.MutualSettlementID=MutualSettlements.MutualSettlementID AND MutualSettlements.FactoryID=" + FactoryID +
                @"INNER JOIN infiniu2_marketingreference.dbo.Clients ON MutualSettlements.ClientID=infiniu2_marketingreference.dbo.Clients.ClientID
                WHERE ClientsDispatches.CurrencyTypeID=1 ORDER BY DispatchDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DispDT);
            }
            SelectCommand = @"SELECT MutualSettlementID, IncomeSum FROM ClientsIncomes WHERE ClientsIncomes.CurrencyTypeID=1 ORDER BY IncomeDateTime";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(IncomeDT);
            }
            for (int i = 0; i < DispDT.Rows.Count; i++)
            {
                int ClientID = Convert.ToInt32(DispDT.Rows[i]["ClientID"]);
                int MutualSettlementID = Convert.ToInt32(DispDT.Rows[i]["MutualSettlementID"]);
                decimal DispatchSum = Convert.ToDecimal(DispDT.Rows[i]["DispatchSum"]);
                DataRow[] Rows = IncomeDT.Select("MutualSettlementID = " + MutualSettlementID);
                decimal AllDispatchSum = 0;
                decimal AllIncomeSum = 0;
                DataRow[] dRows = DispDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (dRows.Any())
                {
                    AllDispatchSum += dRows.Sum(item => Convert.ToDecimal(item["DispatchSum"]));
                }
                DataRow[] cRows = IncomeDT.Select("MutualSettlementID = " + MutualSettlementID);
                if (cRows.Any())
                {
                    AllIncomeSum += cRows.Sum(item => Convert.ToDecimal(item["IncomeSum"]));
                }
                if (cRows.Any())
                {
                    //decimal IncomeSum = Convert.ToDecimal(Rows[0]["Column1"]);
                    if (AllIncomeSum - AllDispatchSum < 0)
                    {
                        TotalDispatchSum += DispatchSum;
                        decimal d = DispatchSum;
                        foreach (DataRow dr in IncomeDT.Select("MutualSettlementID = " + MutualSettlementID))
                        {
                            decimal c = Convert.ToDecimal(dr["IncomeSum"]);
                            if (d <= 0)
                                break;
                            if (c == 0)
                                continue;
                            if (Convert.ToDecimal(dr["IncomeSum"]) >= d)
                            {
                                dr["IncomeSum"] = Convert.ToDecimal(dr["IncomeSum"]) - d;
                                d = 0;
                                break;
                            }
                            d -= Convert.ToDecimal(dr["IncomeSum"]);
                            dr["IncomeSum"] = 0;
                        }

                        if (d <= 0)
                            continue;
                        int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                        DataRow NewRow = ICReportDT.NewRow();
                        NewRow["Overdue"] = false;
                        if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        {
                            int DelayOfPayment = Convert.ToInt32(DispDT.Rows[i]["DelayOfPayment"]);
                            DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                            DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                            NewRow["DispatchDateTime"] = DispatchDateTime;
                            NewRow["Deadline"] = Deadline;
                            NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                            if (Deadline < DateTime.Now)
                                NewRow["Overdue"] = true;
                        }
                        NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                        NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                        NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                        NewRow["DebtSum"] = d;
                        //if ((AllIncomeSum - d) * -1 > d)
                        //    NewRow["DebtSum"] = d;
                        //else
                        //    NewRow["DebtSum"] = (AllIncomeSum - d) * -1;
                        NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                        ICReportDT.Rows.Add(NewRow);
                    }
                }
                else
                {
                    TotalDispatchSum += DispatchSum;
                    int CurrencyTypeID = Convert.ToInt32(DispDT.Rows[i]["CurrencyTypeID"]);
                    DataRow NewRow = ICReportDT.NewRow();
                    NewRow["Overdue"] = false;
                    if (DispDT.Rows[i]["DispatchDateTime"] != DBNull.Value)
                    {
                        int DelayOfPayment = Convert.ToInt32(DispDT.Rows[i]["DelayOfPayment"]);
                        DateTime DispatchDateTime = Convert.ToDateTime(DispDT.Rows[i]["DispatchDateTime"]);
                        DateTime Deadline = DispatchDateTime.AddDays(DelayOfPayment);
                        NewRow["DispatchDateTime"] = DispatchDateTime;
                        NewRow["Deadline"] = Deadline;
                        NewRow["TotalDays"] = -(DateTime.Now - Deadline).TotalDays;
                        if (Deadline < DateTime.Now)
                            NewRow["Overdue"] = true;
                    }
                    NewRow["CurrencyType"] = GetCurrencyType(CurrencyTypeID);
                    NewRow["ClientName"] = DispDT.Rows[i]["ClientName"];
                    NewRow["DispatchSum"] = DispDT.Rows[i]["DispatchSum"];
                    NewRow["DebtSum"] = DispDT.Rows[i]["DispatchSum"];
                    NewRow["Waybill"] = DispDT.Rows[i]["DispatchWaybills"];
                    ICReportDT.Rows.Add(NewRow);
                }
            }

            using (DataView DV = new DataView(ICReportDT.Copy(), string.Empty, "TotalDays", DataViewRowState.CurrentRows))
            {
                ICReportDT.Clear();
                ICReportDT = DV.ToTable();
            }

            DispDT.Dispose();
            IncomeDT.Dispose();
            return ICReportDT.Copy();
        }
        #endregion

        public string TotalClientDispatch()
        {
            System.Globalization.NumberFormatInfo nfi1 = new System.Globalization.NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ","
            };
            decimal S = 0;

            foreach (DataRow Row in ClientsDispatchesDT.Rows)
            {
                if (Row["DispatchSum"] != DBNull.Value)
                    S += Convert.ToDecimal(Row["DispatchSum"]);
            }
            S = Decimal.Round(S, 3, MidpointRounding.AwayFromZero);
            return S.ToString("C", nfi1) + " €";
        }

        public string TotalClientIncomes()
        {
            System.Globalization.NumberFormatInfo nfi1 = new System.Globalization.NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ","
            };
            decimal S = 0;

            foreach (DataRow Row in ClientsIncomesDT.Rows)
            {
                if (Row["IncomeSum"] != DBNull.Value)
                    S += Convert.ToDecimal(Row["IncomeSum"]);
            }
            S = Decimal.Round(S, 3, MidpointRounding.AwayFromZero);
            return S.ToString("C", nfi1) + " €";
        }

        public void ConfirmVAT(int ClientDispatchID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsDispatches WHERE ClientDispatchID=" + ClientDispatchID, ConnectionStrings.MarketingOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0 && !Convert.ToBoolean(DT.Rows[0]["ConfirmVAT"]))
                        {
                            DT.Rows[0]["ConfirmVAT"] = true;
                            DT.Rows[0]["ConfirmDateTime"] = Security.GetCurrentDate();
                            DT.Rows[0]["ConfirmUserID"] = Security.CurrentUserID;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public int GetClientCountry(int ClientID)
        {
            int ClientCountryID = 0;
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, CountryID FROM Clients WHERE ClientID=" + ClientID,
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    DA.Fill(DT);
                    if (DT.Rows.Count > 0)
                        ClientCountryID = Convert.ToInt32(DT.Rows[0]["CountryID"]);
                }
            }
            return ClientCountryID;
        }

        private string GetClientName(int ClientID)
        {
            string Name = string.Empty;
            try
            {
                DataRow[] Rows = ClientsDT.Select("ClientID = " + ClientID);
                Name = Rows[0]["ClientName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return Name;
        }

        private string GetCurrencyType(int CurrencyTypeID)
        {
            string Name = string.Empty;
            try
            {
                DataRow[] Rows = CurrencyTypesDT.Select("CurrencyTypeID = " + CurrencyTypeID);
                Name = Rows[0]["CurrencyType"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return Name;
        }

        public int[] ToIntArray(string value, char separator)
        {
            return Array.ConvertAll(value.Split(separator), s => int.Parse(s));
        }

        public int CompareBalance(int MutualSettlementID)
        {
            int Result = 0;
            decimal TotalDispatchSum = 0;
            decimal TotalIncomeSum = 0;
            DataRow[] drows = ClientsDispatchesDT.Select("MutualSettlementID=" + MutualSettlementID);
            for (int i = 0; i < drows.Count(); i++)
                TotalDispatchSum += Convert.ToDecimal(drows[i]["DispatchSum"]);
            drows = ClientsIncomesDT.Select("MutualSettlementID=" + MutualSettlementID);
            for (int i = 0; i < drows.Count(); i++)
                TotalIncomeSum += Convert.ToDecimal(drows[i]["IncomeSum"]);

            if (TotalDispatchSum == TotalIncomeSum)
                Result = 1;
            if (TotalDispatchSum > TotalIncomeSum)
                Result = 2;
            if (TotalDispatchSum < TotalIncomeSum)
                Result = 3;
            if (TotalDispatchSum == 0 && TotalIncomeSum == 0)
                Result = 0;

            return Result;
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

    }


    public class VATReportToExcel
    {
        HSSFWorkbook hssfworkbook;
        HSSFSheet sheet1;
        HSSFSheet sheet2;

        HSSFFont CurrencyF;
        HSSFFont HeaderF;
        HSSFFont OverdueSimpleF;
        HSSFFont SimpleF;
        HSSFFont TotalCountF;
        HSSFCellStyle CountCS;
        HSSFCellStyle BelCountCS;
        HSSFCellStyle CurrencyCS;
        HSSFCellStyle HeaderCS;
        HSSFCellStyle OverdueCountCS;
        HSSFCellStyle BelOverdueCountCS;
        HSSFCellStyle OverdueSimpleCS;
        HSSFCellStyle SimpleCS;
        HSSFCellStyle TotalCS;
        HSSFCellStyle TotalCountCS;
        HSSFCellStyle BelTotalCountCS;

        public VATReportToExcel()
        {
            Create();
        }

        private void Create()
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            sheet1 = hssfworkbook.CreateSheet("ЗОВ-Профиль");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int DisplayIndex = 0;
            sheet1.SetColumnWidth(DisplayIndex++, 40 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 17 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 17 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 17 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 10 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 17 * 256);

            sheet2 = hssfworkbook.CreateSheet("ЗОВ-ТПС");
            sheet2.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet2.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet2.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet2.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet2.SetMargin(HSSFSheet.BottomMargin, (double).20);

            DisplayIndex = 0;
            sheet2.SetColumnWidth(DisplayIndex++, 40 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 17 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 17 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 17 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 10 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 17 * 256);

            CurrencyF = hssfworkbook.CreateFont();
            CurrencyF.FontHeightInPoints = 11;
            CurrencyF.Boldweight = 10 * 256;
            CurrencyF.FontName = "Times New Roman";

            HeaderF = hssfworkbook.CreateFont();
            HeaderF.FontHeightInPoints = 12;
            HeaderF.Boldweight = 10 * 256;
            HeaderF.FontName = "Times New Roman";

            OverdueSimpleF = hssfworkbook.CreateFont();
            OverdueSimpleF.Color = HSSFColor.RED.index;
            OverdueSimpleF.FontHeightInPoints = 11;
            OverdueSimpleF.FontName = "Times New Roman";

            SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 11;
            SimpleF.FontName = "Times New Roman";

            TotalCountF = hssfworkbook.CreateFont();
            TotalCountF.FontHeightInPoints = 10;
            TotalCountF.Boldweight = 10 * 256;
            TotalCountF.FontName = "Times New Roman";
            TotalCountF.Underline = (byte)HSSFFontFormatting.U_SINGLE;
            TotalCountF.IsItalic = true;

            OverdueSimpleCS = hssfworkbook.CreateCellStyle();
            OverdueSimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            OverdueSimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            OverdueSimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            OverdueSimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            OverdueSimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            OverdueSimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            OverdueSimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            OverdueSimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            OverdueSimpleCS.SetFont(OverdueSimpleF);

            SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCS.SetFont(SimpleF);

            OverdueCountCS = hssfworkbook.CreateCellStyle();
            OverdueCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            OverdueCountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            OverdueCountCS.BottomBorderColor = HSSFColor.BLACK.index;
            OverdueCountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            OverdueCountCS.LeftBorderColor = HSSFColor.BLACK.index;
            OverdueCountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            OverdueCountCS.RightBorderColor = HSSFColor.BLACK.index;
            OverdueCountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            OverdueCountCS.TopBorderColor = HSSFColor.BLACK.index;
            OverdueCountCS.SetFont(OverdueSimpleF);

            BelOverdueCountCS = hssfworkbook.CreateCellStyle();
            BelOverdueCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            BelOverdueCountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            BelOverdueCountCS.BottomBorderColor = HSSFColor.BLACK.index;
            BelOverdueCountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BelOverdueCountCS.LeftBorderColor = HSSFColor.BLACK.index;
            BelOverdueCountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            BelOverdueCountCS.RightBorderColor = HSSFColor.BLACK.index;
            BelOverdueCountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            BelOverdueCountCS.TopBorderColor = HSSFColor.BLACK.index;
            BelOverdueCountCS.SetFont(OverdueSimpleF);

            CountCS = hssfworkbook.CreateCellStyle();
            CountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            CountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CountCS.BottomBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            CountCS.LeftBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CountCS.RightBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CountCS.TopBorderColor = HSSFColor.BLACK.index;
            CountCS.SetFont(SimpleF);

            BelCountCS = hssfworkbook.CreateCellStyle();
            BelCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            BelCountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            BelCountCS.BottomBorderColor = HSSFColor.BLACK.index;
            BelCountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BelCountCS.LeftBorderColor = HSSFColor.BLACK.index;
            BelCountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            BelCountCS.RightBorderColor = HSSFColor.BLACK.index;
            BelCountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            BelCountCS.TopBorderColor = HSSFColor.BLACK.index;
            BelCountCS.SetFont(SimpleF);

            CurrencyCS = hssfworkbook.CreateCellStyle();
            CurrencyCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            CurrencyCS.SetFont(CurrencyF);

            HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            HeaderCS.SetFont(HeaderF);

            TotalCS = hssfworkbook.CreateCellStyle();
            TotalCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalCS.SetFont(CurrencyF);

            TotalCountCS = hssfworkbook.CreateCellStyle();
            TotalCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ### ##0.00");
            TotalCountCS.SetFont(TotalCountF);

            BelTotalCountCS = hssfworkbook.CreateCellStyle();
            BelTotalCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ### ##0");
            BelTotalCountCS.SetFont(TotalCountF);

            int pos = 1;
            DisplayIndex = 0;
            HSSFCell Cell1;
            Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
            Cell1.SetCellValue("Отчет по фирме ЗОВ-Профиль");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Клиент");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Дата отгрузки");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("№ ТТН");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Сумма");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Валюта");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Крайний срок");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Дни");
            Cell1.CellStyle = HeaderCS;

            pos = 1;
            DisplayIndex = 0;
            Cell1 = sheet2.CreateRow(pos++).CreateCell(0);
            Cell1.SetCellValue("Отчет по фирме ЗОВ-ТПС");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Клиент");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Дата отгрузки");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("№ ТТН");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Сумма");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Валюта");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Крайний срок");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Дни");
            Cell1.CellStyle = HeaderCS;
        }

        public void SaveOpenReport()
        {
            string FileName = "Отчет по НДС";
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

            System.Diagnostics.Process.Start(file.FullName);
        }

        public void ZOVProfilReport(DataTable ReportTable, string Currency, decimal TotalDispatchSum, ref int RowIndex)
        {
            if (ReportTable.Rows.Count == 0)
                return;

            int DisplayIndex = 0;

            HSSFCell Cell1;
            int pos = RowIndex;

            pos++;

            Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue(Currency);
            Cell1.CellStyle = CurrencyCS;
            pos++;

            for (int i = 0; i < ReportTable.Rows.Count; i++)
            {
                DisplayIndex = 0;
                if (Convert.ToBoolean(ReportTable.Rows[i]["Overdue"]))
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["ClientName"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["DispatchDateTime"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["WayBill"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DispatchSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelOverdueCountCS;
                    //else
                    Cell1.CellStyle = OverdueCountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["CurrencyType"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["Deadline"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["Deadline"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["TotalDays"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(ReportTable.Rows[i]["TotalDays"]));
                    Cell1.CellStyle = OverdueSimpleCS;
                }
                else
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["ClientName"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["DispatchDateTime"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["WayBill"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DispatchSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelCountCS;
                    //else
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["CurrencyType"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["Deadline"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["Deadline"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["TotalDays"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(ReportTable.Rows[i]["TotalDays"]));
                    Cell1.CellStyle = SimpleCS;
                }
                pos++;
            }
            Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("Итого:");
            Cell1.CellStyle = TotalCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(3);
            Cell1.SetCellValue(Convert.ToDouble(TotalDispatchSum));
            //if (Currency == "BYN")
            //    Cell1.CellStyle = BelTotalCountCS;
            //else
            Cell1.CellStyle = TotalCountCS;

            RowIndex = pos++;
        }

        public void ZOVTPSReport(DataTable ReportTable, string Currency, decimal TotalDispatchSum, ref int RowIndex)
        {
            if (ReportTable.Rows.Count == 0)
                return;

            int DisplayIndex = 0;

            HSSFCell Cell1;
            int pos = RowIndex;

            pos++;

            Cell1 = sheet2.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue(Currency);
            Cell1.CellStyle = CurrencyCS;
            pos++;

            for (int i = 0; i < ReportTable.Rows.Count; i++)
            {
                DisplayIndex = 0;
                if (Convert.ToBoolean(ReportTable.Rows[i]["Overdue"]))
                {
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["ClientName"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["DispatchDateTime"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["WayBill"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DispatchSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelOverdueCountCS;
                    //else
                    Cell1.CellStyle = OverdueCountCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["CurrencyType"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["Deadline"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["Deadline"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["TotalDays"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(ReportTable.Rows[i]["TotalDays"]));
                    Cell1.CellStyle = OverdueSimpleCS;
                }
                else
                {
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["ClientName"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["DispatchDateTime"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["WayBill"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DispatchSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelCountCS;
                    //else
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["CurrencyType"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["Deadline"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["Deadline"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["TotalDays"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(ReportTable.Rows[i]["TotalDays"]));
                    Cell1.CellStyle = SimpleCS;
                }
                pos++;
            }

            Cell1 = sheet2.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("Итого:");
            Cell1.CellStyle = TotalCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(3);
            Cell1.SetCellValue(Convert.ToDouble(TotalDispatchSum));
            //if (Currency == "BYN")
            //    Cell1.CellStyle = BelTotalCountCS;
            //else
            Cell1.CellStyle = TotalCountCS;

            RowIndex = pos++;
        }

        public static string ReplaceEscape(string str)
        {
            str = str.Replace("'", "''");
            return str;
        }
    }


    public class ICReportToExcel
    {
        HSSFWorkbook hssfworkbook;
        HSSFSheet sheet1;
        HSSFSheet sheet2;

        HSSFFont CurrencyF;
        HSSFFont HeaderF;
        HSSFFont OverdueSimpleF;
        HSSFFont SimpleF;
        HSSFFont TotalCountF;
        HSSFCellStyle CountCS;
        HSSFCellStyle BelCountCS;
        HSSFCellStyle CurrencyCS;
        HSSFCellStyle HeaderCS;
        HSSFCellStyle OverdueCountCS;
        HSSFCellStyle BelOverdueCountCS;
        HSSFCellStyle OverdueSimpleCS;
        HSSFCellStyle SimpleCS;
        HSSFCellStyle TotalCS;
        HSSFCellStyle TotalCountCS;
        HSSFCellStyle BelTotalCountCS;

        public ICReportToExcel()
        {
            Create();
        }

        private void Create()
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            sheet1 = hssfworkbook.CreateSheet("ЗОВ-Профиль");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int DisplayIndex = 0;
            sheet1.SetColumnWidth(DisplayIndex++, 40 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 18 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 15 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 20 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 10 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 15 * 256);

            sheet2 = hssfworkbook.CreateSheet("ЗОВ-ТПС");
            sheet2.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet2.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet2.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet2.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet2.SetMargin(HSSFSheet.BottomMargin, (double).20);

            DisplayIndex = 0;
            sheet2.SetColumnWidth(DisplayIndex++, 40 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 18 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 15 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 20 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 10 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 15 * 256);

            CurrencyF = hssfworkbook.CreateFont();
            CurrencyF.FontHeightInPoints = 11;
            CurrencyF.Boldweight = 10 * 256;
            CurrencyF.FontName = "Times New Roman";

            HeaderF = hssfworkbook.CreateFont();
            HeaderF.FontHeightInPoints = 12;
            HeaderF.Boldweight = 10 * 256;
            HeaderF.FontName = "Times New Roman";

            OverdueSimpleF = hssfworkbook.CreateFont();
            OverdueSimpleF.Color = HSSFColor.RED.index;
            OverdueSimpleF.FontHeightInPoints = 11;
            OverdueSimpleF.FontName = "Times New Roman";

            SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 11;
            SimpleF.FontName = "Times New Roman";

            TotalCountF = hssfworkbook.CreateFont();
            TotalCountF.FontHeightInPoints = 10;
            TotalCountF.Boldweight = 10 * 256;
            TotalCountF.FontName = "Times New Roman";
            TotalCountF.Underline = (byte)HSSFFontFormatting.U_SINGLE;
            TotalCountF.IsItalic = true;

            OverdueSimpleCS = hssfworkbook.CreateCellStyle();
            OverdueSimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            OverdueSimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            OverdueSimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            OverdueSimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            OverdueSimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            OverdueSimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            OverdueSimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            OverdueSimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            OverdueSimpleCS.SetFont(OverdueSimpleF);

            OverdueCountCS = hssfworkbook.CreateCellStyle();
            OverdueCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            OverdueCountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            OverdueCountCS.BottomBorderColor = HSSFColor.BLACK.index;
            OverdueCountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            OverdueCountCS.LeftBorderColor = HSSFColor.BLACK.index;
            OverdueCountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            OverdueCountCS.RightBorderColor = HSSFColor.BLACK.index;
            OverdueCountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            OverdueCountCS.TopBorderColor = HSSFColor.BLACK.index;
            OverdueCountCS.SetFont(OverdueSimpleF);

            BelOverdueCountCS = hssfworkbook.CreateCellStyle();
            BelOverdueCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            BelOverdueCountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            BelOverdueCountCS.BottomBorderColor = HSSFColor.BLACK.index;
            BelOverdueCountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BelOverdueCountCS.LeftBorderColor = HSSFColor.BLACK.index;
            BelOverdueCountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            BelOverdueCountCS.RightBorderColor = HSSFColor.BLACK.index;
            BelOverdueCountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            BelOverdueCountCS.TopBorderColor = HSSFColor.BLACK.index;
            BelOverdueCountCS.SetFont(OverdueSimpleF);

            CountCS = hssfworkbook.CreateCellStyle();
            CountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            CountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CountCS.BottomBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            CountCS.LeftBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CountCS.RightBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CountCS.TopBorderColor = HSSFColor.BLACK.index;
            CountCS.SetFont(SimpleF);

            BelCountCS = hssfworkbook.CreateCellStyle();
            BelCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            BelCountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            BelCountCS.BottomBorderColor = HSSFColor.BLACK.index;
            BelCountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BelCountCS.LeftBorderColor = HSSFColor.BLACK.index;
            BelCountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            BelCountCS.RightBorderColor = HSSFColor.BLACK.index;
            BelCountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            BelCountCS.TopBorderColor = HSSFColor.BLACK.index;
            BelCountCS.SetFont(SimpleF);

            CurrencyCS = hssfworkbook.CreateCellStyle();
            CurrencyCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            CurrencyCS.SetFont(CurrencyF);

            HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            HeaderCS.SetFont(HeaderF);

            SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCS.SetFont(SimpleF);

            TotalCS = hssfworkbook.CreateCellStyle();
            TotalCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalCS.SetFont(CurrencyF);

            TotalCountCS = hssfworkbook.CreateCellStyle();
            TotalCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ### ##0.00");
            TotalCountCS.SetFont(TotalCountF);

            BelTotalCountCS = hssfworkbook.CreateCellStyle();
            BelTotalCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ### ##0");
            BelTotalCountCS.SetFont(TotalCountF);

            int pos = 1;
            DisplayIndex = 0;
            HSSFCell Cell1;
            Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
            Cell1.SetCellValue("Отчет по фирме ЗОВ-Профиль");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Клиент");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Дата отгрузки");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("№ ТТН");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Сумма");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Валюта");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Крайний срок");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Дни");
            Cell1.CellStyle = HeaderCS;

            pos = 1;
            DisplayIndex = 0;
            Cell1 = sheet2.CreateRow(pos++).CreateCell(0);
            Cell1.SetCellValue("Отчет по фирме ЗОВ-ТПС");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Клиент");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Дата отгрузки");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("№ ТТН");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Сумма");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Валюта");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Крайний срок");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Дни");
            Cell1.CellStyle = HeaderCS;

        }

        public void SaveOpenReport1()
        {
            string FileName = "Отчет по ВС";
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

            System.Diagnostics.Process.Start(file.FullName);
        }

        public void ZOVProfilReport(DataTable ReportTable, string Currency, decimal TotalDispatchSum, ref int RowIndex)
        {
            if (ReportTable.Rows.Count == 0)
                return;
            int DisplayIndex = 0;

            HSSFCell Cell1;
            int pos = RowIndex;

            pos++;

            Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue(Currency);
            Cell1.CellStyle = CurrencyCS;
            pos++;

            for (int i = 0; i < ReportTable.Rows.Count; i++)
            {
                DisplayIndex = 0;
                if (Convert.ToBoolean(ReportTable.Rows[i]["Overdue"]))
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["ClientName"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["DispatchDateTime"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["WayBill"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DispatchSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelOverdueCountCS;
                    //else
                    Cell1.CellStyle = OverdueCountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["CurrencyType"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["Deadline"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["Deadline"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["TotalDays"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(ReportTable.Rows[i]["TotalDays"]));
                    Cell1.CellStyle = OverdueSimpleCS;
                }
                else
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["ClientName"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["DispatchDateTime"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["WayBill"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DispatchSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelCountCS;
                    //else
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["CurrencyType"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["Deadline"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["Deadline"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["TotalDays"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(ReportTable.Rows[i]["TotalDays"]));
                    Cell1.CellStyle = SimpleCS;
                }
                pos++;
            }

            Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("Итого:");
            Cell1.CellStyle = TotalCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(3);
            Cell1.SetCellValue(Convert.ToDouble(TotalDispatchSum));
            //if (Currency == "BYN")
            //    Cell1.CellStyle = BelTotalCountCS;
            //else
            Cell1.CellStyle = TotalCountCS;

            RowIndex = pos++;
        }

        public void ZOVTPSReport(DataTable ReportTable, string Currency, decimal TotalDispatchSum, ref int RowIndex)
        {
            if (ReportTable.Rows.Count == 0)
                return;
            int DisplayIndex = 0;

            HSSFCell Cell1;
            int pos = RowIndex;

            pos++;

            Cell1 = sheet2.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue(Currency);
            Cell1.CellStyle = CurrencyCS;
            pos++;

            for (int i = 0; i < ReportTable.Rows.Count; i++)
            {
                DisplayIndex = 0;
                if (Convert.ToBoolean(ReportTable.Rows[i]["Overdue"]))
                {
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["ClientName"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["DispatchDateTime"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["WayBill"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DispatchSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelOverdueCountCS;
                    //else
                    Cell1.CellStyle = OverdueCountCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["CurrencyType"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["Deadline"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["Deadline"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["TotalDays"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(ReportTable.Rows[i]["TotalDays"]));
                    Cell1.CellStyle = OverdueSimpleCS;
                }
                else
                {
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["ClientName"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["DispatchDateTime"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["WayBill"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DispatchSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelCountCS;
                    //else
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["CurrencyType"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["Deadline"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["Deadline"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["TotalDays"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(ReportTable.Rows[i]["TotalDays"]));
                    Cell1.CellStyle = SimpleCS;
                }
                pos++;
            }

            Cell1 = sheet2.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("Итого:");
            Cell1.CellStyle = TotalCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(3);
            Cell1.SetCellValue(Convert.ToDouble(TotalDispatchSum));
            //if (Currency == "BYN")
            //    Cell1.CellStyle = BelTotalCountCS;
            //else
            Cell1.CellStyle = TotalCountCS;

            RowIndex = pos++;
        }

        public static string ReplaceEscape(string str)
        {
            str = str.Replace("'", "''");
            return str;
        }
    }


    public class DelayOfPaymentReportToExcel
    {
        HSSFWorkbook hssfworkbook;
        HSSFSheet sheet1;
        HSSFSheet sheet2;

        HSSFFont CurrencyF;
        HSSFFont HeaderF;
        HSSFFont OverdueSimpleF;
        HSSFFont SimpleF;
        HSSFFont TotalCountF;
        HSSFCellStyle CountCS;
        HSSFCellStyle BelCountCS;
        HSSFCellStyle CurrencyCS;
        HSSFCellStyle HeaderCS;
        HSSFCellStyle OverdueCountCS;
        HSSFCellStyle BelOverdueCountCS;
        HSSFCellStyle OverdueSimpleCS;
        HSSFCellStyle SimpleCS;
        HSSFCellStyle TotalCS;
        HSSFCellStyle TotalCountCS;
        HSSFCellStyle BelTotalCountCS;

        public DelayOfPaymentReportToExcel()
        {
            Create();
        }

        private void Create()
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            sheet1 = hssfworkbook.CreateSheet("ЗОВ-Профиль");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int DisplayIndex = 0;
            sheet1.SetColumnWidth(DisplayIndex++, 40 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 18 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 15 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 20 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 22 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 10 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 15 * 256);

            sheet2 = hssfworkbook.CreateSheet("ЗОВ-ТПС");
            sheet2.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet2.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet2.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet2.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet2.SetMargin(HSSFSheet.BottomMargin, (double).20);

            DisplayIndex = 0;
            sheet2.SetColumnWidth(DisplayIndex++, 40 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 18 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 15 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 20 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 22 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 10 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 15 * 256);

            CurrencyF = hssfworkbook.CreateFont();
            CurrencyF.FontHeightInPoints = 11;
            CurrencyF.Boldweight = 10 * 256;
            CurrencyF.FontName = "Times New Roman";

            HeaderF = hssfworkbook.CreateFont();
            HeaderF.FontHeightInPoints = 12;
            HeaderF.Boldweight = 10 * 256;
            HeaderF.FontName = "Times New Roman";

            OverdueSimpleF = hssfworkbook.CreateFont();
            OverdueSimpleF.Color = HSSFColor.RED.index;
            OverdueSimpleF.FontHeightInPoints = 11;
            OverdueSimpleF.FontName = "Times New Roman";

            SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 11;
            SimpleF.FontName = "Times New Roman";

            TotalCountF = hssfworkbook.CreateFont();
            TotalCountF.FontHeightInPoints = 10;
            TotalCountF.Boldweight = 10 * 256;
            TotalCountF.FontName = "Times New Roman";
            TotalCountF.Underline = (byte)HSSFFontFormatting.U_SINGLE;
            TotalCountF.IsItalic = true;

            OverdueSimpleCS = hssfworkbook.CreateCellStyle();
            OverdueSimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            OverdueSimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            OverdueSimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            OverdueSimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            OverdueSimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            OverdueSimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            OverdueSimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            OverdueSimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            OverdueSimpleCS.SetFont(OverdueSimpleF);

            OverdueCountCS = hssfworkbook.CreateCellStyle();
            OverdueCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            OverdueCountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            OverdueCountCS.BottomBorderColor = HSSFColor.BLACK.index;
            OverdueCountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            OverdueCountCS.LeftBorderColor = HSSFColor.BLACK.index;
            OverdueCountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            OverdueCountCS.RightBorderColor = HSSFColor.BLACK.index;
            OverdueCountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            OverdueCountCS.TopBorderColor = HSSFColor.BLACK.index;
            OverdueCountCS.SetFont(OverdueSimpleF);

            BelOverdueCountCS = hssfworkbook.CreateCellStyle();
            BelOverdueCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            BelOverdueCountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            BelOverdueCountCS.BottomBorderColor = HSSFColor.BLACK.index;
            BelOverdueCountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BelOverdueCountCS.LeftBorderColor = HSSFColor.BLACK.index;
            BelOverdueCountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            BelOverdueCountCS.RightBorderColor = HSSFColor.BLACK.index;
            BelOverdueCountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            BelOverdueCountCS.TopBorderColor = HSSFColor.BLACK.index;
            BelOverdueCountCS.SetFont(OverdueSimpleF);

            CountCS = hssfworkbook.CreateCellStyle();
            CountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            CountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CountCS.BottomBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            CountCS.LeftBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CountCS.RightBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CountCS.TopBorderColor = HSSFColor.BLACK.index;
            CountCS.SetFont(SimpleF);

            BelCountCS = hssfworkbook.CreateCellStyle();
            BelCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            BelCountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            BelCountCS.BottomBorderColor = HSSFColor.BLACK.index;
            BelCountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BelCountCS.LeftBorderColor = HSSFColor.BLACK.index;
            BelCountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            BelCountCS.RightBorderColor = HSSFColor.BLACK.index;
            BelCountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            BelCountCS.TopBorderColor = HSSFColor.BLACK.index;
            BelCountCS.SetFont(SimpleF);

            CurrencyCS = hssfworkbook.CreateCellStyle();
            CurrencyCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            CurrencyCS.SetFont(CurrencyF);

            HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            HeaderCS.SetFont(HeaderF);

            SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCS.SetFont(SimpleF);

            TotalCS = hssfworkbook.CreateCellStyle();
            TotalCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalCS.SetFont(CurrencyF);

            TotalCountCS = hssfworkbook.CreateCellStyle();
            TotalCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ### ##0.00");
            TotalCountCS.SetFont(TotalCountF);

            BelTotalCountCS = hssfworkbook.CreateCellStyle();
            BelTotalCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ### ##0");
            BelTotalCountCS.SetFont(TotalCountF);

            int pos = 1;
            DisplayIndex = 0;
            HSSFCell Cell1;
            Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
            Cell1.SetCellValue("Отчет по фирме ЗОВ-Профиль");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Клиент");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Дата отгрузки");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("№ ТТН");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Сумма накладной");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Долг по накладной");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Валюта");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Крайний срок");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Дни");
            Cell1.CellStyle = HeaderCS;

            pos = 1;
            DisplayIndex = 0;
            Cell1 = sheet2.CreateRow(pos++).CreateCell(0);
            Cell1.SetCellValue("Отчет по фирме ЗОВ-ТПС");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Клиент");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Дата отгрузки");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("№ ТТН");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Сумма накладной");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Долг по накладной");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Валюта");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Крайний срок");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Дни");
            Cell1.CellStyle = HeaderCS;

        }

        public void SaveOpenReport2()
        {
            string FileName = "Отчет по отсрочке";
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

            System.Diagnostics.Process.Start(file.FullName);
        }

        public void ZOVProfilReportDelayOfpayment(DataTable ReportTable, string Currency, decimal TotalDispatchSum, ref int RowIndex)
        {
            if (ReportTable.Rows.Count == 0)
                return;
            int DisplayIndex = 0;

            HSSFCell Cell1;
            int pos = RowIndex;

            pos++;

            Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue(Currency);
            Cell1.CellStyle = CurrencyCS;
            pos++;

            for (int i = 0; i < ReportTable.Rows.Count; i++)
            {
                DisplayIndex = 0;
                if (Convert.ToBoolean(ReportTable.Rows[i]["Overdue"]))
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["ClientName"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["DispatchDateTime"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["WayBill"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DispatchSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelOverdueCountCS;
                    //else
                    Cell1.CellStyle = OverdueCountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DebtSum"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DebtSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelOverdueCountCS;
                    //else
                    Cell1.CellStyle = OverdueCountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["CurrencyType"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["Deadline"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["Deadline"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["TotalDays"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(ReportTable.Rows[i]["TotalDays"]));
                    Cell1.CellStyle = OverdueSimpleCS;
                }
                else
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["ClientName"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["DispatchDateTime"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["WayBill"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DispatchSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelCountCS;
                    //else
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DebtSum"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DebtSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelCountCS;
                    //else
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["CurrencyType"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["Deadline"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["Deadline"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["TotalDays"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(ReportTable.Rows[i]["TotalDays"]));
                    Cell1.CellStyle = SimpleCS;
                }
                pos++;
            }

            Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("Итого:");
            Cell1.CellStyle = TotalCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(3);
            Cell1.SetCellValue(Convert.ToDouble(TotalDispatchSum));
            //if (Currency == "BYN")
            //    Cell1.CellStyle = BelTotalCountCS;
            //else
            Cell1.CellStyle = TotalCountCS;

            RowIndex = pos++;
        }

        public void ZOVTPSReportDelayOfpayment(DataTable ReportTable, string Currency, decimal TotalDispatchSum, ref int RowIndex)
        {
            if (ReportTable.Rows.Count == 0)
                return;
            int DisplayIndex = 0;

            HSSFCell Cell1;
            int pos = RowIndex;

            pos++;

            Cell1 = sheet2.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue(Currency);
            Cell1.CellStyle = CurrencyCS;
            pos++;

            for (int i = 0; i < ReportTable.Rows.Count; i++)
            {
                DisplayIndex = 0;
                if (Convert.ToBoolean(ReportTable.Rows[i]["Overdue"]))
                {
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["ClientName"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["DispatchDateTime"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["WayBill"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DispatchSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelOverdueCountCS;
                    //else
                    Cell1.CellStyle = OverdueCountCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DebtSum"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DebtSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelOverdueCountCS;
                    //else
                    Cell1.CellStyle = OverdueCountCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["CurrencyType"].ToString());
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["Deadline"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["Deadline"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = OverdueSimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["TotalDays"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(ReportTable.Rows[i]["TotalDays"]));
                    Cell1.CellStyle = OverdueSimpleCS;
                }
                else
                {
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["ClientName"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DispatchDateTime"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["DispatchDateTime"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["WayBill"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DispatchSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelCountCS;
                    //else
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["DebtSum"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["DebtSum"]));
                    //if (Currency == "BYN")
                    //    Cell1.CellStyle = BelCountCS;
                    //else
                    Cell1.CellStyle = CountCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    Cell1.SetCellValue(ReportTable.Rows[i]["CurrencyType"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["Deadline"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToDateTime(ReportTable.Rows[i]["Deadline"]).ToString("dd.MM.yyyy"));
                    Cell1.CellStyle = SimpleCS;
                    Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                    if (ReportTable.Rows[i]["TotalDays"] != DBNull.Value)
                        Cell1.SetCellValue(Convert.ToInt32(ReportTable.Rows[i]["TotalDays"]));
                    Cell1.CellStyle = SimpleCS;
                }
                pos++;
            }

            Cell1 = sheet2.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("Итого:");
            Cell1.CellStyle = TotalCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(3);
            Cell1.SetCellValue(Convert.ToDouble(TotalDispatchSum));
            //if (Currency == "BYN")
            //    Cell1.CellStyle = BelTotalCountCS;
            //else
            Cell1.CellStyle = TotalCountCS;

            RowIndex = pos++;
        }

        public static string ReplaceEscape(string str)
        {
            str = str.Replace("'", "''");
            return str;
        }
    }


    public class CashReportToExcel
    {
        HSSFWorkbook hssfworkbook;
        HSSFSheet sheet1;
        HSSFSheet sheet2;
        HSSFFont CurrencyF;
        HSSFFont HeaderF;
        HSSFFont SimpleF;
        HSSFFont TotalCountF;
        HSSFCellStyle CountCS;
        HSSFCellStyle BelCountCS;
        HSSFCellStyle CurrencyCS;
        HSSFCellStyle HeaderCS;
        HSSFCellStyle SimpleCS;
        HSSFCellStyle TotalCS;
        HSSFCellStyle TotalCountCS;
        HSSFCellStyle BelTotalCountCS;

        public CashReportToExcel()
        {
            Create();
        }

        private void Create()
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            sheet1 = hssfworkbook.CreateSheet("ЗОВ-Профиль");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            int DisplayIndex = 0;
            sheet1.SetColumnWidth(DisplayIndex++, 40 * 256);
            //sheet1.SetColumnWidth(DisplayIndex++, 9 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 17 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 17 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 17 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 20 * 256);

            sheet2 = hssfworkbook.CreateSheet("ЗОВ-ТПС");
            sheet2.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet2.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet2.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet2.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet2.SetMargin(HSSFSheet.BottomMargin, (double).20);

            DisplayIndex = 0;
            sheet2.SetColumnWidth(DisplayIndex++, 40 * 256);
            //sheet2.SetColumnWidth(DisplayIndex++, 9 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 17 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 17 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 17 * 256);
            sheet2.SetColumnWidth(DisplayIndex++, 20 * 256);

            CurrencyF = hssfworkbook.CreateFont();
            CurrencyF.FontHeightInPoints = 11;
            CurrencyF.Boldweight = 10 * 256;
            CurrencyF.FontName = "Times New Roman";

            HeaderF = hssfworkbook.CreateFont();
            HeaderF.FontHeightInPoints = 12;
            HeaderF.Boldweight = 10 * 256;
            HeaderF.FontName = "Times New Roman";

            SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 11;
            SimpleF.FontName = "Times New Roman";

            TotalCountF = hssfworkbook.CreateFont();
            TotalCountF.FontHeightInPoints = 10;
            TotalCountF.Boldweight = 10 * 256;
            TotalCountF.FontName = "Times New Roman";
            TotalCountF.Underline = (byte)HSSFFontFormatting.U_SINGLE;
            TotalCountF.IsItalic = true;

            CountCS = hssfworkbook.CreateCellStyle();
            CountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0.00");
            CountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            CountCS.BottomBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            CountCS.LeftBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            CountCS.RightBorderColor = HSSFColor.BLACK.index;
            CountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            CountCS.TopBorderColor = HSSFColor.BLACK.index;
            CountCS.SetFont(SimpleF);

            BelCountCS = hssfworkbook.CreateCellStyle();
            BelCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ##0");
            BelCountCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            BelCountCS.BottomBorderColor = HSSFColor.BLACK.index;
            BelCountCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            BelCountCS.LeftBorderColor = HSSFColor.BLACK.index;
            BelCountCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            BelCountCS.RightBorderColor = HSSFColor.BLACK.index;
            BelCountCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            BelCountCS.TopBorderColor = HSSFColor.BLACK.index;
            BelCountCS.SetFont(SimpleF);

            CurrencyCS = hssfworkbook.CreateCellStyle();
            CurrencyCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            CurrencyCS.SetFont(CurrencyF);

            HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            HeaderCS.SetFont(HeaderF);

            SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCS.SetFont(SimpleF);

            TotalCS = hssfworkbook.CreateCellStyle();
            TotalCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            TotalCS.SetFont(CurrencyF);

            TotalCountCS = hssfworkbook.CreateCellStyle();
            TotalCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ### ##0.00");
            TotalCountCS.SetFont(TotalCountF);

            BelTotalCountCS = hssfworkbook.CreateCellStyle();
            BelTotalCountCS.DataFormat = hssfworkbook.CreateDataFormat().GetFormat("### ### ### ##0");
            BelTotalCountCS.SetFont(TotalCountF);

            int pos = 1;
            DisplayIndex = 0;
            HSSFCell Cell1;
            Cell1 = sheet1.CreateRow(pos++).CreateCell(0);
            Cell1.SetCellValue("Отчет по фирме ЗОВ-Профиль");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Клиент");
            Cell1.CellStyle = HeaderCS;

            //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            //Cell1.SetCellValue("Валюта");
            //Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Нач. сальдо");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Отгрузка");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Поступление");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Кон. сальдо");
            Cell1.CellStyle = HeaderCS;

            pos = 1;
            DisplayIndex = 0;
            Cell1 = sheet2.CreateRow(pos++).CreateCell(0);
            Cell1.SetCellValue("Отчет по фирме ЗОВ-ТПС");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Клиент");
            Cell1.CellStyle = HeaderCS;

            //Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            //Cell1.SetCellValue("Валюта");
            //Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Нач. сальдо");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Отгрузка");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Поступление");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Кон. сальдо");
            Cell1.CellStyle = HeaderCS;
        }

        public void SaveOpenReport1()
        {
            string FileName = "Отчет по ВС";
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

            System.Diagnostics.Process.Start(file.FullName);
        }

        public void SaveOpenReport2()
        {
            string FileName = "Отчет по ДС";
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

            System.Diagnostics.Process.Start(file.FullName);
        }

        public void ZOVProfilReport(DataTable ReportTable, string Currency, decimal TotalOpeningBalance, decimal TotalDispatchSum, decimal TotalIncomeSum, decimal TotalClosingBalance, ref int RowIndex)
        {
            if (ReportTable.Rows.Count == 0)
                return;
            int DisplayIndex = 0;

            HSSFCell Cell1;
            int pos = RowIndex;

            pos++;

            Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue(Currency);
            Cell1.CellStyle = CurrencyCS;
            pos++;

            for (int i = 0; i < ReportTable.Rows.Count; i++)
            {
                DisplayIndex = 0;
                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(ReportTable.Rows[i]["ClientName"].ToString());
                Cell1.CellStyle = SimpleCS;
                //Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                //Cell1.SetCellValue(ReportTable.Rows[i]["CurrencyType"].ToString());
                //Cell1.CellStyle = SimpleCS;
                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["OpeningBalance"]));
                //if (Currency == "BYN")
                //    Cell1.CellStyle = BelCountCS;
                //else
                Cell1.CellStyle = CountCS;
                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["TotalDispatchSum"]));
                //if (Currency == "BYN")
                //    Cell1.CellStyle = BelCountCS;
                //else
                Cell1.CellStyle = CountCS;
                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["TotalIncomeSum"]));
                //if (Currency == "BYN")
                //    Cell1.CellStyle = BelCountCS;
                //else
                Cell1.CellStyle = CountCS;
                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["ClosingBalance"]));
                //if (Currency == "BYN")
                //    Cell1.CellStyle = BelCountCS;
                //else
                Cell1.CellStyle = CountCS;
                pos++;
            }

            Cell1 = sheet1.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("Итого:");
            Cell1.CellStyle = TotalCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(1);
            Cell1.SetCellValue(Convert.ToDouble(TotalOpeningBalance));
            //if (Currency == "BYN")
            //    Cell1.CellStyle = BelTotalCountCS;
            //else
            Cell1.CellStyle = TotalCountCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(2);
            Cell1.SetCellValue(Convert.ToDouble(TotalDispatchSum));
            //if (Currency == "BYN")
            //    Cell1.CellStyle = BelTotalCountCS;
            //else
            Cell1.CellStyle = TotalCountCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(3);
            Cell1.SetCellValue(Convert.ToDouble(TotalIncomeSum));
            //if (Currency == "BYN")
            //    Cell1.CellStyle = BelTotalCountCS;
            //else
            Cell1.CellStyle = TotalCountCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(4);
            Cell1.SetCellValue(Convert.ToDouble(TotalClosingBalance));
            //if (Currency == "BYN")
            //    Cell1.CellStyle = BelTotalCountCS;
            //else
            Cell1.CellStyle = TotalCountCS;

            RowIndex = pos++;
        }

        public void ZOVTPSReport(DataTable ReportTable, string Currency, decimal TotalOpeningBalance, decimal TotalDispatchSum, decimal TotalIncomeSum, decimal TotalClosingBalance, ref int RowIndex)
        {
            if (ReportTable.Rows.Count == 0)
                return;
            int DisplayIndex = 0;

            HSSFCell Cell1;
            int pos = RowIndex;

            pos++;

            Cell1 = sheet2.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue(Currency);
            Cell1.CellStyle = CurrencyCS;
            pos++;

            for (int i = 0; i < ReportTable.Rows.Count; i++)
            {
                DisplayIndex = 0;
                Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(ReportTable.Rows[i]["ClientName"].ToString());
                Cell1.CellStyle = SimpleCS;
                //Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                //Cell1.SetCellValue(ReportTable.Rows[i]["CurrencyType"].ToString());
                //Cell1.CellStyle = SimpleCS;
                Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["OpeningBalance"]));
                //if (Currency == "BYN")
                //    Cell1.CellStyle = BelCountCS;
                //else
                Cell1.CellStyle = CountCS;
                Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["TotalDispatchSum"]));
                //if (Currency == "BYN")
                //    Cell1.CellStyle = BelCountCS;
                //else
                Cell1.CellStyle = CountCS;
                Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["TotalIncomeSum"]));
                //if (Currency == "BYN")
                //    Cell1.CellStyle = BelCountCS;
                //else
                Cell1.CellStyle = CountCS;
                Cell1 = sheet2.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(Convert.ToDouble(ReportTable.Rows[i]["ClosingBalance"]));
                //if (Currency == "BYN")
                //    Cell1.CellStyle = BelCountCS;
                //else
                Cell1.CellStyle = CountCS;
                pos++;
            }

            Cell1 = sheet2.CreateRow(pos).CreateCell(0);
            Cell1.SetCellValue("Итого:");
            Cell1.CellStyle = TotalCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(1);
            Cell1.SetCellValue(Convert.ToDouble(TotalOpeningBalance));
            //if (Currency == "BYN")
            //    Cell1.CellStyle = BelTotalCountCS;
            //else
            Cell1.CellStyle = TotalCountCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(2);
            Cell1.SetCellValue(Convert.ToDouble(TotalDispatchSum));
            //if (Currency == "BYN")
            //    Cell1.CellStyle = BelTotalCountCS;
            //else
            Cell1.CellStyle = TotalCountCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(3);
            Cell1.SetCellValue(Convert.ToDouble(TotalIncomeSum));
            //if (Currency == "BYN")
            //    Cell1.CellStyle = BelTotalCountCS;
            //else
            Cell1.CellStyle = TotalCountCS;

            Cell1 = sheet2.CreateRow(pos).CreateCell(4);
            Cell1.SetCellValue(Convert.ToDouble(TotalClosingBalance));
            //if (Currency == "BYN")
            //    Cell1.CellStyle = BelTotalCountCS;
            //else
            Cell1.CellStyle = TotalCountCS;

            RowIndex = pos++;
        }

        public static string ReplaceEscape(string str)
        {
            str = str.Replace("'", "''");
            return str;
        }
    }
}
