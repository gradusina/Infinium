using System;
using System.Globalization;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DateForm : Form
    {
        public static string SelectedDate = "";
        Form TopForm = null;

        public DateForm()
        {
            InitializeComponent();

            YearTrackBar.Properties.Maximum = Security.GetCurrentDate().Year;
            YearTrackBar.Properties.Minimum = YearTrackBar.Properties.Maximum - 100;
        }

        private void MonthTrackBar_EditValueChanged(object sender, EventArgs e)
        {
            CultureInfo CIRU = new CultureInfo("RU-ru");
            MonthTextBox.Text = CIRU.DateTimeFormat.MonthNames[MonthTrackBar.Value];
        }

        private void YearTrackBar_EditValueChanged(object sender, EventArgs e)
        {
            YearTextBox.Text = YearTrackBar.Value.ToString();
        }

        private void DayTrackBar_EditValueChanged(object sender, EventArgs e)
        {
            string ds = "";

            if (DayTrackBar.Value < 10)
                ds = "0" + DayTrackBar.Value;
            else
                ds = DayTrackBar.Value.ToString();

            DayTextBox.Text = ds;
        }

        private void OKDateButton_Click(object sender, EventArgs e)
        {
            string ms = "";

            if (MonthTrackBar.Value < 9)
                ms = "0" + (MonthTrackBar.Value + 1);
            else
                ms = (MonthTrackBar.Value + 1).ToString();

            SelectedDate = DayTextBox.Text + "." + ms + "." + YearTextBox.Text;

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
