using System;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class UsersForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;


        LightUsers LightUsers;
        LightStartForm LightStartForm;

        Form TopForm = null;

        public UsersForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;


            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void UsersForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            FormEvent = eShow;
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

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.Activate();
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

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.Activate();
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

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void NavigateMenuBackButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }


        private void Initialize()
        {
            LightUsers = new LightUsers();

            DepartmentsDataGrid.DataSource = LightUsers.DepartmentsDataTable;
            DepartmentsDataGrid.Columns["DepartmentID"].Visible = false;
            DepartmentsDataGrid.Columns["Photo"].Visible = false;
            DepartmentsDataGrid.Columns["Count"].Visible = false;
            DepartmentsDataGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DepartmentsDataGrid.Columns["Name"].MinimumWidth = 150;

            UsersList.UsersDataTable = LightUsers.UsersDataTable;
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

        private void DepartmentList_Click(object sender, EventArgs e)
        {
            kryptonCheckSet1_CheckedButtonChanged(null, null);
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (LightUsers == null || LightUsers.DepartmentsDataTable.Rows.Count < 1 || DepartmentsDataGrid.SelectedRows.Count < 1)
                return;

            if (kryptonCheckSet1.CheckedButton.Name == "AlphabeticalButton")
            {
                if (DepartmentsDataGrid.SelectedRows[0].Cells["DepartmentID"].Value.ToString() == "")
                {
                    UsersList.Filter("");
                    return;
                }
                else
                {
                    UsersList.Filter("DepartmentID = " + Convert.ToInt32(DepartmentsDataGrid.SelectedRows[0].Cells["DepartmentID"].Value));

                    return;
                }
            }

            if (kryptonCheckSet1.CheckedButton.Name == "OnlineButton")
            {
                if (DepartmentsDataGrid.SelectedRows[0].Cells["DepartmentID"].Value.ToString() == "")
                {
                    UsersList.Filter("Online = true");
                    return;
                }

                UsersList.Filter("DepartmentID = " + Convert.ToInt32(DepartmentsDataGrid.SelectedRows[0].Cells["DepartmentID"].Value) + " AND Online = true");
            }

            if (kryptonCheckSet1.CheckedButton.Name == "OfflineButton")
            {
                if (DepartmentsDataGrid.SelectedRows[0].Cells["DepartmentID"].Value.ToString() == "")
                {
                    UsersList.Filter("Online = false");
                    return;
                }

                UsersList.Filter("DepartmentID = " + Convert.ToInt32(DepartmentsDataGrid.SelectedRows[0].Cells["DepartmentID"].Value) + " AND Online = false");
            }
        }

        private void menuLabel2_Click(object sender, EventArgs e)
        {

        }

        private void kryptonCheckSet2_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (kryptonCheckSet2.CheckedButton == ShortViewButton)
                UsersList.ShortView = true;
            else
                UsersList.ShortView = false;
        }

        private void UsersList_UserClick(object sender, string Name)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            UserInfoForm UserInfoForm = new UserInfoForm(Name);

            TopForm = UserInfoForm;
            UserInfoForm.ShowDialog();
            TopForm = null;

            UserInfoForm.Dispose();
            UserInfoForm = null;
        }

        private void DepartmentsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            kryptonCheckSet1_CheckedButtonChanged(null, null);
        }


    }
}
