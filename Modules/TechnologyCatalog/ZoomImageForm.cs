using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ZoomImageForm : Form
    {
        Form TopForm = null;

        public ZoomImageForm(Image ZoomImage, ref Form tTopForm)
        {
            TopForm = tTopForm;

            InitializeComponent();
            if (ZoomImage != null)
            {
                if (ZoomImage.Height > pictureBox1.Height | ZoomImage.Width > pictureBox1.Width)
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                else
                    pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            pictureBox1.Image = ZoomImage;
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

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            GC.Collect();
            this.Close();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (((PictureBox)sender).Image == null)
            {
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                e.Graphics.DrawString("Нет данных", new Font("Segoe UI Semilight", 15f, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.Gray), (((PictureBox)sender).Width - 80) / 2, (((PictureBox)sender).Height - 15) / 2 - 2);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            GC.Collect();
            this.Close();
        }
    }
}
