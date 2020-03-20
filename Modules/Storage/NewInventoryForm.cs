using Infinium.Store;

using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace Infinium
{
    public partial class NewInventoryForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        DataTable MonthsDT;
        Form MainForm = null;
        NewInventoryParameters NewInventoryParameters;

        public NewInventoryForm(Form tMainForm, ref NewInventoryParameters tNewInventoryParameters)
        {
            MainForm = tMainForm;
            NewInventoryParameters = tNewInventoryParameters;
            InitializeComponent();
        }

        private void OKReportButton_Click(object sender, EventArgs e)
        {
            NewInventoryParameters.OKPress = true;
            NewInventoryParameters.Month = Convert.ToInt32(cbxMonths.SelectedValue);
            NewInventoryParameters.Year = Convert.ToInt32(cbxYears.SelectedValue);
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelReportButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
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

        private void Initialize()
        {
            MonthsDT = new DataTable();
            MonthsDT.Columns.Add(new DataColumn("MonthID", Type.GetType("System.Int32")));
            MonthsDT.Columns.Add(new DataColumn("MonthName", Type.GetType("System.String")));

            for (int i = 1; i <= 12; i++)
            {
                DataRow NewRow = MonthsDT.NewRow();
                NewRow["MonthID"] = i;
                NewRow["MonthName"] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).ToString();
                MonthsDT.Rows.Add(NewRow);
            }
            cbxMonths.DataSource = MonthsDT.DefaultView;
            cbxMonths.ValueMember = "MonthID";
            cbxMonths.DisplayMember = "MonthName";

            DateTime LastDay = new System.DateTime(DateTime.Now.Year, 12, 31);
            System.Collections.ArrayList Years = new System.Collections.ArrayList();

            for (int i = 2013; i <= LastDay.Year; i++)
            {
                Years.Add(i);
            }
            cbxYears.DataSource = Years.ToArray();
            cbxYears.SelectedIndex = cbxYears.Items.Count - 1;
        }

        private void NewInventoryForm_Load(object sender, EventArgs e)
        {
            Initialize();
        }
    }
}
