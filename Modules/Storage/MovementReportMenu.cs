using Infinium.Store;

using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MovementReportMenu : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int StoreType = 0;
        int FormEvent = 0;
        int FactoryID = 1;
        int TechStoreGroupID = -1;

        DataTable MonthsDT;
        Form MainForm = null;
        ReportParameters ReportMenu;

        DataTable GroupsDT;
        DataTable SubGroupsDT;
        BindingSource GroupsBS;
        BindingSource SubGroupsBS;

        public MovementReportMenu(Form tMainForm, int iFactoryID, int iStoreType, ref ReportParameters tReportMenu)
        {
            MainForm = tMainForm;
            StoreType = iStoreType;
            FactoryID = iFactoryID;
            ReportMenu = tReportMenu;
            InitializeComponent();
        }

        private void OKReportButton_Click(object sender, EventArgs e)
        {
            if (NaturalRadioButton.Checked)
                ReportMenu.ReportType = 1;
            if (FinancialRadioButton.Checked)
                ReportMenu.ReportType = 2;
            if (StoreParametersRadioButton.Checked)
                ReportMenu.ReportType = 3;

            if (CurrentMonthRadioButton.Checked)
            {
                ReportMenu.DatePeriodType = 1;
                ReportMenu.FirstDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), Convert.ToInt32(cbxMonths.SelectedValue), 1);
                ReportMenu.SecondDate = ReportMenu.FirstDate.AddMonths(1);
            }
            if (PeriodRadioButton.Checked)
            {
                ReportMenu.DatePeriodType = 2;
                if (Convert.ToInt32(cbxQuarters.SelectedValue) == 1)
                {
                    ReportMenu.QuarterNumber = 1;
                    ReportMenu.FirstDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), 1, 1);
                    ReportMenu.SecondDate = ReportMenu.FirstDate.AddMonths(3);
                }
                if (Convert.ToInt32(cbxQuarters.SelectedValue) == 2)
                {
                    ReportMenu.QuarterNumber = 2;
                    ReportMenu.FirstDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), 4, 1);
                    ReportMenu.SecondDate = ReportMenu.FirstDate.AddMonths(3);
                }
                if (Convert.ToInt32(cbxQuarters.SelectedValue) == 3)
                {
                    ReportMenu.QuarterNumber = 3;
                    ReportMenu.FirstDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), 7, 1);
                    ReportMenu.SecondDate = ReportMenu.FirstDate.AddMonths(3);
                }
                if (Convert.ToInt32(cbxQuarters.SelectedValue) == 4)
                {
                    ReportMenu.QuarterNumber = 4;
                    ReportMenu.FirstDate = new DateTime(Convert.ToInt32(cbxYears.SelectedValue), 10, 1);
                    ReportMenu.SecondDate = ReportMenu.FirstDate.AddMonths(3);
                }
            }

            for (int i = 0; i < SubGroupsDT.Rows.Count; i++)
            {
                if (Convert.ToBoolean(SubGroupsDT.Rows[i]["Checked"]))
                    ReportMenu.SubGroups.Add(Convert.ToInt32(SubGroupsDT.Rows[i]["TechStoreSubGroupID"]));
            }

            ReportMenu.OKPress = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelReportButton_Click(object sender, EventArgs e)
        {
            ReportMenu.OKPress = false;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void AnimateTimer_Tick(object sender, EventArgs e)
        {
            if (!DatabaseConfigsManager.Animation)
            {
                this.Opacity = 1;

                if (FormEvent == eClose || FormEvent == eHide)
                {
                    AnimateTimer.Enabled = false;

                    if (FormEvent == eClose)
                    {
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        MainForm.Activate();
                        this.Hide();
                    }

                    return;
                }

                if (FormEvent == eShow)
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                    return;
                }

            }

            if (FormEvent == eClose || FormEvent == eHide)
            {
                if (Convert.ToDecimal(this.Opacity) != Convert.ToDecimal(0.00))
                    this.Opacity = Convert.ToDouble(Convert.ToDecimal(this.Opacity) - Convert.ToDecimal(0.05));
                else
                {
                    AnimateTimer.Enabled = false;

                    if (FormEvent == eClose)
                    {
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        MainForm.Activate();
                        this.Hide();
                    }
                }

                return;
            }


            if (FormEvent == eShow || FormEvent == eShow)
            {
                if (this.Opacity != 1)
                    this.Opacity += 0.05;
                else
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                }

                return;
            }
        }

        private void Initialize()
        {
            ReportMenu.SubGroups = new ArrayList();

            MonthsDT = new DataTable();
            MonthsDT.Columns.Add(new DataColumn("MonthID", Type.GetType("System.Int32")));
            MonthsDT.Columns.Add(new DataColumn("MonthName", Type.GetType("System.String")));

            for (int i = 1; i <= 12; i++)
            {
                DataRow NewRow = MonthsDT.NewRow();
                NewRow["MonthID"] = i;
                NewRow["MonthName"] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).ToString();
                MonthsDT.Rows.Add(NewRow);
            }
            cbxMonths.DataSource = MonthsDT.DefaultView;
            cbxMonths.ValueMember = "MonthID";
            cbxMonths.DisplayMember = "MonthName";

            DateTime LastDay = new System.DateTime(DateTime.Now.Year, 12, 31);
            System.Collections.ArrayList Quarters = new System.Collections.ArrayList();
            System.Collections.ArrayList Years = new System.Collections.ArrayList();
            for (int i = 1; i <= 4; i++)
            {
                Quarters.Add(i);
            }
            for (int i = 2013; i <= LastDay.Year; i++)
            {
                Years.Add(i);
            }
            cbxQuarters.DataSource = Quarters.ToArray();
            cbxQuarters.SelectedIndex = 0;
            cbxYears.DataSource = Years.ToArray();
            cbxYears.SelectedIndex = cbxYears.Items.Count - 1;

            GroupsDT = new DataTable();
            SubGroupsDT = new DataTable();
            GroupsBS = new BindingSource();
            SubGroupsBS = new BindingSource();

            if (StoreType == 1)
            {
                using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter("SELECT * FROM TechStoreGroups" +
                    " WHERE TechStoreGroupID IN (SELECT TechStoreGroupID FROM TechStoreSubGroups WHERE TechStoreSubGroupID IN" +
                    " (SELECT TechStoreSubGroupID FROM TechStore WHERE TechStoreID IN (SELECT DISTINCT StoreItemID FROM infiniu2_storage.dbo.Store" +
                    " WHERE FactoryID = " + FactoryID + ")))" +
                    " ORDER BY TechStoreGroupName", ConnectionStrings.CatalogConnectionString))
                {
                    DA.Fill(GroupsDT);
                    GroupsDT.Columns.Add(new DataColumn("Checked", Type.GetType("System.Boolean")));
                    foreach (DataRow Row in GroupsDT.Rows)
                    {
                        Row["Checked"] = false;
                    }
                }
                using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter("SELECT * FROM TechStoreSubGroups WHERE TechStoreSubGroupID IN" +
                    " (SELECT TechStoreSubGroupID FROM TechStore WHERE TechStoreID IN (SELECT DISTINCT StoreItemID FROM infiniu2_storage.dbo.Store" +
                    " WHERE FactoryID = " + FactoryID + "))" +
                    " ORDER BY TechStoreSubGroupName", ConnectionStrings.CatalogConnectionString))
                {
                    DA.Fill(SubGroupsDT);
                    SubGroupsDT.Columns.Add(new DataColumn("Checked", Type.GetType("System.Boolean")));
                    foreach (DataRow Row in SubGroupsDT.Rows)
                    {
                        Row["Checked"] = false;
                    }
                }
            }
            if (StoreType == 2)
            {
                using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter("SELECT * FROM TechStoreGroups" +
                    " WHERE TechStoreGroupID IN (SELECT TechStoreGroupID FROM TechStoreSubGroups WHERE TechStoreSubGroupID IN" +
                    " (SELECT TechStoreSubGroupID FROM TechStore WHERE TechStoreID IN (SELECT DISTINCT StoreItemID FROM infiniu2_storage.dbo.ManufactureStore" +
                    " WHERE FactoryID = " + FactoryID + ")))" +
                    " ORDER BY TechStoreGroupName", ConnectionStrings.CatalogConnectionString))
                {
                    DA.Fill(GroupsDT);
                    GroupsDT.Columns.Add(new DataColumn("Checked", Type.GetType("System.Boolean")));
                    foreach (DataRow Row in GroupsDT.Rows)
                    {
                        Row["Checked"] = false;
                    }
                }
                using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter("SELECT * FROM TechStoreSubGroups WHERE TechStoreSubGroupID IN" +
                    " (SELECT TechStoreSubGroupID FROM TechStore WHERE TechStoreID IN (SELECT DISTINCT StoreItemID FROM infiniu2_storage.dbo.ManufactureStore" +
                    " WHERE FactoryID = " + FactoryID + "))" +
                    " ORDER BY TechStoreSubGroupName", ConnectionStrings.CatalogConnectionString))
                {
                    DA.Fill(SubGroupsDT);
                    SubGroupsDT.Columns.Add(new DataColumn("Checked", Type.GetType("System.Boolean")));
                    foreach (DataRow Row in SubGroupsDT.Rows)
                    {
                        Row["Checked"] = false;
                    }
                }
            }
            GroupsBS.DataSource = GroupsDT;
            SubGroupsBS.DataSource = SubGroupsDT;

            GroupsGridSettings();
            SubGroupsGridSettings();
        }

        private void GroupsGridSettings()
        {
            GroupsDataGrid.DataSource = GroupsBS;
            foreach (DataGridViewColumn Column in GroupsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            if (GroupsDataGrid.Columns.Contains("TechStoreGroupID"))
                GroupsDataGrid.Columns["TechStoreGroupID"].Visible = false;

            GroupsDataGrid.Columns["Checked"].ReadOnly = false;
            GroupsDataGrid.Columns["Checked"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            GroupsDataGrid.Columns["Checked"].HeaderText = "Выбрать";
            GroupsDataGrid.Columns["TechStoreGroupName"].MinimumWidth = 150;
            GroupsDataGrid.Columns["TechStoreGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            GroupsDataGrid.Columns["TechStoreGroupName"].HeaderText = "Группа";
            GroupsDataGrid.AutoGenerateColumns = false;
            GroupsDataGrid.Columns["Checked"].DisplayIndex = 1;
            GroupsDataGrid.Columns["TechStoreGroupName"].DisplayIndex = 2;
        }

        private void SubGroupsGridSettings()
        {
            SubGroupsDataGrid.DataSource = SubGroupsBS;
            foreach (DataGridViewColumn Column in SubGroupsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Column.ReadOnly = true;
            }
            if (SubGroupsDataGrid.Columns.Contains("TechStoreGroupID"))
                SubGroupsDataGrid.Columns["TechStoreGroupID"].Visible = false;
            if (SubGroupsDataGrid.Columns.Contains("TechStoreSubGroupID"))
                SubGroupsDataGrid.Columns["TechStoreSubGroupID"].Visible = false;

            SubGroupsDataGrid.Columns["Checked"].ReadOnly = false;
            SubGroupsDataGrid.Columns["Checked"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SubGroupsDataGrid.Columns["Checked"].HeaderText = "Выбрать";
            SubGroupsDataGrid.Columns["TechStoreSubGroupName"].MinimumWidth = 150;
            SubGroupsDataGrid.Columns["TechStoreSubGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            SubGroupsDataGrid.Columns["TechStoreSubGroupName"].HeaderText = "Подгруппа";
            SubGroupsDataGrid.AutoGenerateColumns = false;
            SubGroupsDataGrid.Columns["Checked"].DisplayIndex = 1;
            SubGroupsDataGrid.Columns["TechStoreSubGroupName"].DisplayIndex = 2;
        }

        private void CurrentMonthRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentMonthRadioButton.Checked)
            {
                cbxMonths.Enabled = true;
                cbxQuarters.Enabled = false;
            }
        }

        private void PeriodRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (PeriodRadioButton.Checked)
            {
                cbxMonths.Enabled = false;
                cbxQuarters.Enabled = true;
            }
        }

        private void MovementReportMenu_Load(object sender, EventArgs e)
        {
            Initialize();
        }

        private void GroupsDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (GroupsDataGrid.Columns[e.ColumnIndex].Name == "Checked" && e.RowIndex != -1)
            {
                DataGridViewCheckBoxCell checkCell =
                    (DataGridViewCheckBoxCell)GroupsDataGrid.
                    Rows[e.RowIndex].Cells["Checked"];

                bool Checked = Convert.ToBoolean(checkCell.Value);

                string GroupFilter = string.Empty;

                GroupFilter = "TechStoreGroupID = " + TechStoreGroupID;
                DataRow[] Rows = SubGroupsDT.Select(GroupFilter);
                foreach (DataRow row in Rows)
                {
                    row["Checked"] = Checked;
                }

                GroupsDataGrid.Invalidate();
            }
        }

        private void GroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (((DataRowView)GroupsBS.Current).Row["TechStoreGroupID"] == DBNull.Value)
                return;
            else
                TechStoreGroupID = Convert.ToInt32(((DataRowView)GroupsBS.Current).Row["TechStoreGroupID"]);

            SubGroupsBS.Filter = "TechStoreGroupID = " + TechStoreGroupID;
            SubGroupsBS.MoveFirst();
        }
    }
}
