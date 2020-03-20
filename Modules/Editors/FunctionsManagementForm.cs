using System;
using System.Drawing;
using System.Windows.Forms;

namespace Infinium
{
    public partial class FunctionsManagementForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;
        int CurrentFunctionID = 0;

        Form TopForm = null;
        LightStartForm LightStartForm;

        bool bFromStartMenu = true;
        public bool bAddUserResponsibility = false;
        public int StaffListID = -1;
        public int UserID = -1;

        AdminFunctionsEdit AdminFunctionsEdit;

        public FunctionsManagementForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            AdminFunctionsEdit = new AdminFunctionsEdit();

            while (!SplashForm.bCreated) ;
        }

        public FunctionsManagementForm(int iStaffListID, int iUserID)
        {
            bFromStartMenu = false;
            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            StaffListID = iStaffListID;
            UserID = iUserID;
            AdminFunctionsEdit = new AdminFunctionsEdit();
            btnAddFunction.Visible = false;
            btnEditFunction.Visible = false;
            btnDeleteFunction.Visible = false;
            btnAddUserResponsibility.Visible = true;
            btnFormClose.Visible = true;
            while (!SplashForm.bCreated) ;
        }

        public FunctionsManagementForm(ref AdminFunctionsEdit tAdminFunctionsEdit, int iStaffListID, int iUserID)
        {
            bFromStartMenu = false;
            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            StaffListID = iStaffListID;
            UserID = iUserID;
            AdminFunctionsEdit = tAdminFunctionsEdit;
            btnAddFunction.Visible = false;
            btnEditFunction.Visible = false;
            btnDeleteFunction.Visible = false;
            btnAddUserResponsibility.Visible = true;
            btnFormClose.Visible = true;
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
                        if (bFromStartMenu)
                            LightStartForm.CloseForm(this);
                        else
                            this.Close();
                    }

                    if (FormEvent == eHide)
                    {

                        if (bFromStartMenu)
                            LightStartForm.HideForm(this);
                        else
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

                        if (bFromStartMenu)
                            LightStartForm.CloseForm(this);
                        else
                            this.Close();
                    }

                    if (FormEvent == eHide)
                    {

                        if (bFromStartMenu)
                            LightStartForm.HideForm(this);
                        else
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

        private void FunctionsManagementForm_Shown(object sender, EventArgs e)
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

        private void AdminResponsibilitiesForm_Load(object sender, EventArgs e)
        {
            DepartmentsDataGrid.DataSource = AdminFunctionsEdit.DepartmentsDataTable;
            DepartmentsDataGrid.Columns["DepartmentID"].Visible = false;
            DepartmentsDataGrid.Columns["DepartmentName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DepartmentsDataGrid.Columns["DepartmentName"].MinimumWidth = 150;

            dgvAllFunctions.DataSource = AdminFunctionsEdit.FunctionsBindingSource;
            foreach (DataGridViewColumn Column in dgvAllFunctions.Columns)
            {
                Column.Visible = false;
            }
            if (dgvAllFunctions.Columns.Contains("FunctionName"))
                dgvAllFunctions.Columns["FunctionName"].Visible = true;
        }

        private void btnAddFunction_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            AddFunctionsForm AddResponsibilitiesForm = new AddFunctionsForm(ref AdminFunctionsEdit);

            TopForm = AddResponsibilitiesForm;

            AddResponsibilitiesForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            AddResponsibilitiesForm.Dispose();
        }

        private void btnEditFunction_Click(object sender, EventArgs e)
        {
            if (dgvAllFunctions.SelectedRows.Count == 0)
                return;
            int FunctionID = Convert.ToInt32(dgvAllFunctions.SelectedRows[0].Cells["FunctionID"].Value);
            string FunctionName = dgvAllFunctions.SelectedRows[0].Cells["FunctionName"].Value.ToString();
            string FunctionDescription = dgvAllFunctions.SelectedRows[0].Cells["FunctionDescription"].Value.ToString();

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            AddFunctionsForm AddResponsibilitiesForm = new AddFunctionsForm(ref AdminFunctionsEdit,
                FunctionID, FunctionName, FunctionDescription);

            TopForm = AddResponsibilitiesForm;

            AddResponsibilitiesForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            AddResponsibilitiesForm.Dispose();
            AdminFunctionsEdit.MoveToFunction(FunctionID);
        }

        private void dgvAllFunctions_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAllFunctions.SelectedRows.Count == 0)
                return;
            int FunctionID = Convert.ToInt32(dgvAllFunctions.SelectedRows[0].Cells["FunctionID"].Value);
            string FunctionName = dgvAllFunctions.SelectedRows[0].Cells["FunctionName"].Value.ToString();
            string FunctionDescription = dgvAllFunctions.SelectedRows[0].Cells["FunctionDescription"].Value.ToString();

            rtbFunctionDescription.Text = FunctionDescription;
        }

        private void DepartmentsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (AdminFunctionsEdit == null)
                return;

            int DepartmentID = 0;
            if (DepartmentsDataGrid.SelectedRows.Count != 0 && DepartmentsDataGrid.SelectedRows[0].Cells["DepartmentID"].Value != DBNull.Value)
                DepartmentID = Convert.ToInt32(DepartmentsDataGrid.SelectedRows[0].Cells["DepartmentID"].Value);
            AdminFunctionsEdit.CurrentDepartmentID = DepartmentID;
            AdminFunctionsEdit.FilterFunctions(DepartmentID);
        }

        private void btnDeleteFunction_Click(object sender, EventArgs e)
        {
            //bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
            //        "Функция временно недоступна",
            //        "Открепление обязанности от сотрудника");
            //if (!OKCancel)
            //    return;
            if (dgvAllFunctions.SelectedRows.Count != 0)
            {
                bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Удалить обязаность? Запись будет удалена безвозвратно.",
                        "Удаление обязаности");

                if (OKCancel)
                {
                    int FunctionID = Convert.ToInt32(dgvAllFunctions.SelectedRows[0].Cells["FunctionID"].Value);
                    AdminFunctionsEdit.DeleteFunction(FunctionID);
                    AdminFunctionsEdit.Save();
                }
            }
        }

        private void dgvAllFunctions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvAllFunctions.SelectedRows.Count == 0)
                return;
            int FunctionID = Convert.ToInt32(dgvAllFunctions.SelectedRows[0].Cells["FunctionID"].Value);
            string FunctionName = dgvAllFunctions.SelectedRows[0].Cells["FunctionName"].Value.ToString();
            string FunctionDescription = dgvAllFunctions.SelectedRows[0].Cells["FunctionDescription"].Value.ToString();

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            AddFunctionsForm AddResponsibilitiesForm = new AddFunctionsForm(ref AdminFunctionsEdit,
                FunctionID, FunctionName, FunctionDescription);

            TopForm = AddResponsibilitiesForm;

            AddResponsibilitiesForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            AddResponsibilitiesForm.Dispose();
            AdminFunctionsEdit.MoveToFunction(FunctionID);
        }

        private void btnAddUserResponsibility_Click(object sender, EventArgs e)
        {
            int FunctionID = -1;
            if (dgvAllFunctions.SelectedRows.Count != 0 && dgvAllFunctions.SelectedRows[0].Cells["FunctionID"].Value != DBNull.Value)
                FunctionID = Convert.ToInt32(dgvAllFunctions.SelectedRows[0].Cells["FunctionID"].Value);
            if (FunctionID == -1)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Обязанность не выбрана",
                        "Ошибка добавления обязанности");
                return;
            }
            if (!AdminFunctionsEdit.AddUsersResponsibility(StaffListID, UserID, FunctionID))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                        "Обязанность уже добавлена",
                        "Ошибка добавления обязанности");
                return;
            }
            InfiniumTips.ShowTip(this, 50, 85, "Обязанность прикреплена", 1700);
        }

        private void btnFormClose_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void DepartmentsDataGrid_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void DepartmentsDataGrid_DragDrop(object sender, DragEventArgs e)
        {
            Point clientPoint = DepartmentsDataGrid.PointToClient(new Point(e.X, e.Y));
            if (e.Effect == DragDropEffects.Move)
            {
                //int DepartmentID = (int)e.Data.GetData(typeof(int));
                var hittest = DepartmentsDataGrid.HitTest(clientPoint.X, clientPoint.Y);
                if (hittest.ColumnIndex != -1 && hittest.RowIndex != -1)
                {
                    AdminFunctionsEdit.ChangeDepartment(CurrentFunctionID, Convert.ToInt32(DepartmentsDataGrid.Rows[hittest.RowIndex].Cells["DepartmentID"].Value));
                    AdminFunctionsEdit.Save();
                    InfiniumTips.ShowTip(this, 50, 85, "Обязанность перенесена в отдел \"" +
                        DepartmentsDataGrid.Rows[hittest.RowIndex].Cells["DepartmentName"].Value.ToString() + "\"", 1700);
                }
            }
        }

        private void dgvAllFunctions_MouseDown(object sender, MouseEventArgs e)
        {// Get the index of the item the mouse is below.
            var hittestInfo = dgvAllFunctions.HitTest(e.X, e.Y);

            if (hittestInfo.RowIndex != -1 && hittestInfo.ColumnIndex != -1)
            {
                valueFromMouseDown = dgvAllFunctions.Rows[hittestInfo.RowIndex].Cells[hittestInfo.ColumnIndex].Value;
                CurrentFunctionID = Convert.ToInt32(dgvAllFunctions.Rows[hittestInfo.RowIndex].Cells["FunctionID"].Value);
                if (valueFromMouseDown != null)
                {
                    // Remember the point where the mouse down occurred. 
                    // The DragSize indicates the size that the mouse can move 
                    // before a drag event should be started.                
                    Size dragSize = SystemInformation.DragSize;

                    // Create a rectangle using the DragSize, with the mouse position being
                    // at the center of the rectangle.
                    dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
                }
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private Rectangle dragBoxFromMouseDown;
        private object valueFromMouseDown;
        private void dgvAllFunctions_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = dgvAllFunctions.DoDragDrop(valueFromMouseDown, DragDropEffects.Move);
                }
            }
        }

        private void DepartmentsDataGrid_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void dgvAllFunctions_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            PercentageDataGrid grid = (PercentageDataGrid)sender;
            int FunctionID = 0;
            if (grid.Rows[e.RowIndex].Cells["FunctionID"].Value != DBNull.Value)
                FunctionID = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["FunctionID"].Value);
            bool IsFunctionUsing = AdminFunctionsEdit.IsFunctionUsing(FunctionID);
            if (IsFunctionUsing)
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.Green;
            }
            else
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                grid.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }
    }
}
