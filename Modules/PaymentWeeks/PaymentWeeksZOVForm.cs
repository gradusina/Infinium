using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class PaymentWeeksZOVForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        Form TopForm = null;
        PaymentWeeksZOVSelectDateForm MainForm = null;

        int PaymentWeekID = 0;

        public Modules.PaymentWeeks.PaymentWeeks PaymentWeeks;

        public PaymentWeeksZOVForm(PaymentWeeksZOVSelectDateForm tMainForm, int tPaymentWeekID, string Period)
        {
            InitializeComponent();
            MainForm = tMainForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            PaymentWeekID = tPaymentWeekID;

            label1.Text = "Infinium. Расчетная неделя " + Period;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void PaymentWeeksZOVForm_Shown(object sender, EventArgs e)
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

                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        MainForm.Activate();
                        this.Hide();
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
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        MainForm.Activate();
                        this.Hide();
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

        private void NavigateMenuHerculesButton_Click(object sender, EventArgs e)
        {
            FormEvent = eMainMenu;
            AnimateTimer.Enabled = true;
        }



        private void Initialize()
        {
            PaymentWeeks = new Modules.PaymentWeeks.PaymentWeeks(ref ResultTotalDataGrid, ref WriteOffDataGrid, ref CalcWriteOffDataGrid,
                                                                 ref WriteOffDetailGrid, ref ResultLabel, ref CalcWriteOffResultLabel,
                                                                 ref WriteOffResultLabel);
            PaymentWeeks.Load(PaymentWeekID);
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

        private void WriteOffDetailGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Add this
                WriteOffDetailGrid.CurrentCell = WriteOffDetailGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                // Can leave these here - doesn't hurt
                WriteOffDetailGrid.Rows[e.RowIndex].Selected = true;
                WriteOffDetailGrid.Focus();

                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void btnMoveToExpedition_Click(object sender, EventArgs e)
        {
            string DocNumber = WriteOffDetailGrid.SelectedRows[0].Cells["DocNumber"].Value.ToString();
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ZOVExpeditionForm ZOVExpeditionForm = new ZOVExpeditionForm(this, DocNumber);

            TopForm = ZOVExpeditionForm;

            ZOVExpeditionForm.ShowDialog();

            ZOVExpeditionForm.Close();
            ZOVExpeditionForm.Dispose();

            TopForm = null;
        }
    }
}
