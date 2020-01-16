using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Infinium
{
    public partial class InsetTypesSelectForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        DataTable dtInsetTypes;
        public bool PressOK = false;
        public int GroupID = 0;
        int FormEvent = 0;

        Form MainForm = null;

        public InsetTypesSelectForm(Form tMainForm)
        {
            MainForm = tMainForm;
            InitializeComponent();
            dtInsetTypes = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetGroups ORDER BY GroupName",
                ConnectionStrings.CatalogConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Fill(dtInsetTypes);
                }
            }

            //dtInsetTypes.Columns.Add(new DataColumn("GroupID", Type.GetType("System.Int32")));
            //dtInsetTypes.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            //{
            //    DataRow NewRow = dtInsetTypes.NewRow();
            //    NewRow["GroupID"] = 2;
            //    NewRow["Name"] = "Стекло";
            //    dtInsetTypes.Rows.Add(NewRow);
            //}
            //{
            //    DataRow NewRow = dtInsetTypes.NewRow();
            //    NewRow["GroupID"] = 3;
            //    NewRow["Name"] = "Кашир";
            //    dtInsetTypes.Rows.Add(NewRow);
            //}
            //{
            //    DataRow NewRow = dtInsetTypes.NewRow();
            //    NewRow["GroupID"] = 4;
            //    NewRow["Name"] = "ЛДСтП";
            //    dtInsetTypes.Rows.Add(NewRow);
            //}
            //{
            //    DataRow NewRow = dtInsetTypes.NewRow();
            //    NewRow["GroupID"] = 5;
            //    NewRow["Name"] = "Решетка ПВХ";
            //    dtInsetTypes.Rows.Add(NewRow);
            //}
            //{
            //    DataRow NewRow = dtInsetTypes.NewRow();
            //    NewRow["GroupID"] = 6;
            //    NewRow["Name"] = "Кромка";
            //    dtInsetTypes.Rows.Add(NewRow);
            //}
            dgvInsetTypes.AllowUserToAddRows = false;
            dgvInsetTypes.DataSource = dtInsetTypes;
            dgvInsetTypes.Columns["GroupID"].Visible = false;
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

        private void btnOKInput_Click(object sender, EventArgs e)
        {
            PressOK = true;
            if (dgvInsetTypes.SelectedRows.Count > 0 && dgvInsetTypes.SelectedRows[0].Cells["GroupID"].Value != DBNull.Value)
                GroupID = Convert.ToInt32(dgvInsetTypes.SelectedRows[0].Cells["GroupID"].Value);
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnCancelInput_Click(object sender, EventArgs e)
        {
            PressOK = false;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }
    }
}
