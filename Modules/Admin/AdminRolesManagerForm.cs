using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AdminRolesManagerForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;
        LightStartForm LightStartForm;


        RolesAndPermissionsManager RolesAndPermissionsManager;

        public AdminRolesManagerForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            RolesAndPermissionsManager = new RolesAndPermissionsManager(ref ModulesDataGrid, ref RolesDataGrid, ref PermissionsDataGrid, ref
                                                                        RolesPermissionsDataGrid, ref RolesPermissionsRolesDataGrid,
                                                                        ref RoleUsersDataGrid, ref UserRolesDataGrid);

            UsersComboBox.DataSource = RolesAndPermissionsManager.UsersBindingSource;
            UsersComboBox.DisplayMember = "Name";
            UsersComboBox.ValueMember = "UserID";

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

        private void ResponsibilitiesForm_Shown(object sender, EventArgs e)
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

        private void xtraTabPage1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ModulesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (RolesAndPermissionsManager == null)
                return;

            if (RolesAndPermissionsManager.ModulesBindingSource.Count == 0)
                return;

            RolesAndPermissionsManager.FilterRoles(Convert.ToInt32(((DataRowView)RolesAndPermissionsManager.ModulesBindingSource.Current)["ModuleID"]));
            RolesAndPermissionsManager.FilterPermissions(Convert.ToInt32(((DataRowView)RolesAndPermissionsManager.ModulesBindingSource.Current)["ModuleID"]));
        }

        private void DeleteRoleButton_Click(object sender, EventArgs e)
        {
            RolesAndPermissionsManager.RemoveCurrentRole();
        }

        private void AddRoleButton_Click(object sender, EventArgs e)
        {
            if (RoleNameTextBox.Text.Length == 0)
                return;

            RolesAndPermissionsManager.AddRole(Convert.ToInt32(((DataRowView)RolesAndPermissionsManager.ModulesBindingSource.Current)["ModuleID"]),
                                               RoleNameTextBox.Text, RolesDescriptionRichTextBox.Text);
        }

        private void SaveRoleButton_Click(object sender, EventArgs e)
        {
            RolesAndPermissionsManager.SaveRoles(Convert.ToInt32(((DataRowView)RolesAndPermissionsManager.ModulesBindingSource.Current)["ModuleID"]));
        }

        private void RolesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            RolesDescriptionRichTextBox.Clear();

            if (RolesDataGrid.SelectedRows.Count > 0)
                RolesDescriptionRichTextBox.Text = RolesDataGrid.SelectedRows[0].Cells["RoleDescription"].FormattedValue.ToString();
        }

        private void AddPermissionButton_Click(object sender, EventArgs e)
        {
            if (PermissionNameTextBox.Text.Length == 0)
                return;

            RolesAndPermissionsManager.AddPermission(Convert.ToInt32(((DataRowView)RolesAndPermissionsManager.ModulesBindingSource.Current)["ModuleID"]),
                                               PermissionNameTextBox.Text, PermissionsRichTextBox.Text);
        }

        private void RemovePermissionButton_Click(object sender, EventArgs e)
        {
            RolesAndPermissionsManager.RemoveCurrentPermission();
        }

        private void SavePermissionsButton_Click(object sender, EventArgs e)
        {
            RolesAndPermissionsManager.SavePermissions(Convert.ToInt32(((DataRowView)RolesAndPermissionsManager.ModulesBindingSource.Current)["ModuleID"]));
        }

        private void PermissionsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            PermissionsRichTextBox.Clear();

            if (PermissionsDataGrid.SelectedRows.Count > 0)
                PermissionsRichTextBox.Text = PermissionsDataGrid.SelectedRows[0].Cells["PermissionDescription"].FormattedValue.ToString();
        }

        private void xtraTabPage4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void RolesPermissionsRolesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (RolesAndPermissionsManager == null)
            {
                return;
            }

            if (RolesAndPermissionsManager.RolesBindingSource.Count == 0)
            {
                RolesAndPermissionsManager.FilterRolePermissions(-1);
                return;
            }

            if (((DataRowView)RolesAndPermissionsManager.RolesBindingSource.Current)["RoleID"] == DBNull.Value)
            {
                RolesAndPermissionsManager.FilterRolePermissions(-1);
                return;
            }

            RolesAndPermissionsManager.FilterRolePermissions(Convert.ToInt32(((DataRowView)RolesAndPermissionsManager.RolesBindingSource.Current)["RoleID"]));
        }

        private void ClearDescription_Click(object sender, EventArgs e)
        {
            RolesAndPermissionsManager.UpdateRoleDescription(RolesDescriptionRichTextBox.Text);
        }

        private void UpdatePermissionsDescription_Click(object sender, EventArgs e)
        {
            RolesAndPermissionsManager.UpdatePermissionsDescription(PermissionsRichTextBox.Text);
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            RolesAndPermissionsManager.SaveRolePermissions();
        }

        private void RoleUsersDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (RolesAndPermissionsManager == null)
            {
                return;
            }

            if (RolesAndPermissionsManager.RolesBindingSource.Count == 0)
            {
                RolesAndPermissionsManager.FilterUserRoles(-1);
                return;
            }

            if (((DataRowView)RolesAndPermissionsManager.RolesBindingSource.Current)["RoleID"] == DBNull.Value)
            {
                RolesAndPermissionsManager.FilterUserRoles(-1);
                return;
            }

            RolesAndPermissionsManager.FilterUserRoles(Convert.ToInt32(((DataRowView)RolesAndPermissionsManager.RolesBindingSource.Current)["RoleID"]));
        }

        private void AddUser_Click(object sender, EventArgs e)
        {


            RolesAndPermissionsManager.AddUserRole(Convert.ToInt32(((DataRowView)RolesAndPermissionsManager.RolesBindingSource.Current)["RoleID"]),
                                                   Convert.ToInt32(UsersComboBox.SelectedValue));
        }

        private void RemoveCurrentUser_Click(object sender, EventArgs e)
        {
            RolesAndPermissionsManager.RemoveCurrentUserRole();
        }

        private void SaveUserRoles_Click(object sender, EventArgs e)
        {
            RolesAndPermissionsManager.SaveUserRoles();
        }
    }
}
