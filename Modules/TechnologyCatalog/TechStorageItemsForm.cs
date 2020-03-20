using Infinium.Modules.TechnologyCatalog;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class TechStorageItemsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        bool CopyConfigs = false;
        bool MoveConfigs = false;
        List<int> TechStoreIDList;

        int TechCatalogOperationsDetailID = 0;
        public int ColorInsetGroupID = 0;
        public int InsetGroupID = 0;

        bool CreateStoreDetail = false;
        bool bPrintLabels = false;
        public ArrayList TechStoreIDArray;
        public ArrayList LengthArray;
        public ArrayList IsHalfStuff1Array;
        public ArrayList HeightArray;
        public ArrayList WidthArray;
        Form TopForm = null;
        InsetTypesEditForm InsetTypesEditForm;
        InsetTypesSelectForm InsetTypesSelectForm;

        TechStoreManager TechStoreManager;
        public TechStoreItemsManager StorageItemsManager;

        public TechStorageItemsForm(TechStoreManager tTechStoreManager, bool PrintLabels)
        {
            InitializeComponent();
            TechStoreManager = tTechStoreManager;
            bPrintLabels = PrintLabels;
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();
            while (!SplashForm.bCreated) ;
        }

        public TechStorageItemsForm(TechStoreManager tTechStoreManager, int iTechCatalogOperationsDetailID)
        {
            InitializeComponent();
            CreateStoreDetail = true;
            ItemsDataGrid.MultiSelect = true;
            TechStoreManager = tTechStoreManager;
            TechCatalogOperationsDetailID = iTechCatalogOperationsDetailID;
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;


            Initialize();
            while (!SplashForm.bCreated) ;
        }

        private void TechStorageItemsForm_Shown(object sender, EventArgs e)
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
            TechStoreIDArray.Clear();
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void Initialize()
        {
            TechStoreIDArray = new ArrayList();
            IsHalfStuff1Array = new ArrayList();
            LengthArray = new ArrayList();
            HeightArray = new ArrayList();
            WidthArray = new ArrayList();
            StorageItemsManager = new TechStoreItemsManager(ref GroupsDataGrid, ref SubGroupsDataGrid, ref ItemsDataGrid);
            //GroupTextBox.DataBindings.Add("Text", StorageItemsManager.GroupsBindingSource, "StoreGroup");
            //SubGroupTextBox.DataBindings.Add("Text", StorageItemsManager.SubGroupsBindingSource, "StoreSubGroup");
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

        private void AddGroupButton_Click(object sender, EventArgs e)
        {
            if (GroupTextBox.Text.Length == 0)
                return;

            TechStoreManager.AddTechStoreGroup(GroupTextBox.Text);
            StorageItemsManager.RefreshGroups();
            //StorageItemsManager.AddGroup(GroupTextBox.Text);
        }

        private void RemoveGroupButton_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Группа будет удалена. Продолжить?",
                    "Удаление");

            if (OKCancel)
            {
                TechStoreManager.RemoveTechStoreGroup(StorageItemsManager.CurrentGroup);
                StorageItemsManager.RefreshGroups();
                //StorageItemsManager.RemoveGroup();
            }
        }

        private void AddItemButton_Click(object sender, EventArgs e)
        {
            if (SubGroupsDataGrid.SelectedRows.Count == 0 || SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value == DBNull.Value)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            AddNewTechStoreItemForm AddNewStoreItemForm = new AddNewTechStoreItemForm(ref StorageItemsManager);

            TopForm = AddNewStoreItemForm;

            AddNewStoreItemForm.ShowDialog();

            AddNewStoreItemForm.Close();
            AddNewStoreItemForm.Dispose();

            TopForm = null;
            TechStoreManager.RefreshTechStore();
            StorageItemsManager.RefreshStoreItems();
            SubGroupsDataGrid_SelectionChanged(null, null);
            //StorageItemsManager.MoveToStore(TechStoreID);
        }

        private void GroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (StorageItemsManager == null)
                return;

            if (StorageItemsManager.GroupsBS.Count == 0)
                return;

            StorageItemsManager.SubGroupsBS.Filter = "TechStoreGroupID = " + ((DataRowView)StorageItemsManager.GroupsBS.Current).Row["TechStoreGroupID"];
            StorageItemsManager.SubGroupsBS.MoveFirst();

            if (((DataRowView)StorageItemsManager.GroupsBS.Current).Row["TechStoreGroupName"] != DBNull.Value)
                GroupTextBox.Text = ((DataRowView)StorageItemsManager.GroupsBS.Current).Row["TechStoreGroupName"].ToString();
            else
                GroupTextBox.Text = string.Empty;
        }

        private void RemoveItemButton_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                       "Наименование будет удалено. Продолжить?",
                       "Удаление");

            if (OKCancel)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                TechStoreManager.RemoveTechStore(StorageItemsManager.CurrentStoreItem);
                StorageItemsManager.RefreshStoreItems();

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
        }

        private void ChangeItemButton_Click(object sender, EventArgs e)
        {
            if (!StorageItemsManager.EditItem())
                return;

            int TechStoreID = 0;

            if (ItemsDataGrid.SelectedRows.Count > 0 && ItemsDataGrid.SelectedRows[0].Cells["TechStoreID"].Value != DBNull.Value)
                TechStoreID = Convert.ToInt32(ItemsDataGrid.SelectedRows[0].Cells["TechStoreID"].Value);
            if (TechStoreID == 0)
                return;

            string TechStoreName = string.Empty;
            string TechStoreSubGroupName = string.Empty;
            string SubGroupNotes = string.Empty;
            string SubGroupNotes1 = string.Empty;
            string SubGroupNotes2 = string.Empty;

            if (ItemsDataGrid.SelectedRows.Count > 0 && ItemsDataGrid.SelectedRows[0].Cells["TechStoreName"].Value != DBNull.Value)
                TechStoreName = ItemsDataGrid.SelectedRows[0].Cells["TechStoreName"].Value.ToString();
            if (SubGroupsDataGrid.SelectedRows.Count > 0 && SubGroupsDataGrid.SelectedRows[0].Cells["Notes"].Value != DBNull.Value)
                SubGroupNotes = SubGroupsDataGrid.SelectedRows[0].Cells["Notes"].Value.ToString();
            if (SubGroupsDataGrid.SelectedRows.Count > 0 && SubGroupsDataGrid.SelectedRows[0].Cells["Notes1"].Value != DBNull.Value)
                SubGroupNotes1 = SubGroupsDataGrid.SelectedRows[0].Cells["Notes1"].Value.ToString();
            if (SubGroupsDataGrid.SelectedRows.Count > 0 && SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupName"].Value != DBNull.Value)
                TechStoreSubGroupName = SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupName"].Value.ToString();
            if (SubGroupsDataGrid.SelectedRows.Count > 0 && SubGroupsDataGrid.SelectedRows[0].Cells["Notes2"].Value != DBNull.Value)
                SubGroupNotes2 = SubGroupsDataGrid.SelectedRows[0].Cells["Notes2"].Value.ToString();

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            AddNewTechStoreItemForm AddNewStoreItemForm = new AddNewTechStoreItemForm(ref StorageItemsManager, TechStoreName, TechStoreSubGroupName,
                SubGroupNotes, SubGroupNotes1, SubGroupNotes2, bPrintLabels);

            TopForm = AddNewStoreItemForm;

            AddNewStoreItemForm.ShowDialog();

            AddNewStoreItemForm.Close();
            AddNewStoreItemForm.Dispose();

            TopForm = null;
            TechStoreManager.RefreshTechStore();
            StorageItemsManager.RefreshStoreItems();
            SubGroupsDataGrid_SelectionChanged(null, null);
            StorageItemsManager.MoveToStore(TechStoreID);
        }

        private void EditGroupButton_Click(object sender, EventArgs e)
        {
            if (GroupsDataGrid.SelectedRows.Count == 1 && GroupTextBox.Text != string.Empty)
            {
                TechStoreManager.EditTechStoreGroup(StorageItemsManager.CurrentGroup, GroupTextBox.Text);
                StorageItemsManager.RefreshGroups();
            }
        }

        private void SubGroupsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (StorageItemsManager == null)
                return;

            if (StorageItemsManager.SubGroupsBS.Count == 0)
            {
                StorageItemsManager.FilterStoreItems(0);
                return;
            }

            StorageItemsManager.FilterStoreItems(Convert.ToInt32(((DataRowView)StorageItemsManager.SubGroupsBS.Current).Row["TechStoreSubGroupID"]));
            //StorageItemsManager.ItemsBS.Filter = "TechStoreSubGroupID = " +
            //    ((DataRowView)StorageItemsManager.SubGroupsBS.Current).Row["TechStoreSubGroupID"];
            StorageItemsManager.ItemsBS.MoveFirst();

            foreach (DataGridViewColumn Column in ItemsDataGrid.Columns)
            {
                foreach (DataGridViewRow Row in ItemsDataGrid.Rows)
                {
                    if (Row.Cells[Column.Index].FormattedValue.ToString().Length > 0)
                    {
                        ItemsDataGrid.Columns[Column.Index].Visible = true;
                        break;
                    }
                    else
                        ItemsDataGrid.Columns[Column.Index].Visible = false;
                }
            }

            if (((DataRowView)StorageItemsManager.SubGroupsBS.Current).Row["TechStoreSubGroupName"] != DBNull.Value)
                SubGroupTextBox.Text = ((DataRowView)StorageItemsManager.SubGroupsBS.Current).Row["TechStoreSubGroupName"].ToString();
            else
                SubGroupTextBox.Text = string.Empty;

            ItemsDataGrid.Columns["TechStoreSubGroupID"].Visible = false;
            //ItemsDataGrid.Columns["TechStoreID"].Visible = false;
            ItemsDataGrid.Columns["MeasureID"].Visible = false;
            ItemsDataGrid.Columns["ColorID"].Visible = false;
            ItemsDataGrid.Columns["CoverID"].Visible = false;
            ItemsDataGrid.Columns["PatinaID"].Visible = false;
            ItemsDataGrid.Columns["InsetTypeID"].Visible = false;
            ItemsDataGrid.Columns["InsetColorID"].Visible = false;
            ItemsDataGrid.Columns["TechnoInsetTypeID"].Visible = false;
            ItemsDataGrid.Columns["TechnoInsetColorID"].Visible = false;

            if (ItemsDataGrid.Columns.Contains("InsetColorsColumn"))
                CheckInsetColors();
            if (ItemsDataGrid.Columns.Contains("InsetTypesColumn") && SubGroupsDataGrid.SelectedRows.Count > 0)
                CheckInsetTypes();
        }

        private void CheckInsetColors()
        {
            for (int i = 0; i < ItemsDataGrid.Rows.Count; i++)
            {
                if (StorageItemsManager.IsInsetColorAlreadyExist(Convert.ToInt32(ItemsDataGrid.Rows[i].Cells["TechStoreID"].Value), ColorInsetGroupID))
                {
                    ItemsDataGrid.Rows[i].Cells["InsetColorsColumn"].Value = true;
                }
            }
        }

        private void CheckInsetTypes()
        {
            for (int i = 0; i < ItemsDataGrid.Rows.Count; i++)
            {
                //if (Convert.ToInt32(SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value) == 8)//ЛДСтП-8
                //    GroupID = 0;
                //if (Convert.ToInt32(SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value) == 12)//полноформатное стекло
                //    GroupID = 2;
                if (StorageItemsManager.IsInsetTypeAlreadyExist(Convert.ToInt32(ItemsDataGrid.Rows[i].Cells["TechStoreID"].Value), InsetGroupID))
                {
                    ItemsDataGrid.Rows[i].Cells["InsetTypesColumn"].Value = true;
                }
            }
        }

        private void AddSubGroupButton_Click(object sender, EventArgs e)
        {
            if (SubGroupTextBox.Text.Length == 0 || StorageItemsManager.GroupsBS.Current == null)
                return;

            TechStoreManager.AddTechStoreSubGroup(SubGroupTextBox.Text, StorageItemsManager.CurrentGroup);
            StorageItemsManager.RefreshSubGroups();
            //StorageItemsManager.AddSubGroup(SubGroupTextBox.Text,
            //    Convert.ToInt32(((DataRowView)StorageItemsManager.GroupsBindingSource.Current).Row["TechStoreGroupID"]));
        }

        private void EditSubGroupButton_Click(object sender, EventArgs e)
        {
            if (SubGroupsDataGrid.SelectedRows.Count == 1 && SubGroupTextBox.Text != string.Empty)
            {
                TechStoreManager.EditTechStoreSubGroup(StorageItemsManager.CurrentSubGroup, SubGroupTextBox.Text, StorageItemsManager.CurrentSubGroupNotes,
                    StorageItemsManager.CurrentSubGroupNotes1, StorageItemsManager.CurrentSubGroupNotes2);
                StorageItemsManager.RefreshSubGroups();
                //StorageItemsManager.UpdateSubGroups(SubGroupTextBox.Text);
            }
        }

        private void RemoveSubGroupButton_Click(object sender, EventArgs e)
        {
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                    "Подгруппа будет удалена. Продолжить?",
                    "Удаление");

            if (OKCancel)
            {
                TechStoreManager.RemoveTechStoreSubGroup(StorageItemsManager.CurrentSubGroup);
                StorageItemsManager.RefreshSubGroups();
                //StorageItemsManager.RemoveSubGroup();
            }
        }

        private void ItemsDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!StorageItemsManager.EditItem() || CreateStoreDetail || ItemsDataGrid.Columns[e.ColumnIndex].Name == "CheckBoxColumn")
                return;

            int TechStoreID = 0;

            if (ItemsDataGrid.SelectedRows.Count > 0 && ItemsDataGrid.SelectedRows[0].Cells["TechStoreID"].Value != DBNull.Value)
                TechStoreID = Convert.ToInt32(ItemsDataGrid.SelectedRows[0].Cells["TechStoreID"].Value);
            if (TechStoreID == 0)
                return;

            string TechStoreName = string.Empty;
            string TechStoreSubGroupName = string.Empty;
            string SubGroupNotes = string.Empty;
            string SubGroupNotes1 = string.Empty;
            string SubGroupNotes2 = string.Empty;

            if (ItemsDataGrid.SelectedRows.Count > 0 && ItemsDataGrid.SelectedRows[0].Cells["TechStoreName"].Value != DBNull.Value)
                TechStoreName = ItemsDataGrid.SelectedRows[0].Cells["TechStoreName"].Value.ToString();
            if (SubGroupsDataGrid.SelectedRows.Count > 0 && SubGroupsDataGrid.SelectedRows[0].Cells["Notes"].Value != DBNull.Value)
                SubGroupNotes = SubGroupsDataGrid.SelectedRows[0].Cells["Notes"].Value.ToString();
            if (SubGroupsDataGrid.SelectedRows.Count > 0 && SubGroupsDataGrid.SelectedRows[0].Cells["Notes1"].Value != DBNull.Value)
                SubGroupNotes1 = SubGroupsDataGrid.SelectedRows[0].Cells["Notes1"].Value.ToString();
            if (SubGroupsDataGrid.SelectedRows.Count > 0 && SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupName"].Value != DBNull.Value)
                TechStoreSubGroupName = SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupName"].Value.ToString();
            if (SubGroupsDataGrid.SelectedRows.Count > 0 && SubGroupsDataGrid.SelectedRows[0].Cells["Notes2"].Value != DBNull.Value)
                SubGroupNotes2 = SubGroupsDataGrid.SelectedRows[0].Cells["Notes2"].Value.ToString();

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            AddNewTechStoreItemForm AddNewStoreItemForm = new AddNewTechStoreItemForm(ref StorageItemsManager, TechStoreName, TechStoreSubGroupName,
                SubGroupNotes, SubGroupNotes1, SubGroupNotes2, bPrintLabels);

            TopForm = AddNewStoreItemForm;

            AddNewStoreItemForm.ShowDialog();

            AddNewStoreItemForm.Close();
            AddNewStoreItemForm.Dispose();

            TopForm = null;
            SubGroupsDataGrid_SelectionChanged(null, null);
            StorageItemsManager.MoveToStore(TechStoreID);
        }

        private void AddTechStoreToStoreDetailButton_Click(object sender, EventArgs e)
        {
            TechStoreIDArray.Clear();
            IsHalfStuff1Array.Clear();
            LengthArray.Clear();
            HeightArray.Clear();
            WidthArray.Clear();
            for (int i = 0; i < ItemsDataGrid.SelectedRows.Count; i++)
            {
                TechStoreIDArray.Add(Convert.ToInt32(ItemsDataGrid.SelectedRows[i].Cells["TechStoreID"].Value));
                IsHalfStuff1Array.Add(ItemsDataGrid.SelectedRows[i].Cells["IsHalfStuff"].Value);
                LengthArray.Add(ItemsDataGrid.SelectedRows[i].Cells["Length"].Value);
                HeightArray.Add(ItemsDataGrid.SelectedRows[i].Cells["Height"].Value);
                WidthArray.Add(ItemsDataGrid.SelectedRows[i].Cells["Width"].Value);
            }
            if (TechStoreIDArray.Count == 0)
                return;

            int[] TechStoreID = TechStoreIDArray.OfType<int>().Distinct().ToArray();
            object[] IsHalfStuff1 = IsHalfStuff1Array.OfType<object>().ToArray();
            object[] Length = LengthArray.OfType<object>().ToArray();
            object[] Height = HeightArray.OfType<object>().ToArray();
            object[] Width = WidthArray.OfType<object>().ToArray();

            TechStoreManager.AddTechCatalogStoreDetail(TechCatalogOperationsDetailID, TechStoreID, IsHalfStuff1, Length, Height, Width);
            InfiniumTips.ShowTip(this, 50, 85, "Добавлено в материалы", 1700);
        }

        private void DeleteInsetColorsColumn()
        {
            if (ItemsDataGrid.Columns.Contains("InsetColorsColumn"))
                ItemsDataGrid.Columns.Remove("InsetColorsColumn");
        }

        private void DeleteInsetTypesColumn()
        {
            if (ItemsDataGrid.Columns.Contains("InsetTypesColumn"))
                ItemsDataGrid.Columns.Remove("InsetTypesColumn");
        }

        private void AddInsetColorsColumn()
        {
            if (ItemsDataGrid.Columns.Contains("InsetColorsColumn"))
                return;
            DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle()
            {
                Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter,
                NullValue = false
            };
            ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn CheckBoxColumn = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn()
            {
                DefaultCellStyle = dataGridViewCellStyle1,
                FalseValue = null,
                HeaderText = "",
                IndeterminateValue = null,
                Name = "InsetColorsColumn",
                ReadOnly = true,
                TrueValue = null,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 60
            };
            CheckBoxColumn.ReadOnly = false;
            CheckBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            CheckBoxColumn.DisplayIndex = 0;
            ItemsDataGrid.Columns.Add(CheckBoxColumn);
        }

        private void AddInsetTypesColumn()
        {
            if (ItemsDataGrid.Columns.Contains("InsetTypesColumn"))
                return;
            DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle()
            {
                Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter,
                NullValue = false
            };
            ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn CheckBoxColumn = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn()
            {
                DefaultCellStyle = dataGridViewCellStyle1,
                FalseValue = null,
                HeaderText = "",
                IndeterminateValue = null,
                Name = "InsetTypesColumn",
                TrueValue = null,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 60,
                ReadOnly = false,
                SortMode = DataGridViewColumnSortMode.Automatic,
                DisplayIndex = 0
            };
            ItemsDataGrid.Columns.Add(CheckBoxColumn);
        }

        private void btnSaveInsetColors_Click(object sender, EventArgs e)
        {
            if (!ItemsDataGrid.Columns.Contains("InsetColorsColumn"))
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            for (int i = 0; i < ItemsDataGrid.Rows.Count; i++)
            {
                if (Convert.ToBoolean(ItemsDataGrid.Rows[i].Cells["InsetColorsColumn"].Value))
                {
                    if (!StorageItemsManager.IsInsetColorAlreadyExist(Convert.ToInt32(ItemsDataGrid.Rows[i].Cells["TechStoreID"].Value), ColorInsetGroupID))
                        StorageItemsManager.AddInsetColor(Convert.ToInt32(ItemsDataGrid.Rows[i].Cells["TechStoreID"].Value), ColorInsetGroupID, ItemsDataGrid.Rows[i].Cells["TechStoreName"].Value.ToString());
                    //else
                    //    StorageItemsManager.EditInsetColor(Convert.ToInt32(ItemsDataGrid.Rows[i].Cells["TechStoreID"].Value), 1, ItemsDataGrid.Rows[i].Cells["TechStoreName"].Value.ToString());
                }
                else
                {
                    StorageItemsManager.DeleteInsetColor(Convert.ToInt32(ItemsDataGrid.Rows[i].Cells["TechStoreID"].Value), ColorInsetGroupID);
                }
            }
            StorageItemsManager.SaveInsetColors();

            DeleteInsetColorsColumn();
            btnSaveInsetColors.Visible = false;
            btnAddInsetColors.Visible = true;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnSaveInsetTypes_Click(object sender, EventArgs e)
        {
            if (!ItemsDataGrid.Columns.Contains("InsetTypesColumn"))
                return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            for (int i = 0; i < ItemsDataGrid.Rows.Count; i++)
            {
                if (Convert.ToBoolean(ItemsDataGrid.Rows[i].Cells["InsetTypesColumn"].Value))
                {
                    if (!StorageItemsManager.IsInsetTypeAlreadyExist(Convert.ToInt32(ItemsDataGrid.Rows[i].Cells["TechStoreID"].Value), InsetGroupID))
                        StorageItemsManager.AddInsetType(Convert.ToInt32(ItemsDataGrid.Rows[i].Cells["TechStoreID"].Value), InsetGroupID, ItemsDataGrid.Rows[i].Cells["TechStoreName"].Value.ToString());
                }
                else
                {
                    StorageItemsManager.DeleteInsetType(Convert.ToInt32(ItemsDataGrid.Rows[i].Cells["TechStoreID"].Value), InsetGroupID);
                }
            }
            StorageItemsManager.SaveInsetTypes();

            DeleteInsetTypesColumn();
            btnSaveInsetTypes.Visible = false;
            btnAddInsetTypes.Visible = true;

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void AddTechSubGroupToStoreDetailButton_Click(object sender, EventArgs e)
        {
            int TechSubGroupID = Convert.ToInt32(SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);

            TechStoreManager.AddTechCatalogStoreDetail(TechCatalogOperationsDetailID, TechSubGroupID);

            InfiniumTips.ShowTip(this, 50, 85, "Добавлено в материалы", 1700);
        }

        private void TechStorageItemsForm_Load(object sender, EventArgs e)
        {
            if (CreateStoreDetail)
                panel5.BringToFront();
            else
                panel4.BringToFront();

            StorageItemsManager.MoveToStoreGroup(TechStoreManager.CurrentTechStoreGroupID);
            StorageItemsManager.MoveToStoreSubGroup(TechStoreManager.CurrentTechStoreSubGroupID);
            StorageItemsManager.MoveToStore(TechStoreManager.CurrentTechStoreID);
        }

        private void btnAddInsetColors_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            InsetTypesSelectForm = new InsetTypesSelectForm(this);

            TopForm = InsetTypesSelectForm;
            InsetTypesSelectForm.ShowDialog();
            TopForm = null;

            PhantomForm.Close();

            PhantomForm.Dispose();
            if (InsetTypesSelectForm.PressOK)
            {
                ColorInsetGroupID = InsetTypesSelectForm.GroupID;
                InsetTypesSelectForm.Dispose();
                InsetTypesSelectForm = null;
                GC.Collect();

                AddInsetColorsColumn();
                CheckInsetColors();
                btnSaveInsetColors.Visible = true;
                btnAddInsetColors.Visible = false;
            }
            else
            {
                InsetTypesSelectForm.Dispose();
                InsetTypesSelectForm = null;
                GC.Collect();
            }
        }

        private void btnAddInsetTypes_Click(object sender, EventArgs e)
        {
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            InsetTypesEditForm = new InsetTypesEditForm(this);

            TopForm = InsetTypesEditForm;
            InsetTypesEditForm.ShowDialog();
            TopForm = null;

            PhantomForm.Close();

            PhantomForm.Dispose();
            if (InsetTypesEditForm.PressOK)
            {
                InsetGroupID = InsetTypesEditForm.GroupID;
                InsetTypesEditForm.Dispose();
                InsetTypesEditForm = null;
                GC.Collect();

                AddInsetTypesColumn();
                CheckInsetTypes();
                btnSaveInsetTypes.Visible = true;
                btnAddInsetTypes.Visible = false;
            }
            else
            {
                InsetTypesEditForm.Dispose();
                InsetTypesEditForm = null;
                GC.Collect();
            }

        }

        private void ItemsDataGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void btnStoreColors_Click(object sender, EventArgs e)
        {
        }

        private void ItemsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void SubGroupsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex != -1 && StorageItemsManager.CopyTechStoreID != -1)
            {
                PercentageDataGrid grid = (PercentageDataGrid)sender;
                grid.CurrentCell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                kryptonContextMenu2.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            int TechStoreSubGroupID = 0;
            if (SubGroupsDataGrid.SelectedRows.Count > 0 && SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                TechStoreSubGroupID = Convert.ToInt32(SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);
            StorageItemsManager.CopyTechStoreToSubGroup(TechStoreSubGroupID);
            StorageItemsManager.RefreshSubGroups();
            StorageItemsManager.MoveToStoreSubGroup(TechStoreSubGroupID);
            //StorageItemsManager.CopyTechStoreID = -1;
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            int TechStoreSubGroupID = 0;
            if (SubGroupsDataGrid.SelectedRows.Count > 0 && SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                TechStoreSubGroupID = Convert.ToInt32(SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);
            StorageItemsManager.MoveTechStoreToSubGroup(TechStoreSubGroupID);
            StorageItemsManager.RefreshSubGroups();
            StorageItemsManager.MoveToStoreSubGroup(TechStoreSubGroupID);
            //StorageItemsManager.CopyTechStoreID = -1;
        }

        private void kryptonContextMenuItem59_Click(object sender, EventArgs e)
        {
            int TechStoreID = 0;
            if (ItemsDataGrid.SelectedRows.Count > 0 && ItemsDataGrid.SelectedRows[0].Cells["TechStoreID"].Value != DBNull.Value)
                TechStoreID = Convert.ToInt32(ItemsDataGrid.SelectedRows[0].Cells["TechStoreID"].Value);
            StorageItemsManager.CopyTechStoreID = TechStoreID;
        }

        private void btnCopyTechStoreIDs_Click(object sender, EventArgs e)
        {
            if (TechStoreIDList == null)
                TechStoreIDList = new List<int>();
            TechStoreIDList.Clear();
            for (int i = 0; i < ItemsDataGrid.SelectedRows.Count; i++)
                TechStoreIDList.Add(Convert.ToInt32(ItemsDataGrid.SelectedRows[i].Cells["TechStoreID"].Value));
            kryptonButton2.Visible = true;
            kryptonButton3.Visible = true;
            CopyConfigs = true;
        }

        private void btnMoveTechStoreIDs_Click(object sender, EventArgs e)
        {
            if (TechStoreIDList == null)
                TechStoreIDList = new List<int>();
            TechStoreIDList.Clear();
            for (int i = 0; i < ItemsDataGrid.SelectedRows.Count; i++)
                TechStoreIDList.Add(Convert.ToInt32(ItemsDataGrid.SelectedRows[i].Cells["TechStoreID"].Value));
            kryptonButton2.Visible = true;
            kryptonButton3.Visible = true;
            MoveConfigs = true;
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            int TechStoreSubGroupID = 0;
            if (SubGroupsDataGrid.SelectedRows.Count > 0 && SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value != DBNull.Value)
                TechStoreSubGroupID = Convert.ToInt32(SubGroupsDataGrid.SelectedRows[0].Cells["TechStoreSubGroupID"].Value);
            if (TechStoreSubGroupID > 0)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                if (CopyConfigs)
                    TechStoreManager.CopyTechStore(TechStoreIDList, TechStoreSubGroupID);
                if (MoveConfigs)
                    TechStoreManager.MoveTechStore(TechStoreIDList, TechStoreSubGroupID);
                TechStoreManager.RefreshTechStore();
                StorageItemsManager.RefreshStoreItems();
                SubGroupsDataGrid_SelectionChanged(null, null);

                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            CopyConfigs = false;
            MoveConfigs = false;
            kryptonButton2.Visible = false;
            kryptonButton3.Visible = false;
        }

        private void kryptonButton3_Click(object sender, EventArgs e)
        {
            kryptonButton2.Visible = false;
            kryptonButton3.Visible = false;
        }
    }
}
