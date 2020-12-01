using Infinium.Modules.TechnologyCatalog;

using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewCabFurCellForm : Form
    {
        public bool bOk = false;
        public string sName = string.Empty;

        Form TopForm = null;

        public NewCabFurCellForm(string oldName)
        {
            InitializeComponent();

            tbName.Text = oldName;
        }

        public NewCabFurCellForm()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное название",
                                    "Ошибка");
                return;
            }

            sName = tbName.Text;

            bOk = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
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
    }
}
