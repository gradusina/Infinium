using Infinium.Modules.TechnologyCatalog;

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AddNewTechStoreItemForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;

        bool bEdit = false;
        bool bPrintLabels = false;
        string TechStoreName = string.Empty;
        string TechStoreSubGroupName = string.Empty;
        string SubGroupNotes = string.Empty;
        string SubGroupNotes1 = string.Empty;
        string SubGroupNotes2 = string.Empty;
        string DocDateTime = string.Empty;
        int FormEvent = 0;

        Form TopForm = null;

        TechStoreItemsManager StorageItemsManager;
        CabFurLabel CabFurLabelLabelManager = null;
        SampleLabel SampleLabelManager = null;
        DataTable DecorDT;
        DataTable CabFurDT;

        public AddNewTechStoreItemForm(ref TechStoreItemsManager tStorageItemsManager)
        {
            InitializeComponent();

            StorageItemsManager = tStorageItemsManager;
            Initialize();
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            StorageItemsManager.SetNewItemGrid(ref NewItemsDataGrid);

            while (!SplashForm.bCreated) ;
        }

        public AddNewTechStoreItemForm(ref TechStoreItemsManager tStorageItemsManager,
            string sTechStoreName, string sTechStoreSubGroupName, string sSubGroupNotes, string sSubGroupNotes1, string sSubGroupNotes2,
            bool PrintLabels)
        {
            InitializeComponent();

            bEdit = true;
            bPrintLabels = PrintLabels;
            if (bPrintLabels)
                SaveButton.Visible = false;
            NewItemsDataGrid.AllowUserToAddRows = false;
            NewItemsDataGrid.AllowUserToDeleteRows = false;

            TechStoreName = sTechStoreName;
            TechStoreSubGroupName = sTechStoreSubGroupName;
            SubGroupNotes = sSubGroupNotes;
            SubGroupNotes1 = sSubGroupNotes1;
            SubGroupNotes2 = sSubGroupNotes2;

            StorageItemsManager = tStorageItemsManager;
            Initialize();

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            StorageItemsManager.SetNewItemGrid(ref NewItemsDataGrid);

            while (!SplashForm.bCreated) ;
        }

        public void Initialize()
        {
            CabFurLabelLabelManager = new CabFurLabel();
            SampleLabelManager = new SampleLabel();
            if (CabFurDT == null)
            {
                CabFurDT = new DataTable();
                CabFurDT.Columns.Add(new DataColumn("TechStoreName", Type.GetType("System.String")));
                CabFurDT.Columns.Add(new DataColumn("TechStoreSubGroupName", Type.GetType("System.String")));
                CabFurDT.Columns.Add(new DataColumn("SubGroupNotes", Type.GetType("System.String")));
                CabFurDT.Columns.Add(new DataColumn("SubGroupNotes1", Type.GetType("System.String")));
                CabFurDT.Columns.Add(new DataColumn("SubGroupNotes2", Type.GetType("System.String")));
                CabFurDT.Columns.Add(new DataColumn("Product", Type.GetType("System.String")));
                CabFurDT.Columns.Add(new DataColumn("Color", Type.GetType("System.String")));
                CabFurDT.Columns.Add(new DataColumn("Height", Type.GetType("System.String")));
                CabFurDT.Columns.Add(new DataColumn("Width", Type.GetType("System.String")));
                CabFurDT.Columns.Add(new DataColumn("Length", Type.GetType("System.String")));
                CabFurDT.Columns.Add(new DataColumn("PositionsCount", Type.GetType("System.String")));
                CabFurDT.Columns.Add(new DataColumn("LabelsCount", Type.GetType("System.Int32")));
                CabFurDT.Columns.Add(new DataColumn("FactoryType", Type.GetType("System.Int32")));
                CabFurDT.Columns.Add(new DataColumn("DecorConfigID", Type.GetType("System.Int32")));
            }
            if (DecorDT == null)
            {
                DecorDT = new DataTable();
                DecorDT.Columns.Add(new DataColumn("Product", Type.GetType("System.String")));
                DecorDT.Columns.Add(new DataColumn("Decor", Type.GetType("System.String")));
                DecorDT.Columns.Add(new DataColumn("Color", Type.GetType("System.String")));
                DecorDT.Columns.Add(new DataColumn("Length", Type.GetType("System.String")));
                DecorDT.Columns.Add(new DataColumn("Height", Type.GetType("System.String")));
                DecorDT.Columns.Add(new DataColumn("Width", Type.GetType("System.String")));
                DecorDT.Columns.Add(new DataColumn("PositionsCount", Type.GetType("System.String")));
                DecorDT.Columns.Add(new DataColumn("LabelsCount", Type.GetType("System.Int32")));
                DecorDT.Columns.Add(new DataColumn("FactoryType", Type.GetType("System.Int32")));
                DecorDT.Columns.Add(new DataColumn("DecorConfigID", Type.GetType("System.Int32")));
            }
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

        private void NewsCommentsForm_Shown(object sender, EventArgs e)
        {
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
        }

        private void MenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!bEdit)
                StorageItemsManager.SaveNewItems();
            else
                StorageItemsManager.SaveEditItems();

            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void NewItemsDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!bEdit)
                    StorageItemsManager.SaveNewItems();
                else
                    StorageItemsManager.SaveEditItems();

                FormEvent = eClose;
                AnimateTimer.Enabled = true;
            }
        }

        private void NewItemsDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (NewItemsDataGrid.Columns[e.ColumnIndex].Name == "InsetTypeColumn"
                && NewItemsDataGrid.SelectedRows.Count > 0)
            {
                NewItemsDataGrid.SelectedRows[0].Cells["InsetColorID"].Value = DBNull.Value;
                NewItemsDataGrid.SelectedRows[0].Cells["InsetColorColumn"].Value = DBNull.Value;
                StorageItemsManager.FilterInsetColors(StorageItemsManager.CurrentInsetTypeGroup);
            }
        }

        private void NewItemsDataGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void NewItemsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem4_Click(object sender, EventArgs e)
        {
            int Length = 0;
            int Height = 0;
            int Width = 0;
            if (NewItemsDataGrid.SelectedRows[0].Cells["Length"].Value != DBNull.Value)
                Length = Convert.ToInt32(NewItemsDataGrid.SelectedRows[0].Cells["Length"].Value);
            if (NewItemsDataGrid.SelectedRows[0].Cells["Height"].Value != DBNull.Value)
                Height = Convert.ToInt32(NewItemsDataGrid.SelectedRows[0].Cells["Height"].Value);
            if (NewItemsDataGrid.SelectedRows[0].Cells["Width"].Value != DBNull.Value)
                Width = Convert.ToInt32(NewItemsDataGrid.SelectedRows[0].Cells["Width"].Value);

            bool bNeedLength = false;
            bool bNeedHeight = false;
            bool bNeedWidth = false;
            bool PressOK = false;
            int LabelsCount = 0;
            int PositionsCount = 0;
            int LabelsLength = 0;
            int LabelsHeight = 0;
            int LabelsWidth = 0;
            if (Length == 0)
                bNeedLength = true;
            if (Height == 0)
                bNeedHeight = true;
            if (Width == 0)
                bNeedWidth = true;
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            SampleDecorLabelInfoMenu SampleLabelInfoMenu = new SampleDecorLabelInfoMenu(this, bNeedLength, bNeedHeight, bNeedWidth);
            TopForm = SampleLabelInfoMenu;
            SampleLabelInfoMenu.ShowDialog();
            PressOK = SampleLabelInfoMenu.PressOK;
            LabelsCount = SampleLabelInfoMenu.LabelsCount;
            LabelsLength = SampleLabelInfoMenu.LabelsLength;
            LabelsHeight = SampleLabelInfoMenu.LabelsHeight;
            LabelsWidth = SampleLabelInfoMenu.LabelsWidth;
            PositionsCount = SampleLabelInfoMenu.PositionsCount;
            PhantomForm.Close();
            PhantomForm.Dispose();
            SampleLabelInfoMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;

            if (Length > 0)
                LabelsLength = Length;
            if (Height > 0)
                LabelsHeight = Height;
            if (Width > 0)
                LabelsWidth = Width;
            string Product = TechStoreSubGroupName;
            string Decor = NewItemsDataGrid.SelectedRows[0].Cells["TechStoreName"].Value.ToString();
            string Color = NewItemsDataGrid.SelectedRows[0].Cells["CoverColumn"].FormattedValue.ToString();
            string Patina = NewItemsDataGrid.SelectedRows[0].Cells["PatinaColumn"].FormattedValue.ToString();
            if (Patina != "на выбор")
                Color = Color + "+патина " + Patina;
            //int Height = 0;
            //int Width = 0;
            int DecorConfigID = 0;

            DataRow NewRow = DecorDT.NewRow();

            NewRow["Decor"] = Decor;
            NewRow["Color"] = Color;
            NewRow["Product"] = Product;
            NewRow["Length"] = LabelsLength.ToString();
            NewRow["Height"] = LabelsHeight.ToString();
            NewRow["Width"] = LabelsWidth.ToString();
            NewRow["LabelsCount"] = LabelsCount;
            NewRow["PositionsCount"] = PositionsCount;
            NewRow["DecorConfigID"] = DecorConfigID;

            //if (kryptonCheckSet2.CheckedIndex == 0)
            //    NewRow["FactoryType"] = 2;
            //if (kryptonCheckSet2.CheckedIndex == 1)
            //    NewRow["FactoryType"] = 1;
            DecorDT.Rows.Add(NewRow);
        }

        private void kryptonContextMenuItem6_Click(object sender, EventArgs e)
        {
            SampleLabelManager.ClearLabelInfo();

            //Проверка
            if (DecorDT.Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Очередь для печати пуста",
                    "Ошибка печати");
                return;
            }

            for (int i = 0; i < DecorDT.Rows.Count; i++)
            {
                int LabelsCount = Convert.ToInt32(DecorDT.Rows[i]["LabelsCount"]);
                int DecorConfigID = Convert.ToInt32(DecorDT.Rows[i]["DecorConfigID"]);
                int SampleLabelID = 0;
                for (int j = 0; j < LabelsCount; j++)
                {
                    SampleInfo LabelInfo = new SampleInfo();

                    DataTable DT = DecorDT.Clone();

                    DataRow destRow = DT.NewRow();
                    foreach (DataColumn column in DecorDT.Columns)
                    {
                        if (column.ColumnName == "FactoryType")
                            continue;
                        destRow[column.ColumnName] = DecorDT.Rows[i][column.ColumnName];
                    }
                    LabelInfo.ProductType = 1;
                    SampleLabelID = SampleLabelManager.SaveSampleLabel(DecorConfigID, DateTime.Now, Security.CurrentUserID, 1);
                    LabelInfo.BarcodeNumber = SampleLabelManager.GetBarcodeNumber(17, SampleLabelID);
                    //LabelInfo.FactoryType = Convert.ToInt32(DecorDT.Rows[i]["FactoryType"]);
                    LabelInfo.FactoryType = 1;
                    LabelInfo.DocDateTime = DateTime.Now.ToString("dd.MM.yyyy");
                    DT.Rows.Add(destRow);
                    LabelInfo.OrderData = DT;

                    SampleLabelManager.AddLabelInfo(ref LabelInfo);
                }
            }
            PrintDialog.Document = SampleLabelManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                SampleLabelManager.Print();
            }
        }

        private void kryptonContextMenuItem5_Click(object sender, EventArgs e)
        {
            SampleLabelManager.ClearOrderData();
            DecorDT.Clear();
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            if (bEdit)
            {
                StorageItemsManager.SaveChangesToStoreDetail();
                StorageItemsManager.SaveEditItems();
            }

            InfiniumTips.ShowTip(this, 50, 85, "Связанные конфигурации изменены", 1700);
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            bool PressOK = false;
            int LabelsCount = 0;
            int PositionsCount = 0;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            CabFurLabelInfoMenu CabFurLabelInfoMenu = new CabFurLabelInfoMenu(this, false, false, false);
            TopForm = CabFurLabelInfoMenu;
            CabFurLabelInfoMenu.ShowDialog();
            PressOK = CabFurLabelInfoMenu.PressOK;
            LabelsCount = CabFurLabelInfoMenu.LabelsCount;
            DocDateTime = CabFurLabelInfoMenu.DocDateTime;
            PositionsCount = CabFurLabelInfoMenu.PositionsCount;
            PhantomForm.Close();
            PhantomForm.Dispose();
            CabFurLabelInfoMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;

            string Product = TechStoreSubGroupName;
            int LabelsLength = Convert.ToInt32(NewItemsDataGrid.SelectedRows[0].Cells["Length"].Value);
            int LabelsHeight = Convert.ToInt32(NewItemsDataGrid.SelectedRows[0].Cells["Height"].Value);
            int LabelsWidth = Convert.ToInt32(NewItemsDataGrid.SelectedRows[0].Cells["Width"].Value);
            string Color = NewItemsDataGrid.SelectedRows[0].Cells["CoverColumn"].FormattedValue.ToString();

            int DecorConfigID = 0;
            CabFurDT.Clear();
            DataRow NewRow = CabFurDT.NewRow();

            NewRow["TechStoreName"] = TechStoreName;
            NewRow["TechStoreSubGroupName"] = TechStoreSubGroupName;
            NewRow["SubGroupNotes"] = SubGroupNotes;
            NewRow["SubGroupNotes1"] = SubGroupNotes1;
            NewRow["SubGroupNotes2"] = SubGroupNotes2;
            NewRow["Product"] = Product;
            NewRow["Color"] = Color;
            NewRow["Length"] = LabelsLength;
            NewRow["Height"] = LabelsHeight;
            NewRow["Width"] = LabelsWidth;
            NewRow["LabelsCount"] = LabelsCount;
            NewRow["PositionsCount"] = PositionsCount;
            NewRow["DecorConfigID"] = DecorConfigID;

            CabFurDT.Rows.Add(NewRow);

            CabFurLabelLabelManager.ClearLabelInfo();

            //Проверка
            if (CabFurDT.Rows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                    "Очередь для печати пуста",
                    "Ошибка печати");
                return;
            }

            for (int i = 0; i < CabFurDT.Rows.Count; i++)
            {
                LabelsCount = Convert.ToInt32(CabFurDT.Rows[i]["LabelsCount"]);
                DecorConfigID = Convert.ToInt32(CabFurDT.Rows[i]["DecorConfigID"]);
                int SampleLabelID = 0;
                for (int j = 0; j < LabelsCount; j++)
                {
                    CabFurInfo LabelInfo = new CabFurInfo();

                    DataTable DT = CabFurDT.Clone();

                    DataRow destRow = DT.NewRow();
                    foreach (DataColumn column in CabFurDT.Columns)
                    {
                        if (column.ColumnName == "FactoryType")
                            continue;
                        destRow[column.ColumnName] = CabFurDT.Rows[i][column.ColumnName];
                    }
                    LabelInfo.TechStoreName = CabFurDT.Rows[i]["TechStoreName"].ToString();
                    LabelInfo.TechStoreSubGroupName = CabFurDT.Rows[i]["TechStoreSubGroupName"].ToString();
                    LabelInfo.SubGroupNotes = CabFurDT.Rows[i]["SubGroupNotes"].ToString();
                    LabelInfo.SubGroupNotes1 = CabFurDT.Rows[i]["SubGroupNotes1"].ToString();
                    LabelInfo.SubGroupNotes2 = CabFurDT.Rows[i]["SubGroupNotes2"].ToString();
                    SampleLabelID = CabFurLabelLabelManager.SaveSampleLabel(DecorConfigID, DateTime.Now, Security.CurrentUserID, 2);
                    LabelInfo.BarcodeNumber = CabFurLabelLabelManager.GetBarcodeNumber(19, SampleLabelID);
                    LabelInfo.FactoryType = 2;
                    LabelInfo.ProductType = 2;
                    LabelInfo.DocDateTime = DocDateTime;
                    DT.Rows.Add(destRow);
                    LabelInfo.OrderData = DT;

                    CabFurLabelLabelManager.AddLabelInfo(ref LabelInfo);
                }
            }
            PrintDialog.Document = CabFurLabelLabelManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                CabFurLabelLabelManager.Print();
            }
        }

        private void NewItemsDataGrid_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["CreateUserID"].Value = Security.CurrentUserID;
            e.Row.Cells["CreateDate"].Value = Security.GetCurrentDate();
        }
    }
}
