using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Infinium
{
    public partial class PictureEditForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;
        Form MainForm = null;

        DecorCatalog DecorCatalog;

        int DecorID;

        public PictureEditForm(ref DecorCatalog tDecorCatalog, int tDecorID)
        {
            InitializeComponent();

            DecorCatalog = tDecorCatalog;
            DecorID = tDecorID;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            pictureBox1.Image = DecorCatalog.GetPicture(DecorID);

            Initialize();
        }


        private void PersonalSettingsForm_Shown(object sender, EventArgs e)
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




        private void Initialize()
        {

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

        private void cropImageEdit1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cropImageEdit1_FrameMoved(object sender, EventArgs e)
        {

        }

        private void SaveImageButton_Click(object sender, EventArgs e)
        {
            DecorCatalog.SetPicture(pictureBox1.Image, DecorID);

            if (pictureBox1.Image != null)
            {
                MemoryStream ms = new MemoryStream();

                ImageCodecInfo ImageCodecInfo = UserProfile.GetEncoderInfo("image/jpeg");
                System.Drawing.Imaging.Encoder eEncoder1 = System.Drawing.Imaging.Encoder.Quality;

                EncoderParameter myEncoderParameter1 = new EncoderParameter(eEncoder1, 100L);
                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                myEncoderParameters.Param[0] = myEncoderParameter1;

                pictureBox1.Image.Save(ms, ImageCodecInfo, myEncoderParameters);

                //DecorCatalog.DecorDataTable.Select("DecorID = " + DecorID)[0]["Picture"] = ms.ToArray();
                ms.Dispose();
            }
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
            //else
            //    DecorCatalog.DecorDataTable.Select("DecorID = " + DecorID)[0]["Picture"] = DBNull.Value;
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
