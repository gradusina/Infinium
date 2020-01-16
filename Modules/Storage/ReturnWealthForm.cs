using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ReturnWealthForm : Form
    {
        public bool ok = false;

        Form TopForm = null;

        public ReturnWealthForm()
        {
            InitializeComponent();
        }

        private void OKDateButton_Click(object sender, EventArgs e)
        {
            ok = true;
            this.Close();
        }

        private void CancelDateButton_Click(object sender, EventArgs e)
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
