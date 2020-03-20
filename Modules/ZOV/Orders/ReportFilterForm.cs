using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ReportFilterForm : Form
    {
        public static bool IsOKPress = true;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        public bool Curved = false;
        public bool Aluminium = false;
        public bool ArchDecor = false;
        public bool Glass = false;
        public bool Hands = false;

        DataTable DecorProducts;

        List<int> listBox2_selectionhistory;

        ArrayList ProductIDs;

        Form MainForm = null;

        public ReportFilterForm(ref ArrayList aProductIDs)
        {
            InitializeComponent();

            ProductIDs = aProductIDs;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void Initialize()
        {
            listBox2_selectionhistory = new List<int>();

            DecorProducts = new DataTable();

            using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter(
                "SELECT * FROM DecorProducts ORDER BY ProductName",
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(DecorProducts);
            }

            DecorCheckedListBox.DataSource = DecorProducts;
            DecorCheckedListBox.DisplayMember = "ProductName";
            DecorCheckedListBox.ValueMember = "ProductID";
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

        private void ReportFilterForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MarketingSplitPackagesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void DecorCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int actualcount = listBox2_selectionhistory.Count;
            if (actualcount == 1)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    int lastindex = listBox2_selectionhistory[0];
                    int currentindex = DecorCheckedListBox.SelectedIndex;
                    int upper = Math.Max(lastindex, currentindex);
                    int lower = Math.Min(lastindex, currentindex);
                    for (int i = lower; i <= upper; i++)
                    {
                        DecorCheckedListBox.SetItemCheckState(i, CheckState.Checked);
                    }
                }
                listBox2_selectionhistory.Clear();
                listBox2_selectionhistory.Add(DecorCheckedListBox.SelectedIndex);
            }
            else
            {
                listBox2_selectionhistory.Clear();
                listBox2_selectionhistory.Add(DecorCheckedListBox.SelectedIndex);
            }
        }

        private void CheckAllDecorButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < DecorCheckedListBox.Items.Count; i++)
                DecorCheckedListBox.SetItemChecked(i, true);
        }

        private void UnCheckAllDecorButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < DecorCheckedListBox.Items.Count; i++)
                DecorCheckedListBox.SetItemChecked(i, false);
        }

        private void GetDecor()
        {
            foreach (object itemChecked in DecorCheckedListBox.CheckedItems)
            {
                DataRowView castedItem = (DataRowView)itemChecked;
                ProductIDs.Add(Convert.ToInt32(castedItem["ProductID"]));
            }
        }

        private void OKReportButton_Click(object sender, EventArgs e)
        {
            ProductIDs.Clear();

            Curved = CurvedFrontsButton.Checked;
            Aluminium = AluminiumFrontsButton.Checked;
            ArchDecor = ArchDecorButton.Checked;
            foreach (object itemChecked in DecorCheckedListBox.CheckedItems)
            {
                DataRowView castedItem = (DataRowView)itemChecked;
                if (castedItem["ProductName"].ToString() == "Ручки" || castedItem["ProductName"].ToString() == "Ручки УКВ")
                    Hands = true;
                if (castedItem["ProductName"].ToString() == "Стекло")
                    Glass = true;
            }
            GetDecor();
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
            IsOKPress = true;
        }

        private void CancelReportButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
            IsOKPress = false;
        }

        private void CheckAllDecorButton_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckAllDecorButton.Checked)
                CheckAllDecorButton_Click(null, null);
            else
                UnCheckAllDecorButton_Click(null, null);
        }

        private void ArchDecorButton_CheckedChanged(object sender, EventArgs e)
        {
            int ProductID = 0;
            for (int i = 0; i < DecorCheckedListBox.Items.Count; i++)
            {
                DataRowView castedItem = (DataRowView)DecorCheckedListBox.Items[i];
                ProductID = Convert.ToInt32(castedItem["ProductID"]);
                if (ProductID == 31 || ProductID == 4 || ProductID == 32 ||
                       ProductID == 24 || ProductID == 18 || ProductID == 19 ||
                       ProductID == 27 || ProductID == 28 || ProductID == 26 ||
                       ProductID == 11 || ProductID == 12)
                    continue;
                else
                    DecorCheckedListBox.SetItemChecked(i, ArchDecorButton.Checked);
            }
        }
    }
}
