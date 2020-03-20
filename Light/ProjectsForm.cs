using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ProjectsForm : InfiniumForm
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form TopForm = null;

        InfiniumProjects InfiniumProjects;

        bool bC = false;

        bool bNeedSplash = false;
        bool bNeedNewsSplash = false;

        bool bNewProjectsSelected = false;
        bool bNewMessagesSelected = false;
        bool bProposSelected = false;

        public ProjectsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            InfiniumProjects = new InfiniumProjects();
            InfiniumProjects.Fill();
            InfiniumProjects.FillProjects(infiniumProjectsFilterStates1.Selected, infiniumProjectsFilterGroups1.Selected,
                                          infiniumProjectsFilterGroups1.SelectedUser, infiniumProjectsFilterGroups1.SelectedDepartment);
            InfiniumProjects.FillProjectSubs();
            InfiniumProjects.FillComments();
            InfiniumProjects.FillMembers(infiniumProjectsFilterStates1.Selected);
            infiniumProjectsFilterGroups1.Selected = 0;
            infiniumProjectsList1.ProjectsDataTable = InfiniumProjects.ProjectsDataTable;
            infiniumProjectsFilterGroups1.UsersDataTable = InfiniumProjects.ProjectUsersMembersDataTable;
            infiniumProjectsFilterGroups1.DepartmentsDataTable = InfiniumProjects.ProjectDepsMembersDataTable;

            //LightNewsContainer.PageChanged(null);

            OnANSUpdate();
            CheckNewsAndProjects();

            infiniumProjectsList1.UsersDataTable = InfiniumProjects.UsersDataTable;
            infiniumProjectsList1.Filter();

            while (!SplashForm.bCreated) ;
        }


        public void CheckNewsAndProjects()
        {
            if (InfiniumProjects.ProjectNewsSubsRecordsDataTable.Rows.Count > 0 || InfiniumProjects.ProjectNewsCommentsSubsRecordsDataTable.Rows.Count > 0)
            {
                NewMessagesCountLabel.Text = (InfiniumProjects.ProjectNewsSubsRecordsDataTable.Rows.Count + InfiniumProjects.ProjectNewsCommentsSubsRecordsDataTable.Rows.Count).ToString();
                NewMessagesLabel.ForeColor = Color.FromArgb(60, 60, 60);
                NewMessagesLabel.Tag = "true";
                MessagesActivePicture.BringToFront();
            }
            else
            {
                NewMessagesCountLabel.Text = "";
                NewMessagesLabel.ForeColor = Color.FromArgb(180, 180, 180);
                NewMessagesLabel.Tag = "false";
                MessagesInactivePicture.BringToFront();
            }


            if (InfiniumProjects.ProjectSubsRecordsDataTable.Rows.Count > 0)
            {
                NewProjectsCountLabel.Text = InfiniumProjects.ProjectSubsRecordsDataTable.Rows.Count.ToString();
                NewProjectsLabel.ForeColor = Color.FromArgb(60, 60, 60);
                NewProjectsLabel.Tag = "true";
                NewProjectsActivePicture.BringToFront();
            }
            else
            {
                NewProjectsCountLabel.Text = "";
                NewProjectsLabel.ForeColor = Color.FromArgb(180, 180, 180);
                NewProjectsLabel.Tag = "false";
                NewProjectsInactivePicture.BringToFront();
            }
        }

        public void SelectNewProject(int ProjectID)//after create new project
        {
            NewProjectsCountLabel.Text = "1";
            NewProjectsLabel.ForeColor = Color.FromArgb(60, 60, 60);
            NewProjectsLabel.Tag = "true";
            NewProjectsActivePicture.BringToFront();

            if (bNewMessagesSelected)
                bNewMessagesSelected = false;

            bNewProjectsSelected = true;
            NewProjectsLabel.ForeColor = Color.FromArgb(56, 184, 238);

            if (bNeedSplash)
            {
                bNeedNewsSplash = false;
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }


            InfiniumProjects.FillProject(ProjectID);
            infiniumProjectsList1.Filter();

            infiniumProjectsFilterGroups1.DeselectAll();
            infiniumProjectsFilterStates1.DeselectAll();

            if (bNeedSplash)
                bC = true;

            bNeedNewsSplash = true;
        }

        private void LightNewsForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            NewsContainer.Visible = true;
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

                    InfiniumProjects.ClearAllSubs();//clear news and comments subs

                    ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);

                    return;
                }

                if (FormEvent == eShow)
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                    bNeedSplash = true;
                    bNeedNewsSplash = true;

                    infiniumProjectsList1_SelectedChanged(null, null);

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



                    InfiniumProjects.ClearAllSubs();//clear news and comments subs

                    ActiveNotifySystem.ClearSubscribesRecords(Security.CurrentUserID, this.Name);
                }

                return;
            }


            if (FormEvent == eShow)
            {
                if (this.Opacity != 1)
                    this.Opacity += 0.05;
                else
                {
                    bNeedSplash = true;
                    bNeedNewsSplash = true;

                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;

                    infiniumProjectsList1_SelectedChanged(null, null);
                }

                return;
            }
        }

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            NewsContainer.Visible = false;

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
            int n = InfiniumProjects.GetNewsUpdatesCount();
            int p = InfiniumProjects.GetProjectsUpdatesCount();
            int pr = InfiniumProjects.GetPropositionsUpdatesCount();

            if (n > 0)
            {
                InfiniumProjects.GetNewsUpdates();
                CheckNewsAndProjects();
            }

            if (p > 0)
            {
                InfiniumProjects.GetProjectsUpdates();
                CheckNewsAndProjects();
            }

            if (pr > 0)
            {
                ProposCountLabel.Visible = true;
                ProposCountLabel.Text = pr.ToString();
            }

            // ActiveNotifySystem.ClearNewUpdates("ProjectsButton");
        }



        private void AddProjectButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            AddProjectForm AddProjectForm = new AddProjectForm(ref InfiniumProjects, ref TopForm);

            TopForm = AddProjectForm;

            AddProjectForm.ShowDialog();

            int ProjectID = -1;

            if (!AddProjectForm.Canceled)
                ProjectID = AddProjectForm.ProjectID;

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (AddProjectForm.Canceled)
                return;

            if (bNeedSplash)
            {
                bNeedNewsSplash = false;
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }


            //clear new projects and messages/////////////////////////////////


            if (bNewProjectsSelected)
            {
                NewProjectsCountLabel.Text = "";
                NewProjectsLabel.ForeColor = Color.FromArgb(180, 180, 180);
                NewProjectsLabel.Tag = "false";
                NewProjectsInactivePicture.BringToFront();
                bNewProjectsSelected = false;
            }

            if (bNewMessagesSelected)
            {
                NewMessagesCountLabel.Text = "";
                NewMessagesLabel.ForeColor = Color.FromArgb(180, 180, 180);
                NewMessagesLabel.Tag = "false";
                MessagesInactivePicture.BringToFront();

                bNewMessagesSelected = false;
            }
            //////////////////////////////////////////////////////////////////


            InfiniumProjects.ClearProjectsPending(ProjectID);


            if (bProposSelected)
            {
                InfiniumProjects.FillPropositions();
                infiniumProjectsList1.ProjectsDataTable = InfiniumProjects.ProjectsDataTable;
                infiniumProjectsList1.Filter();
            }
            else
            {
                if (infiniumProjectsFilterGroups1.Selected == -1)
                    infiniumProjectsFilterGroups1.Selected = 0;

                if (infiniumProjectsFilterStates1.Selected == -1)
                    infiniumProjectsFilterStates1.Selected = 0;

                InfiniumProjects.FillProjects(infiniumProjectsFilterStates1.Selected, infiniumProjectsFilterGroups1.Selected,
                                              infiniumProjectsFilterGroups1.SelectedUser, infiniumProjectsFilterGroups1.SelectedDepartment);
                InfiniumProjects.FillMembers(infiniumProjectsFilterStates1.Selected);
                infiniumProjectsFilterGroups1.UsersDataTable = InfiniumProjects.ProjectUsersMembersDataTable;
                infiniumProjectsFilterGroups1.DepartmentsDataTable = InfiniumProjects.ProjectDepsMembersDataTable;
                infiniumProjectsList1.Filter();

                InfiniumProjects.AddSubscribeForNewProject(ProjectID);
                SelectNewProject(ProjectID);
            }



            if (bNeedSplash)
                bC = true;

            bNeedNewsSplash = true;


        }

        private void infiniumProjectsFilterStates1_ItemClicked(object sender, EventArgs e)
        {
            if (bNeedSplash)
            {
                bNeedNewsSplash = false;
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            StatusStartActivePicture.SendToBack();
            StatusCanceledActivePicture.SendToBack();
            StatusPauseActivePicture.SendToBack();
            StatusEndActivePicture.SendToBack();

            StartProjectDateLabel.Text = "нет";
            PauseProjectDateLabel.Text = "нет";
            CancelProjectDateLabel.Text = "нет";
            EndProjectDateLabel.Text = "нет";

            if (infiniumProjectsFilterGroups1.Selected == -1)
                infiniumProjectsFilterGroups1.Selected = 0;

            if (bNewProjectsSelected)
            {
                NewProjectsCountLabel.Text = "";
                NewProjectsLabel.ForeColor = Color.FromArgb(180, 180, 180);
                NewProjectsLabel.Tag = "false";
                NewProjectsInactivePicture.BringToFront();

                bNewProjectsSelected = false;
            }

            if (bNewMessagesSelected)
            {
                NewMessagesCountLabel.Text = "";
                NewMessagesLabel.ForeColor = Color.FromArgb(180, 180, 180);
                NewMessagesLabel.Tag = "false";
                MessagesInactivePicture.BringToFront();

                bNewMessagesSelected = false;
            }

            if (bProposSelected)
            {
                ProposCountLabel.Text = "";
                ProposLabel.ForeColor = Color.FromArgb(60, 60, 60);
                ProposLabel.Tag = "false";
                ProposInactivePicture.BringToFront();

                bProposSelected = false;
            }

            InfiniumProjects.FillProjects(infiniumProjectsFilterStates1.Selected, infiniumProjectsFilterGroups1.Selected,
                                          infiniumProjectsFilterGroups1.SelectedUser, infiniumProjectsFilterGroups1.SelectedDepartment);
            InfiniumProjects.FillMembers(infiniumProjectsFilterStates1.Selected);
            infiniumProjectsFilterGroups1.UsersDataTable = InfiniumProjects.ProjectUsersMembersDataTable;
            infiniumProjectsFilterGroups1.DepartmentsDataTable = InfiniumProjects.ProjectDepsMembersDataTable;
            infiniumProjectsFilterGroups1.Expand();
            infiniumProjectsList1.Filter();

            if (bNeedSplash)
                bC = true;

            bNeedNewsSplash = true;
        }

        private void infiniumProjectsFilterGroups1_ItemClicked(object sender, EventArgs e)
        {

        }

        private void infiniumProjectsFilterGroups1_UserItemClicked(object sender, EventArgs e)
        {

        }

        private void infiniumProjectsFilterGroups1_DepartmentItemClicked(object sender, EventArgs e)
        {

        }

        private void infiniumProjectsFilterGroups1_SelectedChanged(object sender, EventArgs e)
        {
            if (bNeedSplash)
            {
                bNeedNewsSplash = false;
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            if (infiniumProjectsFilterStates1.Selected == -1)
                infiniumProjectsFilterStates1.Selected = 0;



            StatusStartActivePicture.SendToBack();
            StatusCanceledActivePicture.SendToBack();
            StatusPauseActivePicture.SendToBack();
            StatusEndActivePicture.SendToBack();

            StartProjectDateLabel.Text = "нет";
            PauseProjectDateLabel.Text = "нет";
            CancelProjectDateLabel.Text = "нет";
            EndProjectDateLabel.Text = "нет";




            InfiniumProjects.FillProjects(infiniumProjectsFilterStates1.Selected, infiniumProjectsFilterGroups1.Selected,
                                        infiniumProjectsFilterGroups1.SelectedUser, infiniumProjectsFilterGroups1.SelectedDepartment);
            infiniumProjectsList1.Filter();

            if (bNeedSplash)
                bC = true;

            bNeedNewsSplash = true;
        }

        private void infiniumProjectsList1_SelectedChanged(object sender, EventArgs e)
        {
            if (bNeedNewsSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(panel4.Top + UpdatePanel.Top, panel4.Left + UpdatePanel.Left + 1,
                                                   panel4.Height, panel4.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            if (infiniumProjectsList1.Selected != -1)
            {
                panel4.Visible = true;
                ProjectsSplitContainer.Visible = true;

                int iRes = InfiniumProjects.CheckSubscribeToUpdates(128,
                    Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]),
                    Security.CurrentUserID);

                if (iRes == 1)
                {
                    SubscribeItPicture.Visible = true;
                    SubscribeItLabel.Visible = true;
                    SubscribeItLabel.Text = "Отписаться";
                }
                else
                    if (iRes == 0)
                {
                    SubscribeItPicture.Visible = true;
                    SubscribeItLabel.Visible = true;
                    SubscribeItLabel.Text = "Подписаться";
                }
                else
                        if (iRes == -1)
                {
                    SubscribeItPicture.Visible = false;
                    SubscribeItLabel.Visible = false;
                }

                ProjectCaptionLabel.Text = InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectName"].ToString();

                AuthorLabel.Text =
                     InfiniumProjects.GetUserName(Convert.ToInt32(
                           InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["AuthorID"])) + ", " +
                     Convert.ToDateTime(
                         InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["CreationDate"]).ToString("dd MMMM yyyy HH:mm");

                AuthorPhotoBox.Image = InfiniumProjects.GetUserPhoto(
                    Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["AuthorID"]));

                infiniumProjectsDescriptionBox1.DescriptionItem.DescriptionText =
                    InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectDescription"].ToString();

                InfiniumProjects.FillProjectNews(Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]));
                InfiniumProjects.FillComments();
                InfiniumProjects.FillLikes();
                InfiniumProjects.FillAttachments();


                StatusStartActivePicture.SendToBack();
                StatusCanceledActivePicture.SendToBack();
                StatusPauseActivePicture.SendToBack();
                StatusEndActivePicture.SendToBack();

                StartProjectDateLabel.Text = "нет";
                PauseProjectDateLabel.Text = "нет";
                CancelProjectDateLabel.Text = "нет";
                EndProjectDateLabel.Text = "нет";

                StartProjectDateLabel.ForeColor = Color.Silver;
                PauseProjectDateLabel.ForeColor = Color.Silver;
                CancelProjectDateLabel.ForeColor = Color.Silver;
                EndProjectDateLabel.ForeColor = Color.Silver;


                //labels
                if (infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + infiniumProjectsList1.ProjectID)[0]["StartDate"] != DBNull.Value)
                {
                    StatusStartActivePicture.BringToFront();
                    StartProjectDateLabel.Text = Convert.ToDateTime(infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + infiniumProjectsList1.ProjectID)[0]["StartDate"]).ToString("dd MMMM yyyy HH:mm");

                    if (infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + infiniumProjectsList1.ProjectID)[0]["ProjectStatusID"].ToString() == "0")
                        StartProjectDateLabel.ForeColor = Color.FromArgb(31, 158, 0);
                }

                if (infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + infiniumProjectsList1.ProjectID)[0]["SuspendedDate"] != DBNull.Value)
                {
                    StatusPauseActivePicture.BringToFront();
                    PauseProjectDateLabel.Text = Convert.ToDateTime(infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + infiniumProjectsList1.ProjectID)[0]["SuspendedDate"]).ToString("dd MMMM yyyy HH:mm");

                    if (infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + infiniumProjectsList1.ProjectID)[0]["ProjectStatusID"].ToString() == "1")
                        PauseProjectDateLabel.ForeColor = Color.FromArgb(31, 158, 0);
                }

                if (infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + infiniumProjectsList1.ProjectID)[0]["CanceledDate"] != DBNull.Value)
                {
                    StatusCanceledActivePicture.BringToFront();
                    CancelProjectDateLabel.Text = Convert.ToDateTime(infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + infiniumProjectsList1.ProjectID)[0]["CanceledDate"]).ToString("dd MMMM yyyy HH:mm");

                    if (infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + infiniumProjectsList1.ProjectID)[0]["ProjectStatusID"].ToString() == "2")
                        CancelProjectDateLabel.ForeColor = Color.FromArgb(31, 158, 0);
                }

                if (infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + infiniumProjectsList1.ProjectID)[0]["CompletedDate"] != DBNull.Value)
                {
                    StatusEndActivePicture.BringToFront();
                    EndProjectDateLabel.Text = Convert.ToDateTime(infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + infiniumProjectsList1.ProjectID)[0]["CompletedDate"]).ToString("dd MMMM yyyy HH:mm");

                    if (infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + infiniumProjectsList1.ProjectID)[0]["ProjectStatusID"].ToString() == "3")
                        EndProjectDateLabel.ForeColor = Color.FromArgb(31, 158, 0);
                }
                /////////////////////////////

                NewsContainer.Clear();
                NewsContainer.CurrentUserID = Security.CurrentUserID;
                NewsContainer.NewsDataTable = InfiniumProjects.ProjectNewsDataTable.Copy();
                NewsContainer.UsersDataTable = InfiniumProjects.UsersDataTable.Copy();
                NewsContainer.AttachsDT = InfiniumProjects.ProjectNewsAttachmentsDataTable.Copy();
                NewsContainer.CommentsDT = InfiniumProjects.ProjectNewsCommentsDataTable.Copy();
                NewsContainer.NewsLikesDT = InfiniumProjects.ProjectNewsLikesDataTable.Copy();
                NewsContainer.CommentsLikesDT = InfiniumProjects.ProjectNewsCommentsLikesDataTable.Copy();
                NewsContainer.CreateNews();
                //InfiniumProjects.ClearAllSubs(Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]));
                NewsContainer.Visible = true;
                NewsContainer.Focus();

                if (InfiniumProjects.ProjectNewsDataTable.Rows.Count > 0)
                {
                    NoNewsLabel.SendToBack();
                    NoNewsPicture.SendToBack();
                }
                else
                {
                    NoNewsPicture.BringToFront();
                    NoNewsLabel.BringToFront();
                }
            }
            else
            {
                NewsContainer.Clear();
                ProjectCaptionLabel.Text = "";
                AuthorLabel.Text = "";
                AuthorPhotoBox.Image = null;

                panel4.Visible = false;
                ProjectsSplitContainer.Visible = false;

                InfiniumProjects.FillProjectMembers(infiniumProjectsList1.ProjectID);

                StatusStartActivePicture.SendToBack();
                StatusCanceledActivePicture.SendToBack();
                StatusPauseActivePicture.SendToBack();
                StatusEndActivePicture.SendToBack();

                StartProjectDateLabel.Text = "нет";
                PauseProjectDateLabel.Text = "нет";
                CancelProjectDateLabel.Text = "нет";
                EndProjectDateLabel.Text = "нет";

                infiniumProjectsDescriptionBox1.DescriptionItem.DescriptionText = "";
                NoNewsLabel.BringToFront();
                NoNewsPicture.BringToFront();
            }


            InfiniumProjects.FillProjectMembers(infiniumProjectsList1.ProjectID);
            ProjectMembersList.UsersDataTable = InfiniumProjects.CurrentProjectUsersDataTable;
            ProjectMembersList.DepartmentsDataTable = InfiniumProjects.CurrentProjectDepartmentsDataTable;
            ProjectMembersList.CreateItems();



            if (bNeedNewsSplash)
                bC = true;
        }


        private void AddNewsLabel_Click(object sender, EventArgs e)
        {
            if (infiniumProjectsList1.ProjectID == -1)
                return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            AddProjectNewsForm AddProjectNewsForm = new AddProjectNewsForm(ref InfiniumProjects, Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]), ref TopForm);

            TopForm = AddProjectNewsForm;

            AddProjectNewsForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (AddProjectNewsForm.Canceled)
                return;

            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(NewsContainer.Top + panel4.Top + UpdatePanel.Top,
                                               NewsContainer.Left + panel4.Left + UpdatePanel.Left,
                                               NewsContainer.Height, NewsContainer.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;



            InfiniumProjects.ReloadNews(20, Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]));//default
            InfiniumProjects.ReloadComments();
            InfiniumProjects.ReloadAttachments();
            NewsContainer.NewsDataTable = InfiniumProjects.ProjectNewsDataTable.Copy();
            NewsContainer.CommentsDT = InfiniumProjects.ProjectNewsCommentsDataTable.Copy();
            NewsContainer.AttachsDT = InfiniumProjects.ProjectNewsAttachmentsDataTable.Copy();
            NewsContainer.CreateNews();
            NewsContainer.ScrollToTop();
            NewsContainer.Focus();


            if (InfiniumProjects.ProjectNewsDataTable.Rows.Count > 0)
            {
                NoNewsLabel.SendToBack();
                NoNewsPicture.SendToBack();
            }
            else
            {
                NoNewsPicture.BringToFront();
                NoNewsLabel.BringToFront();
            }


            bC = true;
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

        private void NewsContainer_RemoveNewsClicked(object sender, int NewsID)
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
                    SplashWindow.CreateCoverSplash(NewsContainer.Top + panel4.Top + UpdatePanel.Top,
                                               NewsContainer.Left + panel4.Left + UpdatePanel.Left,
                                               NewsContainer.Height, NewsContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;


                InfiniumProjects.RemoveNews(NewsID);
                InfiniumProjects.ReloadNews(NewsContainer.NewsCount, Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]));
                InfiniumProjects.ReloadComments();
                InfiniumProjects.ReloadAttachments();
                NewsContainer.NewsDataTable = InfiniumProjects.ProjectNewsDataTable.Copy();
                NewsContainer.CommentsDT = InfiniumProjects.ProjectNewsCommentsDataTable.Copy();
                NewsContainer.CreateNews();
                NewsContainer.SetNewsPositions();
                NewsContainer.Refresh();
            }

            LightMessageBoxForm.Dispose();

            if (InfiniumProjects.ProjectNewsDataTable.Rows.Count > 0)
            {
                NoNewsLabel.SendToBack();
                NoNewsPicture.SendToBack();
            }
            else
            {
                NoNewsPicture.BringToFront();
                NoNewsLabel.BringToFront();
            }


            bC = true;
        }

        private void NewsContainer_AttachClicked(object sender, int NewsAttachID)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            ProjectAttachDownloadForm ProjectAttachDownloadForm = new ProjectAttachDownloadForm(NewsAttachID, ref InfiniumProjects.FM, ref InfiniumProjects);

            TopForm = ProjectAttachDownloadForm;

            ProjectAttachDownloadForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();

            TopForm = null;

            ProjectAttachDownloadForm.Dispose();
        }

        private void NewsContainer_EditNewsClicked(object sender, int NewsID)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            AddProjectNewsForm AddProjectNewsForm = new AddProjectNewsForm(ref InfiniumProjects,
                             Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]),
                                                      InfiniumProjects.GetThisNewsBodyText(NewsID),
                                                      NewsID, InfiniumProjects.GetThisNewsDateTime(NewsID), ref TopForm);

            TopForm = AddProjectNewsForm;

            AddProjectNewsForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (AddProjectNewsForm.Canceled)
                return;

            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(NewsContainer.Top + panel4.Top + UpdatePanel.Top,
                                               NewsContainer.Left + panel4.Left + UpdatePanel.Left,
                                               NewsContainer.Height, NewsContainer.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;


            InfiniumProjects.ReloadNews(20, Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]));//default
            InfiniumProjects.ReloadComments();
            InfiniumProjects.ReloadAttachments();
            NewsContainer.NewsDataTable = InfiniumProjects.ProjectNewsDataTable.Copy();
            NewsContainer.CommentsDT = InfiniumProjects.ProjectNewsCommentsDataTable.Copy();
            NewsContainer.AttachsDT = InfiniumProjects.ProjectNewsAttachmentsDataTable.Copy();
            NewsContainer.CreateNews();
            NewsContainer.ScrollToTop();
            NewsContainer.Focus();

            bC = true;
        }

        private void NewsContainer_NewsQuoteLabelClicked(object sender)
        {
            InfiniumTips.ShowTip(this, 50, 85, "Текст скопирован в буфер обмена...", 2200);
        }

        private void NewsContainer_CommentsQuoteLabelClicked(object sender)
        {
            InfiniumTips.ShowTip(this, 50, 85, "Комментарий скопирован в буфер обмена...", 2200);
        }

        private void NewsContainer_CommentSendButtonClicked(object sender, string Text, int NewsID, int SenderID, bool bEdit, int NewsCommentID, bool bNoNotify)
        {
            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(NewsContainer.Top + panel4.Top + UpdatePanel.Top,
                                               NewsContainer.Left + panel4.Left + UpdatePanel.Left,
                                               NewsContainer.Height, NewsContainer.Width);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            if (bEdit)
                InfiniumProjects.EditComments(SenderID, NewsCommentID, Text);
            else
                InfiniumProjects.AddComments(SenderID,
                                             Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]),
                                             NewsID, Text, bNoNotify);


            InfiniumProjects.ReloadNews(20, Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]));//default
            InfiniumProjects.ReloadComments();
            NewsContainer.NewsDataTable = InfiniumProjects.ProjectNewsDataTable.Copy();
            NewsContainer.CommentsDT = InfiniumProjects.ProjectNewsCommentsDataTable.Copy();
            NewsContainer.CreateNews();
            NewsContainer.ScrollToTop();
            NewsContainer.ReloadNewsItem(NewsID, true);
            NewsContainer.Refresh();
            NewsContainer.Focus();


            bC = true;
        }

        private void NewsContainer_RemoveCommentClicked(object sender, int NewsID, int NewsCommentID)
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
                    SplashWindow.CreateCoverSplash(NewsContainer.Top + panel4.Top + UpdatePanel.Top,
                                               NewsContainer.Left + panel4.Left + UpdatePanel.Left,
                                               NewsContainer.Height, NewsContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                InfiniumProjects.RemoveComment(NewsCommentID);

                InfiniumProjects.ReloadNews(20, Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]));//default
                InfiniumProjects.ReloadComments();
                NewsContainer.NewsDataTable = InfiniumProjects.ProjectNewsDataTable.Copy();
                NewsContainer.CommentsDT = InfiniumProjects.ProjectNewsCommentsDataTable.Copy();
                NewsContainer.CreateNews();
                NewsContainer.ScrollToTop();
                NewsContainer.ReloadNewsItem(NewsID, true);
                NewsContainer.Refresh();
                NewsContainer.Focus();

                bC = true;
            }
        }

        private void NewsContainer_CommentLikeClicked(object sender, int NewsID, int NewsCommentID)
        {
            InfiniumProjects.LikeComments(Security.CurrentUserID, NewsID, NewsCommentID, infiniumProjectsList1.ProjectID);

            InfiniumProjects.FillLikes();
            NewsContainer.CommentsLikesDT = InfiniumProjects.ProjectNewsCommentsLikesDataTable.Copy();
            NewsContainer.ReloadLikes(NewsID);
            NewsContainer.Focus();
        }

        private void NewsContainer_LikeClicked(object sender, int NewsID)
        {
            InfiniumProjects.LikeNews(Security.CurrentUserID, NewsID, infiniumProjectsList1.ProjectID);

            InfiniumProjects.FillLikes();
            NewsContainer.NewsLikesDT = InfiniumProjects.ProjectNewsLikesDataTable;
            NewsContainer.ReloadLikes(NewsID);
            NewsContainer.Focus();
        }

        private void NewMessagesLabel_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void NewProjectsLabel_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void NewProjectsLabel_MouseLeave(object sender, EventArgs e)
        {

        }

        private void NewMessagesLabel_MouseLeave(object sender, EventArgs e)
        {

        }

        private void NewProjectsLabel_Click(object sender, EventArgs e)
        {
            if (NewProjectsLabel.Tag.ToString() == "false")
                return;

            if (bNewMessagesSelected)
            {
                bNewMessagesSelected = false;
                NewMessagesLabel.ForeColor = Color.FromArgb(60, 60, 60);
            }

            if (bProposSelected)
            {
                bProposSelected = false;
                ProposLabel.ForeColor = Color.FromArgb(60, 60, 60);
                ProposActivePicture.SendToBack();
            }

            bNewProjectsSelected = true;
            NewProjectsLabel.ForeColor = Color.FromArgb(56, 184, 238);


            if (bNeedSplash)
            {
                bNeedNewsSplash = false;
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }


            StatusStartActivePicture.SendToBack();
            StatusCanceledActivePicture.SendToBack();
            StatusPauseActivePicture.SendToBack();
            StatusEndActivePicture.SendToBack();

            StartProjectDateLabel.Text = "нет";
            PauseProjectDateLabel.Text = "нет";
            CancelProjectDateLabel.Text = "нет";
            EndProjectDateLabel.Text = "нет";


            InfiniumProjects.FillProjectsNew(Security.CurrentUserID);
            infiniumProjectsList1.Filter();

            infiniumProjectsFilterGroups1.DeselectAll();
            infiniumProjectsFilterStates1.DeselectAll();

            if (bNeedSplash)
                bC = true;

            bNeedNewsSplash = true;


            NewProjectsCountLabel.Text = "";

        }

        private void NewMessagesLabel_Click(object sender, EventArgs e)
        {
            if (NewMessagesLabel.Tag.ToString() == "false")
                return;

            if (bNewProjectsSelected)
            {
                bNewProjectsSelected = false;
                NewProjectsLabel.ForeColor = Color.FromArgb(60, 60, 60);
            }

            if (bProposSelected)
            {
                bProposSelected = false;
                ProposLabel.ForeColor = Color.FromArgb(60, 60, 60);
                ProposActivePicture.SendToBack();
            }

            bNewMessagesSelected = true;
            NewMessagesLabel.ForeColor = Color.FromArgb(56, 184, 238);

            if (bNeedSplash)
            {
                bNeedNewsSplash = false;
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }


            StatusStartActivePicture.SendToBack();
            StatusCanceledActivePicture.SendToBack();
            StatusPauseActivePicture.SendToBack();
            StatusEndActivePicture.SendToBack();

            StartProjectDateLabel.Text = "нет";
            PauseProjectDateLabel.Text = "нет";
            CancelProjectDateLabel.Text = "нет";
            EndProjectDateLabel.Text = "нет";


            InfiniumProjects.FillProjectsNewsUpdates(Security.CurrentUserID);
            infiniumProjectsList1.Filter();

            infiniumProjectsFilterGroups1.DeselectAll();
            infiniumProjectsFilterStates1.DeselectAll();

            if (bNeedSplash)
                bC = true;

            bNeedNewsSplash = true;

            NewMessagesCountLabel.Text = "";
        }


        private void DeleteProjectLabel_Click(object sender, EventArgs e)
        {
            if (infiniumProjectsList1.ProjectID == -1)
                return;

            if (InfiniumProjects.CanRemove(infiniumProjectsList1.ProjectID) == false)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Только автор проекта может его удалить", 2200);
                return;
            }


            bool OK = LightMessageBox.Show(ref TopForm, true,
                    "Удалить выбранный проект?", "Удаление проекта");

            if (!OK)
                return;


            if (bNeedSplash)
            {
                bNeedNewsSplash = false;
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;



                InfiniumProjects.RemoveProject(infiniumProjectsList1.ProjectID);


                if (infiniumProjectsFilterStates1.Selected == -1 || infiniumProjectsFilterGroups1.Selected == -1)//new projects or messages selected
                {
                    if (bNewMessagesSelected)
                    {
                        InfiniumProjects.FillProjectsNewsUpdates(Security.CurrentUserID);
                    }

                    if (bNewProjectsSelected)
                    {
                        InfiniumProjects.FillProjectsNew(Security.CurrentUserID);
                    }
                }
                else
                    InfiniumProjects.FillProjects(infiniumProjectsFilterStates1.Selected, infiniumProjectsFilterGroups1.Selected,
                                                  infiniumProjectsFilterGroups1.SelectedUser, infiniumProjectsFilterGroups1.SelectedDepartment);

                InfiniumProjects.FillMembers(infiniumProjectsFilterStates1.Selected);
                infiniumProjectsFilterGroups1.UsersDataTable = InfiniumProjects.ProjectUsersMembersDataTable;
                infiniumProjectsFilterGroups1.DepartmentsDataTable = InfiniumProjects.ProjectDepsMembersDataTable;
                infiniumProjectsList1.Filter();

                if (bNeedSplash)
                    bC = true;

                bNeedNewsSplash = true;


                InfiniumTips.ShowTip(this, 50, 85, "Проект удален", 2200);
            }
        }

        private void NewsContainer_EditCommentClicked(object sender, int NewsID, int NewsCommentID)
        {
            NewsContainer.Refresh();
        }


        private void EditProjectLabel_Click(object sender, EventArgs e)
        {
            if (infiniumProjectsList1.ProjectID == -1)
                return;

            if (InfiniumProjects.CanRemove(infiniumProjectsList1.ProjectID) == false)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Только автор проекта может его редактировать", 3000);
                return;
            }

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            AddProjectForm AddProjectForm = new AddProjectForm(ref InfiniumProjects, ref TopForm, infiniumProjectsList1.ProjectID);

            TopForm = AddProjectForm;

            AddProjectForm.ProjectID = infiniumProjectsList1.ProjectID;

            AddProjectForm.ShowDialog();

            int ProjectID = -1;

            if (!AddProjectForm.Canceled)
                ProjectID = AddProjectForm.ProjectID;

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (AddProjectForm.Canceled)
                return;


            if (bNeedSplash)
            {
                bNeedNewsSplash = false;
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            int iProjectID = infiniumProjectsList1.ProjectID;

            //clear new projects and messages/////////////////////////////////
            if (infiniumProjectsFilterGroups1.Selected == -1)
                infiniumProjectsFilterGroups1.Selected = 0;

            if (infiniumProjectsFilterStates1.Selected == -1)
                infiniumProjectsFilterStates1.Selected = 0;

            if (bNewProjectsSelected)
            {
                NewProjectsCountLabel.Text = "";
                NewProjectsLabel.ForeColor = Color.FromArgb(180, 180, 180);
                NewProjectsLabel.Tag = "false";
                NewProjectsInactivePicture.BringToFront();
                bNewProjectsSelected = false;
            }

            if (bNewMessagesSelected)
            {
                NewMessagesCountLabel.Text = "";
                NewMessagesLabel.ForeColor = Color.FromArgb(180, 180, 180);
                NewMessagesLabel.Tag = "false";
                MessagesInactivePicture.BringToFront();

                bNewMessagesSelected = false;
            }
            //////////////////////////////////////////////////////////////////

            InfiniumProjects.ClearProjectsPending(ProjectID);

            if (bProposSelected)
            {
                InfiniumProjects.FillPropositions();
                infiniumProjectsList1.ProjectsDataTable = InfiniumProjects.ProjectsDataTable;
                infiniumProjectsList1.Filter();
            }
            else
            {
                InfiniumProjects.FillProjects(infiniumProjectsFilterStates1.Selected, infiniumProjectsFilterGroups1.Selected,
                                        infiniumProjectsFilterGroups1.SelectedUser, infiniumProjectsFilterGroups1.SelectedDepartment);
                InfiniumProjects.FillMembers(infiniumProjectsFilterStates1.Selected);
                infiniumProjectsFilterGroups1.UsersDataTable = InfiniumProjects.ProjectUsersMembersDataTable;
                infiniumProjectsFilterGroups1.DepartmentsDataTable = InfiniumProjects.ProjectDepsMembersDataTable;
                infiniumProjectsList1.Filter();
            }

            infiniumProjectsList1.Selected = infiniumProjectsList1.ProjectsDataTable.Rows.IndexOf(
                                infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + iProjectID)[0]);

            if (bNeedSplash)
                bC = true;

            bNeedNewsSplash = true;
        }

        private void StatusStartActivePicture_Click(object sender, EventArgs e)
        {
            if (InfiniumProjects.GetProjectStatus(infiniumProjectsList1.ProjectID) == 0)
                return;

            if (InfiniumProjects.IsProposition(infiniumProjectsList1.ProjectID) == true)
            {
                InfiniumProjects.StartProject(infiniumProjectsList1.ProjectID, true);
            }

            if (InfiniumProjects.CanChangeStatusProject(infiniumProjectsList1.ProjectID) == false)//only members and author can edit projects
            {
                InfiniumTips.ShowTip(this, 50, 85, "Только автор и участники могут редактировать проект", 2800);
                return;
            }


            bool OK = LightMessageBox.Show(ref TopForm, true,
                    "Возобновить выбранный проект?", "Удаление проекта");

            if (!OK)
                return;


            if (bNeedSplash)
            {
                bNeedNewsSplash = false;
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;


                int iProjectID = infiniumProjectsList1.ProjectID;

                InfiniumProjects.StartProject(infiniumProjectsList1.ProjectID);


                if (infiniumProjectsFilterStates1.Selected == -1 || infiniumProjectsFilterGroups1.Selected == -1)//new projects or messages selected
                {
                    if (bNewMessagesSelected)
                    {
                        InfiniumProjects.FillProjectsNewsUpdates(Security.CurrentUserID);
                    }

                    if (bNewProjectsSelected)
                    {
                        InfiniumProjects.FillProjectsNew(Security.CurrentUserID);
                    }
                }
                else
                    InfiniumProjects.FillProjects(infiniumProjectsFilterStates1.Selected, infiniumProjectsFilterGroups1.Selected,
                                                  infiniumProjectsFilterGroups1.SelectedUser, infiniumProjectsFilterGroups1.SelectedDepartment);

                InfiniumProjects.FillMembers(infiniumProjectsFilterStates1.Selected);
                infiniumProjectsFilterGroups1.UsersDataTable = InfiniumProjects.ProjectUsersMembersDataTable;
                infiniumProjectsFilterGroups1.DepartmentsDataTable = InfiniumProjects.ProjectDepsMembersDataTable;
                infiniumProjectsList1.Filter();

                //infiniumProjectsList1.Selected = infiniumProjectsList1.ProjectsDataTable.Rows.IndexOf(
                //                infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + iProjectID)[0]);

                if (bNeedSplash)
                    bC = true;

                bNeedNewsSplash = true;
            }
        }

        private void StatusPauseActivePicture_Click(object sender, EventArgs e)
        {
            if (InfiniumProjects.GetProjectStatus(infiniumProjectsList1.ProjectID) == 1)
                return;

            if (InfiniumProjects.IsProposition(infiniumProjectsList1.ProjectID) == true)
                return;

            if (InfiniumProjects.CanChangeStatusProject(infiniumProjectsList1.ProjectID) == false)//only members and author can edit projects
            {
                InfiniumTips.ShowTip(this, 50, 85, "Только автор и участники могут редактировать проект", 2800);
                return;
            }

            bool OK = LightMessageBox.Show(ref TopForm, true,
                    "Приостановить выбранный проект?", "Удаление проекта");

            if (!OK)
                return;

            if (bNeedSplash)
            {
                bNeedNewsSplash = false;
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;


                int iProjectID = infiniumProjectsList1.ProjectID;

                InfiniumProjects.PauseProject(infiniumProjectsList1.ProjectID);


                if (infiniumProjectsFilterStates1.Selected == -1 || infiniumProjectsFilterGroups1.Selected == -1)//new projects or messages selected
                {
                    if (bNewMessagesSelected)
                    {
                        InfiniumProjects.FillProjectsNewsUpdates(Security.CurrentUserID);
                    }

                    if (bNewProjectsSelected)
                    {
                        InfiniumProjects.FillProjectsNew(Security.CurrentUserID);
                    }
                }
                else
                    InfiniumProjects.FillProjects(infiniumProjectsFilterStates1.Selected, infiniumProjectsFilterGroups1.Selected,
                                                  infiniumProjectsFilterGroups1.SelectedUser, infiniumProjectsFilterGroups1.SelectedDepartment);

                InfiniumProjects.FillMembers(infiniumProjectsFilterStates1.Selected);
                infiniumProjectsFilterGroups1.UsersDataTable = InfiniumProjects.ProjectUsersMembersDataTable;
                infiniumProjectsFilterGroups1.DepartmentsDataTable = InfiniumProjects.ProjectDepsMembersDataTable;
                infiniumProjectsList1.Filter();

                //infiniumProjectsList1.Selected = infiniumProjectsList1.ProjectsDataTable.Rows.IndexOf(
                //                infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + iProjectID)[0]);

                if (bNeedSplash)
                    bC = true;

                bNeedNewsSplash = true;
            }
        }

        private void StatusCanceledActivePicture_Click_1(object sender, EventArgs e)
        {
            if (InfiniumProjects.GetProjectStatus(infiniumProjectsList1.ProjectID) == 2)
                return;

            if (InfiniumProjects.IsProposition(infiniumProjectsList1.ProjectID) == true)
                return;

            if (InfiniumProjects.CanChangeStatusProject(infiniumProjectsList1.ProjectID) == false)//only members and author can edit projects
            {
                InfiniumTips.ShowTip(this, 50, 85, "Только автор и участники могут редактировать проект", 2800);
                return;
            }

            bool OK = LightMessageBox.Show(ref TopForm, true,
                    "Отменить выбранный проект?", "Удаление проекта");

            if (!OK)
                return;

            if (bNeedSplash)
            {
                bNeedNewsSplash = false;
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                int iProjectID = infiniumProjectsList1.ProjectID;

                InfiniumProjects.CancelProject(infiniumProjectsList1.ProjectID);


                if (infiniumProjectsFilterStates1.Selected == -1 || infiniumProjectsFilterGroups1.Selected == -1)//new projects or messages selected
                {
                    if (bNewMessagesSelected)
                    {
                        InfiniumProjects.FillProjectsNewsUpdates(Security.CurrentUserID);
                    }

                    if (bNewProjectsSelected)
                    {
                        InfiniumProjects.FillProjectsNew(Security.CurrentUserID);
                    }
                }
                else
                    InfiniumProjects.FillProjects(infiniumProjectsFilterStates1.Selected, infiniumProjectsFilterGroups1.Selected,
                                                  infiniumProjectsFilterGroups1.SelectedUser, infiniumProjectsFilterGroups1.SelectedDepartment);

                InfiniumProjects.FillMembers(infiniumProjectsFilterStates1.Selected);
                infiniumProjectsFilterGroups1.UsersDataTable = InfiniumProjects.ProjectUsersMembersDataTable;
                infiniumProjectsFilterGroups1.DepartmentsDataTable = InfiniumProjects.ProjectDepsMembersDataTable;
                infiniumProjectsList1.Filter();

                //infiniumProjectsList1.Selected = infiniumProjectsList1.ProjectsDataTable.Rows.IndexOf(
                //                infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + iProjectID)[0]);

                if (bNeedSplash)
                    bC = true;

                bNeedNewsSplash = true;
            }
        }

        private void StatusEndActivePicture_Click(object sender, EventArgs e)
        {
            if (InfiniumProjects.GetProjectStatus(infiniumProjectsList1.ProjectID) == 3)
                return;

            if (InfiniumProjects.IsProposition(infiniumProjectsList1.ProjectID) == true)
                return;

            if (InfiniumProjects.CanChangeStatusProject(infiniumProjectsList1.ProjectID) == false)//only members and author can edit projects
            {
                InfiniumTips.ShowTip(this, 50, 85, "Только автор и участники могут редактировать проект", 2800);
                return;
            }

            bool OK = LightMessageBox.Show(ref TopForm, true,
                    "Завершить выбранный проект?", "Удаление проекта");

            if (!OK)
                return;

            if (bNeedSplash)
            {
                bNeedNewsSplash = false;
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;


                int iProjectID = infiniumProjectsList1.ProjectID;

                InfiniumProjects.EndProject(infiniumProjectsList1.ProjectID);


                if (infiniumProjectsFilterStates1.Selected == -1 || infiniumProjectsFilterGroups1.Selected == -1)//new projects or messages selected
                {
                    if (bNewMessagesSelected)
                    {
                        InfiniumProjects.FillProjectsNewsUpdates(Security.CurrentUserID);
                    }

                    if (bNewProjectsSelected)
                    {
                        InfiniumProjects.FillProjectsNew(Security.CurrentUserID);
                    }
                }
                else
                    InfiniumProjects.FillProjects(infiniumProjectsFilterStates1.Selected, infiniumProjectsFilterGroups1.Selected,
                                                  infiniumProjectsFilterGroups1.SelectedUser, infiniumProjectsFilterGroups1.SelectedDepartment);

                InfiniumProjects.FillMembers(infiniumProjectsFilterStates1.Selected);
                infiniumProjectsFilterGroups1.UsersDataTable = InfiniumProjects.ProjectUsersMembersDataTable;
                infiniumProjectsFilterGroups1.DepartmentsDataTable = InfiniumProjects.ProjectDepsMembersDataTable;
                infiniumProjectsList1.Filter();

                //infiniumProjectsList1.Selected = infiniumProjectsList1.ProjectsDataTable.Rows.IndexOf(
                //                infiniumProjectsList1.ProjectsDataTable.Select("ProjectID = " + iProjectID)[0]);

                if (bNeedSplash)
                    bC = true;

                bNeedNewsSplash = true;
            }
        }

        private void AddNewsPicture_Click(object sender, EventArgs e)
        {
            AddNewsLabel_Click(null, null);
        }

        private void NewsContainer_NeedMoreNews(object sender, EventArgs e)
        {
            if (InfiniumProjects.IsMoreNews(NewsContainer.NewsCount, infiniumProjectsList1.ProjectID))
                MoreNewsLabel.Visible = true;
            else
                MoreNewsLabel.Visible = false;
        }

        private void NewsContainer_NoNeedMoreNews(object sender, EventArgs e)
        {
            if (MoreNewsLabel.Visible)
                MoreNewsLabel.Visible = false;
        }

        private void MoreNewsLabel_Click(object sender, EventArgs e)
        {
            int iNewsCount = NewsContainer.NewsCount;

            InfiniumProjects.FillMoreNews(NewsContainer.NewsCount + 20, infiniumProjectsList1.ProjectID);
            NewsContainer.NewsDataTable = InfiniumProjects.ProjectNewsDataTable.Copy();
            NewsContainer.CreateNews();
            NewsContainer.ScrollToNews(iNewsCount);
            MoreNewsLabel.Visible = false;
            NewsContainer.bNeedMoreNews = false;
        }

        private void EditProjectActivePicture_Click(object sender, EventArgs e)
        {
            EditProjectLabel_Click(null, null);
        }

        private void DeleteProjectActivePicture_Click(object sender, EventArgs e)
        {
            DeleteProjectLabel_Click(null, null);
        }

        private void SubscribeItLabel_Click(object sender, EventArgs e)
        {
            if (SubscribeItLabel.Text == "Подписаться")
            {
                InfiniumProjects.AddSubscribeToUpdates(128, Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]), Security.CurrentUserID);
                SubscribeItLabel.Text = "Отписаться";
            }
            else
            {
                InfiniumProjects.RemoveSubscribeToUpdates(128, Convert.ToInt32(InfiniumProjects.ProjectsDataTable.Rows[infiniumProjectsList1.Selected]["ProjectID"]), Security.CurrentUserID);
                SubscribeItLabel.Text = "Подписаться";
            }

        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NewsContainer.Visible = false;

            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void ProposLabel_Click(object sender, EventArgs e)
        {
            if (bNewProjectsSelected)
            {
                bNewProjectsSelected = false;
                NewProjectsLabel.ForeColor = Color.FromArgb(60, 60, 60);
            }

            if (bProposSelected)
            {
                bProposSelected = false;
                ProposLabel.ForeColor = Color.FromArgb(60, 60, 60);
            }

            bProposSelected = true;
            ProposLabel.ForeColor = Color.FromArgb(56, 184, 238);
            ProposActivePicture.BringToFront();

            if (bNeedSplash)
            {
                bNeedNewsSplash = false;
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(UpdatePanel.Top, UpdatePanel.Left,
                                                   UpdatePanel.Height, UpdatePanel.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            InfiniumProjects.FillPropositions();
            infiniumProjectsList1.ProjectsDataTable = InfiniumProjects.ProjectsDataTable;
            infiniumProjectsList1.Filter();

            StatusStartActivePicture.SendToBack();
            StatusCanceledActivePicture.SendToBack();
            StatusPauseActivePicture.SendToBack();
            StatusEndActivePicture.SendToBack();

            StartProjectDateLabel.Text = "нет";
            PauseProjectDateLabel.Text = "нет";
            CancelProjectDateLabel.Text = "нет";
            EndProjectDateLabel.Text = "нет";

            //infiniumProjectsList1.Filter();

            infiniumProjectsFilterGroups1.DeselectAll();
            infiniumProjectsFilterStates1.DeselectAll();

            if (bNeedSplash)
                bC = true;

            bNeedNewsSplash = true;


            ProposCountLabel.Text = "";
        }

    }
}