using ComponentFactory.Krypton.Toolkit;

using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.UserModel.Contrib;
using NPOI.HSSF.Util;

using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Infinium.Modules.ZOV.ClientErrors
{
    public class ClientErrorsWriteOffs
    {
        DataTable SearchMainOrdersDT;
        DataTable ClientErrorsDT;
        DataTable ClientsDT;
        DataTable ClientsColumnDT;
        DataTable DocNumbersDT;

        BindingSource SearchPartDocNumberBS;

        BindingSource ClientErrorsBS;
        BindingSource ClientsBS;
        BindingSource ClientsColumnBS;
        BindingSource DocNumbersBS;
        public DataGridViewComboBoxColumn ClientColumn = null;
        public KryptonDataGridViewDateTimePickerColumn DateTimeColumn = null;

        public ClientErrorsWriteOffs()
        {

        }

        public bool HasCurrentClient
        {
            get { return ClientsBS.Current != null; }
        }

        public bool HasCurrentDocNumber
        {
            get { return DocNumbersBS.Current != null; }
        }

        public BindingSource ClientErrorsList
        {
            get { return ClientErrorsBS; }
        }

        public BindingSource ClientsList
        {
            get { return ClientsBS; }
        }

        public BindingSource DocNumbersList
        {
            get { return DocNumbersBS; }
        }

        public BindingSource SearchPartDocNumberList
        {
            get { return SearchPartDocNumberBS; }
        }

        public void Initialize()
        {
            Create();
            FillTables();
            DataBinding();
            CreateClientColumn();
        }

        private void Create()
        {
            SearchMainOrdersDT = new DataTable();

            ClientErrorsDT = new DataTable();
            ClientsDT = new DataTable();
            ClientsColumnDT = new DataTable();
            DocNumbersDT = new DataTable();

            SearchPartDocNumberBS = new BindingSource();

            ClientErrorsBS = new BindingSource();
            ClientsBS = new BindingSource();
            ClientsColumnBS = new BindingSource();
            DocNumbersBS = new BindingSource();
        }

        private void FillTables()
        {
            UpdateClientErrors(true, null, null);
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients ORDER BY ClientName",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDT);
                DA.Fill(ClientsColumnDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 ClientID, DocNumber, MainOrderID FROM MainOrders ORDER BY DocNumber",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DocNumbersDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 MainOrderID, DocNumber FROM ClientErrorsWriteOffs",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(SearchMainOrdersDT);
            }
        }

        private void DataBinding()
        {
            ClientErrorsBS.DataSource = ClientErrorsDT;
            ClientsBS.DataSource = ClientsDT;
            ClientsColumnBS.DataSource = ClientsColumnDT;
            DocNumbersBS.DataSource = DocNumbersDT;

            SearchPartDocNumberBS.DataSource = new DataView(SearchMainOrdersDT);
        }

        private void CreateClientColumn()
        {
            ClientColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ClientColumn",
                HeaderText = "Клиент",
                DataPropertyName = "ClientID",
                DataSource = ClientsColumnDT,
                ValueMember = "ClientID",
                DisplayMember = "ClientName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            DateTimeColumn = new KryptonDataGridViewDateTimePickerColumn()
            {
                //DateTimeColumn.CalendarTodayDate = new System.DateTime(DateTime.Now);
                Checked = false,
                DataPropertyName = "OrderDate",
                HeaderText = "Дата",
                Name = "DateTimeColumn",
                Width = 100,
                Format = DateTimePickerFormat.Short
            };
        }

        public void SearchPartDocNumber(string DocText)
        {
            //string Search = string.Format("[DocNumber] LIKE '%" + DocText + "%'");

            //ClientErrorsBS.Filter = Search;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, DocNumber FROM ClientErrorsWriteOffs WHERE DocNumber LIKE '%" + DocText + "%'",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                SearchMainOrdersDT.Clear();
                DA.Fill(SearchMainOrdersDT);
            }
        }

        public void SearchDocNumber(int MainOrderID)
        {
            DataRow[] Rows = ClientErrorsDT.Select("MainOrderID = " + MainOrderID);
            if (Rows.Count() < 1)
                return;
            ClientErrorsBS.Position = ClientErrorsBS.Find("MainOrderID", MainOrderID);
        }

        public void AddClientError(int ClientID, int MainOrderID, string DocNumber, string Product, DateTime OrderDate, decimal Cost, string Reason)
        {
            DataRow NewRow = ClientErrorsDT.NewRow();
            NewRow["ClientID"] = ClientID;
            NewRow["MainOrderID"] = MainOrderID;
            NewRow["DocNumber"] = DocNumber;
            if (Product.Length > 0)
                NewRow["Product"] = Product;
            NewRow["OrderDate"] = OrderDate;
            NewRow["Cost"] = Cost;
            if (Reason.Length > 0)
                NewRow["Reason"] = Reason;
            NewRow["Created"] = Security.GetCurrentDate();
            ClientErrorsDT.Rows.Add(NewRow);
        }

        public void DeleteClientError()
        {
            if (ClientErrorsBS.Current != null)
            {
                ClientErrorsBS.RemoveCurrent();
            }
        }

        public void SaveClientErrors()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM ClientErrorsWriteOffs",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    if (ClientErrorsDT.GetChanges() != null)
                    {
                        DataTable DT = ClientErrorsDT.GetChanges();
                        foreach (DataRow item in DT.Rows)
                        {
                            if (item.RowState == DataRowState.Deleted)
                                continue;
                            if (item["Created"] == DBNull.Value)
                                item["Created"] = Security.GetCurrentDate();
                        }
                        DA.Update(DT);
                        DT.Dispose();
                    }
                }
            }
        }

        public void UpdateClientErrors(bool bShowAllErrors, object DateFrom, object DateTo)
        {
            string SelectCommand = @"SELECT * FROM ClientErrorsWriteOffs ORDER BY Created";
            if (!bShowAllErrors)
                SelectCommand = @"SELECT * FROM ClientErrorsWriteOffs WHERE CAST(OrderDate AS Date) >= '" + Convert.ToDateTime(DateFrom).ToString("yyyy-MM-dd") +
                   "' AND CAST(OrderDate AS Date) <= '" + Convert.ToDateTime(DateTo).ToString("yyyy-MM-dd") + "' ORDER BY Created";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                ClientErrorsDT.Clear();
                DA.Fill(ClientErrorsDT);
            }
        }

        public void GetDocNumbers(int ClientID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT DocNumber, MainOrderID FROM MainOrders" +
                " WHERE ClientID = " + ClientID + " ORDER BY DocNumber",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DocNumbersDT.Clear();
                DA.Fill(DocNumbersDT);
            }
        }

        public void MoveToClientError(int ClientErrorsWriteOffID)
        {
            ClientErrorsBS.Position = ClientErrorsBS.Find("ClientErrorsWriteOffID", ClientErrorsWriteOffID);
        }
    }

    public class AssemblyOrders
    {
        DataTable AssemblyOrdersDT;
        DataTable ClientsDT;
        DataTable ClientsColumnDT;
        DataTable DocNumbersDT;

        BindingSource AssemblyOrdersBS;
        BindingSource ClientsBS;
        BindingSource ClientsColumnBS;
        BindingSource DocNumbersBS;

        public AssemblyOrders()
        {

        }

        public bool HasCurrentClient
        {
            get { return ClientsBS.Current != null; }
        }

        public bool HasDocNumbers
        {
            get { return DocNumbersBS.Current != null; }
        }

        public BindingSource AssemblyOrdersList
        {
            get { return AssemblyOrdersBS; }
        }

        public BindingSource ClientsList
        {
            get { return ClientsBS; }
        }

        public BindingSource DocNumbersList
        {
            get { return DocNumbersBS; }
        }

        public void Initialize()
        {
            Create();
            FillTables();
            DataBinding();
        }

        private void Create()
        {
            AssemblyOrdersDT = new DataTable();
            //AssemblyOrdersDT.ColumnChanged += new DataColumnChangeEventHandler(AssemblyOrdersDT_ColumnChanged);
            ClientsDT = new DataTable();
            ClientsColumnDT = new DataTable();
            DocNumbersDT = new DataTable();
            AssemblyOrdersBS = new BindingSource();
            ClientsBS = new BindingSource();
            ClientsColumnBS = new BindingSource();
            DocNumbersBS = new BindingSource();
        }

        void AssemblyOrdersDT_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            string s = e.Row["PaymentDate"] + " " + e.Row["PaymentDate", DataRowVersion.Current];
        }

        private void FillTables()
        {
            UpdateAssemblyOrders();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients ORDER BY ClientName",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDT);
                DA.Fill(ClientsColumnDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 ClientID, DocNumber, MainOrderID FROM MainOrders ORDER BY DocNumber",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DocNumbersDT);
            }
        }

        private void DataBinding()
        {
            AssemblyOrdersBS.DataSource = AssemblyOrdersDT;
            ClientsBS.DataSource = ClientsDT;
            ClientsColumnBS.DataSource = ClientsColumnDT;
            DocNumbersBS.DataSource = DocNumbersDT;
        }

        public DataGridViewComboBoxColumn ClientColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ClientColumn",
                    HeaderText = "Клиент",
                    DataPropertyName = "ClientID",
                    DataSource = ClientsColumnDT,
                    ValueMember = "ClientID",
                    DisplayMember = "ClientName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public KryptonDataGridViewDateTimePickerColumn DispatchDateColumn
        {
            get
            {
                KryptonDataGridViewDateTimePickerColumn Column = new KryptonDataGridViewDateTimePickerColumn();
                Column.DefaultCellStyle.Format = "dd.MM.yyyy";
                Column.Checked = false;
                Column.DataPropertyName = "DispatchDate";
                Column.HeaderText = "Дата отгрузки";
                Column.Name = "DispatchDateColumn";
                Column.Width = 100;
                Column.Format = DateTimePickerFormat.Short;

                return Column;
            }
        }

        public KryptonDataGridViewDateTimePickerColumn PaymentDateColumn
        {
            get
            {
                KryptonDataGridViewDateTimePickerColumn Column = new KryptonDataGridViewDateTimePickerColumn();
                //DateTimeColumn.CalendarTodayDate = new System.DateTime(DateTime.Now);
                Column.DefaultCellStyle.Format = "dd.MM.yyyy";
                Column.Checked = false;
                Column.DataPropertyName = "PaymentDate";
                Column.HeaderText = "Дата оплаты";
                Column.Name = "PaymentDateColumn";
                Column.Width = 100;
                Column.Format = DateTimePickerFormat.Short;

                return Column;
            }
        }

        public void AddAssemblyOrder(int ClientID, int MainOrderID, string DocNumber, DateTime DispatchDate, string AssemblyNumber, decimal Cost, string Notes)
        {
            DataRow NewRow = AssemblyOrdersDT.NewRow();
            NewRow["ClientID"] = ClientID;
            NewRow["MainOrderID"] = MainOrderID;
            NewRow["DocNumber"] = DocNumber;
            NewRow["DispatchDate"] = DispatchDate;
            NewRow["AssemblyNumber"] = AssemblyNumber;
            NewRow["Cost"] = Cost;
            if (Notes.Length > 0)
                NewRow["Notes"] = Notes;
            NewRow["Created"] = Security.GetCurrentDate();
            AssemblyOrdersDT.Rows.Add(NewRow);
        }

        public void DeleteAssemblyOrder()
        {
            if (AssemblyOrdersBS.Current != null)
            {
                AssemblyOrdersBS.RemoveCurrent();
            }
        }

        public void SaveAssemblyOrders()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM AssemblyOrders",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    if (AssemblyOrdersDT.GetChanges() != null)
                    {
                        DataTable DT = AssemblyOrdersDT.GetChanges();
                        foreach (DataRow item in DT.Rows)
                        {
                            DataRowState State = item.RowState;
                            switch (State)
                            {
                                case DataRowState.Added:
                                    if (item["Created"] == DBNull.Value)
                                        item["Created"] = Security.GetCurrentDate();
                                    ToAssemblyOrder(Convert.ToInt32(item["MainOrderID"]));
                                    break;
                                case DataRowState.Deleted:
                                    break;
                                case DataRowState.Detached:
                                    break;
                                case DataRowState.Modified:
                                    object CurrentPaymentDate = item["PaymentDate", DataRowVersion.Current];
                                    object OriginalPaymentDate = item["PaymentDate", DataRowVersion.Original];
                                    if (OriginalPaymentDate != CurrentPaymentDate)
                                    {
                                        item["Active"] = false;
                                        DispatchAssemblyOrder(Convert.ToInt32(item["MainOrderID"]), Convert.ToDateTime(item["PaymentDate"]));
                                    }
                                    break;
                                case DataRowState.Unchanged:
                                    break;
                                default:
                                    break;
                            }
                        }
                        DA.Update(DT);
                        DT.Dispose();
                    }
                }
            }
        }

        public void UpdateAssemblyOrders()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM AssemblyOrders ORDER BY Created",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                AssemblyOrdersDT.Clear();
                DA.Fill(AssemblyOrdersDT);
            }
        }

        public void GetDocNumbers(int ClientID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT DocNumber, MainOrderID FROM MainOrders" +
                " WHERE ClientID = " + ClientID + " ORDER BY DocNumber",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DocNumbersDT.Clear();
                DA.Fill(DocNumbersDT);
            }
        }

        private int GetMegaOrderByDispatchDate(DateTime DispatchDate)
        {
            int MegaOrderID = 0;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID FROM MegaOrders WHERE DispatchDate='" +
                Convert.ToDateTime(DispatchDate).ToString("yyyy-MM-dd") + "'", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);
                    }
                }
            }
            return MegaOrderID;
        }

        public void ToAssemblyOrder(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, ToAssembly FROM MainOrders WHERE MainOrderID=" + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["ToAssembly"] = true;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void DispatchAssemblyOrder(int MainOrderID, DateTime DispatchDate)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, MainOrderID, FromAssembly FROM MainOrders WHERE MainOrderID=" + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            int MegaOrderID = GetMegaOrderByDispatchDate(DispatchDate);
                            DT.Rows[0]["FromAssembly"] = true;
                            DT.Rows[0]["MegaOrderID"] = MegaOrderID;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void MoveToAssemblyOrder(int AssemblyOrderID)
        {
            AssemblyOrdersBS.Position = AssemblyOrdersBS.Find("AssemblyOrderID", AssemblyOrderID);
        }
    }

    public class NotPaidOrders
    {
        DataTable NotPaidOrdersDT;
        DataTable ClientsDT;
        DataTable ClientsColumnDT;
        DataTable DocNumbersDT;

        BindingSource NotPaidOrdersBS;
        BindingSource ClientsBS;
        BindingSource ClientsColumnBS;
        BindingSource DocNumbersBS;

        public NotPaidOrders()
        {

        }

        public bool HasCurrentClient
        {
            get { return ClientsBS.Current != null; }
        }

        public bool HasDocNumbers
        {
            get { return DocNumbersBS.Current != null; }
        }

        public BindingSource NotPaidOrdersList
        {
            get { return NotPaidOrdersBS; }
        }

        public BindingSource ClientsList
        {
            get { return ClientsBS; }
        }

        public BindingSource DocNumbersList
        {
            get { return DocNumbersBS; }
        }

        public void Initialize()
        {
            Create();
            FillTables();
            DataBinding();
        }

        private void Create()
        {
            NotPaidOrdersDT = new DataTable();
            //AssemblyOrdersDT.ColumnChanged += new DataColumnChangeEventHandler(AssemblyOrdersDT_ColumnChanged);
            ClientsDT = new DataTable();
            ClientsColumnDT = new DataTable();
            DocNumbersDT = new DataTable();
            NotPaidOrdersBS = new BindingSource();
            ClientsBS = new BindingSource();
            ClientsColumnBS = new BindingSource();
            DocNumbersBS = new BindingSource();
        }

        void AssemblyOrdersDT_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            string s = e.Row["PaymentDate"] + " " + e.Row["PaymentDate", DataRowVersion.Current];
        }

        private void FillTables()
        {
            UpdateNotPaidOrders();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients ORDER BY ClientName",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDT);
                DA.Fill(ClientsColumnDT);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 ClientID, DocNumber, MainOrderID FROM MainOrders ORDER BY DocNumber",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DocNumbersDT);
            }
        }

        private void DataBinding()
        {
            NotPaidOrdersBS.DataSource = NotPaidOrdersDT;
            ClientsBS.DataSource = ClientsDT;
            ClientsColumnBS.DataSource = ClientsColumnDT;
            DocNumbersBS.DataSource = DocNumbersDT;
        }

        public DataGridViewComboBoxColumn ClientColumn
        {
            get
            {
                DataGridViewComboBoxColumn Column = new DataGridViewComboBoxColumn()
                {
                    Name = "ClientColumn",
                    HeaderText = "Клиент",
                    DataPropertyName = "ClientID",
                    DataSource = ClientsColumnDT,
                    ValueMember = "ClientID",
                    DisplayMember = "ClientName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return Column;
            }
        }

        public KryptonDataGridViewDateTimePickerColumn DispatchDateColumn
        {
            get
            {
                KryptonDataGridViewDateTimePickerColumn Column = new KryptonDataGridViewDateTimePickerColumn();
                Column.DefaultCellStyle.Format = "dd.MM.yyyy";
                Column.Checked = false;
                Column.DataPropertyName = "DispatchDate";
                Column.HeaderText = "Дата отгрузки";
                Column.Name = "DispatchDateColumn";
                Column.Width = 100;
                Column.Format = DateTimePickerFormat.Short;

                return Column;
            }
        }

        public KryptonDataGridViewDateTimePickerColumn PaymentDateColumn
        {
            get
            {
                KryptonDataGridViewDateTimePickerColumn Column = new KryptonDataGridViewDateTimePickerColumn();
                //DateTimeColumn.CalendarTodayDate = new System.DateTime(DateTime.Now);
                Column.DefaultCellStyle.Format = "dd.MM.yyyy";
                Column.Checked = false;
                Column.DataPropertyName = "PaymentDate";
                Column.HeaderText = "Дата оплаты";
                Column.Name = "PaymentDateColumn";
                Column.Width = 100;
                Column.Format = DateTimePickerFormat.Short;

                return Column;
            }
        }

        public void AddNotPaidOrder(int ClientID, int MainOrderID, string DocNumber, DateTime DispatchDate, decimal Cost, string Notes)
        {
            DataRow NewRow = NotPaidOrdersDT.NewRow();
            NewRow["ClientID"] = ClientID;
            NewRow["MainOrderID"] = MainOrderID;
            NewRow["DocNumber"] = DocNumber;
            NewRow["DispatchDate"] = DispatchDate;
            NewRow["Cost"] = Cost;
            if (Notes.Length > 0)
                NewRow["Notes"] = Notes;
            NewRow["Created"] = Security.GetCurrentDate();
            NotPaidOrdersDT.Rows.Add(NewRow);
        }

        public void DeleteAssemblyOrder()
        {
            if (NotPaidOrdersBS.Current != null)
            {
                NotPaidOrdersBS.RemoveCurrent();
            }
        }

        public void SaveNotPaidOrders()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM NotPaidOrders",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    if (NotPaidOrdersDT.GetChanges() != null)
                    {
                        DataTable DT = NotPaidOrdersDT.GetChanges();
                        foreach (DataRow item in DT.Rows)
                        {
                            DataRowState State = item.RowState;
                            switch (State)
                            {
                                case DataRowState.Added:
                                    if (item["Created"] == DBNull.Value)
                                        item["Created"] = Security.GetCurrentDate();
                                    //ToNotPaidOrder(Convert.ToInt32(item["MainOrderID"]));
                                    break;
                                case DataRowState.Deleted:
                                    break;
                                case DataRowState.Detached:
                                    break;
                                case DataRowState.Modified:
                                    object CurrentPaymentDate = item["PaymentDate", DataRowVersion.Current];
                                    object OriginalPaymentDate = item["PaymentDate", DataRowVersion.Original];
                                    if (OriginalPaymentDate != CurrentPaymentDate)
                                    {
                                        item["Active"] = false;
                                        //DispatchNotPaidOrder(Convert.ToInt32(item["MainOrderID"]), Convert.ToDateTime(item["PaymentDate"]));
                                    }
                                    break;
                                case DataRowState.Unchanged:
                                    break;
                                default:
                                    break;
                            }
                        }
                        DA.Update(DT);
                        DT.Dispose();
                    }
                }
            }
        }

        public void UpdateNotPaidOrders()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM NotPaidOrders ORDER BY Created",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                NotPaidOrdersDT.Clear();
                DA.Fill(NotPaidOrdersDT);
            }
        }

        public void GetDocNumbers(int ClientID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT DocNumber, MainOrderID FROM MainOrders" +
                " WHERE ClientID = " + ClientID + " ORDER BY DocNumber",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DocNumbersDT.Clear();
                DA.Fill(DocNumbersDT);
            }
        }

        private int GetMegaOrderByDispatchDate(DateTime DispatchDate)
        {
            int MegaOrderID = 0;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID FROM MegaOrders WHERE DispatchDate='" +
                Convert.ToDateTime(DispatchDate).ToString("yyyy-MM-dd") + "'", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        MegaOrderID = Convert.ToInt32(DT.Rows[0]["MegaOrderID"]);
                    }
                }
            }
            return MegaOrderID;
        }

        public void ToNotPaidOrder(int MainOrderID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, ToAssembly FROM MainOrders WHERE MainOrderID=" + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DT.Rows[0]["ToAssembly"] = true;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void DispatchNotPaidOrder(int MainOrderID, DateTime DispatchDate)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrderID, MainOrderID, FromAssembly FROM MainOrders WHERE MainOrderID=" + MainOrderID,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            int MegaOrderID = GetMegaOrderByDispatchDate(DispatchDate);
                            DT.Rows[0]["FromAssembly"] = true;
                            DT.Rows[0]["MegaOrderID"] = MegaOrderID;
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

        public void MoveToNotPaidOrder(int NotPaidOrderID)
        {
            NotPaidOrdersBS.Position = NotPaidOrdersBS.Find("NotPaidOrderID", NotPaidOrderID);
        }
    }

    public class ReportCalculations
    {
        DataTable ReportCalculationsDT;
        DataTable ReportViewDT;
        DataTable MegaOrdersDT;
        DataTable MainOrdersDT;
        DataTable FrontsOrdersDT;
        DataTable DecorOrdersDT;
        DataTable PackageDetailsDT;
        DataTable ClientErrorsWriteOffsDT;
        DataTable ToAssemblyOrdersDT;
        DataTable FromAssemblyOrdersDT;
        DataTable NotPaidOrdersDT;

        BindingSource ReportViewBS;

        public ReportCalculations()
        {

        }

        public BindingSource ReportViewList
        {
            get { return ReportViewBS; }
        }

        public void Initialize()
        {
            Create();
            FillTables();
            DataBinding();
        }

        private void Create()
        {
            ReportViewDT = new DataTable();
            ReportViewDT.Columns.Add(new DataColumn("DispatchDate", Type.GetType("System.String")));
            ReportViewDT.Columns.Add(new DataColumn("PaymentPlan", Type.GetType("System.Decimal")));
            ReportViewDT.Columns.Add(new DataColumn("DispatchedCost", Type.GetType("System.Decimal")));
            ReportViewDT.Columns.Add(new DataColumn("SummaryReport", Type.GetType("System.Decimal")));
            ReportViewDT.Columns.Add(new DataColumn("DebtsCost", Type.GetType("System.Decimal")));
            ReportViewDT.Columns.Add(new DataColumn("OtherCost", Type.GetType("System.Decimal")));
            ReportViewDT.Columns.Add(new DataColumn("NotDispatchedCost", Type.GetType("System.Decimal")));
            ReportViewDT.Columns.Add(new DataColumn("ToAssemblyCost", Type.GetType("System.Decimal")));
            ReportViewDT.Columns.Add(new DataColumn("FromAssemblyCost", Type.GetType("System.Decimal")));
            ReportViewDT.Columns.Add(new DataColumn("RefundsCost", Type.GetType("System.Decimal")));
            ReportViewDT.Columns.Add(new DataColumn("ZOVReport", Type.GetType("System.Decimal")));
            ReportViewDT.Columns.Add(new DataColumn("DeductionsCost", Type.GetType("System.Decimal")));

            ReportCalculationsDT = new DataTable();
            MegaOrdersDT = new DataTable();
            MainOrdersDT = new DataTable();
            FrontsOrdersDT = new DataTable();
            DecorOrdersDT = new DataTable();
            PackageDetailsDT = new DataTable();
            ClientErrorsWriteOffsDT = new DataTable();
            ToAssemblyOrdersDT = new DataTable();
            FromAssemblyOrdersDT = new DataTable();
            NotPaidOrdersDT = new DataTable();

            ReportViewBS = new BindingSource();
        }

        private void FillTables()
        {
            string SelectCommand = "SELECT TOP 0 MegaOrderID, DispatchDate FROM MegaOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(MegaOrdersDT);
            }
            SelectCommand = "SELECT TOP 0 MainOrderID, MegaOrderID, DocNumber, DebtDocNumber, ReorderDocNumber, ClientID, NeedCalculate, DebtTypeID, PriceTypeID, IsPrepared, IsSample, ToAssembly, FromAssembly, IsNotPaid FROM MainOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(MainOrdersDT);
            }
            SelectCommand = "SELECT TOP 0 FrontsOrdersID, MainOrderID, Cost FROM FrontsOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDT);
            }
            SelectCommand = "SELECT TOP 0 DecorOrderID, MainOrderID, Cost FROM DecorOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDT);
            }
            SelectCommand = @"SELECT TOP 0 PackageDetails.PackageID, Packages.MainOrderID, Packages.ProductType, Packages.PackageStatusID, Packages.DispatchDateTime, PackageDetails.OrderID, PackageDetails.Count FROM PackageDetails
                INNER JOIN Packages ON PackageDetails.PackageID=Packages.PackageID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PackageDetailsDT);
            }
            SelectCommand = "SELECT TOP 0 * FROM ClientErrorsWriteOffs";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(ClientErrorsWriteOffsDT);
            }
            SelectCommand = "SELECT TOP 0 * FROM AssemblyOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(ToAssemblyOrdersDT);
            }
            SelectCommand = "SELECT TOP 0 * FROM AssemblyOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FromAssemblyOrdersDT);
            }
            SelectCommand = "SELECT TOP 0 * FROM NotPaidOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(NotPaidOrdersDT);
            }
            SelectCommand = "SELECT TOP 0 * FROM MainOrders";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(ReportCalculationsDT);
            }
        }

        private void DataBinding()
        {
            ReportViewBS.DataSource = ReportViewDT;
        }

        public KryptonDataGridViewDateTimePickerColumn DispatchDateColumn
        {
            get
            {
                KryptonDataGridViewDateTimePickerColumn Column = new KryptonDataGridViewDateTimePickerColumn();
                Column.DefaultCellStyle.Format = "dd.MM.yyyy";
                Column.Checked = false;
                Column.DataPropertyName = "DispatchDate";
                Column.HeaderText = "Дата отгрузки";
                Column.Name = "DispatchDateColumn";
                Column.Width = 100;
                Column.Format = DateTimePickerFormat.Short;

                return Column;
            }
        }

        public KryptonDataGridViewDateTimePickerColumn PaymentDateColumn
        {
            get
            {
                KryptonDataGridViewDateTimePickerColumn Column = new KryptonDataGridViewDateTimePickerColumn();
                //DateTimeColumn.CalendarTodayDate = new System.DateTime(DateTime.Now);
                Column.DefaultCellStyle.Format = "dd.MM.yyyy";
                Column.Checked = false;
                Column.DataPropertyName = "PaymentDate";
                Column.HeaderText = "Дата оплаты";
                Column.Name = "PaymentDateColumn";
                Column.Width = 100;
                Column.Format = DateTimePickerFormat.Short;

                return Column;
            }
        }

        public void AddReportCalculationsRow(DateTime DispatchDate, decimal PaymentPlan, decimal DispatchedCost, decimal SummaryReport, decimal DebtsCost, decimal OtherCost,
            decimal NotDispatchedCost, decimal ToAssemblyCost, decimal FromAssemblyCost, decimal RefundsCost, decimal ZOVReport, decimal DeductionsCost)
        {
            DataRow[] rows = ReportCalculationsDT.Select("DispatchDate='" + DispatchDate.ToString("yyyy-MM-dd") + "'");
            if (rows.Count() == 0)
            {
                DataRow NewRow = ReportCalculationsDT.NewRow();
                NewRow["DispatchDate"] = DispatchDate;
                NewRow["PaymentPlan"] = PaymentPlan;
                NewRow["DispatchedCost"] = DispatchedCost;
                NewRow["SummaryReport"] = SummaryReport;
                NewRow["DebtsCost"] = DebtsCost;
                NewRow["OtherCost"] = OtherCost;
                NewRow["NotDispatchedCost"] = NotDispatchedCost;
                NewRow["ToAssemblyCost"] = ToAssemblyCost;
                NewRow["FromAssemblyCost"] = FromAssemblyCost;
                NewRow["RefundsCost"] = RefundsCost;
                NewRow["ZOVReport"] = ZOVReport;
                NewRow["DeductionsCost"] = DeductionsCost;
                ReportCalculationsDT.Rows.Add(NewRow);
            }
            else
            {
                rows[0]["DispatchDate"] = DispatchDate;
                rows[0]["PaymentPlan"] = PaymentPlan;
                rows[0]["DispatchedCost"] = DispatchedCost;
                rows[0]["SummaryReport"] = SummaryReport;
                rows[0]["DebtsCost"] = DebtsCost;
                rows[0]["OtherCost"] = OtherCost;
                rows[0]["NotDispatchedCost"] = NotDispatchedCost;
                rows[0]["ToAssemblyCost"] = ToAssemblyCost;
                rows[0]["FromAssemblyCost"] = FromAssemblyCost;
                rows[0]["RefundsCost"] = RefundsCost;
                rows[0]["ZOVReport"] = ZOVReport;
                rows[0]["DeductionsCost"] = DeductionsCost;
            }
        }

        public void AddReportViewRow(string Name, decimal PaymentPlan, decimal DispatchedCost, decimal SummaryReport, decimal DebtsCost, decimal OtherCost,
            decimal NotDispatchedCost, decimal ToAssemblyCost, decimal FromAssemblyCost, decimal RefundsCost, decimal ZOVReport, decimal DeductionsCost)
        {
            DataRow NewRow = ReportViewDT.NewRow();
            NewRow["DispatchDate"] = Name;
            NewRow["PaymentPlan"] = PaymentPlan;
            NewRow["DispatchedCost"] = DispatchedCost;
            NewRow["SummaryReport"] = SummaryReport;
            NewRow["DebtsCost"] = DebtsCost;
            NewRow["OtherCost"] = OtherCost;
            NewRow["NotDispatchedCost"] = NotDispatchedCost;
            NewRow["ToAssemblyCost"] = ToAssemblyCost;
            NewRow["FromAssemblyCost"] = FromAssemblyCost;
            NewRow["RefundsCost"] = RefundsCost;
            NewRow["ZOVReport"] = ZOVReport;
            NewRow["DeductionsCost"] = DeductionsCost;
            ReportViewDT.Rows.Add(NewRow);
        }

        public void UpdateData(DateTime StartDispatchDate, DateTime FinishDispatchDate)
        {
            ReportViewDT.Clear();
            string SelectCommand = @"SELECT MegaOrderID, CAST(DispatchDate AS Date) AS DispatchDate FROM MegaOrders
                WHERE CAST(DispatchDate AS Date)>='" + StartDispatchDate.ToString("yyyy-MM-dd") + "' AND CAST(DispatchDate AS Date)<='" + FinishDispatchDate.ToString("yyyy-MM-dd") + "'";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                MegaOrdersDT.Clear();
                DA.Fill(MegaOrdersDT);
            }
            SelectCommand = @"SELECT CAST(MegaOrders.DispatchDate AS Date) AS DispatchDate, MainOrderID, MainOrders.MegaOrderID, DocNumber, DebtDocNumber, ReorderDocNumber, ClientID, MainOrders.FrontsCost, MainOrders.DecorCost, MainOrders.OrderCost, NeedCalculate,
                DebtTypeID, PriceTypeID, IsPrepared, IsSample, DoNotDispatch, ToAssembly, FromAssembly, IsNotPaid FROM MainOrders
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID=MegaOrders.MegaOrderID AND CAST(MegaOrders.DispatchDate AS Date)>='" + StartDispatchDate.ToString("yyyy-MM-dd") + "' AND CAST(MegaOrders.DispatchDate AS Date)<='" + FinishDispatchDate.ToString("yyyy-MM-dd") + "' ORDER BY DispatchDate";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                MainOrdersDT.Clear();
                DA.Fill(MainOrdersDT);
            }
            SelectCommand = @"SELECT FrontsOrdersID, MainOrderID, Count, Cost FROM FrontsOrders
                WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders
                WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders
                WHERE CAST(DispatchDate AS Date)>='" + StartDispatchDate.ToString("yyyy-MM-dd") + "' AND CAST(DispatchDate AS Date)<='" + FinishDispatchDate.ToString("yyyy-MM-dd") + "')) ORDER BY FrontsOrders.MainOrderID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                FrontsOrdersDT.Clear();
                DA.Fill(FrontsOrdersDT);
            }
            SelectCommand = @"SELECT DecorOrderID, MainOrderID, Count, Cost FROM DecorOrders
                WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders
                WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders
                WHERE CAST(DispatchDate AS Date)>='" + StartDispatchDate.ToString("yyyy-MM-dd") + "' AND CAST(DispatchDate AS Date)<='" + FinishDispatchDate.ToString("yyyy-MM-dd") + "')) ORDER BY DecorOrders.MainOrderID";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DecorOrdersDT.Clear();
                DA.Fill(DecorOrdersDT);
            }
            SelectCommand = @"SELECT PackageDetails.PackageID, Packages.MainOrderID, Packages.ProductType, Packages.PackageStatusID, Packages.DispatchDateTime, PackageDetails.OrderID, PackageDetails.Count FROM PackageDetails
                INNER JOIN Packages ON PackageDetails.PackageID=Packages.PackageID
                WHERE Packages.MainOrderID IN (SELECT MainOrderID FROM MainOrders
                WHERE MainOrders.MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders
                WHERE CAST(DispatchDate AS Date)>='" + StartDispatchDate.ToString("yyyy-MM-dd") + "' AND CAST(DispatchDate AS Date)<='" + FinishDispatchDate.ToString("yyyy-MM-dd") + "'))";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                PackageDetailsDT.Clear();
                DA.Fill(PackageDetailsDT);
            }
            SelectCommand = @"SELECT ClientErrorsWriteOffID, ClientID, MainOrderID, DocNumber, Product, CAST(OrderDate AS Date) AS OrderDate, Cost, Reason, Created FROM ClientErrorsWriteOffs
                WHERE CAST(OrderDate AS Date)>='" + StartDispatchDate.ToString("yyyy-MM-dd") + "' AND CAST(OrderDate AS Date)<='" + FinishDispatchDate.ToString("yyyy-MM-dd") + "'";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                ClientErrorsWriteOffsDT.Clear();
                DA.Fill(ClientErrorsWriteOffsDT);
            }
            SelectCommand = @"SELECT AssemblyOrderID, ClientID, MainOrderID, DocNumber, CAST(DispatchDate AS Date) AS DispatchDate, AssemblyNumber, PaymentDate, Cost, Notes, Created, Active FROM AssemblyOrders
                WHERE CAST(DispatchDate AS Date)>='" + StartDispatchDate.ToString("yyyy-MM-dd") + "' AND CAST(DispatchDate AS Date)<='" + FinishDispatchDate.ToString("yyyy-MM-dd") + "'";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                ToAssemblyOrdersDT.Clear();
                DA.Fill(ToAssemblyOrdersDT);
            }
            SelectCommand = @"SELECT AssemblyOrderID, ClientID, MainOrderID, DocNumber, DispatchDate, AssemblyNumber, CAST(PaymentDate AS Date) AS PaymentDate, Cost, Notes, Created, Active FROM AssemblyOrders
                WHERE CAST(PaymentDate AS Date)>='" + StartDispatchDate.ToString("yyyy-MM-dd") + "' AND CAST(PaymentDate AS Date)<='" + FinishDispatchDate.ToString("yyyy-MM-dd") + "'";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                FromAssemblyOrdersDT.Clear();
                DA.Fill(FromAssemblyOrdersDT);
            }
            SelectCommand = @"SELECT NotPaidOrderID, ClientID, MainOrderID, DocNumber, CAST(DispatchDate AS Date) AS DispatchDate, CAST(PaymentDate AS Date) AS PaymentDate, Cost, Notes, Created, Active FROM NotPaidOrders
                WHERE CAST(DispatchDate AS Date)>='" + StartDispatchDate.ToString("yyyy-MM-dd") + "' AND CAST(DispatchDate AS Date)<='" + FinishDispatchDate.ToString("yyyy-MM-dd") + "'";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                NotPaidOrdersDT.Clear();
                DA.Fill(NotPaidOrdersDT);
            }

            decimal ZOVReport = 0;
            decimal DeductionsCost = 0;
            DataTable Table = new DataTable();
            DataTable TempMainOrdersDT = MainOrdersDT.Clone();
            using (DataView DV = new DataView(MainOrdersDT))
            {
                Table = DV.ToTable(true, new string[] { "DispatchDate" });
            }
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                ZOVReport = 0;
                DeductionsCost = 0;
                DateTime DispatchDate = Convert.ToDateTime(Table.Rows[i]["DispatchDate"]);
                DataRow[] irows = ReportCalculationsDT.Select("DispatchDate='" + DispatchDate.ToString("yyyy-MM-dd") + "'");
                foreach (DataRow item in irows)
                {
                    ZOVReport += Convert.ToDecimal(item["ZOVReport"]);
                    DeductionsCost += Convert.ToDecimal(item["DeductionsCost"]);
                }

                TempMainOrdersDT.Clear();
                DataRow[] rows = MainOrdersDT.Select("DispatchDate='" + DispatchDate.ToString("yyyy-MM-dd") + "'");
                foreach (DataRow item in rows)
                {
                    DataRow NewRow = TempMainOrdersDT.NewRow();
                    NewRow.ItemArray = item.ItemArray;
                    TempMainOrdersDT.Rows.Add(NewRow);
                }
                OrdersNeedCalculate(DispatchDate, TempMainOrdersDT, ZOVReport, DeductionsCost);
            }

            decimal PaymentPlan = 0;
            decimal DispatchedCost = 0;
            decimal SummaryReport = 0;
            decimal DebtsCost = 0;
            decimal OtherCost = 0;
            decimal NotDispatchedCost = 0;
            decimal ToAssemblyCost = 0;
            decimal FromAssemblyCost = 0;
            decimal RefundsCost = 0;
            string Name = "ИТОГО:";

            for (int i = 0; i < ReportViewDT.Rows.Count; i++)
            {
                PaymentPlan += Convert.ToDecimal(ReportViewDT.Rows[i]["PaymentPlan"]);
                DispatchedCost += Convert.ToDecimal(ReportViewDT.Rows[i]["DispatchedCost"]);
                SummaryReport += Convert.ToDecimal(ReportViewDT.Rows[i]["SummaryReport"]);
                DebtsCost += Convert.ToDecimal(ReportViewDT.Rows[i]["DebtsCost"]);
                OtherCost += Convert.ToDecimal(ReportViewDT.Rows[i]["OtherCost"]);
                NotDispatchedCost += Convert.ToDecimal(ReportViewDT.Rows[i]["NotDispatchedCost"]);
                ToAssemblyCost += Convert.ToDecimal(ReportViewDT.Rows[i]["ToAssemblyCost"]);
                FromAssemblyCost += Convert.ToDecimal(ReportViewDT.Rows[i]["FromAssemblyCost"]);
                RefundsCost += Convert.ToDecimal(ReportViewDT.Rows[i]["RefundsCost"]);
                ZOVReport += Convert.ToDecimal(ReportViewDT.Rows[i]["ZOVReport"]);
                DeductionsCost += Convert.ToDecimal(ReportViewDT.Rows[i]["DeductionsCost"]);
            }
            AddReportViewRow(Name, PaymentPlan, DispatchedCost, SummaryReport, DebtsCost, OtherCost,
                NotDispatchedCost, ToAssemblyCost, FromAssemblyCost, RefundsCost, ZOVReport, DeductionsCost);
        }

        public void SaveReportCalculations()
        {
            for (int i = 0; i < ReportViewDT.Rows.Count; i++)
            {
                decimal ZOVReport = Convert.ToDecimal(ReportViewDT.Rows[i]["ZOVReport"]);
                decimal DeductionsCost = Convert.ToDecimal(ReportViewDT.Rows[i]["DeductionsCost"]);
                string DispatchDate = ReportViewDT.Rows[i]["DispatchDate"].ToString();
                if (DispatchDate == "ИТОГО:")
                    continue;
                DataRow[] rows = ReportCalculationsDT.Select("DispatchDate='" + DispatchDate + "'");
                if (rows.Count() == 0)
                {
                }
                else
                {
                    rows[0]["ZOVReport"] = ZOVReport;
                    rows[0]["DeductionsCost"] = DeductionsCost;
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM ReportCalculations", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(ReportCalculationsDT);
                }
            }
        }

        public void UpdateReportCalculations(DateTime StartDispatchDate, DateTime FinishDispatchDate)
        {
            string SelectCommand = @"SELECT ReportCalculationsID,  CAST(DispatchDate AS Date) AS DispatchDate, 
                PaymentPlan, DispatchedCost, SummaryReport, DebtsCost, OtherCost, NotDispatchedCost, ToAssemblyCost, FromAssemblyCost, 
                RefundsCost, ZOVReport, DeductionsCost FROM ReportCalculations
                WHERE CAST(DispatchDate AS Date)>='" + StartDispatchDate.ToString("yyyy-MM-dd") + "' AND CAST(DispatchDate AS Date)<='" + FinishDispatchDate.ToString("yyyy-MM-dd") + "'";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                ReportCalculationsDT.Clear();
                DA.Fill(ReportCalculationsDT);
            }
        }

        public void MoveToAssemblyOrder(int AssemblyOrderID)
        {
            ReportViewBS.Position = ReportViewBS.Find("AssemblyOrderID", AssemblyOrderID);
        }

        private void OrdersNeedCalculate(DateTime Date, DataTable TempMainOrdersDT, decimal ZOVReport, decimal DeductionsCost)
        {
            decimal ClientErrorsCost = 0;

            decimal NotPaidCost = 0;

            decimal fNeedCalcCost = 0;
            decimal dNeedCalcCost = 0;

            decimal fNotNeedCalcCost = 0;
            decimal dNotNeedCalcCost = 0;
            decimal NotNeedCalcCost = 0;

            decimal fDispCost = 0;
            decimal dDispCost = 0;

            decimal fNotDispCost = 0;
            decimal dNotDispCost = 0;

            decimal fDebtCost = 0;
            decimal dDebtCost = 0;

            decimal fOtherCost = 0;
            decimal dOtherCost = 0;

            decimal PaymentPlan = 0;
            decimal DispatchedCost = 0;
            decimal SummaryReport = 0;
            decimal DebtsCost = 0;
            decimal OtherCost = 0;
            decimal NotDispatchedCost = 0;
            decimal ToAssemblyCost = 0;
            decimal FromAssemblyCost = 0;
            decimal RefundsCost = 0;

            for (int i = 0; i < TempMainOrdersDT.Rows.Count; i++)
            {
                bool DoNotDispatch = Convert.ToBoolean(TempMainOrdersDT.Rows[i]["DoNotDispatch"]);
                bool NeedCalculate = Convert.ToBoolean(TempMainOrdersDT.Rows[i]["NeedCalculate"]);
                int MainOrderID = Convert.ToInt32(TempMainOrdersDT.Rows[i]["MainOrderID"]);
                int DebtTypeID = Convert.ToInt32(TempMainOrdersDT.Rows[i]["DebtTypeID"]);
                DateTime DispatchDate = Convert.ToDateTime(TempMainOrdersDT.Rows[i]["DispatchDate"]);
                object ReorderDocNumber = TempMainOrdersDT.Rows[i]["ReorderDocNumber"];
                DataRow[] prows = PackageDetailsDT.Select("MainOrderID=" + MainOrderID);

                bool dDoNotDispatch = false;
                int dMainOrderID = 0;
                int dDebtTypeID = 0;

                if (ReorderDocNumber != DBNull.Value)//под другим п\п
                {
                    string SelectCommand = @"SELECT MainOrderID, DebtTypeID, DoNotDispatch FROM MainOrders WHERE DocNumber='" + ReorderDocNumber + "'";
                    using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
                    {
                        using (DataTable DT = new DataTable())
                        {
                            DA.Fill(DT);
                            dDoNotDispatch = Convert.ToBoolean(DT.Rows[0]["DoNotDispatch"]);
                            dMainOrderID = Convert.ToInt32(DT.Rows[0]["MainOrderID"]);
                            dDebtTypeID = Convert.ToInt32(DT.Rows[0]["DebtTypeID"]);
                        }
                    }
                }

                foreach (DataRow pitem in prows)
                {
                    decimal Count = Convert.ToDecimal(pitem["Count"]);
                    int OrderID = Convert.ToInt32(pitem["OrderID"]);
                    int PackageStatusID = Convert.ToInt32(pitem["PackageStatusID"]);
                    int ProductType = Convert.ToInt32(pitem["ProductType"]);
                    object DispatchDateTime = pitem["DispatchDateTime"];
                    if (ProductType == 0)
                    {
                        DataRow[] frows = FrontsOrdersDT.Select("FrontsOrdersID=" + OrderID);
                        foreach (DataRow fitem in frows)
                        {
                            decimal Cost = Count * Convert.ToDecimal(fitem["Cost"]) / Convert.ToDecimal(fitem["Count"]);
                            if (!NeedCalculate && DebtTypeID == 1)
                                fDebtCost += Cost;
                            if (!NeedCalculate && (DebtTypeID == 2 || DebtTypeID == 3 || DebtTypeID == 4))
                                fOtherCost += Cost;
                            if (NeedCalculate)//включено в расчет
                            {
                                fNeedCalcCost += Cost;
                                if (ReorderDocNumber != DBNull.Value)//под другим п\п
                                {
                                    if (dDebtTypeID == 2 || dDebtTypeID == 3 || dDebtTypeID == 4)
                                    { }
                                    else
                                        fNotDispCost += Cost;//включено в расчет, но не отгружено
                                }
                                else
                                {
                                    //if (DispatchDateTime != DBNull.Value&& (DispatchDate.ToString("yyyy-MM-dd") == Convert.ToDateTime(DispatchDateTime).ToString("yyyy-MM-dd")))
                                    //    fDispCost += Cost;//включено в расчет и отгружено
                                    //else
                                    //    fNotDispCost += Cost;//включено в расчет, но не отгружено в этот день
                                    if (PackageStatusID == 3)
                                        fDispCost += Cost;//включено в расчет и отгружено
                                    else
                                        fNotDispCost += Cost;//включено в расчет, но не отгружено
                                }
                            }
                            else
                                fNotNeedCalcCost += Cost;//не включено в расчет
                        }
                    }
                    if (ProductType == 1)
                    {
                        DataRow[] drows = DecorOrdersDT.Select("DecorOrderID=" + OrderID);
                        foreach (DataRow ditem in drows)
                        {
                            decimal Cost = Count * Convert.ToDecimal(ditem["Cost"]) / Convert.ToDecimal(ditem["Count"]);
                            if (!NeedCalculate && DebtTypeID == 1)
                                dDebtCost += Cost;
                            if (!NeedCalculate && (DebtTypeID == 2 || DebtTypeID == 3 || DebtTypeID == 4))
                                dOtherCost += Cost;
                            if (NeedCalculate)
                            {
                                dNeedCalcCost += Cost;
                                if (ReorderDocNumber != DBNull.Value)
                                {
                                    if (dDebtTypeID == 2 || dDebtTypeID == 3 || dDebtTypeID == 4)
                                    { }
                                    else
                                        dNotDispCost += Cost;//включено в расчет, но не отгружено
                                }
                                else
                                {
                                    //if (DispatchDateTime != DBNull.Value && (DispatchDate.ToString("yyyy-MM-dd") == Convert.ToDateTime(DispatchDateTime).ToString("yyyy-MM-dd")))
                                    //    dDispCost += Cost;//включено в расчет и отгружено
                                    //else
                                    //    dNotDispCost += Cost;//включено в расчет, но не отгружено
                                    if (PackageStatusID == 3)
                                        dDispCost += Cost;//включено в расчет и отгружено
                                    else
                                        dNotDispCost += Cost;
                                }
                            }
                            else
                                dNotNeedCalcCost += Cost;
                        }
                    }
                }
            }

            //НА СБОРКУ
            {
                DataRow[] rows = ToAssemblyOrdersDT.Select("DispatchDate='" + Date.ToString("yyyy-MM-dd") + "'");
                foreach (DataRow item in rows)
                {
                    decimal Cost = Convert.ToDecimal(item["Cost"]);
                    ToAssemblyCost += Cost;
                }
            }
            //СО СБОРКИ
            {
                DataRow[] rows = FromAssemblyOrdersDT.Select("PaymentDate='" + Date.ToString("yyyy-MM-dd") + "'");
                foreach (DataRow item in rows)
                {
                    decimal Cost = Convert.ToDecimal(item["Cost"]);
                    FromAssemblyCost += Cost;
                }
            }
            //ОШИБКИ ПО ВОЗВРАТУ 
            {
                DataRow[] rows = ClientErrorsWriteOffsDT.Select("OrderDate='" + Date.ToString("yyyy-MM-dd") + "'");
                foreach (DataRow item in rows)
                {
                    decimal Cost = Convert.ToDecimal(item["Cost"]);
                    ClientErrorsCost += Cost;
                }
            }
            //НЕ ОПЛАЧЕНО
            {
                DataRow[] rows = NotPaidOrdersDT.Select("DispatchDate='" + Date.ToString("yyyy-MM-dd") + "'");
                foreach (DataRow item in rows)
                {
                    decimal Cost = Convert.ToDecimal(item["Cost"]);
                    NotPaidCost += Cost;
                }
            }
            //if (NotPaidOrdersDT.Rows.Count > 0) //НЕ ОПЛАЧЕНО
            //{
            //    DataTable Table = new DataTable();
            //    using (DataView DV = new DataView(NotPaidOrdersDT))
            //    {
            //        Table = DV.ToTable(true, new string[] { "DispatchDate" });
            //    }
            //    for (int i = 0; i < Table.Rows.Count; i++)
            //    {
            //        DateTime DispatchDate = Convert.ToDateTime(Table.Rows[i]["DispatchDate"]);
            //        DataRow[] rows = NotPaidOrdersDT.Select("DispatchDate='" + DispatchDate.ToString("yyyy-MM-dd") + "'");
            //        foreach (DataRow item in rows)
            //        {
            //            decimal Cost = Convert.ToDecimal(item["Cost"]);
            //            NotPaidCost += Cost;
            //        }
            //    }
            //}

            RefundsCost = ClientErrorsCost + NotPaidCost;
            RefundsCost = Decimal.Round(RefundsCost, 1, MidpointRounding.AwayFromZero);

            NotNeedCalcCost = fNotNeedCalcCost + dNotNeedCalcCost;
            NotNeedCalcCost = Decimal.Round(NotNeedCalcCost, 1, MidpointRounding.AwayFromZero);
            PaymentPlan = fNeedCalcCost + dNeedCalcCost + RefundsCost;
            PaymentPlan = Decimal.Round(PaymentPlan, 1, MidpointRounding.AwayFromZero);
            DispatchedCost = fDispCost + dDispCost + NotNeedCalcCost;
            DispatchedCost = Decimal.Round(DispatchedCost, 1, MidpointRounding.AwayFromZero);

            DebtsCost = fDebtCost + dDebtCost;
            DebtsCost = Decimal.Round(DebtsCost, 1, MidpointRounding.AwayFromZero);
            OtherCost = fOtherCost + dOtherCost;
            OtherCost = Decimal.Round(OtherCost, 1, MidpointRounding.AwayFromZero);
            NotDispatchedCost = fNotDispCost + dNotDispCost;
            NotDispatchedCost = Decimal.Round(NotDispatchedCost, 1, MidpointRounding.AwayFromZero);

            SummaryReport = PaymentPlan - DebtsCost - OtherCost + NotDispatchedCost - ToAssemblyCost + FromAssemblyCost + NotPaidCost;

            AddReportCalculationsRow(Date, PaymentPlan, DispatchedCost, SummaryReport, DebtsCost, OtherCost,
                NotDispatchedCost, ToAssemblyCost, FromAssemblyCost, RefundsCost, ZOVReport, DeductionsCost);
            AddReportViewRow(Date.ToString("dd.MM.yyyy"), PaymentPlan, DispatchedCost, SummaryReport, DebtsCost, OtherCost,
                NotDispatchedCost, ToAssemblyCost, FromAssemblyCost, RefundsCost, ZOVReport, DeductionsCost);
        }

        public void ReportToExcel(DateTime StartDispatchDate, DateTime FinishDispatchDate)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Отчет " + StartDispatchDate.ToString("dd.MM") + "-" + FinishDispatchDate.ToString("dd.MM"));
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 15 * 256);
            sheet1.SetColumnWidth(1, 15 * 256);
            sheet1.SetColumnWidth(2, 15 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 15 * 256);
            sheet1.SetColumnWidth(6, 15 * 256);
            sheet1.SetColumnWidth(7, 15 * 256);
            sheet1.SetColumnWidth(8, 15 * 256);
            sheet1.SetColumnWidth(9, 15 * 256);
            sheet1.SetColumnWidth(10, 15 * 256);
            sheet1.SetColumnWidth(11, 15 * 256);


            #region Create fonts and styles

            HSSFFont Calibri11F = hssfworkbook.CreateFont();
            Calibri11F.FontHeightInPoints = 11;
            Calibri11F.FontName = "Calibri";

            HSSFFont CalibriBold11F = hssfworkbook.CreateFont();
            CalibriBold11F.FontHeightInPoints = 11;
            CalibriBold11F.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            CalibriBold11F.FontName = "Calibri";

            HSSFCellStyle Calibri11CS = hssfworkbook.CreateCellStyle();
            Calibri11CS.SetFont(Calibri11F);

            HSSFCellStyle CalibriBold11CS = hssfworkbook.CreateCellStyle();
            CalibriBold11CS.SetFont(CalibriBold11F);

            HSSFCellStyle HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            HeaderCS.Alignment = HSSFCellStyle.ALIGN_LEFT;
            HeaderCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            HeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            HeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            HeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            HeaderCS.VerticalAlignment = HSSFCellStyle.VERTICAL_CENTER;
            HeaderCS.Alignment = HSSFCellStyle.ALIGN_CENTER;
            HeaderCS.WrapText = true;
            HeaderCS.SetFont(CalibriBold11F);

            HSSFCellStyle MainContentCS = hssfworkbook.CreateCellStyle();
            MainContentCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000");
            MainContentCS.Alignment = HSSFCellStyle.ALIGN_LEFT;
            MainContentCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            MainContentCS.BottomBorderColor = HSSFColor.BLACK.index;
            MainContentCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            MainContentCS.LeftBorderColor = HSSFColor.BLACK.index;
            MainContentCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            MainContentCS.RightBorderColor = HSSFColor.BLACK.index;
            MainContentCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            MainContentCS.TopBorderColor = HSSFColor.BLACK.index;
            MainContentCS.Alignment = HSSFCellStyle.ALIGN_RIGHT;
            MainContentCS.SetFont(Calibri11F);

            #endregion

            int RowIndex = 0;
            DataTable DT = ReportViewDT.Copy();

            HSSFCell cell = null;

            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "ДАТА ОТГРУЗКИ");
            //cell.CellStyle = TableHeaderCS;
            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            //cell.CellStyle = TableHeaderCS;
            //sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            //DisplayIndex++;

            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), DisplayIndex, "МДФ");
            //cell.CellStyle = TableHeaderCS;
            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), DisplayIndex, string.Empty);
            //cell.CellStyle = TableHeaderCS;
            //sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, DisplayIndex, RowIndex + 1, DisplayIndex));
            //DisplayIndex++;

            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Размер");
            //cell.CellStyle = TableHeaderCS;
            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, string.Empty);
            //cell.CellStyle = TableHeaderCS;
            //sheet1.AddMergedRegion(new NPOI.HSSF.Util.Region(RowIndex, 2, RowIndex, 3));

            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), 2, "длина, мм");
            //cell.CellStyle = TableHeaderCS;
            //cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex + 1), 3, "ширина, мм");
            //cell.CellStyle = TableHeaderCS;
            //DisplayIndex++;
            //DisplayIndex++;

            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 0, "ДАТА ОТГРУЗКИ");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 1, "План расчет");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 2, "Расчет отгруженного");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 3, "Суммарный отчет");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 4, "Снято с расчета (долги)");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 5, "Снято с расчета (другое)");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 6, "ДОЛГИ");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 7, "На сборку");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 8, "Со сборки");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 9, "Возврат");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 10, "Отчет ЗОВ");
            cell.CellStyle = HeaderCS;
            cell = HSSFCellUtil.CreateCell(sheet1.CreateRow(RowIndex), 11, "Минус от ОВ");
            cell.CellStyle = HeaderCS;
            RowIndex++;

            for (int x = 0; x < DT.Rows.Count; x++)
            {
                for (int y = 0; y < DT.Columns.Count; y++)
                {
                    Type t = DT.Rows[x][y].GetType();

                    if (t.Name == "Decimal")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToDouble(DT.Rows[x][y]));
                        cell.CellStyle = MainContentCS;
                        continue;
                    }
                    if (t.Name == "Int32")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(Convert.ToInt32(DT.Rows[x][y]));
                        cell.CellStyle = MainContentCS;
                        continue;
                    }

                    if (t.Name == "String" || t.Name == "DBNull")
                    {
                        cell = sheet1.CreateRow(RowIndex).CreateCell(y);
                        cell.SetCellValue(DT.Rows[x][y].ToString());
                        cell.CellStyle = MainContentCS;
                        continue;
                    }
                }
                RowIndex++;
            }

            string FileName1 = StartDispatchDate.ToString("dd.MM") + "-" + FinishDispatchDate.ToString("dd.MM") + " Отчет";
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");

            //string FileName1 = StartDispatchDate.ToString("dd.MM") + "-" + FinishDispatchDate.ToString("dd.MM") + " Отчет";
            //string tempFolder = @"\\192.168.1.6\Public\ТПС\Infinium\Задания на покраску\";
            //string CurrentMonthName = DateTime.Now.ToString("MMMM");
            //tempFolder = Path.Combine(tempFolder, CurrentMonthName);
            if (!(Directory.Exists(tempFolder)))
            {
                Directory.CreateDirectory(tempFolder);
            }
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName1 + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + FileName1 + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);

        }

    }

}
