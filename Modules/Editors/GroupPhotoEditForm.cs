using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Infinium
{
    public partial class DepartmentPhotoEditForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;

        AdminDepartmentEdit AdminDepartmentsEdit;

        int iDepartmentID = -1;

        public DepartmentPhotoEditForm(ref AdminDepartmentEdit tAdminDepartmentsEdit, int DepartmentID)
        {
            InitializeComponent();

            AdminDepartmentsEdit = tAdminDepartmentsEdit;

            iDepartmentID = DepartmentID;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();
        }


        private void DepartmentPhotoEditForm_Shown(object sender, EventArgs e)
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




        private void Initialize()
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                cropImageEdit1.Image = Image.FromFile(openFileDialog1.FileName);
            }
            catch
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Не удалось загрузить изображение",
                   "Предупреждение");
                LoadFileLabel.Visible = true;
                InfoLabel.Visible = true;
                FileNameTextBox.Text = "";
                cropImageEdit1.Clear();
                return;
            }

            if (cropImageEdit1.Image == null)
            {
                LoadFileLabel.Visible = true;
                InfoLabel.Visible = true;
                FileNameTextBox.Text = "";
            }
            else
            {
                LoadFileLabel.Visible = false;
                InfoLabel.Visible = false;
                FileNameTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void cropImageEdit1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cropImageEdit1_FrameMoved(object sender, EventArgs e)
        {
            PhotoBox.Image = cropImageEdit1.CropImage();
        }

        private void SaveImageButton_Click(object sender, EventArgs e)
        {
            AdminDepartmentsEdit.SetDepartmentPhoto(PhotoBox.Image, iDepartmentID);
            LightMessageBox.Show(ref TopForm, false, "Аватар успешно сохранен", "Сохранение");
        }

        private void DeleteImageButton_Click(object sender, EventArgs e)
        {
            PhotoBox.Image = null;
            AdminDepartmentsEdit.SetDepartmentPhoto(null, iDepartmentID);
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
