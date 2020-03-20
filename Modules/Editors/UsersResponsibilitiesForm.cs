using System;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class UsersResponsibilitiesForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;
        LightStartForm LightStartForm;

        AdminFunctionsEdit AdminFunctionsEdit;
        UsersResponsibilities UsersResponsibilities;

        public UsersResponsibilitiesForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            AdminFunctionsEdit = new AdminFunctionsEdit();
            UsersResponsibilities = new UsersResponsibilities();

            while (!SplashForm.bCreated) ;
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

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.HideForm(this);
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

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.HideForm(this);
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

        private void UsersResponsibilitiesForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void UsersResponsibilitiesForm_Load(object sender, EventArgs e)
        {
            dgvUsersResponsibilitiesSettings();

            cmbFactory.DataSource = UsersResponsibilities.FactoryBS;
            cmbFactory.DisplayMember = "FactoryName";
            cmbFactory.ValueMember = "FactoryID";

            cmbDepartments.DataSource = UsersResponsibilities.DepartmentsBS;
            cmbDepartments.DisplayMember = "DepartmentName";
            cmbDepartments.ValueMember = "DepartmentID";

            cmbUsers.DataSource = UsersResponsibilities.UsersBS;
            cmbUsers.DisplayMember = "Name";
            cmbUsers.ValueMember = "UserID";

            cmbPositions.DataSource = UsersResponsibilities.PositionsBS;
            cmbPositions.DisplayMember = "Position";
            cmbPositions.ValueMember = "PositionID";

            FilterDepartments();
            FilterUsers();
            FilterPositions();
            FilterUsersResponsibilities();
        }

        private void dgvUsersResponsibilitiesSettings()
        {
            dgvUsersResponsibilities.DataSource = UsersResponsibilities.UsersResponsibilitiesBS;

            if (dgvUsersResponsibilities.Columns.Contains("UserFunctionID"))
                dgvUsersResponsibilities.Columns["UserFunctionID"].Visible = false;
            if (dgvUsersResponsibilities.Columns.Contains("StaffListID"))
                dgvUsersResponsibilities.Columns["StaffListID"].Visible = false;
            if (dgvUsersResponsibilities.Columns.Contains("UserID"))
                dgvUsersResponsibilities.Columns["UserID"].Visible = false;
            if (dgvUsersResponsibilities.Columns.Contains("FunctionID"))
                dgvUsersResponsibilities.Columns["FunctionID"].Visible = false;
            if (dgvUsersResponsibilities.Columns.Contains("IsLearning"))
                dgvUsersResponsibilities.Columns["IsLearning"].Visible = false;
            if (dgvUsersResponsibilities.Columns.Contains("IsAble"))
                dgvUsersResponsibilities.Columns["IsAble"].Visible = false;
            if (dgvUsersResponsibilities.Columns.Contains("IsPerform"))
                dgvUsersResponsibilities.Columns["IsPerform"].Visible = false;
            if (dgvUsersResponsibilities.Columns.Contains("LearningPercentage"))
                dgvUsersResponsibilities.Columns["LearningPercentage"].Visible = false;
            if (dgvUsersResponsibilities.Columns.Contains("EstimatedTime"))
                dgvUsersResponsibilities.Columns["EstimatedTime"].Visible = false;
            if (dgvUsersResponsibilities.Columns.Contains("FunctionExecTypeID"))
                dgvUsersResponsibilities.Columns["FunctionExecTypeID"].Visible = false;
            if (dgvUsersResponsibilities.Columns.Contains("WeekDays"))
                dgvUsersResponsibilities.Columns["WeekDays"].Visible = false;
            if (dgvUsersResponsibilities.Columns.Contains("YearDays"))
                dgvUsersResponsibilities.Columns["YearDays"].Visible = false;
            if (dgvUsersResponsibilities.Columns.Contains("ReadyPerform"))
                dgvUsersResponsibilities.Columns["ReadyPerform"].Visible = false;

            dgvUsersResponsibilities.Columns["FunctionName"].HeaderText = "Обязанность";
            int DisplayIndex = 0;
            dgvUsersResponsibilities.Columns["FunctionName"].DisplayIndex = DisplayIndex++;
        }

        private void cmbFactory_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FilterDepartments();
            FilterUsers();
            FilterPositions();
            FilterUsersResponsibilities();
        }

        private void FilterDepartments()
        {
            int FactoryID = 0;

            if (cmbFactory.SelectedItem != null)
                FactoryID = Convert.ToInt32(cmbFactory.SelectedValue);
            UsersResponsibilities.FilterDepartments(FactoryID);
        }

        private void FilterUsers()
        {
            int FactoryID = 0;
            int DepartmentID = 0;

            if (cmbFactory.SelectedItem != null)
                FactoryID = Convert.ToInt32(cmbFactory.SelectedValue);
            if (cmbDepartments.SelectedItem != null)
                DepartmentID = Convert.ToInt32(cmbDepartments.SelectedValue);
            UsersResponsibilities.FilterUsers(FactoryID, DepartmentID);
        }

        private void FilterPositions()
        {
            int FactoryID = 0;
            int DepartmentID = 0;
            int UserID = 0;

            if (cmbFactory.SelectedItem != null)
                FactoryID = Convert.ToInt32(cmbFactory.SelectedValue);
            if (cmbDepartments.SelectedItem != null)
                DepartmentID = Convert.ToInt32(cmbDepartments.SelectedValue);
            if (cmbUsers.SelectedItem != null)
                UserID = Convert.ToInt32(cmbUsers.SelectedValue);
            UsersResponsibilities.FilterPositions(FactoryID, DepartmentID, UserID);
        }

        private void FilterUsersResponsibilities()
        {
            int FactoryID = 0;
            int DepartmentID = 0;
            int PositionID = 0;
            int UserID = 0;

            if (cmbFactory.SelectedItem != null)
                FactoryID = Convert.ToInt32(cmbFactory.SelectedValue);
            if (cmbDepartments.SelectedItem != null)
                DepartmentID = Convert.ToInt32(cmbDepartments.SelectedValue);
            if (cmbUsers.SelectedItem != null)
                UserID = Convert.ToInt32(cmbUsers.SelectedValue);
            if (cmbPositions.SelectedItem != null)
                PositionID = Convert.ToInt32(cmbPositions.SelectedValue);
            UsersResponsibilities.FilterUsersResponsibilities(FactoryID, DepartmentID, UserID, PositionID);
        }

        private void cmbDepartments_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FilterUsers();
            FilterPositions();
            FilterUsersResponsibilities();
        }

        private void btnAddResponsibility_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            int FactoryID = 0;
            int DepartmentID = 0;
            int UserID = 0;
            int PositionID = 0;

            if (cmbFactory.SelectedItem != null)
                FactoryID = Convert.ToInt32(cmbFactory.SelectedValue);
            if (cmbDepartments.SelectedItem != null)
                DepartmentID = Convert.ToInt32(cmbDepartments.SelectedValue);
            if (cmbUsers.SelectedItem != null)
                UserID = Convert.ToInt32(cmbUsers.SelectedValue);
            if (cmbPositions.SelectedItem != null)
                PositionID = Convert.ToInt32(cmbPositions.SelectedValue);
            int StaffListID = UsersResponsibilities.GetStaffListID(FactoryID, DepartmentID, UserID, PositionID);
            FunctionsManagementForm FunctionsManagementForm = new FunctionsManagementForm(ref AdminFunctionsEdit, StaffListID, UserID);

            TopForm = FunctionsManagementForm;

            FunctionsManagementForm.ShowDialog();

            FunctionsManagementForm.Close();
            FunctionsManagementForm.Dispose();
            UsersResponsibilities.UpdateUsersResponsibilities();
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cmbUsers_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FilterPositions();
            FilterUsersResponsibilities();
        }

        private void btnDeleteResponsibility_Click(object sender, EventArgs e)
        {
            //bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
            //        "Функция временно недоступна",
            //        "Открепление обязанности от сотрудника");
            //if (!OKCancel)
            //    return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Открепить обязанность от сотрудника? Запись будет удалена безвозвратно.",
                    "Открепление обязанности");

            if (OKCancel)
            {
                int UserFunctionID = Convert.ToInt32(dgvUsersResponsibilities.SelectedRows[0].Cells["UserFunctionID"].Value);
                UsersResponsibilities.DeleteUsersResponsibility(UserFunctionID);
                InfiniumTips.ShowTip(this, 50, 85, "Удалено", 1700);
                UsersResponsibilities.UpdateUsersResponsibilities();
            }
        }

        private void cmbPositions_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FilterUsersResponsibilities();
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            UsersResponsibilitiesReport report = new UsersResponsibilitiesReport()
            {
                dFunctionsDT = UsersResponsibilities.FunctionsToExportDT
            };
            report.Report("Обязанности " + UsersResponsibilities.UserNameByID(Convert.ToInt32(cmbUsers.SelectedValue)),
                cmbFactory.Text, cmbDepartments.Text, cmbUsers.Text, cmbPositions.Text);
        }
    }
}
