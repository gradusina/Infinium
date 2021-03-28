using Infinium.Modules.TechnologyCatalog;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class TechCatalogForm : Form
    {
        const int iStoreEdit = 37;
        const int iMachinesEdit = 39;
        const int iOperationsEdit = 40;
        const int iToolsEdit = 38;
        const int iAdmin = 36;

        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool bCopyGroupOperationsDetail = false;
        bool bCopyOperationsDetail = false;

        bool AddMachinesOperationClick = false;
        bool AddToolsClick = false;
        bool NeedSplash = false;

        List<int> TechCatalogGroupOperationsIDs;
        List<int> TechCatalogOperationsIDs;
        List<int> TechCatalogStoreDetailIDs;

        FileManager FM;

        bool bStopTransfer = false;

        bool bOperationsRole = false;
        bool bToolsRole = false;
        bool bPrintLabels = false;

        int FormEvent = 0;

        public int AttachsCount = 0;

        DataTable CabFurDT;

        Form TopForm = null;
        LightStartForm LightStartForm;

        CabFurLabel CabFurLabelManager = null;
        TechCatalogOperationsTerms TechCatalogOperationsTermsManager;
        TechCatalogStoreDetailTerms TechCatalogStoreDetailTermsManager;
        TechnologyMaps tTechnologyMaps;
        TechStoreManager TechStoreManager;

        public DataTable AttachDT;
        public DataTable TechStoreAttachDT;
        public DataTable ToolsAttachDT;

        public DataTable AttachDocumentsDT;
        public DataTable TechStoreAttachDocumentsDT;
        public DataTable ToolsAttachDocumentsDT;
        public BindingSource AttachDocumentsBS;
        public BindingSource TechStoreAttachDocumentsBS;
        public BindingSource ToolsAttachDocumentsBS;

        public TechCatalogForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();

            FM = new FileManager();

            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            CreateAttachments();

            FactoryComboBox.SelectedItem = "Профиль";
            ToolsFactoryComboBox.SelectedItem = "Профиль";

            MachineValueParametrsGrid.ReadOnly = false;

            Initialize();
            while (!SplashForm.bCreated) ;
        }

        private void CreateAttachments()
        {
            AttachDT = new DataTable();
            AttachDT.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));
            AttachDT.Columns.Add(new DataColumn("Path", Type.GetType("System.String")));
            TechStoreAttachDT = new DataTable();
            TechStoreAttachDT.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));
            TechStoreAttachDT.Columns.Add(new DataColumn("Extension", Type.GetType("System.String")));
            TechStoreAttachDT.Columns.Add(new DataColumn("Path", Type.GetType("System.String")));
            ToolsAttachDT = new DataTable();
            ToolsAttachDT.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));
            ToolsAttachDT.Columns.Add(new DataColumn("Path", Type.GetType("System.String")));

            AttachDocumentsDT = new DataTable();
            AttachDocumentsDT.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));
            AttachDocumentsDT.Columns.Add(new DataColumn("Path", Type.GetType("System.String")));
            TechStoreAttachDocumentsDT = new DataTable();
            TechStoreAttachDocumentsDT.Columns.Add(new DataColumn("DocType", Type.GetType("System.Int32")));
            TechStoreAttachDocumentsDT.Columns.Add(new DataColumn("TechStoreDocumentID", Type.GetType("System.Int32")));
            TechStoreAttachDocumentsDT.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));
            TechStoreAttachDocumentsDT.Columns.Add(new DataColumn("Path", Type.GetType("System.String")));
            ToolsAttachDocumentsDT = new DataTable();
            ToolsAttachDocumentsDT.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));
            ToolsAttachDocumentsDT.Columns.Add(new DataColumn("Path", Type.GetType("System.String")));

            AttachDocumentsBS = new BindingSource()
            {
                DataSource = AttachDocumentsDT
            };
            AttachDocumentsGrid.DataSource = AttachDocumentsBS;
            TechStoreAttachDocumentsBS = new BindingSource()
            {
                DataSource = new DataView(TechStoreAttachDocumentsDT, "DocType = 1", string.Empty, DataViewRowState.CurrentRows)
            };
            TechStoreAttachDocumentsGrid.DataSource = TechStoreAttachDocumentsBS;
            ToolsAttachDocumentsBS = new BindingSource()
            {
                DataSource = ToolsAttachDocumentsDT
            };
            ToolsAttachDocumentsGrid.DataSource = ToolsAttachDocumentsBS;

            AttachDocumentsGrid.Columns["Path"].Visible = false;
            AttachDocumentsGrid.Columns["FileName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            AttachDocumentsGrid.Columns["FileName"].Width = 250;
            TechStoreAttachDocumentsGrid.Columns["Path"].Visible = false;
            TechStoreAttachDocumentsGrid.Columns["DocType"].Visible = false;
            TechStoreAttachDocumentsGrid.Columns["TechStoreDocumentID"].Visible = false;
            TechStoreAttachDocumentsGrid.Columns["FileName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechStoreAttachDocumentsGrid.Columns["FileName"].Width = 250;
            ToolsAttachDocumentsGrid.Columns["Path"].Visible = false;
            ToolsAttachDocumentsGrid.Columns["FileName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ToolsAttachDocumentsGrid.Columns["FileName"].Width = 250;
        }

        private void CopyAttachs(string TechID)
        {
            using (DataTable DT = TechStoreManager.GetOperationsDocuments(TechID))
            {
                if (DT.Rows.Count == 0)
                    return;

                AttachsCount = DT.Rows.Count;

                foreach (DataRow Row in DT.Rows)
                {
                    DataRow NewRow = AttachDocumentsDT.NewRow();
                    NewRow["FileName"] = Row["FileName"];
                    NewRow["Path"] = "server";
                    AttachDocumentsDT.Rows.Add(NewRow);
                }
            }
        }

        private void CopyTechStoreAttachs(string TechID)
        {
            using (DataTable DT = TechStoreManager.GetTechStoreDocuments(TechID))
            {
                if (DT.Rows.Count == 0)
                    return;

                AttachsCount = DT.Rows.Count;

                foreach (DataRow Row in DT.Rows)
                {
                    DataRow NewRow = TechStoreAttachDocumentsDT.NewRow();
                    NewRow["TechStoreDocumentID"] = Row["TechStoreDocumentID"];
                    NewRow["DocType"] = Row["DocType"];
                    NewRow["FileName"] = Row["FileName"];
                    NewRow["Path"] = "server";
                    TechStoreAttachDocumentsDT.Rows.Add(NewRow);
                }
            }
        }

        private void CopyToolsAttachs(string TechID)
        {
            using (DataTable DT = TechStoreManager.GetToolsDocuments(TechID))
            {
                if (DT.Rows.Count == 0)
                    return;

                AttachsCount = DT.Rows.Count;

                foreach (DataRow Row in DT.Rows)
                {
                    DataRow NewRow = ToolsAttachDocumentsDT.NewRow();
                    NewRow["FileName"] = Row["FileName"];
                    NewRow["Path"] = "server";
                    ToolsAttachDocumentsDT.Rows.Add(NewRow);
                }
            }
        }

        private void TechCatalogForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
            NeedSplash = true;
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
            bool OkCancel = Infinium.LightMessageBox.Show(ref TopForm, true, "Точно выйти?", "Внимание");
            if (!OkCancel)
                return;
            GC.Collect();
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void NavigateMenuBackButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void NavigateMenuHerculesButton_Click(object sender, EventArgs e)
        {
            FormEvent = eMainMenu;
            AnimateTimer.Enabled = true;
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

        private void EditTechStoreButton_Click(object sender, EventArgs e)
        {
            //TechStoreManager.SaveSellersFromExcel();
            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Добавление материалов");
            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            if (TechStoreGrid.SelectedRows.Count > 0)
            {
                TechStoreManager.CurrentTechStoreGroupID = Convert.ToInt32(((DataRowView)cmbxTechStoreSubGroups.SelectedItem).Row["TechStoreGroupID"]);
                TechStoreManager.CurrentTechStoreSubGroupID = Convert.ToInt32(((DataRowView)cmbxTechStoreSubGroups.SelectedItem).Row["TechStoreSubGroupID"]);
                TechStoreManager.CurrentTechStoreID = Convert.ToInt32(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value);
            }
            TechStorageItemsForm TechStoreForm = new TechStorageItemsForm(TechStoreManager, bPrintLabels);

            TopForm = TechStoreForm;

            TechStoreForm.ShowDialog();
            TechStoreForm.Close();
            TechStoreForm.Dispose();

            TopForm = null;
            TechStoreManager.RefreshTechCatalogStoreDetails();
            TechCatalogOperationsDetailGrid_SelectionChanged(null, null);
        }

        private void kryptonCheckSet1_CheckedButtonChanged(object sender, EventArgs e)
        {
            if (!AddMachinesOperationClick)
            {
                MachinesOperationsGrid.MultiSelect = false;
            }
            if (!AddToolsClick)
            {
                ToolsGrid.MultiSelect = false;
            }
            if (CatalogMenuButton.Checked)
            {
                StorePanel.BringToFront();
                return;
            }

            if (MachinesMenuButton.Checked)
            {
                AllMachinesGrid_SelectionChanged(null, null);
                MachinesPanel.BringToFront();
                return;
            }

            if (OperationsMenuButton.Checked)
            {
                FactoryComboBox_SelectedIndexChanged(null, null);
                MachinesGrid_SelectionChanged(null, null);
                if (AddMachinesOperationClick)
                {
                    EditOperationsPanel.Visible = false;
                    AddMachinesOperationToCatalogButton2.Visible = true;
                    AddMachinesOperationClick = false;
                }
                else
                {
                    if (bOperationsRole)
                        EditOperationsPanel.Visible = true;
                    AddMachinesOperationToCatalogButton2.Visible = false;
                }

                OperationsPanel.BringToFront();
                return;
            }

            if (ToolsMenuButton.Checked)
            {
                if (AddToolsClick)
                {
                    EditToolsPanel.Visible = false;
                    EditTechCatalogToolsPanel.Visible = true;
                    AddToolsClick = false;
                }
                else
                {
                    if (bToolsRole)
                        EditToolsPanel.Visible = true;
                    EditTechCatalogToolsPanel.Visible = false;
                }

                ToolsPanel.BringToFront();
                return;
            }
        }

        private void FactoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TechStoreManager == null)
                return;

            int FactoryID;

            if (FactoryComboBox.SelectedItem.ToString() == "Профиль")
                FactoryID = 1;
            else
                FactoryID = 2;

            TechStoreManager.FilterSectors(FactoryID);
        }

        private void SectorsGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (TechStoreManager == null)
                return;

            if (SectorsGrid.Rows.Count == 0)
            {
                TechStoreManager.FilterSubSectors(0);
                return;
            }

            if (SectorsGrid.SelectedRows.Count == 0)
                return;

            if (SectorsGrid.SelectedRows[0].Cells["SectorID"].Value == DBNull.Value)
                return;

            TechStoreManager.FilterSubSectors(Convert.ToInt32(SectorsGrid.SelectedRows[0].Cells["SectorID"].Value));
        }

        private void SubSectorsGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (TechStoreManager == null)
                return;

            if (SubSectorsGrid.Rows.Count == 0)
            {
                TechStoreManager.FilterMachines(0);
                return;
            }

            if (SubSectorsGrid.SelectedRows.Count == 0)
                return;

            if (SubSectorsGrid.SelectedRows[0].Cells["SubSectorID"].Value == DBNull.Value)
                return;

            TechStoreManager.FilterMachines(Convert.ToInt32(SubSectorsGrid.SelectedRows[0].Cells["SubSectorID"].Value));
        }

        private void MachinesGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (TechStoreManager == null)
                return;

            if (MachinesGrid.Rows.Count == 0)
            {
                TechStoreManager.FilterMachinesOperations(0);
                return;
            }

            if (MachinesGrid.SelectedRows.Count == 0)
                return;

            if (MachinesGrid.SelectedRows[0].Cells["MachineID"].Value == DBNull.Value)
                return;

            TechStoreManager.FilterMachinesOperations(Convert.ToInt32(MachinesGrid.SelectedRows[0].Cells["MachineID"].Value));
        }

        private void AddSectorButton_Click(object sender, EventArgs e)
        {
            TechCatalogEvents.SaveEvents("Операции: Нажата кнопка Добавление участка");
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewSectorForm NewSectorForm = new NewSectorForm(FactoryComboBox.SelectedItem.ToString(), ref TechStoreManager);

            TopForm = NewSectorForm;

            NewSectorForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (NewSectorForm.Ok)
            {
                FactoryComboBox.SelectedItem = NewSectorForm.FactoryComboBox.SelectedItem;
                TechStoreManager.SectorsBS.MoveLast();
            }

            NewSectorForm.Dispose();
        }

        private void EditSectorButton_Click(object sender, EventArgs e)
        {
            if (SectorsGrid.SelectedRows.Count == 0)
                return;

            int SectorID = Convert.ToInt32(SectorsGrid.SelectedRows[0].Cells["SectorID"].Value);
            TechCatalogEvents.SaveEvents("Операции: Нажата кнопка Редактирование участка SectorID=" + SectorID);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewSectorForm NewSectorForm = new NewSectorForm(FactoryComboBox.SelectedItem.ToString(), SectorID, ref TechStoreManager);

            TopForm = NewSectorForm;

            NewSectorForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (NewSectorForm.Ok)
            {
                FactoryComboBox.SelectedItem = NewSectorForm.FactoryComboBox.SelectedItem;
                TechStoreManager.SectorsBS.Position = TechStoreManager.SectorsBS.Find("SectorID", SectorID);
                //TechStoreManager.UpdateSectorNameColumn(ref TechCatalogOperationsDetailGrid);
            }

            NewSectorForm.Dispose();
        }

        private void RemoveSectorButton_Click(object sender, EventArgs e)
        {
            if (SectorsGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Операции: Нажата кнопка Удаление участка SectorID=" + Convert.ToInt32(SectorsGrid.SelectedRows[0].Cells["SectorID"].Value));
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить участок?",
                "Удаление");

            if (OKCancel)
            {
                TechStoreManager.RemoveSector(Convert.ToInt32(SectorsGrid.SelectedRows[0].Cells["SectorID"].Value));
                TechStoreManager.UpdateMachinesOperationNameColumn(ref TechCatalogOperationsDetailGrid);
                TechStoreManager.UpdateMachineNameColumn(ref TechCatalogOperationsDetailGrid);
                TechStoreManager.UpdateSubSectorNameColumn(ref TechCatalogOperationsDetailGrid);
                TechStoreManager.UpdateSectorNameColumn(ref TechCatalogOperationsDetailGrid);
            }
            else
            {
                TechCatalogEvents.SaveEvents("Операции: отмена");
            }
        }

        private void AddSubSectorButton_Click(object sender, EventArgs e)
        {
            if (SectorsGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Операции: Нажата кнопка Добавление подучастка");
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewSubSectorForm NewSubSectorForm = new NewSubSectorForm(ref TechStoreManager);

            TopForm = NewSubSectorForm;

            NewSubSectorForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (NewSubSectorForm.Ok)
            {
                TechStoreManager.SubSectorsBS.MoveLast();
            }

            NewSubSectorForm.Dispose();
        }

        private void EditSubSectorButton_Click(object sender, EventArgs e)
        {
            if (SubSectorsGrid.SelectedRows.Count == 0)
                return;

            int SubSectorID = Convert.ToInt32(SubSectorsGrid.SelectedRows[0].Cells["SubSectorID"].Value);
            TechCatalogEvents.SaveEvents("Операции: Нажата кнопка Редактирование подучастка SubSectorID=" + SubSectorID);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewSubSectorForm NewSubSectorForm = new NewSubSectorForm(SubSectorID, ref TechStoreManager);

            TopForm = NewSubSectorForm;

            NewSubSectorForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (NewSubSectorForm.Ok)
            {
                TechStoreManager.SubSectorsBS.Position = TechStoreManager.SubSectorsBS.Find("SubSectorID", SubSectorID);
                //TechStoreManager.UpdateSubSectorNameColumn(ref TechCatalogOperationsDetailGrid);
            }

            NewSubSectorForm.Dispose();
        }

        private void RemoveSubSectorButton_Click(object sender, EventArgs e)
        {
            if (SubSectorsGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Операции: Нажата кнопка Удаление подучастка SubSectorID=" + Convert.ToInt32(SubSectorsGrid.SelectedRows[0].Cells["SubSectorID"].Value));
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить подучасток?",
                "Удаление");

            if (OKCancel)
            {
                TechStoreManager.RemoveSubSector(Convert.ToInt32(SubSectorsGrid.SelectedRows[0].Cells["SubSectorID"].Value));
                TechStoreManager.UpdateMachinesOperationNameColumn(ref TechCatalogOperationsDetailGrid);
                TechStoreManager.UpdateMachineNameColumn(ref TechCatalogOperationsDetailGrid);
                TechStoreManager.UpdateSubSectorNameColumn(ref TechCatalogOperationsDetailGrid);
            }
            else
            {
                TechCatalogEvents.SaveEvents("Операции: отмена");
            }
        }

        private void AddMachineButton_Click(object sender, EventArgs e)
        {
            //TechCatalogEvents.SaveEvents("Станки: Нажата кнопка Добавление станка");
            //PhantomForm PhantomForm = new Infinium.PhantomForm();
            //PhantomForm.Show();

            //NewMachineForm NewMachineForm = new NewMachineForm(ref TechStoreManager);

            //TopForm = NewMachineForm;

            //NewMachineForm.ShowDialog();

            //PhantomForm.Close();
            //PhantomForm.Dispose();

            //TopForm = null;

            //if (NewMachineForm.Ok)
            //{
            //    TechStoreManager.AllMachinesBS.Position = TechStoreManager.AllMachinesBS.Find("MachineName", NewMachineForm.tbMachineName.Text);
            //}

            //NewMachineForm.Dispose();
        }

        private void EditMachineButton_Click(object sender, EventArgs e)
        {
            //if (AllMachinesGrid.SelectedRows.Count == 0)
            //    return;

            //TechCatalogEvents.SaveEvents("Станки: Нажата кнопка Редактирование станка");
            //int MachineID = Convert.ToInt32(AllMachinesGrid.SelectedRows[0].Cells["MachineID"].Value);

            //PhantomForm PhantomForm = new Infinium.PhantomForm();
            //PhantomForm.Show();

            //NewMachineForm NewMachineForm = new NewMachineForm(MachineID, ref TechStoreManager);

            //TopForm = NewMachineForm;

            //NewMachineForm.ShowDialog();

            //PhantomForm.Close();
            //PhantomForm.Dispose();

            //TopForm = null;

            //if (NewMachineForm.Ok)
            //{
            //    TechStoreManager.AllMachinesBS.Position = TechStoreManager.AllMachinesBS.Find("MachineID", MachineID);
            //}

            //NewMachineForm.Dispose();
            //TechStoreManager.UpdateMachineNameColumn(ref TechCatalogOperationsDetailGrid);
        }

        private void RemoveMachineButton_Click(object sender, EventArgs e)
        {
            if (AllMachinesGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Станки: Нажата кнопка Удаление станка");
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                TechStoreManager.RemoveMachine(Convert.ToInt32(AllMachinesGrid.SelectedRows[0].Cells["MachineID"].Value));
                TechStoreManager.UpdateMachineNameColumn(ref TechCatalogOperationsDetailGrid);
            }
            else
            {
                TechCatalogEvents.SaveEvents("Станки: отмена");
            }

            if (AllMachinesGrid.SelectedRows.Count == 0)
            {
                MachineValueParametrsGrid.DataSource = null;
                MachinePhotoPictureBox.Image = null;
                TechStoreManager.FilterMachinesOperations(0);
                // TechStoreManager.FilterOperationsOnMachine(0);
            }
        }

        private void AddMachinesOperationButton_Click(object sender, EventArgs e)
        {
            if (MachinesGrid.SelectedRows.Count == 0)
                return;

            int MachineID = Convert.ToInt32(MachinesGrid.SelectedRows[0].Cells["MachineID"].Value);
            TechCatalogEvents.SaveEvents("Операции: Нажата кнопка Добавление операции к станку MachineID=" + MachineID);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewMachinesOperationForm NewMachinesOperationForm = new NewMachinesOperationForm(MachineID, ref TechStoreManager);

            TopForm = NewMachinesOperationForm;

            NewMachinesOperationForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (NewMachinesOperationForm.Ok)
            {
                TechStoreManager.MachinesOperationsBS.Position = 0;
            }

            NewMachinesOperationForm.Dispose();
        }

        private void EditMachinesOperationButton_Click(object sender, EventArgs e)
        {
            if (MachinesOperationsGrid.SelectedRows.Count == 0)
                return;

            int MachineID = Convert.ToInt32(MachinesGrid.SelectedRows[0].Cells["MachineID"].Value);
            int MachinesOperationID = Convert.ToInt32(MachinesOperationsGrid.SelectedRows[0].Cells["MachinesOperationID"].Value);
            TechCatalogEvents.SaveEvents("Операции: Нажата кнопка Редактирование операции MachinesOperationID=" + MachinesOperationID + " на станке MachineID=" + MachineID);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewMachinesOperationForm NewMachinesOperationForm = new NewMachinesOperationForm(MachineID, MachinesOperationID, ref TechStoreManager);

            TopForm = NewMachinesOperationForm;

            NewMachinesOperationForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            NewMachinesOperationForm.Dispose();
        }

        private void RemoveMachinesOperationButton_Click(object sender, EventArgs e)
        {
            if (MachinesOperationsGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Операции: Нажата кнопка Удаление операции MachinesOperationID=" + Convert.ToInt32(MachinesOperationsGrid.SelectedRows[0].Cells["MachinesOperationID"].Value));
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить операцию?",
                "Удаление");

            if (OKCancel)
            {
                TechStoreManager.RemoveMachinesOperation(Convert.ToInt32(MachinesOperationsGrid.SelectedRows[0].Cells["MachinesOperationID"].Value));
                TechStoreManager.UpdateMachinesOperationNameColumn(ref TechCatalogOperationsDetailGrid);
            }
            else
            {
                TechCatalogEvents.SaveEvents("Операции: отмена");
            }
        }

        private void AllMachinesGrid_SelectionChanged(object sender, EventArgs e)
        {
            MachineValueParametrsGrid.DataSource = null;

            if (AllMachinesGrid.SelectedRows.Count == 0 || TechStoreManager == null || AllMachinesGrid.SelectedRows[0].Cells["MachineID"].Value == DBNull.Value || !NeedSplash)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int MachineID = Convert.ToInt32(AllMachinesGrid.SelectedRows[0].Cells["MachineID"].Value);

            using (DataTable ValueParametrsDT = TechStoreManager.MachineReadValueTable(MachineID))
            {
                MachineValueParametrsGrid.DataSource = ValueParametrsDT;
                MachineValueParametrsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                DataGridViewComboBoxColumn ParametrName = new DataGridViewComboBoxColumn()
                {
                    DataSource = TechStoreManager.CurrentMachineParametrsDT,
                    DisplayMember = "ParametrName",
                    ValueMember = "ParametrID",
                    DataPropertyName = "ParametrID",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                MachineValueParametrsGrid.Columns.Add(ParametrName);

                MachineValueParametrsGrid.Columns["ParametrID"].Visible = false;
                ParametrName.ReadOnly = true;
                ParametrName.DisplayIndex = 0;
            }

            MachinePhotoPictureBox.Image = TechStoreManager.GetMachinePhoto(MachineID);

            TechStoreManager.FilterMachinesOperations(MachineID);

            if (MachinePhotoPictureBox.Image == null)
            {
                ZoomButton.Visible = false;
                MachinePhotoPictureBox.Cursor = Cursors.Default;
            }
            else
            {
                ZoomButton.Visible = true;
                MachinePhotoPictureBox.Cursor = Cursors.Hand;
            }

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void ApplyMachineValueParametrsButton_Click(object sender, EventArgs e)
        {
            if (AllMachinesGrid.SelectedRows.Count == 0 || AllMachinesGrid.SelectedRows[0].Cells["MachineID"].Value == DBNull.Value)
                return;

            TechCatalogEvents.SaveEvents("Станки: Нажата кнопка Сохранение параметров станка");
            string ValueParametrsDTstring;
            using (StringWriter SW = new StringWriter())
            {
                ((DataTable)MachineValueParametrsGrid.DataSource).WriteXml(SW);
                ValueParametrsDTstring = SW.ToString();
            }

            TechStoreManager.EditMachineValueParametrs(Convert.ToInt32(AllMachinesGrid.SelectedRows[0].Cells["MachineID"].Value), ValueParametrsDTstring);
            TechStoreManager.UpdateMachineNameColumn(ref TechCatalogOperationsDetailGrid);

            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void MachineValueParametrsGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!TechStoreManager.MachineParametrCanBeApply(Convert.ToInt32(MachineValueParametrsGrid.Rows[e.RowIndex].Cells["ParametrID"].Value), MachineValueParametrsGrid.Rows[e.RowIndex].Cells["Value"].Value.ToString()))
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                    "Некорректное значение параметра!",
                                    "Ошибка");

                MachineValueParametrsGrid.Rows[e.RowIndex].Cells["Value"].Value = 0;
            }
        }

        private void EditMachinePhotoButton_Click(object sender, EventArgs e)
        {
            if (AllMachinesGrid.SelectedRows.Count == 0 || TechStoreManager == null || AllMachinesGrid.SelectedRows[0].Cells["MachineID"].Value == DBNull.Value)
                return;

            TechCatalogEvents.SaveEvents("Станки: Нажата кнопка Добавление фотографии к станку");
            int MachineID = Convert.ToInt32(AllMachinesGrid.SelectedRows[0].Cells["MachineID"].Value);

            MachinePictureEditForm MachinePictureEditForm = new MachinePictureEditForm(ref TechStoreManager, MachineID);
            TopForm = MachinePictureEditForm;
            MachinePictureEditForm.ShowDialog();
            TopForm = null;
            MachinePictureEditForm.Dispose();

            MachinePhotoPictureBox.Image = TechStoreManager.GetMachinePhoto(MachineID);

            if (MachinePhotoPictureBox.Image == null)
            {
                ZoomButton.Visible = false;
                MachinePhotoPictureBox.Cursor = Cursors.Default;
            }
            else
            {
                ZoomButton.Visible = true;
                MachinePhotoPictureBox.Cursor = Cursors.Hand;
            }
        }

        private void TechCatalogGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (TechStoreManager == null)
                return;

            TechStoreAttachDocumentsDT.Clear();
            if (TechStoreGrid.SelectedRows.Count == 0)
            {
                TechStoreManager.FilterOperationsGroups(0);
                return;
            }

            if (TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value == DBNull.Value)
                return;
            TechStoreManager.CurrentTechStoreID = 0;

            CopyTechStoreAttachs(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value.ToString());
            TechStoreManager.FilterOperationsGroups(Convert.ToInt32(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value));
            GroupsOperationsDetailGrid_SelectionChanged(null, null);
            pcbxTechStoreImage.Image = null;

            if (cbxShowTechStoreImage.Checked)
            {
                DataRow[] rows = TechStoreAttachDocumentsDT.Select("DocType = 0");
                int TechStoreDocumentID = 0;
                if (rows.Count() > 0)
                {
                    TechStoreDocumentID = Convert.ToInt32(rows[0]["TechStoreDocumentID"]);

                    if (NeedSplash)
                    {
                        Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                        T.Start();

                        while (!SplashWindow.bSmallCreated) ;

                        pcbxTechStoreImage.Image = TechStoreManager.GetTechStoreImage(TechStoreDocumentID);

                        while (SplashWindow.bSmallCreated)
                            SmallWaitForm.CloseS = true;
                    }
                    else
                        pcbxTechStoreImage.Image = TechStoreManager.GetTechStoreImage(TechStoreDocumentID);
                }

                if (pcbxTechStoreImage.Image == null)
                {
                    //ZoomTechStoreImageButton.Visible = false;
                    pcbxTechStoreImage.Cursor = Cursors.Default;
                }
                else
                {
                    //ZoomTechStoreImageButton.Visible = true;
                    pcbxTechStoreImage.Cursor = Cursors.Hand;
                }
            }
        }

        private void AddMachinesOperationToCatalogButton_Click(object sender, EventArgs e)
        {
            if (GroupsOperationsDetailGrid.SelectedRows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Добавьте группу операций",
                   "Добавление операций");
                return;
            }

            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Прикрепление операции к складу");
            MachinesOperationsGrid.MultiSelect = true;
            AddMachinesOperationClick = true;
            OperationsMenuButton.Checked = true;
        }

        private void AddMachinesOperationToCatalogButton2_Click(object sender, EventArgs e)
        {
            if (MachinesOperationsGrid.SelectedRows.Count == 0 || TechStoreGrid.SelectedRows.Count == 0)
                return;

            int[] MachinesOperationID = new int[MachinesOperationsGrid.SelectedRows.Count];
            int GroupNumber = Convert.ToInt32(GroupsOperationsDetailGrid.SelectedRows[0].Cells["GroupNumber"].Value);
            int TechCatalogOperationsGroupID = Convert.ToInt32(GroupsOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value);
            //string GroupName = GroupsOperationsDetailGrid.SelectedRows[0].Cells["GroupName"].Value.ToString();
            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Прикрепление операций к складу");

            for (int i = 0; i < MachinesOperationsGrid.SelectedRows.Count; i++)
            {
                MachinesOperationID[i] = Convert.ToInt32(MachinesOperationsGrid.SelectedRows[i].Cells["MachinesOperationID"].Value);
            }
            TechStoreManager.AddTechCatalogOperationsDetail(
                Convert.ToInt32(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value), MachinesOperationID, TechCatalogOperationsGroupID);

            InfiniumTips.ShowTip(this, 50, 85, "Операции прикреплены к складу", 1700);
        }

        private void RemoveMachinesOperationAtCatalogButton_Click(object sender, EventArgs e)
        {
            if (TechCatalogOperationsDetailGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Открепление операции от склада");
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                for (int i = 0; i < TechCatalogOperationsDetailGrid.SelectedRows.Count; i++)
                {
                    TechStoreManager.RemoveTechCatalogOperationsDetail(Convert.ToInt32(TechCatalogOperationsDetailGrid.SelectedRows[i].Cells["TechCatalogOperationsDetailID"].Value));
                }
                if (!TechStoreManager.HasTechCatalogOperationsDetails)
                {
                    TechStoreManager.FilterOperationsGroups(Convert.ToInt32(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value));
                    GroupsOperationsDetailGrid_SelectionChanged(null, null);
                }
            }
            else
            {
                TechCatalogEvents.SaveEvents("Склад: отмена");
            }
        }

        private void AddTechStoreToStoreDetailButton_Click(object sender, EventArgs e)
        {
            if (TechCatalogOperationsDetailGrid.SelectedRows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Добавьте операцию",
                   "Добавление материалов");
                return;
            }
            if (!TechStoreManager.HasTechStoreGroups || !TechStoreManager.HasTechStoreSubGroups)
                return;

            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Прикрепление материала к складу");

            Thread T = new Thread(delegate () { SplashWindow.CreateSplash(); });
            T.Start();

            while (!SplashForm.bCreated) ;

            TechStorageItemsForm TechStoreForm = new TechStorageItemsForm(
                TechStoreManager, Convert.ToInt32(TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value));

            TopForm = TechStoreForm;

            TechStoreForm.ShowDialog();

            TechStoreForm.Close();
            TechStoreForm.Dispose();

            TopForm = null;
        }

        private void RemoveTechStoreAtStoreDetailButton_Click(object sender, EventArgs e)
        {
            if (TechCatalogStoreDetailGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Открепление материала от склада");

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                for (int i = 0; i < TechCatalogStoreDetailGrid.SelectedRows.Count; i++)
                {
                    TechStoreManager.RemoveTechCatalogStoreDetail(Convert.ToInt32(TechCatalogStoreDetailGrid.SelectedRows[i].Cells["TechCatalogStoreDetailID"].Value));
                }
            }
            else
            {
                TechCatalogEvents.SaveEvents("Склад: отмена");
            }
        }

        private void TechCatalogOperationsDetailGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (TechStoreManager == null)
                return;

            if (TechCatalogOperationsDetailGrid.SelectedRows.Count == 0)
            {
                TechStoreManager.FilterStoreDetails(0);
                AttachDocumentsDT.Clear();
                return;
            }

            if (TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value == DBNull.Value)
                return;

            TechStoreManager.FilterStoreDetails(Convert.ToInt32(TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value));
            AttachDocumentsDT.Clear();
            CheckTechStoreDetailsColumns();
            CopyAttachs(TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value.ToString());
        }

        private void ZoomButton_Click(object sender, EventArgs e)
        {
            if (MachinePhotoPictureBox.Image == null)
                return;

            TechCatalogEvents.SaveEvents("Станки: Нажата кнопка Увеличить изображение");
            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            ZoomImageForm ZoomImageForm = new ZoomImageForm(MachinePhotoPictureBox.Image, ref TopForm);

            TopForm = ZoomImageForm;

            ZoomImageForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            ZoomImageForm.Dispose();
        }

        private void ToolsFactoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TechStoreManager == null)
                return;

            int FactoryID;

            if (ToolsFactoryComboBox.SelectedItem.ToString() == "Профиль")
                FactoryID = 1;
            else
                FactoryID = 2;

            TechStoreManager.FilterToolsGroups(FactoryID);
        }

        private void ToolsGroupGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (TechStoreManager == null)
                return;

            if (ToolsGroupGrid.Rows.Count == 0 || ToolsGroupGrid.SelectedRows.Count == 0)
            {
                TechStoreManager.FilterToolsTypes(0);
                return;
            }

            if (ToolsGroupGrid.SelectedRows[0].Cells["ToolsGroupID"].Value == DBNull.Value)
                return;

            TechStoreManager.FilterToolsTypes(Convert.ToInt32(ToolsGroupGrid.SelectedRows[0].Cells["ToolsGroupID"].Value));
        }

        private void ToolsTypeGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (TechStoreManager == null)
                return;

            if (ToolsTypeGrid.Rows.Count == 0 || ToolsTypeGrid.SelectedRows.Count == 0)
            {
                TechStoreManager.FilterToolsSubTypes(0);
                ToolsAttachDocumentsDT.Clear();
                return;
            }

            if (ToolsTypeGrid.SelectedRows[0].Cells["ToolsTypeID"].Value == DBNull.Value)
                return;

            TechStoreManager.FilterToolsSubTypes(Convert.ToInt32(ToolsTypeGrid.SelectedRows[0].Cells["ToolsTypeID"].Value));
        }

        private void ToolsSubTypeGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (TechStoreManager == null)
                return;

            if (ToolsSubTypeGrid.Rows.Count == 0 || ToolsSubTypeGrid.SelectedRows.Count == 0)
            {
                TechStoreManager.FilterTools(0);
                ToolsAttachDocumentsDT.Clear();
                return;
            }

            if (ToolsSubTypeGrid.SelectedRows[0].Cells["ToolsSubTypeID"].Value == DBNull.Value)
                return;

            TechStoreManager.FilterTools(Convert.ToInt32(ToolsSubTypeGrid.SelectedRows[0].Cells["ToolsSubTypeID"].Value));
        }

        private void RemoveToolsGroupButton_Click(object sender, EventArgs e)
        {
            if (ToolsGroupGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Инструмент: Нажата кнопка Удаление группы инструментов");
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            int ToolsGroupID = Convert.ToInt32(ToolsGroupGrid.SelectedRows[0].Cells["ToolsGroupID"].Value);

            if (OKCancel)
            {
                if (TechStoreManager.ToolsUseInCatalogByToolsGroupID(ToolsGroupID))
                    if (!Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Один или более инструментов из группы используются в каталоге. Вы уверены, что хотите удалить?",
                        "Удаление"))
                        return;

                TechStoreManager.RemoveToolsGroup(ToolsGroupID);
            }
            else
            {
                TechCatalogEvents.SaveEvents("Инструмент: отмена");
            }
        }

        private void AddToolsGroupButton_Click(object sender, EventArgs e)
        {
            TechCatalogEvents.SaveEvents("Инструмент: Нажата кнопка Добавление группы инструментов");
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewToolsGroupForm NewToolsGroupForm = new NewToolsGroupForm(ToolsFactoryComboBox.SelectedItem.ToString(), ref TechStoreManager);

            TopForm = NewToolsGroupForm;

            NewToolsGroupForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (NewToolsGroupForm.Ok)
            {
                ToolsFactoryComboBox.SelectedItem = NewToolsGroupForm.FactoryComboBox.SelectedItem;
                TechStoreManager.ToolsGroupsBS.MoveLast();
            }

            NewToolsGroupForm.Dispose();
        }

        private void EditToolsGroupButton_Click(object sender, EventArgs e)
        {
            if (ToolsGroupGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Инструмент: Нажата кнопка Редактирование группы инструментов");
            int ToolsGroupID = Convert.ToInt32(ToolsGroupGrid.SelectedRows[0].Cells["ToolsGroupID"].Value);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewToolsGroupForm NewToolsGroupForm = new NewToolsGroupForm(ToolsFactoryComboBox.SelectedItem.ToString(), ToolsGroupID, ref TechStoreManager);

            TopForm = NewToolsGroupForm;

            NewToolsGroupForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (NewToolsGroupForm.Ok)
            {
                ToolsFactoryComboBox.SelectedItem = NewToolsGroupForm.FactoryComboBox.SelectedItem;
                TechStoreManager.ToolsGroupsBS.Position = TechStoreManager.ToolsGroupsBS.Find("ToolsGroupID", ToolsGroupID);
            }

            NewToolsGroupForm.Dispose();
        }

        private void AddToolsTypeButton_Click(object sender, EventArgs e)
        {
            if (ToolsGroupGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Инструмент: Нажата кнопка Добавление типа инструментов");
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewToolsTypeForm NewToolsTypeForm = new NewToolsTypeForm(ref TechStoreManager);

            TopForm = NewToolsTypeForm;

            NewToolsTypeForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (NewToolsTypeForm.Ok)
            {
                TechStoreManager.ToolsTypesBS.MoveLast();
            }

            NewToolsTypeForm.Dispose();
        }

        private void EditToolsTypeButton_Click(object sender, EventArgs e)
        {
            if (ToolsTypeGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Инструмент: Нажата кнопка Редактирование типа инструментов");
            int ToolsTypeID = Convert.ToInt32(ToolsTypeGrid.SelectedRows[0].Cells["ToolsTypeID"].Value);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewToolsTypeForm NewToolsTypeForm = new NewToolsTypeForm(ToolsTypeID, ref TechStoreManager);

            TopForm = NewToolsTypeForm;

            NewToolsTypeForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (NewToolsTypeForm.Ok)
            {
                TechStoreManager.ToolsTypesBS.Position = TechStoreManager.ToolsTypesBS.Find("ToolsTypeID", ToolsTypeID);
            }

            NewToolsTypeForm.Dispose();
        }

        private void RemoveToolsTypeButton_Click(object sender, EventArgs e)
        {
            if (ToolsTypeGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Инструмент: Нажата кнопка Удаление типа инструментов");
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            int ToolsTypeID = Convert.ToInt32(ToolsTypeGrid.SelectedRows[0].Cells["ToolsTypeID"].Value);

            if (OKCancel)
            {
                if (TechStoreManager.ToolsUseInCatalogByToolsTypeID(ToolsTypeID))
                    if (!Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Один или более инструментов из типа используются в каталоге. Вы уверены, что хотите удалить?",
                        "Удаление"))
                        return;

                TechStoreManager.RemoveToolsType(ToolsTypeID);
            }
            else
            {
                TechCatalogEvents.SaveEvents("Инструмент: отмена");
            }
        }

        private void AddToolsSubTypeButton_Click(object sender, EventArgs e)
        {
            if (ToolsTypeGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Инструмент: Нажата кнопка Добавление подтипа инструментов");
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewToolsSubTypeForm NewToolsSubTypeForm = new NewToolsSubTypeForm(ref TechStoreManager);

            TopForm = NewToolsSubTypeForm;

            NewToolsSubTypeForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (NewToolsSubTypeForm.Ok)
            {
                TechStoreManager.ToolsSubTypesBS.MoveLast();
            }

            NewToolsSubTypeForm.Dispose();
        }

        private void EditToolsSubTypeButton_Click(object sender, EventArgs e)
        {
            if (ToolsSubTypeGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Инструмент: Нажата кнопка Редактирование подтипа инструментов");
            int ToolsSubTypeID = Convert.ToInt32(ToolsSubTypeGrid.SelectedRows[0].Cells["ToolsSubTypeID"].Value);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewToolsSubTypeForm NewToolsSubTypeForm = new NewToolsSubTypeForm(ToolsSubTypeID, ref TechStoreManager);

            TopForm = NewToolsSubTypeForm;

            NewToolsSubTypeForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (NewToolsSubTypeForm.Ok)
            {
                TechStoreManager.ToolsSubTypesBS.Position = TechStoreManager.ToolsSubTypesBS.Find("ToolsSubTypeID", ToolsSubTypeID);
            }

            NewToolsSubTypeForm.Dispose();
        }

        private void RemoveToolsSubTypeButton_Click(object sender, EventArgs e)
        {
            if (ToolsSubTypeGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Инструмент: Нажата кнопка Удаление подтипа инструментов");
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            int ToolsSubTypeID = Convert.ToInt32(ToolsSubTypeGrid.SelectedRows[0].Cells["ToolsSubTypeID"].Value);

            if (OKCancel)
            {
                if (TechStoreManager.ToolsUseInCatalogByToolsSubTypeID(ToolsSubTypeID))
                    if (!Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Один или более инструментов из подтипа используются в каталоге. Вы уверены, что хотите удалить?",
                        "Удаление"))
                        return;

                TechStoreManager.RemoveToolsSubType(ToolsSubTypeID);
            }
            else
            {
                TechCatalogEvents.SaveEvents("Инструмент: отмена");
            }
        }

        private void AddToolsButton_Click(object sender, EventArgs e)
        {
            if (ToolsSubTypeGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Инструмент: Нажата кнопка Добавление инструмента");
            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewToolsForm NewToolsForm = new NewToolsForm(ref TechStoreManager, Convert.ToInt32(ToolsSubTypeGrid.SelectedRows[0].Cells["ToolsSubTypeID"].Value));

            TopForm = NewToolsForm;

            NewToolsForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (NewToolsForm.Ok)
            {
                TechStoreManager.UpdateToolsNameColumn(ref TechCatalogToolsGrid);
                TechStoreManager.ToolsBS.MoveLast();
            }

            NewToolsForm.Dispose();
        }

        private void EditToolsButton_Click(object sender, EventArgs e)
        {
            if (ToolsGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Инструмент: Нажата кнопка Редактирование инструмента");
            int ToolsID = Convert.ToInt32(ToolsGrid.SelectedRows[0].Cells["ToolsID"].Value);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewToolsForm NewToolsForm = new NewToolsForm(ToolsID, ref TechStoreManager);

            TopForm = NewToolsForm;

            NewToolsForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            if (NewToolsForm.Ok)
            {
                TechStoreManager.ToolsBS.Position = TechStoreManager.ToolsBS.Find("ToolsID", ToolsID);
            }

            NewToolsForm.Dispose();

            TechStoreManager.UpdateToolsNameColumn(ref TechCatalogToolsGrid);
        }

        private void RemoveToolsButton_Click(object sender, EventArgs e)
        {
            if (ToolsGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Инструмент: Нажата кнопка Удаление инструмента");
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            int ToolsID = Convert.ToInt32(ToolsGrid.SelectedRows[0].Cells["ToolsID"].Value);

            if (OKCancel)
            {
                if (TechStoreManager.ToolsUseInCatalog(ToolsID))
                    if (!Infinium.LightMessageBox.Show(ref TopForm, true,
                        "Инструмент используется в каталоге. Вы уверены, что хотите удалить?",
                        "Удаление"))
                        return;

                TechStoreManager.RemoveTools(ToolsID);
            }
            else
            {
                TechCatalogEvents.SaveEvents("Инструмент: отмена");
            }
        }

        private void ToolsGrid_SelectionChanged(object sender, EventArgs e)
        {
            ToolsValueParametrsGrid.DataSource = null;
            ToolsAttachDocumentsDT.Clear();

            if (ToolsGrid.SelectedRows.Count == 0 || TechStoreManager == null || ToolsGrid.SelectedRows[0].Cells["ToolsID"].Value == DBNull.Value)
                return;

            using (DataTable ValueParametrsDT = TechStoreManager.ReadValueTable(ToolsGrid.SelectedRows[0].Cells["ValueParametrs"].Value.ToString(), ToolsSubTypeGrid.SelectedRows[0].Cells["Parametrs"].Value.ToString()))
            {
                ToolsValueParametrsGrid.DataSource = ValueParametrsDT;
                ToolsValueParametrsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                DataGridViewComboBoxColumn ParametrName = new DataGridViewComboBoxColumn()
                {
                    DataSource = TechStoreManager.CurrentParametrsDT,
                    DisplayMember = "ParametrName",
                    ValueMember = "ParametrID",
                    DataPropertyName = "ParametrID",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
                };
                ToolsValueParametrsGrid.Columns.Add(ParametrName);

                ToolsValueParametrsGrid.Columns["ParametrID"].Visible = false;
                ParametrName.DisplayIndex = 0;
            }
            ToolsAttachDocumentsDT.Clear();
            CopyToolsAttachs(ToolsGrid.SelectedRows[0].Cells["ToolsID"].Value.ToString());

            //ToolPictureBox.Image = TechStoreManager.ReadPicture(ToolsGrid.SelectedRows[0].Cells["ToolsID"].Value.ToString());
            //if (ToolPictureBox.Image == null)
            //{
            //    DeletePictureButton.Visible = false;
            //    ToolPictureBox.Cursor = Cursors.Default;
            //}
            //else
            //{
            //    DeletePictureButton.Visible = true;
            //    ToolPictureBox.Cursor = Cursors.Hand;
            //}
        }

        private void Catalog_TechStoreGroupsGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (TechStoreManager == null)
                return;

            if (!TechStoreManager.HasTechStoreGroups)
            {
                TechStoreManager.FilterTechStoreSubGroups(0);
                AttachDocumentsDT.Clear();
                return;
            }

            if (cmbxTechStoreGroups.SelectedItem == null)
                return;

            if (((DataRowView)cmbxTechStoreGroups.SelectedItem).Row["TechStoreGroupID"] == DBNull.Value)
                return;

            int TechStoreGroupID = Convert.ToInt32(((DataRowView)cmbxTechStoreGroups.SelectedItem).Row["TechStoreGroupID"]);

            TechStoreManager.FilterTechStoreSubGroups(TechStoreGroupID);
        }

        private void Catalog_TechStoreSubGroupsGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (TechStoreManager == null)
                return;

            if (!TechStoreManager.HasTechStoreSubGroups)
            {
                TechStoreManager.FilterTechStore(0);
                AttachDocumentsDT.Clear();
                return;
            }

            if (cmbxTechStoreSubGroups.SelectedItem == null)
                return;

            if (((DataRowView)cmbxTechStoreSubGroups.SelectedItem).Row["TechStoreSubGroupID"] == DBNull.Value)
                return;

            int TechStoreSubGroupID = Convert.ToInt32(((DataRowView)cmbxTechStoreSubGroups.SelectedItem).Row["TechStoreSubGroupID"]);

            TechStoreManager.FilterTechStore(TechStoreSubGroupID);
            CheckTechStoreColumns();

        }

        private void CheckTechStoreColumns()
        {
            bool IsNotesEmpty = true;

            foreach (DataGridViewRow Row in TechStoreGrid.Rows)
            {
                if (Row.Cells["Notes"].FormattedValue.ToString().Length > 0)
                {
                    IsNotesEmpty = false;
                    break;
                }
            }
            if (IsNotesEmpty)
            {
                //TechStoreGrid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                TechStoreGrid.Columns["Notes"].Visible = false;
            }
            else
            {
                //TechStoreGrid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                TechStoreGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                TechStoreGrid.Columns["Notes"].Visible = true;
            }
        }

        private void CheckTechStoreDetailsColumns()
        {
            bool IsNotesEmpty = true;

            foreach (DataGridViewColumn Column in TechCatalogStoreDetailGrid.Columns)
            {
                foreach (DataGridViewRow Row in TechCatalogStoreDetailGrid.Rows)
                {
                    if (Column.Name == "GroupA"
                        || Column.Name == "GroupB"
                        || Column.Name == "GroupC"
                        || Column.Name == "Count"
                        || Column.Name == "Width"
                        || Column.Name == "Width1"
                        || Column.Name == "TechCatalogStoreDetailID"
                        || Column.Name == "TechCatalogOperationsDetailID"
                        || Column.Name == "TechStoreID"
                        || Column.Name == "IsSubGroup")
                        continue;
                    if (Row.Cells[Column.Index].Value != DBNull.Value)
                    {
                        TechCatalogStoreDetailGrid.Columns[Column.Index].Visible = true;
                        break;
                    }
                    else
                        TechCatalogStoreDetailGrid.Columns[Column.Index].Visible = false;
                }
            }

            foreach (DataGridViewRow Row in TechCatalogStoreDetailGrid.Rows)
            {
                if (Row.Cells["Notes"].FormattedValue.ToString().Length > 0)
                {
                    IsNotesEmpty = false;
                    break;
                }
            }
            if (IsNotesEmpty)
            {
                //TechCatalogStoreDetailGrid.Columns["TechName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                TechCatalogStoreDetailGrid.Columns["Notes"].Visible = false;
            }
            else
            {
                //TechCatalogStoreDetailGrid.Columns["TechName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //TechCatalogStoreDetailGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                TechCatalogStoreDetailGrid.Columns["Notes"].Visible = true;
            }
        }

        private void RemoveTechCatalogToolsButton_Click(object sender, EventArgs e)
        {
            if (TechCatalogToolsGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Открепление инструмента от склада");

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                TechStoreManager.RemoveTechCatalogTools(Convert.ToInt32(TechCatalogToolsGrid.SelectedRows[0].Cells["TechCatalogToolsID"].Value));
            }
            else
            {
                TechCatalogEvents.SaveEvents("Склад: отмена");
            }
        }

        private void AddTechCatalogToolsButton_Click(object sender, EventArgs e)
        {
            if (TechCatalogOperationsDetailGrid.SelectedRows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Добавьте операци.",
                   "Добавление инструмента");
                return;
            }
            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Прикрепление инструмента к складу");
            AddToolsClick = true;
            ToolsMenuButton.Checked = true;
        }

        private void AddTechCatalogToolsButton2_Click(object sender, EventArgs e)
        {
            if (ToolsGrid.SelectedRows.Count == 0 || TechCatalogOperationsDetailGrid.SelectedRows.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Прикрепление инструмента к складу");
            int ToolsCount = 0;

            try
            {
                ToolsCount = Convert.ToInt32(ToolsCountTextBox.Text);
            }
            catch
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                                "Некорректное значение количества!",
                                "Ошибка");
                return;
            }

            TechStoreManager.AddTechCatalogTools(Convert.ToInt32(TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value), Convert.ToInt32(ToolsGrid.SelectedRows[0].Cells["ToolsID"].Value), ToolsCount);
            ToolsCountTextBox.Text = "";

            InfiniumTips.ShowTip(this, 50, 85, "Инструмент прикреплен к складу", 1700);
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog1.FileName);

            if (fileInfo.Length > 20971520)
            {
                MessageBox.Show("Файл больше 20 МБ и не может быть загружен");
                return;
            }

            AttachDT.Clear();

            DataRow NewRow = AttachDT.NewRow();
            NewRow["FileName"] = openFileDialog1.SafeFileName;
            NewRow["Path"] = openFileDialog1.FileName;
            AttachDT.Rows.Add(NewRow);

            if (AttachDT.Rows.Count != 0)
            {
                if (TechCatalogOperationsDetailGrid.SelectedRows.Count == 1)
                    TechStoreManager.AttachOperationsDocument(AttachDT, TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value.ToString());
            }

            AttachDocumentsDT.Clear();
            CopyAttachs(TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value.ToString());
        }

        private void DeletePictureButton_Click(object sender, EventArgs e)
        {
            if (ToolsAttachDocumentsBS.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Инструмент: Нажата кнопка Удаление документа");
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                TechStoreManager.RemoveToolsDocuments(ToolsGrid.SelectedRows[0].Cells["ToolsID"].Value.ToString(),
                    ToolsAttachDocumentsGrid.SelectedRows[0].Cells["FileName"].Value.ToString());
                ToolsAttachDocumentsDT.Clear();
                CopyToolsAttachs(ToolsGrid.SelectedRows[0].Cells["ToolsID"].Value.ToString());
            }
            else
            {
                TechCatalogEvents.SaveEvents("Инструмент: Отмена Удаление документа");
            }
        }

        private void AddDocumentsButton_Click(object sender, EventArgs e)
        {
            if (TechCatalogOperationsDetailGrid.SelectedRows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Добавьте операцию",
                   "Добавление документов");
                return;
            }
            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Прикрепление документа к операции");
            openFileDialog1.ShowDialog();
        }

        private void RemoveDocumentsButton_Click(object sender, EventArgs e)
        {
            if (AttachDocumentsBS.Count == 0)
                return;

            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Открепление документа от операции");
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                TechStoreManager.RemoveOperationsDocuments(TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value.ToString(),
                    AttachDocumentsGrid.SelectedRows[0].Cells["FileName"].Value.ToString());
                AttachDocumentsDT.Clear();
                CopyAttachs(TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value.ToString());
            }
            else
            {
                TechCatalogEvents.SaveEvents("Склад: Отмена Открепление документа от операции");
            }
        }

        System.Threading.Thread T;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (FM.TotalFileSize == 0)
                return;

            if (FM.Position == FM.TotalFileSize || FM.Position > FM.TotalFileSize)
            {
                timer1.Enabled = false;
                return;
            }
        }

        private void AttachmentsDocumentGrid_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string temppath = "";

            {
                T = new System.Threading.Thread(delegate ()
                { temppath = TechStoreManager.SaveOperationsDocuments(AttachDocumentsGrid.SelectedRows[0].Cells["FileName"].Value.ToString()); });
                T.Start();

                while (T.IsAlive)
                {
                    T.Join(50);
                    Application.DoEvents();

                    if (bStopTransfer)
                    {
                        FM.bStopTransfer = true;
                        bStopTransfer = false;
                        timer1.Enabled = false;
                        return;
                    }
                }

                if (!bStopTransfer && temppath != null)
                    System.Diagnostics.Process.Start(temppath);
            }
        }

        private void AddPictureButton_Click(object sender, EventArgs e)
        {
            TechCatalogEvents.SaveEvents("Инструмент: Нажата кнопка Добавление изображения");
            openFileDialog2.ShowDialog();

            //ToolPictureBox.Image = TechStoreManager.ReadPicture(ToolsGrid.SelectedRows[0].Cells["ToolsID"].Value.ToString());

            //if (ToolPictureBox.Image == null)
            //{
            //    DeletePictureButton.Visible = false;
            //    ToolPictureBox.Cursor = Cursors.Default;
            //}
            //else
            //{
            //    DeletePictureButton.Visible = true;
            //    ToolPictureBox.Cursor = Cursors.Hand;
            //}
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            NeedSplash = false;
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void TechCatalogForm_Load(object sender, EventArgs e)
        {
            cmbxTechStoreGroups_SelectionChangeCommitted(null, null);
            GridSettings();
        }

        private void Initialize()
        {

            CabFurDT = new DataTable();
            CabFurDT.Columns.Add(new DataColumn("TechStoreName", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("TechStoreSubGroupName", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("SubGroupNotes", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("SubGroupNotes1", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("SubGroupNotes2", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("Color", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("Color2", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("Height", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("Width", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("Length", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("PositionsCount", Type.GetType("System.String")));
            CabFurDT.Columns.Add(new DataColumn("LabelsCount", Type.GetType("System.Int32")));
            CabFurDT.Columns.Add(new DataColumn("FactoryType", Type.GetType("System.Int32")));
            CabFurDT.Columns.Add(new DataColumn("DecorConfigID", Type.GetType("System.Int32")));

            CabFurLabelManager = new CabFurLabel();

            TechStoreManager = new TechStoreManager();
            //TechStoreManager.SaveCoatingRollersFromExcel();
            //TechStoreManager.ReplaceAccountingName();
            //TechStoreManager.DataSetIntoDBF();
            //TechStoreManager.SaveCabFurnitureDocumentTypesFromExcel();
            TechStoreManager.Initialize();
            DataBinding();
            GridSettings();

            TechStoreManager.GetPermissions(Security.CurrentUserID, this.Name);
            if (TechStoreManager.PermissionGranted(iToolsEdit))
            {
                bToolsRole = true;
                EditToolsPanel.Visible = true;
                EditTechCatalogToolsPanel.Visible = true;
                panel5.Visible = true;
            }
            if (TechStoreManager.PermissionGranted(iMachinesEdit))
            {
                panel24.Visible = true;
                EditMachinePhotoButton.Visible = true;
            }
            if (TechStoreManager.PermissionGranted(iStoreEdit))
            {
                panel26.Visible = true;
                panel1.Visible = true;
                panel3.Visible = true;
            }
            if (TechStoreManager.PermissionGranted(iOperationsEdit))
            {
                bOperationsRole = true;
                EditOperationsPanel.Visible = true;
            }
            if (TechStoreManager.PermissionGranted(iAdmin))
            {
                panel24.Visible = true;
                EditMachinePhotoButton.Visible = true;
                panel26.Visible = true;
                panel1.Visible = true;
                panel3.Visible = true;
                bToolsRole = true;
                EditToolsPanel.Visible = true;
                EditTechCatalogToolsPanel.Visible = true;
                panel5.Visible = true;
                bOperationsRole = true;
                EditOperationsPanel.Visible = true;
            }

            int FactoryID;

            if (ToolsFactoryComboBox.SelectedItem.ToString() == "Профиль")
                FactoryID = 1;
            else
                FactoryID = 2;

            TechStoreManager.FilterToolsGroups(FactoryID);

            if (FactoryComboBox.SelectedItem.ToString() == "Профиль")
                FactoryID = 1;
            else
                FactoryID = 2;

            TechStoreManager.FilterSectors(FactoryID);
        }

        private void DataBinding()
        {
            TechCatalogStoreDetailGrid.DataSource = TechStoreManager.TechCatalogStoreDetailList;
            TechCatalogToolsGrid.DataSource = TechStoreManager.TechCatalogToolsList;
            TechCatalogOperationsDetailGrid.DataSource = TechStoreManager.TechCatalogOperationsDetailList;
            TechStoreGrid.DataSource = TechStoreManager.TechStoreList;
            GroupsOperationsDetailGrid.DataSource = TechStoreManager.TechCatalogOperationsDetailGroupsList;
            cmbxTechStoreSubGroups.DataSource = TechStoreManager.TechStoreSubGroupsList;
            cmbxTechStoreSubGroups.DisplayMember = "TechStoreSubGroupName";
            cmbxTechStoreSubGroups.ValueMember = "TechStoreSubGroupID";
            cmbxTechStoreGroups.DataSource = TechStoreManager.TechStoreGroupsList;
            cmbxTechStoreGroups.DisplayMember = "TechStoreGroupName";
            cmbxTechStoreGroups.ValueMember = "TechStoreGroupID";
            SubSectorsGrid.DataSource = TechStoreManager.SubSectorsList;
            SectorsGrid.DataSource = TechStoreManager.SectorsList;
            MachinesGrid.DataSource = TechStoreManager.MachinesList;
            AllMachinesGrid.DataSource = TechStoreManager.AllMachinesList;
            MachinesOperationsGrid.DataSource = TechStoreManager.MachinesOperationsList;
            OperationsOnMachineGrid.DataSource = TechStoreManager.MachinesOperationsList;
            ToolsTypeGrid.DataSource = TechStoreManager.ToolsTypesList;
            ToolsSubTypeGrid.DataSource = TechStoreManager.ToolsSubTypesList;
            ToolsGroupGrid.DataSource = TechStoreManager.ToolsGroupsList;
            ToolsGrid.DataSource = TechStoreManager.ToolsList;

            MachinesOperationsGrid.Columns.Add(TechStoreManager.MeasureColumn);
            MachinesOperationsGrid.Columns.Add(TechStoreManager.DocTypesColumn);
            MachinesOperationsGrid.Columns.Add(TechStoreManager.AlgorithmsColumn);

            OperationsOnMachineGrid.Columns.Add(TechStoreManager.OperationsOnMachineMeasureColumn);

            TechCatalogOperationsDetailGrid.Columns.Add(TechStoreManager.MachinesOperationNameColumn);
            TechCatalogOperationsDetailGrid.Columns.Add(TechStoreManager.MachinesOperationArticleColumn);
            TechCatalogOperationsDetailGrid.Columns.Add(TechStoreManager.MachineNameColumn);
            //TechCatalogOperationsDetailGrid.Columns.Add(TechStoreManager.SubSectorNameColumn);
            //TechCatalogOperationsDetailGrid.Columns.Add(TechStoreManager.SectorNameColumn);

            TechCatalogToolsGrid.Columns.Add(TechStoreManager.ToolsNameColumn);

        }

        private void GridSettings()
        {
            ToolsGroupGrid.Columns["FactoryID"].Visible = false;
            ToolsGroupGrid.Columns["ToolsGroupID"].Visible = false;

            ToolsTypeGrid.Columns["ToolsTypeID"].Visible = false;
            ToolsTypeGrid.Columns["ToolsGroupID"].Visible = false;

            ToolsSubTypeGrid.Columns["ToolsSubTypeID"].Visible = false;
            ToolsSubTypeGrid.Columns["ToolsTypeID"].Visible = false;
            ToolsSubTypeGrid.Columns["Parametrs"].Visible = false;

            ToolsGrid.Columns["ToolsID"].Visible = false;
            ToolsGrid.Columns["ToolsSubTypeID"].Visible = false;
            ToolsGrid.Columns["ValueParametrs"].Visible = false;

            TechStoreGrid.Columns["TechStoreID"].Visible = false;
            TechStoreGrid.Columns["TechStoreSubGroupID"].Visible = false;
            TechStoreGrid.Columns["MeasureID"].Visible = false;
            TechStoreGrid.Columns["TechStoreName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TechStoreGrid.Columns["TechStoreName"].MinimumWidth = 100;
            TechStoreGrid.Columns["Notes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TechStoreGrid.Columns["Notes"].Width = 100;

            SectorsGrid.Columns["SectorID"].Visible = false;
            SectorsGrid.Columns["FactoryID"].Visible = false;

            SubSectorsGrid.Columns["SubSectorID"].Visible = false;
            SubSectorsGrid.Columns["SectorID"].Visible = false;

            MachinesGrid.Columns["MachineID"].Visible = false;
            MachinesGrid.Columns["SubSectorID"].Visible = false;
            MachinesGrid.Columns["Parametrs"].Visible = false;
            MachinesGrid.Columns["ValueParametrs"].Visible = false;

            AllMachinesGrid.Columns["MachineID"].Visible = false;
            AllMachinesGrid.Columns["SubSectorID"].Visible = false;
            AllMachinesGrid.Columns["Parametrs"].Visible = false;
            AllMachinesGrid.Columns["ValueParametrs"].Visible = false;

            foreach (DataGridViewColumn Column in MachinesOperationsGrid.Columns)
            {
                Column.ReadOnly = true;
            }
            MachinesOperationsGrid.Columns["CabFurDocTypeID"].Visible = false;
            MachinesOperationsGrid.Columns["CabFurAlgorithmID"].Visible = false;
            MachinesOperationsGrid.Columns["Norm"].ReadOnly = false;
            MachinesOperationsGrid.Columns["PreparatoryNorm"].ReadOnly = false;
            MachinesOperationsGrid.Columns["Norm"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MachinesOperationsGrid.Columns["Norm"].Width = 100;
            MachinesOperationsGrid.Columns["Norm"].HeaderText = "Норма";
            MachinesOperationsGrid.Columns["Norm"].SortMode = DataGridViewColumnSortMode.NotSortable;
            MachinesOperationsGrid.Columns["PreparatoryNorm"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MachinesOperationsGrid.Columns["PreparatoryNorm"].Width = 100;
            MachinesOperationsGrid.Columns["PreparatoryNorm"].HeaderText = "Доп. норма";
            MachinesOperationsGrid.Columns["PreparatoryNorm"].SortMode = DataGridViewColumnSortMode.NotSortable;
            MachinesOperationsGrid.Columns["MachinesOperationName"].MinimumWidth = 250;
            MachinesOperationsGrid.Columns["MachinesOperationName"].HeaderText = "Операция";
            MachinesOperationsGrid.Columns["MachinesOperationName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            MachinesOperationsGrid.Columns["MachinesOperationName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            MachinesOperationsGrid.Columns["Article"].HeaderText = "Артикул";
            MachinesOperationsGrid.Columns["Article"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MachinesOperationsGrid.Columns["Article"].Width = 80;
            MachinesOperationsGrid.Columns["Article"].SortMode = DataGridViewColumnSortMode.NotSortable;
            MachinesOperationsGrid.Columns["MachinesOperationID"].Visible = false;
            MachinesOperationsGrid.Columns["MachineID"].Visible = false;
            MachinesOperationsGrid.Columns["MeasureID"].Visible = false;
            MachinesOperationsGrid.Columns["PositionID"].Visible = false;
            MachinesOperationsGrid.Columns["Rank"].Visible = false;
            MachinesOperationsGrid.Columns["Rank2"].Visible = false;
            MachinesOperationsGrid.Columns["PositionID2"].Visible = false;
            int DisplayIndex = 0;
            MachinesOperationsGrid.Columns["MachinesOperationName"].DisplayIndex = DisplayIndex++;
            MachinesOperationsGrid.Columns["Article"].DisplayIndex = DisplayIndex++;
            MachinesOperationsGrid.Columns["Norm"].DisplayIndex = DisplayIndex++;
            MachinesOperationsGrid.Columns["PreparatoryNorm"].DisplayIndex = DisplayIndex++;
            MachinesOperationsGrid.Columns["DocTypesColumn"].DisplayIndex = DisplayIndex++;
            MachinesOperationsGrid.Columns["AlgorithmsColumn"].DisplayIndex = DisplayIndex++;

            OperationsOnMachineGrid.Columns["Norm"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OperationsOnMachineGrid.Columns["Norm"].Width = 100;
            OperationsOnMachineGrid.Columns["Norm"].HeaderText = "Норма";
            OperationsOnMachineGrid.Columns["Norm"].SortMode = DataGridViewColumnSortMode.NotSortable;
            OperationsOnMachineGrid.Columns["PreparatoryNorm"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OperationsOnMachineGrid.Columns["PreparatoryNorm"].Width = 100;
            OperationsOnMachineGrid.Columns["PreparatoryNorm"].HeaderText = "Доп. норма";
            OperationsOnMachineGrid.Columns["PreparatoryNorm"].SortMode = DataGridViewColumnSortMode.NotSortable;
            OperationsOnMachineGrid.Columns["MachinesOperationName"].MinimumWidth = 250;
            OperationsOnMachineGrid.Columns["MachinesOperationName"].HeaderText = "Операция";
            OperationsOnMachineGrid.Columns["MachinesOperationName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            OperationsOnMachineGrid.Columns["MachinesOperationName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            OperationsOnMachineGrid.Columns["Article"].HeaderText = "Артикул";
            OperationsOnMachineGrid.Columns["Article"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OperationsOnMachineGrid.Columns["Article"].Width = 80;
            OperationsOnMachineGrid.Columns["Article"].SortMode = DataGridViewColumnSortMode.NotSortable;
            OperationsOnMachineGrid.Columns["MachinesOperationID"].Visible = false;
            OperationsOnMachineGrid.Columns["MachineID"].Visible = false;
            OperationsOnMachineGrid.Columns["MeasureID"].Visible = false;
            OperationsOnMachineGrid.Columns["PositionID"].Visible = false;
            OperationsOnMachineGrid.Columns["Rank"].Visible = false;
            OperationsOnMachineGrid.Columns["Rank2"].Visible = false;
            OperationsOnMachineGrid.Columns["PositionID2"].Visible = false;

            DisplayIndex = 0;
            OperationsOnMachineGrid.Columns["MachinesOperationName"].DisplayIndex = DisplayIndex++;
            OperationsOnMachineGrid.Columns["Article"].DisplayIndex = DisplayIndex++;
            OperationsOnMachineGrid.Columns["Norm"].DisplayIndex = DisplayIndex++;
            OperationsOnMachineGrid.Columns["PreparatoryNorm"].DisplayIndex = DisplayIndex++;

            foreach (DataGridViewColumn Column in TechCatalogOperationsDetailGrid.Columns)
            {
                Column.ReadOnly = true;
            }
            DisplayIndex = 0;
            TechCatalogOperationsDetailGrid.Columns["GroupA"].HeaderText = "О";
            TechCatalogOperationsDetailGrid.Columns["GroupB"].HeaderText = "П";
            TechCatalogOperationsDetailGrid.Columns["GroupC"].HeaderText = "Н";
            TechCatalogOperationsDetailGrid.Columns["GroupA"].ReadOnly = false;
            TechCatalogOperationsDetailGrid.Columns["GroupB"].ReadOnly = false;
            TechCatalogOperationsDetailGrid.Columns["GroupC"].ReadOnly = false;
            TechCatalogOperationsDetailGrid.Columns["MachinesOperationName"].DisplayIndex = DisplayIndex++;
            TechCatalogOperationsDetailGrid.Columns["MachinesOperationArticle"].DisplayIndex = DisplayIndex++;
            TechCatalogOperationsDetailGrid.Columns["MachineName"].DisplayIndex = DisplayIndex++;
            TechCatalogOperationsDetailGrid.Columns["GroupA"].DisplayIndex = DisplayIndex++;
            TechCatalogOperationsDetailGrid.Columns["GroupB"].DisplayIndex = DisplayIndex++;
            TechCatalogOperationsDetailGrid.Columns["GroupC"].DisplayIndex = DisplayIndex++;
            TechCatalogOperationsDetailGrid.Columns["TechCatalogOperationsGroupID"].Visible = false;
            //TechCatalogOperationsDetailGrid.Columns["TechCatalogOperationsDetailID"].Visible = false;
            TechCatalogOperationsDetailGrid.Columns["MachinesOperationID"].Visible = false;
            TechCatalogOperationsDetailGrid.Columns["MachineID"].Visible = false;
            //TechCatalogOperationsDetailGrid.Columns["SubSectorID"].Visible = false;
            //TechCatalogOperationsDetailGrid.Columns["SectorID"].Visible = false;
            //TechCatalogOperationsDetailGrid.Columns["GroupNumber"].Visible = false;
            TechCatalogOperationsDetailGrid.Columns["SerialNumber"].Visible = false;
            TechCatalogOperationsDetailGrid.Columns["IsPerform"].Visible = false;
            //TechCatalogOperationsDetailGrid.Columns["SerialNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //TechCatalogOperationsDetailGrid.Columns["SerialNumber"].Width = 100;

            GroupsOperationsDetailGrid.Columns["TechCatalogOperationsGroupID"].Visible = false;
            GroupsOperationsDetailGrid.Columns["TechStoreID"].Visible = false;
            //GroupsOperationsDetailGrid.Columns["GroupNumber"].Visible = false;
            //GroupsOperationsDetailGrid.Columns["GroupNumber"].ReadOnly = true;
            GroupsOperationsDetailGrid.Columns["GroupName"].HeaderText = "Название";
            GroupsOperationsDetailGrid.Columns["GroupNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            GroupsOperationsDetailGrid.Columns["GroupNumber"].MinimumWidth = 100;
            GroupsOperationsDetailGrid.Columns["GroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            GroupsOperationsDetailGrid.Columns["GroupName"].MinimumWidth = 100;

            TechCatalogStoreDetailGrid.Columns["TechCatalogStoreDetailID"].Visible = false;
            TechCatalogStoreDetailGrid.Columns["TechCatalogOperationsDetailID"].Visible = false;
            TechCatalogStoreDetailGrid.Columns["TechStoreID"].Visible = false;
            TechCatalogStoreDetailGrid.Columns["IsSubGroup"].Visible = false;
            foreach (DataGridViewColumn Column in TechCatalogStoreDetailGrid.Columns)
            {
                Column.ReadOnly = true;
            }
            TechCatalogStoreDetailGrid.Columns["IsHalfStuff1"].ReadOnly = false;
            TechCatalogStoreDetailGrid.Columns["GroupA"].ReadOnly = false;
            TechCatalogStoreDetailGrid.Columns["GroupB"].ReadOnly = false;
            TechCatalogStoreDetailGrid.Columns["GroupC"].ReadOnly = false;
            TechCatalogStoreDetailGrid.Columns["Length"].ReadOnly = false;
            TechCatalogStoreDetailGrid.Columns["Height"].ReadOnly = false;
            TechCatalogStoreDetailGrid.Columns["Width"].ReadOnly = false;
            TechCatalogStoreDetailGrid.Columns["Width1"].ReadOnly = false;
            TechCatalogStoreDetailGrid.Columns["IsHalfStuff2"].ReadOnly = false;
            TechCatalogStoreDetailGrid.Columns["Count"].ReadOnly = false;
            TechCatalogStoreDetailGrid.Columns["BreakChain"].ReadOnly = false;
            TechCatalogStoreDetailGrid.Columns["GroupA"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TechCatalogStoreDetailGrid.Columns["GroupA"].Width = 35;
            TechCatalogStoreDetailGrid.Columns["GroupB"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TechCatalogStoreDetailGrid.Columns["GroupB"].Width = 35;
            TechCatalogStoreDetailGrid.Columns["GroupC"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TechCatalogStoreDetailGrid.Columns["GroupC"].Width = 35;
            TechCatalogStoreDetailGrid.Columns["IsHalfStuff2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TechCatalogStoreDetailGrid.Columns["IsHalfStuff2"].Width = 75;
            TechCatalogStoreDetailGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TechCatalogStoreDetailGrid.Columns["Count"].Width = 85;
            TechCatalogStoreDetailGrid.Columns["Measure"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TechCatalogStoreDetailGrid.Columns["Measure"].Width = 68;
            TechCatalogStoreDetailGrid.Columns["BreakChain"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TechCatalogStoreDetailGrid.Columns["BreakChain"].Width = 75;
            TechCatalogStoreDetailGrid.Columns["IsHalfStuff1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TechCatalogStoreDetailGrid.Columns["IsHalfStuff1"].Width = 75;
            TechCatalogStoreDetailGrid.Columns["IsHalfStuff2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TechCatalogStoreDetailGrid.Columns["IsHalfStuff2"].Width = 75;
            TechCatalogStoreDetailGrid.Columns["TechName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            TechCatalogStoreDetailGrid.Columns["TechName"].MinimumWidth = 145;
            //TechCatalogStoreDetailGrid.Columns["SubGroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //TechCatalogStoreDetailGrid.Columns["GroupName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            TechCatalogStoreDetailGrid.Columns["Length"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            TechCatalogStoreDetailGrid.Columns["Height"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            TechCatalogStoreDetailGrid.Columns["Width"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            TechCatalogStoreDetailGrid.Columns["Width1"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            TechCatalogStoreDetailGrid.Columns["Notes"].MinimumWidth = 200;
            TechCatalogStoreDetailGrid.Columns["GroupA"].HeaderText = "A";
            TechCatalogStoreDetailGrid.Columns["GroupB"].HeaderText = "B";
            TechCatalogStoreDetailGrid.Columns["GroupC"].HeaderText = "C";
            TechCatalogStoreDetailGrid.Columns["Length"].HeaderText = "Длина";
            TechCatalogStoreDetailGrid.Columns["Height"].HeaderText = "Высота";
            TechCatalogStoreDetailGrid.Columns["Width"].HeaderText = "Ширина/4,5 мм";
            TechCatalogStoreDetailGrid.Columns["Width1"].HeaderText = "Ширина/3,2 мм";
            TechCatalogStoreDetailGrid.Columns["TechName"].HeaderText = "Наименование";
            //TechCatalogStoreDetailGrid.Columns["SubGroupName"].HeaderText = "Подгруппа";
            //TechCatalogStoreDetailGrid.Columns["GroupName"].HeaderText = "Группа";
            TechCatalogStoreDetailGrid.Columns["Count"].HeaderText = "Расход";
            TechCatalogStoreDetailGrid.Columns["IsHalfStuff1"].HeaderText = "п/ф 1";
            TechCatalogStoreDetailGrid.Columns["IsHalfStuff2"].HeaderText = "п/ф 2";
            TechCatalogStoreDetailGrid.Columns["Measure"].HeaderText = "Ед.изм.";
            TechCatalogStoreDetailGrid.Columns["Notes"].HeaderText = "Примечание";
            TechCatalogStoreDetailGrid.Columns["BreakChain"].HeaderText = "Отменить";
            TechCatalogStoreDetailGrid.AutoGenerateColumns = false;

            TechCatalogStoreDetailGrid.Columns["GroupA"].DisplayIndex = 1;
            TechCatalogStoreDetailGrid.Columns["GroupB"].DisplayIndex = 2;
            TechCatalogStoreDetailGrid.Columns["GroupC"].DisplayIndex = 3;
            TechCatalogStoreDetailGrid.Columns["TechName"].DisplayIndex = 4;
            //TechCatalogStoreDetailGrid.Columns["SubGroupName"].DisplayIndex = 6;
            //TechCatalogStoreDetailGrid.Columns["GroupName"].DisplayIndex = 7;
            TechCatalogStoreDetailGrid.Columns["Count"].DisplayIndex = 8;
            TechCatalogStoreDetailGrid.Columns["IsHalfStuff1"].DisplayIndex = 9;
            TechCatalogStoreDetailGrid.Columns["IsHalfStuff2"].DisplayIndex = 10;
            TechCatalogStoreDetailGrid.Columns["Measure"].DisplayIndex = 11;
            TechCatalogStoreDetailGrid.Columns["Notes"].DisplayIndex = 12;
            TechCatalogStoreDetailGrid.Columns["BreakChain"].DisplayIndex = 13;

            TechCatalogToolsGrid.Columns["TechCatalogToolsID"].Visible = false;
            TechCatalogToolsGrid.Columns["TechCatalogOperationsDetailID"].Visible = false;
            TechCatalogToolsGrid.Columns["ToolsID"].Visible = false;

            foreach (DataGridViewColumn Column in TechCatalogToolsGrid.Columns)
            {
                Column.ReadOnly = true;
            }
            TechCatalogToolsGrid.Columns["GroupNumber"].ReadOnly = false;
            TechCatalogToolsGrid.Columns["GroupNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TechCatalogToolsGrid.Columns["GroupNumber"].Width = 65;
            TechCatalogToolsGrid.Columns["ToolsName"].MinimumWidth = 150;
            TechCatalogToolsGrid.Columns["Count"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TechCatalogToolsGrid.Columns["Count"].Width = 45;
            TechCatalogToolsGrid.Columns["GroupNumber"].HeaderText = "Группа";
            TechCatalogToolsGrid.Columns["ToolsName"].HeaderText = "Наименование";
            TechCatalogToolsGrid.Columns["Count"].HeaderText = "Кол-во";
            TechCatalogToolsGrid.AutoGenerateColumns = false;
            TechCatalogToolsGrid.Columns["GroupNumber"].DisplayIndex = 0;
            TechCatalogToolsGrid.Columns["ToolsName"].DisplayIndex = 1;
            TechCatalogToolsGrid.Columns["Count"].DisplayIndex = 2;
            TechCatalogToolsGrid.Columns["Count"].Width = 100;
        }

        private void GroupsOperationsDetailGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (TechStoreManager == null)
                return;

            if (GroupsOperationsDetailGrid.SelectedRows.Count == 0)
            {
                TechStoreManager.FilterOperationsDetails(0);
                return;
            }

            if (GroupsOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value == DBNull.Value)
                return;

            int TechCatalogOperationsGroupID = Convert.ToInt32(GroupsOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value);
            TechStoreManager.FilterOperationsDetails(TechCatalogOperationsGroupID);
        }

        private void AddGroupOperationsDetail_Click(object sender, EventArgs e)
        {
            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Добавление группы операций");
            int TechStoreID = Convert.ToInt32(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value);
            TechStoreManager.AddTechCatalogOperationsDetailGroup(TechStoreID);
            TechStoreManager.UpdateTechCatalogOperationsDetailGroups();
        }

        private void btnSaveStoreDetails_Click(object sender, EventArgs e)
        {
            TechCatalogEvents.SaveEvents("Склад: нажата кнопка Сохранение материалов");
            TechStoreManager.UpdateTechCatalogStoreDetails();
            TechStoreManager.RefreshTechCatalogStoreDetails();
            TechCatalogEvents.SaveEvents("Склад: материалы сохранены");
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void btnSaveStoreTools_Click(object sender, EventArgs e)
        {
            TechCatalogEvents.SaveEvents("Склад: нажата кнопка Сохранение инструмента");
            TechStoreManager.UpdateTechCatalogTools();
            TechStoreManager.RefreshTechCatalogTools();
            TechCatalogEvents.SaveEvents("Склад: инструмент сохранен");
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog2.FileName);

            if (fileInfo.Length > 20971520)
            {
                MessageBox.Show("Файл больше 20 МБ и не может быть загружен");
                return;
            }
            ToolsAttachDT.Clear();

            DataRow NewRow = ToolsAttachDT.NewRow();
            NewRow["FileName"] = openFileDialog2.SafeFileName;
            NewRow["Path"] = openFileDialog2.FileName;
            ToolsAttachDT.Rows.Add(NewRow);

            if (ToolsAttachDT.Rows.Count != 0)
            {
                if (ToolsGrid.SelectedRows.Count == 1)
                    TechStoreManager.AttachToolsDocument(ToolsAttachDT, ToolsGrid.SelectedRows[0].Cells["ToolsID"].Value.ToString());
            }

            ToolsAttachDocumentsDT.Clear();
            CopyToolsAttachs(ToolsGrid.SelectedRows[0].Cells["ToolsID"].Value.ToString());
        }

        private void ToolsAttachmentsDocumentGrid_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string temppath = "";

            {
                T = new System.Threading.Thread(delegate ()
                { temppath = TechStoreManager.SaveToolsDocuments(ToolsAttachDocumentsGrid.SelectedRows[0].Cells["FileName"].Value.ToString()); });
                T.Start();

                while (T.IsAlive)
                {
                    T.Join(50);
                    Application.DoEvents();

                    if (bStopTransfer)
                    {
                        FM.bStopTransfer = true;
                        bStopTransfer = false;
                        timer1.Enabled = false;
                        return;
                    }
                }

                if (!bStopTransfer && temppath != null)
                    System.Diagnostics.Process.Start(temppath);
            }
        }

        private void btnGroupNumberMoveUp_Click(object sender, EventArgs e)
        {
            if (GroupsOperationsDetailGrid.SelectedRows.Count == 0
                || GroupsOperationsDetailGrid.Rows.Count == 1)
            {
                return;
            }

            int GroupNumber = Convert.ToInt32(GroupsOperationsDetailGrid.SelectedRows[0].Cells["GroupNumber"].Value);
            int OldOperationsGroupID = Convert.ToInt32(GroupsOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value);
            if (GroupsOperationsDetailGrid.SelectedRows[0].Index - 1 == -1)
            {
                return;
            }

            int NewOldOperationsGroupID = Convert.ToInt32(GroupsOperationsDetailGrid.Rows[GroupsOperationsDetailGrid.SelectedRows[0].Index - 1].Cells["TechCatalogOperationsGroupID"].Value);
            int TechStoreID = Convert.ToInt32(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            TechStoreManager.ChangeGroupNumber(OldOperationsGroupID, NewOldOperationsGroupID);
            TechStoreManager.RefreshTechCatalogOperationsDetail();
            //TechCatalogGrid_SelectionChanged(null, null);

            TechStoreManager.RefreshTechCatalogOperationsDetailGroups();

            TechStoreManager.MoveToGroupNumber(OldOperationsGroupID);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnGroupNumberMoveDown_Click(object sender, EventArgs e)
        {
            if (GroupsOperationsDetailGrid.SelectedRows.Count == 0
                || GroupsOperationsDetailGrid.Rows.Count == 1)
            {
                return;
            }

            int GroupNumber = Convert.ToInt32(GroupsOperationsDetailGrid.SelectedRows[0].Cells["GroupNumber"].Value);
            int OldOperationsGroupID = Convert.ToInt32(GroupsOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value);
            if (GroupsOperationsDetailGrid.SelectedRows[0].Index + 1 == GroupsOperationsDetailGrid.Rows.Count)
            {
                return;
            }

            int NewOldOperationsGroupID = Convert.ToInt32(GroupsOperationsDetailGrid.Rows[GroupsOperationsDetailGrid.SelectedRows[0].Index + 1].Cells["TechCatalogOperationsGroupID"].Value);
            int TechStoreID = Convert.ToInt32(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value);

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            TechStoreManager.ChangeGroupNumber(OldOperationsGroupID, NewOldOperationsGroupID);
            TechStoreManager.RefreshTechCatalogOperationsDetail();
            //TechCatalogGrid_SelectionChanged(null, null);

            TechStoreManager.RefreshTechCatalogOperationsDetailGroups();

            TechStoreManager.MoveToGroupNumber(OldOperationsGroupID);
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void CopyGroupOperationsDetail_Click(object sender, EventArgs e)
        {
            if (GroupsOperationsDetailGrid.SelectedRows.Count == 0)
                return;
            bCopyGroupOperationsDetail = true;
            if (TechCatalogGroupOperationsIDs == null)
                TechCatalogGroupOperationsIDs = new List<int>();
            TechCatalogGroupOperationsIDs.Clear();

            kryptonButton4.Visible = true;
            kryptonButton5.Visible = true;

            TechCatalogGroupOperationsIDs.Add(Convert.ToInt32(GroupsOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value));

            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Копирование группы операций");
        }

        private void btnSerialNumberMoveUp_Click(object sender, EventArgs e)
        {
            if (TechCatalogOperationsDetailGrid.SelectedRows.Count == 0
                || TechCatalogOperationsDetailGrid.SelectedRows[0].Index == 0)
            {
                return;
            }

            int FirstTechCatalogOperationsDetailID = Convert.ToInt32(TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value);
            int SecondTechCatalogOperationsDetailID = Convert.ToInt32(TechCatalogOperationsDetailGrid.Rows[TechCatalogOperationsDetailGrid.SelectedRows[0].Index - 1].Cells["TechCatalogOperationsDetailID"].Value);
            TechStoreManager.ChangeSerialNumber(FirstTechCatalogOperationsDetailID, SecondTechCatalogOperationsDetailID);
            TechStoreManager.RefreshTechCatalogOperationsDetail();
            GroupsOperationsDetailGrid_SelectionChanged(null, null);
            TechStoreManager.MoveToSerialNumber(FirstTechCatalogOperationsDetailID);
        }

        private void btnSerialNumberMoveDown_Click(object sender, EventArgs e)
        {
            if (TechCatalogOperationsDetailGrid.SelectedRows.Count == 0
                || TechCatalogOperationsDetailGrid.SelectedRows[0].Index + 1 == TechCatalogOperationsDetailGrid.Rows.Count)
            {
                return;
            }

            int FirstTechCatalogOperationsDetailID = Convert.ToInt32(TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value);
            int SecondTechCatalogOperationsDetailID = Convert.ToInt32(TechCatalogOperationsDetailGrid.Rows[TechCatalogOperationsDetailGrid.SelectedRows[0].Index + 1].Cells["TechCatalogOperationsDetailID"].Value);
            TechStoreManager.ChangeSerialNumber(FirstTechCatalogOperationsDetailID, SecondTechCatalogOperationsDetailID);
            TechStoreManager.RefreshTechCatalogOperationsDetail();
            GroupsOperationsDetailGrid_SelectionChanged(null, null);
            TechStoreManager.MoveToSerialNumber(FirstTechCatalogOperationsDetailID);
        }

        private void FilterTechStoreSubGroups()
        {
            if (TechStoreManager == null)
                return;

            if (!TechStoreManager.HasTechStoreGroups)
            {
                TechStoreManager.FilterTechStoreSubGroups(0);
                AttachDocumentsDT.Clear();
                return;
            }

            if (cmbxTechStoreGroups.SelectedItem == null)
                return;

            if (((DataRowView)cmbxTechStoreGroups.SelectedItem).Row["TechStoreGroupID"] == DBNull.Value)
                return;

            int TechStoreGroupID = Convert.ToInt32(((DataRowView)cmbxTechStoreGroups.SelectedItem).Row["TechStoreGroupID"]);

            TechStoreManager.FilterTechStoreSubGroups(TechStoreGroupID);
        }

        private void FilterTechStore()
        {
            if (TechStoreManager == null)
                return;

            if (!TechStoreManager.HasTechStoreSubGroups)
            {
                TechStoreManager.FilterTechStore(0);
                AttachDocumentsDT.Clear();
                return;
            }

            if (cmbxTechStoreSubGroups.SelectedItem == null)
                return;

            if (((DataRowView)cmbxTechStoreSubGroups.SelectedItem).Row["TechStoreSubGroupID"] == DBNull.Value)
                return;

            int TechStoreSubGroupID = Convert.ToInt32(((DataRowView)cmbxTechStoreSubGroups.SelectedItem).Row["TechStoreSubGroupID"]);

            TechStoreManager.FilterTechStore(TechStoreSubGroupID);
        }

        private void cmbxTechStoreGroups_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //FilterTechStoreSubGroups();
            //cmbxTechStoreSubGroups_SelectionChangeCommitted(null, null);
        }

        private void cmbxTechStoreSubGroups_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //FilterTechStore();
            //CheckTechStoreColumns();
        }

        private void openFileDialog3_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog3.FileName);

            if (fileInfo.Length > 20971520)
            {
                MessageBox.Show("Файл больше 20 МБ и не может быть загружен");
                return;
            }
            TechStoreAttachDT.Clear();

            DataRow NewRow = TechStoreAttachDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog3.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog3.FileName;
            TechStoreAttachDT.Rows.Add(NewRow);

            if (TechStoreAttachDT.Rows.Count != 0)
            {
                if (TechStoreGrid.SelectedRows.Count == 1)
                    TechStoreManager.AttachTechStoreDocument(TechStoreAttachDT, TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value.ToString(), 1);
            }

            TechStoreAttachDocumentsDT.Clear();
            CopyTechStoreAttachs(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value.ToString());
        }

        private void TechStoreAttachmentsDocumentGrid_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string temppath = "";

            {
                T = new System.Threading.Thread(delegate ()
                { temppath = TechStoreManager.SaveTechStoreDocuments(Convert.ToInt32(TechStoreAttachDocumentsGrid.SelectedRows[0].Cells["TechStoreDocumentID"].Value)); });
                T.Start();

                while (T.IsAlive)
                {
                    T.Join(50);
                    Application.DoEvents();

                    if (bStopTransfer)
                    {
                        FM.bStopTransfer = true;
                        bStopTransfer = false;
                        timer1.Enabled = false;
                        return;
                    }
                }

                if (!bStopTransfer && temppath != null)
                    System.Diagnostics.Process.Start(temppath);
            }
        }

        private void AddTechStoreDocumentsButton_Click(object sender, EventArgs e)
        {
            if (TechStoreGrid.SelectedRows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Нет наименования",
                   "Добавление документов");
                return;
            }
            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Прикрепление документа к наименованию");
            openFileDialog3.ShowDialog();
        }

        private void RemoveTechStoreDocumentsButton_Click(object sender, EventArgs e)
        {
            if (TechStoreAttachDocumentsDT.Select("DocType = 1").Count() == 0)
                return;

            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Открепление документа от наименования");
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                TechStoreManager.RemoveTechStoreDocuments(Convert.ToInt32(TechStoreAttachDocumentsGrid.SelectedRows[0].Cells["TechStoreDocumentID"].Value));
                TechStoreAttachDocumentsDT.Clear();
                CopyTechStoreAttachs(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value.ToString());
            }
            else
            {
                TechCatalogEvents.SaveEvents("Склад: Отмена Открепление документа от наименования");
            }
        }

        private void openFileDialog4_FileOk(object sender, CancelEventArgs e)
        {
            var fileInfo = new System.IO.FileInfo(openFileDialog4.FileName);

            if (fileInfo.Length > 1500000)
            {
                MessageBox.Show("Файл больше 1,5 МБ и не может быть загружен");
                return;
            }
            TechStoreAttachDT.Clear();

            DataRow NewRow = TechStoreAttachDT.NewRow();
            NewRow["FileName"] = Path.GetFileNameWithoutExtension(openFileDialog4.FileName);
            NewRow["Extension"] = fileInfo.Extension;
            NewRow["Path"] = openFileDialog4.FileName;
            TechStoreAttachDT.Rows.Add(NewRow);

            if (TechStoreAttachDT.Rows.Count != 0)
            {
                if (TechStoreGrid.SelectedRows.Count == 1)
                    TechStoreManager.AttachTechStoreDocument(TechStoreAttachDT, TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value.ToString(), 0);
            }

            TechStoreAttachDocumentsDT.Clear();
            CopyTechStoreAttachs(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value.ToString());
        }

        private void ZoomTechStoreImageButton_Click(object sender, EventArgs e)
        {
            if (pcbxTechStoreImage.Image == null)
                return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            ZoomImageForm ZoomImageForm = new ZoomImageForm(pcbxTechStoreImage.Image, ref TopForm);

            TopForm = ZoomImageForm;

            ZoomImageForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            ZoomImageForm.Dispose();
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
        }

        private void AddTechStoreImageButton_Click(object sender, EventArgs e)
        {
            if (TechStoreGrid.SelectedRows.Count == 0)
            {
                Infinium.LightMessageBox.Show(ref TopForm, false,
                   "Нет наименования",
                   "Добавление изображения");
                return;
            }
            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Прикрепление изображения к наименованию");
            openFileDialog4.ShowDialog();
        }

        private void RemoveTechStoreImageButton_Click(object sender, EventArgs e)
        {
            if (TechStoreAttachDocumentsDT.Select("DocType = 0").Count() == 0)
                return;

            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Открепление изображения от наименования");
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Вы уверены, что хотите удалить?",
                "Удаление");

            if (OKCancel)
            {
                DataRow[] rows = TechStoreAttachDocumentsDT.Select("DocType = 0");
                int TechStoreDocumentID = 0;
                if (rows.Count() > 0)
                {
                    TechStoreDocumentID = Convert.ToInt32(rows[0]["TechStoreDocumentID"]);

                    TechStoreManager.RemoveTechStoreDocuments(TechStoreDocumentID);
                }
                TechStoreAttachDocumentsDT.Clear();
                CopyTechStoreAttachs(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value.ToString());
                pcbxTechStoreImage.Image = null;
            }
            else
            {
                TechCatalogEvents.SaveEvents("Склад: Отмена Открепление изображения от наименования");
            }
        }

        private void btnOperationsTerms_Click(object sender, EventArgs e)
        {
            if (TechStoreManager == null ||
                TechCatalogOperationsDetailGrid.SelectedRows.Count == 0 ||
                TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value == DBNull.Value)
                return;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int TechCatalogOperationsDetailID = Convert.ToInt32(TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value);
            string MachinesOperationName = TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["MachinesOperationName"].FormattedValue.ToString();

            if (TechCatalogOperationsTermsManager == null)
            {
                TechCatalogOperationsTermsManager = new TechCatalogOperationsTerms();
                TechCatalogOperationsTermsManager.Initialize();
            }
            AddOperationsTermsForm AddOperationsTermsForm = new AddOperationsTermsForm(this, ref TechCatalogOperationsTermsManager,
                TechCatalogOperationsDetailID,
                MachinesOperationName);

            TopForm = AddOperationsTermsForm;
            AddOperationsTermsForm.ShowDialog();
            PhantomForm.Close();

            PhantomForm.Dispose();

            AddOperationsTermsForm.Dispose();
            TopForm = null;

            TechStoreManager.RefreshTechCatalogOperationsDetail();
            TechStoreManager.MoveToOperationsDetail(TechCatalogOperationsDetailID);
        }

        private void TechCatalogStoreDetailGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (TechCatalogStoreDetailGrid.Columns.Contains("TechName") && (e.ColumnIndex == this.TechCatalogStoreDetailGrid.Columns["TechName"].Index)
                && e.Value != null)
            {
                DataGridViewCell cell =
                    this.TechCatalogStoreDetailGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                int TechDecorID = -1;
                string StoreName = string.Empty;
                if (TechCatalogStoreDetailGrid.Rows[e.RowIndex].Cells["TechStoreID"].Value != DBNull.Value)
                {
                    TechDecorID = Convert.ToInt32(TechCatalogStoreDetailGrid.Rows[e.RowIndex].Cells["TechStoreID"].Value);
                    StoreName = TechStoreManager.StoreName(TechDecorID, Convert.ToBoolean(TechCatalogStoreDetailGrid.Rows[e.RowIndex].Cells["IsSubGroup"].Value));
                }
                cell.ToolTipText = StoreName;
            }
        }

        private void TechCatalogOperationsDetailGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (TechCatalogOperationsDetailGrid.Columns.Contains("MachinesOperationName") && (e.ColumnIndex == this.TechCatalogOperationsDetailGrid.Columns["MachinesOperationName"].Index)
                && e.Value != null)
            {
                DataGridViewCell cell =
                    this.TechCatalogOperationsDetailGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                int MachineID = -1;
                string SectorName = string.Empty;
                if (TechCatalogOperationsDetailGrid.Rows[e.RowIndex].Cells["MachineID"].Value != DBNull.Value)
                {
                    MachineID = Convert.ToInt32(TechCatalogOperationsDetailGrid.Rows[e.RowIndex].Cells["MachineID"].Value);
                    SectorName = TechStoreManager.SectorName(MachineID);
                }
                cell.ToolTipText = SectorName;
            }
        }

        private void btnSaveOperationsGroupName_Click(object sender, EventArgs e)
        {
            //if (GroupsOperationsDetailGrid.SelectedRows.Count == 0)
            //{
            //    return;
            //}
            //int GroupNumber = Convert.ToInt32(GroupsOperationsDetailGrid.SelectedRows[0].Cells["GroupNumber"].Value);
            //string GroupName = TechStoreManager.GetGroupName(GroupNumber);
            //for (int i = 0; i < TechCatalogOperationsDetailGrid.Rows.Count; i++)
            //{
            //    TechCatalogOperationsDetailGrid.Rows[i].Cells["GroupName"].Value = GroupName;
            //}
            //TechStoreManager.UpdateTechCatalogOperationsDetails();
            //TechCatalogGrid_SelectionChanged(null, null);
            //TechStoreManager.MoveToGroupNumber(GroupNumber);
        }

        private void btnSaveGroupOperationsGroup_Click(object sender, EventArgs e)
        {
            TechCatalogEvents.SaveEvents("Склад: нажата кнопка Сохранение группы операций");
            int OldOperationsGroupID = Convert.ToInt32(GroupsOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value);
            TechStoreManager.UpdateTechCatalogOperationsDetailGroups();
            TechStoreManager.RefreshTechCatalogOperationsDetailGroups();
            TechStoreManager.MoveToGroupNumber(OldOperationsGroupID);
            TechCatalogEvents.SaveEvents("Склад: группы операций сохранены");
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void RemoveGroupOperationsDetail_Click(object sender, EventArgs e)
        {
            if (GroupsOperationsDetailGrid.SelectedRows.Count == 0)
                return;

            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                "Удалить группу операций?",
                "Удаление");

            if (!OKCancel) return;
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            TechStoreManager.RemoveTechCatalogOperationsGroup(Convert.ToInt32(GroupsOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value));

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;

            TechCatalogGrid_SelectionChanged(null, null);
        }

        private void SaveMachinesOperationButton_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Сохранение данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            TechStoreManager.UpdateMachinesOperations();
            TechStoreManager.RefreshMachinesOperations();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void cmbxTechStoreGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterTechStoreSubGroups();
            cmbxTechStoreSubGroups_SelectedIndexChanged(null, null);
        }

        private void cmbxTechStoreSubGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterTechStore();
            CheckTechStoreColumns();
        }

        private void TechCatalogStoreDetailGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                kryptonContextMenu1.Show(new System.Drawing.Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void kryptonContextMenuItem6_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int TechStoreID = 0;

            if (TechCatalogStoreDetailGrid.SelectedRows[0].Cells["TechStoreID"].Value != DBNull.Value)
                TechStoreID = Convert.ToInt32(TechCatalogStoreDetailGrid.SelectedRows[0].Cells["TechStoreID"].Value);

            TechStoreManager.SearchStoreDetail(TechStoreID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnAttachDocAssignment_Click(object sender, EventArgs e)
        {
            if (MachinesOperationsGrid.SelectedRows.Count == 0)
                return;

            int MachinesOperationID = Convert.ToInt32(MachinesOperationsGrid.SelectedRows[0].Cells["MachinesOperationID"].Value);
            int CabFurDocTypeID = Convert.ToInt32(MachinesOperationsGrid.SelectedRows[0].Cells["CabFurDocTypeID"].Value);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            CabFurDocTypesForm CabFurDocTypesForm = new CabFurDocTypesForm(CabFurDocTypeID, MachinesOperationID, ref TechStoreManager);

            TopForm = CabFurDocTypesForm;

            CabFurDocTypesForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            CabFurDocTypesForm.Dispose();
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            int TechCatalogOperationsDetailID = 0;
            if (TechCatalogOperationsDetailGrid.SelectedRows.Count > 0 && TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value != DBNull.Value)
                TechCatalogOperationsDetailID = Convert.ToInt32(TechCatalogOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsDetailID"].Value);
            if (TechCatalogOperationsDetailID > 0)
                TechStoreManager.CopyTechStoreDetail(TechCatalogStoreDetailIDs, TechCatalogOperationsDetailID);
            TechStoreManager.RefreshTechCatalogStoreDetails();
            TechCatalogOperationsDetailGrid_SelectionChanged(null, null);
            kryptonButton2.Visible = false;
            kryptonButton3.Visible = false;
        }

        private void kryptonButton3_Click(object sender, EventArgs e)
        {
            kryptonButton2.Visible = false;
            kryptonButton3.Visible = false;
        }

        private void kryptonButton1_Click_1(object sender, EventArgs e)
        {
            if (TechCatalogStoreDetailIDs == null)
                TechCatalogStoreDetailIDs = new List<int>();
            TechCatalogStoreDetailIDs.Clear();
            for (int i = 0; i < TechCatalogStoreDetailGrid.SelectedRows.Count; i++)
            {
                TechCatalogStoreDetailIDs.Add(Convert.ToInt32(TechCatalogStoreDetailGrid.SelectedRows[i].Cells["TechCatalogStoreDetailID"].Value));
            }
            kryptonButton2.Visible = true;
            kryptonButton3.Visible = true;
        }

        private void btnStoreDetailTerms_Click(object sender, EventArgs e)
        {
            if (TechStoreManager == null ||
                   TechCatalogStoreDetailGrid.SelectedRows.Count == 0 ||
                   TechCatalogStoreDetailGrid.SelectedRows[0].Cells["TechCatalogStoreDetailID"].Value == DBNull.Value)
                return;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int TechCatalogStoreDetailID = Convert.ToInt32(TechCatalogStoreDetailGrid.SelectedRows[0].Cells["TechCatalogStoreDetailID"].Value);
            string TechName = TechCatalogStoreDetailGrid.SelectedRows[0].Cells["TechName"].FormattedValue.ToString();

            if (TechCatalogStoreDetailTermsManager == null)
            {
                TechCatalogStoreDetailTermsManager = new TechCatalogStoreDetailTerms();
                TechCatalogStoreDetailTermsManager.Initialize();
            }
            AddTechStoreTermsForm AddTechStoreTermsForm = new AddTechStoreTermsForm(this, ref TechCatalogStoreDetailTermsManager,
                TechCatalogStoreDetailID,
                TechName);

            TopForm = AddTechStoreTermsForm;
            AddTechStoreTermsForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();

            AddTechStoreTermsForm.Dispose();
            TopForm = null;

            TechStoreManager.RefreshTechCatalogStoreDetails();
            TechStoreManager.MoveToStoreDetail(TechCatalogStoreDetailID);
        }

        private void kryptonContextMenuItem1_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int TechStoreID = 0;
            if (TechCatalogStoreDetailGrid.SelectedRows[0].Cells["TechStoreID"].Value != DBNull.Value)
                TechStoreID = Convert.ToInt32(TechCatalogStoreDetailGrid.SelectedRows[0].Cells["TechStoreID"].Value);
            if (tTechnologyMaps == null)
            {
                tTechnologyMaps = new TechnologyMaps();
                tTechnologyMaps.Initialize();
            }

            tTechnologyMaps.MainFunction(TechStoreID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void kryptonButton4_Click(object sender, EventArgs e)
        {
            kryptonButton4.Visible = false;
            kryptonButton5.Visible = false;
        }

        private void kryptonButton5_Click(object sender, EventArgs e)
        {
            if (bCopyGroupOperationsDetail)
            {
                if (TechCatalogGroupOperationsIDs.Count == 0 || TechStoreGrid.SelectedRows.Count == 0)
                    return;

                Thread T =
                    new Thread(
                        delegate ()
                        {
                            SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите...");
                        });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                int TechStoreID = 0;
                int NewGroupNumber = 1;
                if (GroupsOperationsDetailGrid.SelectedRows.Count > 0)
                    NewGroupNumber = GroupsOperationsDetailGrid.Rows.Count + 1;

                if (TechStoreGrid.SelectedRows.Count > 0 &&
                    TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value != DBNull.Value)
                    TechStoreID = Convert.ToInt32(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value);
                if (TechStoreID > 0)
                {
                    TechStoreManager.CopyGroupOperationsDetail(TechStoreID, TechCatalogGroupOperationsIDs[0],
                        NewGroupNumber);
                    InfiniumTips.ShowTip(this, 50, 85, "Группа операций скопирована", 1700);
                }

                TechStoreManager.RefreshTechCatalogOperationsDetailGroups();
                TechStoreManager.RefreshTechCatalogOperationsDetail();
                TechStoreManager.RefreshTechCatalogStoreDetails();
                TechStoreManager.RefreshTechCatalogTools();
                TechCatalogGrid_SelectionChanged(null, null);
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                kryptonButton4.Visible = false;
                kryptonButton5.Visible = false;
                bCopyGroupOperationsDetail = false;
            }
            if (bCopyOperationsDetail)
            {
                if (TechCatalogOperationsIDs.Count == 0 || GroupsOperationsDetailGrid.SelectedRows.Count == 0)
                    return;

                Thread T =
                    new Thread(
                        delegate ()
                        {
                            SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите...");
                        });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                int TechCatalogOperationsGroupID = 0;

                if (GroupsOperationsDetailGrid.SelectedRows.Count > 0 &&
                    GroupsOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value != DBNull.Value)
                    TechCatalogOperationsGroupID = Convert.ToInt32(GroupsOperationsDetailGrid.SelectedRows[0].Cells["TechCatalogOperationsGroupID"].Value);
                if (TechCatalogOperationsGroupID > 0)
                {
                    TechStoreManager.CopyOperationsDetail(TechCatalogOperationsIDs, TechCatalogOperationsGroupID);
                    InfiniumTips.ShowTip(this, 50, 85, "Операции скопированы", 1700);
                }

                TechStoreManager.RefreshTechCatalogOperationsDetail();
                TechStoreManager.RefreshTechCatalogStoreDetails();
                TechStoreManager.RefreshTechCatalogTools();
                TechCatalogGrid_SelectionChanged(null, null);
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;

                kryptonButton4.Visible = false;
                kryptonButton5.Visible = false;
                bCopyOperationsDetail = false;
            }
        }

        private void CopyOperationsDetail_Click(object sender, EventArgs e)
        {
            if (TechCatalogOperationsDetailGrid.SelectedRows.Count == 0)
                return;
            bCopyOperationsDetail = true;
            if (TechCatalogOperationsIDs == null)
                TechCatalogOperationsIDs = new List<int>();
            TechCatalogOperationsIDs.Clear();

            kryptonButton4.Visible = true;
            kryptonButton5.Visible = true;

            TechCatalogOperationsIDs.Clear();
            for (int i = 0; i < TechCatalogOperationsDetailGrid.SelectedRows.Count; i++)
            {
                TechCatalogOperationsIDs.Add(Convert.ToInt32(TechCatalogOperationsDetailGrid.SelectedRows[i].Cells["TechCatalogOperationsDetailID"].Value));
            }

            TechCatalogEvents.SaveEvents("Склад: Нажата кнопка Копирование операций");
        }

        private void kryptonContextMenuItem3_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            int TechStoreID = 0;
            if (TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value != DBNull.Value)
                TechStoreID = Convert.ToInt32(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value);
            if (tTechnologyMaps == null)
            {
                tTechnologyMaps = new TechnologyMaps();
                tTechnologyMaps.Initialize();
            }

            tTechnologyMaps.MainFunction(TechStoreID);

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void TechStoreGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Right)
            //{
            //    kryptonContextMenu2.Show(new System.Drawing.Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            //}
        }

        private void btnSaveStoreOperationDetails_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            TechStoreManager.UpdateTechCatalogOperationsDetails();
            TechStoreManager.RefreshTechCatalogOperationsDetail();

            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
            InfiniumTips.ShowTip(this, 50, 85, "Сохранено", 1700);
        }

        private void MachinesOperationsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (MachinesOperationsGrid.SelectedRows.Count == 0)
                return;

            int MachineID = Convert.ToInt32(MachinesGrid.SelectedRows[0].Cells["MachineID"].Value);
            int MachinesOperationID = Convert.ToInt32(MachinesOperationsGrid.SelectedRows[0].Cells["MachinesOperationID"].Value);
            TechCatalogEvents.SaveEvents("Операции: Нажата кнопка Редактирование операции MachinesOperationID=" + MachinesOperationID + " на станке MachineID=" + MachineID);

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            NewMachinesOperationForm NewMachinesOperationForm = new NewMachinesOperationForm(MachineID, MachinesOperationID, ref TechStoreManager);

            TopForm = NewMachinesOperationForm;

            NewMachinesOperationForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            NewMachinesOperationForm.Dispose();
        }

        private void kryptonContextMenuItem2_Click(object sender, EventArgs e)
        {
            bool PressOK = false;

            bool NeedLength = false;
            bool NeedHeight = false;
            bool NeedWidth = false;

            int TechStoreID = Convert.ToInt32(TechStoreGrid.SelectedRows[0].Cells["TechStoreID"].Value);

            TechLabelInfo tlInfo = new TechLabelInfo();
            TechStoreGroupInfo techStoreGroupInfo = TechStoreManager.GetSubGroupInfo(TechStoreID);
            if (techStoreGroupInfo.Length == 0)
                NeedLength = true;
            if (techStoreGroupInfo.Height == 0)
                NeedHeight = true;
            if (techStoreGroupInfo.Width == 0)
                NeedWidth = true;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();
            TechnologyLabelInfoMenu CabFurLabelInfoMenu = new TechnologyLabelInfoMenu(this, NeedLength, NeedHeight, NeedWidth);
            TopForm = CabFurLabelInfoMenu;
            CabFurLabelInfoMenu.ShowDialog();
            PressOK = CabFurLabelInfoMenu.PressOK;
            tlInfo = CabFurLabelInfoMenu.tlInfo;
            PhantomForm.Close();
            PhantomForm.Dispose();
            CabFurLabelInfoMenu.Dispose();
            TopForm = null;

            if (!PressOK)
                return;


            int DecorConfigID = 0;
            CabFurDT.Clear();
            DataRow NewRow = CabFurDT.NewRow();

            NewRow["TechStoreName"] = techStoreGroupInfo.TechStoreName;
            NewRow["TechStoreSubGroupName"] = techStoreGroupInfo.TechStoreSubGroupName;
            NewRow["SubGroupNotes"] = techStoreGroupInfo.SubGroupNotes;
            NewRow["SubGroupNotes1"] = techStoreGroupInfo.SubGroupNotes1;
            NewRow["SubGroupNotes2"] = techStoreGroupInfo.SubGroupNotes2;
            NewRow["Color"] = tlInfo.Color;
            if (techStoreGroupInfo.Length == 0)
                NewRow["Length"] = tlInfo.Length;
            else
                NewRow["Length"] = techStoreGroupInfo.Length;
            if (techStoreGroupInfo.Height == 0)
                NewRow["Height"] = tlInfo.Height;
            else
                NewRow["Height"] = techStoreGroupInfo.Height;
            if (techStoreGroupInfo.Width == 0)
                NewRow["Width"] = tlInfo.Width;
            else
                NewRow["Width"] = techStoreGroupInfo.Width;
            NewRow["LabelsCount"] = tlInfo.LabelsCount;
            NewRow["PositionsCount"] = tlInfo.PositionsCount;
            NewRow["DecorConfigID"] = DecorConfigID;

            CabFurDT.Rows.Add(NewRow);

            CabFurLabelManager.ClearLabelInfo();

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
                int LabelsCount = Convert.ToInt32(CabFurDT.Rows[i]["LabelsCount"]);
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
                    SampleLabelID = CabFurLabelManager.SaveSampleLabel(DecorConfigID, DateTime.Now, Security.CurrentUserID, 2);
                    LabelInfo.BarcodeNumber = CabFurLabelManager.GetBarcodeNumber(19, SampleLabelID);

                    LabelInfo.FactoryType = tlInfo.Factory;
                    LabelInfo.ProductType = 2;
                    LabelInfo.DocDateTime = tlInfo.DocDateTime;
                    DT.Rows.Add(destRow);
                    LabelInfo.OrderData = DT;

                    CabFurLabelManager.AddLabelInfo(ref LabelInfo);
                }
            }
            PrintDialog.Document = CabFurLabelManager.PD;

            if (PrintDialog.ShowDialog() == DialogResult.OK)
            {
                CabFurLabelManager.Print();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            kryptonContextMenuItem3_Click(null, null);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            kryptonContextMenuItem2_Click(null, null);
        }
    }
}
