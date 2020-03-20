using System;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DepartmentsManagementForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;

        LightStartForm LightStartForm;


        AdminDepartmentEdit AdminDepartmentsEdit;

        public DepartmentsManagementForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            AdminDepartmentsEdit = new AdminDepartmentEdit();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;
        }


        private void EditDepartmentsForm_Shown(object sender, EventArgs e)
        {
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
            UsersGrid.DataSource = AdminDepartmentsEdit.UsersBindingSource;

            DepartmentsDataGrid.DataSource = AdminDepartmentsEdit.DepartmentsDataTable;
            DepartmentsDataGrid.Columns["DepartmentID"].Visible = false;
            DepartmentsDataGrid.Columns["DepartmentName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DepartmentsDataGrid.Columns["DepartmentName"].MinimumWidth = 150;

            //DataGridViewComboBoxColumn PositionColumn = new DataGridViewComboBoxColumn();
            //PositionColumn.Name = "PositionColumn";
            //PositionColumn.HeaderText = "Должность";
            //PositionColumn.DataPropertyName = "PositionID";
            //PositionColumn.DataSource = new DataView(AdminDepartmentsEdit.PositionsDataTable);
            //PositionColumn.ValueMember = "PositionID";
            //PositionColumn.DisplayMember = "Position";
            //PositionColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //UsersGrid.Columns.Add(PositionColumn);
            UsersGrid.Columns["DepartmentID"].Visible = false;
            UsersGrid.Columns["Name"].HeaderText = "ФИО";
            UsersGrid.Columns["FactoryName"].HeaderText = "Участок";
            UsersGrid.Columns["Position"].HeaderText = "Должность";
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

        private void PhotoEditButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            DepartmentPhotoEditForm DepartmentPhotoEditForm = new DepartmentPhotoEditForm(ref AdminDepartmentsEdit,
                Convert.ToInt32(DepartmentsDataGrid.SelectedRows[0].Cells["DepartmentID"].Value));

            TopForm = DepartmentPhotoEditForm;

            DepartmentPhotoEditForm.ShowDialog();

            DepartmentPhotoEditForm.Dispose();

            TopForm = null;

            DepartmentsDataGrid_SelectionChanged(null, null);
        }

        private void AddDepartmentButton_Click(object sender, EventArgs e)
        {
            if (NewDepartmentTextBox.Text.Length == 0)
                return;

            AdminDepartmentsEdit.AddDepartment(NewDepartmentTextBox.Text);

            InfiniumTips.ShowTip(this, 50, 85, "Отдел добавлен", 1700);
        }

        private void DepartmentsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (AdminDepartmentsEdit == null)
                return;

            if (AdminDepartmentsEdit.DepartmentsDataTable.Rows.Count > 0 && DepartmentsDataGrid.SelectedRows.Count > 0)
            {
                AdminDepartmentsEdit.UsersBindingSource.Filter = "DepartmentID = " + Convert.ToInt32(DepartmentsDataGrid.SelectedRows[0].Cells["DepartmentID"].Value);
                NameLabel.Text = (DepartmentsDataGrid.SelectedRows[0].Cells["DepartmentName"].Value.ToString());
                photoBox1.Image = AdminDepartmentsEdit.GetPhoto(Convert.ToInt32(DepartmentsDataGrid.SelectedRows[0].Cells["DepartmentID"].Value));
            }
        }

        private void btnSaveDepartments_Click(object sender, EventArgs e)
        {
            AdminDepartmentsEdit.SaveDepartments();
            InfiniumTips.ShowTip(this, 50, 85, "Сохранение завершено", 1700);
        }
    }
}
