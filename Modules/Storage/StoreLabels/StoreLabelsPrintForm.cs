using Infinium.Modules.Storage.StoreLabels;

using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class StoreLabelsPrintForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int BarcodeType = 13;
        int FormEvent = 0;


        Form TopForm = null;
        LightStartForm LightStartForm;
        StoreLabelsManager StoreLabelManager;
        PrintManagerStoreLabels StoreLabelPrintManager;

        public StoreLabelsPrintForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            LightStartForm = tLightStartForm;

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private void StoreLabelsForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
            txbPrintLabelsCount.Focus();
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

        private void Initialize()
        {
            StoreLabelManager = new StoreLabelsManager();
            StoreLabelManager.Initialize();
            GridSettings();
            CheckStoreColumns(ref dgvStore);

            StoreLabelPrintManager = new PrintManagerStoreLabels();
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

        public void GridSettings()
        {
            dgvStore.DataSource = StoreLabelManager.StoreList;

            dgvStore.Columns.Add(StoreLabelManager.ColorsColumn);
            dgvStore.Columns.Add(StoreLabelManager.PatinaColumn);
            dgvStore.Columns.Add(StoreLabelManager.CoversColumn);

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                CurrencyGroupSeparator = " ",
                CurrencySymbol = "",
                CurrencyDecimalDigits = 3,

                NumberGroupSeparator = " ",
                NumberDecimalDigits = 3,
                NumberDecimalSeparator = ","
            };
            dgvStore.Columns["Thickness"].DefaultCellStyle.Format = "N";
            dgvStore.Columns["Thickness"].DefaultCellStyle.FormatProvider = nfi1;
            dgvStore.Columns["Length"].DefaultCellStyle.Format = "N";
            dgvStore.Columns["Length"].DefaultCellStyle.FormatProvider = nfi1;
            dgvStore.Columns["Height"].DefaultCellStyle.Format = "N";
            dgvStore.Columns["Height"].DefaultCellStyle.FormatProvider = nfi1;
            dgvStore.Columns["Width"].DefaultCellStyle.Format = "N";
            dgvStore.Columns["Width"].DefaultCellStyle.FormatProvider = nfi1;
            dgvStore.Columns["Admission"].DefaultCellStyle.Format = "N";
            dgvStore.Columns["Admission"].DefaultCellStyle.FormatProvider = nfi1;
            dgvStore.Columns["Diameter"].DefaultCellStyle.Format = "N";
            dgvStore.Columns["Diameter"].DefaultCellStyle.FormatProvider = nfi1;
            dgvStore.Columns["Capacity"].DefaultCellStyle.Format = "N";
            dgvStore.Columns["Capacity"].DefaultCellStyle.FormatProvider = nfi1;
            dgvStore.Columns["Weight"].DefaultCellStyle.Format = "N";
            dgvStore.Columns["Weight"].DefaultCellStyle.FormatProvider = nfi1;

            dgvStore.Columns["StoreID"].Visible = false;

            dgvStore.Columns["StoreItemColumn"].HeaderText = "Наименование";
            dgvStore.Columns["Length"].HeaderText = "Длина, мм";
            dgvStore.Columns["Width"].HeaderText = "Ширина, мм";
            dgvStore.Columns["Height"].HeaderText = "Высота, мм";
            dgvStore.Columns["Thickness"].HeaderText = "Толщина, мм";
            dgvStore.Columns["Diameter"].HeaderText = "Диаметр, мм";
            dgvStore.Columns["Admission"].HeaderText = "Допуск, мм";
            dgvStore.Columns["Weight"].HeaderText = "Вес, кг";
            dgvStore.Columns["Capacity"].HeaderText = "Емкость, л";

            dgvStore.AutoGenerateColumns = false;

            dgvStore.Columns["StoreItemColumn"].DisplayIndex = 0;
            dgvStore.Columns["Diameter"].DisplayIndex = 1;
            dgvStore.Columns["Capacity"].DisplayIndex = 2;
            dgvStore.Columns["Thickness"].DisplayIndex = 3;
            dgvStore.Columns["Length"].DisplayIndex = 4;
            dgvStore.Columns["Height"].DisplayIndex = 5;
            dgvStore.Columns["Width"].DisplayIndex = 6;
            dgvStore.Columns["Admission"].DisplayIndex = 7;
            dgvStore.Columns["CoversColumn"].DisplayIndex = 8;
            dgvStore.Columns["PatinaColumn"].DisplayIndex = 9;
            dgvStore.Columns["ColorsColumn"].DisplayIndex = 10;
            dgvStore.Columns["Weight"].DisplayIndex = 11;

            dgvStore.Columns["StoreItemColumn"].MinimumWidth = 100;
            dgvStore.Columns["StoreItemColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvStore.Columns["ColorsColumn"].MinimumWidth = 100;
            dgvStore.Columns["ColorsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvStore.Columns["PatinaColumn"].MinimumWidth = 100;
            dgvStore.Columns["PatinaColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvStore.Columns["CoversColumn"].MinimumWidth = 100;
            dgvStore.Columns["CoversColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvStore.Columns["Thickness"].MinimumWidth = 60;
            dgvStore.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvStore.Columns["Length"].MinimumWidth = 60;
            dgvStore.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvStore.Columns["Thickness"].MinimumWidth = 60;
            dgvStore.Columns["Thickness"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvStore.Columns["Height"].MinimumWidth = 60;
            dgvStore.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvStore.Columns["Width"].MinimumWidth = 60;
            dgvStore.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvStore.Columns["Admission"].MinimumWidth = 60;
            dgvStore.Columns["Admission"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvStore.Columns["Diameter"].MinimumWidth = 60;
            dgvStore.Columns["Diameter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvStore.Columns["Capacity"].MinimumWidth = 60;
            dgvStore.Columns["Capacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvStore.Columns["Weight"].MinimumWidth = 60;
            dgvStore.Columns["Weight"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvStoreLabels.DataSource = StoreLabelManager.StoreLabelsList;
            dgvStoreLabels.Columns["StoreID"].Visible = false;
            dgvStoreLabels.Columns["StoreLabelID"].MinimumWidth = 100;
            dgvStoreLabels.Columns["StoreLabelID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void CheckStoreColumns(ref PercentageDataGrid Grid)
        {
            foreach (DataGridViewColumn Column in Grid.Columns)
            {
                foreach (DataGridViewRow Row in Grid.Rows)
                {
                    if (Row.Cells[Column.Index].FormattedValue.ToString().Length > 0)
                    {
                        Grid.Columns[Column.Index].Visible = true;
                        break;
                    }
                    else
                        Grid.Columns[Column.Index].Visible = false;
                }
            }

            Grid.Columns["StoreID"].Visible = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void dgvStore_SelectionChanged(object sender, EventArgs e)
        {
            if (StoreLabelManager == null)
                return;
            if (!StoreLabelManager.HasStoreLabels)
            {
                StoreLabelManager.ClearStoreTable();
                return;
            }

            StoreLabelManager.FilterStore(StoreLabelManager.CurrentStore);
            CheckStoreColumns(ref dgvStore);
        }

        private void btnPrintStoreLabel_Click(object sender, EventArgs e)
        {
            int[] StoreLabels = GetSelectedStoreLabels();
            if (StoreLabels.Count() < 1)
                return;

            StoreLabelPrintManager.ClearLabelInfo();
            for (int i = 0; i < StoreLabels.Count(); i++)
            {
                Info LabelInfo = new Info()
                {
                    BarcodeNumber = StoreLabelPrintManager.GetBarcodeNumber(BarcodeType, StoreLabels[i])
                };
                StoreLabelPrintManager.AddLabelInfo(ref LabelInfo);
            }

            PrintDialog.Document = StoreLabelPrintManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                StoreLabelPrintManager.Print();
            }
        }

        private void btnCreateStoreLabels_Click(object sender, EventArgs e)
        {
            if (StoreLabelManager == null)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создаются этикеток.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            StoreLabelManager.CreateGroupLabels(100);
            StoreLabelManager.SaveStoreLabels();
            StoreLabelManager.UpdateStoreLabels();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private int[] GetSelectedStoreLabels()
        {
            int[] rows = new int[dgvStoreLabels.SelectedRows.Count];

            for (int i = 0; i < dgvStoreLabels.SelectedRows.Count; i++)
                rows[i] = Convert.ToInt32(dgvStoreLabels.SelectedRows[i].Cells["StoreLabelID"].Value);
            Array.Sort(rows);

            return rows;
        }

        private void dgvStoreLabels_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void btnPrintLabels_Click(object sender, EventArgs e)
        {
            if (StoreLabelManager == null)
                return;

            int.TryParse(txbPrintLabelsCount.Text, out int LabelsCount);
            if (LabelsCount < 1)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Создаются этикетки.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            StoreLabelManager.CreateGroupLabels(LabelsCount);
            StoreLabelManager.SaveStoreLabels();
            StoreLabelManager.UpdateStoreLabels();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            int[] StoreLabels = StoreLabelManager.GetStoreLabels(LabelsCount);
            if (StoreLabels.Count() < 1)
                return;

            StoreLabelPrintManager.ClearLabelInfo();
            for (int i = 0; i < StoreLabels.Count(); i++)
            {
                Info LabelInfo = new Info()
                {
                    BarcodeNumber = StoreLabelPrintManager.GetBarcodeNumber(BarcodeType, StoreLabels[i])
                };
                StoreLabelPrintManager.AddLabelInfo(ref LabelInfo);
            }

            PrintDialog.Document = StoreLabelPrintManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                StoreLabelPrintManager.Print();
            }
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                FFFF FFFF = new FFFF()
                {
                    ImageType = 1
                };
                FFFF.Print();
            }
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                FFFF FFFF = new FFFF()
                {
                    ImageType = 2
                };
                FFFF.Print();
            }
        }
    }
}
