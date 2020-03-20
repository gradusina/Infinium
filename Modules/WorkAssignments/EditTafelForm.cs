using Infinium.Modules.WorkAssignments;

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class EditTafelForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        bool NeedSplash = false;
        public bool bPrint = false;

        int FormEvent = 0;

        Form MainForm = null;
        Form TopForm = null;

        TafelManager TafelManager;

        public EditTafelForm(Form tMainForm, ref TafelManager tTafelManager)
        {
            InitializeComponent();
            MainForm = tMainForm;
            TafelManager = tTafelManager;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            while (!SplashForm.bCreated) ;
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
                        NeedSplash = false;
                        MainForm.Activate();
                        this.Hide();
                    }

                    return;
                }

                if (FormEvent == eShow)
                {
                    NeedSplash = true;
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
                        NeedSplash = false;
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
                    NeedSplash = true;
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                }

                return;
            }
        }

        private void EditTafelForm_Shown(object sender, EventArgs e)
        {
            NeedSplash = true;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void dgvMainOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (TafelManager == null)
                return;
            int GroupType = 0;
            int MainOrderID = 0;
            if (dgvMainOrders.SelectedRows.Count != 0 && dgvMainOrders.SelectedRows[0].Cells["GroupType"].Value != DBNull.Value)
                GroupType = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["GroupType"].Value);
            if (dgvMainOrders.SelectedRows.Count != 0 && dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value != DBNull.Value)
                MainOrderID = Convert.ToInt32(dgvMainOrders.SelectedRows[0].Cells["MainOrderID"].Value);

            if (NeedSplash)
            {
                NeedSplash = false;
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();
                while (!SplashWindow.bSmallCreated) ;
                TafelManager.FilterOrdersByMainOrder(GroupType, MainOrderID);
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
                NeedSplash = true;
            }
            else
                TafelManager.FilterOrdersByMainOrder(GroupType, MainOrderID);
        }

        private void EditTafelForm_Load(object sender, EventArgs e)
        {
            dgvMainOrders.DataSource = TafelManager.MainOrdersList;
            dgvMainOrdersSetting(ref dgvMainOrders);
            dgvFrontOrders.DataSource = TafelManager.FrontOrdersList;
            dgvFrontOrdersSetting(ref dgvFrontOrders);
        }

        private void dgvMainOrdersSetting(ref PercentageDataGrid grid)
        {
            grid.Columns["GroupType"].Visible = false;

            grid.AutoGenerateColumns = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
            }

            grid.Columns["ClientName"].HeaderText = "Клиент";
            grid.Columns["OrderNumber"].HeaderText = "№ док.";
            grid.Columns["MainOrderID"].HeaderText = "№ п\\п";

            grid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ClientName"].MinimumWidth = 105;
            grid.Columns["OrderNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["OrderNumber"].MinimumWidth = 55;
            grid.Columns["MainOrderID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["MainOrderID"].MinimumWidth = 55;
        }

        private void dgvFrontOrdersSetting(ref PercentageDataGrid grid)
        {
            if (!grid.Columns.Contains("FrontsColumn"))
                grid.Columns.Add(TafelManager.FrontsOrdersManager.FrontsColumn);
            if (!grid.Columns.Contains("FrameColorsColumn"))
                grid.Columns.Add(TafelManager.FrontsOrdersManager.FrameColorsColumn);
            if (!grid.Columns.Contains("PatinaColumn"))
                grid.Columns.Add(TafelManager.FrontsOrdersManager.PatinaColumn);
            if (!grid.Columns.Contains("InsetTypesColumn"))
                grid.Columns.Add(TafelManager.FrontsOrdersManager.InsetTypesColumn);
            if (!grid.Columns.Contains("InsetColorsColumn"))
                grid.Columns.Add(TafelManager.FrontsOrdersManager.InsetColorsColumn);
            if (!grid.Columns.Contains("TechnoProfilesColumn"))
                grid.Columns.Add(TafelManager.FrontsOrdersManager.TechnoProfilesColumn);
            if (!grid.Columns.Contains("TechnoFrameColorsColumn"))
                grid.Columns.Add(TafelManager.FrontsOrdersManager.TechnoFrameColorsColumn);
            if (!grid.Columns.Contains("TechnoInsetTypesColumn"))
                grid.Columns.Add(TafelManager.FrontsOrdersManager.TechnoInsetTypesColumn);
            if (!grid.Columns.Contains("TechnoInsetColorsColumn"))
                grid.Columns.Add(TafelManager.FrontsOrdersManager.TechnoInsetColorsColumn);
            if (grid.Columns.Contains("ImpostMargin"))
                grid.Columns["ImpostMargin"].Visible = false;
            if (grid.Columns.Contains("CreateDateTime"))
            {
                grid.Columns["CreateDateTime"].HeaderText = "Добавлено";
                grid.Columns["CreateDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                grid.Columns["CreateDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grid.Columns["CreateDateTime"].Width = 100;
            }
            if (grid.Columns.Contains("CreateUserID"))
                grid.Columns["CreateUserID"].Visible = false;
            grid.Columns["FrontsOrdersID"].Visible = false;
            grid.Columns["MainOrderID"].Visible = false;
            grid.Columns["FrontID"].Visible = false;
            grid.Columns["ColorID"].Visible = false;
            grid.Columns["InsetColorID"].Visible = false;
            grid.Columns["PatinaID"].Visible = false;
            grid.Columns["InsetTypeID"].Visible = false;
            grid.Columns["TechnoProfileID"].Visible = false;
            grid.Columns["TechnoColorID"].Visible = false;
            grid.Columns["TechnoInsetTypeID"].Visible = false;
            grid.Columns["TechnoInsetColorID"].Visible = false;

            grid.AutoGenerateColumns = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = true;
                //Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            grid.Columns["Height"].HeaderText = "Высота";
            grid.Columns["Width"].HeaderText = "Ширина";
            grid.Columns["Count"].HeaderText = "Кол-во";
            grid.Columns["Notes"].HeaderText = "Примечание";
            grid.Columns["Square"].HeaderText = "Площадь";
            grid.Columns["IsNonStandard"].HeaderText = "Н\\С";
            grid.Columns["ClientName"].HeaderText = "Клиент";
            grid.Columns["GroupNumber"].HeaderText = "№ группы";

            grid.Columns["FrontsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["FrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["InsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["InsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechnoProfilesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechnoFrameColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechnoInsetTypesColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["TechnoInsetColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Height"].Width = 85;
            grid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Width"].Width = 85;
            grid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Count"].Width = 85;
            grid.Columns["Square"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Square"].Width = 85;
            grid.Columns["IsNonStandard"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["IsNonStandard"].MinimumWidth = 55;
            grid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["ClientName"].MinimumWidth = 185;
            grid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["Notes"].MinimumWidth = 185;
            grid.Columns["GroupNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            grid.Columns["GroupNumber"].Width = 85;
            grid.Columns["GroupNumber"].ReadOnly = false;

            grid.AutoGenerateColumns = false;

            int DisplayIndex = 0;
            grid.Columns["FrontsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["FrameColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnoProfilesColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnoFrameColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["InsetTypesColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["InsetColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnoInsetTypesColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["TechnoInsetColorsColumn"].DisplayIndex = DisplayIndex++;
            grid.Columns["Height"].DisplayIndex = DisplayIndex++;
            grid.Columns["Width"].DisplayIndex = DisplayIndex++;
            grid.Columns["Count"].DisplayIndex = DisplayIndex++;
            grid.Columns["GroupNumber"].DisplayIndex = DisplayIndex++;
            grid.Columns["Square"].DisplayIndex = DisplayIndex++;
            grid.Columns["IsNonStandard"].DisplayIndex = DisplayIndex++;
            grid.Columns["ClientName"].DisplayIndex = DisplayIndex++;
            grid.Columns["Notes"].DisplayIndex = DisplayIndex++;
        }

        private void btnPrintTafel_Click(object sender, EventArgs e)
        {
            bPrint = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            TafelSplitStruct TafelSplitStruct = new Modules.WorkAssignments.TafelSplitStruct()
            {
                bEqual = true,
                SourceCount = TafelManager.CurrentFrontsCount
            };
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            TafelSplitMenu TafelSplitMenu = new TafelSplitMenu(this, ref TafelSplitStruct);
            TopForm = TafelSplitMenu;
            TafelSplitMenu.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();
            TafelSplitMenu.Dispose();
            TopForm = null;

            if (!TafelSplitStruct.bOk)
                return;

            decimal Square = 0;
            if (TafelSplitStruct.SecondCount == 0)
            {
                TafelManager.EditCurrentFrontsCout(TafelSplitStruct.FirstCount, ref Square);
                for (int i = 0; i < TafelSplitStruct.PosCount - 1; i++)
                {
                    TafelManager.AddNewRow(TafelSplitStruct.FirstCount, Square);
                }
            }
            else
            {
                TafelManager.EditCurrentFrontsCout(TafelSplitStruct.SecondCount, ref Square);
                for (int i = 0; i < TafelSplitStruct.PosCount; i++)
                {
                    TafelManager.AddNewRow(TafelSplitStruct.FirstCount, Square);
                }
            }
        }

        private void dgvFrontOrders_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                TafelManager.MoveToPosition(e.RowIndex);
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            TafelSplitStruct TafelSplitStruct = new Modules.WorkAssignments.TafelSplitStruct()
            {
                bEqual = false,
                SourceCount = TafelManager.CurrentFrontsCount
            };
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            TafelSplitMenu TafelSplitMenu = new TafelSplitMenu(this, ref TafelSplitStruct);
            TopForm = TafelSplitMenu;
            TafelSplitMenu.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();
            TafelSplitMenu.Dispose();
            TopForm = null;

            if (!TafelSplitStruct.bOk)
                return;

            decimal Square = 0;
            if (TafelSplitStruct.SecondCount == 0)
            {
                TafelManager.EditCurrentFrontsCout(TafelSplitStruct.FirstCount, ref Square);
                for (int i = 0; i < TafelSplitStruct.PosCount - 1; i++)
                {
                    TafelManager.AddNewRow(TafelSplitStruct.FirstCount, Square);
                }
            }
            else
            {
                TafelManager.EditCurrentFrontsCout(TafelSplitStruct.SecondCount, ref Square);
                for (int i = 0; i < TafelSplitStruct.PosCount; i++)
                {
                    TafelManager.AddNewRow(TafelSplitStruct.FirstCount, Square);
                }
            }
        }

    }
}
