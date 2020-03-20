using Infinium.Store;

using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AddWealthForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        int FormEvent = 0;

        DataTable TableMeasures, TableCurrency, TableUsers;

        Form TopForm = null;

        ConnectUnloads ConnectUnloads;

        LightNotes LightNotes;

        public AddWealthForm(ref Form tTopForm, ref ConnectUnloads tConnectUnloads, ref DataTable tTableMeasures, ref DataTable tTableCurrency, ref DataTable tTableUsers)
        {
            TableMeasures = tTableMeasures;
            TableCurrency = tTableCurrency;
            TableUsers = tTableUsers;
            ConnectUnloads = tConnectUnloads;

            InitializeComponent();
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;

            GoodGrid.DataSource = ConnectUnloads.StoreBindingSource;
            GoodGrid.Columns["SubjectName"].HeaderText = "Наименование";
            GoodGrid.Columns["Count"].HeaderText = "Кол-во";
            GoodGrid.Columns["Cost"].HeaderText = "Цена";
            GoodGrid.Columns["MeasureID"].Visible = false;
            GoodGrid.Columns["CurrencyTypeID"].Visible = false;
            GoodGrid.Columns["GoodsID"].Visible = false;
            GoodGrid.Columns["UnloadID"].Visible = false;

            GoodGrid.Columns["SubjectName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            GoodGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            GoodGrid.Columns["Count"].MinimumWidth = 150;
            GoodGrid.Columns["Cost"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            GoodGrid.Columns["Cost"].MinimumWidth = 150;

            DataGridViewComboBoxColumn MeasureColumn = new DataGridViewComboBoxColumn()
            {
                Name = "Measure",
                DataSource = TableMeasures,
                DisplayMember = "Measure",
                ValueMember = "MeasureID",
                DataPropertyName = "MeasureID",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            GoodGrid.Columns.Add(MeasureColumn);
            GoodGrid.Columns["Measure"].HeaderText = "Ед.измерения";
            GoodGrid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            GoodGrid.Columns["Measure"].MinimumWidth = 150;

            DataGridViewComboBoxColumn CurrencyColumn = new DataGridViewComboBoxColumn()
            {
                Name = "CurrencyType",
                DataSource = TableCurrency,
                DisplayMember = "CurrencyType",
                ValueMember = "CurrencyTypeID",
                DataPropertyName = "CurrencyTypeID",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            GoodGrid.Columns.Add(CurrencyColumn);
            GoodGrid.Columns["CurrencyType"].HeaderText = "Валюта";
            GoodGrid.Columns["CurrencyType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            GoodGrid.Columns["CurrencyType"].MinimumWidth = 150;

            GoodGrid.Columns["SubjectName"].DisplayIndex = 0;
            GoodGrid.Columns["Count"].DisplayIndex = 1;
            GoodGrid.Columns["Measure"].DisplayIndex = 2;
            GoodGrid.Columns["Cost"].DisplayIndex = 3;
            GoodGrid.Columns["CurrencyType"].DisplayIndex = 4;
        }


        private void LightNotesForm_Shown(object sender, EventArgs e)
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

        private void Initialize()
        {
            LightNotes = new LightNotes(Security.CurrentUserID);

            DateFromPicker.Enabled = false;
            FIOTextBox.Enabled = false;

            TableUsers = ConnectUnloads.TableUsers();
            TableMeasures = ConnectUnloads.TableMeasures();
            TableCurrency = ConnectUnloads.TableCurrency();

            FIOComboBox.DataSource = TableUsers;
            FIOComboBox.DisplayMember = "Name";
            FIOComboBox.ValueMember = "UserID";

            MeasureComboBox.DataSource = TableMeasures;
            MeasureComboBox.DisplayMember = "Measure";
            MeasureComboBox.ValueMember = "MeasureID";

            CurrencyComboBox.DataSource = TableCurrency;
            CurrencyComboBox.DisplayMember = "CurrencyType";
            CurrencyComboBox.ValueMember = "CurrencyTypeID";
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
        private void ReturnCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DateFromPicker.Enabled = ReturnCheckBox.Checked;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            int Count = 0;

            if (ObjectNameTextBox.Text == "")
                Infinium.LightMessageBox.Show(ref TopForm, false, "Введите название!",
                    "Ошибка");
            else
            {

                try
                {
                    Count = Convert.ToInt32(CountTextBox.Text);
                }
                catch
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Некорректное кол-во!",
                        "Ошибка");
                    return;
                }

                try
                {
                    Convert.ToDecimal(CostTextBox.Text);
                }
                catch
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Некорректно задана цена!",
                       "Ошибка");
                    return;
                }

                ConnectUnloads.AddGoods(ObjectNameTextBox.Text, Count, CostTextBox.Text, MeasureComboBox.SelectedValue.ToString(), CurrencyComboBox.SelectedValue.ToString());

                ObjectNameTextBox.Clear();
                CountTextBox.Clear();
                CostTextBox.Clear();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string UserName;
            if (YesFIOCheckBox.Checked)
                UserName = FIOComboBox.Text;
            else
                UserName = FIOTextBox.Text;

            if (YesFIOCheckBox.Checked | NoFIOCheckBox.Checked)
            {

                ConnectUnloads.Save(YesFIOCheckBox.Checked, Convert.ToInt32(FIOComboBox.SelectedValue), UserName, ReturnCheckBox.Checked, DateFromPicker.Value, NotesTextBox.Text);

                FormEvent = eClose;
                AnimateTimer.Enabled = true;
            }
            else
            {
                Infinium.LightMessageBox.Show(ref TopForm, false, "Пользователь не указан!",
                    "Ошибка");
            }
        }

        private void YesFIOCheckBox_Click(object sender, EventArgs e)
        {
            YesFIOCheckBox.Checked = true;
            NoFIOCheckBox.Checked = false;
            FIOComboBox.Enabled = true;
            FIOTextBox.Enabled = false;
        }

        private void NoFIOCheckBox_Click(object sender, EventArgs e)
        {
            YesFIOCheckBox.Checked = false;
            NoFIOCheckBox.Checked = true;
            FIOTextBox.Enabled = true;
            FIOComboBox.Enabled = false;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (GoodGrid.SelectedRows.Count != 0)
                ConnectUnloads.StoreBindingSource.RemoveCurrent();
        }
    }
}
