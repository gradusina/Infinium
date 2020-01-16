using System;
using System.Windows.Forms;

namespace Infinium
{
    public partial class PhantomForm : Form
    {
        public PhantomForm()
        {
            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
        }

        private void PhantomForm_Shown(object sender, EventArgs e)
        {

        }
    }
}
