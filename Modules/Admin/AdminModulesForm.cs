using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AdminModulesForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;
        public AdminModulesManagement AdminModulesManagement = null;


        public AdminModulesForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();


            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;


            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void AdminModulesForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

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

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.HideForm(this);
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

                        LightStartForm.CloseForm(this);
                    }

                    if (FormEvent == eHide)
                    {

                        LightStartForm.HideForm(this);
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



        private void Initialize()
        {
            AdminModulesManagement = new AdminModulesManagement(ref ModulesDataGrid);

            MainMenuTabsListBox.DataSource = AdminModulesManagement.MainMenuTabsBingingSource;
            MainMenuTabsListBox.DisplayMember = "MainItemName";
            MainMenuTabsListBox.ValueMember = "MenuItemID";



            AdminModulesManagement.Rating();



            if (MainMenuTabsListBox.ValueMember == "")
                return;

            AdminModulesManagement.Filter(Convert.ToInt32(MainMenuTabsListBox.SelectedValue));
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            AdminModulesManagement.Save();
        }

        private void MainMenuTabsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AllModulesCheckBox.Checked)
                return;

            if (MainMenuTabsListBox.ValueMember == "")
                return;

            AdminModulesManagement.Filter(Convert.ToInt32(MainMenuTabsListBox.SelectedValue));
        }

        private void AllModulesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AllModulesCheckBox.Checked)
                AdminModulesManagement.ModulesBindingSource.Filter = "";
            else
            {
                if (MainMenuTabsListBox.ValueMember == "")
                    return;

                AdminModulesManagement.Filter(Convert.ToInt32(MainMenuTabsListBox.SelectedValue));
            }
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

        private void ModulesDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (AdminModulesManagement == null)
                return;

            if (AdminModulesManagement.ModulesBindingSource.Current == null)
                return;

            DataRow Row = AdminModulesManagement.ModulesDataTable.Select("ModuleID = " +
                ((DataRowView)AdminModulesManagement.ModulesBindingSource.Current)["ModuleID"])[0];

            if (Row["Picture"] == DBNull.Value)
            {
                pictureBox1.Image = null;
                return;
            }

            byte[] b = (byte[])Row["Picture"];
            MemoryStream ms = new MemoryStream(b);

            pictureBox1.Image = Image.FromStream(ms);
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);

                AdminModulesManagement.SetPicture(Convert.ToInt32(((DataRowView)AdminModulesManagement.ModulesBindingSource.Current)["ModuleID"]),
                pictureBox1.Image);
            }
        }
    }
}
