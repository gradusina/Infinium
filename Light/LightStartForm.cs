using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class LightStartForm : Form
    {
        //NotifyForm NotifyForm = null;

        public Form TopForm = null;
        private LoginForm LoginForm;
        private const int eHide = 2;
        private const int eShow = 1;
        private const int eClose = 3;
        private const int eMainMenu = 4;

        public static bool NotifyShowed = false;
        private int FormEvent = 0;
        private bool bC = false;
        private bool bNeedSplash = false;
        private ActiveNotifySystem ActiveNotifySystem;
        private InfiniumStart InfiniumStart;
        private bool Logout = false;
        private bool FirstLoad = true;
        private System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("ru-RU");

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        private Thread NotifyThread;
        private TablesManager TM;
        private int iRefreshTime = 150000;
        private NotifyForm NotifyForm;

        public LightStartForm(LoginForm tLoginForm)
        {
            InitializeComponent();

            LoginForm = tLoginForm;

            TM = new TablesManager();

            MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            //CurrentTimeLabel.Text = DateTime.Now.ToString("HH:mm");
            //CurrentDayOfWeekLabel.Text = DateTime.Now.ToString("dddd");
            //CurrentDayMonthLabel.Text = DateTime.Now.ToString("dd MMMM");

            AnimateTimer.Enabled = true;

            UserLabel.Text = Security.CurrentUserRName;

            //UpdatesPanel.Width = UserPanel.Left - UpdatesPanel.Left - 5;

            PhotoBox.Image = UserProfile.GetUserPhoto();

            InfiniumStart = new Infinium.InfiniumStart();
            InfiniumTilesContainer.ItemsDataTable = InfiniumStart.ModulesDataTable;
            InfiniumTilesContainer.MenuItemID = 0;
            InfiniumTilesContainer.InitializeItems();

            ActiveNotifySystem = new Infinium.ActiveNotifySystem();
            InfiniumNotifyList.ModulesDataTable = InfiniumStart.FullModulesDataTable;

            InfiniumStartMenu.ItemsDataTable = InfiniumStart.MenuItemsDataTable;
            InfiniumStartMenu.InitializeItems();
            InfiniumStartMenu.Selected = 0;

            InfiniumMinimizeList.ModulesDataTable = InfiniumStart.FullModulesDataTable;

            if (InfiniumStart.ModulesDataTable.Select("MenuItemID = 0").Count() == 0)
                InfiniumStartMenu.Selected = 1;

            TopForm = null;
            if (TopForm != null)
                OnLineControl.IamOnline(ActiveNotifySystem.GetModuleIDByForm(TopForm.Name), true);
            else
                OnLineControl.IamOnline(0, true);

            OnLineControl.SetOffline();

            NotifyRefreshT = new NotifyRefresh(FuckingNotify);
            OnlineFuck = new OnlineFuckingDelegate(GetTopMostAndModuleName);

            NotifyThread = new Thread(delegate () { NotifyCheck(); });
            NotifyThread.Start();

            while (!SplashForm.bCreated) ;
        }

        private OnlineStruct OS;

        private delegate void NotifyRefresh();

        //delegate void ANSNotifyContainerRefresh();
        private delegate OnlineStruct OnlineFuckingDelegate();

        private OnlineFuckingDelegate OnlineFuck;
        private NotifyRefresh NotifyRefreshT;

        private void FuckingNotify()
        {
            //try
            //{
            InfiniumNotifyList.ItemsDataTable = ActiveNotifySystem.ModulesUpdatesDataTable;
            InfiniumNotifyList.InitializeItems();

            if (TopForm != null)
            {
                if (TopForm.Name == "MessagesForm")
                {
                    using (Stream str = Properties.Resources.MESSAGENOTIFY)
                    {
                        using (SoundPlayer snd = new SoundPlayer(str))
                        {
                            snd.Play();
                        }
                    }
                }
                else
                {
                    using (Stream str = Properties.Resources._01_01)
                    {
                        using (SoundPlayer snd = new SoundPlayer(str))
                        {
                            snd.Play();
                        }
                    }
                }
            }
            else
                using (Stream str = Properties.Resources._01_01)
                {
                    using (SoundPlayer snd = new SoundPlayer(str))
                    {
                        snd.Play();
                    }
                }



            //если открытый модуль типа InfiniumForm
            if (TopForm != null)
            {
                if (TopForm.GetType().BaseType.Name == "InfiniumForm")
                    ((InfiniumForm)TopForm).OnANSUpdate();
            }

            if (TopForm != null)
                if (ActiveNotifySystem.ModulesDataTable.Select("ModuleID = " + ActiveNotifySystem.iCurrentModuleID)[0]["FormName"].ToString() == TopForm.Name)
                    if (GetActiveWindow() == TopForm.Handle)
                        return;

            if (GetActiveWindow() == Handle)//только звук
            {
                return;
            }

            //показываем всплывающее окно
            if (NotifyForm.bShowed == true)
            {
                if (NotifyForm != null)
                    NotifyForm.Close();

                int ModuleID = 0;
                int MoreCount = 0;
                int Count = ActiveNotifySystem.GetLastModuleUpdate(ref ModuleID, ref MoreCount);
                NotifyForm = new NotifyForm(this, ref ActiveNotifySystem, ModuleID, Count, MoreCount);
                NotifyForm.Show();
            }
            else
            {
                int ModuleID = 0;
                int MoreCount = 0;
                int Count = ActiveNotifySystem.GetLastModuleUpdate(ref ModuleID, ref MoreCount);
                NotifyForm = new NotifyForm(this, ref ActiveNotifySystem, ModuleID, Count, MoreCount);
                NotifyForm.Show();
            }
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //}
        }

        public void StartModuleFromNotify(int ModuleID)
        {
            if (TopForm != null)
                if (TopForm.Name == InfiniumStart.FullModulesDataTable.Select("ModuleID = " + ModuleID)[0]["FormName"].ToString())
                {
                    return;
                }

            if (ModuleID != 80)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
                T.Start();

                while (!SplashForm.bCreated) ;
            }

            if (TopForm != null)
                if (TopForm.Name != InfiniumStart.FullModulesDataTable.Select("ModuleID = " + ModuleID)[0]["FormName"].ToString())
                {
                    HideForm(TopForm);
                }

            Form ModuleForm = null;

            if (ModuleID == 80)//messages
            {
                MessagesButton_Click(null, null);
            }
            else
            {
                //check if running
                Form Form = InfiniumMinimizeList.GetForm(InfiniumStart.FullModulesDataTable.Select("ModuleID = " + ModuleID)[0]["FormName"].ToString());
                if (Form != null)
                {
                    TopForm = Form;
                    Form.Show();
                }
                else
                {
                    Type CAType = Type.GetType("Infinium." + InfiniumStart.FullModulesDataTable.Select("ModuleID = " + ModuleID)[0]["FormName"].ToString());
                    ModuleForm = (Form)Activator.CreateInstance(CAType, this);
                    TopForm = ModuleForm;
                    ModuleForm.ShowDialog();
                }
            }

            ActiveNotifySystem.FillUpdates();
            InfiniumNotifyList.ItemsDataTable = ActiveNotifySystem.ModulesUpdatesDataTable;
            InfiniumNotifyList.InitializeItems();
        }

        private struct OnlineStruct
        {
            public int ModuleID;
            public bool TopMost;
        }

        private OnlineStruct GetTopMostAndModuleName()
        {
            if (TopForm == null)
            {
                if (GetActiveWindow() == Handle)
                {
                    OS.TopMost = true;
                    OS.ModuleID = 0;
                }
                else
                {
                    OS.TopMost = false;
                    OS.ModuleID = 0;
                }
            }
            else
            {
                if (TopForm == this && GetActiveWindow() != Handle)
                {
                    OS.TopMost = false;
                    OS.ModuleID = 0;
                }
                else
                {
                    if (GetActiveWindow() == Handle)
                    {
                        OS.TopMost = true;
                        OS.ModuleID = 0;
                    }
                    else
                        if ((int)GetActiveWindow() == 0)
                    {
                        OS.TopMost = false;
                        OS.ModuleID = ActiveNotifySystem.GetModuleIDByForm(TopForm.Name);
                    }
                    else
                    {
                        OS.TopMost = true;
                        OS.ModuleID = ActiveNotifySystem.GetModuleIDByForm(TopForm.Name);
                    }
                }
            }

            return OS;
        }

        private void NotifyCheck()
        {
            while (true)
            {
                if (IsHandleCreated)
                {
                    //OnLineControl.SetOffline();
                    //OnLineControl.SetOfflineClient();
                    //OnLineControl.SetOfflineManager();

                    Invoke(OnlineFuck);
                    OnLineControl.IamOnline(OS.ModuleID, OS.TopMost);

                    if (ActiveNotifySystem.IsNewUpdates(Security.CurrentUserID) > 0)
                    {
                        if (ActiveNotifySystem.CheckLastUpdate())
                        {
                            ActiveNotifySystem.FillUpdates();

                            Invoke(NotifyRefreshT);
                        }
                    }
                }

                Thread.Sleep(iRefreshTime);
            }

        }

        public void HideForm(Form Form)
        {
            Activate();
            TopMost = true;
            Form.Hide();

            LoginForm.Activate();
            TopMost = false;
            Activate();

            TopForm = null;

            InfiniumMinimizeList.AddModule(ref Form);
        }


        public void CloseForm(Form Form)
        {
            Activate();
            TopMost = true;
            Form.Hide();

            LoginForm.Activate();
            TopMost = false;
            Activate();

            TopForm = null;

            string FormName = Form.Name;

            InfiniumMinimizeList.RemoveModule(Form.Name);

            Thread T = new Thread(delegate ()
            {
                Security.ExitFromModule(FormName);
            });
            T.Start();

            Form.Dispose();

            GC.Collect();
        }


        private void AnimateTimer_Tick(object sender, EventArgs e)
        {
            //without animation
            if (!DatabaseConfigsManager.Animation)
            {
                Opacity = 1;
                AnimateTimer.Enabled = false;

                if (FormEvent == eClose)
                {
                    LoginForm.Show();
                    LoginForm.ShowInTaskbar = true;
                    //LoginForm.CloseJournalRec();
                    Close();
                    return;
                }

                SplashForm.CloseS = true;

                if (FirstLoad)
                {
                    LoginForm.ShowInTaskbar = false;
                    LoginForm.Visible = false;
                    AnimateTimer.Enabled = false;
                    FirstLoad = false;
                }
                else
                {
                    Activate();
                    TopMost = true;
                    AnimateTimer.Enabled = false;
                    TopMost = false;
                }

                bNeedSplash = true;

                //InfiniumTilesContainer.InitializeItems();

                return;
            }


            //with animation
            if (FormEvent == eClose)
            {
                if (Convert.ToDecimal(Opacity) != Convert.ToDecimal(0.00))
                {
                    Opacity = Convert.ToDouble(Convert.ToDecimal(Opacity) - Convert.ToDecimal(0.05));
                }
                else
                {
                    AnimateTimer.Enabled = false;
                    LoginForm.Show();
                    LoginForm.ShowInTaskbar = true;

                    Close();
                }

                return;
            }

            if (Opacity != 1)
                Opacity += 0.05;
            else
            {
                AnimateTimer.Enabled = false;

                SplashForm.CloseS = true;

                if (FirstLoad)
                {
                    LoginForm.ShowInTaskbar = false;
                    LoginForm.Visible = false;
                    AnimateTimer.Enabled = false;
                    FirstLoad = false;
                }
                else
                {
                    Activate();
                    TopMost = true;
                    AnimateTimer.Enabled = false;
                    TopMost = false;
                }

                bNeedSplash = true;
            }
        }


        private void MenuCloseButton_Click_1(object sender, EventArgs e)
        {
            NotifyThread.Abort();

            LoginForm.CloseJournalRec();
            LoginForm.Close();
        }

        private void LightStartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LoginForm.CloseJournalRec();

            if (!Logout)
                LoginForm.Close();
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            NotifyThread.Abort();
            Logout = true;

            FormEvent = eClose;

            AnimateTimer.Enabled = true;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }


        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (Control.FromHandle(m.LParam) == null)
                {
                    if (TopForm != null)
                        TopForm.Activate();
                }
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Maximized;

            Activate();
        }

        private void LightStartForm_Activated(object sender, EventArgs e)
        {
            if (TopForm != null)
                TopForm.Activate();
        }

        private void LightStartForm_Load(object sender, EventArgs e)
        {
            InfiniumTilesContainer.InitializeItems();
        }

        private void CurrentTimer_Tick(object sender, EventArgs e)
        {
            InfiniumClock.iMinutes = (DateTime.Now).Minute;
            InfiniumClock.iSeconds = (DateTime.Now).Second;
            InfiniumClock.iHours = (DateTime.Now).Hour;
            InfiniumClock.Refresh();

            TimeLabel.Text = (DateTime.Now).ToString("HH:mm:ss");
            DateLabel.Text = (DateTime.Now).ToString("dd MMMM yyyy");
            DayLabel.Text = (DateTime.Now).ToString("dddd");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }


        private void UpdatesTimer_Tick(object sender, EventArgs e)
        {
            //NotifyCheck();
        }

        private void InfiniumStartMenu_ItemClicked(object sender, string MenuItemName, int MenuItemID)
        {
            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(InfiniumTilesContainer.Top + MainPanel.Top, InfiniumTilesContainer.Left,
                                                   InfiniumTilesContainer.Height, InfiniumTilesContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            InfiniumTilesContainer.MenuItemID = MenuItemID;
            InfiniumTilesContainer.InitializeItems();

            bC = true;
        }

        private void SplashTimer_Tick(object sender, EventArgs e)
        {
            if (bC)
            {
                while (SplashWindow.bSmallCreated)
                    CoverWaitForm.CloseS = true;

                bC = false;
            }
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void InfiniumTilesContainer_RightMouseClicked(object sender, string FormName)
        {
            CurFormName = FormName;

            if (InfiniumStartMenu.Selected == 0)
            {
                MenuRemoveFromFavorite.Visible = true;
                MenuAddToFavorite.Visible = false;
            }
            else
            {
                MenuRemoveFromFavorite.Visible = false;
                MenuAddToFavorite.Visible = true;
            }

            TileContextMenu.Show(InfiniumTilesContainer);
        }

        private string CurFormName = "";

        private void MenuAddToFavorite_Click(object sender, EventArgs e)
        {
            if (InfiniumStart.AddModuleToFavorite(CurFormName) == false)
            {
                InfiniumTips.ShowTip(this, 50, 85, "Модуль уже был добавлен ранее", 4000);
                return;
            }
            else
            {
                InfiniumTips.ShowTip(this, 50, 85, "Модуль добавлен", 2500);
                return;
            }
        }

        private void MenuRemoveFromFavorite_Click(object sender, EventArgs e)
        {
            InfiniumStart.RemoveModuleFromFavorite(CurFormName);

            if (bNeedSplash)
            {
                Thread T = new Thread(delegate ()
                {
                    SplashWindow.CreateCoverSplash(InfiniumTilesContainer.Top + MainPanel.Top, InfiniumTilesContainer.Left,
                                                   InfiniumTilesContainer.Height, InfiniumTilesContainer.Width);
                });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;
            }

            InfiniumTilesContainer.MenuItemID = 0;
            InfiniumTilesContainer.InitializeItems();

            bC = true;
        }

        private void InfiniumTilesContainer_ItemClicked(object sender, string FormName)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            Form ModuleForm = null;

            //check if running
            Form Form = InfiniumMinimizeList.GetForm(FormName);
            if (Form != null)
            {
                TopForm = Form;
                Form.ShowDialog();
            }
            else
            {
                Type CAType = Type.GetType("Infinium." + FormName);
                ModuleForm = (Form)Activator.CreateInstance(CAType, this);
                TopForm = ModuleForm;
                Security.EnterInModule(FormName);
                ModuleForm.ShowDialog();
            }

            if (InfiniumNotifyList.Items != null)
                if (InfiniumNotifyList.Items.Count() > 0)
                {
                    ActiveNotifySystem.FillUpdates();
                    InfiniumNotifyList.ItemsDataTable = ActiveNotifySystem.ModulesUpdatesDataTable;
                    InfiniumNotifyList.InitializeItems();
                }
        }

        private void InfiniumNotifyList_ItemClicked(object sender, string FormName)
        {
            if (FormName == "MessagesForm")
            {
                MessagesButton_Click(null, null);

                if (NotifyForm != null)
                {
                    NotifyForm.Close();
                    NotifyForm.Dispose();
                    NotifyForm = null;
                }

                ActiveNotifySystem.FillUpdates();
                InfiniumNotifyList.ItemsDataTable = ActiveNotifySystem.ModulesUpdatesDataTable;
                InfiniumNotifyList.InitializeItems();

                return;
            }

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;


            if (NotifyForm != null)
            {
                NotifyForm.Close();
                NotifyForm.Dispose();
                NotifyForm = null;
            }

            Form ModuleForm = null;

            //check if running
            Form Form = InfiniumMinimizeList.GetForm(FormName);
            if (Form != null)
            {
                TopForm = Form;
                Form.ShowDialog();
            }
            else
            {
                Type CAType = Type.GetType("Infinium." + FormName);
                ModuleForm = (Form)Activator.CreateInstance(CAType, this);
                TopForm = ModuleForm;
                ModuleForm.ShowDialog();
            }

            ActiveNotifySystem.FillUpdates();
            InfiniumNotifyList.ItemsDataTable = ActiveNotifySystem.ModulesUpdatesDataTable;
            InfiniumNotifyList.InitializeItems();
        }

        private void InfiniumMinimizeList_ItemClicked(object sender, string FormName)
        {
            if (FormName == "MessagesForm")
            {
                MessagesButton_Click(null, null);
                InfiniumMinimizeList.RemoveModule(FormName);
            }
            else
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
                T.Start();

                while (!SplashForm.bCreated) ;

                Form ModuleForm = InfiniumMinimizeList.GetForm(FormName);
                TopForm = ModuleForm;
                ModuleForm.ShowDialog();
            }

            if (InfiniumNotifyList.Items != null)
                if (InfiniumNotifyList.Items.Count() > 0)
                {
                    ActiveNotifySystem.FillUpdates();
                    InfiniumNotifyList.ItemsDataTable = ActiveNotifySystem.ModulesUpdatesDataTable;
                    InfiniumNotifyList.InitializeItems();
                }
        }

        private void InfiniumMinimizeList_CloseClicked(object sender, string FormName)
        {
            Form ModuleForm = InfiniumMinimizeList.GetForm(FormName);

            InfiniumMinimizeList.RemoveModule(FormName);
            Security.ExitFromModule(FormName);
            ModuleForm.Dispose();

            GC.Collect();
        }

        private void InfiniumClock_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            Form ModuleForm = null;

            //check if running
            Form Form = InfiniumMinimizeList.GetForm("DayPlannerForm");
            if (Form != null)
            {
                TopForm = Form;
                Form.ShowDialog();
            }
            else
            {
                Type CAType = Type.GetType("Infinium." + "DayPlannerForm");
                ModuleForm = (Form)Activator.CreateInstance(CAType, this);
                TopForm = ModuleForm;
                Security.EnterInModule("DayPlannerForm");
                ModuleForm.ShowDialog();
            }

            if (InfiniumNotifyList.Items != null)
                if (InfiniumNotifyList.Items.Count() > 0)
                {
                    ActiveNotifySystem.FillUpdates();
                    InfiniumNotifyList.ItemsDataTable = ActiveNotifySystem.ModulesUpdatesDataTable;
                    InfiniumNotifyList.InitializeItems();
                }
        }

        private void MessagesButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            MessagesForm MessagesForm = new MessagesForm(ref TopForm);

            TopForm = MessagesForm;

            Security.EnterInModule("MessagesForm");

            MessagesForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (InfiniumNotifyList.Items != null)
                if (InfiniumNotifyList.Items.Count() > 0)
                {
                    ActiveNotifySystem.FillUpdates();
                    InfiniumNotifyList.ItemsDataTable = ActiveNotifySystem.ModulesUpdatesDataTable;
                    InfiniumNotifyList.InitializeItems();
                }
        }

        private void PhotoBox_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            Form ModuleForm = null;

            //check if running
            Form Form = InfiniumMinimizeList.GetForm("PersonalSettingsForm");
            if (Form != null)
            {
                TopForm = Form;
                Form.ShowDialog();
            }
            else
            {
                Type CAType = Type.GetType("Infinium." + "PersonalSettingsForm");
                ModuleForm = (Form)Activator.CreateInstance(CAType, this);
                TopForm = ModuleForm;
                Security.EnterInModule("PersonalSettingsForm");
                ModuleForm.ShowDialog();
            }

            if (InfiniumNotifyList.Items != null)
                if (InfiniumNotifyList.Items.Count() > 0)
                {
                    ActiveNotifySystem.FillUpdates();
                    InfiniumNotifyList.ItemsDataTable = ActiveNotifySystem.ModulesUpdatesDataTable;
                    InfiniumNotifyList.InitializeItems();
                }
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
        }
    }
}
