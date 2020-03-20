using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class PictureViewForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm;

        InfiniumPictures InfiniumPictures;
        InfiniumPicturesContainer PicturesContainer;

        int iAlbumID = -1;
        int iPictureID = -1;

        bool bC = false;

        public PictureViewForm(ref Infinium.InfiniumPictures tInfiniumPictures,
                               ref InfiniumPicturesContainer tPicturesContainer, int AlbumID, int PictureID, ref Form tTopForm)
        {
            InitializeComponent();

            InfiniumPictures = tInfiniumPictures;

            PicturesContainer = tPicturesContainer;

            iAlbumID = AlbumID;
            iPictureID = PictureID;

            TopForm = tTopForm;
        }

        public void LoadPic()
        {
            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(panel1.Top, panel1.Left,
                                               panel1.Height, panel1.Width, Color.FromArgb(60, 60, 60));
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            pictureBox1.Image = InfiniumPictures.GetPicture(iPictureID);

            int Count = PicturesContainer.PicItems[PicturesContainer.ItemsDataTable.Rows.IndexOf(
                PicturesContainer.ItemsDataTable.Select("PictureID = " + iPictureID)[0])].LikesCount;

            if (Count > 0)
            {
                PictureLikeButton.Image = Properties.Resources.LikePicBigActive;
                PictureLikeButton.Caption = Count.ToString();
            }
            else
            {
                PictureLikeButton.Image = Properties.Resources.LikePicBigInactive;
                PictureLikeButton.Caption = "";
            }

            GC.Collect();

            bC = true;
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
                    LoadPic();
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
                    LoadPic();
                }

                return;
            }
        }

        private void PictureViewForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }


        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {

        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bC)
            {
                while (SplashWindow.bSmallCreated)
                    CoverWaitForm.CloseS = true;

                bC = false;
            }
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            int index = InfiniumPictures.GetNextPictureID(iPictureID);

            if (index > -1)
            {
                iPictureID = index;
                LoadPic();
            }
        }

        private void PreviousButton_Click(object sender, EventArgs e)
        {
            int index = InfiniumPictures.GetPreviousPictureID(iPictureID);

            if (index > -1)
            {
                iPictureID = index;
                LoadPic();
            }
        }

        private void PictureLikeButton_Click(object sender, EventArgs e)
        {
            int Count = InfiniumPictures.LikePicture(iPictureID);
            PicturesContainer.RefreshItem(iPictureID, Count);

            if (Count > 0)
            {
                PictureLikeButton.Image = Properties.Resources.LikePicBigActive;
                PictureLikeButton.Caption = Count.ToString();
            }
            else
            {
                PictureLikeButton.Image = Properties.Resources.LikePicBigInactive;
                PictureLikeButton.Caption = "";
            }

            InfiniumPictures.PicturesItemsDataTable.Select("PictureID = " + iPictureID)[0]["LikesCount"] = Count;
        }
    }
}
