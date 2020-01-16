using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class LightMessageBoxForm : Form
    {
        public static bool OKCancel = false;

        public LightMessageBoxForm(bool ShowCancelButton, string Text, string Header)
        {
            InitializeComponent();

            label1.Text = Text;

            CancelMessageButton.Visible = ShowCancelButton;

            if (ShowCancelButton == false)
                OKMessageButton.Left = (this.Width - OKMessageButton.Width) / 2;

            label2.Text = Header;
        }

        private void OKMessageButton_Click(object sender, EventArgs e)
        {
            OKCancel = true;

            this.Close();
        }

        private void CancelMessageButton_Click(object sender, EventArgs e)
        {
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
