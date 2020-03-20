using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class PositionsManagementForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;

        LightStartForm LightStartForm;


        AdminPositionsEdit AdminPositionsEdit;

        public PositionsManagementForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            AdminPositionsEdit = new AdminPositionsEdit();

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
            UsersGrid.DataSource = AdminPositionsEdit.UsersBindingSource;

            PositionsDataGrid.DataSource = AdminPositionsEdit.PositionsDataTable;
            PositionsDataGrid.Columns["PositionID"].Visible = false;
            PositionsDataGrid.Columns["Position"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            PositionsDataGrid.Columns["Position"].MinimumWidth = 150;

            UsersGrid.Columns["PositionID"].Visible = false;
            UsersGrid.Columns["Name"].HeaderText = "ФИО";
            UsersGrid.Columns["FactoryName"].HeaderText = "Участок";
            UsersGrid.Columns["DepartmentName"].HeaderText = "Служба";
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

        private void AddDepartmentButton_Click(object sender, EventArgs e)
        {
            if (NewPositionTextBox.Text.Length == 0)
                return;

            AdminPositionsEdit.AddPosition(NewPositionTextBox.Text);
        }

        private void DepartmentsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (AdminPositionsEdit == null)
                return;

            if (AdminPositionsEdit.PositionsDataTable.Rows.Count > 0 && PositionsDataGrid.SelectedRows.Count > 0)
            {
                AdminPositionsEdit.UsersBindingSource.Filter = "PositionID = " + Convert.ToInt32(PositionsDataGrid.SelectedRows[0].Cells["PositionID"].Value);
                NameLabel.Text = (PositionsDataGrid.SelectedRows[0].Cells["Position"].Value.ToString());
            }
        }
    }
}
