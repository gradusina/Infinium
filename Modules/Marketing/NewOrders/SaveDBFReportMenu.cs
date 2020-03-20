using System;
using System.IO;
using System.Windows.Forms;

namespace Infinium
{
    public partial class SaveDBFReportMenu : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        public int Result = 1;
        public bool Old = false;
        public bool InMutualSettlement = false;
        public string Notes = string.Empty;
        public string SaveFilePath = string.Empty;
        int FormEvent = 0;
        string ReportName = string.Empty;

        Form MainForm = null;

        public SaveDBFReportMenu(Form tMainForm, string sReportName)
        {
            MainForm = tMainForm;
            ReportName = sReportName;
            InitializeComponent();
            string CurrentMonthName = DateTime.Now.ToString("dd-MM-yyyy");
            label1.Text = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Infinium/1C data/" + CurrentMonthName;
            label3.Text = Security.DBFSaveFilePath;
        }

        private void ReportMenuOKButton_Click(object sender, EventArgs e)
        {
            InMutualSettlement = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void ReportMenuCancelButton_Click(object sender, EventArgs e)
        {
            InMutualSettlement = false;
            FormEvent = eClose;
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

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "(*" + Path.GetExtension(ReportName) + ")|*" + Path.GetExtension(ReportName);
            saveFileDialog1.FileName = ReportName;
            if (Security.DBFSaveFilePath.Length > 0)
                saveFileDialog1.InitialDirectory = Security.DBFSaveFilePath;

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Security.SetDBFSaveFilePath(Path.GetDirectoryName(saveFileDialog1.FileName));
                Security.DBFSaveFilePath = Path.GetDirectoryName(saveFileDialog1.FileName);
                label3.Text = Security.DBFSaveFilePath;
            }
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (kryptonCheckSet1.CheckedButton == SendReportCheck)
            {
                label1.Visible = true;
                label3.Visible = false;
                kryptonButton1.Visible = false;
            }
            if (kryptonCheckSet1.CheckedButton == NotSendReportCheck)
            {
                label1.Visible = false;
                label3.Visible = true;
                kryptonButton1.Visible = true;
            }
        }

        private void btnSaveReport_Click(object sender, EventArgs e)
        {
            if (kryptonCheckSet1.CheckedButton == SendReportCheck)
            {
                SaveFilePath = label1.Text;
            }
            if (kryptonCheckSet1.CheckedButton == NotSendReportCheck)
            {
                SaveFilePath = label3.Text;
                Security.DBFSaveFilePath = label3.Text;
            }
            Notes = tbNotes.Text;

            InMutualSettlement = true;
            Result = 1;
            Old = cbOld.Checked;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnNotSaveReport_Click(object sender, EventArgs e)
        {
            if (kryptonCheckSet1.CheckedButton == SendReportCheck)
            {
                SaveFilePath = label1.Text;
            }
            if (kryptonCheckSet1.CheckedButton == NotSendReportCheck)
            {
                SaveFilePath = label3.Text;
                Security.DBFSaveFilePath = label3.Text;
            }

            InMutualSettlement = false;
            Result = 2;
            Old = cbOld.Checked;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void btnCancelReport_Click(object sender, EventArgs e)
        {
            InMutualSettlement = true;
            Result = 3;
            Old = cbOld.Checked;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }
    }
}
