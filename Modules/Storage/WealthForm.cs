using Infinium.Store;

using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class WealthForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        bool MoveFromPermits = false;
        int FormEvent = 0;

        LightStartForm LightStartForm;

        Form MainForm;
        Form TopForm = null;

        DataTable TableMeasures, TableCurrency, TableUsers;

        AddWealthForm AddWealthForm;

        ViewConnectUnloads ViewConnectUnloads;
        ConnectUnloads ConnectUnloads;
        LightNotes LightNotes;

        public WealthForm(LightStartForm tLightStartForm)
        {
            TableMeasures = new DataTable();
            TableCurrency = new DataTable();
            TableUsers = new DataTable();

            InitializeComponent();

            LightStartForm = tLightStartForm;


            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;

            //--------------------GoodGrid---------------------->       
            GoodGrid.Columns["SubjectName"].HeaderText = "Наименование";
            GoodGrid.Columns["Count"].HeaderText = "Кол-во";
            GoodGrid.Columns["MeasureID"].HeaderText = "Ед.измерения";
            GoodGrid.Columns["Cost"].HeaderText = "Цена";
            GoodGrid.Columns["CurrencyTypeID"].HeaderText = "Валюта";
            GoodGrid.Columns["GoodsID"].Visible = false;
            GoodGrid.Columns["UnloadID"].Visible = false;
            GoodGrid.Columns["MeasureID"].Visible = false;
            GoodGrid.Columns["CurrencyTypeID"].Visible = false;

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

            //----------------UnloadDataGrid------------------>
            UnloadDataGrid.Columns["UserName"].HeaderText = "ФИО";
            UnloadDataGrid.Columns["UnloadID"].HeaderText = "№\r\nнакладной";
            UnloadDataGrid.Columns["UnloadDateTime"].HeaderText = "Дата возврата";
            UnloadDataGrid.Columns["OrderedDateTime"].HeaderText = "Когда вернул";
            UnloadDataGrid.Columns["NeedReturnObject"].HeaderText = "С возвратом";
            UnloadDataGrid.Columns["OutObject"].HeaderText = "Проверка на\r\nпроходной";
            UnloadDataGrid.Columns["FactReturnDateTime"].HeaderText = "Фактическая\r\nдата возврата";
            UnloadDataGrid.Columns["ReturnObject"].HeaderText = "Подтвержд.\r\nвозврата";
            UnloadDataGrid.Columns["Notes"].HeaderText = "Примечание";

            UnloadDataGrid.Columns["UserID"].Visible = false;
            UnloadDataGrid.Columns["ResponsibleUserID"].Visible = false;
            UnloadDataGrid.Columns["OrderedDateTime"].Visible = false;
            UnloadDataGrid.Columns["ReturnNotes"].Visible = false;
            UnloadDataGrid.Columns["Notes"].Visible = false;

            UnloadDataGrid.Columns["UnloadID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UnloadDataGrid.Columns["UnloadID"].Width = 100;
            UnloadDataGrid.Columns["UserName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            UnloadDataGrid.Columns["UnloadDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UnloadDataGrid.Columns["UnloadDateTime"].Width = 130;
            UnloadDataGrid.Columns["FactReturnDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UnloadDataGrid.Columns["FactReturnDateTime"].Width = 130;
            UnloadDataGrid.Columns["NeedReturnObject"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UnloadDataGrid.Columns["NeedReturnObject"].Width = 120;
            UnloadDataGrid.Columns["ReturnObject"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UnloadDataGrid.Columns["ReturnObject"].Width = 120;
            UnloadDataGrid.Columns["OutObject"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UnloadDataGrid.Columns["OutObject"].Width = 120;

            DataGridViewComboBoxColumn UsersColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ResponsibleUser",
                DataSource = TableUsers,
                DisplayMember = "Name",
                ValueMember = "UserID",
                DataPropertyName = "ResponsibleUserID",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            UnloadDataGrid.Columns.Add(UsersColumn);
            UnloadDataGrid.Columns["ResponsibleUser"].HeaderText = "Ответственный";
            UnloadDataGrid.Columns["ResponsibleUser"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            UnloadDataGrid.Columns["UnloadID"].DisplayIndex = 0;
            UnloadDataGrid.Columns["UserName"].DisplayIndex = 1;
            UnloadDataGrid.Columns["ResponsibleUser"].DisplayIndex = 2;
            UnloadDataGrid.Columns["UnloadDateTime"].DisplayIndex = 3;
            UnloadDataGrid.Columns["FactReturnDateTime"].DisplayIndex = 4;
            UnloadDataGrid.Columns["NeedReturnObject"].DisplayIndex = 5;
            UnloadDataGrid.Columns["ReturnObject"].DisplayIndex = 6;
            UnloadDataGrid.Columns["OutObject"].DisplayIndex = 7;
        }

        public WealthForm(Form tMainForm)
        {
            TableMeasures = new DataTable();
            TableCurrency = new DataTable();
            TableUsers = new DataTable();

            InitializeComponent();

            MainForm = tMainForm;
            MoveFromPermits = true;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();

            while (!SplashForm.bCreated) ;

            //--------------------GoodGrid---------------------->       
            GoodGrid.Columns["SubjectName"].HeaderText = "Наименование";
            GoodGrid.Columns["Count"].HeaderText = "Кол-во";
            GoodGrid.Columns["MeasureID"].HeaderText = "Ед.измерения";
            GoodGrid.Columns["Cost"].HeaderText = "Цена";
            GoodGrid.Columns["CurrencyTypeID"].HeaderText = "Валюта";
            GoodGrid.Columns["GoodsID"].Visible = false;
            GoodGrid.Columns["UnloadID"].Visible = false;
            GoodGrid.Columns["MeasureID"].Visible = false;
            GoodGrid.Columns["CurrencyTypeID"].Visible = false;

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

            //----------------UnloadDataGrid------------------>
            UnloadDataGrid.Columns["UserName"].HeaderText = "ФИО";
            UnloadDataGrid.Columns["UnloadID"].HeaderText = "№\r\nнакладной";
            UnloadDataGrid.Columns["UnloadDateTime"].HeaderText = "Дата возврата";
            UnloadDataGrid.Columns["OrderedDateTime"].HeaderText = "Когда вернул";
            UnloadDataGrid.Columns["NeedReturnObject"].HeaderText = "С возвратом";
            UnloadDataGrid.Columns["OutObject"].HeaderText = "Проверка на\r\nпроходной";
            UnloadDataGrid.Columns["FactReturnDateTime"].HeaderText = "Фактическая\r\nдата возврата";
            UnloadDataGrid.Columns["ReturnObject"].HeaderText = "Подтвержд.\r\nвозврата";
            UnloadDataGrid.Columns["Notes"].HeaderText = "Примечание";

            UnloadDataGrid.Columns["UserID"].Visible = false;
            UnloadDataGrid.Columns["ResponsibleUserID"].Visible = false;
            UnloadDataGrid.Columns["OrderedDateTime"].Visible = false;
            UnloadDataGrid.Columns["ReturnNotes"].Visible = false;
            UnloadDataGrid.Columns["Notes"].Visible = false;

            UnloadDataGrid.Columns["UnloadID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UnloadDataGrid.Columns["UnloadID"].Width = 100;
            UnloadDataGrid.Columns["UserName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            UnloadDataGrid.Columns["UnloadDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UnloadDataGrid.Columns["UnloadDateTime"].Width = 130;
            UnloadDataGrid.Columns["FactReturnDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UnloadDataGrid.Columns["FactReturnDateTime"].Width = 130;
            UnloadDataGrid.Columns["NeedReturnObject"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UnloadDataGrid.Columns["NeedReturnObject"].Width = 120;
            UnloadDataGrid.Columns["ReturnObject"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UnloadDataGrid.Columns["ReturnObject"].Width = 120;
            UnloadDataGrid.Columns["OutObject"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UnloadDataGrid.Columns["OutObject"].Width = 120;

            DataGridViewComboBoxColumn UsersColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ResponsibleUser",
                DataSource = TableUsers,
                DisplayMember = "Name",
                ValueMember = "UserID",
                DataPropertyName = "ResponsibleUserID",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            UnloadDataGrid.Columns.Add(UsersColumn);
            UnloadDataGrid.Columns["ResponsibleUser"].HeaderText = "Ответственный";
            UnloadDataGrid.Columns["ResponsibleUser"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            UnloadDataGrid.Columns["UnloadID"].DisplayIndex = 0;
            UnloadDataGrid.Columns["UserName"].DisplayIndex = 1;
            UnloadDataGrid.Columns["ResponsibleUser"].DisplayIndex = 2;
            UnloadDataGrid.Columns["UnloadDateTime"].DisplayIndex = 3;
            UnloadDataGrid.Columns["FactReturnDateTime"].DisplayIndex = 4;
            UnloadDataGrid.Columns["NeedReturnObject"].DisplayIndex = 5;
            UnloadDataGrid.Columns["ReturnObject"].DisplayIndex = 6;
            UnloadDataGrid.Columns["OutObject"].DisplayIndex = 7;
        }

        private void LightNotesForm_Shown(object sender, EventArgs e)
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
                        if (!MoveFromPermits)
                            LightStartForm.CloseForm(this);
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        if (!MoveFromPermits)
                            LightStartForm.HideForm(this);
                        else
                        {
                            MainForm.Activate();
                            this.Hide();
                        }
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
                        if (!MoveFromPermits)
                            LightStartForm.CloseForm(this);
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        if (!MoveFromPermits)
                            LightStartForm.HideForm(this);
                        else
                        {
                            MainForm.Activate();
                            this.Hide();
                        }
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
            LightNotes = new LightNotes(Security.CurrentUserID);

            ConnectUnloads = new ConnectUnloads();
            TableUsers = ConnectUnloads.TableUsers();
            TableMeasures = ConnectUnloads.TableMeasures();
            TableCurrency = ConnectUnloads.TableCurrency();

            ViewConnectUnloads = new ViewConnectUnloads(ref UnloadDataGrid, ref GoodGrid);
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

        private void AddButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            ConnectUnloads.StoreDataTable.Clear();
            AddWealthForm = new AddWealthForm(ref TopForm, ref ConnectUnloads, ref TableMeasures, ref TableCurrency, ref TableUsers);

            TopForm = AddWealthForm;

            AddWealthForm.ShowDialog();

            AddWealthForm.Close();
            AddWealthForm.Dispose();

            TopForm = null;

            ViewConnectUnloads.CreateAndFill();
        }

        private void UnloadDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (ViewConnectUnloads != null && UnloadDataGrid.SelectedRows.Count == 1)
            {
                ViewConnectUnloads.UpdateGood(UnloadDataGrid.SelectedRows[0].Cells["UnloadID"].Value.ToString());
                NotesTextBox.Text = UnloadDataGrid.SelectedRows[0].Cells["Notes"].Value.ToString();
            }
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создание документа Excel.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated)

                if (UnloadDataGrid.SelectedRows.Count == 1)
                    ViewConnectUnloads.ToExcel(Convert.ToInt32(UnloadDataGrid.SelectedRows[0].Cells["UnloadID"].Value));

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void ReturnButton_Click(object sender, EventArgs e)
        {
            if (UnloadDataGrid.SelectedRows.Count != 0)
                if (UnloadDataGrid.SelectedRows[0].Cells["UnloadDateTime"].Value.ToString() == "")
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Предмет не надо возвращать",
                    "Ошибка");
                }
                else
                    if ((bool)UnloadDataGrid.SelectedRows[0].Cells["ReturnObject"].Value == false)
                {
                    PhantomForm PhantomForm = new PhantomForm();
                    PhantomForm.Show();

                    ReturnWealthForm ReturnWealthForm = new ReturnWealthForm();

                    TopForm = ReturnWealthForm;
                    ReturnWealthForm.ShowDialog();

                    if (ReturnWealthForm.ok)

                        ViewConnectUnloads.ReturnObject(ReturnWealthForm.NotesTextBox.Text);

                    PhantomForm.Close();
                    PhantomForm.Dispose();

                    TopForm = null;

                    ReturnWealthForm.Dispose();
                }
                else
                {
                    Infinium.LightMessageBox.Show(ref TopForm, false, "Предмет уже вернули",
                        "Ошибка");
                }

            ViewConnectUnloads.CreateAndFill();
        }

        private void DetachButton_Click(object sender, EventArgs e)
        {

            if (UnloadDataGrid.SelectedRows.Count != 0)

                if (Infinium.LightMessageBox.Show(ref TopForm, true, "Вы уверены, что хотите удалить запись?", "Удаление"))
                {
                    ViewConnectUnloads.DeleteObject();
                    ViewConnectUnloads.CreateAndFill();
                }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void UnloadDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                UnloadDataGrid.Rows[e.RowIndex].Selected = true;
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void cmiBindToPermit_Click(object sender, EventArgs e)
        {
            if (UnloadDataGrid.SelectedRows.Count == 0)
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            bool BindingOk = false;
            int UnloadID = Convert.ToInt32(UnloadDataGrid.SelectedRows[0].Cells["UnloadID"].Value);
            PermitsForm PermitsForm = new PermitsForm(this, UnloadID);

            TopForm = PermitsForm;

            PermitsForm.ShowDialog();
            BindingOk = PermitsForm.BindingOk;
            PermitsForm.Close();
            PermitsForm.Dispose();

            TopForm = null;
            if (BindingOk)
                InfiniumTips.ShowTip(this, 50, 85, "Привязка выполнена", 1700);
        }

        private void WealthForm_Load(object sender, EventArgs e)
        {
            if (MoveFromPermits)
                MinimizeButton.Visible = false;
        }
    }
}
