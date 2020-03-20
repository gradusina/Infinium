using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class UserFunctionsContainer : UserControl
    {
        int iFactoryID = -1;
        int iDepartmentID = -1;
        int iPositionID = -1;

        UsersResponsibilities tUsersResponsibilities;
        Form TopForm;
        public UserFunctionsContainer()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (TopForm != null)
                    TopForm.Activate();
            }
        }

        public string Position
        {
            get { return lbPosition.Text; }
            set { lbPosition.Text = value; }
        }

        public int FactoryID
        {
            get { return iFactoryID; }
            set { iFactoryID = value; }
        }

        public int DepartmentID
        {
            get { return iDepartmentID; }
            set { iDepartmentID = value; }
        }

        public int PositionID
        {
            get { return iPositionID; }
            set { iPositionID = value; }
        }

        public object UsersFunctionsDataTable
        {
            set { dgvUsersFunctions.DataSource = value; }
        }

        public UsersResponsibilities AdminResponsibilities
        {
            set { tUsersResponsibilities = value; }
        }

        public void UsersFunctionsSetting()
        {
            foreach (DataGridViewColumn Column in dgvUsersFunctions.Columns)
            {
                Column.ReadOnly = true;
                Column.Visible = false;
            }
            if (dgvUsersFunctions.Columns.Contains("FunctionExecTypeID"))
                dgvUsersFunctions.Columns["FunctionExecTypeID"].Visible = false;
            if (dgvUsersFunctions.Columns.Contains("PositionID"))
                dgvUsersFunctions.Columns["PositionID"].Visible = false;
            if (dgvUsersFunctions.Columns.Contains("UserFunctionID"))
                dgvUsersFunctions.Columns["UserFunctionID"].Visible = false;
            if (dgvUsersFunctions.Columns.Contains("PositionFunctionID"))
                dgvUsersFunctions.Columns["PositionFunctionID"].Visible = false;
            if (dgvUsersFunctions.Columns.Contains("UserPositionID"))
                dgvUsersFunctions.Columns["UserPositionID"].Visible = false;
            if (dgvUsersFunctions.Columns.Contains("Name"))
                dgvUsersFunctions.Columns["Name"].Visible = false;
            if (dgvUsersFunctions.Columns.Contains("FactoryName"))
                dgvUsersFunctions.Columns["FactoryName"].Visible = false;
            if (dgvUsersFunctions.Columns.Contains("FactoryID"))
                dgvUsersFunctions.Columns["FactoryID"].Visible = false;
            if (dgvUsersFunctions.Columns.Contains("FactoryName"))
                dgvUsersFunctions.Columns["FactoryName"].Visible = false;
            if (dgvUsersFunctions.Columns.Contains("Name"))
                dgvUsersFunctions.Columns["Name"].Visible = false;
            if (dgvUsersFunctions.Columns.Contains("Position"))
                dgvUsersFunctions.Columns["Position"].Visible = false;
            if (dgvUsersFunctions.Columns.Contains("Rate"))
                dgvUsersFunctions.Columns["Rate"].Visible = false;
            if (dgvUsersFunctions.Columns.Contains("FunctionDescription"))
                dgvUsersFunctions.Columns["FunctionDescription"].Visible = false;
            if (dgvUsersFunctions.Columns.Contains("UserID"))
                dgvUsersFunctions.Columns["UserID"].Visible = false;
            //dgvUsersFunctions.Columns["Name"].HeaderText = "Сотрудник";
            //dgvUsersFunctions.Columns["Position"].HeaderText = "Должность";
            //dgvUsersFunctions.Columns["FactoryName"].HeaderText = "Участок";
            //dgvUsersFunctions.Columns["Rate"].HeaderText = "Ставка";
            dgvUsersFunctions.Columns["FunctionName"].Visible = true;
            dgvUsersFunctions.Columns["FunctionName"].HeaderText = "Обязанность";

            //dgvUsersFunctions.Columns["FactoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //dgvUsersFunctions.Columns["FactoryName"].Width = 100;
            //dgvUsersFunctions.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dgvUsersFunctions.Columns["Name"].MinimumWidth = 100;
            //dgvUsersFunctions.Columns["Rate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //dgvUsersFunctions.Columns["Rate"].Width = 100;
            dgvUsersFunctions.Columns["FunctionName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvUsersFunctions.Columns["FunctionName"].MinimumWidth = 100;
            //dgvUsersFunctions.Columns["Position"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dgvUsersFunctions.Columns["Position"].MinimumWidth = 100;

            int DisplayIndex = 0;
            dgvUsersFunctions.Columns["FunctionName"].DisplayIndex = DisplayIndex++;
        }

        private void dgvUsersFunctions_SelectionChanged(object sender, EventArgs e)
        {
            rtbFunctionDescription.Clear();
            if (dgvUsersFunctions.SelectedRows.Count == 0)
                return;
            string FunctionDescription = dgvUsersFunctions.SelectedRows[0].Cells["FunctionDescription"].Value.ToString();
            rtbFunctionDescription.Text = FunctionDescription;
        }

        private void btnDeleteUserFunction_Click(object sender, EventArgs e)
        {
            if (dgvUsersFunctions.SelectedRows.Count != 0)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Открепить обязанность от сотрудника? Запись будет удалена безвозвратно.",
                        "Открепление обязанности");

                if (OKCancel)
                {
                    int UserFunctionID = Convert.ToInt32(dgvUsersFunctions.SelectedRows[0].Cells["UserFunctionID"].Value);
                    tUsersResponsibilities.DeleteUsersResponsibility(UserFunctionID);
                    //InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
                    if (iFactoryID == 1)
                    {
                        tUsersResponsibilities.UpdateProfilFunctions();
                        dgvUsersFunctions.DataSource = tUsersResponsibilities.GetProfilPositionFunctions(iPositionID);
                    }
                    if (iFactoryID == 2)
                    {
                        tUsersResponsibilities.UpdateTPSFunctions();
                        dgvUsersFunctions.DataSource = tUsersResponsibilities.GetTPSPositionFunctions(iPositionID);
                    }
                }
            }
        }

        private void btnAddUserFunction_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int StaffListID = UsersResponsibilities.GetStaffListID(iFactoryID, iDepartmentID, Security.CurrentUserID, iPositionID);
            FunctionsManagementForm FunctionsManagementForm = new FunctionsManagementForm(StaffListID, Security.CurrentUserID);

            TopForm = FunctionsManagementForm;

            FunctionsManagementForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            FunctionsManagementForm.Close();
            FunctionsManagementForm.Dispose();

            if (iFactoryID == 1)
            {
                tUsersResponsibilities.UpdateProfilFunctions();
                dgvUsersFunctions.DataSource = tUsersResponsibilities.GetProfilPositionFunctions(iPositionID);
            }
            if (iFactoryID == 2)
            {
                tUsersResponsibilities.UpdateTPSFunctions();
                dgvUsersFunctions.DataSource = tUsersResponsibilities.GetTPSPositionFunctions(iPositionID);
            }
        }
    }
}
