using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class MessagesForm : InfiniumForm
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm;

        InfiniumMessages InfiniumMessages;

        bool bC = false;

        bool bNeedSplash = false;

        public MessagesForm(ref Form tTopForm)
        {
            InitializeComponent();

            Thread T = new Thread(delegate ()
            {
                SplashWindow.CreateCoverSplash(true, (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2, (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                                                503, 1014);
            });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;


            TopForm = tTopForm;

            InfiniumMessages = new InfiniumMessages();
            UsersList.ItemsDataTable = InfiniumMessages.UsersDataTable;
            UsersList.InitializeItems();

            InfiniumMessages.FillSelectedUsers(-1);

            SelectedUsersList.ItemsDataTable = InfiniumMessages.SelectedUsersDataTable;
            SelectedUsersList.InitializeItems();

            MessagesContainer.UsersDataTable = InfiniumMessages.FullUsersDataTable;
            MessagesContainer.CurrentUserID = Security.CurrentUserID;

            if (InfiniumMessages.SelectedUsersDataTable.Rows.Count > 0)
            {
                UsersList.Top = 159;
                UsersList.Height = UsersList.Parent.Height - UsersList.Top;

                SelectedUsersList.Selected = 0;
                InfiniumMessages.FillMessages(SelectedUsersList.Items[0].UserID);

                MessagesContainer.ItemsDataTable = InfiniumMessages.MessagesDataTable;
                MessagesContainer.InitializeItems();
            }
            else
            {
                UsersList.Top = 0;
                UsersList.Height = UsersList.Parent.Height;

                UsersList.Selected = 0;

                InfiniumMessages.FillMessages(UsersList.Items[0].UserID);
                MessagesContainer.ItemsDataTable = InfiniumMessages.MessagesDataTable;
                MessagesContainer.InitializeItems();
            }

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
                    bNeedSplash = true;
                }

                return;
            }
        }

        private void AddNewsForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void UsersList_ItemClicked(object sender, string Name, int UserID)
        {
            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(true, this.Top + MessagesContainer.Top, this.Left + MessagesContainer.Left,
                                                    MessagesContainer.Height, MessagesContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

            }

            UserImageBox.Image = UsersList.Items[UsersList.Selected].Image;

            InfiniumMessages.FillMessages(UserID);

            MessagesContainer.InitializeItems();

            InfiniumMessages.ClearSubscribes(UserID);

            SelectedUsersList.Selected = -1;

            if (bNeedSplash)
                bC = true;
        }

        private void kryptonRichTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13 && e.Control)
            {
                SendMessagesButton_Click(null, null);
                e.SuppressKeyPress = false;
                e.Handled = true;
            }
        }

        private void SendMessagesButton_Click(object sender, EventArgs e)
        {
            if (TextBox.Text.Length == 0)
                return;

            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(true, this.Top + MessagesContainer.Top, this.Left + MessagesContainer.Left,
                                                    MessagesContainer.Height, MessagesContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            if (SelectedUsersList.Selected == -1)
            {
                int iRes = InfiniumMessages.AddUserToSelected(UsersList.Items[UsersList.Selected].UserID);

                SelectedUsersList.InitializeItems();

                if (iRes == -1)//ok
                {
                    SelectedUsersList.Selected = SelectedUsersList.Items.Count() - 1;
                    SelectedUsersList.ScrollDown();
                }
                else//in the list already
                {
                    SelectedUsersList.SelectOnly(iRes);
                }

            }

            InfiniumMessages.SendMessage(TextBox.Text, SelectedUsersList.Items[SelectedUsersList.Selected].UserID);

            InfiniumMessages.FillMessages(SelectedUsersList.Items[SelectedUsersList.Selected].UserID);

            MessagesContainer.InitializeItems();

            TextBox.Clear();

            if (UsersList.Top != 159)
            {
                UsersList.Top = 159;
                UsersList.Height = UsersList.Parent.Height - UsersList.Top;
            }

            if (bNeedSplash)
                bC = true;
        }

        private void CancelMessagesButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void SelectedUsersList_CloseClicked(object sender, int UserID)
        {
            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(true, this.Top + panel1.Top, this.Left + panel1.Left,
                                                    panel1.Height, panel1.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            int iSel = SelectedUsersList.Selected;


            InfiniumMessages.ClearSubscribes(UserID);

            InfiniumMessages.RemoveUserFromSelected(UserID);
            SelectedUsersList.InitializeItems();


            if (SelectedUsersList.Items.Count() > 0)
            {
                if (iSel > SelectedUsersList.Items.Count() - 1)
                    SelectedUsersList.Selected = SelectedUsersList.Items.Count() - 1;
                else
                    SelectedUsersList.Selected = iSel;

                UsersList.Top = 159;
                UsersList.Height = UsersList.Parent.Height - UsersList.Top;
            }
            else
            {
                if (UsersList.Selected == -1)
                    UsersList.Selected = 0;

                UsersList.Top = 0;
                UsersList.Height = UsersList.Parent.Height;
            }

            if (bNeedSplash)
                bC = true;
        }

        private void SplashTimer_Tick(object sender, EventArgs e)
        {
            if (bC)
            {
                while (SplashWindow.bSmallCreated)
                    CoverWaitForm.CloseS = true;

                bC = false;

                TextBox.Focus();
            }
        }

        private void SelectedUsersList_ItemClicked(object sender, string Name, int UserID)
        {
            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(true, this.Top + MessagesContainer.Top, this.Left + MessagesContainer.Left,
                                                    MessagesContainer.Height, MessagesContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

            }

            UserImageBox.Image = SelectedUsersList.Items[SelectedUsersList.Selected].Image;

            InfiniumMessages.FillMessages(UserID);

            MessagesContainer.InitializeItems();

            InfiniumMessages.ClearSubscribes(UserID);

            InfiniumMessages.ClearCount(UserID);

            int iSel = SelectedUsersList.Selected;

            SelectedUsersList.InitializeItems();

            SelectedUsersList.SelectOnly(iSel);

            UsersList.Selected = -1;

            if (bNeedSplash)
                bC = true;
        }

        private void MessagesForm_ANSUpdate(object sender)
        {
            int iSel = -1;

            bool bRes = false;

            iSel = SelectedUsersList.Selected;


            if (iSel > -1)
                bRes = InfiniumMessages.FillSelectedUsers(SelectedUsersList.Items[iSel].UserID);
            else
                InfiniumMessages.FillSelectedUsers(iSel);

            SelectedUsersList.InitializeItems();

            if (iSel > -1)
            {
                if (bRes)
                    SelectedUsersList.Selected = iSel;
                else
                    SelectedUsersList.SelectOnly(iSel);
            }

            UsersList.Top = 159;
            UsersList.Height = UsersList.Parent.Height - UsersList.Top;
        }
    }
}
