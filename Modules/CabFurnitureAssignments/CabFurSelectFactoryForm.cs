using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CabFurSelectFactoryForm : Form
    {
        public bool OKCancel = false;
        public bool bTPS = false;

        public CabFurSelectFactoryForm()
        {
            InitializeComponent();
        }

        private void OKMessageButton_Click(object sender, EventArgs e)
        {
            if (rbtn2.Checked)
                bTPS = true;
            OKCancel = true;

            this.Close();
        }

        private void CancelMessageButton_Click(object sender, EventArgs e)
        {
            bTPS = false;
            OKCancel = false;

            this.Close();
        }

        private void LightMessageBoxForm_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //    OKMessageButton_Click(null, null);
            //if (e.KeyCode == Keys.Cancel)
            //    CancelMessageButton_Click(null, null);
        }
    }
}
