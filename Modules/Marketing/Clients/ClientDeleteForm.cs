using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ClientDeleteForm : Form
    {
        public bool OKCancel = false;
        public bool bDeleteOrders = false;

        public ClientDeleteForm()
        {
            InitializeComponent();
        }

        private void OKMessageButton_Click(object sender, EventArgs e)
        {
            bDeleteOrders = cbDeleteOrders.Checked;
            OKCancel = true;

            this.Close();
        }

        private void CancelMessageButton_Click(object sender, EventArgs e)
        {
            bDeleteOrders = false;
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
