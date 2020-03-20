using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MarketingNewsForm : InfiniumForm
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm;

        DataTable SDT = new DataTable();

        MarketingNews MarketingNews;

        bool bNeedSplash = false;

        public MarketingNewsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            MarketingNews = new Infinium.MarketingNews();
            MarketingNews.FillLikes();

            Initialize();

            ClientsManagersMenu.ClientsManagersDataTable = MarketingNews.ClientsManagersDataTable;
            ClientsManagersMenu.InitializeItems();
            ClientsManagersMenu.Selected = 0;

            MarketingNews.FillMNews(ClientsManagersMenu.Items[ClientsManagersMenu.Selected].ClientID);
            MarketingNews.FillNewManagersNews();

            NewClientsManagersMenu.ClientsManagersDataTable = MarketingNews.ClientsManagersNewsDataTable;
            NewClientsManagersMenu.InitializeItems();

            ClientsMenu.ClientsDataTable = MarketingNews.ClientsDataTable;
            ClientsMenu.InitializeItems();
            ClientsMenu.Selected = 0;

            MarketingNews.FillNews(ClientsMenu.Items[ClientsMenu.Selected].ClientID);
            MarketingNews.FillNewClientsNews();

            NewClientsMenu.ClientsDataTable = MarketingNews.ClientsNewsDataTable;
            NewClientsMenu.InitializeItems();

            if (MarketingNews.ClientsNewsDataTable.Rows.Count == 0)
            {
                ClientsMenu.Top = 0;
                ClientsMenu.Height = panel1.Height;
                if (MarketingNews.ClientsManagersNewsDataTable.Rows.Count == 0)
                {
                    ClientsManagersMenu.Top = 0;
                    ClientsManagersMenu.Height = panel4.Height;
                    return;
                }
                else
                {
                    NewClientsManagersMenu.Selected = 0;
                    cbtnManagers.Checked = true;
                    ClientsManagersMenu.Top = 239;
                    ClientsManagersMenu.Height = panel4.Height - ClientsManagersMenu.Top;
                    ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);
                }
                return;
            }
            else
            {
                ClientsMenu.Top = 239;
                ClientsMenu.Height = panel1.Height - ClientsMenu.Top;
                NewClientsMenu.Selected = 0;
                ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);
            }


            while (!SplashForm.bCreated) ;
        }


        private void LightNewsForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            LightNewsContainer.Visible = true;
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
                    LightNewsContainer.CreateNews();
                    LightNewsContainer.Focus();
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                    bNeedSplash = true;
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


            if (FormEvent == eShow)
            {
                if (this.Opacity != 1)
                    this.Opacity += 0.05;
                else
                {
                    LightNewsContainer.CreateNews();
                    LightNewsContainer.Focus();
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                    bNeedSplash = true;
                }

                return;
            }
        }

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            LightNewsContainer.Visible = false;

            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void Initialize()
        {
            LightNewsContainer.ManagersDT = MarketingNews.ClientsDataTable;
            LightNewsContainer.ClientsManagersDT = MarketingNews.ClientsManagersDataTable;
            LightNewsContainer.CurrentUserID = Security.CurrentUserID;
            LightNewsContainer.NewsDataTable = MarketingNews.NewsDataTable.Copy();
            LightNewsContainer.UsersDataTable = MarketingNews.UsersDataTable.Copy();
            LightNewsContainer.AttachsDT = MarketingNews.AttachmentsDataTable.Copy();
            LightNewsContainer.CommentsDT = MarketingNews.CommentsDataTable.Copy();
            LightNewsContainer.NewsLikesDT = MarketingNews.NewsLikesDataTable.Copy();
            LightNewsContainer.CommentsLikesDT = MarketingNews.CommentsLikesDataTable.Copy();
            LightNewsContainer.CreateNews();
            LightNewsContainer.Visible = true;
            LightNewsContainer.Focus();
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

        private void LightNewsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //LightNews.ClearCommentsSubs(Security.CurrentUserID);
        }

        private void LightNewsForm_ANSUpdate(object sender)
        {
            int c = MarketingNews.GetNewsUpdatesCount();

            if (c > 0)
            {
                UpdateNewsButton.Text = "Обновления: " + c.ToString();
                UpdateNewsButton.Visible = true;
            }

            //ActiveNotifySystem.ClearNewUpdates("LightNewsButton");
        }

        bool bC = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bC)
            {
                while (SplashWindow.bSmallCreated)
                    CoverWaitForm.CloseS = true;

                bC = false;
            }
        }

        private void AddNewsButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            AddMarketingNewsForm AddNewsForm = new AddMarketingNewsForm(ref MarketingNews, ref TopForm);

            TopForm = AddNewsForm;

            AddNewsForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (AddNewsForm.Canceled)
                return;

            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(LightNewsContainer.Top, LightNewsContainer.Left,
                                     LightNewsContainer.Height, LightNewsContainer.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            if (cbtnClients.Checked)
            {
                if (ClientsMenu.Selected == -1)
                    MarketingNews.ReloadClientsNews(20, NewClientsMenu.Items[NewClientsMenu.Selected].ClientID);
                else
                    MarketingNews.ReloadClientsNews(20, ClientsMenu.Items[ClientsMenu.Selected].ClientID);
            }
            if (cbtnManagers.Checked)
            {
                if (ClientsManagersMenu.Selected == -1)
                    MarketingNews.ReloadManagersNews(20, NewClientsManagersMenu.Items[NewClientsManagersMenu.Selected].ClientID);
                else
                    MarketingNews.ReloadManagersNews(20, ClientsManagersMenu.Items[ClientsManagersMenu.Selected].ClientID);
            }
            MarketingNews.ReloadComments();
            MarketingNews.ReloadAttachments();
            LightNewsContainer.NewsDataTable = MarketingNews.NewsDataTable.Copy();
            LightNewsContainer.CommentsDT = MarketingNews.CommentsDataTable.Copy();
            LightNewsContainer.AttachsDT = MarketingNews.AttachmentsDataTable.Copy();
            LightNewsContainer.CreateNews();
            LightNewsContainer.ScrollToTop();
            LightNewsContainer.Focus();

            bC = true;

        }

        private void LightNewsContainer_RemoveNewsClicked(object sender, int NewsID)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            LightMessageBoxForm LightMessageBoxForm = new Infinium.LightMessageBoxForm(true, "Сообщение будет удалено безвозвратно.\nПродолжить?",
                                                                                    "Удаление сообщения");

            TopForm = LightMessageBoxForm;

            LightMessageBoxForm.ShowDialog();

            TopForm = null;

            PhantomForm.Close();
            PhantomForm.Dispose();


            if (LightMessageBoxForm.OKCancel)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(LightNewsContainer.Top, LightNewsContainer.Left,
                    LightNewsContainer.Height, LightNewsContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;


                MarketingNews.RemoveNews(NewsID);
                if (cbtnClients.Checked)
                {
                    if (ClientsMenu.Selected == -1)
                    {
                        MarketingNews.ReloadClientsNews(LightNewsContainer.NewsCount, NewClientsMenu.Items[NewClientsMenu.Selected].ClientID);
                    }
                    else
                    {
                        MarketingNews.ReloadClientsNews(LightNewsContainer.NewsCount, ClientsMenu.Items[ClientsMenu.Selected].ClientID);
                    }
                }

                if (cbtnManagers.Checked)
                {
                    if (ClientsManagersMenu.Selected == -1)
                    {
                        MarketingNews.ReloadManagersNews(LightNewsContainer.NewsCount, NewClientsManagersMenu.Items[NewClientsManagersMenu.Selected].ClientID);
                    }
                    else
                    {
                        MarketingNews.ReloadManagersNews(LightNewsContainer.NewsCount, ClientsManagersMenu.Items[ClientsManagersMenu.Selected].ClientID);
                    }
                }

                MarketingNews.ReloadComments();
                LightNewsContainer.NewsDataTable = MarketingNews.NewsDataTable.Copy();
                LightNewsContainer.CommentsDT = MarketingNews.CommentsDataTable.Copy();
                LightNewsContainer.CreateNews();
            }

            LightMessageBoxForm.Dispose();


            bC = true;

        }

        private void LightNewsContainer_CommentSendButtonClicked(object sender, string Text, int NewsID, int SenderID, bool bEdit, int NewsCommentID)
        {
            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(LightNewsContainer.Top, LightNewsContainer.Left,
                                     LightNewsContainer.Height, LightNewsContainer.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            if (bEdit)
                MarketingNews.EditComments(SenderID, NewsCommentID, Text);
            else
                MarketingNews.AddComments(SenderID, 0, NewsID, Text);
            if (cbtnClients.Checked)
            {
                if (ClientsMenu.Selected == -1)
                    MarketingNews.ReloadClientsNews(LightNewsContainer.NewsCount, NewClientsMenu.Items[NewClientsMenu.Selected].ClientID);
                else
                    MarketingNews.ReloadClientsNews(LightNewsContainer.NewsCount, ClientsMenu.Items[ClientsMenu.Selected].ClientID);
            }
            if (cbtnManagers.Checked)
            {
                if (ClientsManagersMenu.Selected == -1)
                    MarketingNews.ReloadManagersNews(LightNewsContainer.NewsCount, NewClientsManagersMenu.Items[NewClientsManagersMenu.Selected].ClientID);
                else
                    MarketingNews.ReloadManagersNews(LightNewsContainer.NewsCount, ClientsManagersMenu.Items[ClientsManagersMenu.Selected].ClientID);
            }
            MarketingNews.ReloadComments();
            LightNewsContainer.NewsDataTable = MarketingNews.NewsDataTable.Copy();
            LightNewsContainer.CommentsDT = MarketingNews.CommentsDataTable.Copy();
            LightNewsContainer.CreateNews();
            LightNewsContainer.ScrollToTop();
            LightNewsContainer.ReloadNewsItem(NewsID, true);

            bC = true;
        }

        private void LightNewsContainer_RemoveCommentClicked(object sender, int NewsID, int NewsCommentID)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            LightMessageBoxForm LightMessageBoxForm = new Infinium.LightMessageBoxForm(true, "Комментарий будет удален.\nПродолжить?",
                                                                                    "Удаление комментария");

            TopForm = LightMessageBoxForm;

            LightMessageBoxForm.ShowDialog();

            TopForm = null;

            PhantomForm.Close();
            PhantomForm.Dispose();


            if (LightMessageBoxForm.OKCancel)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(LightNewsContainer.Top, LightNewsContainer.Left,
                    LightNewsContainer.Height, LightNewsContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                MarketingNews.RemoveComment(NewsCommentID);

                if (cbtnClients.Checked)
                {
                    if (ClientsMenu.Selected == -1)
                        MarketingNews.ReloadClientsNews(LightNewsContainer.NewsCount, NewClientsMenu.Items[NewClientsMenu.Selected].ClientID);
                    else
                        MarketingNews.ReloadClientsNews(LightNewsContainer.NewsCount, ClientsMenu.Items[ClientsMenu.Selected].ClientID);
                }
                if (cbtnManagers.Checked)
                {
                    if (ClientsManagersMenu.Selected == -1)
                        MarketingNews.ReloadManagersNews(LightNewsContainer.NewsCount, NewClientsManagersMenu.Items[NewClientsManagersMenu.Selected].ClientID);
                    else
                        MarketingNews.ReloadManagersNews(LightNewsContainer.NewsCount, ClientsManagersMenu.Items[ClientsManagersMenu.Selected].ClientID);
                }
                MarketingNews.ReloadComments();
                LightNewsContainer.NewsDataTable = MarketingNews.NewsDataTable.Copy();
                LightNewsContainer.CommentsDT = MarketingNews.CommentsDataTable.Copy();
                LightNewsContainer.CreateNews();
                LightNewsContainer.ReloadNewsItem(NewsID, true);

                bC = true;
            }
        }

        private void LightNewsContainer_EditNewsClicked(object sender, int NewsID)
        {
            //PhantomForm PhantomForm = new PhantomForm();
            //PhantomForm.Show();

            //AddMarketingNewsForm AddNewsForm = new AddMarketingNewsForm(ref MarketingNews, MarketingNews.GetThisNewsSenderTypeID(NewsID),
            //                                          MarketingNews.GetThisNewsHeaderText(NewsID),
            //                                          MarketingNews.GetThisNewsBodyText(NewsID),
            //                                          NewsID, MarketingNews.GetThisNewsDateTime(NewsID), ref TopForm);

            //TopForm = AddNewsForm;

            //AddNewsForm.ShowDialog();

            //PhantomForm.Close();
            //PhantomForm.Dispose();

            //TopForm = null;

            //if (AddNewsForm.Canceled)
            //    return;

            //Thread T = new Thread(delegate() { SplashWindow.CreateCoverSplash(LightNewsContainer.Top, LightNewsContainer.Left,
            //                         LightNewsContainer.Height, LightNewsContainer.Width); });
            //T.Start();

            //while (!SplashWindow.bSmallCreated) ;


            //MarketingNews.ReloadNews(LightNewsContainer.NewsCount);
            //MarketingNews.ReloadComments();
            //MarketingNews.ReloadAttachments();
            //LightNewsContainer.NewsDataTable = MarketingNews.NewsDataTable.Copy();
            //LightNewsContainer.CommentsDT = MarketingNews.CommentsDataTable.Copy();
            //LightNewsContainer.AttachsDT = MarketingNews.AttachmentsDataTable.Copy();
            //LightNewsContainer.CreateNews();

            //bC = true;
        }

        private void LightNewsContainer_Refreshed(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(100);

            while (SplashWindow.bSmallCreated)
                CoverWaitForm.CloseS = true;
        }

        private void LightNewsContainer_AttachClicked(object sender, int NewsAttachID)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            MarketingNewsAttachDownloadForm AttachDownloadForm = new MarketingNewsAttachDownloadForm(NewsAttachID, ref MarketingNews.FM, ref MarketingNews);

            TopForm = AttachDownloadForm;

            AttachDownloadForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();

            TopForm = null;

            AttachDownloadForm.Dispose();
        }

        private void LightNewsContainer_EditCommentClicked(object sender, int NewsID, int NewsCommentID)
        {

        }

        private void LightNewsContainer_NeedMoreNews(object sender, EventArgs e)
        {
            int ClientID = -1;
            if (cbtnClients.Checked)
            {
                if (NewClientsMenu.Selected == -1)
                    ClientID = ClientsMenu.Items[ClientsMenu.Selected].ClientID;
                else
                    ClientID = NewClientsMenu.Items[NewClientsMenu.Selected].ClientID;
            }
            if (cbtnManagers.Checked)
            {
                if (NewClientsManagersMenu.Selected == -1)
                    ClientID = ClientsManagersMenu.Items[ClientsManagersMenu.Selected].ClientID;
                else
                    ClientID = NewClientsManagersMenu.Items[NewClientsManagersMenu.Selected].ClientID;
            }
            if (MarketingNews.IsMoreNews(LightNewsContainer.NewsCount, ClientID))
                MoreNewsButton.Visible = true;
            else
                MoreNewsButton.Visible = false;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int O = LightNewsContainer.Offset;
            int SC = LightNewsContainer.ScrollContainer.Height;
            int H = LightNewsContainer.Height;
        }

        private void LightNewsContainer_VerticalScrollBar_ScrollPositionChanged(object sender, int tOffset)
        {

        }

        private void LightNewsContainer_NoNeedMoreNews(object sender, EventArgs e)
        {
            if (MoreNewsButton.Visible)
                MoreNewsButton.Visible = false;
        }

        private void MoreNewsButton_Click(object sender, EventArgs e)
        {
            int iNewsCount = LightNewsContainer.NewsCount;

            int ClientID = -1;
            if (cbtnClients.Checked)
            {
                if (NewClientsMenu.Selected == -1)
                    ClientID = ClientsMenu.Items[ClientsMenu.Selected].ClientID;
                else
                    ClientID = NewClientsMenu.Items[NewClientsMenu.Selected].ClientID;
            }
            if (cbtnManagers.Checked)
            {
                if (NewClientsManagersMenu.Selected == -1)
                    ClientID = ClientsManagersMenu.Items[ClientsManagersMenu.Selected].ClientID;
                else
                    ClientID = NewClientsManagersMenu.Items[NewClientsManagersMenu.Selected].ClientID;
            }
            MarketingNews.FillMoreNews(LightNewsContainer.NewsCount + 20, ClientID);
            LightNewsContainer.NewsDataTable = MarketingNews.NewsDataTable.Copy();
            LightNewsContainer.CreateNews();
            LightNewsContainer.ScrollToNews(iNewsCount);
            LightNewsContainer.Focus();
            MoreNewsButton.Visible = false;
            LightNewsContainer.bNeedMoreNews = false;
        }


        private void LightNewsContainer_CommentsClicked(object sender, EventArgs e)
        {
            //this.Refresh();
        }

        private void LightNewsContainer_LikeClicked(object sender, int NewsID)
        {
            MarketingNews.LikeNews(Security.CurrentUserID, NewsID);

            MarketingNews.FillLikes();
            LightNewsContainer.NewsLikesDT = MarketingNews.NewsLikesDataTable;
            LightNewsContainer.ReloadLikes(NewsID);
            LightNewsContainer.Focus();
        }

        private void LightNewsContainer_CommentLikeClicked(object sender, int NewsID, int NewsCommentID)
        {
            MarketingNews.LikeComments(Security.CurrentUserID, NewsID, NewsCommentID);

            MarketingNews.FillLikes();
            LightNewsContainer.CommentsLikesDT = MarketingNews.CommentsLikesDataTable;
            LightNewsContainer.ReloadLikes(NewsID);
            LightNewsContainer.Focus();
        }


        private void LightNewsContainer_CommentsQuoteLabelClicked(object sender)
        {
            InfiniumTips.ShowTip(this, 50, 85, "Комментарий скопирован в буфер обмена...", 1700);
        }

        private void LightNewsContainer_NewsQuoteLabelClicked(object sender)
        {
            InfiniumTips.ShowTip(this, 50, 85, "Текст скопирован в буфер обмена...", 1700);
        }

        private void UpdateNewsButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(LightNewsContainer.Top, LightNewsContainer.Left,
                                     LightNewsContainer.Height, LightNewsContainer.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            if (cbtnClients.Checked)
            {
                MarketingNews.FillNewClientsNews();

                NewClientsMenu.ClientsDataTable = MarketingNews.ClientsNewsDataTable;
                NewClientsMenu.InitializeItems();
                if (MarketingNews.ClientsNewsDataTable.Rows.Count == 0)
                {
                    ClientsMenu.Top = 0;
                    ClientsMenu.Height = panel1.Height;

                    MarketingNews.FillNewManagersNews();

                    NewClientsManagersMenu.ClientsManagersDataTable = MarketingNews.ClientsManagersNewsDataTable;
                    NewClientsManagersMenu.InitializeItems();
                    if (MarketingNews.ClientsManagersNewsDataTable.Rows.Count == 0)
                    {
                        ClientsManagersMenu.Top = 0;
                        ClientsManagersMenu.Height = panel1.Height;
                    }
                    else
                    {
                        cbtnManagers.Checked = true;
                        ClientsManagersMenu.Top = 239;
                        ClientsManagersMenu.Height = panel1.Height - ClientsManagersMenu.Top;
                        NewClientsManagersMenu.Selected = 0;
                        ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);
                    }
                }
                else
                {
                    ClientsMenu.Top = 239;
                    ClientsMenu.Height = panel1.Height - ClientsMenu.Top;
                    NewClientsMenu.Selected = 0;
                    ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);
                }
            }
            if (cbtnManagers.Checked)
            {
                MarketingNews.FillNewManagersNews();

                NewClientsManagersMenu.ClientsManagersDataTable = MarketingNews.ClientsManagersNewsDataTable;
                NewClientsManagersMenu.InitializeItems();
                if (MarketingNews.ClientsManagersNewsDataTable.Rows.Count == 0)
                {
                    ClientsManagersMenu.Top = 0;
                    ClientsManagersMenu.Height = panel1.Height;

                    MarketingNews.FillNewClientsNews();

                    NewClientsMenu.ClientsDataTable = MarketingNews.ClientsNewsDataTable;
                    NewClientsMenu.InitializeItems();
                    if (MarketingNews.ClientsNewsDataTable.Rows.Count == 0)
                    {
                        ClientsMenu.Top = 0;
                        ClientsMenu.Height = panel1.Height;
                    }
                    else
                    {
                        cbtnClients.Checked = true;
                        ClientsMenu.Top = 239;
                        ClientsMenu.Height = panel1.Height - ClientsMenu.Top;
                        NewClientsMenu.Selected = 0;
                        ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);
                    }
                }
                else
                {
                    ClientsManagersMenu.Top = 239;
                    ClientsManagersMenu.Height = panel1.Height - ClientsManagersMenu.Top;
                    NewClientsManagersMenu.Selected = 0;
                    ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);
                }
            }

            UpdateNewsButton.Visible = false;

            bC = true;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            LightNewsContainer.Visible = false;

            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void ClientsMenu_ItemClicked(object sender, string ClientName, int ClientID)
        {
            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(LightNewsContainer.Top, LightNewsContainer.Left,
                                         LightNewsContainer.Height, LightNewsContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            if (NewClientsMenu.Selected > -1)
                NewClientsMenu.Selected = -1;

            MarketingNews.ReloadClientsNews(20, ClientID);//default
            MarketingNews.ReloadComments();
            MarketingNews.ReloadAttachments();
            LightNewsContainer.NewsDataTable = MarketingNews.NewsDataTable.Copy();
            LightNewsContainer.CommentsDT = MarketingNews.CommentsDataTable.Copy();
            LightNewsContainer.AttachsDT = MarketingNews.AttachmentsDataTable.Copy();
            LightNewsContainer.CreateNews();
            LightNewsContainer.ScrollToTop();
            LightNewsContainer.Focus();

            if (bNeedSplash)
                bC = true;
        }

        private void NewClientsMenu_ItemClicked(object sender, string ClientName, int ClientID)
        {
            ClientsMenu.Selected = -1;

            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(LightNewsContainer.Top, LightNewsContainer.Left,
                                         LightNewsContainer.Height, LightNewsContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            MarketingNews.ReloadClientsNews(20, ClientID);//default
            MarketingNews.ReloadComments();
            MarketingNews.ReloadAttachments();
            LightNewsContainer.NewsDataTable = MarketingNews.NewsDataTable.Copy();
            LightNewsContainer.CommentsDT = MarketingNews.CommentsDataTable.Copy();
            LightNewsContainer.AttachsDT = MarketingNews.AttachmentsDataTable.Copy();
            LightNewsContainer.CreateNews();
            LightNewsContainer.ScrollToTop();
            LightNewsContainer.Focus();


            if (bNeedSplash)
                bC = true;
        }

        private void ClientsManagersMenu_ItemClicked(object sender, string Name, int ManagerID)
        {
            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(LightNewsContainer.Top, LightNewsContainer.Left,
                                         LightNewsContainer.Height, LightNewsContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            if (NewClientsManagersMenu.Selected > -1)
                NewClientsManagersMenu.Selected = -1;

            MarketingNews.ReloadManagersNews(20, ManagerID);//default
            MarketingNews.ReloadComments();
            MarketingNews.ReloadAttachments();
            LightNewsContainer.NewsDataTable = MarketingNews.NewsDataTable.Copy();
            LightNewsContainer.CommentsDT = MarketingNews.CommentsDataTable.Copy();
            LightNewsContainer.AttachsDT = MarketingNews.AttachmentsDataTable.Copy();
            LightNewsContainer.CreateNews();
            LightNewsContainer.ScrollToTop();
            LightNewsContainer.Focus();

            if (bNeedSplash)
                bC = true;
        }

        private void NewClientsManagersMenu_ItemClicked(object sender, string Name, int ManagerID)
        {
            ClientsManagersMenu.Selected = -1;

            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(LightNewsContainer.Top, LightNewsContainer.Left,
                                         LightNewsContainer.Height, LightNewsContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            MarketingNews.ReloadManagersNews(20, ManagerID);//default
            MarketingNews.ReloadComments();
            MarketingNews.ReloadAttachments();
            LightNewsContainer.NewsDataTable = MarketingNews.NewsDataTable.Copy();
            LightNewsContainer.CommentsDT = MarketingNews.CommentsDataTable.Copy();
            LightNewsContainer.AttachsDT = MarketingNews.AttachmentsDataTable.Copy();
            LightNewsContainer.CreateNews();
            LightNewsContainer.ScrollToTop();
            LightNewsContainer.Focus();


            if (bNeedSplash)
                bC = true;
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (MarketingNews == null)
                return;
            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(LightNewsContainer.Top, LightNewsContainer.Left,
                                     LightNewsContainer.Height, LightNewsContainer.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            MarketingNews.FillLikes();

            if (cbtnClients.Checked)
            {
                ClientsMenu.ClientsDataTable = MarketingNews.ClientsDataTable;
                ClientsMenu.InitializeItems();
                ClientsMenu.Selected = 0;

                MarketingNews.FillNews(ClientsMenu.Items[ClientsMenu.Selected].ClientID);
                MarketingNews.FillNewClientsNews();

                NewClientsMenu.ClientsDataTable = MarketingNews.ClientsNewsDataTable;
                NewClientsMenu.InitializeItems();


                if (ClientsMenu.Selected == -1)
                    MarketingNews.ReloadClientsNews(20, NewClientsMenu.Items[NewClientsMenu.Selected].ClientID);
                else
                    MarketingNews.ReloadClientsNews(20, ClientsMenu.Items[ClientsMenu.Selected].ClientID);
                if (MarketingNews.ClientsNewsDataTable.Rows.Count == 0)
                {
                    ClientsMenu.Top = 0;
                    ClientsMenu.Height = panel1.Height;
                }
                else
                {
                    ClientsMenu.Top = 239;
                    ClientsMenu.Height = panel1.Height - ClientsMenu.Top;
                    NewClientsMenu.Selected = 0;
                    ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);
                }
                panel1.BringToFront();
            }
            else
            {
                ClientsManagersMenu.ClientsManagersDataTable = MarketingNews.ClientsManagersDataTable;
                ClientsManagersMenu.InitializeItems();
                ClientsManagersMenu.Selected = 0;

                MarketingNews.FillMNews(ClientsManagersMenu.Items[ClientsManagersMenu.Selected].ClientID);
                MarketingNews.FillNewManagersNews();

                NewClientsManagersMenu.ClientsManagersDataTable = MarketingNews.ClientsManagersNewsDataTable;
                NewClientsManagersMenu.InitializeItems();

                if (ClientsManagersMenu.Selected == -1)
                    MarketingNews.ReloadManagersNews(20, NewClientsManagersMenu.Items[NewClientsManagersMenu.Selected].ClientID);
                else
                    MarketingNews.ReloadManagersNews(20, ClientsManagersMenu.Items[ClientsManagersMenu.Selected].ClientID);
                if (MarketingNews.ClientsManagersNewsDataTable.Rows.Count == 0)
                {
                    ClientsManagersMenu.Top = 0;
                    ClientsManagersMenu.Height = panel4.Height;
                }
                else
                {
                    ClientsManagersMenu.Top = 239;
                    ClientsManagersMenu.Height = panel4.Height - ClientsManagersMenu.Top;
                    NewClientsManagersMenu.Selected = 0;
                    ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);
                }
                panel4.BringToFront();
            }

            MarketingNews.ReloadComments();
            MarketingNews.ReloadAttachments();
            LightNewsContainer.NewsDataTable = MarketingNews.NewsDataTable.Copy();
            LightNewsContainer.CommentsDT = MarketingNews.CommentsDataTable.Copy();
            LightNewsContainer.AttachsDT = MarketingNews.AttachmentsDataTable.Copy();
            LightNewsContainer.CreateNews();
            LightNewsContainer.ScrollToTop();
            LightNewsContainer.Focus();
            bC = true;
        }

    }
}
