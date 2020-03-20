using Infinium.Modules.TechnologyCatalog;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MachinePictureEditForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;
        Form MainForm = null;

        TechStoreManager TechStoreManager;

        int MachineID;

        public MachinePictureEditForm(ref TechStoreManager tTechStoreManager, int tMachineID)
        {
            InitializeComponent();

            TechStoreManager = tTechStoreManager;
            MachineID = tMachineID;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            pictureBox1.Image = TechStoreManager.GetMachinePhoto(MachineID);
        }

        private void MachinePictureEditForm_Shown(object sender, EventArgs e)
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

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
            }
            catch
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Не удалось загрузить изображение",
                   "Предупреждение");
                LoadFileLabel.Visible = true;
                FileNameTextBox.Text = "";
                pictureBox1.Image = null;
                return;
            }

            if (pictureBox1.Image == null)
            {
                LoadFileLabel.Visible = true;
                FileNameTextBox.Text = "";
            }
            else
            {
                LoadFileLabel.Visible = false;
                FileNameTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void SaveImageButton_Click(object sender, EventArgs e)
        {
            TechStoreManager.SetMachinePhoto(MachineID, pictureBox1.Image);
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено...", 1700);
        }

        private void DeleteImageButton_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
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
