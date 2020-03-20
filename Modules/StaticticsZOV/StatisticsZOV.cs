using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Infinium.Modules.StaticticsZOV
{




    public class Payments
    {
        int CurrentClientGroupID = -1;

        string sResult = string.Empty;
        string sCalcWriteOffResult = string.Empty;
        string sWriteOffResult = string.Empty;

        DataTable ClientsDT = null;
        DataTable ClientsGroupsDT = null;

        DataTable ClientCalcDebtsDT = null;
        DataTable ClientsGroupCalcDebtsDT = null;
        DataTable ClientWriteOffDT = null;
        DataTable ClientsGroupWriteOffDT = null;
        DataTable ClientDispatchDT = null;
        DataTable ClientsGroupDispatchDT = null;

        DataTable ResultTotalDataTable = null;
        DataTable WriteOffDataTable = null;
        DataTable CalcWriteOffDataTable = null;

        public BindingSource ClientsBS;
        public BindingSource ClientsGroupsBS;
        public BindingSource ClientCalcDebtsBS = null;
        public BindingSource ClientsGroupCalcDebtsBS = null;
        public BindingSource ClientWriteOffBS = null;
        public BindingSource ClientsGroupWriteOffBS = null;
        public BindingSource ClientDispatchBS = null;
        public BindingSource ClientsGroupDispatchBS = null;
        public BindingSource ResultTotalBindingSource = null;
        public BindingSource WriteOffBindingSource = null;
        public BindingSource CalcWriteOffBindingSource = null;

        PercentageDataGrid ResultTotalDataGrid = null;
        PercentageDataGrid WriteOffDataGrid = null;
        PercentageDataGrid CalcWriteOffDataGrid = null;

        ArrayList Clients;
        ArrayList ClientsGroups;

        public Payments(ref PercentageDataGrid tResultTotalDataGrid, ref PercentageDataGrid tWriteOffDataGrid,
            ref PercentageDataGrid tCalcWriteOffDataGrid)
        {
            ResultTotalDataGrid = tResultTotalDataGrid;
            CalcWriteOffDataGrid = tCalcWriteOffDataGrid;
            WriteOffDataGrid = tWriteOffDataGrid;

            Initialize();
        }

        private void Create()
        {
            ClientsDT = new DataTable();
            ClientsGroupsDT = new DataTable();

            ClientCalcDebtsDT = new DataTable();
            ClientCalcDebtsDT.Columns.Add(new DataColumn("ClientID", Type.GetType("System.Int32")));
            ClientCalcDebtsDT.Columns.Add(new DataColumn("ClientGroupID", Type.GetType("System.Int32")));
            ClientCalcDebtsDT.Columns.Add(new DataColumn("DebtTypeID", Type.GetType("System.Int32")));
            ClientCalcDebtsDT.Columns.Add(new DataColumn("ClientName", Type.GetType("System.String")));
            ClientCalcDebtsDT.Columns.Add(new DataColumn("CalcDebtCost", Type.GetType("System.Decimal")));
            ClientCalcDebtsDT.Columns.Add(new DataColumn("CalcDefectsCost", Type.GetType("System.Decimal")));
            ClientCalcDebtsDT.Columns.Add(new DataColumn("CalcProductionErrorsCost", Type.GetType("System.Decimal")));
            ClientCalcDebtsDT.Columns.Add(new DataColumn("CalcZOVErrorsCost", Type.GetType("System.Decimal")));
            ClientCalcDebtsDT.Columns.Add(new DataColumn("Percentage1", Type.GetType("System.Decimal")));
            ClientCalcDebtsDT.Columns.Add(new DataColumn("Percentage2", Type.GetType("System.Decimal")));
            ClientCalcDebtsDT.Columns.Add(new DataColumn("Percentage3", Type.GetType("System.Decimal")));
            ClientCalcDebtsDT.Columns.Add(new DataColumn("Percentage4", Type.GetType("System.Decimal")));

            ClientsGroupCalcDebtsDT = new DataTable();
            ClientsGroupCalcDebtsDT.Columns.Add(new DataColumn("ClientGroupID", Type.GetType("System.Int32")));
            ClientsGroupCalcDebtsDT.Columns.Add(new DataColumn("DebtTypeID", Type.GetType("System.Int32")));
            ClientsGroupCalcDebtsDT.Columns.Add(new DataColumn("ClientGroupName", Type.GetType("System.String")));
            ClientsGroupCalcDebtsDT.Columns.Add(new DataColumn("CalcDebtCost", Type.GetType("System.Decimal")));
            ClientsGroupCalcDebtsDT.Columns.Add(new DataColumn("CalcDefectsCost", Type.GetType("System.Decimal")));
            ClientsGroupCalcDebtsDT.Columns.Add(new DataColumn("CalcProductionErrorsCost", Type.GetType("System.Decimal")));
            ClientsGroupCalcDebtsDT.Columns.Add(new DataColumn("CalcZOVErrorsCost", Type.GetType("System.Decimal")));
            ClientsGroupCalcDebtsDT.Columns.Add(new DataColumn("Percentage1", Type.GetType("System.Decimal")));
            ClientsGroupCalcDebtsDT.Columns.Add(new DataColumn("Percentage2", Type.GetType("System.Decimal")));
            ClientsGroupCalcDebtsDT.Columns.Add(new DataColumn("Percentage3", Type.GetType("System.Decimal")));
            ClientsGroupCalcDebtsDT.Columns.Add(new DataColumn("Percentage4", Type.GetType("System.Decimal")));

            ClientWriteOffDT = new DataTable();
            ClientWriteOffDT.Columns.Add(new DataColumn("ClientID", Type.GetType("System.Int32")));
            ClientWriteOffDT.Columns.Add(new DataColumn("ClientGroupID", Type.GetType("System.Int32")));
            ClientWriteOffDT.Columns.Add(new DataColumn("DebtTypeID", Type.GetType("System.Int32")));
            ClientWriteOffDT.Columns.Add(new DataColumn("ClientName", Type.GetType("System.String")));
            ClientWriteOffDT.Columns.Add(new DataColumn("CalcDebtCost", Type.GetType("System.Decimal")));
            ClientWriteOffDT.Columns.Add(new DataColumn("CalcDefectsCost", Type.GetType("System.Decimal")));
            ClientWriteOffDT.Columns.Add(new DataColumn("CalcProductionErrorsCost", Type.GetType("System.Decimal")));
            ClientWriteOffDT.Columns.Add(new DataColumn("CalcZOVErrorsCost", Type.GetType("System.Decimal")));
            ClientWriteOffDT.Columns.Add(new DataColumn("SamplesWriteOffCost", Type.GetType("System.Decimal")));
            ClientWriteOffDT.Columns.Add(new DataColumn("Percentage1", Type.GetType("System.Decimal")));
            ClientWriteOffDT.Columns.Add(new DataColumn("Percentage2", Type.GetType("System.Decimal")));
            ClientWriteOffDT.Columns.Add(new DataColumn("Percentage3", Type.GetType("System.Decimal")));
            ClientWriteOffDT.Columns.Add(new DataColumn("Percentage4", Type.GetType("System.Decimal")));
            ClientWriteOffDT.Columns.Add(new DataColumn("Percentage5", Type.GetType("System.Decimal")));

            ClientsGroupWriteOffDT = new DataTable();
            ClientsGroupWriteOffDT.Columns.Add(new DataColumn("ClientGroupID", Type.GetType("System.Int32")));
            ClientsGroupWriteOffDT.Columns.Add(new DataColumn("DebtTypeID", Type.GetType("System.Int32")));
            ClientsGroupWriteOffDT.Columns.Add(new DataColumn("ClientGroupName", Type.GetType("System.String")));
            ClientsGroupWriteOffDT.Columns.Add(new DataColumn("CalcDebtCost", Type.GetType("System.Decimal")));
            ClientsGroupWriteOffDT.Columns.Add(new DataColumn("CalcDefectsCost", Type.GetType("System.Decimal")));
            ClientsGroupWriteOffDT.Columns.Add(new DataColumn("CalcProductionErrorsCost", Type.GetType("System.Decimal")));
            ClientsGroupWriteOffDT.Columns.Add(new DataColumn("CalcZOVErrorsCost", Type.GetType("System.Decimal")));
            ClientsGroupWriteOffDT.Columns.Add(new DataColumn("SamplesWriteOffCost", Type.GetType("System.Decimal")));
            ClientsGroupWriteOffDT.Columns.Add(new DataColumn("Percentage1", Type.GetType("System.Decimal")));
            ClientsGroupWriteOffDT.Columns.Add(new DataColumn("Percentage2", Type.GetType("System.Decimal")));
            ClientsGroupWriteOffDT.Columns.Add(new DataColumn("Percentage3", Type.GetType("System.Decimal")));
            ClientsGroupWriteOffDT.Columns.Add(new DataColumn("Percentage4", Type.GetType("System.Decimal")));
            ClientsGroupWriteOffDT.Columns.Add(new DataColumn("Percentage5", Type.GetType("System.Decimal")));

            ClientDispatchDT = new DataTable();
            ClientDispatchDT.Columns.Add(new DataColumn("DispatchID", Type.GetType("System.Int32")));
            ClientDispatchDT.Columns.Add(new DataColumn("ClientID", Type.GetType("System.Int32")));
            ClientDispatchDT.Columns.Add(new DataColumn("ClientGroupID", Type.GetType("System.Int32")));
            ClientDispatchDT.Columns.Add(new DataColumn("ClientName", Type.GetType("System.String")));
            ClientDispatchDT.Columns.Add(new DataColumn("DispatchCost", Type.GetType("System.Decimal")));
            ClientDispatchDT.Columns.Add(new DataColumn("NotDispatchCost", Type.GetType("System.Decimal")));
            ClientDispatchDT.Columns.Add(new DataColumn("SamplesCost", Type.GetType("System.Decimal")));
            ClientDispatchDT.Columns.Add(new DataColumn("Percentage1", Type.GetType("System.Decimal")));
            ClientDispatchDT.Columns.Add(new DataColumn("Percentage2", Type.GetType("System.Decimal")));
            ClientDispatchDT.Columns.Add(new DataColumn("Percentage3", Type.GetType("System.Decimal")));

            ClientsGroupDispatchDT = new DataTable();
            ClientsGroupDispatchDT.Columns.Add(new DataColumn("DispatchID", Type.GetType("System.Int32")));
            ClientsGroupDispatchDT.Columns.Add(new DataColumn("ClientGroupID", Type.GetType("System.Int32")));
            ClientsGroupDispatchDT.Columns.Add(new DataColumn("ClientGroupName", Type.GetType("System.String")));
            ClientsGroupDispatchDT.Columns.Add(new DataColumn("DispatchCost", Type.GetType("System.Decimal")));
            ClientsGroupDispatchDT.Columns.Add(new DataColumn("NotDispatchCost", Type.GetType("System.Decimal")));
            ClientsGroupDispatchDT.Columns.Add(new DataColumn("SamplesCost", Type.GetType("System.Decimal")));
            ClientsGroupDispatchDT.Columns.Add(new DataColumn("Percentage1", Type.GetType("System.Decimal")));
            ClientsGroupDispatchDT.Columns.Add(new DataColumn("Percentage2", Type.GetType("System.Decimal")));
            ClientsGroupDispatchDT.Columns.Add(new DataColumn("Percentage3", Type.GetType("System.Decimal")));

            ResultTotalDataTable = new DataTable();
            ResultTotalDataTable.Columns.Add(new DataColumn("DispatchID", Type.GetType("System.Int32")));
            ResultTotalDataTable.Columns.Add(new DataColumn("Parameter", Type.GetType("System.String")));
            ResultTotalDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));

            WriteOffDataTable = new DataTable();
            WriteOffDataTable.Columns.Add(new DataColumn("DebtTypeID", Type.GetType("System.Int32")));
            WriteOffDataTable.Columns.Add(new DataColumn("Parameter", Type.GetType("System.String")));
            WriteOffDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));

            CalcWriteOffDataTable = new DataTable();
            CalcWriteOffDataTable.Columns.Add(new DataColumn("DebtTypeID", Type.GetType("System.Int32")));
            CalcWriteOffDataTable.Columns.Add(new DataColumn("Parameter", Type.GetType("System.String")));
            CalcWriteOffDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));

            Clients = new ArrayList();
            ClientsGroups = new ArrayList();
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients ORDER BY ClientName",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDT);
                ClientsDT.Columns.Add(new DataColumn("Checked", Type.GetType("System.Boolean")));
                foreach (DataRow row in ClientsDT.Rows)
                {
                    row["Checked"] = false;
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientsGroups ORDER BY ClientGroupName",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsGroupsDT);
                ClientsGroupsDT.Columns.Add(new DataColumn("Checked", Type.GetType("System.Boolean")));
                foreach (DataRow row in ClientsGroupsDT.Rows)
                {
                    row["Checked"] = false;
                }

                //DataRow NewRow = ClientsGroupsDT.NewRow();
                //NewRow["ClientGroupID"] = -2;
                //NewRow["ClientGroupName"] = "Выбрать все";
                //NewRow["Checked"] = true;
                //ClientsGroupsDT.Rows.InsertAt(NewRow, 0);
            }
        }

        private void Binding()
        {
            ClientsBS = new BindingSource()
            {
                DataSource = ClientsDT
            };
            ClientsGroupsBS = new BindingSource()
            {
                DataSource = ClientsGroupsDT
            };
            ClientCalcDebtsBS = new BindingSource()
            {
                DataSource = ClientCalcDebtsDT
            };
            ClientsGroupCalcDebtsBS = new BindingSource()
            {
                DataSource = ClientsGroupCalcDebtsDT
            };
            ClientWriteOffBS = new BindingSource()
            {
                DataSource = ClientWriteOffDT
            };
            ClientsGroupWriteOffBS = new BindingSource()
            {
                DataSource = ClientsGroupWriteOffDT
            };
            ClientDispatchBS = new BindingSource()
            {
                DataSource = ClientDispatchDT
            };
            ClientsGroupDispatchBS = new BindingSource()
            {
                DataSource = ClientsGroupDispatchDT
            };
            ResultTotalBindingSource = new BindingSource()
            {
                DataSource = ResultTotalDataTable
            };
            ResultTotalDataGrid.DataSource = ResultTotalBindingSource;

            WriteOffBindingSource = new BindingSource()
            {
                DataSource = WriteOffDataTable
            };
            WriteOffDataGrid.DataSource = WriteOffBindingSource;

            CalcWriteOffBindingSource = new BindingSource()
            {
                DataSource = CalcWriteOffDataTable
            };
            CalcWriteOffDataGrid.DataSource = CalcWriteOffBindingSource;
        }

        private void SetGrids()
        {
            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 1,
                NumberGroupSeparator = " ",
                NumberDecimalSeparator = ","
            };
            ResultTotalDataGrid.Columns["DispatchID"].Visible = false;
            ResultTotalDataGrid.Columns["Cost"].DefaultCellStyle.Format = "C";
            ResultTotalDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;

            ResultTotalDataGrid.Columns["Parameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ResultTotalDataGrid.Columns["Parameter"].MinimumWidth = 150;
            ResultTotalDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ResultTotalDataGrid.Columns["Cost"].MinimumWidth = 50;

            ResultTotalDataGrid.Columns["Parameter"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomLeft;
            ResultTotalDataGrid.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;


            WriteOffDataGrid.Columns["DebtTypeID"].Visible = false;
            WriteOffDataGrid.Columns["Parameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            WriteOffDataGrid.Columns["Parameter"].MinimumWidth = 150;
            WriteOffDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            WriteOffDataGrid.Columns["Cost"].MinimumWidth = 50;

            WriteOffDataGrid.Columns["Cost"].DefaultCellStyle.Format = "C";
            WriteOffDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;

            WriteOffDataGrid.Columns["Parameter"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomLeft;
            WriteOffDataGrid.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;


            CalcWriteOffDataGrid.Columns["DebtTypeID"].Visible = false;
            CalcWriteOffDataGrid.Columns["Cost"].DefaultCellStyle.Format = "C";
            CalcWriteOffDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;

            CalcWriteOffDataGrid.Columns["Parameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            CalcWriteOffDataGrid.Columns["Parameter"].MinimumWidth = 150;
            CalcWriteOffDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            CalcWriteOffDataGrid.Columns["Cost"].MinimumWidth = 50;

            //CalcWriteOffDataGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //CalcWriteOffDataGrid.Columns["Cost"].Width = 150;

            CalcWriteOffDataGrid.Columns["Parameter"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomLeft;
            CalcWriteOffDataGrid.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;

        }

        private void Initialize()
        {
            Create();
            Fill();
            Binding();
            SetGrids();
        }

        public void GetClientCalcDebts(DateTime DateFrom, DateTime DateTo)
        {
            string ClientsFilter = string.Empty;
            string GroupsFilter = string.Empty;

            if (Clients.Count > 0)
                ClientsFilter = " AND MainOrders.ClientID IN (" + string.Join(",", Clients.OfType<Int32>().ToArray()) + ")";
            else
                ClientsFilter = " AND MainOrders.ClientID = -1";
            if (ClientsGroups.Count > 0)
                GroupsFilter = " AND MainOrders.ClientID IN (SELECT ClientID FROM infiniu2_zovreference.dbo.Clients WHERE ClientGroupID IN (" +
                    string.Join(",", ClientsGroups.OfType<Int32>().ToArray()) + "))";

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ClientName, DebtTypeID, ClientGroupID, MainOrders.ClientID, SUM(CalcDebtCost) AS DebtCost FROM MainOrders
            INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID
            WHERE CalcDebtCost > 0 AND MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE CAST(DispatchDate AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") +
            "' AND CAST(DispatchDate AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + "')" + GroupsFilter + ClientsFilter +
            " GROUP BY ClientName, DebtTypeID, ClientGroupID, MainOrders.ClientID" +
            " ORDER BY ClientName, DebtTypeID, ClientGroupID, MainOrders.ClientID",
            ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    ClientCalcDebtsDT.Clear();
                    if (DA.Fill(DT) == 0)
                        return;

                    int ClientGroupID = 0;
                    int DebtType = 0;
                    decimal CalcDebtCost = 0;
                    decimal CalcDefectsCost = 0;
                    decimal CalcProductionErrorsCost = 0;
                    decimal CalcZOVErrorsCost = 0;
                    decimal DebtCost = 0;
                    decimal ProgressVal = 0;

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        ClientGroupID = Convert.ToInt32(DT.Rows[i]["ClientGroupID"]);
                        DebtCost = Convert.ToDecimal(DT.Rows[i]["DebtCost"]);
                        DebtType = Convert.ToInt32(DT.Rows[i]["DebtTypeID"]);
                        if (DebtCost == 0)
                            continue;

                        DataRow[] Rows = ClientsGroupCalcDebtsDT.Select("ClientGroupID = " + ClientGroupID + " AND DebtTypeID = " + DebtType);
                        if (Rows.Count() > 0)
                        {
                            DataRow NewRow = ClientCalcDebtsDT.NewRow();
                            NewRow["ClientID"] = DT.Rows[i]["ClientID"];
                            NewRow["DebtTypeID"] = DT.Rows[i]["DebtTypeID"];
                            NewRow["ClientGroupID"] = DT.Rows[i]["ClientGroupID"];
                            NewRow["ClientName"] = DT.Rows[i]["ClientName"];

                            if (DebtType == 1)
                            {
                                CalcDebtCost = Convert.ToDecimal(Rows[0]["CalcDebtCost"]);

                                if (CalcDebtCost > 0)
                                {
                                    ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcDebtCost));
                                    ProgressVal = ProgressVal * 100;
                                    //ProgressVal = Decimal.Round(ProgressVal * 100, 1, MidpointRounding.AwayFromZero);
                                    NewRow["CalcDebtCost"] = DebtCost;
                                    NewRow["Percentage1"] = ProgressVal;
                                }
                            }
                            if (DebtType == 2)
                            {
                                CalcDefectsCost = Convert.ToDecimal(Rows[0]["CalcDefectsCost"]);

                                if (CalcDefectsCost > 0)
                                {
                                    ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcDefectsCost));
                                    ProgressVal = ProgressVal * 100;
                                    //ProgressVal = Decimal.Round(ProgressVal * 100, 1, MidpointRounding.AwayFromZero);
                                    NewRow["CalcDefectsCost"] = DebtCost;
                                    NewRow["Percentage2"] = ProgressVal;
                                }
                            }
                            if (DebtType == 3)
                            {
                                CalcProductionErrorsCost = Convert.ToDecimal(Rows[0]["CalcProductionErrorsCost"]);

                                if (CalcProductionErrorsCost > 0)
                                {
                                    ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcProductionErrorsCost));
                                    ProgressVal = ProgressVal * 100;
                                    //ProgressVal = Decimal.Round(ProgressVal * 100, 1, MidpointRounding.AwayFromZero);
                                    NewRow["CalcProductionErrorsCost"] = DebtCost;
                                    NewRow["Percentage3"] = ProgressVal;
                                }
                            }
                            if (DebtType == 4)
                            {
                                CalcZOVErrorsCost = Convert.ToDecimal(Rows[0]["CalcZOVErrorsCost"]);

                                if (CalcZOVErrorsCost > 0)
                                {
                                    ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcZOVErrorsCost));
                                    ProgressVal = ProgressVal * 100;
                                    //ProgressVal = Decimal.Round(ProgressVal * 100, 1, MidpointRounding.AwayFromZero);
                                    NewRow["CalcZOVErrorsCost"] = DebtCost;
                                    NewRow["Percentage4"] = ProgressVal;
                                }
                            }

                            ClientCalcDebtsDT.Rows.Add(NewRow);
                        }
                    }
                }
            }
            ClientCalcDebtsBS.MoveFirst();
            ClientCalcDebtsDT.DefaultView.Sort = "Percentage1 DESC, Percentage2 DESC, Percentage3 DESC, Percentage4 DESC";
        }

        public void GetClientGroupCalcDebts(DateTime DateFrom, DateTime DateTo)
        {
            string ClientsFilter = string.Empty;
            string GroupsFilter = string.Empty;

            if (Clients.Count > 0)
                ClientsFilter = " AND MainOrders.ClientID IN (" + string.Join(",", Clients.OfType<Int32>().ToArray()) + ")";
            else
                ClientsFilter = " AND MainOrders.ClientID = -1";
            if (ClientsGroups.Count > 0)
                GroupsFilter = " AND MainOrders.ClientID IN (SELECT ClientID FROM infiniu2_zovreference.dbo.Clients WHERE ClientGroupID IN (" +
                    string.Join(",", ClientsGroups.OfType<Int32>().ToArray()) + "))";

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ClientGroupName, DebtTypeID, infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID, 
            SUM(CalcDebtCost) AS DebtCost FROM MainOrders
            INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID
            INNER JOIN infiniu2_zovreference.dbo.ClientsGroups ON infiniu2_zovreference.dbo.Clients.ClientGroupID = infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID
            WHERE CalcDebtCost > 0 AND MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE CAST(DispatchDate AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") +
            "' AND CAST(DispatchDate AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + "')" + GroupsFilter + ClientsFilter +
            " GROUP BY ClientGroupName, DebtTypeID, infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID" +
            " ORDER BY ClientGroupName, DebtTypeID, infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID",
            ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    ClientsGroupCalcDebtsDT.Clear();
                    if (DA.Fill(DT) == 0)
                        return;

                    int DebtType = 0;
                    decimal CalcDebtCost = 0;
                    decimal CalcDefectsCost = 0;
                    decimal CalcProductionErrorsCost = 0;
                    decimal CalcZOVErrorsCost = 0;
                    decimal DebtCost = 0;
                    decimal ProgressVal = 0;

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DebtType = Convert.ToInt32(DT.Rows[i]["DebtTypeID"]);

                        if (DebtType == 1)
                            CalcDebtCost += Convert.ToDecimal(DT.Rows[i]["DebtCost"]);
                        if (DebtType == 2)
                            CalcDefectsCost += Convert.ToDecimal(DT.Rows[i]["DebtCost"]);
                        if (DebtType == 3)
                            CalcProductionErrorsCost += Convert.ToDecimal(DT.Rows[i]["DebtCost"]);
                        if (DebtType == 4)
                            CalcZOVErrorsCost += Convert.ToDecimal(DT.Rows[i]["DebtCost"]);
                    }

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DebtCost = Convert.ToDecimal(DT.Rows[i]["DebtCost"]);
                        DebtType = Convert.ToInt32(DT.Rows[i]["DebtTypeID"]);
                        if (DebtCost == 0)
                            continue;

                        DataRow NewRow = ClientsGroupCalcDebtsDT.NewRow();
                        NewRow["DebtTypeID"] = DT.Rows[i]["DebtTypeID"];
                        NewRow["ClientGroupID"] = DT.Rows[i]["ClientGroupID"];
                        NewRow["ClientGroupName"] = DT.Rows[i]["ClientGroupName"];

                        if (DebtType == 1)
                        {
                            if (CalcDebtCost > 0)
                                ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcDebtCost));
                            //ProgressVal = Decimal.Round(ProgressVal * 100, 1, MidpointRounding.AwayFromZero);
                            ProgressVal = ProgressVal * 100;
                            NewRow["CalcDebtCost"] = DebtCost;
                            NewRow["Percentage1"] = ProgressVal;
                        }
                        if (DebtType == 2)
                        {
                            if (CalcDefectsCost > 0)
                                ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcDefectsCost));
                            //ProgressVal = Decimal.Round(ProgressVal * 100, 1, MidpointRounding.AwayFromZero);
                            ProgressVal = ProgressVal * 100;
                            NewRow["CalcDefectsCost"] = DebtCost;
                            NewRow["Percentage2"] = ProgressVal;
                        }
                        if (DebtType == 3)
                        {
                            if (CalcProductionErrorsCost > 0)
                                ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcProductionErrorsCost));
                            //ProgressVal = Decimal.Round(ProgressVal * 100, 1, MidpointRounding.AwayFromZero);
                            ProgressVal = ProgressVal * 100;
                            NewRow["CalcProductionErrorsCost"] = DebtCost;
                            NewRow["Percentage3"] = ProgressVal;
                        }
                        if (DebtType == 4)
                        {
                            if (CalcZOVErrorsCost > 0)
                                ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcZOVErrorsCost));
                            //ProgressVal = Decimal.Round(ProgressVal * 100, 1, MidpointRounding.AwayFromZero);
                            ProgressVal = ProgressVal * 100;
                            NewRow["CalcZOVErrorsCost"] = DebtCost;
                            NewRow["Percentage4"] = ProgressVal;
                        }

                        ClientsGroupCalcDebtsDT.Rows.Add(NewRow);
                    }
                }
            }
            ClientsGroupCalcDebtsBS.MoveFirst();
            ClientsGroupCalcDebtsDT.DefaultView.Sort = "Percentage1 DESC, Percentage2 DESC, Percentage3 DESC, Percentage4 DESC";
        }

        public void GetClientWriteOff(DateTime DateFrom, DateTime DateTo)
        {
            string ClientsFilter = string.Empty;

            if (Clients.Count > 0)
                ClientsFilter = " AND ClientID IN (" + string.Join(",", Clients.OfType<Int32>().ToArray()) + ")";
            else
                ClientsFilter = " AND ClientID = -1";

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT PaymentDetail.ClientName, PaymentDetail.DebtTypeID, infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID, infiniu2_zovreference.dbo.Clients.ClientID,
            SUM(PaymentDetail.SamplesWriteOffCost) AS SamplesWriteOffCost, SUM(PaymentDetail.DebtCost) AS DebtCost FROM PaymentDetail
            INNER JOIN infiniu2_zovreference.dbo.Clients ON (PaymentDetail.ClientName = infiniu2_zovreference.dbo.Clients.ClientName" + ClientsFilter + @")
            INNER JOIN infiniu2_zovreference.dbo.ClientsGroups ON infiniu2_zovreference.dbo.Clients.ClientGroupID = infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID
            WHERE PaymentWeekID IN (SELECT PaymentWeekID FROM PaymentWeeks WHERE CAST(DateFrom AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") +
            "' AND CAST(DateTo AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + @"')
            GROUP BY infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID, infiniu2_zovreference.dbo.Clients.ClientID, PaymentDetail.ClientName, PaymentDetail.DebtTypeID
            ORDER BY infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID, infiniu2_zovreference.dbo.Clients.ClientID, PaymentDetail.ClientName, PaymentDetail.DebtTypeID",
            ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    ClientWriteOffDT.Clear();
                    if (DA.Fill(DT) == 0)
                        return;

                    int ClientGroupID = 0;
                    int DebtType = 0;
                    decimal CalcDebtCost = 0;
                    decimal CalcDefectsCost = 0;
                    decimal CalcProductionErrorsCost = 0;
                    decimal CalcZOVErrorsCost = 0;
                    decimal SamplesWriteOffCost = 0;
                    decimal DebtCost = 0;
                    decimal SamplesCost = 0;
                    decimal ProgressVal = 0;

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        ClientGroupID = Convert.ToInt32(DT.Rows[i]["ClientGroupID"]);
                        DebtCost = Convert.ToDecimal(DT.Rows[i]["DebtCost"]);
                        SamplesCost = Convert.ToDecimal(DT.Rows[i]["SamplesWriteOffCost"]);
                        DebtType = Convert.ToInt32(DT.Rows[i]["DebtTypeID"]);
                        if (DebtCost == 0 && SamplesCost == 0)
                            continue;

                        DataRow[] Rows = ClientsGroupWriteOffDT.Select("ClientGroupID = " + ClientGroupID + " AND DebtTypeID = " + DebtType);
                        if (Rows.Count() > 0)
                        {
                            DataRow NewRow = ClientWriteOffDT.NewRow();
                            NewRow["ClientID"] = DT.Rows[i]["ClientID"];
                            NewRow["DebtTypeID"] = DT.Rows[i]["DebtTypeID"];
                            NewRow["ClientGroupID"] = DT.Rows[i]["ClientGroupID"];
                            NewRow["ClientName"] = DT.Rows[i]["ClientName"];

                            if (DebtType == 0)
                            {
                                SamplesWriteOffCost = Convert.ToDecimal(Rows[0]["SamplesWriteOffCost"]);

                                if (SamplesWriteOffCost > 0)
                                {
                                    ProgressVal = Convert.ToDecimal(Convert.ToDecimal(SamplesCost) / Convert.ToDecimal(SamplesWriteOffCost));
                                    ProgressVal = ProgressVal * 100;
                                    NewRow["SamplesWriteOffCost"] = SamplesCost;
                                    NewRow["Percentage5"] = ProgressVal;
                                }
                            }
                            if (DebtType == 1)
                            {
                                CalcDebtCost = Convert.ToDecimal(Rows[0]["CalcDebtCost"]);

                                if (CalcDebtCost > 0)
                                {
                                    ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcDebtCost));
                                    ProgressVal = ProgressVal * 100;
                                    NewRow["CalcDebtCost"] = DebtCost;
                                    NewRow["Percentage1"] = ProgressVal;
                                }
                            }
                            if (DebtType == 2)
                            {
                                CalcDefectsCost = Convert.ToDecimal(Rows[0]["CalcDefectsCost"]);

                                if (CalcDefectsCost > 0)
                                {
                                    ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcDefectsCost));
                                    ProgressVal = ProgressVal * 100;
                                    NewRow["CalcDefectsCost"] = DebtCost;
                                    NewRow["Percentage2"] = ProgressVal;
                                }
                            }
                            if (DebtType == 3)
                            {
                                CalcProductionErrorsCost = Convert.ToDecimal(Rows[0]["CalcProductionErrorsCost"]);

                                if (CalcProductionErrorsCost > 0)
                                {
                                    ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcProductionErrorsCost));
                                    ProgressVal = ProgressVal * 100;
                                    NewRow["CalcProductionErrorsCost"] = DebtCost;
                                    NewRow["Percentage3"] = ProgressVal;
                                }
                            }
                            if (DebtType == 4)
                            {
                                CalcZOVErrorsCost = Convert.ToDecimal(Rows[0]["CalcZOVErrorsCost"]);

                                if (CalcZOVErrorsCost > 0)
                                {
                                    ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcZOVErrorsCost));
                                    ProgressVal = ProgressVal * 100;
                                    NewRow["CalcZOVErrorsCost"] = DebtCost;
                                    NewRow["Percentage4"] = ProgressVal;
                                }
                            }

                            ClientWriteOffDT.Rows.Add(NewRow);
                        }
                    }
                }
            }
            ClientWriteOffBS.MoveFirst();
            ClientWriteOffDT.DefaultView.Sort = "Percentage1 DESC, Percentage2 DESC, Percentage3 DESC, Percentage4 DESC, Percentage5 DESC";
        }

        public void GetClientGroupWriteOff(DateTime DateFrom, DateTime DateTo)
        {
            string ClientsFilter = string.Empty;

            if (Clients.Count > 0)
                ClientsFilter = " AND ClientID IN (" + string.Join(",", Clients.OfType<Int32>().ToArray()) + ")";
            else
                ClientsFilter = " AND ClientID = -1";

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT infiniu2_zovreference.dbo.ClientsGroups.ClientGroupName, infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID,
            SUM(PaymentDetail.SamplesWriteOffCost) AS SamplesWriteOffCost, SUM(PaymentDetail.DebtCost) AS DebtCost, PaymentDetail.DebtTypeID FROM PaymentDetail
            INNER JOIN infiniu2_zovreference.dbo.Clients ON (PaymentDetail.ClientName = infiniu2_zovreference.dbo.Clients.ClientName" + ClientsFilter + @")
            INNER JOIN infiniu2_zovreference.dbo.ClientsGroups ON infiniu2_zovreference.dbo.Clients.ClientGroupID = infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID
            WHERE PaymentWeekID IN (SELECT PaymentWeekID FROM PaymentWeeks WHERE CAST(DateFrom AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") +
            "' AND CAST(DateTo AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + @"')
            GROUP BY infiniu2_zovreference.dbo.ClientsGroups.ClientGroupName, infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID, PaymentDetail.DebtTypeID
            ORDER BY infiniu2_zovreference.dbo.ClientsGroups.ClientGroupName, infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID, PaymentDetail.DebtTypeID",
            ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    ClientsGroupWriteOffDT.Clear();
                    if (DA.Fill(DT) == 0)
                        return;

                    int DebtType = 0;
                    decimal CalcDebtCost = 0;
                    decimal CalcDefectsCost = 0;
                    decimal CalcProductionErrorsCost = 0;
                    decimal CalcZOVErrorsCost = 0;
                    decimal SamplesWriteOffCost = 0;
                    decimal DebtCost = 0;
                    decimal SamplesCost = 0;
                    decimal ProgressVal = 0;

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DebtType = Convert.ToInt32(DT.Rows[i]["DebtTypeID"]);

                        if (DebtType == 0)
                            SamplesWriteOffCost += Convert.ToDecimal(DT.Rows[i]["SamplesWriteOffCost"]);
                        if (DebtType == 1)
                            CalcDebtCost += Convert.ToDecimal(DT.Rows[i]["DebtCost"]);
                        if (DebtType == 2)
                            CalcDefectsCost += Convert.ToDecimal(DT.Rows[i]["DebtCost"]);
                        if (DebtType == 3)
                            CalcProductionErrorsCost += Convert.ToDecimal(DT.Rows[i]["DebtCost"]);
                        if (DebtType == 4)
                            CalcZOVErrorsCost += Convert.ToDecimal(DT.Rows[i]["DebtCost"]);
                    }

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DebtCost = Convert.ToDecimal(DT.Rows[i]["DebtCost"]);
                        SamplesCost = Convert.ToDecimal(DT.Rows[i]["SamplesWriteOffCost"]);
                        DebtType = Convert.ToInt32(DT.Rows[i]["DebtTypeID"]);
                        if (DebtCost == 0 && SamplesCost == 0)
                            continue;

                        DataRow NewRow = ClientsGroupWriteOffDT.NewRow();
                        NewRow["DebtTypeID"] = DT.Rows[i]["DebtTypeID"];
                        NewRow["ClientGroupID"] = DT.Rows[i]["ClientGroupID"];
                        NewRow["ClientGroupName"] = DT.Rows[i]["ClientGroupName"];

                        if (DebtType == 0)
                        {
                            if (SamplesWriteOffCost > 0)
                                ProgressVal = Convert.ToDecimal(Convert.ToDecimal(SamplesCost) / Convert.ToDecimal(SamplesWriteOffCost));
                            ProgressVal = ProgressVal * 100;
                            NewRow["SamplesWriteOffCost"] = SamplesCost;
                            NewRow["Percentage5"] = ProgressVal;
                        }
                        if (DebtType == 1)
                        {
                            if (CalcDebtCost > 0)
                                ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcDebtCost));
                            ProgressVal = ProgressVal * 100;
                            NewRow["CalcDebtCost"] = DebtCost;
                            NewRow["Percentage1"] = ProgressVal;
                        }
                        if (DebtType == 2)
                        {
                            if (CalcDefectsCost > 0)
                                ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcDefectsCost));
                            ProgressVal = ProgressVal * 100;
                            NewRow["CalcDefectsCost"] = DebtCost;
                            NewRow["Percentage2"] = ProgressVal;
                        }
                        if (DebtType == 3)
                        {
                            if (CalcProductionErrorsCost > 0)
                                ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcProductionErrorsCost));
                            ProgressVal = ProgressVal * 100;
                            NewRow["CalcProductionErrorsCost"] = DebtCost;
                            NewRow["Percentage3"] = ProgressVal;
                        }
                        if (DebtType == 4)
                        {
                            if (CalcZOVErrorsCost > 0)
                                ProgressVal = Convert.ToDecimal(Convert.ToDecimal(DebtCost) / Convert.ToDecimal(CalcZOVErrorsCost));
                            ProgressVal = ProgressVal * 100;
                            NewRow["CalcZOVErrorsCost"] = DebtCost;
                            NewRow["Percentage4"] = ProgressVal;
                        }

                        ClientsGroupWriteOffDT.Rows.Add(NewRow);
                    }
                }
            }
            ClientsGroupWriteOffBS.MoveFirst();
            ClientsGroupWriteOffDT.DefaultView.Sort = "Percentage1 DESC, Percentage2 DESC, Percentage3 DESC, Percentage4 DESC, Percentage5 DESC";
        }

        public void GetClientDispatch(DateTime DateFrom, DateTime DateTo)
        {
            string ClientsFilter = string.Empty;

            if (Clients.Count > 0)
                ClientsFilter = " AND MainOrders.ClientID IN (" + string.Join(",", Clients.OfType<Int32>().ToArray()) + ")";
            else
                ClientsFilter = " AND MainOrders.ClientID = -1";

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT infiniu2_zovreference.dbo.Clients.ClientName, Packages.PackageStatusID, 
            infiniu2_zovreference.dbo.Clients.ClientGroupID, MainOrders.ClientID, OrderID, FrontsOrders.Cost AS Cost
            FROM FrontsOrders
            INNER JOIN PackageDetails ON FrontsOrders.FrontsOrdersID = PackageDetails.OrderID
            INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID AND ProductType = 0
            INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID
            INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND CAST(DispatchDate AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") + "'" +
            " AND CAST(DispatchDate AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + "'" + ClientsFilter +
            @" INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID
            INNER JOIN infiniu2_zovreference.dbo.ClientsGroups ON infiniu2_zovreference.dbo.Clients.ClientGroupID = infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID
            GROUP BY infiniu2_zovreference.dbo.Clients.ClientName, Packages.PackageStatusID, infiniu2_zovreference.dbo.Clients.ClientGroupID, MainOrders.ClientID, OrderID, Cost
            UNION 
            SELECT infiniu2_zovreference.dbo.Clients.ClientName, Packages.PackageStatusID, 
            infiniu2_zovreference.dbo.Clients.ClientGroupID, MainOrders.ClientID, OrderID, DecorOrders.Cost AS Cost
            FROM DecorOrders
            INNER JOIN PackageDetails ON DecorOrders.DecorOrderID = PackageDetails.OrderID
            INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID AND ProductType = 1
            INNER JOIN MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID
            INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND CAST(DispatchDate AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") + "'" +
            " AND CAST(DispatchDate AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + "'" + ClientsFilter +
            @" INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID
            INNER JOIN infiniu2_zovreference.dbo.ClientsGroups ON infiniu2_zovreference.dbo.Clients.ClientGroupID = infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID
            GROUP BY infiniu2_zovreference.dbo.Clients.ClientName, Packages.PackageStatusID, infiniu2_zovreference.dbo.Clients.ClientGroupID, MainOrders.ClientID, OrderID, Cost
            ORDER BY infiniu2_zovreference.dbo.Clients.ClientName, Packages.PackageStatusID, infiniu2_zovreference.dbo.Clients.ClientGroupID, MainOrders.ClientID, OrderID, Cost",
            ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    ClientDispatchDT.Clear();
                    if (DA.Fill(DT) == 0)
                        return;

                    int ClientID = 0;
                    int ClientGroupID = 0;
                    int DispatchID = 0;
                    int PackageStatusID = 0;
                    decimal DispatchCost = 0;
                    decimal NotDispatchCost = 0;
                    decimal Cost = 0;
                    decimal ProgressVal = 0;

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        Cost = Convert.ToDecimal(DT.Rows[i]["Cost"]);
                        if (Cost == 0)
                            continue;

                        ClientID = Convert.ToInt32(DT.Rows[i]["ClientID"]);
                        ClientGroupID = Convert.ToInt32(DT.Rows[i]["ClientGroupID"]);
                        PackageStatusID = Convert.ToInt32(DT.Rows[i]["PackageStatusID"]);

                        if (PackageStatusID == 3)
                            DispatchID = 1;
                        else
                            DispatchID = 2;

                        DataRow[] Rows = ClientsGroupDispatchDT.Select("ClientGroupID = " + ClientGroupID + " AND DispatchID = " + DispatchID);
                        if (Rows.Count() > 0)
                        {
                            DataRow[] CRows = ClientDispatchDT.Select("ClientID = " + ClientID + " AND DispatchID = " + DispatchID);
                            if (CRows.Count() == 0)
                            {
                                DataRow NewRow = ClientDispatchDT.NewRow();
                                NewRow["ClientID"] = DT.Rows[i]["ClientID"];
                                NewRow["ClientGroupID"] = DT.Rows[i]["ClientGroupID"];
                                NewRow["DispatchID"] = DispatchID;
                                NewRow["ClientName"] = DT.Rows[i]["ClientName"];

                                if (DispatchID == 1)
                                {
                                    DispatchCost = Convert.ToDecimal(Rows[0]["DispatchCost"]);

                                    if (DispatchCost > 0)
                                    {
                                        ProgressVal = Convert.ToDecimal(Convert.ToDecimal(Cost) / Convert.ToDecimal(DispatchCost));
                                        ProgressVal = ProgressVal * 100;
                                        NewRow["DispatchCost"] = Cost;
                                        NewRow["Percentage1"] = ProgressVal;
                                    }
                                }
                                if (DispatchID == 2)
                                {
                                    NotDispatchCost = Convert.ToDecimal(Rows[0]["NotDispatchCost"]);

                                    if (NotDispatchCost > 0)
                                    {
                                        ProgressVal = Convert.ToDecimal(Convert.ToDecimal(Cost) / Convert.ToDecimal(NotDispatchCost));
                                        ProgressVal = ProgressVal * 100;
                                        NewRow["NotDispatchCost"] = Cost;
                                        NewRow["Percentage2"] = ProgressVal;
                                    }
                                }

                                ClientDispatchDT.Rows.Add(NewRow);
                            }
                            else
                            {
                                if (DispatchID == 1)
                                {
                                    DispatchCost = Convert.ToDecimal(Rows[0]["DispatchCost"]);

                                    if (DispatchCost > 0)
                                    {
                                        ProgressVal = Convert.ToDecimal((Convert.ToDecimal(CRows[0]["DispatchCost"]) + Convert.ToDecimal(Cost)) / Convert.ToDecimal(DispatchCost));
                                        ProgressVal = ProgressVal * 100;
                                        CRows[0]["DispatchCost"] = Convert.ToDecimal(CRows[0]["DispatchCost"]) + Convert.ToDecimal(Cost);
                                        CRows[0]["Percentage1"] = ProgressVal;
                                    }
                                }
                                if (DispatchID == 2)
                                {
                                    NotDispatchCost = Convert.ToDecimal(Rows[0]["NotDispatchCost"]);

                                    if (NotDispatchCost > 0)
                                    {
                                        ProgressVal = Convert.ToDecimal((Convert.ToDecimal(CRows[0]["NotDispatchCost"]) + Convert.ToDecimal(Cost)) / Convert.ToDecimal(NotDispatchCost));
                                        ProgressVal = ProgressVal * 100;
                                        CRows[0]["NotDispatchCost"] = Convert.ToDecimal(CRows[0]["NotDispatchCost"]) + Convert.ToDecimal(Cost);
                                        CRows[0]["Percentage2"] = ProgressVal;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            ClientDispatchBS.MoveFirst();
            ClientDispatchDT.DefaultView.Sort = "Percentage1 DESC, Percentage2 DESC";
        }

        public void GetClientGroupDispatch(DateTime DateFrom, DateTime DateTo)
        {
            string ClientsFilter = string.Empty;

            if (Clients.Count > 0)
                ClientsFilter = " AND MainOrders.ClientID IN (" + string.Join(",", Clients.OfType<Int32>().ToArray()) + ")";
            else
                ClientsFilter = " AND MainOrders.ClientID = -1";

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT infiniu2_zovreference.dbo.ClientsGroups.ClientGroupName, Packages.PackageStatusID, 
            infiniu2_zovreference.dbo.Clients.ClientGroupID, OrderID, FrontsOrders.Cost AS Cost
            FROM FrontsOrders
            INNER JOIN PackageDetails ON FrontsOrders.FrontsOrdersID = PackageDetails.OrderID
            INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID AND ProductType = 0
            INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID
            INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND CAST(DispatchDate AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") + "'" +
            " AND CAST(DispatchDate AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + "'" + ClientsFilter +
            @" INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID
            INNER JOIN infiniu2_zovreference.dbo.ClientsGroups ON infiniu2_zovreference.dbo.Clients.ClientGroupID = infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID
            GROUP BY infiniu2_zovreference.dbo.ClientsGroups.ClientGroupName, Packages.PackageStatusID, infiniu2_zovreference.dbo.Clients.ClientGroupID, OrderID, Cost
            UNION 
            SELECT infiniu2_zovreference.dbo.ClientsGroups.ClientGroupName, Packages.PackageStatusID, 
            infiniu2_zovreference.dbo.Clients.ClientGroupID, OrderID, DecorOrders.Cost AS Cost
            FROM DecorOrders
            INNER JOIN PackageDetails ON DecorOrders.DecorOrderID = PackageDetails.OrderID
            INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID AND ProductType = 1
            INNER JOIN MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID
            INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND CAST(DispatchDate AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") + "'" +
            " AND CAST(DispatchDate AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + "'" + ClientsFilter +
            @" INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID
            INNER JOIN infiniu2_zovreference.dbo.ClientsGroups ON infiniu2_zovreference.dbo.Clients.ClientGroupID = infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID
            GROUP BY infiniu2_zovreference.dbo.ClientsGroups.ClientGroupName, Packages.PackageStatusID, infiniu2_zovreference.dbo.Clients.ClientGroupID, OrderID, Cost
            ORDER BY infiniu2_zovreference.dbo.ClientsGroups.ClientGroupName, Packages.PackageStatusID, infiniu2_zovreference.dbo.Clients.ClientGroupID, OrderID, Cost",
            ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    ClientsGroupDispatchDT.Clear();
                    if (DA.Fill(DT) == 0)
                        return;

                    int ClientGroupID = 0;
                    int DispatchID = 0;
                    int PackageStatusID = 0;
                    decimal DispatchCost = 0;
                    decimal NotDispatchCost = 0;
                    decimal Cost = 0;
                    decimal ProgressVal = 0;

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        PackageStatusID = Convert.ToInt32(DT.Rows[i]["PackageStatusID"]);

                        if (PackageStatusID == 3)
                            DispatchCost += Convert.ToDecimal(DT.Rows[i]["Cost"]);
                        else
                            NotDispatchCost += Convert.ToDecimal(DT.Rows[i]["Cost"]);
                    }

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        Cost = Convert.ToDecimal(DT.Rows[i]["Cost"]);
                        if (Cost == 0)
                            continue;

                        ClientGroupID = Convert.ToInt32(DT.Rows[i]["ClientGroupID"]);
                        PackageStatusID = Convert.ToInt32(DT.Rows[i]["PackageStatusID"]);

                        if (PackageStatusID == 3)
                            DispatchID = 1;
                        else
                            DispatchID = 2;

                        DataRow[] Rows = ClientsGroupDispatchDT.Select("ClientGroupID = " + ClientGroupID + " AND DispatchID = " + DispatchID);
                        if (Rows.Count() == 0)
                        {
                            DataRow NewRow = ClientsGroupDispatchDT.NewRow();
                            NewRow["ClientGroupID"] = DT.Rows[i]["ClientGroupID"];
                            NewRow["DispatchID"] = DispatchID;
                            NewRow["ClientGroupName"] = DT.Rows[i]["ClientGroupName"];

                            if (DispatchID == 1)
                            {
                                if (DispatchCost > 0)
                                    ProgressVal = Convert.ToDecimal(Convert.ToDecimal(Cost) / Convert.ToDecimal(DispatchCost));
                                ProgressVal = ProgressVal * 100;
                                NewRow["DispatchCost"] = Cost;
                                NewRow["Percentage1"] = ProgressVal;
                            }
                            if (DispatchID == 2)
                            {
                                if (NotDispatchCost > 0)
                                    ProgressVal = Convert.ToDecimal(Convert.ToDecimal(Cost) / Convert.ToDecimal(NotDispatchCost));
                                ProgressVal = ProgressVal * 100;
                                NewRow["NotDispatchCost"] = Cost;
                                NewRow["Percentage2"] = ProgressVal;
                            }
                            ClientsGroupDispatchDT.Rows.Add(NewRow);
                        }
                        else
                        {
                            if (DispatchID == 1)
                            {
                                if (DispatchCost > 0)
                                    ProgressVal = Convert.ToDecimal((Cost + Convert.ToDecimal(Rows[0]["DispatchCost"])) / Convert.ToDecimal(DispatchCost));
                                ProgressVal = ProgressVal * 100;
                                Rows[0]["DispatchCost"] = Cost + Convert.ToDecimal(Rows[0]["DispatchCost"]);
                                Rows[0]["Percentage1"] = ProgressVal;
                            }
                            if (DispatchID == 2)
                            {
                                if (NotDispatchCost > 0)
                                    ProgressVal = Convert.ToDecimal((Cost + Convert.ToDecimal(Rows[0]["NotDispatchCost"])) / Convert.ToDecimal(NotDispatchCost));
                                ProgressVal = ProgressVal * 100;
                                Rows[0]["NotDispatchCost"] = Cost + Convert.ToDecimal(Rows[0]["NotDispatchCost"]);
                                Rows[0]["Percentage2"] = ProgressVal;
                            }
                        }
                    }
                }
            }
            ClientsGroupDispatchBS.MoveFirst();
            ClientsGroupDispatchDT.DefaultView.Sort = "Percentage1 DESC, Percentage2 DESC";
        }

        public void GetClientSamples(DateTime DateFrom, DateTime DateTo)
        {
            string ClientsFilter = string.Empty;

            if (Clients.Count > 0)
                ClientsFilter = " AND MainOrders.ClientID IN (" + string.Join(",", Clients.OfType<Int32>().ToArray()) + ")";
            else
                ClientsFilter = " AND MainOrders.ClientID = -1";

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ClientName, infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID, MainOrders.ClientID, IsSample, 
            SUM(OrderCost) AS OrderCost FROM MainOrders
            INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID
            INNER JOIN infiniu2_zovreference.dbo.ClientsGroups ON infiniu2_zovreference.dbo.Clients.ClientGroupID = infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID
            WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE CAST(DispatchDate AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") +
            "' AND CAST(DispatchDate AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + "')" + ClientsFilter +
            " GROUP BY ClientName, infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID, MainOrders.ClientID, IsSample" +
            " ORDER BY ClientName, infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID, MainOrders.ClientID, IsSample",
            ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return;

                    int ClientID = 0;
                    int ClientGroupID = 0;
                    decimal OrderCost = 0;
                    decimal SampleCost = 0;
                    decimal ProgressVal = 0;

                    foreach (DataRow SampleRow in DT.Select("IsSample = 1"))
                    {
                        ClientGroupID = Convert.ToInt32(SampleRow["ClientGroupID"]);
                        ClientID = Convert.ToInt32(SampleRow["ClientID"]);
                        SampleCost = Convert.ToDecimal(SampleRow["OrderCost"]);
                        OrderCost = Convert.ToDecimal(SampleRow["OrderCost"]);

                        DataRow[] Rows = DT.Select("IsSample = 0 AND ClientID = " + ClientID);

                        if (Rows.Count() > 0)
                        {
                            OrderCost += Convert.ToDecimal(Rows[0]["OrderCost"]);
                        }

                        DataRow NewRow = ClientDispatchDT.NewRow();
                        NewRow["DispatchID"] = 3;
                        NewRow["ClientGroupID"] = ClientGroupID;
                        NewRow["ClientID"] = ClientID;
                        NewRow["ClientName"] = SampleRow["ClientName"];

                        if (OrderCost > 0)
                            ProgressVal = Convert.ToDecimal(Convert.ToDecimal(SampleCost) / Convert.ToDecimal(OrderCost));
                        ProgressVal = ProgressVal * 100;
                        NewRow["SamplesCost"] = SampleCost;
                        NewRow["Percentage3"] = ProgressVal;

                        ClientDispatchDT.Rows.Add(NewRow);
                    }
                }
            }
            ClientDispatchBS.MoveFirst();
            ClientDispatchDT.DefaultView.Sort = "Percentage1 DESC, Percentage2 DESC, Percentage3 DESC";
        }

        public void GetClientGroupSamples(DateTime DateFrom, DateTime DateTo)
        {
            string ClientsFilter = string.Empty;

            if (Clients.Count > 0)
                ClientsFilter = " AND MainOrders.ClientID IN (" + string.Join(",", Clients.OfType<Int32>().ToArray()) + ")";
            else
                ClientsFilter = " AND MainOrders.ClientID = -1";

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT ClientGroupName, infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID, IsSample, 
            SUM(OrderCost) AS OrderCost FROM MainOrders
            INNER JOIN infiniu2_zovreference.dbo.Clients ON MainOrders.ClientID = infiniu2_zovreference.dbo.Clients.ClientID
            INNER JOIN infiniu2_zovreference.dbo.ClientsGroups ON infiniu2_zovreference.dbo.Clients.ClientGroupID = infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID
            WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE CAST(DispatchDate AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") +
            "' AND CAST(DispatchDate AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + "')" + ClientsFilter +
            " GROUP BY ClientGroupName, infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID, IsSample" +
            " ORDER BY ClientGroupName, infiniu2_zovreference.dbo.ClientsGroups.ClientGroupID, IsSample",
            ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return;

                    int ClientGroupID = 0;
                    decimal OrderCost = 0;
                    decimal SampleCost = 0;
                    decimal ProgressVal = 0;

                    foreach (DataRow SampleRow in DT.Select("IsSample = 1"))
                    {
                        ClientGroupID = Convert.ToInt32(SampleRow["ClientGroupID"]);
                        SampleCost = Convert.ToDecimal(SampleRow["OrderCost"]);
                        OrderCost = Convert.ToDecimal(SampleRow["OrderCost"]);

                        DataRow[] Rows = DT.Select("IsSample = 0 AND ClientGroupID = " + ClientGroupID);

                        if (Rows.Count() > 0)
                        {
                            OrderCost += Convert.ToDecimal(Rows[0]["OrderCost"]);
                        }

                        DataRow NewRow = ClientsGroupDispatchDT.NewRow();
                        NewRow["DispatchID"] = 3;
                        NewRow["ClientGroupID"] = ClientGroupID;
                        NewRow["ClientGroupName"] = SampleRow["ClientGroupName"];

                        if (OrderCost > 0)
                            ProgressVal = Convert.ToDecimal(Convert.ToDecimal(SampleCost) / Convert.ToDecimal(OrderCost));
                        ProgressVal = ProgressVal * 100;
                        NewRow["SamplesCost"] = SampleCost;
                        NewRow["Percentage3"] = ProgressVal;

                        ClientsGroupDispatchDT.Rows.Add(NewRow);
                    }
                }
            }
            ClientsGroupDispatchBS.MoveFirst();
            ClientsGroupDispatchDT.DefaultView.Sort = "Percentage1 DESC, Percentage2 DESC, Percentage3 DESC";
        }

        public void FilterClientDebts(int ClientGroupID, int DebtTypeID)
        {
            ClientCalcDebtsBS.RemoveFilter();
            ClientCalcDebtsBS.Filter = "ClientGroupID = " + ClientGroupID + " AND DebtTypeID = " + DebtTypeID;
            ClientCalcDebtsBS.MoveFirst();
        }

        public void FilterClientGroupDebts(int DebtTypeID)
        {
            ClientsGroupCalcDebtsBS.RemoveFilter();
            ClientsGroupCalcDebtsBS.Filter = "DebtTypeID = " + DebtTypeID;
            ClientsGroupCalcDebtsBS.MoveFirst();
        }

        public void FilterClientWriteOff(int ClientGroupID, int DebtTypeID)
        {
            ClientWriteOffBS.RemoveFilter();
            ClientWriteOffBS.Filter = "ClientGroupID = " + ClientGroupID + " AND DebtTypeID = " + DebtTypeID;
            ClientWriteOffBS.MoveFirst();
        }

        public void FilterClientGroupWriteOff(int DebtTypeID)
        {
            ClientsGroupWriteOffBS.RemoveFilter();
            ClientsGroupWriteOffBS.Filter = "DebtTypeID = " + DebtTypeID;
            ClientsGroupWriteOffBS.MoveFirst();
        }

        public void FilterClientGroupDispatch(int DispatchID)
        {
            ClientsGroupDispatchBS.RemoveFilter();
            ClientsGroupDispatchBS.Filter = "DispatchID = " + DispatchID;
            ClientsGroupDispatchBS.MoveFirst();
        }

        public void FilterClientDispatchID(int ClientGroupID, int DispatchID)
        {
            ClientDispatchBS.RemoveFilter();
            ClientDispatchBS.Filter = "ClientGroupID = " + ClientGroupID + " AND DispatchID = " + DispatchID;
            ClientDispatchBS.MoveFirst();
        }

        private decimal GetCost(string ColumnName, DateTime DateFrom, DateTime DateTo)
        {
            decimal Cost = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, FactoryID, OrderCost, " + ColumnName +
                " FROM MainOrders WHERE MegaOrderID IN" +
                "(SELECT MegaOrderID FROM MegaOrders WHERE DispatchDate >= '" +
                DateFrom.ToString("yyyy-MM-dd") + "' AND DispatchDate <= '" +
                DateTo.ToString("yyyy-MM-dd") + "')",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in DT.Rows)
                    {
                        if (Row["FactoryID"].ToString() == "1")
                            Cost += Convert.ToDecimal(Row[ColumnName]);

                        if (Row["FactoryID"].ToString() == "2")
                            Cost += Convert.ToDecimal(Row[ColumnName]);
                    }
                }
            }

            return Cost;
        }

        private void AddResultRow(String Parameter, int DispatchID, object Cost)
        {
            DataRow NewRow = ResultTotalDataTable.NewRow();
            NewRow["Parameter"] = Parameter;
            NewRow["DispatchID"] = DispatchID;
            NewRow["Cost"] = Cost;
            ResultTotalDataTable.Rows.Add(NewRow);
        }

        private void AddWriteOffRow(String Parameter, int DebtTypeID, object Cost)
        {
            DataRow NewRow = WriteOffDataTable.NewRow();
            NewRow["Parameter"] = Parameter;
            NewRow["DebtTypeID"] = DebtTypeID;
            NewRow["Cost"] = Cost;
            WriteOffDataTable.Rows.Add(NewRow);
        }

        private void AddCalcWriteOffRow(String Parameter, int DebtTypeID, object Cost)
        {
            DataRow NewRow = CalcWriteOffDataTable.NewRow();
            NewRow["Parameter"] = Parameter;
            NewRow["DebtTypeID"] = DebtTypeID;
            NewRow["Cost"] = Cost;
            CalcWriteOffDataTable.Rows.Add(NewRow);
        }

        public string ResultText
        {
            get { return sResult; }
        }

        public string CalcWriteOffResult
        {
            get { return sCalcWriteOffResult; }
        }

        public string WriteOffResult
        {
            get { return sWriteOffResult; }
        }

        public void CalcDebtsOnScaner(DateTime DateFrom, DateTime DateTo)
        {
            string DocNumber = string.Empty;
            string SelectCommand = string.Empty;

            int MainOrderID = 0;

            bool IsSample = false;

            decimal TotalCost = 0;
            decimal DispatchedCost = 0;
            decimal DispatchedDebtCost = 0;
            decimal TotalWriteOff = 0;
            decimal ProfitCost = 0;
            decimal SamplesCost = 0;

            decimal CalcDebtCost = 0;
            decimal CalcDefectsCost = 0;
            decimal CalcProductionErrorsCost = 0;
            decimal CalcZOVErrorsCost = 0;
            decimal TotalCalcWriteOffCost = 0;

            decimal SamplesWriteOffCost = 0;
            decimal WriteOffDebtCost = 0;
            decimal WriteOffDefectsCost = 0;
            decimal WriteOffProductionErrorsCost = 0;
            decimal WriteOffZOVErrorsCost = 0;
            decimal TotalWriteOffCost = 0;

            decimal ErrorWriteOffCost = 0;
            decimal CompensationCost = 0;

            string ClientsFilter = string.Empty;

            DataTable DebtsDT = new DataTable();
            DataTable DispatchDT = new DataTable();
            DataTable PaymentDetailDT = new DataTable();

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 1,
                NumberGroupSeparator = " ",
                NumberDecimalSeparator = ","
            };
            ResultTotalDataTable.Clear();
            CalcWriteOffDataTable.Clear();
            WriteOffDataTable.Clear();

            if (Clients.Count > 0)
            {
                ClientsFilter = " AND ClientID IN (" + string.Join(",", Clients.OfType<Int32>().ToArray()) + ")";
            }
            else
            {
                ClientsFilter = " AND ClientID = -1";
            }

            SelectCommand = @"SELECT Packages.PackageStatusID, OrderID, FrontsOrders.Cost AS Cost
                FROM FrontsOrders
                INNER JOIN PackageDetails ON FrontsOrders.FrontsOrdersID = PackageDetails.OrderID
                INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID AND ProductType = 0
                INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND CAST(DispatchDate AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") + "'" +
                " AND CAST(DispatchDate AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + "'" + ClientsFilter +
                " GROUP BY Packages.PackageStatusID, OrderID, Cost" +
                @" UNION 
                SELECT Packages.PackageStatusID, OrderID, DecorOrders.Cost AS Cost
                FROM DecorOrders
                INNER JOIN PackageDetails ON DecorOrders.DecorOrderID = PackageDetails.OrderID
                INNER JOIN Packages ON PackageDetails.PackageID = Packages.PackageID AND ProductType = 1
                INNER JOIN MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID
                INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND CAST(DispatchDate AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") + "'" +
                " AND CAST(DispatchDate AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + "'" + ClientsFilter +
                " GROUP BY Packages.PackageStatusID, OrderID, Cost" +
                " ORDER BY Packages.PackageStatusID, OrderID, Cost";
            //Отгружено
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DispatchDT);
            }
            for (int i = 0; i < DispatchDT.Rows.Count; i++)
            {
                if (DispatchDT.Rows[i]["PackageStatusID"].ToString() == "3")
                    DispatchedCost += Convert.ToDecimal(DispatchDT.Rows[i]["Cost"]);
                TotalCost += Convert.ToDecimal(DispatchDT.Rows[i]["Cost"]);
            }

            SelectCommand = @"SELECT * FROM PaymentDetail
                INNER JOIN infiniu2_zovreference.dbo.Clients ON (PaymentDetail.ClientName = infiniu2_zovreference.dbo.Clients.ClientName" + ClientsFilter + ")" +
                @" WHERE PaymentWeekID IN (SELECT PaymentWeekID FROM PaymentWeeks WHERE CAST(DateFrom AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") +
                "' AND CAST(DateTo AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + "')";
            //Списание в минусовом
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(PaymentDetailDT);

                for (int i = 0; i < PaymentDetailDT.Rows.Count; i++)
                {
                    DocNumber = PaymentDetailDT.Rows[i]["DocNumber"].ToString();

                    if (Convert.ToDecimal(PaymentDetailDT.Rows[i]["DebtCost"]) > 0)
                    {
                        if (PaymentDetailDT.Rows[i]["DebtTypeID"].ToString() == "1")
                        {
                            WriteOffDebtCost += Convert.ToDecimal(PaymentDetailDT.Rows[i]["DebtCost"]);
                        }
                        if (PaymentDetailDT.Rows[i]["DebtTypeID"].ToString() == "2")
                        {
                            WriteOffDefectsCost += Convert.ToDecimal(PaymentDetailDT.Rows[i]["DebtCost"]);
                        }
                        if (PaymentDetailDT.Rows[i]["DebtTypeID"].ToString() == "3")
                        {
                            WriteOffProductionErrorsCost += Convert.ToDecimal(PaymentDetailDT.Rows[i]["DebtCost"]);
                        }
                        if (PaymentDetailDT.Rows[i]["DebtTypeID"].ToString() == "4")
                        {
                            WriteOffZOVErrorsCost += Convert.ToDecimal(PaymentDetailDT.Rows[i]["DebtCost"]);
                        }
                    }

                    if (Convert.ToDecimal(PaymentDetailDT.Rows[i]["SamplesWriteOffCost"]) > 0)
                    {
                        SamplesWriteOffCost += Convert.ToDecimal(PaymentDetailDT.Rows[i]["SamplesWriteOffCost"]);
                    }
                }
            }

            SelectCommand = @"SELECT * FROM PaymentWeeks WHERE DateFrom BETWEEN '" + DateFrom.ToString("yyyy-MM-dd") +
            "' AND '" + DateTo.ToString("yyyy-MM-dd") + "'";
            //Ошибки списания и компенсации
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                PaymentDetailDT.Clear();
                DA.Fill(PaymentDetailDT);

                for (int i = 0; i < PaymentDetailDT.Rows.Count; i++)
                {
                    ErrorWriteOffCost += Convert.ToDecimal(PaymentDetailDT.Rows[i]["ErrorWriteOffCost"]);
                    CompensationCost += Convert.ToDecimal(PaymentDetailDT.Rows[i]["CompensationCost"]);
                }
            }

            SelectCommand = @"SELECT MainOrderID, ClientID, DebtTypeID, IsSample, OrderCost, DispatchedCost, DispatchedDebtCost, 
            SamplesWriteOffCost, CalcDebtCost, WriteOffDebtCost, WriteOffDefectsCost, WriteOffProductionErrorsCost, WriteOffZOVErrorsCost,
            TotalWriteOffCost, IncomeCost, ProfitCost FROM MainOrders
            WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE CAST(DispatchDate AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") +
            "' AND CAST(DispatchDate AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + "')" + ClientsFilter;
            //Списание в расчете
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DebtsDT);

                for (int i = 0; i < DebtsDT.Rows.Count; i++)
                {
                    MainOrderID = Convert.ToInt32(DebtsDT.Rows[i]["MainOrderID"]);
                    IsSample = Convert.ToBoolean(DebtsDT.Rows[i]["IsSample"]);
                    if (IsSample)
                        SamplesCost += Convert.ToDecimal(DebtsDT.Rows[i]["OrderCost"]);

                    if (Convert.ToDecimal(DebtsDT.Rows[i]["CalcDebtCost"]) > 0)
                    {
                        if (DebtsDT.Rows[i]["DebtTypeID"].ToString() == "1")
                        {
                            CalcDebtCost += Convert.ToDecimal(DebtsDT.Rows[i]["CalcDebtCost"]);
                        }
                        if (DebtsDT.Rows[i]["DebtTypeID"].ToString() == "2")
                        {
                            CalcDefectsCost += Convert.ToDecimal(DebtsDT.Rows[i]["CalcDebtCost"]);
                        }
                        if (DebtsDT.Rows[i]["DebtTypeID"].ToString() == "3")
                        {
                            CalcProductionErrorsCost += Convert.ToDecimal(DebtsDT.Rows[i]["CalcDebtCost"]);
                        }
                        if (DebtsDT.Rows[i]["DebtTypeID"].ToString() == "4")
                        {
                            CalcZOVErrorsCost += Convert.ToDecimal(DebtsDT.Rows[i]["CalcDebtCost"]);
                        }
                    }
                }

                TotalCalcWriteOffCost = CalcDebtCost + CalcDefectsCost + CalcProductionErrorsCost + CalcZOVErrorsCost;
                TotalWriteOffCost = SamplesWriteOffCost + WriteOffDebtCost + WriteOffDefectsCost + WriteOffProductionErrorsCost + WriteOffZOVErrorsCost;
                TotalWriteOff = TotalCalcWriteOffCost + TotalWriteOffCost;

                ProfitCost = TotalCost - TotalWriteOff;
                DispatchedDebtCost = TotalCost - DispatchedCost;

                sCalcWriteOffResult = "Итого списано: " + TotalCalcWriteOffCost.ToString();

                AddResultRow("Стоимость заказов", 0, TotalCost);
                AddResultRow("Отгружено", 1, DispatchedCost);
                AddResultRow("Не отгружено", 2, DispatchedDebtCost);
                AddResultRow("Списано (в расчете + в минусовом)", 0, TotalWriteOff);
                AddResultRow("Образцы", 3, SamplesCost);
                AddResultRow("Ошибки списания", 0, ErrorWriteOffCost);
                AddResultRow("Компенсация", 0, CompensationCost);
                sResult = "Итого: " + ProfitCost.ToString("N", nfi1);

                AddCalcWriteOffRow("Долги", 1, CalcDebtCost);
                AddCalcWriteOffRow("Браки", 2, CalcDefectsCost);
                AddCalcWriteOffRow("Ошибки пр-ва", 3, CalcProductionErrorsCost);
                AddCalcWriteOffRow("Ошибки ЗОВа", 4, CalcZOVErrorsCost);
                sCalcWriteOffResult = "Итого списано: " + TotalCalcWriteOffCost.ToString("N", nfi1);

                AddWriteOffRow("Долги", 1, WriteOffDebtCost);
                AddWriteOffRow("Браки", 2, WriteOffDefectsCost);
                AddWriteOffRow("Ошибки пр-ва", 3, WriteOffProductionErrorsCost);
                AddWriteOffRow("Ошибки ЗОВа", 4, WriteOffZOVErrorsCost);
                AddWriteOffRow("Образцы", 0, SamplesWriteOffCost);
                sWriteOffResult = "Итого списано: " + TotalWriteOffCost.ToString("N", nfi1);
            }

            DebtsDT.Dispose();
            DispatchDT.Dispose();
        }

        public void CalcDebtsTillScaner(DateTime DateFrom, DateTime DateTo)
        {
            int MainOrderID = 0;

            ArrayList CalcDebtMainOrders = new ArrayList();
            ArrayList CalcDefectsMainOrders = new ArrayList();
            ArrayList CalcProductionErrorsMainOrders = new ArrayList();
            ArrayList CalcZOVErrorsMainOrders = new ArrayList();

            ArrayList SamplesWriteOffMainOrders = new ArrayList();

            ArrayList WriteOffDebtMainOrders = new ArrayList();
            ArrayList WriteOffDefectsMainOrders = new ArrayList();
            ArrayList WriteOffProductionErrorsMainOrders = new ArrayList();
            ArrayList WriteOffZOVErrorsMainOrders = new ArrayList();

            decimal TotalCost = 0;
            decimal DispatchedCost = 0;
            decimal DispatchedDebtCost = 0;
            decimal TotalWriteOff = 0;
            decimal ProfitCost = 0;

            decimal CalcDebtCost = 0;
            decimal CalcDefectsCost = 0;
            decimal CalcProductionErrorsCost = 0;
            decimal CalcZOVErrorsCost = 0;
            decimal TotalCalcWriteOffCost = 0;

            decimal SamplesWriteOffCost = 0;
            decimal WriteOffDebtCost = 0;
            decimal WriteOffDefectsCost = 0;
            decimal WriteOffProductionErrorsCost = 0;
            decimal WriteOffZOVErrorsCost = 0;
            decimal TotalWriteOffCost = 0;

            string ClientsFilter = string.Empty;
            string GroupsFilter = string.Empty;

            DataTable DebtsDT = new DataTable();

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalSeparator = ",",
                CurrencyDecimalDigits = 1,
                NumberGroupSeparator = " ",
                NumberDecimalSeparator = ","
            };
            ResultTotalDataTable.Clear();
            CalcWriteOffDataTable.Clear();
            WriteOffDataTable.Clear();

            if (Clients.Count > 0)
                ClientsFilter = " AND ClientID IN (" + string.Join(",", Clients.OfType<Int32>().ToArray()) + ")";
            else
                ClientsFilter = " AND ClientID = -1";
            if (ClientsGroups.Count > 0)
                GroupsFilter = " AND ClientID IN (SELECT ClientID FROM infiniu2_zovreference.dbo.Clients WHERE ClientGroupID IN (" +
                    string.Join(",", ClientsGroups.OfType<Int32>().ToArray()) + "))";

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT MainOrderID, DebtTypeID, OrderCost, DispatchedCost, DispatchedDebtCost, 
            SamplesWriteOffCost, CalcDebtCost, WriteOffDebtCost, WriteOffDefectsCost, WriteOffProductionErrorsCost, WriteOffZOVErrorsCost,
            TotalWriteOffCost, IncomeCost, ProfitCost FROM MainOrders
            WHERE MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE CAST(DispatchDate AS date) >= '" + DateFrom.ToString("yyyy-MM-dd") +
            "' AND CAST(DispatchDate AS date) <= '" + DateTo.ToString("yyyy-MM-dd") + "')" + GroupsFilter + ClientsFilter,
            ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DebtsDT);

                for (int i = 0; i < DebtsDT.Rows.Count; i++)
                {
                    MainOrderID = Convert.ToInt32(DebtsDT.Rows[i]["MainOrderID"]);

                    TotalCost += Convert.ToDecimal(DebtsDT.Rows[i]["OrderCost"]);
                    DispatchedCost += Convert.ToDecimal(DebtsDT.Rows[i]["DispatchedCost"]);
                    DispatchedDebtCost += Convert.ToDecimal(DebtsDT.Rows[i]["DispatchedDebtCost"]);
                    ProfitCost += Convert.ToDecimal(DebtsDT.Rows[i]["ProfitCost"]);

                    if (Convert.ToDecimal(DebtsDT.Rows[i]["CalcDebtCost"]) > 0)
                    {
                        if (DebtsDT.Rows[i]["DebtTypeID"].ToString() == "1")
                        {
                            CalcDebtCost += Convert.ToDecimal(DebtsDT.Rows[i]["CalcDebtCost"]);
                            CalcDebtMainOrders.Add(MainOrderID);
                        }
                        if (DebtsDT.Rows[i]["DebtTypeID"].ToString() == "2")
                        {
                            CalcDefectsCost += Convert.ToDecimal(DebtsDT.Rows[i]["CalcDebtCost"]);
                            CalcDefectsMainOrders.Add(MainOrderID);
                        }
                        if (DebtsDT.Rows[i]["DebtTypeID"].ToString() == "3")
                        {
                            CalcProductionErrorsCost += Convert.ToDecimal(DebtsDT.Rows[i]["CalcDebtCost"]);
                            CalcProductionErrorsMainOrders.Add(MainOrderID);
                        }
                        if (DebtsDT.Rows[i]["DebtTypeID"].ToString() == "4")
                        {
                            CalcZOVErrorsCost += Convert.ToDecimal(DebtsDT.Rows[i]["CalcDebtCost"]);
                            CalcZOVErrorsMainOrders.Add(MainOrderID);
                        }
                    }

                    if (Convert.ToDecimal(DebtsDT.Rows[i]["WriteOffDebtCost"]) > 0)
                    {
                        WriteOffDebtCost += Convert.ToDecimal(DebtsDT.Rows[i]["WriteOffDebtCost"]);
                        WriteOffDebtMainOrders.Add(MainOrderID);
                    }
                    if (Convert.ToDecimal(DebtsDT.Rows[i]["WriteOffDefectsCost"]) > 0)
                    {
                        WriteOffDefectsCost += Convert.ToDecimal(DebtsDT.Rows[i]["WriteOffDefectsCost"]);
                        WriteOffDefectsMainOrders.Add(MainOrderID);
                    }
                    if (Convert.ToDecimal(DebtsDT.Rows[i]["WriteOffProductionErrorsCost"]) > 0)
                    {
                        WriteOffProductionErrorsCost += Convert.ToDecimal(DebtsDT.Rows[i]["WriteOffProductionErrorsCost"]);
                        WriteOffProductionErrorsMainOrders.Add(MainOrderID);
                    }
                    if (Convert.ToDecimal(DebtsDT.Rows[i]["WriteOffZOVErrorsCost"]) > 0)
                    {
                        WriteOffZOVErrorsCost += Convert.ToDecimal(DebtsDT.Rows[i]["WriteOffZOVErrorsCost"]);
                        WriteOffZOVErrorsMainOrders.Add(MainOrderID);
                    }

                    if (Convert.ToDecimal(DebtsDT.Rows[i]["SamplesWriteOffCost"]) > 0)
                    {
                        SamplesWriteOffCost += Convert.ToDecimal(DebtsDT.Rows[i]["SamplesWriteOffCost"]);
                        SamplesWriteOffMainOrders.Add(MainOrderID);
                    }
                }

                TotalCalcWriteOffCost = CalcDebtCost + CalcDefectsCost + CalcProductionErrorsCost + CalcZOVErrorsCost;
                TotalWriteOffCost = SamplesWriteOffCost + WriteOffDebtCost + WriteOffDefectsCost + WriteOffProductionErrorsCost + WriteOffZOVErrorsCost;
                TotalWriteOff = TotalCalcWriteOffCost + TotalWriteOffCost;

                //ProfitCost = TotalCost - TotalWriteOff;
                //DispatchedDebtCost = TotalCost - DispatchedCost;

                sCalcWriteOffResult = "Итого списано: " + TotalCalcWriteOffCost.ToString();

                AddResultRow("Стоимость заказов", 0, TotalCost);
                AddResultRow("Отгружено", 1, DispatchedCost);
                AddResultRow("Не отгружено", 2, DispatchedDebtCost);
                AddResultRow("Списано (в расчете + в минусовом)", 0, TotalWriteOff);
                AddResultRow("Образцы", 0, SamplesWriteOffCost);
                sResult = "Итого: " + ProfitCost.ToString("N", nfi1);

                AddCalcWriteOffRow("Долги", 1, CalcDebtCost);
                AddCalcWriteOffRow("Браки", 2, CalcDefectsCost);
                AddCalcWriteOffRow("Ошибки пр-ва", 3, CalcProductionErrorsCost);
                AddCalcWriteOffRow("Ошибки ЗОВа", 4, CalcZOVErrorsCost);
                sCalcWriteOffResult = "Итого списано: " + TotalCalcWriteOffCost.ToString("N", nfi1);

                AddWriteOffRow("Долги", 1, WriteOffDebtCost);
                AddWriteOffRow("Браки", 2, WriteOffDefectsCost);
                AddWriteOffRow("Ошибки пр-ва", 3, WriteOffProductionErrorsCost);
                AddWriteOffRow("Ошибки ЗОВа", 4, WriteOffZOVErrorsCost);
                AddWriteOffRow("Образцы", 0, SamplesWriteOffCost);
                sWriteOffResult = "Итого списано: " + TotalWriteOffCost.ToString("N", nfi1);
            }

            DebtsDT.Dispose();
        }

        public void GetCurrentGroup()
        {
            if (ClientsGroupsBS.Count == 0)
            {
                CurrentClientGroupID = -1;
                return;
            }
            if (((DataRowView)ClientsGroupsBS.Current).Row["ClientGroupID"] == DBNull.Value)
                return;
            else
                CurrentClientGroupID = Convert.ToInt32(((DataRowView)ClientsGroupsBS.Current).Row["ClientGroupID"]);
        }

        public void FilterClientsByGroup()
        {
            ClientsBS.Filter = "ClientGroupID = " + CurrentClientGroupID;
            //if (CurrentClientGroupID == -2)
            //    ClientsBS.RemoveFilter();
            ClientsBS.MoveFirst();
        }

        public void GetCheckedClients()
        {
            Clients.Clear();

            for (int i = 0; i < ClientsDT.Rows.Count; i++)
            {
                if (ClientsDT.Rows[i]["Checked"] == DBNull.Value
                    || !Convert.ToBoolean(ClientsDT.Rows[i]["Checked"]))
                    continue;

                Clients.Add(Convert.ToInt32(ClientsDT.Rows[i]["ClientID"]));
            }
        }

        public void GetCheckedGroups()
        {
            ClientsGroups.Clear();

            for (int i = 0; i < ClientsGroupsDT.Rows.Count; i++)
            {
                if (ClientsGroupsDT.Rows[i]["Checked"] == DBNull.Value
                    || !Convert.ToBoolean(ClientsGroupsDT.Rows[i]["Checked"]))
                    continue;

                ClientsGroups.Add(Convert.ToInt32(ClientsGroupsDT.Rows[i]["ClientGroupID"]));
            }
        }

        public void CheckAllClients(bool Check)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            foreach (DataRow row in ClientsDT.Rows)
            {
                row["Checked"] = Check;
            }

            sw.Stop();
            double G = sw.Elapsed.Milliseconds;
        }

        public void CheckAllGroups(bool Check)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            foreach (DataRow row in ClientsGroupsDT.Rows)
            {
                row["Checked"] = Check;
            }

            sw.Stop();
            double G = sw.Elapsed.Milliseconds;
        }

        public void SetCheckClients(bool Check)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            string GroupFilter = string.Empty;
            //if (CurrentClientGroupID != -2)
            GroupFilter = "ClientGroupID = " + CurrentClientGroupID;

            DataRow[] Rows = ClientsDT.Select(GroupFilter);

            foreach (DataRow row in Rows)
            {
                row["Checked"] = Check;
            }

            sw.Stop();
            double G = sw.Elapsed.Milliseconds;
        }
    }





    public class ZOVStorageStatistics
    {
        DataTable PrepareFSummaryDT = null;
        DataTable CurvedPrepareFSummaryDT = null;
        DataTable PrepareDSummaryDT = null;

        DataTable ZOVPrepareDT = null;

        DataTable PrepareReadyFrontsCostDT = null;
        DataTable CurvedPrepareReadyFrontsCostDT = null;
        DataTable PrepareReadyDecorCostDT = null;
        DataTable PrepareAllFrontsCostDT = null;
        DataTable CurvedPrepareAllFrontsCostDT = null;
        DataTable PrepareAllDecorCostDT = null;

        DataTable ZReadyFrontsCostDT = null;
        DataTable CurvedZReadyFrontsCostDT = null;
        DataTable ZReadyDecorCostDT = null;
        DataTable ZAllFrontsCostDT = null;
        DataTable CurvedZAllFrontsCostDT = null;
        DataTable ZAllDecorCostDT = null;

        DataTable FrontsOrdersDT = null;
        DataTable CurvedFrontsOrdersDT = null;
        DataTable DecorOrdersDT = null;

        DataTable FrontsSummaryDT = null;
        DataTable CurvedFrontsSummaryDT = null;
        DataTable DecorProductsSummaryDT = null;
        DataTable DecorItemsSummaryDT = null;
        DataTable DecorConfigDT = null;

        DataTable FrontsDT = null;
        DataTable DecorProductsDT = null;
        DataTable DecorItemsDT = null;

        public BindingSource PrepareFSummaryBS = null;
        public BindingSource CurvedPrepareFSummaryBS = null;
        public BindingSource PrepareDSummaryBS = null;

        public BindingSource FrontsSummaryBS = null;
        public BindingSource CurvedFrontsSummaryBS = null;
        public BindingSource DecorProductsSummaryBS = null;
        public BindingSource DecorItemsSummaryBS = null;

        PercentageDataGrid PrepareFSummaryDG = null;
        PercentageDataGrid PrepareCurvedFSummaryDG = null;
        PercentageDataGrid PrepareDSummaryDG = null;
        PercentageDataGrid FrontsDG = null;
        PercentageDataGrid CurvedFrontsDG = null;
        PercentageDataGrid DecorProductsDG = null;
        PercentageDataGrid DecorItemsDG = null;

        public ZOVStorageStatistics(
            ref PercentageDataGrid tPrepareFSummaryDG,
            ref PercentageDataGrid tCurvedPrepareFSummaryDG,
            ref PercentageDataGrid tPrepareDSummaryDG,
            ref PercentageDataGrid tFrontsDG,
            ref PercentageDataGrid tCurvedFrontsDG,
            ref PercentageDataGrid tDecorProductsDG,
            ref PercentageDataGrid tDecorItemsDG)
        {
            PrepareCurvedFSummaryDG = tCurvedPrepareFSummaryDG;
            PrepareFSummaryDG = tPrepareFSummaryDG;
            PrepareDSummaryDG = tPrepareDSummaryDG;
            CurvedFrontsDG = tCurvedFrontsDG;
            FrontsDG = tFrontsDG;
            DecorProductsDG = tDecorProductsDG;
            DecorItemsDG = tDecorItemsDG;

            Initialize();
        }

        private void Create()
        {
            ZOVPrepareDT = new DataTable();

            CurvedZReadyFrontsCostDT = new DataTable();
            ZReadyFrontsCostDT = new DataTable();
            ZReadyDecorCostDT = new DataTable();
            CurvedZAllFrontsCostDT = new DataTable();
            ZAllFrontsCostDT = new DataTable();
            ZAllDecorCostDT = new DataTable();

            CurvedPrepareReadyFrontsCostDT = new DataTable();
            PrepareReadyFrontsCostDT = new DataTable();
            PrepareReadyDecorCostDT = new DataTable();
            CurvedPrepareAllFrontsCostDT = new DataTable();
            PrepareAllFrontsCostDT = new DataTable();
            PrepareAllDecorCostDT = new DataTable();

            DecorItemsDT = new DataTable();
            FrontsDT = new DataTable();
            DecorProductsDT = new DataTable();
            DecorConfigDT = new DataTable();

            CurvedFrontsOrdersDT = new DataTable();
            FrontsOrdersDT = new DataTable();
            DecorOrdersDT = new DataTable();

            PrepareFSummaryDT = new DataTable();
            PrepareFSummaryDT.Columns.Add(new DataColumn(("DocDateTime"), System.Type.GetType("System.DateTime")));
            PrepareFSummaryDT.Columns.Add(new DataColumn(("ReadyPercTPS"), System.Type.GetType("System.Decimal")));
            PrepareFSummaryDT.Columns.Add(new DataColumn(("ReadyPercProfil"), System.Type.GetType("System.Decimal")));
            PrepareFSummaryDT.Columns.Add(new DataColumn(("CostTPS"), System.Type.GetType("System.Decimal")));
            PrepareFSummaryDT.Columns.Add(new DataColumn(("CostProfil"), System.Type.GetType("System.Decimal")));
            CurvedPrepareFSummaryDT = PrepareFSummaryDT.Clone();

            PrepareDSummaryDT = new DataTable();
            PrepareDSummaryDT.Columns.Add(new DataColumn(("DocDateTime"), System.Type.GetType("System.DateTime")));
            PrepareDSummaryDT.Columns.Add(new DataColumn(("ReadyPercTPS"), System.Type.GetType("System.Decimal")));
            PrepareDSummaryDT.Columns.Add(new DataColumn(("ReadyPercProfil"), System.Type.GetType("System.Decimal")));
            PrepareDSummaryDT.Columns.Add(new DataColumn(("CostTPS"), System.Type.GetType("System.Decimal")));
            PrepareDSummaryDT.Columns.Add(new DataColumn(("CostProfil"), System.Type.GetType("System.Decimal")));

            FrontsSummaryDT = new DataTable();
            FrontsSummaryDT.Columns.Add(new DataColumn(("Front"), System.Type.GetType("System.String")));
            FrontsSummaryDT.Columns.Add(new DataColumn(("FrontID"), System.Type.GetType("System.Int32")));
            FrontsSummaryDT.Columns.Add(new DataColumn(("Square"), System.Type.GetType("System.Decimal")));
            FrontsSummaryDT.Columns.Add(new DataColumn(("Cost"), Type.GetType("System.Decimal")));
            FrontsSummaryDT.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Int32")));
            CurvedFrontsSummaryDT = FrontsSummaryDT.Clone();

            DecorProductsSummaryDT = new DataTable();
            DecorProductsSummaryDT.Columns.Add(new DataColumn(("DecorProduct"), System.Type.GetType("System.String")));
            DecorProductsSummaryDT.Columns.Add(new DataColumn(("ProductID"), System.Type.GetType("System.Int32")));
            DecorProductsSummaryDT.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            DecorProductsSummaryDT.Columns.Add(new DataColumn(("Cost"), Type.GetType("System.Decimal")));
            DecorProductsSummaryDT.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Decimal")));
            DecorProductsSummaryDT.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));

            DecorItemsSummaryDT = new DataTable();
            DecorItemsSummaryDT.Columns.Add(new DataColumn(("DecorItem"), System.Type.GetType("System.String")));
            DecorItemsSummaryDT.Columns.Add(new DataColumn(("ProductID"), System.Type.GetType("System.Int32")));
            DecorItemsSummaryDT.Columns.Add(new DataColumn(("MeasureID"), System.Type.GetType("System.Int32")));
            DecorItemsSummaryDT.Columns.Add(new DataColumn(("DecorID"), System.Type.GetType("System.Int32")));
            DecorItemsSummaryDT.Columns.Add(new DataColumn(("Cost"), Type.GetType("System.Decimal")));
            DecorItemsSummaryDT.Columns.Add(new DataColumn(("Count"), System.Type.GetType("System.Decimal")));
            DecorItemsSummaryDT.Columns.Add(new DataColumn(("Measure"), System.Type.GetType("System.String")));
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(
            @"SELECT CONVERT(varchar(10), DocDateTime, 121) AS DocDateTime
            FROM MainOrders WHERE MainOrderID IN (SELECT MainOrderID FROM Packages WHERE PackageStatusID IN (1, 2)) AND MegaOrderID = 0
            GROUP BY CONVERT(varchar(10), DocDateTime, 121)
            ORDER BY DocDateTime",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(ZOVPrepareDT);
            }

            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDT);
            }

            SelectCommand = @"SELECT ProductID, ProductName FROM DecorProducts" +
                " WHERE ProductID IN (SELECT ProductID FROM DecorConfig WHERE Enabled = 1) ORDER BY ProductName ASC";
            DecorProductsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorProductsDT);
            }
            DecorItemsDT = new DataTable();
            SelectCommand = @"SELECT DISTINCT TechStore.TechStoreID AS DecorID, TechStore.TechStoreName AS Name, DecorConfig.ProductID FROM TechStore 
                INNER JOIN DecorConfig ON TechStore.TechStoreID = DecorConfig.DecorID AND Enabled = 1 ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorItemsDT);
            }

            //using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM DecorConfig", ConnectionStrings.CatalogConnectionString))
            //{
            //    DA.Fill(DecorConfigDT);
            //}
            DecorConfigDT = TablesManager.DecorConfigDataTable;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            FillZOVPrepareTables();

            sw.Stop();
            double G = sw.Elapsed.TotalMilliseconds;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 FrontID, PatinaID, ColorID, InsetTypeID," +
                " InsetColorID, Height, Width, Count, Square, Cost FROM FrontsOrders",
                ConnectionStrings.MarketingOrdersConnectionString))
            {
                FrontsOrdersDT.Clear();
                DA.Fill(FrontsOrdersDT);
                FrontsOrdersDT.Columns.Add(new DataColumn("MarketOrZOV", Type.GetType("System.Int32")));
                CurvedFrontsOrdersDT = FrontsOrdersDT.Clone();
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID,DecorOrders.PatinaID," +
                " DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, DecorOrders.Count, DecorOrders.Cost, DecorOrders.DecorConfigID, " +
                " MeasureID FROM DecorOrders" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig" +
                " ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID", ConnectionStrings.MarketingOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDT);
                DecorOrdersDT.Columns.Add(new DataColumn("MarketOrZOV", Type.GetType("System.Int32")));
            }
        }

        private void FillZOVPrepareTables()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT CONVERT(varchar(10), MainOrders.DocDateTime, 121) AS DocDateTime, FrontsOrders.FactoryID, SUM(FrontsOrders.Cost * PackageDetails.Count / FrontsOrders.Count) AS FrontsCost
            FROM PackageDetails INNER JOIN
            FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID AND FrontsOrders.Width=-1 INNER JOIN
            MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID INNER JOIN
            MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND MegaOrders.MegaOrderID = 0
            WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0 AND PackageStatusID IN (1, 2))
            GROUP BY CONVERT(varchar(10), MainOrders.DocDateTime, 121), FrontsOrders.FactoryID
            ORDER BY CONVERT(varchar(10), MainOrders.DocDateTime, 121), FrontsOrders.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                CurvedPrepareReadyFrontsCostDT.Clear();
                DA.Fill(CurvedPrepareReadyFrontsCostDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT CONVERT(varchar(10), MainOrders.DocDateTime, 121) AS DocDateTime, FrontsOrders.FactoryID, SUM(FrontsOrders.Cost * PackageDetails.Count / FrontsOrders.Count) AS FrontsCost
            FROM PackageDetails INNER JOIN
            FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID AND FrontsOrders.Width<>-1 INNER JOIN
            MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID INNER JOIN
            MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND MegaOrders.MegaOrderID = 0
            WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0 AND PackageStatusID IN (1, 2))
            GROUP BY CONVERT(varchar(10), MainOrders.DocDateTime, 121), FrontsOrders.FactoryID
            ORDER BY CONVERT(varchar(10), MainOrders.DocDateTime, 121), FrontsOrders.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                PrepareReadyFrontsCostDT.Clear();
                DA.Fill(PrepareReadyFrontsCostDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT CONVERT(varchar(10), MainOrders.DocDateTime, 121) AS DocDateTime, DecorOrders.FactoryID, SUM(DecorOrders.Cost * PackageDetails.Count / DecorOrders.Count) AS DecorCost
            FROM PackageDetails INNER JOIN
            DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID INNER JOIN
            MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID INNER JOIN
            MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND MegaOrders.MegaOrderID = 0
            WHERE PackageDetails.PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 1 AND PackageStatusID IN (1, 2))
            GROUP BY CONVERT(varchar(10), MainOrders.DocDateTime, 121), DecorOrders.FactoryID
            ORDER BY CONVERT(varchar(10), MainOrders.DocDateTime, 121), DecorOrders.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                PrepareReadyDecorCostDT.Clear();
                DA.Fill(PrepareReadyDecorCostDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT CONVERT(varchar(10), MainOrders.DocDateTime, 121) AS DocDateTime, FrontsOrders.FactoryID, SUM(FrontsOrders.Cost) AS FrontsCost
            FROM FrontsOrders INNER JOIN
            MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID INNER JOIN
            MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND MegaOrders.MegaOrderID = 0
            WHERE FrontsOrders.FrontsOrdersID IN
            (SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0)) AND FrontsOrders.Width=-1
            GROUP BY CONVERT(varchar(10), MainOrders.DocDateTime, 121), FrontsOrders.FactoryID
            ORDER BY CONVERT(varchar(10), MainOrders.DocDateTime, 121), FrontsOrders.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                CurvedPrepareAllFrontsCostDT.Clear();
                DA.Fill(CurvedPrepareAllFrontsCostDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT CONVERT(varchar(10), MainOrders.DocDateTime, 121) AS DocDateTime, FrontsOrders.FactoryID, SUM(FrontsOrders.Cost) AS FrontsCost
            FROM FrontsOrders INNER JOIN
            MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID INNER JOIN
            MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND MegaOrders.MegaOrderID = 0
            WHERE FrontsOrders.FrontsOrdersID IN
            (SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 0)) AND FrontsOrders.Width<>-1
            GROUP BY CONVERT(varchar(10), MainOrders.DocDateTime, 121), FrontsOrders.FactoryID
            ORDER BY CONVERT(varchar(10), MainOrders.DocDateTime, 121), FrontsOrders.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                PrepareAllFrontsCostDT.Clear();
                DA.Fill(PrepareAllFrontsCostDT);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT CONVERT(varchar(10), MainOrders.DocDateTime, 121) AS DocDateTime, DecorOrders.FactoryID, SUM(DecorOrders.Cost) AS DecorCost
            FROM DecorOrders INNER JOIN
            MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID INNER JOIN
            MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID AND MegaOrders.MegaOrderID = 0
            WHERE DecorOrders.DecorOrderID IN
            (SELECT OrderID FROM PackageDetails WHERE PackageID IN (SELECT PackageID FROM Packages WHERE ProductType = 1))
            GROUP BY CONVERT(varchar(10), MainOrders.DocDateTime, 121), DecorOrders.FactoryID
            ORDER BY CONVERT(varchar(10), MainOrders.DocDateTime, 121), DecorOrders.FactoryID",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                PrepareAllDecorCostDT.Clear();
                DA.Fill(PrepareAllDecorCostDT);
            }
        }

        private void Binding()
        {
            CurvedPrepareFSummaryBS = new BindingSource()
            {
                DataSource = CurvedPrepareFSummaryDT
            };
            PrepareCurvedFSummaryDG.DataSource = CurvedPrepareFSummaryBS;

            PrepareFSummaryBS = new BindingSource()
            {
                DataSource = PrepareFSummaryDT
            };
            PrepareFSummaryDG.DataSource = PrepareFSummaryBS;

            PrepareDSummaryBS = new BindingSource()
            {
                DataSource = PrepareDSummaryDT
            };
            PrepareDSummaryDG.DataSource = PrepareDSummaryBS;

            CurvedFrontsSummaryBS = new BindingSource()
            {
                DataSource = CurvedFrontsSummaryDT
            };
            CurvedFrontsDG.DataSource = CurvedFrontsSummaryBS;

            FrontsSummaryBS = new BindingSource()
            {
                DataSource = FrontsSummaryDT
            };
            FrontsDG.DataSource = FrontsSummaryBS;

            DecorProductsSummaryBS = new BindingSource()
            {
                DataSource = DecorProductsSummaryDT
            };
            DecorProductsDG.DataSource = DecorProductsSummaryBS;

            DecorItemsSummaryBS = new BindingSource()
            {
                DataSource = DecorItemsSummaryDT
            };
            DecorItemsDG.DataSource = DecorItemsSummaryBS;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            SetProductsGrids();
            PrepareSummaryGridSettings();
        }

        public void ShowColumns(ref PercentageDataGrid FrontsGrid, ref PercentageDataGrid CurvedFrontsGrid, ref PercentageDataGrid DecorGrid, bool Profil, bool TPS)
        {
            if (Profil && TPS)
            {
                FrontsGrid.Columns["CostProfil"].Visible = true;
                FrontsGrid.Columns["CostTPS"].Visible = true;
                FrontsGrid.Columns["ReadyPercProfil"].Visible = true;
                FrontsGrid.Columns["ReadyPercTPS"].Visible = true;

                CurvedFrontsGrid.Columns["CostProfil"].Visible = true;
                CurvedFrontsGrid.Columns["CostTPS"].Visible = true;
                CurvedFrontsGrid.Columns["ReadyPercProfil"].Visible = true;
                CurvedFrontsGrid.Columns["ReadyPercTPS"].Visible = true;

                DecorGrid.Columns["CostProfil"].Visible = true;
                DecorGrid.Columns["CostTPS"].Visible = true;
                DecorGrid.Columns["ReadyPercProfil"].Visible = true;
                DecorGrid.Columns["ReadyPercTPS"].Visible = true;
            }
            if (Profil && !TPS)
            {
                FrontsGrid.Columns["CostProfil"].Visible = true;
                FrontsGrid.Columns["CostTPS"].Visible = false;
                FrontsGrid.Columns["ReadyPercProfil"].Visible = true;
                FrontsGrid.Columns["ReadyPercTPS"].Visible = false;

                CurvedFrontsGrid.Columns["CostProfil"].Visible = true;
                CurvedFrontsGrid.Columns["CostTPS"].Visible = false;
                CurvedFrontsGrid.Columns["ReadyPercProfil"].Visible = true;
                CurvedFrontsGrid.Columns["ReadyPercTPS"].Visible = false;

                DecorGrid.Columns["CostProfil"].Visible = true;
                DecorGrid.Columns["CostTPS"].Visible = false;
                DecorGrid.Columns["ReadyPercProfil"].Visible = true;
                DecorGrid.Columns["ReadyPercTPS"].Visible = false;
            }
            if (!Profil && TPS)
            {
                FrontsGrid.Columns["CostProfil"].Visible = false;
                FrontsGrid.Columns["CostTPS"].Visible = true;
                FrontsGrid.Columns["ReadyPercProfil"].Visible = false;
                FrontsGrid.Columns["ReadyPercTPS"].Visible = true;

                CurvedFrontsGrid.Columns["CostProfil"].Visible = false;
                CurvedFrontsGrid.Columns["CostTPS"].Visible = true;
                CurvedFrontsGrid.Columns["ReadyPercProfil"].Visible = false;
                CurvedFrontsGrid.Columns["ReadyPercTPS"].Visible = true;

                DecorGrid.Columns["CostProfil"].Visible = false;
                DecorGrid.Columns["CostTPS"].Visible = true;
                DecorGrid.Columns["ReadyPercProfil"].Visible = false;
                DecorGrid.Columns["ReadyPercTPS"].Visible = true;
            }
        }

        private void PrepareSummaryGridSettings()
        {
            if (!Security.PriceAccess)
            {
                PrepareCurvedFSummaryDG.Columns["CostProfil"].Visible = false;
                PrepareCurvedFSummaryDG.Columns["CostTPS"].Visible = false;
                PrepareFSummaryDG.Columns["CostProfil"].Visible = false;
                PrepareFSummaryDG.Columns["CostTPS"].Visible = false;
                PrepareDSummaryDG.Columns["CostProfil"].Visible = false;
                PrepareDSummaryDG.Columns["CostTPS"].Visible = false;
            }

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ".",

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            foreach (DataGridViewColumn Column in PrepareFSummaryDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in PrepareCurvedFSummaryDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in PrepareDSummaryDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            PrepareFSummaryDG.Columns["DocDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy";
            PrepareFSummaryDG.Columns["DocDateTime"].HeaderText = "Дата создания";
            PrepareFSummaryDG.Columns["ReadyPercProfil"].HeaderText = "Произведено\n\r  Профиль, %";
            PrepareFSummaryDG.Columns["ReadyPercTPS"].HeaderText = "Произведено\n\r      ТПС, %";
            PrepareFSummaryDG.Columns["CostProfil"].HeaderText = "Профиль, €";
            PrepareFSummaryDG.Columns["CostTPS"].HeaderText = "ТПС, €";
            PrepareFSummaryDG.Columns["DocDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            PrepareFSummaryDG.Columns["DocDateTime"].MinimumWidth = 110;
            PrepareFSummaryDG.Columns["ReadyPercProfil"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PrepareFSummaryDG.Columns["ReadyPercProfil"].MinimumWidth = 135;
            PrepareFSummaryDG.Columns["ReadyPercTPS"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PrepareFSummaryDG.Columns["ReadyPercTPS"].MinimumWidth = 135;
            PrepareFSummaryDG.Columns["CostProfil"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PrepareFSummaryDG.Columns["CostProfil"].Width = 110;
            PrepareFSummaryDG.Columns["CostTPS"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PrepareFSummaryDG.Columns["CostTPS"].Width = 110;
            PrepareFSummaryDG.Columns["CostProfil"].DefaultCellStyle.Format = "N";
            PrepareFSummaryDG.Columns["CostProfil"].DefaultCellStyle.FormatProvider = nfi1;
            PrepareFSummaryDG.Columns["CostTPS"].DefaultCellStyle.Format = "N";
            PrepareFSummaryDG.Columns["CostTPS"].DefaultCellStyle.FormatProvider = nfi1;

            PrepareFSummaryDG.Columns["ReadyPercProfil"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            PrepareFSummaryDG.AddPercentageColumn("ReadyPercProfil");
            PrepareFSummaryDG.Columns["ReadyPercTPS"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            PrepareFSummaryDG.AddPercentageColumn("ReadyPercTPS");

            PrepareCurvedFSummaryDG.Columns["DocDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy";
            PrepareCurvedFSummaryDG.Columns["DocDateTime"].HeaderText = "Дата создания";
            PrepareCurvedFSummaryDG.Columns["ReadyPercProfil"].HeaderText = "Произведено\n\r  Профиль, %";
            PrepareCurvedFSummaryDG.Columns["ReadyPercTPS"].HeaderText = "Произведено\n\r      ТПС, %";
            PrepareCurvedFSummaryDG.Columns["CostProfil"].HeaderText = "Профиль, €";
            PrepareCurvedFSummaryDG.Columns["CostTPS"].HeaderText = "ТПС, €";
            PrepareCurvedFSummaryDG.Columns["DocDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            PrepareCurvedFSummaryDG.Columns["DocDateTime"].MinimumWidth = 110;
            PrepareCurvedFSummaryDG.Columns["ReadyPercProfil"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PrepareCurvedFSummaryDG.Columns["ReadyPercProfil"].MinimumWidth = 135;
            PrepareCurvedFSummaryDG.Columns["ReadyPercTPS"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PrepareCurvedFSummaryDG.Columns["ReadyPercTPS"].MinimumWidth = 135;
            PrepareCurvedFSummaryDG.Columns["CostProfil"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PrepareCurvedFSummaryDG.Columns["CostProfil"].Width = 110;
            PrepareCurvedFSummaryDG.Columns["CostTPS"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PrepareCurvedFSummaryDG.Columns["CostTPS"].Width = 110;
            PrepareCurvedFSummaryDG.Columns["CostProfil"].DefaultCellStyle.Format = "N";
            PrepareCurvedFSummaryDG.Columns["CostProfil"].DefaultCellStyle.FormatProvider = nfi1;
            PrepareCurvedFSummaryDG.Columns["CostTPS"].DefaultCellStyle.Format = "N";
            PrepareCurvedFSummaryDG.Columns["CostTPS"].DefaultCellStyle.FormatProvider = nfi1;

            PrepareCurvedFSummaryDG.Columns["ReadyPercProfil"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            PrepareCurvedFSummaryDG.AddPercentageColumn("ReadyPercProfil");
            PrepareCurvedFSummaryDG.Columns["ReadyPercTPS"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            PrepareCurvedFSummaryDG.AddPercentageColumn("ReadyPercTPS");

            PrepareDSummaryDG.Columns["DocDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy";
            PrepareDSummaryDG.Columns["DocDateTime"].HeaderText = "Дата создания";
            PrepareDSummaryDG.Columns["ReadyPercProfil"].HeaderText = "Произведено\n\r  Профиль, %";
            PrepareDSummaryDG.Columns["ReadyPercTPS"].HeaderText = "Произведено\n\r      ТПС, %";
            PrepareDSummaryDG.Columns["CostProfil"].HeaderText = "Профиль, €";
            PrepareDSummaryDG.Columns["CostTPS"].HeaderText = "ТПС, €";
            PrepareDSummaryDG.Columns["DocDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            PrepareDSummaryDG.Columns["DocDateTime"].MinimumWidth = 110;
            PrepareDSummaryDG.Columns["ReadyPercProfil"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PrepareDSummaryDG.Columns["ReadyPercProfil"].MinimumWidth = 135;
            PrepareDSummaryDG.Columns["ReadyPercTPS"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PrepareDSummaryDG.Columns["ReadyPercTPS"].MinimumWidth = 135;
            PrepareDSummaryDG.Columns["CostProfil"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PrepareDSummaryDG.Columns["CostProfil"].Width = 110;
            PrepareDSummaryDG.Columns["CostTPS"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PrepareDSummaryDG.Columns["CostTPS"].Width = 110;
            PrepareDSummaryDG.Columns["CostProfil"].DefaultCellStyle.Format = "N";
            PrepareDSummaryDG.Columns["CostProfil"].DefaultCellStyle.FormatProvider = nfi1;
            PrepareDSummaryDG.Columns["CostTPS"].DefaultCellStyle.Format = "N";
            PrepareDSummaryDG.Columns["CostTPS"].DefaultCellStyle.FormatProvider = nfi1;

            PrepareDSummaryDG.Columns["ReadyPercProfil"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            PrepareDSummaryDG.AddPercentageColumn("ReadyPercProfil");
            PrepareDSummaryDG.Columns["ReadyPercTPS"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            PrepareDSummaryDG.AddPercentageColumn("ReadyPercTPS");
        }

        private void SetProductsGrids()
        {
            if (!Security.PriceAccess)
            {
                CurvedFrontsDG.Columns["Cost"].Visible = false;
                FrontsDG.Columns["Cost"].Visible = false;
                DecorProductsDG.Columns["Cost"].Visible = false;
                DecorItemsDG.Columns["Cost"].Visible = false;
            }

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = string.Empty,
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ".",

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            CurvedFrontsDG.ColumnHeadersHeight = 38;
            FrontsDG.ColumnHeadersHeight = 38;
            DecorProductsDG.ColumnHeadersHeight = 38;
            DecorItemsDG.ColumnHeadersHeight = 38;

            foreach (DataGridViewColumn Column in CurvedFrontsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in FrontsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in DecorProductsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewColumn Column in DecorItemsDG.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            CurvedFrontsDG.Columns["FrontID"].Visible = false;
            CurvedFrontsDG.Columns["Square"].Visible = false;
            FrontsDG.Columns["FrontID"].Visible = false;

            CurvedFrontsDG.Columns["Front"].HeaderText = "Фасад";
            CurvedFrontsDG.Columns["Cost"].HeaderText = " € ";
            CurvedFrontsDG.Columns["Square"].HeaderText = "м.кв.";
            CurvedFrontsDG.Columns["Count"].HeaderText = "шт.";
            FrontsDG.Columns["Front"].HeaderText = "Фасад";
            FrontsDG.Columns["Cost"].HeaderText = " € ";
            FrontsDG.Columns["Square"].HeaderText = "м.кв.";
            FrontsDG.Columns["Count"].HeaderText = "шт.";

            CurvedFrontsDG.Columns["Front"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            CurvedFrontsDG.Columns["Front"].MinimumWidth = 110;
            CurvedFrontsDG.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            CurvedFrontsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            CurvedFrontsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            CurvedFrontsDG.Columns["Square"].Width = 100;
            CurvedFrontsDG.Columns["Cost"].Width = 100;
            CurvedFrontsDG.Columns["Count"].Width = 90;
            FrontsDG.Columns["Front"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FrontsDG.Columns["Front"].MinimumWidth = 110;
            FrontsDG.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsDG.Columns["Square"].Width = 100;
            FrontsDG.Columns["Cost"].Width = 100;
            FrontsDG.Columns["Count"].Width = 90;

            FrontsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            FrontsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            FrontsDG.Columns["Square"].DefaultCellStyle.Format = "N";
            FrontsDG.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;
            FrontsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            FrontsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            FrontsDG.Columns["Square"].DefaultCellStyle.Format = "N";
            FrontsDG.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            DecorProductsDG.Columns["ProductID"].Visible = false;
            DecorProductsDG.Columns["MeasureID"].Visible = false;

            DecorItemsDG.Columns["ProductID"].Visible = false;
            DecorItemsDG.Columns["DecorID"].Visible = false;
            DecorItemsDG.Columns["MeasureID"].Visible = false;

            DecorProductsDG.Columns["DecorProduct"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DecorProductsDG.Columns["DecorProduct"].MinimumWidth = 100;
            DecorProductsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorProductsDG.Columns["Cost"].Width = 100;
            DecorProductsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorProductsDG.Columns["Count"].Width = 100;
            DecorProductsDG.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorProductsDG.Columns["Measure"].Width = 90;

            DecorProductsDG.Columns["DecorProduct"].HeaderText = "Продукт";
            DecorProductsDG.Columns["Cost"].HeaderText = " € ";
            DecorProductsDG.Columns["Count"].HeaderText = "Кол-во";
            DecorProductsDG.Columns["Measure"].HeaderText = "Ед.изм.";

            DecorProductsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            DecorProductsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            DecorProductsDG.Columns["Count"].DefaultCellStyle.Format = "N";
            DecorProductsDG.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            DecorItemsDG.Columns["DecorID"].Visible = false;

            DecorItemsDG.Columns["DecorItem"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            DecorItemsDG.Columns["DecorItem"].MinimumWidth = 100;
            DecorItemsDG.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorItemsDG.Columns["Cost"].Width = 100;
            DecorItemsDG.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorItemsDG.Columns["Count"].Width = 100;
            DecorItemsDG.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorItemsDG.Columns["Measure"].Width = 90;

            DecorItemsDG.Columns["DecorItem"].HeaderText = "Наименование";
            DecorItemsDG.Columns["Cost"].HeaderText = " € ";
            DecorItemsDG.Columns["Count"].HeaderText = "Кол-во";
            DecorItemsDG.Columns["Measure"].HeaderText = "Ед.изм.";

            DecorItemsDG.Columns["Cost"].DefaultCellStyle.Format = "N";
            DecorItemsDG.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            DecorItemsDG.Columns["Count"].DefaultCellStyle.Format = "N";
            DecorItemsDG.Columns["Count"].DefaultCellStyle.FormatProvider = nfi1;

            CurvedFrontsDG.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            CurvedFrontsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            CurvedFrontsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FrontsDG.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FrontsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            FrontsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            DecorProductsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DecorProductsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DecorItemsDG.Columns["Cost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            DecorItemsDG.Columns["Count"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        public void PrepareSummary(int FactoryID)
        {
            decimal ReadyPercProfil = 0;
            decimal ReadyPercTPS = 0;

            decimal ReadyCostProfil = 0;
            decimal ReadyCostTPS = 0;
            decimal AllCostProfil = 0;
            decimal AllCostTPS = 0;

            string DocDateTime = string.Empty;

            CurvedPrepareFSummaryDT.Clear();
            PrepareFSummaryDT.Clear();
            PrepareDSummaryDT.Clear();

            if (FactoryID == -1)
                return;

            for (int i = 0; i < ZOVPrepareDT.Rows.Count; i++)
            {
                DocDateTime = ZOVPrepareDT.Rows[i]["DocDateTime"].ToString();

                if (FactoryID == 0)
                {
                    ReadyCostProfil = 0;
                    ReadyCostTPS = 0;
                    ReadyPercProfil = CurvedPrepareReadyFrontsCost(DocDateTime, 1, ref ReadyCostProfil, ref AllCostProfil);
                    ReadyPercTPS = CurvedPrepareReadyFrontsCost(DocDateTime, 2, ref ReadyCostTPS, ref AllCostTPS);

                    if (ReadyPercProfil != 0 || ReadyPercTPS != 0)
                    {
                        DataRow NewRow = CurvedPrepareFSummaryDT.NewRow();
                        NewRow["DocDateTime"] = DocDateTime;
                        if (ReadyPercProfil > 0)
                        {
                            NewRow["ReadyPercProfil"] = ReadyPercProfil;
                            NewRow["CostProfil"] = ReadyCostProfil;
                        }
                        if (ReadyPercTPS > 0)
                        {
                            NewRow["ReadyPercTPS"] = ReadyPercTPS;
                            NewRow["CostTPS"] = ReadyCostTPS;
                        }
                        CurvedPrepareFSummaryDT.Rows.Add(NewRow);
                    }

                    ReadyCostProfil = 0;
                    ReadyCostTPS = 0;
                    ReadyPercProfil = PrepareReadyFrontsCost(DocDateTime, 1, ref ReadyCostProfil, ref AllCostProfil);
                    ReadyPercTPS = PrepareReadyFrontsCost(DocDateTime, 2, ref ReadyCostTPS, ref AllCostTPS);

                    if (ReadyPercProfil != 0 || ReadyPercTPS != 0)
                    {
                        DataRow NewRow = PrepareFSummaryDT.NewRow();
                        NewRow["DocDateTime"] = DocDateTime;
                        if (ReadyPercProfil > 0)
                        {
                            NewRow["ReadyPercProfil"] = ReadyPercProfil;
                            NewRow["CostProfil"] = ReadyCostProfil;
                        }
                        if (ReadyPercTPS > 0)
                        {
                            NewRow["ReadyPercTPS"] = ReadyPercTPS;
                            NewRow["CostTPS"] = ReadyCostTPS;
                        }
                        PrepareFSummaryDT.Rows.Add(NewRow);
                    }

                    ReadyCostProfil = 0;
                    ReadyCostTPS = 0;
                    ReadyPercProfil = PrepareReadyDecorCost(DocDateTime, 1, ref ReadyCostProfil, ref AllCostProfil);
                    ReadyPercTPS = PrepareReadyDecorCost(DocDateTime, 2, ref ReadyCostTPS, ref AllCostTPS);

                    if (ReadyCostProfil != 0 || ReadyCostTPS != 0)
                    {
                        DataRow NewRow = PrepareDSummaryDT.NewRow();
                        NewRow["DocDateTime"] = DocDateTime;
                        if (ReadyCostProfil > 0)
                        {
                            NewRow["ReadyPercProfil"] = ReadyPercProfil;
                            NewRow["CostProfil"] = ReadyCostProfil;
                        }
                        if (ReadyCostTPS > 0)
                        {
                            NewRow["ReadyPercTPS"] = ReadyPercTPS;
                            NewRow["CostTPS"] = ReadyCostTPS;
                        }
                        PrepareDSummaryDT.Rows.Add(NewRow);
                    }
                }
                if (FactoryID == 1)
                {
                    ReadyCostProfil = 0;
                    ReadyPercProfil = CurvedPrepareReadyFrontsCost(DocDateTime, 1, ref ReadyCostProfil, ref AllCostProfil);

                    if (ReadyPercProfil > 0)
                    {
                        DataRow NewRow = CurvedPrepareFSummaryDT.NewRow();
                        NewRow["DocDateTime"] = DocDateTime;
                        NewRow["ReadyPercProfil"] = ReadyPercProfil;
                        NewRow["CostProfil"] = ReadyCostProfil;
                        CurvedPrepareFSummaryDT.Rows.Add(NewRow);
                    }

                    ReadyCostProfil = 0;
                    ReadyPercProfil = PrepareReadyFrontsCost(DocDateTime, 1, ref ReadyCostProfil, ref AllCostProfil);

                    if (ReadyPercProfil > 0)
                    {
                        DataRow NewRow = PrepareFSummaryDT.NewRow();
                        NewRow["DocDateTime"] = DocDateTime;
                        NewRow["ReadyPercProfil"] = ReadyPercProfil;
                        NewRow["CostProfil"] = ReadyCostProfil;
                        PrepareFSummaryDT.Rows.Add(NewRow);
                    }

                    ReadyCostProfil = 0;
                    ReadyPercProfil = PrepareReadyDecorCost(DocDateTime, 1, ref ReadyCostProfil, ref AllCostProfil);

                    if (ReadyPercProfil > 0)
                    {
                        DataRow NewRow = PrepareDSummaryDT.NewRow();
                        NewRow["DocDateTime"] = DocDateTime;
                        NewRow["ReadyPercProfil"] = ReadyPercProfil;
                        NewRow["CostProfil"] = ReadyCostProfil;
                        PrepareDSummaryDT.Rows.Add(NewRow);
                    }
                }
                if (FactoryID == 2)
                {
                    ReadyCostTPS = 0;
                    ReadyPercTPS = CurvedPrepareReadyFrontsCost(DocDateTime, 2, ref ReadyCostTPS, ref AllCostTPS);

                    if (ReadyCostTPS > 0)
                    {
                        DataRow NewRow = CurvedPrepareFSummaryDT.NewRow();
                        NewRow["DocDateTime"] = DocDateTime;
                        NewRow["ReadyPercTPS"] = ReadyPercTPS;
                        NewRow["CostTPS"] = ReadyCostTPS;
                        CurvedPrepareFSummaryDT.Rows.Add(NewRow);
                    }

                    ReadyCostTPS = 0;
                    ReadyPercTPS = PrepareReadyFrontsCost(DocDateTime, 2, ref ReadyCostTPS, ref AllCostTPS);

                    if (ReadyCostTPS > 0)
                    {
                        DataRow NewRow = PrepareFSummaryDT.NewRow();
                        NewRow["DocDateTime"] = DocDateTime;
                        NewRow["ReadyPercTPS"] = ReadyPercTPS;
                        NewRow["CostTPS"] = ReadyCostTPS;
                        PrepareFSummaryDT.Rows.Add(NewRow);
                    }

                    ReadyCostTPS = 0;
                    ReadyPercTPS = PrepareReadyDecorCost(DocDateTime, 2, ref ReadyCostTPS, ref AllCostTPS);

                    if (ReadyCostTPS > 0)
                    {
                        DataRow NewRow = PrepareDSummaryDT.NewRow();
                        NewRow["DocDateTime"] = DocDateTime;
                        NewRow["ReadyPercTPS"] = ReadyPercTPS;
                        NewRow["CostTPS"] = ReadyCostTPS;
                        PrepareDSummaryDT.Rows.Add(NewRow);
                    }
                }
            }

            CurvedPrepareFSummaryBS.MoveFirst();
            PrepareFSummaryBS.MoveFirst();
            PrepareDSummaryBS.MoveFirst();
        }

        public void PrepareOrders(int FactoryID)
        {
            string ZOVSelectCommand = string.Empty;

            string ZFrontsPackageFilter = string.Empty;
            string ZDecorPackageFilter = string.Empty;

            string PackageFactoryFilter = string.Empty;

            if (FactoryID != 0)
                PackageFactoryFilter = " AND FactoryID = " + FactoryID;

            ZFrontsPackageFilter = " PackageID IN" +
                " (SELECT PackageID FROM Packages WHERE PackageStatusID IN (1, 2) AND ProductType = 0 " + PackageFactoryFilter + ")";
            ZDecorPackageFilter = " PackageID IN" +
                " (SELECT PackageID FROM Packages WHERE PackageStatusID IN (1, 2) AND ProductType = 1 " + PackageFactoryFilter + ")";

            ZOVSelectCommand = "SELECT FrontsOrders.FrontID, FrontsOrders.PatinaID," +
                " FrontsOrders.ColorID, FrontsOrders.InsetTypeID," +
                " FrontsOrders.InsetColorID, FrontsOrders.TechnoColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, FrontsOrders.Height, FrontsOrders.Width," +
                " PackageDetails.Count, (FrontsOrders.Square * PackageDetails.Count / FrontsOrders.Count) AS Square, (FrontsOrders.Cost * PackageDetails.Count / FrontsOrders.Count) AS Cost, MeasureID FROM PackageDetails" +
                " INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID AND FrontsOrders.Width<>-1" +
                " INNER JOIN infiniu2_catalog.dbo.FrontsConfig" +
                " ON FrontsOrders.FrontConfigID=infiniu2_catalog.dbo.FrontsConfig.FrontConfigID" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = 0) AND " + ZFrontsPackageFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(ZOVSelectCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                FrontsOrdersDT.Clear();
                DA.Fill(FrontsOrdersDT);

                for (int i = 0; i < FrontsOrdersDT.Rows.Count; i++)
                {
                    FrontsOrdersDT.Rows[i]["MarketOrZOV"] = 1;
                }
            }

            GetFronts();

            ZOVSelectCommand = "SELECT FrontsOrders.FrontID, FrontsOrders.PatinaID," +
                " FrontsOrders.ColorID, FrontsOrders.InsetTypeID," +
                " FrontsOrders.InsetColorID, FrontsOrders.TechnoColorID, FrontsOrders.TechnoInsetTypeID, FrontsOrders.TechnoInsetColorID, FrontsOrders.Height, FrontsOrders.Width," +
                " PackageDetails.Count, (FrontsOrders.Square * PackageDetails.Count / FrontsOrders.Count) AS Square, (FrontsOrders.Cost * PackageDetails.Count / FrontsOrders.Count) AS Cost, MeasureID FROM PackageDetails" +
                " INNER JOIN FrontsOrders ON PackageDetails.OrderID = FrontsOrders.FrontsOrdersID AND FrontsOrders.Width=-1" +
                " INNER JOIN infiniu2_catalog.dbo.FrontsConfig" +
                " ON FrontsOrders.FrontConfigID=infiniu2_catalog.dbo.FrontsConfig.FrontConfigID" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = 0) AND " + ZFrontsPackageFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(ZOVSelectCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                CurvedFrontsOrdersDT.Clear();
                DA.Fill(CurvedFrontsOrdersDT);

                for (int i = 0; i < CurvedFrontsOrdersDT.Rows.Count; i++)
                {
                    CurvedFrontsOrdersDT.Rows[i]["MarketOrZOV"] = 1;
                }
            }

            GetCurvedFronts();

            //decor
            ZOVSelectCommand = "SELECT DecorOrders.ProductID, DecorOrders.DecorID, DecorOrders.ColorID,DecorOrders.PatinaID," +
                " DecorOrders.Height, DecorOrders.Length, DecorOrders.Width, PackageDetails.Count," +
                " (DecorOrders.Cost * PackageDetails.Count / DecorOrders.Count) AS Cost, DecorOrders.DecorConfigID, " +
                " MeasureID FROM PackageDetails" +
                " INNER JOIN DecorOrders ON PackageDetails.OrderID = DecorOrders.DecorOrderID" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig" +
                " ON DecorOrders.DecorConfigID=infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE MegaOrderID = 0) AND " + ZDecorPackageFilter;

            using (SqlDataAdapter DA = new SqlDataAdapter(ZOVSelectCommand,
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DecorOrdersDT.Clear();
                DA.Fill(DecorOrdersDT);

                for (int i = 0; i < DecorOrdersDT.Rows.Count; i++)
                {
                    DecorOrdersDT.Rows[i]["MarketOrZOV"] = 1;
                }
            }

            GetDecorProducts();
            GetDecorItems();

            PrepareSummary(FactoryID);
        }

        public bool HasCurvedFronts
        {
            get { return CurvedFrontsOrdersDT.Rows.Count > 0; }
        }

        public bool HasFronts
        {
            get { return FrontsOrdersDT.Rows.Count > 0; }
        }

        public bool HasDecor
        {
            get { return DecorOrdersDT.Rows.Count > 0; }
        }

        public void FilterDecorProducts(int ProductID, int MeasureID)
        {
            DecorItemsSummaryBS.Filter = "ProductID=" + ProductID + " AND MeasureID=" + MeasureID;
            DecorItemsSummaryBS.MoveFirst();
        }

        private void GetCurvedFronts()
        {
            decimal FrontCost = 0;
            int FrontCount = 0;

            CurvedFrontsSummaryDT.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(CurvedFrontsOrdersDT))
            {
                Table = DV.ToTable(true, new string[] { "FrontID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = CurvedFrontsOrdersDT.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND Width=-1");
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        FrontCost += Convert.ToDecimal(row["Cost"]);
                        FrontCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = CurvedFrontsSummaryDT.NewRow();
                    NewRow["Front"] = GetFrontName(Convert.ToInt32(Table.Rows[i]["FrontID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["Cost"] = Decimal.Round(FrontCost, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = FrontCount;
                    CurvedFrontsSummaryDT.Rows.Add(NewRow);

                    FrontCost = 0;
                    FrontCount = 0;
                }
            }

            Table.Dispose();
            CurvedFrontsSummaryDT.DefaultView.Sort = "Front, Count DESC";
            CurvedFrontsSummaryBS.MoveFirst();
        }

        private void GetFronts()
        {
            decimal FrontCost = 0;
            decimal FrontSquare = 0;
            int FrontCount = 0;

            FrontsSummaryDT.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(FrontsOrdersDT))
            {
                Table = DV.ToTable(true, new string[] { "FrontID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = FrontsOrdersDT.Select("FrontID=" + Convert.ToInt32(Table.Rows[i]["FrontID"]) +
                    " AND Width<>-1");
                if (Rows.Count() != 0)
                {
                    foreach (DataRow row in Rows)
                    {
                        FrontCost += Convert.ToDecimal(row["Cost"]);
                        FrontSquare += Convert.ToDecimal(row["Square"]);
                        FrontCount += Convert.ToInt32(row["Count"]);
                    }

                    DataRow NewRow = FrontsSummaryDT.NewRow();
                    NewRow["Front"] = GetFrontName(Convert.ToInt32(Table.Rows[i]["FrontID"]));
                    NewRow["FrontID"] = Convert.ToInt32(Table.Rows[i]["FrontID"]);
                    NewRow["Square"] = Decimal.Round(FrontSquare, 2, MidpointRounding.AwayFromZero);
                    NewRow["Cost"] = Decimal.Round(FrontCost, 2, MidpointRounding.AwayFromZero);
                    NewRow["Count"] = FrontCount;
                    FrontsSummaryDT.Rows.Add(NewRow);

                    FrontCost = 0;
                    FrontSquare = 0;
                    FrontCount = 0;
                }
            }

            Table.Dispose();
            FrontsSummaryDT.DefaultView.Sort = "Front, Square DESC";
            FrontsSummaryBS.MoveFirst();
        }

        private void GetDecorProducts()
        {
            decimal DecorProductCost = 0;
            decimal DecorProductCount = 0;
            int decimals = 2;
            string Measure = string.Empty;
            DecorProductsSummaryDT.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(DecorConfigDT))
            {
                Table = DV.ToTable(true, new string[] { "ProductID", "MeasureID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = DecorOrdersDT.Select("ProductID=" + Convert.ToInt32(Table.Rows[i]["ProductID"]) +
                    " AND MeasureID=" + Convert.ToInt32(Table.Rows[i]["MeasureID"]));
                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow row in Rows)
                {
                    if (Convert.ToInt32(row["ProductID"]) == 2)
                    {
                        DecorProductCost += Convert.ToDecimal(row["Cost"]);
                        if (row["Height"].ToString() == "-1")
                            DecorProductCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorProductCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        Measure = "м.п.";
                        continue;
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 1)
                    {
                        DecorProductCost += Convert.ToDecimal(row["Cost"]);
                        DecorProductCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 3)
                    {
                        DecorProductCost += Convert.ToDecimal(row["Cost"]);
                        DecorProductCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 2)
                    {
                        //нет параметра "высота"
                        if (row["Height"].ToString() == "-1")
                        {
                            DecorProductCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        }
                        else
                        {
                            DecorProductCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        }

                        DecorProductCost += Convert.ToDecimal(row["Cost"]);
                        Measure = "м.п.";
                    }
                }

                DataRow NewRow = DecorProductsSummaryDT.NewRow();
                NewRow["ProductID"] = Convert.ToInt32(Table.Rows[i]["ProductID"]);
                NewRow["MeasureID"] = Convert.ToInt32(Table.Rows[i]["MeasureID"]);
                NewRow["DecorProduct"] = GetProductName(Convert.ToInt32(Table.Rows[i]["ProductID"]));
                //if (DecorProductCount < 3)
                //    decimals = 1;
                NewRow["Cost"] = Decimal.Round(DecorProductCost, decimals, MidpointRounding.AwayFromZero);
                NewRow["Count"] = Decimal.Round(DecorProductCount, decimals, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = Measure;
                DecorProductsSummaryDT.Rows.Add(NewRow);

                Measure = string.Empty;
                DecorProductCost = 0;
                DecorProductCount = 0;
            }
            DecorProductsSummaryDT.DefaultView.Sort = "DecorProduct, Measure ASC, Count DESC";
            DecorProductsSummaryBS.MoveFirst();
        }

        private void GetDecorItems()
        {
            decimal DecorItemCost = 0;
            decimal DecorItemCount = 0;
            int decimals = 2;
            string Measure = string.Empty;
            DecorItemsSummaryDT.Clear();

            DataTable Table = new DataTable();

            using (DataView DV = new DataView(DecorOrdersDT))
            {
                Table = DV.ToTable(true, new string[] { "ProductID", "DecorID", "MeasureID" });
            }

            for (int i = 0; i < Table.Rows.Count; i++)
            {
                DataRow[] Rows = DecorOrdersDT.Select("ProductID=" + Convert.ToInt32(Table.Rows[i]["ProductID"]) +
                    " AND DecorID=" + Convert.ToInt32(Table.Rows[i]["DecorID"]) + " AND MeasureID=" + Convert.ToInt32(Table.Rows[i]["MeasureID"]));
                if (Rows.Count() == 0)
                    continue;

                foreach (DataRow row in Rows)
                {
                    if (Convert.ToInt32(row["ProductID"]) == 2)
                    {
                        DecorItemCost += Convert.ToDecimal(row["Cost"]);
                        if (row["Height"].ToString() == "-1")
                            DecorItemCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorItemCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        Measure = "м.п.";
                        continue;
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 1)
                    {
                        DecorItemCost += Convert.ToDecimal(row["Cost"]);
                        DecorItemCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 3)
                    {
                        DecorItemCost += Convert.ToDecimal(row["Cost"]);
                        DecorItemCount += Convert.ToInt32(row["Count"]);
                        Measure = "шт.";
                    }

                    if (Convert.ToInt32(row["MeasureID"]) == 2)
                    {
                        //нет параметра "высота"
                        if (row["Height"].ToString() == "-1")
                            DecorItemCount += Convert.ToDecimal(row["Length"]) * Convert.ToDecimal(row["Count"]) / 1000;
                        else
                            DecorItemCount += Convert.ToDecimal(row["Height"]) * Convert.ToDecimal(row["Count"]) / 1000;

                        DecorItemCost += Convert.ToDecimal(row["Cost"]);
                        Measure = "м.п.";
                    }
                }

                DataRow NewRow = DecorItemsSummaryDT.NewRow();
                NewRow["ProductID"] = Convert.ToInt32(Table.Rows[i]["ProductID"]);
                NewRow["DecorID"] = Convert.ToInt32(Table.Rows[i]["DecorID"]);
                NewRow["MeasureID"] = Convert.ToInt32(Table.Rows[i]["MeasureID"]);
                NewRow["DecorItem"] = GetDecorName(Convert.ToInt32(Table.Rows[i]["DecorID"]));
                if (DecorItemCount < 3)
                    decimals = 1;
                NewRow["Cost"] = Decimal.Round(DecorItemCost, decimals, MidpointRounding.AwayFromZero);
                NewRow["Count"] = Decimal.Round(DecorItemCount, decimals, MidpointRounding.AwayFromZero);
                NewRow["Measure"] = Measure;
                DecorItemsSummaryDT.Rows.Add(NewRow);

                Measure = string.Empty;
                DecorItemCost = 0;
                DecorItemCount = 0;
            }
            Table.Dispose();
            DecorItemsSummaryDT.DefaultView.Sort = "DecorItem, Count DESC";
            DecorItemsSummaryBS.MoveFirst();
        }

        public void GetCurvedFrontsInfo(ref decimal Cost, ref int CurvedCount)
        {
            for (int i = 0; i < CurvedFrontsSummaryDT.Rows.Count; i++)
            {
                CurvedCount += Convert.ToInt32(CurvedFrontsSummaryDT.Rows[i]["Count"]);
                Cost += Convert.ToDecimal(CurvedFrontsSummaryDT.Rows[i]["Cost"]);
            }
            Cost = Decimal.Round(Cost, 2, MidpointRounding.AwayFromZero);
        }

        public void GetFrontsInfo(ref decimal Square, ref decimal Cost, ref int Count)
        {
            for (int i = 0; i < FrontsSummaryDT.Rows.Count; i++)
            {
                Square += Convert.ToDecimal(FrontsSummaryDT.Rows[i]["Square"]);
                Count += Convert.ToInt32(FrontsSummaryDT.Rows[i]["Count"]);
                Cost += Convert.ToDecimal(FrontsSummaryDT.Rows[i]["Cost"]);
            }

            Cost = Decimal.Round(Cost, 2, MidpointRounding.AwayFromZero);
            Square = Decimal.Round(Square, 2, MidpointRounding.AwayFromZero);
        }

        public void GetDecorInfo(ref decimal Pogon, ref decimal Cost, ref int Count)
        {
            for (int i = 0; i < DecorProductsSummaryDT.Rows.Count; i++)
            {
                if (Convert.ToInt32(DecorProductsSummaryDT.Rows[i]["MeasureID"]) != 2)
                    Count += Convert.ToInt32(DecorProductsSummaryDT.Rows[i]["Count"]);
                else
                {
                    Pogon += Convert.ToDecimal(DecorProductsSummaryDT.Rows[i]["Count"]);
                }
                Cost += Convert.ToDecimal(DecorProductsSummaryDT.Rows[i]["Cost"]);
            }

            Cost = Decimal.Round(Cost, 2, MidpointRounding.AwayFromZero);
            Pogon = Decimal.Round(Pogon, 2, MidpointRounding.AwayFromZero);
        }

        public string GetFrontName(int FrontID)
        {
            string FrontName = string.Empty;
            DataRow[] Rows = FrontsDT.Select("FrontID = " + FrontID);
            if (Rows.Count() > 0)
                FrontName = Rows[0]["FrontName"].ToString();
            return FrontName;
        }

        /// <summary>
        /// Возвращает название продукта
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        private string GetProductName(int ProductID)
        {
            string ProductName = string.Empty;
            try
            {
                DataRow[] Rows = DecorProductsDT.Select("ProductID = " + ProductID);
                ProductName = Rows[0]["ProductName"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return ProductName;
        }

        /// <summary>
        /// Возвращает название наименования
        /// </summary>
        /// <param name="DecorID"></param>
        /// <returns></returns>
        private string GetDecorName(int DecorID)
        {
            string DecorName = string.Empty;
            try
            {
                DataRow[] Rows = DecorItemsDT.Select("DecorID = " + DecorID);
                DecorName = Rows[0]["Name"].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return DecorName;
        }

        private DateTime GetCurrentDateTime()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return Convert.ToDateTime(DT.Rows[0][0]);
                }
            }
        }

        public void ClearCurvedFrontsOrders(int TypeOrders)
        {
            if (TypeOrders == 2)
            {
                CurvedPrepareFSummaryDT.Clear();
            }
        }

        public void ClearFrontsOrders(int TypeOrders)
        {
            if (TypeOrders == 2)
            {
                PrepareFSummaryDT.Clear();
            }
        }

        public void ClearDecorOrders(int TypeOrders)
        {
            if (TypeOrders == 2)
            {
                PrepareDSummaryDT.Clear();
            }
        }

        private decimal CurvedPrepareReadyFrontsCost(string DocDateTime, int FactoryID, ref decimal ReadyCost, ref decimal AllCost)
        {
            decimal Percentage = 0;
            decimal ReadyFrontsCost = 0;
            decimal AllFrontsCost = 0;

            DataRow[] RFRows = CurvedPrepareReadyFrontsCostDT.Select("DocDateTime = '" + DocDateTime + "' AND FactoryID = " + FactoryID);
            foreach (DataRow Row in RFRows)
                ReadyFrontsCost += Convert.ToDecimal(Row["FrontsCost"]);

            DataRow[] AFRows = CurvedPrepareAllFrontsCostDT.Select("DocDateTime = '" + DocDateTime + "' AND FactoryID = " + FactoryID);
            foreach (DataRow Row in AFRows)
                AllFrontsCost += Convert.ToDecimal(Row["FrontsCost"]);

            ReadyCost = ReadyFrontsCost;
            AllCost = AllFrontsCost;

            if (AllFrontsCost > 0)
                Percentage = ReadyFrontsCost / AllFrontsCost;
            Percentage = Decimal.Round(Percentage * 100, 2, MidpointRounding.AwayFromZero);

            return Percentage;
        }

        private decimal PrepareReadyFrontsCost(string DocDateTime, int FactoryID, ref decimal ReadyCost, ref decimal AllCost)
        {
            decimal Percentage = 0;
            decimal ReadyFrontsCost = 0;
            decimal AllFrontsCost = 0;

            DataRow[] RFRows = PrepareReadyFrontsCostDT.Select("DocDateTime = '" + DocDateTime + "' AND FactoryID = " + FactoryID);
            foreach (DataRow Row in RFRows)
                ReadyFrontsCost += Convert.ToDecimal(Row["FrontsCost"]);

            DataRow[] AFRows = PrepareAllFrontsCostDT.Select("DocDateTime = '" + DocDateTime + "' AND FactoryID = " + FactoryID);
            foreach (DataRow Row in AFRows)
                AllFrontsCost += Convert.ToDecimal(Row["FrontsCost"]);

            ReadyCost = ReadyFrontsCost;
            AllCost = AllFrontsCost;

            if (AllFrontsCost > 0)
                Percentage = ReadyFrontsCost / AllFrontsCost;
            Percentage = Decimal.Round(Percentage * 100, 2, MidpointRounding.AwayFromZero);

            return Percentage;
        }

        private decimal PrepareReadyDecorCost(string DocDateTime, int FactoryID, ref decimal ReadyCost, ref decimal AllCost)
        {
            decimal Percentage = 0;
            decimal ReadyDecorCost = 0;
            decimal AllDecorCost = 0;

            DataRow[] RDRows = PrepareReadyDecorCostDT.Select("DocDateTime = '" + DocDateTime + "' AND FactoryID = " + FactoryID);
            foreach (DataRow Row in RDRows)
                ReadyDecorCost += Convert.ToDecimal(Row["DecorCost"]);

            DataRow[] ADRows = PrepareAllDecorCostDT.Select("DocDateTime = '" + DocDateTime + "' AND FactoryID = " + FactoryID);
            foreach (DataRow Row in ADRows)
                AllDecorCost += Convert.ToDecimal(Row["DecorCost"]);

            ReadyCost = ReadyDecorCost;
            AllCost = AllDecorCost;

            if (AllDecorCost > 0)
                Percentage = ReadyDecorCost / AllDecorCost;
            Percentage = Decimal.Round(Percentage * 100, 2, MidpointRounding.AwayFromZero);

            return Percentage;
        }

        private object GetCreationDate(int MegaOrderID)
        {
            string ConnectionString = ConnectionStrings.MarketingOrdersConnectionString;

            object DocDateTime = null;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT OrderDate FROM MegaOrders" +
                " WHERE MegaOrderID = " + MegaOrderID,
                ConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && DT.Rows[0]["OrderDate"] != DBNull.Value)
                        DocDateTime = Convert.ToDateTime(DT.Rows[0]["OrderDate"]);
                }
            }
            return DocDateTime;
        }

        private object StorageAdoptionDate(int MegaOrderID, int FactoryID, int ProductType)
        {
            string ConnectionString = ConnectionStrings.ZOVOrdersConnectionString;

            object StorageDateTime = null;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MIN(StorageDateTime) AS StorageDate FROM Packages" +
                " WHERE ProductType = " + ProductType + " AND FactoryID = " + FactoryID + " AND MainOrderID IN (SELECT MainOrderID FROM MainOrders" +
                " WHERE MegaOrderID = " + MegaOrderID + ")",
                ConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0 && DT.Rows[0]["StorageDate"] != DBNull.Value)
                        StorageDateTime = Convert.ToDateTime(DT.Rows[0]["StorageDate"]);
                }
            }
            return StorageDateTime;
        }
    }






    public class ClientStatisticsZOV
    {
        DataTable MainOrdersDataTable = null;
        DataTable FrontsOrdersDataTable = null;
        DataTable DecorOrdersDataTable = null;

        DataTable FrontsOrdersResultTable = null;
        DataTable FrontsCountResultTable = null;
        DataTable DecorCountResultTable = null;


        DataTable ClientsDataTable = null;
        DataTable FrontsDataTable = null;
        DataTable FrameColorsDataTable = null;

        DataTable DecorProductsDataTable = null;
        DataTable DecorConfigDataTable = null;

        public decimal TotalFrontsSquare = 0;
        public int TotalFrontsCount = 0;
        public decimal TotalDecorLength = 0;
        public int TotalDecorCount = 0;

        public decimal TotalFrontsCost = 0;
        public decimal TotalDecorCost = 0;
        public decimal TotalCost = 0;

        public decimal TotalDefectsCost = 0;

        public decimal TotalSamplesCost = 0;

        public BindingSource FrontsCountBindingSource = null;
        public BindingSource FrontsColorsBindingSource = null;
        public BindingSource DecorCountBindingSource = null;

        public BindingSource ClientsBindingSource = null;


        PercentageDataGrid ClientsDataGrid = null;
        PercentageDataGrid FrontsCountDataGrid = null;
        PercentageDataGrid FrontsColorsDataGrid = null;
        PercentageDataGrid DecorCountDataGrid = null;


        public ClientStatisticsZOV(ref PercentageDataGrid tClientsDataGrid)
        {
            ClientsDataGrid = tClientsDataGrid;

            Initialize();
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
        }


        private void Create()
        {
            MainOrdersDataTable = new DataTable();
            FrontsOrdersDataTable = new DataTable();
            DecorOrdersDataTable = new DataTable();
            DecorConfigDataTable = new DataTable();

            FrontsOrdersResultTable = new DataTable();
            FrontsOrdersResultTable.Columns.Add(new DataColumn("FrontID", Type.GetType("System.Int32")));
            FrontsOrdersResultTable.Columns.Add(new DataColumn("Front", Type.GetType("System.String")));
            FrontsOrdersResultTable.Columns.Add(new DataColumn("FrontTypeID", Type.GetType("System.Int32")));
            FrontsOrdersResultTable.Columns.Add(new DataColumn("FrontType", Type.GetType("System.String")));
            FrontsOrdersResultTable.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int32")));
            FrontsOrdersResultTable.Columns.Add(new DataColumn("Color", Type.GetType("System.String")));
            FrontsOrdersResultTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            FrontsOrdersResultTable.Columns.Add(new DataColumn("MeasureID", Type.GetType("System.Int32")));
            FrontsOrdersResultTable.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));


            FrontsCountResultTable = new DataTable();
            FrontsCountResultTable.Columns.Add(new DataColumn("Front", Type.GetType("System.String")));
            FrontsCountResultTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            FrontsCountResultTable.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));


            DecorCountResultTable = new DataTable();
            DecorCountResultTable.Columns.Add(new DataColumn("Product", Type.GetType("System.String")));
            DecorCountResultTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Decimal")));
            DecorCountResultTable.Columns.Add(new DataColumn("Measure", Type.GetType("System.String")));
        }

        private void Fill()
        {
            ClientsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients",
                ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }

            FrontsDataTable = new DataTable();

            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FrontsDataTable);
            }
            SelectCommand = @"SELECT ProductID, ProductName, MeasureID, ReportParam FROM DecorProducts" +
                " WHERE (ProductID IN (SELECT ProductID FROM DecorConfig WHERE (Enabled = 1))) ORDER BY ProductName ASC";
            DecorProductsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorProductsDataTable);
            }
            GetColorsDT();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorConfigID, MeasureID FROM DecorConfig", ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorConfigDataTable);
            }
        }

        private void Binding()
        {
            ClientsBindingSource = new BindingSource()
            {
                DataSource = ClientsDataTable,
                Sort = "ClientName ASC"
            };
            ClientsDataGrid.DataSource = ClientsBindingSource;

            ClientsDataGrid.Columns["ClientID"].Visible = false;
            ClientsDataGrid.Columns["ClientGroupID"].Visible = false;

            FrontsCountBindingSource = new BindingSource()
            {
                DataSource = FrontsCountResultTable,
                Sort = "Count DESC"
            };
            FrontsColorsBindingSource = new BindingSource()
            {
                DataSource = FrontsOrdersResultTable,
                Sort = "Count DESC"
            };
            DecorCountBindingSource = new BindingSource()
            {
                DataSource = DecorCountResultTable,
                Sort = "Count DESC"
            };
        }

        private void SetGrids()
        {
            FrontsCountDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsCountDataGrid.Columns["Count"].Width = 100;
            FrontsCountDataGrid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsCountDataGrid.Columns["Measure"].Width = 70;


            DecorCountDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorCountDataGrid.Columns["Count"].Width = 100;
            DecorCountDataGrid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            DecorCountDataGrid.Columns["Measure"].Width = 70;

            ClientsDataGrid.Columns["ClientID"].Visible = false;
            ClientsDataGrid.Columns["ClientGroupID"].Visible = false;

            FrontsColorsDataGrid.Columns["FrontID"].Visible = false;
            FrontsColorsDataGrid.Columns["Front"].Visible = false;
            FrontsColorsDataGrid.Columns["FrontTypeID"].Visible = false;
            FrontsColorsDataGrid.Columns["FrontType"].Visible = false;
            FrontsColorsDataGrid.Columns["ColorID"].Visible = false;
            FrontsColorsDataGrid.Columns["MeasureID"].Visible = false;


            FrontsColorsDataGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsColorsDataGrid.Columns["Count"].Width = 100;
            FrontsColorsDataGrid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            FrontsColorsDataGrid.Columns["Measure"].Width = 70;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
        }

        public void Load(int ClientID, DateTime DateFrom, DateTime DateTo)
        {
            MainOrdersDataTable.Clear();
            FrontsOrdersDataTable.Clear();
            DecorOrdersDataTable.Clear();
            FrontsOrdersResultTable.Clear();
            DecorCountResultTable.Clear();
            FrontsCountResultTable.Clear();

            TotalFrontsSquare = 0;
            TotalFrontsCount = 0;
            TotalDecorLength = 0;
            TotalDecorCount = 0;

            TotalFrontsCost = 0;
            TotalDecorCost = 0;
            TotalCost = 0;

            TotalDefectsCost = 0;
            TotalSamplesCost = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MainOrderID, FrontsSquare, OrderCost, FrontsCost, DecorCost, WriteOffDefectsCost, CalcDebtCost, DebtTypeID, IsSample FROM MainOrders" +
                                                         " WHERE ClientID = " + ClientID + " AND MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders" +
                                                         " WHERE DispatchDate >='" + DateFrom.ToString("yyyy-MM-dd") +
                                                         "' AND DispatchDate <='" + DateTo.ToString("yyyy-MM-dd") + "')  AND MegaOrderID != 0", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(MainOrdersDataTable);

                foreach (DataRow Row in MainOrdersDataTable.Rows)
                {
                    TotalFrontsCost += Convert.ToDecimal(Row["FrontsCost"]);
                    TotalDecorCost += Convert.ToDecimal(Row["DecorCost"]);
                    TotalCost += Convert.ToDecimal(Row["OrderCost"]);

                    //defects
                    TotalDefectsCost += Convert.ToDecimal(Row["WriteOffDefectsCost"]);

                    if (Row["DebtTypeID"].ToString() == "2")
                        TotalDefectsCost += Convert.ToDecimal(Row["CalcDebtCost"]);

                    if (Convert.ToBoolean(Row["IsSample"]) == true)
                        TotalSamplesCost += Convert.ToDecimal(Row["OrderCost"]);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FrontID, Width, ColorID, Count, Square, Cost FROM FrontsOrders " +
                                                         "WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE ClientID = " + ClientID +
                                                         " AND MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders" +
                                                         " WHERE DispatchDate >='" + DateFrom.ToString("yyyy-MM-dd") +
                                                         "' AND DispatchDate <='" + DateTo.ToString("yyyy-MM-dd") + "')  AND MegaOrderID != 0)", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);

                foreach (DataRow Row in FrontsOrdersDataTable.Rows)
                {
                    TotalFrontsCount += Convert.ToInt32(Row["Count"]);
                    TotalFrontsSquare += Convert.ToDecimal(Row["Square"]);
                    TotalFrontsSquare += Convert.ToDecimal(Row["Cost"]);

                    int Width = 0;

                    if (Convert.ToInt32(Row["Width"]) == -1)
                        Width = -1;
                    else
                        Width = 0;

                    DataRow[] fRow = FrontsOrdersResultTable.Select("FrontID = " + Row["FrontID"] + " AND Width = " + Width +
                                                                    " AND ColorID = " + Row["ColorID"]);

                    if (fRow.Count() == 0)
                    {
                        DataRow NewRow = FrontsOrdersResultTable.NewRow();
                        NewRow["FrontID"] = Row["FrontID"];


                        if (Convert.ToInt32(Row["Width"]) == -1)
                        {
                            NewRow["Front"] = FrontsDataTable.Select("FrontID = " + Row["FrontID"])[0]["FrontName"] + " гнутый";
                            NewRow["FrontTypeID"] = 1;
                            NewRow["FrontType"] = "Гнутый";
                            NewRow["Count"] = Row["Count"];
                            NewRow["MeasureID"] = 3;
                            NewRow["Measure"] = "шт.";
                        }
                        else
                        {
                            NewRow["Front"] = FrontsDataTable.Select("FrontID = " + Row["FrontID"])[0]["FrontName"];
                            NewRow["FrontTypeID"] = 0;
                            NewRow["FrontType"] = "Прямой";
                            NewRow["Count"] = Convert.ToDecimal(Row["Square"]);
                            NewRow["MeasureID"] = 1;
                            NewRow["Measure"] = "м.кв.";
                        }

                        NewRow["ColorID"] = Row["ColorID"];
                        NewRow["Color"] = FrameColorsDataTable.Select("ColorID = " + Row["ColorID"])[0]["ColorName"];

                        FrontsOrdersResultTable.Rows.Add(NewRow);
                    }
                    else
                    {
                        if (Convert.ToInt32(Row["Width"]) != -1)
                        {
                            fRow[0]["Count"] = Convert.ToDecimal(fRow[0]["Count"]) + Convert.ToDecimal(Row["Square"]);
                        }
                        else
                        {
                            fRow[0]["Count"] = Convert.ToDecimal(fRow[0]["Count"]) + Convert.ToDecimal(Row["Count"]);
                        }
                    }
                }


                foreach (DataRow Row in FrontsOrdersResultTable.Rows)
                {
                    DataRow[] fRow = FrontsCountResultTable.Select("Front = '" + Row["Front"] + "'");

                    if (fRow.Count() == 0)
                    {
                        DataRow NewRow = FrontsCountResultTable.NewRow();
                        NewRow["Front"] = Row["Front"];
                        NewRow["Count"] = Row["Count"];
                        NewRow["Measure"] = Row["Measure"];
                        FrontsCountResultTable.Rows.Add(NewRow);
                    }
                    else
                    {
                        fRow[0]["Count"] = Convert.ToDecimal(fRow[0]["Count"]) + Convert.ToDecimal(Row["Count"]);
                    }

                    Row["Count"] = Decimal.Round(Convert.ToDecimal(Row["Count"]), 1, MidpointRounding.AwayFromZero);
                }

                foreach (DataRow Row in FrontsCountResultTable.Rows)
                {
                    Row["Count"] = Decimal.Round(Convert.ToDecimal(Row["Count"]), 1, MidpointRounding.AwayFromZero);
                }

                FrontsCountBindingSource.MoveFirst();
            }


            //DECOR
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ProductID, Length, Count, DecorConfigID FROM DecorOrders " +
                                                         "WHERE MainOrderID IN (SELECT MainOrderID FROM MainOrders WHERE ClientID = " + ClientID +
                                                         " AND MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders" +
                                                         " WHERE DispatchDate >='" + DateFrom.ToString("yyyy-MM-dd") +
                                                         "' AND DispatchDate <='" + DateTo.ToString("yyyy-MM-dd") + "')  AND MegaOrderID != 0)", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);

                foreach (DataRow Row in DecorOrdersDataTable.Rows)
                {
                    string Product = DecorProductsDataTable.Select("ProductID = " + Row["ProductID"])[0]["ProductName"].ToString();


                    DataRow[] dRow = DecorCountResultTable.Select("Product = '" + Product + "'");

                    if (dRow.Count() == 0)
                    {
                        DataRow NewRow = DecorCountResultTable.NewRow();
                        NewRow["Product"] = Product;

                        if (DecorConfigDataTable.Select("DecorConfigID = " + Row["DecorConfigID"])[0]["MeasureID"].ToString() == "1" ||
                            DecorConfigDataTable.Select("DecorConfigID = " + Row["DecorConfigID"])[0]["MeasureID"].ToString() == "3")
                        {
                            NewRow["Measure"] = "шт.";
                            NewRow["Count"] = Row["Count"];
                        }
                        else
                        {
                            NewRow["Measure"] = "м.п.";
                            NewRow["Count"] = Convert.ToDecimal(Row["Length"]) / 1000 * Convert.ToDecimal(Row["Count"]);
                        }

                        DecorCountResultTable.Rows.Add(NewRow);
                    }
                    else
                    {
                        if (DecorConfigDataTable.Select("DecorConfigID = " + Row["DecorConfigID"])[0]["MeasureID"].ToString() == "1" ||
                            DecorConfigDataTable.Select("DecorConfigID = " + Row["DecorConfigID"])[0]["MeasureID"].ToString() == "3")
                        {
                            dRow[0]["Measure"] = "шт.";
                            dRow[0]["Count"] = Convert.ToInt32(dRow[0]["Count"]) + Convert.ToInt32(Row["Count"]);
                        }
                        else
                        {
                            dRow[0]["Measure"] = "м.п.";
                            dRow[0]["Count"] = Convert.ToDecimal(Row["Length"]) / 1000 * Convert.ToDecimal(Row["Count"]);
                        }
                    }
                }

                foreach (DataRow Row in DecorCountResultTable.Rows)
                {
                    if (Row["Measure"].ToString() == "шт.")
                        TotalDecorCount += Convert.ToInt32(Row["Count"]);
                    else
                        TotalDecorLength += Convert.ToDecimal(Row["Count"]);

                    Row["Count"] = Decimal.Round(Convert.ToDecimal(Row["Count"]), 1, MidpointRounding.AwayFromZero);
                }
            }

            TotalFrontsSquare = Decimal.Round(TotalFrontsSquare, 1, MidpointRounding.AwayFromZero);
            TotalDecorLength = Decimal.Round(TotalDecorLength, 1, MidpointRounding.AwayFromZero);
        }

        public string GetClientName(int ClientID)
        {
            return ClientsDataTable.Select("ClientID = " + ClientID)[0]["ClientName"].ToString();
        }

        public string CheckClientAndPeriod(int ClientID, DateTime DateFrom, DateTime DateTo)
        {
            if (DateTo < DateFrom)
                return "Выбран неверный период";

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 MainOrderID FROM MainOrders WHERE ClientID = " + ClientID +
                " AND MegaOrderID IN (SELECT MegaOrderID FROM MegaOrders WHERE" +
                " DispatchDate >='" + DateFrom.ToString("yyyy-MM-dd") + "' AND DispatchDate <='" + DateTo.ToString("yyyy-MM-dd") + "')", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return "В выбранном периоде у данного клиента нет заказов";
                }
            }

            return "";
        }

        public void BindingDetailGrids(ref PercentageDataGrid tFrontsCountDataGrid,
                                       ref PercentageDataGrid tFrontsColorsDataGrid,
                                       ref PercentageDataGrid tDecorCountDataGrid)
        {
            FrontsCountDataGrid = tFrontsCountDataGrid;
            FrontsColorsDataGrid = tFrontsColorsDataGrid;
            DecorCountDataGrid = tDecorCountDataGrid;

            FrontsCountDataGrid.DataSource = FrontsCountBindingSource;
            FrontsColorsDataGrid.DataSource = FrontsColorsBindingSource;
            DecorCountDataGrid.DataSource = DecorCountBindingSource;

            SetGrids();
        }

        public void GetAllPeriod(ref DateTime DateFrom, ref DateTime DateTo)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 DispatchDate FROM MegaOrders WHERE DispatchDate IS NOT NULL " +
                                                         "ORDER BY DispatchDate ASC", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    DateFrom = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]);
                }
            }


            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 DispatchDate FROM MegaOrders WHERE DispatchDate IS NOT NULL " +
                                                         "ORDER BY DispatchDate DESC", ConnectionStrings.ZOVOrdersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    DateTo = Convert.ToDateTime(DT.Rows[0]["DispatchDate"]);
                }
            }
        }

        public void FilterColors(string Front)
        {
            FrontsColorsBindingSource.Filter = "Front = '" + Front + "'";
        }
    }






    public class AllClientsStatistics
    {
        DataTable ClientsDataTable = null;
        DataTable MainOrdersDataTable = null;

        DataTable ResultDataTable = null;

        BindingSource ResultBindingSource;

        PercentageDataGrid ResultDataGrid;

        public AllClientsStatistics(ref PercentageDataGrid tResultDataGrid)
        {
            ResultDataGrid = tResultDataGrid;

            Initialize();
        }

        private void Create()
        {
            ClientsDataTable = new DataTable();
            MainOrdersDataTable = new DataTable();

            ResultDataTable = new DataTable();
            ResultDataTable.Columns.Add(new DataColumn("Client", Type.GetType("System.String")));
            ResultDataTable.Columns.Add(new DataColumn("FrontsSquare", Type.GetType("System.Decimal")));
            ResultDataTable.Columns.Add(new DataColumn("FrontsCost", Type.GetType("System.Decimal")));
            ResultDataTable.Columns.Add(new DataColumn("DefectsCost", Type.GetType("System.Decimal")));
            ResultDataTable.Columns.Add(new DataColumn("SamplesCost", Type.GetType("System.Decimal")));
            ResultDataTable.Columns.Add(new DataColumn("DecorCost", Type.GetType("System.Decimal")));
            ResultDataTable.Columns.Add(new DataColumn("TotalCost", Type.GetType("System.Decimal")));
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ClientsDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, FrontsSquare, DecorCost, FrontsCost, OrderCost, CalcDebtCost," +
                                                         " DebtTypeID, IsSample, WriteOffDefectsCost FROM MainOrders WHERE MegaOrderID != 0", ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(MainOrdersDataTable);

                foreach (DataRow Row in MainOrdersDataTable.Rows)
                {
                    DataRow[] CRows = ClientsDataTable.Select("ClientID = " + Row["ClientID"]);

                    if (CRows.Count() < 1)
                        continue;

                    string Client = CRows[0]["ClientName"].ToString();

                    DataRow[] mRow = ResultDataTable.Select("Client = '" + Client + "'");

                    if (mRow.Count() == 0)
                    {
                        DataRow NewRow = ResultDataTable.NewRow();
                        NewRow["Client"] = Client;
                        NewRow["FrontsSquare"] = Decimal.Round(Convert.ToDecimal(Row["FrontsSquare"]), 1, MidpointRounding.AwayFromZero);

                        if (Row["DebtTypeID"].ToString() == "2")
                            NewRow["DefectsCost"] = Row["CalcDebtCost"];

                        NewRow["DefectsCost"] = Row["WriteOffDefectsCost"];

                        if (Convert.ToBoolean(Row["IsSample"]))
                            NewRow["SamplesCost"] = Row["OrderCost"];
                        else
                            NewRow["SamplesCost"] = 0;

                        NewRow["FrontsCost"] = Row["FrontsCost"];
                        NewRow["DecorCost"] = Row["DecorCost"];
                        NewRow["TotalCost"] = Row["OrderCost"];
                        ResultDataTable.Rows.Add(NewRow);
                    }
                    else
                    {
                        mRow[0]["FrontsSquare"] = Convert.ToDecimal(mRow[0]["FrontsSquare"]) + Decimal.Round(Convert.ToDecimal(Row["FrontsSquare"]), 1, MidpointRounding.AwayFromZero);
                        mRow[0]["DefectsCost"] = Convert.ToDecimal(mRow[0]["DefectsCost"]) + Convert.ToDecimal(Row["WriteOffDefectsCost"]);
                        if (Row["DebtTypeID"].ToString() == "2")
                            mRow[0]["DefectsCost"] = Convert.ToDecimal(mRow[0]["DefectsCost"]) + Convert.ToDecimal(Row["CalcDebtCost"]);

                        if (Convert.ToBoolean(Row["IsSample"]))
                            mRow[0]["SamplesCost"] = Convert.ToDecimal(mRow[0]["SamplesCost"]) + Convert.ToDecimal(Row["OrderCost"]);

                        mRow[0]["FrontsCost"] = Convert.ToDecimal(mRow[0]["FrontsCost"]) + Convert.ToDecimal(Row["FrontsCost"]);
                        mRow[0]["DecorCost"] = Convert.ToDecimal(mRow[0]["DecorCost"]) + Convert.ToDecimal(Row["DecorCost"]);
                        mRow[0]["TotalCost"] = Convert.ToDecimal(mRow[0]["TotalCost"]) + Convert.ToDecimal(Row["OrderCost"]);
                    }
                }
            }
        }

        private void Binding()
        {
            ResultBindingSource = new BindingSource()
            {
                DataSource = ResultDataTable
            };
            ResultDataGrid.DataSource = ResultBindingSource;
            ResultBindingSource.Sort = "TotalCost DESC";
        }

        private void SetGrids()
        {
            ResultDataGrid.Columns["Client"].HeaderText = "Клиент";
            ResultDataGrid.Columns["FrontsSquare"].HeaderText = "Квадратура";
            ResultDataGrid.Columns["FrontsCost"].HeaderText = "Фасады, €";
            ResultDataGrid.Columns["DefectsCost"].HeaderText = "Возвраты, €";
            ResultDataGrid.Columns["SamplesCost"].HeaderText = "Образцы, €";
            ResultDataGrid.Columns["DecorCost"].HeaderText = "Декор, €";
            ResultDataGrid.Columns["TotalCost"].HeaderText = "Итого, €";

            ResultDataGrid.Columns["Client"].MinimumWidth = 280;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1,
                CurrencyDecimalSeparator = ","
            };
            ResultDataGrid.Columns["FrontsSquare"].DefaultCellStyle.Format = "C";
            ResultDataGrid.Columns["FrontsSquare"].DefaultCellStyle.FormatProvider = nfi1;

            ResultDataGrid.Columns["FrontsCost"].DefaultCellStyle.Format = "C";
            ResultDataGrid.Columns["FrontsCost"].DefaultCellStyle.FormatProvider = nfi1;

            ResultDataGrid.Columns["DefectsCost"].DefaultCellStyle.Format = "C";
            ResultDataGrid.Columns["DefectsCost"].DefaultCellStyle.FormatProvider = nfi1;

            ResultDataGrid.Columns["SamplesCost"].DefaultCellStyle.Format = "C";
            ResultDataGrid.Columns["SamplesCost"].DefaultCellStyle.FormatProvider = nfi1;

            ResultDataGrid.Columns["DecorCost"].DefaultCellStyle.Format = "C";
            ResultDataGrid.Columns["DecorCost"].DefaultCellStyle.FormatProvider = nfi1;

            ResultDataGrid.Columns["TotalCost"].DefaultCellStyle.Format = "C";
            ResultDataGrid.Columns["TotalCost"].DefaultCellStyle.FormatProvider = nfi1;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            SetGrids();
        }
    }




    public class IncomeMonthZOV
    {
        private int FactoryID = 0;

        public DataTable IncomeTotalDataTable = null;
        public DataTable IncomeDataTable = null;

        public DataTable FrontsOrdersDataTable = null;
        public DataTable DecorOrdersDataTable = null;

        public DataTable DecorConfigDataTable = null;

        public BindingSource IncomeBindingSource = null;

        PercentageDataGrid IncomeDataGrid;

        public IncomeMonthZOV(ref PercentageDataGrid tIncomeDataGrid)
        {
            IncomeDataGrid = tIncomeDataGrid;

            Initialize();
        }

        public int Factory
        {
            get { return FactoryID; }
            set { FactoryID = value; }

        }

        private void Create()
        {
            IncomeTotalDataTable = new DataTable();
            IncomeTotalDataTable.Columns.Add(new DataColumn("Date", Type.GetType("System.String")));
            IncomeTotalDataTable.Columns.Add(new DataColumn("Cost", Type.GetType("System.Decimal")));

            IncomeDataTable = new DataTable();
            IncomeDataTable.Columns.Add(new DataColumn("DateTime", Type.GetType("System.DateTime")));
            IncomeDataTable.Columns.Add(new DataColumn("Date", Type.GetType("System.String")));
            IncomeDataTable.Columns.Add(new DataColumn("Square", Type.GetType("System.Decimal")));
            IncomeDataTable.Columns.Add(new DataColumn("DecorLinearCount", Type.GetType("System.Decimal")));
            IncomeDataTable.Columns.Add(new DataColumn("DecorItemsCount", Type.GetType("System.Decimal")));
            IncomeDataTable.Columns.Add(new DataColumn("FrontsCost", Type.GetType("System.Decimal")));
            IncomeDataTable.Columns.Add(new DataColumn("DecorCost", Type.GetType("System.Decimal")));
            IncomeDataTable.Columns.Add(new DataColumn("TotalCost", Type.GetType("System.Decimal")));

            DecorConfigDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DecorConfigID, MeasureID FROM DecorConfig",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorConfigDataTable);
            }

            FrontsOrdersDataTable = new DataTable();
            DecorOrdersDataTable = new DataTable();
        }

        public void Fill()
        {
            FrontsOrdersDataTable.Clear();
            DecorOrdersDataTable.Clear();
            IncomeTotalDataTable.Clear();
            IncomeDataTable.Clear();

            string FactoryFilter = "";

            if (FactoryID != 0)
                FactoryFilter = " AND FrontsOrders.FactoryID = " + FactoryID;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrders.DispatchDate, FrontsOrders.Cost," +
                " FrontsOrders.Square, FrontsOrders.FactoryID FROM FrontsOrders" +
                " INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE DispatchDate IS NOT NULL" + FactoryFilter + " ORDER BY DispatchDate",

                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(FrontsOrdersDataTable);
            }

            if (FactoryID != 0)
                FactoryFilter = " AND DecorOrders.FactoryID = " + FactoryID;
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrders.DispatchDate, DecorOrders.Cost," +
                " DecorOrders.Count, DecorOrders.Length, DecorOrders.FactoryID, MeasureID FROM DecorOrders" +
                " INNER JOIN MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                " WHERE DispatchDate IS NOT NULL" + FactoryFilter + " ORDER BY DispatchDate",
                ConnectionStrings.ZOVOrdersConnectionString))
            {
                DA.Fill(DecorOrdersDataTable);
            }

            foreach (DataRow Row in FrontsOrdersDataTable.Rows)
            {
                string Date = Convert.ToDateTime(Row["DispatchDate"]).ToString("yyyy. MMMM");

                DataRow[] iRows = IncomeTotalDataTable.Select("Date = '" + Date + "'");

                if (iRows.Count() == 0)
                {
                    DataRow NewRow = IncomeTotalDataTable.NewRow();
                    NewRow["Date"] = Date;
                    NewRow["Cost"] = Convert.ToDecimal(Row["Cost"]);
                    IncomeTotalDataTable.Rows.Add(NewRow);
                }
                else
                {
                    iRows[0]["Cost"] = Convert.ToDecimal(iRows[0]["Cost"]) + Convert.ToDecimal(Row["Cost"]);
                }
            }

            foreach (DataRow Row in DecorOrdersDataTable.Rows)
            {
                string Date = Convert.ToDateTime(Row["DispatchDate"]).ToString("yyyy. MMMM");

                DataRow[] iRows = IncomeTotalDataTable.Select("Date = '" + Date + "'");

                if (iRows.Count() == 0)
                {
                    DataRow NewRow = IncomeTotalDataTable.NewRow();
                    NewRow["Date"] = Date;
                    NewRow["Cost"] = Convert.ToDecimal(Row["Cost"]);
                    IncomeTotalDataTable.Rows.Add(NewRow);
                }
                else
                {
                    iRows[0]["Cost"] = Convert.ToDecimal(iRows[0]["Cost"]) + Convert.ToDecimal(Row["Cost"]);
                }
            }

            FactoryFilter = "";



            foreach (DataRow Row in IncomeTotalDataTable.Rows)
            {
                string D = Convert.ToDateTime(Row["Date"]).ToString("yyyy-MM") + "-%";

                if (FactoryID != 0)
                    FactoryFilter = " AND FrontsOrders.FactoryID = " + FactoryID;
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrders.DispatchDate, FrontsOrders.Cost," +
                " FrontsOrders.Square, FrontsOrders.FactoryID FROM FrontsOrders" +
                " INNER JOIN MainOrders ON FrontsOrders.MainOrderID = MainOrders.MainOrderID" +
                " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                " WHERE MegaOrders.DispatchDate LIKE '" + D + "'" + FactoryFilter,
                ConnectionStrings.ZOVOrdersConnectionString))
                {
                    FrontsOrdersDataTable.Clear();
                    DA.Fill(FrontsOrdersDataTable);
                }

                if (FactoryID != 0)
                    FactoryFilter = " AND DecorOrders.FactoryID = " + FactoryID;
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MegaOrders.DispatchDate, DecorOrders.Cost," +
                    " DecorOrders.Count, DecorOrders.Length, DecorOrders.FactoryID, MeasureID FROM DecorOrders" +
                    " INNER JOIN MainOrders ON DecorOrders.MainOrderID = MainOrders.MainOrderID" +
                    " INNER JOIN MegaOrders ON MainOrders.MegaOrderID = MegaOrders.MegaOrderID" +
                    " INNER JOIN infiniu2_catalog.dbo.DecorConfig ON DecorOrders.DecorConfigID = infiniu2_catalog.dbo.DecorConfig.DecorConfigID" +
                    " WHERE MegaOrders.DispatchDate LIKE '" + D + "'" + FactoryFilter,
                    ConnectionStrings.ZOVOrdersConnectionString))
                {
                    DecorOrdersDataTable.Clear();
                    DA.Fill(DecorOrdersDataTable);
                }

                decimal Square = 0;
                decimal DecorLinearCount = 0;
                decimal DecorItemsCount = 0;
                decimal FrontsCost = 0;
                decimal DecorCost = 0;

                foreach (DataRow mRow in FrontsOrdersDataTable.Rows)
                {
                    Square += Convert.ToDecimal(mRow["Square"]);
                    FrontsCost += Convert.ToDecimal(mRow["Cost"]);
                }

                foreach (DataRow dRow in DecorOrdersDataTable.Rows)
                {
                    DecorCost += Convert.ToDecimal(dRow["Cost"]);
                    if (dRow["MeasureID"].ToString() == "2")
                        DecorLinearCount += Convert.ToDecimal(dRow["Count"]) * Convert.ToDecimal(dRow["Length"]) / 1000;
                    else
                        DecorItemsCount += Convert.ToDecimal(dRow["Count"]);
                }


                DataRow NewRow = IncomeDataTable.NewRow();
                NewRow["DateTime"] = Row["Date"];
                NewRow["Date"] = Convert.ToDateTime(Row["Date"]).ToString("yyyy. MMMM");
                NewRow["Square"] = Decimal.Round(Square, 1, MidpointRounding.AwayFromZero);
                NewRow["FrontsCost"] = Decimal.Round(FrontsCost, 2, MidpointRounding.AwayFromZero);
                NewRow["DecorCost"] = Decimal.Round(DecorCost, 2, MidpointRounding.AwayFromZero);
                NewRow["TotalCost"] = Decimal.Round(FrontsCost + DecorCost, 2, MidpointRounding.AwayFromZero);
                NewRow["DecorLinearCount"] = Decimal.Round(DecorLinearCount, 2, MidpointRounding.AwayFromZero);
                NewRow["DecorItemsCount"] = Decimal.Round(DecorItemsCount, 2, MidpointRounding.AwayFromZero);
                IncomeDataTable.Rows.Add(NewRow);
            }
        }

        private void Binding()
        {
            IncomeBindingSource = new BindingSource()
            {
                DataSource = IncomeDataTable
            };
            IncomeDataGrid.DataSource = IncomeBindingSource;
        }

        private void SetGrids()
        {
            foreach (DataGridViewColumn Column in IncomeDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            IncomeDataGrid.Columns["DateTime"].Visible = false;

            IncomeDataGrid.Columns["Date"].HeaderText = "Месяц";
            IncomeDataGrid.Columns["Square"].HeaderText = "Квадратура";
            IncomeDataGrid.Columns["FrontsCost"].HeaderText = "Фасады, €";
            IncomeDataGrid.Columns["DecorCost"].HeaderText = "Декор, €";
            IncomeDataGrid.Columns["TotalCost"].HeaderText = "Итого, €";
            IncomeDataGrid.Columns["DecorLinearCount"].HeaderText = "Декор, м.п.";
            IncomeDataGrid.Columns["DecorItemsCount"].HeaderText = "Декор, шт.";

            //ResultDataGrid.Columns["Client"].HeaderText = "Клиент";

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            NumberFormatInfo nfi2 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 1
            };
            IncomeDataGrid.Columns["Square"].DefaultCellStyle.Format = "C";
            IncomeDataGrid.Columns["Square"].DefaultCellStyle.FormatProvider = nfi1;

            IncomeDataGrid.Columns["FrontsCost"].DefaultCellStyle.Format = "C";
            IncomeDataGrid.Columns["FrontsCost"].DefaultCellStyle.FormatProvider = nfi2;

            IncomeDataGrid.Columns["DecorCost"].DefaultCellStyle.Format = "C";
            IncomeDataGrid.Columns["DecorCost"].DefaultCellStyle.FormatProvider = nfi2;

            IncomeDataGrid.Columns["TotalCost"].DefaultCellStyle.Format = "C";
            IncomeDataGrid.Columns["TotalCost"].DefaultCellStyle.FormatProvider = nfi2;

            IncomeDataGrid.Columns["DecorLinearCount"].DefaultCellStyle.Format = "C";
            IncomeDataGrid.Columns["DecorLinearCount"].DefaultCellStyle.FormatProvider = nfi1;

            IncomeDataGrid.Columns["DecorItemsCount"].DefaultCellStyle.Format = "C";
            IncomeDataGrid.Columns["DecorItemsCount"].DefaultCellStyle.FormatProvider = nfi2;

            //IncomeDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //IncomeDataGrid.Columns["FrontsCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //IncomeDataGrid.Columns["DecorCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //IncomeDataGrid.Columns["TotalCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //IncomeDataGrid.Columns["DecorLinearCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //IncomeDataGrid.Columns["DecorItemsCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //IncomeDataGrid.Columns["Square"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            SetGrids();
        }
    }
}
