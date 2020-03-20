using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class PersonalSettingsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        LightStartForm LightStartForm;


        Form TopForm = null;

        UserProfile.Contacts Contacts;
        UserProfile.PersonalInform PersonalInform;

        UserProfile UserProfile;

        Security Security;

        public PersonalSettingsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            LightStartForm = tLightStartForm;

            UserProfile = new UserProfile();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            TabNameLabel.Text = MainSelectPanelCheckSet.CheckedButton.Text;

            Contacts = UserProfile.GetContacts();

            ContactsToControls();

            PersonalInform = UserProfile.GetPersonalInform();

            Security = new Infinium.Security();


            label1.Text = "Infinium. Профиль пользователя. " + UserProfile.GetUserName();

            //PositionComboBox.DataSource = UserProfile.PositionsBindingSource;
            //PositionComboBox.DisplayMember = "Position";
            //PositionComboBox.ValueMember = "PositionID";

            PersonalToControls();

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
            PhotoBox.Image = UserProfile.GetUserPhoto();

            BirthDateTextEdit.Text = PersonalInform.BirthDate;
            //PositionComboBox.SelectedValue = PersonalInform.PositionID;

            LanguagesListBox.Items.Clear();
            EduComboBox.SelectedIndex = -1;
            MilitaryRankComboBox.SelectedIndex = -1;

            if (PersonalInform.Education.Length > 0)
                EduComboBox.SelectedIndex = EduComboBox.Items.IndexOf(PersonalInform.Education);

            EduPlace.Text = PersonalInform.EducationPlace;


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

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SuccessLabel.Visible = false;
            OldPassLabel.Visible = false;
            NewPassLabel.Visible = false;
            RepeatNewPassLabel.Visible = false;

            OldAuthLabel.Visible = false;
            NewAuthLabel.Visible = false;
            RepeatNewAuthLabel.Visible = false;

            if (CurrentPasswordTextBox.Text.Length > 0 || NewPasswordTextBox.Text.Length > 0 || AcceptNewPasswordTextBox.Text.Length > 0)
            {
                if (CurrentPasswordTextBox.Text.Length == 0)
                {
                    OldPassLabel.Text = "Введите старый пароль";
                    OldPassLabel.Visible = true;
                    return;
                }

                if (Security.CheckPass(CurrentPasswordTextBox.Text) == false)
                {
                    OldPassLabel.Text = "Пароль неверный";
                    OldPassLabel.Visible = true;
                    return;
                }

                if (NewPasswordTextBox.Text.Length == 0)
                {
                    NewPassLabel.Text = "Введите новый пароль";
                    NewPassLabel.Visible = true;
                    return;
                }

                if (NewPasswordTextBox.Text.Length < 4)
                {
                    NewPassLabel.Text = "Длина пароля меньше 4 символов";
                    NewPassLabel.Visible = true;
                    return;
                }

                if (AcceptNewPasswordTextBox.Text.Length == 0)
                {
                    RepeatNewPassLabel.Text = "Повторите новый пароль";
                    RepeatNewPassLabel.Visible = true;
                    return;
                }

                if (AcceptNewPasswordTextBox.Text != NewPasswordTextBox.Text)
                {
                    RepeatNewPassLabel.Text = "Пароли не совпадают";
                    RepeatNewPassLabel.Visible = true;
                    return;
                }

                Security.ChangePassword(CurrentPasswordTextBox.Text, NewPasswordTextBox.Text);

                SuccessLabel.Visible = true;

                CurrentPasswordTextBox.Clear();
                NewPasswordTextBox.Clear();
                AcceptNewPasswordTextBox.Clear();
            }


            //authorization code
            if (CurrentAuthTextBox.Text.Length > 0 || NewAuthTextBox.Text.Length > 0 || AcceptNewAuthTextBox.Text.Length > 0)
            {
                if (CurrentAuthTextBox.Text.Length == 0)
                {
                    if (Security.CheckAuth(CurrentAuthTextBox.Text) == false)
                    {
                        if (CurrentAuthTextBox.Text.Length == 0)
                        {
                            OldAuthLabel.Text = "Введите старый авторизационный код";
                            OldAuthLabel.Visible = true;
                            return;
                        }
                    }
                }
                else
                {
                    if (Security.CheckAuth(CurrentAuthTextBox.Text) == false)
                    {
                        OldAuthLabel.Text = "Авторизационный код неверный";
                        OldAuthLabel.Visible = true;
                        return;
                    }
                }

                if (NewAuthTextBox.Text.Length == 0)
                {
                    NewAuthLabel.Text = "Введите новый авторизационный код";
                    NewAuthLabel.Visible = true;
                    return;
                }

                if (NewAuthTextBox.Text.Length < 6)
                {
                    NewAuthLabel.Text = "Длина авторизационного кода меньше 6 символов";
                    NewAuthLabel.Visible = true;
                    return;
                }

                if (AcceptNewAuthTextBox.Text.Length == 0)
                {
                    RepeatNewAuthLabel.Text = "Повторите новый авторизационный код";
                    RepeatNewAuthLabel.Visible = true;
                    return;
                }

                if (AcceptNewAuthTextBox.Text != NewAuthTextBox.Text)
                {
                    RepeatNewAuthLabel.Text = "Авторизационные коды не совпадают";
                    RepeatNewAuthLabel.Visible = true;
                    return;
                }

                Security.ChangeAuth(NewAuthTextBox.Text);

                SuccessLabel.Visible = true;

                CurrentAuthTextBox.Clear();
                NewAuthTextBox.Clear();
                AcceptNewAuthTextBox.Clear();
            }
        }


        private void MainSelectPanelCheckSet_CheckedButtonChanged(object sender, EventArgs e)
        {
            TabNameLabel.Text = MainSelectPanelCheckSet.CheckedButton.Text;

            if (MainSelectPanelCheckSet.CheckedButton.Name == "ChangePassSelectButton")
                ChangePasswordPanel.BringToFront();

            if (MainSelectPanelCheckSet.CheckedButton.Name == "ContactInformationSelectButton")
                ContactInformationPanel.BringToFront();

            if (MainSelectPanelCheckSet.CheckedButton.Name == "PersonalSelectButton")
                PersonalPanel.BringToFront();

            if (MainSelectPanelCheckSet.CheckedButton.Name == "SettingsButton")
                SettingsPanel.BringToFront();
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
            if (ICQTextBox.Text[0] == ' ')
                Contacts.ICQ = "";
            else
                Contacts.ICQ = ICQTextBox.Text;
            Contacts.NeedSpam = NeedSpamCheckBox.Checked;

            UserProfile.SetContacts(Contacts);
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
            PhotoEditForm PhotoEditForm = new PhotoEditForm(ref UserProfile, true, -1);

            TopForm = PhotoEditForm;

            PhotoEditForm.ShowDialog();

            TopForm = null;

            PhotoEditForm.Dispose();

            PhotoBox.Image = UserProfile.GetUserPhoto();
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

            UserProfile.SetPersonalInform(PersonalInform);
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

        private void lightBackButton2_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            GridsBackColorForm GridsBackColorForm = new GridsBackColorForm();

            TopForm = GridsBackColorForm;

            GridsBackColorForm.ShowDialog();

            GridsBackColorForm.Dispose();

            TopForm = null;
        }
    }
}
