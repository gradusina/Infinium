using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingPickFrontsSelectForm : Form
    {
        public bool IsOKPress = true;
        public bool bFrontType = true;
        public bool bFrameColor = false;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;
        int FactoryID = 1;

        DataTable Fronts;

        List<int> listBox1_selectionhistory;

        ArrayList FrontIDs;

        Form MainForm = null;

        public MarketingPickFrontsSelectForm(int iFactoryID, ref ArrayList aFrontIDs)
        {
            InitializeComponent();

            FactoryID = iFactoryID;
            FrontIDs = aFrontIDs;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void Initialize()
        {
            listBox1_selectionhistory = new List<int>();

            Fronts = new DataTable();

            string SelectCommand = @"SELECT TechStoreID AS FrontID, TechStoreName AS FrontName FROM TechStore 
                WHERE TechStoreID IN (SELECT FrontID FROM FrontsConfig WHERE Enabled = 1 AND FactoryID = " + FactoryID + @")
                ORDER BY TechStoreName";
            using (System.Data.SqlClient.SqlDataAdapter DA = new System.Data.SqlClient.SqlDataAdapter(SelectCommand,
                ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(Fronts);
            }

            FrontsCheckedListBox.DataSource = Fronts;
            FrontsCheckedListBox.DisplayMember = "FrontName";
            FrontsCheckedListBox.ValueMember = "FrontID";
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

        private void SplitPackagesForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MarketingPickFrontsSelectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void FrontsCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int actualcount = listBox1_selectionhistory.Count;
            if (actualcount == 1)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    int lastindex = listBox1_selectionhistory[0];
                    int currentindex = FrontsCheckedListBox.SelectedIndex;
                    int upper = Math.Max(lastindex, currentindex);
                    int lower = Math.Min(lastindex, currentindex);
                    for (int i = lower; i <= upper; i++)
                    {
                        FrontsCheckedListBox.SetItemCheckState(i, CheckState.Checked);
                    }
                }
                listBox1_selectionhistory.Clear();
                listBox1_selectionhistory.Add(FrontsCheckedListBox.SelectedIndex);
            }
            else
            {
                listBox1_selectionhistory.Clear();
                listBox1_selectionhistory.Add(FrontsCheckedListBox.SelectedIndex);
            }
        }

        private void GetFronts()
        {
            foreach (object itemChecked in FrontsCheckedListBox.CheckedItems)
            {
                DataRowView castedItem = (DataRowView)itemChecked;
                FrontIDs.Add(Convert.ToInt32(castedItem["FrontID"]));
            }
        }

        private void btnOkAdd_Click(object sender, EventArgs e)
        {
            FrontIDs.Clear();

            GetFronts();
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
            IsOKPress = true;
        }

        private void btnCancelAdd_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
            IsOKPress = false;
        }

        private void rbtnFrameColor_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbtnFrameColor.Checked)
                return;

            bFrontType = false;
            bFrameColor = true;
            FrontsCheckedListBox.Visible = false;
            this.Size = new Size(284, 129);
        }

        private void rbtnFrontType_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbtnFrontType.Checked)
                return;

            bFrontType = true;
            bFrameColor = false;
            FrontsCheckedListBox.Visible = true;
            this.Size = new Size(284, 395);
        }
    }
}
