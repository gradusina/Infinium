using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AddFunctionsForm : Form
    {
        int FunctionID = 0;
        bool Edit = false;

        Form TopForm = null;

        AdminFunctionsEdit AdminFunctionsEdit;

        public AddFunctionsForm(ref AdminFunctionsEdit tAdminFunctionsEdit)
        {
            InitializeComponent();
            AdminFunctionsEdit = tAdminFunctionsEdit;
        }

        public AddFunctionsForm(ref AdminFunctionsEdit tAdminFunctionsEdit, int iFunctionID, string sFunctionName, string sFunctionDescription)
        {
            InitializeComponent();
            AdminFunctionsEdit = tAdminFunctionsEdit;

            FunctionID = iFunctionID;
            rtbFunctionName.Text = sFunctionName;
            rtbFunctionDescription.Text = sFunctionDescription;
            btnAddResponsibility.Text = "Сохранить";
            Edit = true;
        }

        private void btnbtnAddResponsibility_Click(object sender, EventArgs e)
        {
            if (!Edit)
            {
                if (string.IsNullOrWhiteSpace(rtbFunctionName.Text))
                    return;

                if (AdminFunctionsEdit.IsFunctionAlreadyAdded(AdminFunctionsEdit.CurrentDepartmentID, rtbFunctionName.Text, rtbFunctionDescription.Text))
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Обязанность уже существует", "Добавление обязанности");
                    return;
                }
                AdminFunctionsEdit.AddFunction(AdminFunctionsEdit.CurrentDepartmentID, rtbFunctionName.Text, rtbFunctionDescription.Text);

            }
            else
            {
                if (string.IsNullOrWhiteSpace(rtbFunctionName.Text))
                    return;

                AdminFunctionsEdit.EditFunction(FunctionID, rtbFunctionName.Text, rtbFunctionDescription.Text);
            }
            AdminFunctionsEdit.Save();

            this.Close();
        }

        private void btnFormClose_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void AddFunctionsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
