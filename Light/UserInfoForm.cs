using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Infinium
{
    public partial class UserInfoForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        Form TopForm = null;

        UserProfile UserProfile;

        UserProfile.Contacts Contacts;
        UserProfile.PersonalInform PersonalInform;

        string CurrentName;
        int CurrentUserID = -1;

        public UserInfoForm(string UserName)
        {
            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            UserProfile = new Infinium.UserProfile();

            CurrentUserID = UserProfile.GetUserByName(UserName);
            Contacts = UserProfile.GetContacts(CurrentUserID);
            PersonalInform = UserProfile.GetPersonalInform(CurrentUserID);

            CurrentName = UserName;

            Initialize();

            SetInfo();
        }

        private string OneLineToTwo(string text)
        {
            if (text.Length == 0)
                return "";

            Graphics G = this.CreateGraphics();

            if (G.MeasureString(text, this.PositionLabel.Font).Width > kryptonBorderEdge1.Width)
            {
                int LastSpace = GetLastSpace(text);

                text = text.Insert(LastSpace + 1, "\n");
            }

            G.Dispose();

            return text;
        }

        private int GetLastSpace(string Text)
        {
            int LastSpace = 0;

            for (int i = Text.Length - 1; i >= 0; i--)
            {
                if (Text[i] == ' ')
                {
                    LastSpace = i;
                    break;
                }
            }

            if (LastSpace == 0)//no spaces was found
            {
                for (int i = Text.Length - 1; i >= 0; i--)
                {
                    if (Text[i] == '.' || Text[i] == ',' || Text[i] == ';' || Text[i] == ':')
                    {
                        LastSpace = i;
                        break;
                    }
                }
            }

            return LastSpace;
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

        private string GetLanguages(string Language)
        {
            if (Language.Length == 0)
                return "";

            int CurrentSize = 0;

            Graphics G = this.CreateGraphics();

            string temp = "";
            string res = "";

            for (int i = 0; i < Language.Length; i++)
            {
                if (Language[i] != '\r')
                    temp += Language[i];
                else
                {
                    if (CurrentSize + G.MeasureString(temp, LanguagesLabel.Font).Width < kryptonBorderEdge1.Width)
                    {
                        if (i + 1 < Language.Length - 1)
                            temp += ", ";

                        i += 1;

                        res += temp;

                        CurrentSize += Convert.ToInt32(G.MeasureString(temp, LanguagesLabel.Font).Width);

                        temp = "";

                        continue;
                    }

                    if (i + 1 < Language.Length)
                        temp += ", ";

                    temp += Language[i];
                    temp += Language[i + 1];

                    i += 1;

                    res += temp;

                    CurrentSize = 0;

                    temp = "";
                }
            }

            G.Dispose();

            return res;
        }

        private string GetDrive(UserProfile.PersonalInform PI)
        {
            string Drive = "";

            if (PI.DriveA)
                Drive += "A";

            if (PI.DriveB)
                if (Drive.Length > 0)
                    Drive += ", B";
                else
                    Drive += "B";

            if (PI.DriveC)
                if (Drive.Length > 0)
                    Drive += ", C";
                else
                    Drive += "C";

            if (PI.DriveD)
                if (Drive.Length > 0)
                    Drive += ", D";
                else
                    Drive += "D";

            if (PI.DriveE)
                if (Drive.Length > 0)
                    Drive += ", E";
                else
                    Drive += "E";

            if (Drive.Length == 0)
                Drive = "нет\\не указано";

            return Drive;
        }

        private string GetAge(string BirthDate)
        {
            if (BirthDate.Length == 0)
                return "не указан";

            DateTime today = Security.GetCurrentDate();
            int age = today.Year - Convert.ToDateTime(BirthDate).Year;
            if (Convert.ToDateTime(BirthDate) > today.AddYears(-age)) age--;

            return age.ToString();
        }

        private string GetCombat(string CombatArm, string Rate)
        {
            if (CombatArm.Length == 0)
                return "не служил(а)\\не указано";

            string res = "";

            Graphics G = this.CreateGraphics();

            if (G.MeasureString(CombatArm, MilitaryLabel.Font).Width > kryptonBorderEdge1.Width)
            {
                string temp = "";

                for (int i = 0; i < CombatArm.Length; i++)
                {
                    temp += CombatArm[i];

                    if (Convert.ToInt32(G.MeasureString(temp, MilitaryLabel.Font).Width) >= kryptonBorderEdge1.Width - 4)
                    {
                        res += temp + "...";
                        break;
                    }
                }
            }
            else
                res += CombatArm;

            if (Rate.Length == 0)
                return res;

            res += "\n";

            if (G.MeasureString(Rate, MilitaryLabel.Font).Width > kryptonBorderEdge1.Width)
            {
                string temp = "";

                for (int i = 0; i < Rate.Length; i++)
                {
                    temp += Rate[i];

                    if (Convert.ToInt32(G.MeasureString(temp, MilitaryLabel.Font).Width) >= kryptonBorderEdge1.Width - 4)
                    {
                        res += temp + "...";
                        break;
                    }
                }
            }
            else
                res += Rate;

            G.Dispose();

            return res;
        }

        private void SetInfo()
        {
            PhotoBox.Image = UserProfile.GetUserPhoto(CurrentUserID);

            NameLabel.Text = CurrentName;

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

            DepartmentLabel.Text = OneLineToTwo(UserProfile.GetDepartmentName(PersonalInform.DepartmentID));

            if (PersonalInform.Education.Length == 0)
                EducationLabel.Text = "не указано";
            else
                EducationLabel.Text = PersonalInform.Education;

            if (PersonalInform.EducationPlace.Length == 0)
                EduPlaceLabel.Text = "не указано";
            else
                EduPlaceLabel.Text = OneLineToTwo(PersonalInform.EducationPlace);

            if (PersonalInform.BirthDate.Length == 0)
                BirthDateLabel.Text = "не указана";
            else
                BirthDateLabel.Text = PersonalInform.BirthDate;

            AgeLabel.Text = GetAge(PersonalInform.BirthDate).ToString();

            if (PersonalInform.Language.Length == 0)
                LanguagesLabel.Text = "нет\\не указано";
            else
                LanguagesLabel.Text = GetLanguages(PersonalInform.Language);

            DriveLabel.Text = GetDrive(PersonalInform);
            MilitaryLabel.Text = GetCombat(PersonalInform.CombatArm, PersonalInform.MilitaryRank);

            MobilePersonalLabel.Text = LightUsers.GetPhoneFormat(Contacts.PersonalMobilePhone);
            MobileWorkLabel.Text = LightUsers.GetPhoneFormat(Contacts.WorkMobilePhone);
            CityWorkPhoneLabel.Text = LightUsers.GetPhoneFormat(Contacts.WorkStatPhone);

            if (Contacts.WorkExtPhone.Length == 0)
                WorkExtPhoneLabel.Text = "не указан";
            else
                WorkExtPhoneLabel.Text = Contacts.WorkExtPhone;

            if (Contacts.Skype.Length == 0)
                SkypeLabel.Text = "не указан";
            else
                SkypeLabel.Text = Contacts.Skype;

            if (Contacts.Email.Length == 0)
                MailLabel.Text = "не указан";
            else
                MailLabel.Text = Contacts.Email;

            if (Contacts.ICQ.Length == 0)
                ICQLabel.Text = "не указан";
            else
                ICQLabel.Text = Contacts.ICQ;
        }

        private void UserInfoForm_Shown(object sender, EventArgs e)
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
