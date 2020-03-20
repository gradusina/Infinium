using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Infinium
{
    public partial class InfiniumTipForm : Form
    {
        int iLeft = -1;
        int iTop = -1;
        int iTime = -1;
        int iLeftPercents = -1;
        int iTopPercents = -1;
        Form fParentForm;

        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        public InfiniumTipForm(int Left, int Top, int Time, string sText)
        {
            InitializeComponent();

            iLeft = Left;
            iTop = Top;

            TipsLabel1.Text = sText;

            iTime = Time;
        }

        public InfiniumTipForm(Form ParentForm, int LeftPercents, int TopPercents, int Time, string sText)
        {
            InitializeComponent();

            iLeftPercents = LeftPercents;
            iTopPercents = TopPercents;
            fParentForm = ParentForm;

            TipsLabel1.Text = sText;

            iTime = Time;
        }


        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern System.IntPtr CreateRoundRectRgn
        (
         int nLeftRect, // x-coordinate of upper-left corner
         int nTopRect, // y-coordinate of upper-left corner
         int nRightRect, // x-coordinate of lower-right corner
         int nBottomRect, // y-coordinate of lower-right corner
         int nWidthEllipse, // height of ellipse
         int nHeightEllipse // width of ellipse
        );

        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        private static extern bool DeleteObject(System.IntPtr hObject);

        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (GetActiveWindow() != this.Handle)
            //    this.Close();
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            if (iTopPercents > -1 || iLeftPercents > -1)
            {
                this.Left = fParentForm.Left + fParentForm.Width * iLeftPercents / 100 - this.Width / 2;
                this.Top = fParentForm.Top + fParentForm.Height * iTopPercents / 100 - this.Height / 2;
            }
            else
            {

                if (iLeft > -1 || iTop > -1)
                {
                    this.Left = iLeft;
                    this.Top = iTop;
                }
            }

            if (iTime > -1)
            {
                TimeStayTimer.Interval = iTime;
                TimeStayTimer.Enabled = true;
            }

            TipsLabel1_SizeChanged(null, null);

            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void TimeStayTimer_Tick(object sender, EventArgs e)
        {
            TimeStayTimer.Enabled = false;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void TipsLabel1_SizeChanged(object sender, EventArgs e)
        {
            if (TipsLabel1.Width > this.Width)
                this.Width = TipsLabel1.Width;

            if (TipsLabel1.Height > this.Height)
                this.Height = TipsLabel1.Height;

            TipsLabel1.Left = (this.Width - TipsLabel1.Width) / 2;
            TipsLabel1.Top = (this.Height - TipsLabel1.Height) / 2;
        }

        private void TipsLabel1_Click(object sender, EventArgs e)
        {
            TimeStayTimer.Enabled = false;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void Form2_Click(object sender, EventArgs e)
        {
            TimeStayTimer.Enabled = false;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void TipsLabel1_MouseMove(object sender, MouseEventArgs e)
        {
            TimeStayTimer.Enabled = false;
        }

        private void TipsLabel1_MouseLeave(object sender, EventArgs e)
        {
            TimeStayTimer.Enabled = true;
        }

        private void AnimateTimer_Tick(object sender, EventArgs e)
        {
            if (FormEvent == eClose)
            {
                if (Convert.ToDecimal(this.Opacity) != Convert.ToDecimal(0.00))
                    this.Opacity = Convert.ToDouble(Convert.ToDecimal(this.Opacity) - Convert.ToDecimal(0.10));
                else
                {
                    AnimateTimer.Enabled = false;

                    if (FormEvent == eClose)
                    {
                        this.Close();
                    }
                }

                return;
            }


            if (FormEvent == eShow)
            {
                if (this.Opacity < 0.88)
                    this.Opacity += 0.10;
                else
                {
                    AnimateTimer.Enabled = false;
                }

                return;
            }
        }

        private void TipsLabel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            System.IntPtr ptr = CreateRoundRectRgn(0, 0, this.Width, this.Height, 7, 7); // _BoarderRaduis can be adjusted to your needs, try 15 to start.
            this.Region = System.Drawing.Region.FromHrgn(ptr);
            DeleteObject(ptr);
        }
    }
}
