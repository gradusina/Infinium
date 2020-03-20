using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class UserProfilesForm : Form
    {
        const int iEditUsers = 84;
        const int iAdminRole = 85;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;


        int CurrentUserID;

        Form TopForm = null;

        AdminUsersManagement AdminUsersManagement;
        UserProfile.Contacts Contacts;
        UserProfile.PersonalInform PersonalInform;

        UserProfile UserProfile;

        //RoleTypes RoleType = RoleTypes.OrdinaryRole;
        //public enum RoleTypes
        //{
        //    OrdinaryRole = 0,
        //    AdminRole = 1,
        //    LogisticsRole = 2
        //}

        public UserProfilesForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();



            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            UserProfile = new Infinium.UserProfile();
            LightStartForm = tLightStartForm;


            UserComboBox.DataSource = UserProfile.UsersBindingSource;
            UserComboBox.DisplayMember = "Name";
            UserComboBox.ValueMember = "UserID";

            Initialize();

            SaveContactsButton.Enabled = false;
            SavePersonalButton.Enabled = false;
            btnSaveUsers.Enabled = false;
            btnAddUser.Enabled = false;

            UserProfile.GetPermissions(Security.CurrentUserID, this.Name);
            if (UserProfile.PermissionGranted(iAdminRole))
            {
                SaveContactsButton.Enabled = true;
                SavePersonalButton.Enabled = true;
                btnSaveUsers.Enabled = true;
                btnAddUser.Enabled = true;
                //RoleType = RoleTypes.AdminRole;
            }
            if (UserProfile.PermissionGranted(iEditUsers))
            {
                SaveContactsButton.Enabled = true;
                SavePersonalButton.Enabled = true;
                btnSaveUsers.Enabled = true;
                btnAddUser.Enabled = true;
                //RoleType = RoleTypes.LogisticsRole;
            }

            TabNameLabel.Text = MainSelectPanelCheckSet.CheckedButton.Text;

            UserComboBox_SelectionChangeCommitted(null, null);

            UserComboBox_SelectionChangeCommitted(null, null);

            while (!SplashForm.bCreated) ;
        }

        private void ContactsToControls()
        {
            MobPersCCodeTextBox.Text = "";
            MobPersNumberTextBox.Text = "";
            MobPersOpCodeTextBox.Text = "";
            MobWorkCCodeTextBox.Text = "";
            MobWorkNumberTextBox.Text = "";
            MobWorkOpCodeTextBox.Text = "";
            CityCityCodeTextBox.Text = "";
            CityCountryCodeTextBox.Text = "";
            CityNumberTextBox.Text = "";
            WorkExtTextBox.Text = "";
            SkypeTextBox.Text = "";
            EmailTextBox.Text = "";
            ICQTextBox.Text = "";

            if (Contacts.PersonalMobilePhone.Length == 12)
            {
                MobPersCCodeTextBox.Text = Contacts.PersonalMobilePhone.Substring(0, 3);
                MobPersOpCodeTextBox.Text = Contacts.PersonalMobilePhone.Substring(3, 2);
                MobPersNumberTextBox.Text = Contacts.PersonalMobilePhone.Substring(5, 7);
            }

            if (Contacts.WorkMobilePhone.Length == 12)
            {
                MobWorkCCodeTextBox.Text = Contacts.WorkMobilePhone.Substring(0, 3);
                MobWorkOpCodeTextBox.Text = Contacts.WorkMobilePhone.Substring(3, 2);
                MobWorkNumberTextBox.Text = Contacts.WorkMobilePhone.Substring(5, 7);

            }

            if (Contacts.WorkStatPhone.Length == 12)
            {
                CityCountryCodeTextBox.Text = Contacts.WorkStatPhone.Substring(0, 3);
                CityCityCodeTextBox.Text = Contacts.WorkStatPhone.Substring(3, 2);
                CityNumberTextBox.Text = Contacts.WorkStatPhone.Substring(5, 7);

            }

            if (Contacts.WorkExtPhone.Length > 0)
                WorkExtTextBox.Text = Contacts.WorkExtPhone;

            if (Contacts.Skype.Length > 0)
                SkypeTextBox.Text = Contacts.Skype;

            if (Contacts.Email.Length > 0)
                EmailTextBox.Text = Contacts.Email;

            if (Contacts.ICQ.Length > 0)
                ICQTextBox.Text = Contacts.ICQ;

            NeedSpamCheckBox.Checked = Contacts.NeedSpam;
        }

        private void PersonalToControls()
        {
            PhotoBox.Image = UserProfile.GetUserPhoto(CurrentUserID);

            BirthDateTextEdit.Text = PersonalInform.BirthDate;
            //PositionComboBox.SelectedValue = PersonalInform.PositionID;

            if (PersonalInform.Education.Length > 0)
                EduComboBox.SelectedIndex = EduComboBox.Items.IndexOf(PersonalInform.Education);

            EduPlace.Text = PersonalInform.EducationPlace;

            LanguagesListBox.Items.Clear();
            EduComboBox.SelectedIndex = -1;
            MilitaryRankComboBox.SelectedIndex = -1;

            if (PersonalInform.Language.Length > 0)
            {
                string res = "";

                for (int i = 0; i < PersonalInform.Language.Length; i++)
                {
                    if (PersonalInform.Language[i] != '\r')
                        res += PersonalInform.Language[i];
                    else
                    {
                        LanguagesListBox.Items.Add(res);
                        res = "";
                        i++;
                    }
                }


            }

            PositionLabel.Text = string.Empty;
            string ProfilPosition = GetPosition(PersonalInform.ProfilPositionsDT, 1);
            string TPSPosition = GetPosition(PersonalInform.TPSPositionsDT, 2);

            if (ProfilPosition.Length > 0 && TPSPosition.Length > 0)
                PositionLabel.Text = ProfilPosition + "\n" + TPSPosition;
            else
            {
                if (ProfilPosition.Length > 0)
                    PositionLabel.Text = ProfilPosition;
                if (TPSPosition.Length > 0)
                    PositionLabel.Text = TPSPosition;
            }

            DriveACheckBox.Checked = PersonalInform.DriveA;
            DriveBCheckBox.Checked = PersonalInform.DriveB;
            DriveCCheckBox.Checked = PersonalInform.DriveC;
            DriveDCheckBox.Checked = PersonalInform.DriveD;
            DriveECheckBox.Checked = PersonalInform.DriveE;
            CombatArmTextBox.Text = PersonalInform.CombatArm;

            if (PersonalInform.MilitaryRank.Length > 0)
                MilitaryRankComboBox.SelectedIndex = MilitaryRankComboBox.Items.IndexOf(PersonalInform.MilitaryRank);
        }

        private string GetPosition(DataTable UsersPositionsDT, int FactoryID)
        {
            if (UsersPositionsDT.Rows.Count == 0)
                return "";

            Graphics G = this.CreateGraphics();
            string text = string.Empty;
            for (int i = 0; i < UsersPositionsDT.Rows.Count; i++)
            {
                string Rate = string.Empty;
                if (UsersPositionsDT.Rows[i]["Rate"].ToString().Length > 0)
                    Rate = Convert.ToDecimal(UsersPositionsDT.Rows[i]["Rate"]).ToString("G29");
                text += UserProfile.PositionsDataTable.Select("PositionID=" + Convert.ToInt32(UsersPositionsDT.Rows[i]["PositionID"]))[0]["Position"].ToString() +
                    " (" + Rate + " ЗОВ-Профиль)";
                if (FactoryID == 2)
                    text = UserProfile.PositionsDataTable.Select("PositionID=" + Convert.ToInt32(UsersPositionsDT.Rows[i]["PositionID"]))[0]["Position"].ToString() +
                        " (" + Rate + " ЗОВ-ТПС)";
                if (i != UsersPositionsDT.Rows.Count - 1)
                    text = text.Insert(text.Length, "\n");

            }
            G.Dispose();

            return text;
        }

        private void UserProfilesForm_Shown(object sender, EventArgs e)
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
            AdminUsersManagement = new AdminUsersManagement(ref UsersDataGrid);


            AdminUsersManagement.Rating();
        }


        private void MainSelectPanelCheckSet_CheckedButtonChanged(object sender, EventArgs e)
        {
            TabNameLabel.Text = MainSelectPanelCheckSet.CheckedButton.Text;

            if (MainSelectPanelCheckSet.CheckedButton.Name == "ContactInformationSelectButton")
                ContactInformationPanel.BringToFront();

            if (MainSelectPanelCheckSet.CheckedButton.Name == "PersonalSelectButton")
                PersonalPanel.BringToFront();

            if (MainSelectPanelCheckSet.CheckedButton.Name == "AllUsersSelectButton")
                AllUsersPanel.BringToFront();
        }

        private void SaveContactsButton_Click(object sender, EventArgs e)
        {
            Contacts.PersonalMobilePhone = "";

            if (MobPersCCodeTextBox.Text.Length == 3)
                if (MobPersOpCodeTextBox.Text.Length == 2)
                    if (MobPersNumberTextBox.Text.Length == 9)
                    {
                        Contacts.PersonalMobilePhone = MobPersCCodeTextBox.Text + MobPersOpCodeTextBox.Text +
                                        UserProfile.PhoneFormatToNumberFormat(MobPersNumberTextBox.Text);
                    }

            Contacts.WorkMobilePhone = "";

            if (MobWorkCCodeTextBox.Text.Length == 3)
                if (MobWorkOpCodeTextBox.Text.Length == 2)
                    if (MobWorkNumberTextBox.Text.Length == 9)
                    {
                        Contacts.WorkMobilePhone = MobWorkCCodeTextBox.Text + MobWorkOpCodeTextBox.Text + UserProfile.PhoneFormatToNumberFormat(MobWorkNumberTextBox.Text);
                    }

            Contacts.WorkStatPhone = "";

            if (CityCountryCodeTextBox.Text.Length == 3)
                if (CityCityCodeTextBox.Text.Length == 2)
                    if (CityNumberTextBox.Text.Length == 9)
                    {
                        Contacts.WorkStatPhone = CityCountryCodeTextBox.Text + CityCityCodeTextBox.Text + UserProfile.PhoneFormatToNumberFormat(CityNumberTextBox.Text);
                    }

            Contacts.WorkExtPhone = WorkExtTextBox.Text;
            Contacts.Skype = SkypeTextBox.Text;
            Contacts.Email = EmailTextBox.Text;
            Contacts.ICQ = ICQTextBox.Text;
            Contacts.NeedSpam = NeedSpamCheckBox.Checked;

            UserProfile.SetContacts(CurrentUserID, Contacts);
        }

        private void xtraScrollableControl1_Click(object sender, EventArgs e)
        {
            ContactsScrollPanel.Focus();
        }

        private void LanguageEditButton_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            LanguageForm LanguageForm = new LanguageForm();

            for (int i = 0; i < LanguagesListBox.ItemCount; i++)
            {
                LanguageForm.ProfLangs.Add(LanguagesListBox.Items[i]);
            }

            TopForm = LanguageForm;

            LanguageForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            LanguagesListBox.Items.Clear();

            if (LanguageForm.ProfLangs.Count == 0)
            {
                LanguageForm.Dispose();
                return;
            }

            for (int i = 0; i < LanguageForm.ProfLangs.Count; i++)
                LanguagesListBox.Items.Add(LanguageForm.ProfLangs[i]);

            LanguageForm.Dispose();
        }

        private void PersonalScrollPanel_Click(object sender, EventArgs e)
        {
            PersonalScrollPanel.Focus();
        }

        private void PhotoEditButton_Click(object sender, EventArgs e)
        {
            PhotoEditForm PhotoEditForm = new PhotoEditForm(ref UserProfile, false, CurrentUserID);

            TopForm = PhotoEditForm;

            PhotoEditForm.ShowDialog();

            TopForm = null;

            PhotoEditForm.Dispose();

            PhotoBox.Image = UserProfile.GetUserPhoto(CurrentUserID);
        }

        private void SavePersonalButton_Click(object sender, EventArgs e)
        {
            UserProfile.PersonalInform PersonalInform = new UserProfile.PersonalInform();

            if (BirthDateTextEdit.Text.Length > 0)
                PersonalInform.BirthDate = BirthDateTextEdit.Text;

            //PersonalInform.PositionID = Convert.ToInt32(PositionComboBox.SelectedValue);
            PersonalInform.Education = EduComboBox.Text;
            PersonalInform.EducationPlace = EduPlace.Text;

            for (int i = 0; i < LanguagesListBox.ItemCount; i++)
                PersonalInform.Language += LanguagesListBox.Items[i].ToString() + "\r\n";

            PersonalInform.DriveA = DriveACheckBox.Checked;
            PersonalInform.DriveB = DriveBCheckBox.Checked;
            PersonalInform.DriveC = DriveCCheckBox.Checked;
            PersonalInform.DriveD = DriveDCheckBox.Checked;
            PersonalInform.DriveE = DriveECheckBox.Checked;
            PersonalInform.CombatArm = CombatArmTextBox.Text;
            PersonalInform.MilitaryRank = MilitaryRankComboBox.Text;

            UserProfile.SetPersonalInform(CurrentUserID, PersonalInform);
            if (cbClientManager.Checked)
                UserProfile.SaveClientManager(CurrentUserID);
            else
                UserProfile.DeleteClientManager(CurrentUserID);
        }

        private void ClearDateButton_Click(object sender, EventArgs e)
        {
            BirthDateTextEdit.Text = "";
        }

        private void ClearEducationButton_Click(object sender, EventArgs e)
        {
            EduComboBox.SelectedIndex = -1;
        }

        private void MilitaryRankClearButton_Click(object sender, EventArgs e)
        {
            MilitaryRankComboBox.SelectedIndex = -1;
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

        private void lightBackButton1_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            TopForm = null;
        }

        private void UserComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            CurrentUserID = Convert.ToInt32(UserComboBox.SelectedValue);

            Contacts = UserProfile.GetContacts(CurrentUserID);

            ContactsToControls();

            PersonalInform = UserProfile.GetPersonalInform(CurrentUserID);

            PersonalToControls();
            cbClientManager.Checked = UserProfile.IsUserClientManager(CurrentUserID);
        }

        private void UserComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                UserComboBox_SelectionChangeCommitted(sender, null);
        }

        private void NoImageButton_Click(object sender, EventArgs e)
        {
            UserProfile.SetUserPhoto(Properties.Resources.NoImage, Convert.ToInt32(UserComboBox.SelectedValue));
        }

        private void btnSaveUsers_Click(object sender, EventArgs e)
        {
            AdminUsersManagement.Save();
            UserProfile.UpdateUsersDataTable();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            if (NameTextBox.Text.Length < 1)
                return;

            AdminUsersManagement.Add(NameTextBox.Text);
            NameTextBox.Clear();
        }
    }
}
