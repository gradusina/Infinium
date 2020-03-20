using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class PicturesForm : InfiniumForm
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;


        InfiniumPictures InfiniumPictures;

        bool bC = false;

        bool bShow = false;
        int iAlbumID = -1;

        //bool bNeedNewsSplash = false;

        //bool bNewProjectsSelected = false;
        //bool bNewMessagesSelected = false;

        public PicturesForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();



            ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);
            //ActiveNotifySystem.ClearModuleUpdates(LightTile.Name);
            //ActiveNotifySystem.ClearCurrentOpenModuleUpdates(this.Name);


            while (!SplashForm.bCreated) ;
        }

        private void LightNewsForm_Shown(object sender, EventArgs e)
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

                        LightStartForm.Activate();
                        this.Hide();
                    }


                    return;
                }

                if (FormEvent == eShow)
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;

                    PicturesAlbums.ItemsDataTable = InfiniumPictures.AlbumsItemsDataTable;
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

                        LightStartForm.Activate();
                        this.Hide();
                    }

                }

                return;
            }


            if (FormEvent == eShow)
            {
                if (this.Opacity != 1)
                    this.Opacity += 0.05;
                else
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                    PicturesAlbums.ItemsDataTable = InfiniumPictures.AlbumsItemsDataTable;
                }

                return;
            }
        }

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            if (bShow)
            {
                bool OK = LightMessageBox.Show(ref TopForm, true,
                       "Для возврата к альбомам нажмите кнопку \"Назад\" слева вверху, или нажмите OK чтобы закрыть модуль", "");

                if (!OK)
                    return;
            }

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
            InfiniumPictures = new InfiniumPictures();
            InfiniumPictures.FillAlbums();
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


        private void ProjectsForm_ANSUpdate(object sender)
        {

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

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void PicturesAlbums_ItemClicked(object sender, int AlbumID)
        {
            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(PicturesContainer.Top, PicturesContainer.Left,
                                               PicturesContainer.Height, PicturesContainer.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            bShow = true;

            InfiniumPictures.FillPictures(AlbumID);
            PicturesContainer.ItemsDataTable = InfiniumPictures.PicturesItemsDataTable;
            PicturesContainer.BringToFront();
            PicturesContainer.Focus();

            iAlbumID = AlbumID;

            BackButton.Visible = true;

            bC = true;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(PicturesContainer.Top, PicturesContainer.Left,
                                               PicturesContainer.Height, PicturesContainer.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            bShow = false;

            PicturesAlbums.BringToFront();
            PicturesAlbums.Focus();

            PicturesAlbums.AlbumItems[PicturesAlbums.ItemsDT.Rows.IndexOf(PicturesAlbums.ItemsDT.Select("AlbumID = " + iAlbumID)[0])].LikesCount =
                                InfiniumPictures.RefreshLikes(iAlbumID);

            BackButton.Visible = false;

            bC = true;
        }

        private void PicturesContainer_ItemClicked(object sender, int AlbumID, int PictureID)
        {
            PhantomForm PhantomForm = new PhantomForm()
            {
                BackColor = Color.FromArgb(40, 40, 40),
                Opacity = 0.8f
            };
            PhantomForm.Show();

            PictureViewForm PictureViewForm = new PictureViewForm(ref InfiniumPictures, ref PicturesContainer, AlbumID, PictureID, ref TopForm);

            TopForm = PictureViewForm;

            PictureViewForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            PictureViewForm.Dispose();

            GC.Collect();
        }

        private void PicturesContainer_LikeClicked(object sender, int PictureID)
        {
            int Count = InfiniumPictures.LikePicture(PictureID);
            PicturesContainer.RefreshItem(PictureID, Count);
        }

    }
}