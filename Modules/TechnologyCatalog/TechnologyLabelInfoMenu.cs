using Infinium.Modules.TechnologyCatalog;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Infinium
{
    public partial class TechnologyLabelInfoMenu : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedLength = false;
        bool NeedHeight = false;
        bool NeedWidth = false;
        public bool PressOK = false;
        public TechLabelInfo tlInfo;
        int FormEvent = 0;

        DataTable ColorsDT;
        BindingSource ColorsBS;

        private void GetColorsDT()
        {
            ColorsDT = new DataTable();
            ColorsDT.Columns.Add(new DataColumn("ColorID", Type.GetType("System.Int64")));
            ColorsDT.Columns.Add(new DataColumn("GroupID", Type.GetType("System.Int64")));
            ColorsDT.Columns.Add(new DataColumn("ColorName", Type.GetType("System.String")));
            string SelectCommand = @"SELECT TechStoreID, TechStoreName FROM TechStore
                WHERE TechStoreSubGroupID IN (SELECT TechStoreSubGroupID FROM TechStoreSubGroups WHERE TechStoreGroupID = 11)
                ORDER BY TechStoreName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = ColorsDT.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["TechStoreID"]);
                        NewRow["GroupID"] = 1;
                        NewRow["ColorName"] = DT.Rows[i]["TechStoreName"].ToString();
                        ColorsDT.Rows.Add(NewRow);
                    }
                }
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM InsetColors", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = ColorsDT.NewRow();
                        NewRow["ColorID"] = Convert.ToInt64(DT.Rows[i]["InsetColorID"]);
                        NewRow["GroupID"] = Convert.ToInt64(DT.Rows[i]["GroupID"]);
                        NewRow["ColorName"] = DT.Rows[i]["InsetColorName"].ToString();
                        ColorsDT.Rows.Add(NewRow);
                    }
                }
            }
            {
                DataRow NewRow = ColorsDT.NewRow();
                NewRow["ColorID"] = -1;
                NewRow["GroupID"] = -1;
                NewRow["ColorName"] = "-";
                ColorsDT.Rows.Add(NewRow);
            }
            {
                DataRow NewRow = ColorsDT.NewRow();
                NewRow["ColorID"] = 0;
                NewRow["GroupID"] = -1;
                NewRow["ColorName"] = "на выбор";
                ColorsDT.Rows.Add(NewRow);
            }
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(ColorsDT))
            {
                DV.Sort = "GroupID, ColorName";
                Table = DV.ToTable();
            }
            ColorsDT.Clear();
            ColorsDT = Table.Copy();
        }

        Form MainForm = null;

        public TechnologyLabelInfoMenu(Form tMainForm, bool bNeedLength, bool bNeedHeight, bool bNeedWidth)
        {
            MainForm = tMainForm;
            NeedLength = bNeedLength;
            NeedHeight = bNeedHeight;
            NeedWidth = bNeedWidth;
            InitializeComponent();
            if (!bNeedLength)
                tbLength.Enabled = false;
            if (!bNeedHeight)
                tbHeight.Enabled = false;
            if (!bNeedWidth)
                tbWidth.Enabled = false;
            GetColorsDT();
            ColorsBS = new BindingSource()
            {
                DataSource = new DataView(ColorsDT)
            };
            cbColors.DataSource = ColorsBS;
            cbColors.DisplayMember = "ColorName";
            cbColors.ValueMember = "ColorID";
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
            if (NeedLength)
                int.TryParse(tbLength.Text, out tlInfo.Length);
            if (NeedHeight)
                int.TryParse(tbHeight.Text, out tlInfo.Height);
            if (NeedWidth)
                int.TryParse(tbWidth.Text, out tlInfo.Width);
            int.TryParse(tbLabelsCount.Text, out tlInfo.LabelsCount);
            int.TryParse(tbPositionsCount.Text, out tlInfo.PositionsCount);
            tlInfo.DocDateTime = kryptonDateTimePicker1.Value.ToString("dd.MM.yyyy");
            if (rbProfil.Checked)
                tlInfo.Factory = 1;
            if (rbTPS.Checked)
                tlInfo.Factory = 2;
            if (cbColors.SelectedItem != null || ((DataRowView)cbColors.SelectedItem).Row["CoverID"] != DBNull.Value)
                tlInfo.Color = ((DataRowView)cbColors.SelectedItem).Row["ColorName"].ToString();

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnCancelInput_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }
    }
}
