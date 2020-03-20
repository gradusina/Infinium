using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Infinium
{
    public partial class ZOVClientsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;
        const int iPriceGroup = 98;
        bool bPriceGroup = false;

        int FormEvent = 0;
        int CurrentRowIndex = 0;
        int CurrentColumnIndex = 0;

        DataTable RolePermissionsDataTable;

        LightStartForm LightStartForm;

        Form TopForm = null;
        Infinium.Modules.ZOV.Clients.Clients Clients;

        public ZOVClientsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            RolePermissionsDataTable = RolesAndPermissionsManager.GetPermissions(Security.CurrentUserID, this.Name);

            Initialize();

            while (!SplashForm.bCreated) ;
        }

        private bool PermissionGranted(int PermissionID)
        {
            DataRow[] Rows = RolePermissionsDataTable.Select("PermissionID = " + PermissionID);

            if (Rows.Count() > 0)
            {
                return Convert.ToBoolean(Rows[0]["Granted"]);
            }

            return false;
        }

        private void ZOVClientsForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;

            if (!PermissionGranted(Convert.ToInt32(ClientsNewClientButton.Tag)))
            {
                ClientsClientAreaPanel.Height = this.Height - NavigatePanel.Height - 10;
                ToolsPanel.Visible = false;
            }
            if (PermissionGranted(iPriceGroup))
            {
                bPriceGroup = true;
            }

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

        private void NavigateMenuHerculesButton_Click(object sender, EventArgs e)
        {
            FormEvent = eMainMenu;
            AnimateTimer.Enabled = true;
        }


        private void Initialize()
        {
            Clients = new Modules.ZOV.Clients.Clients(ref ClientsDataGrid, ref ClientsContactsDataGrid, ref ShopAddressesDataGrid);
        }

        private void ClientsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (Clients == null || Clients.ClientsBindingSource.Count == 0)
                return;

            int ClientID = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientID"]);

            Clients.FillContactsDataTable(ClientID);
            Clients.FillShopAddressesDataTable(ClientID);
        }

        private void ClientsDataGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CurrentColumnIndex = e.ColumnIndex;
                CurrentRowIndex = e.RowIndex;
                kryptonContextMenu1.Show(new Point(Cursor.Position.X - 212, Cursor.Position.Y - 10));
            }
        }

        private void CopyContextMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ClientsDataGrid.Rows[CurrentRowIndex].Cells[CurrentColumnIndex].Value.ToString());
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

        private void ClientsNewClientButton_Click(object sender, EventArgs e)
        {
            Clients.NewClient = true;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int ClientID = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientID"]);
            NewZOVClientForm NewZOVClientForm = new NewZOVClientForm(this, Clients, ClientID, bPriceGroup);

            TopForm = NewZOVClientForm;
            NewZOVClientForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            NewZOVClientForm.Dispose();
            TopForm = null;
        }

        private void ClientsEditButton_Click(object sender, EventArgs e)
        {
            Clients.NewClient = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int ClientID = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientID"]);
            NewZOVClientForm NewZOVClientForm = new NewZOVClientForm(this, Clients, ClientID, bPriceGroup);

            NewZOVClientForm.EditClient();
            TopForm = NewZOVClientForm;
            NewZOVClientForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            NewZOVClientForm.Dispose();
            TopForm = null;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            FormEvent = eHide;
            AnimateTimer.Enabled = true;
        }

        private void ClientsDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Clients.NewClient = false;

            PhantomForm PhantomForm = new Infinium.PhantomForm();
            PhantomForm.Show();

            int ClientID = Convert.ToInt32(((DataRowView)Clients.ClientsBindingSource.Current).Row["ClientID"]);
            NewZOVClientForm NewZOVClientForm = new NewZOVClientForm(this, Clients, ClientID, bPriceGroup);

            NewZOVClientForm.EditClient();
            TopForm = NewZOVClientForm;
            NewZOVClientForm.ShowDialog();

            PhantomForm.Close();

            PhantomForm.Dispose();
            NewZOVClientForm.Dispose();
            TopForm = null;
        }

        private bool saveFile(ref string FileName)
        {
            saveFileDialog1.Title = "Сохранить как";
            saveFileDialog1.FileName = "*.xls";
            saveFileDialog1.Filter = "Excel 1998-2003 Document( *.xls )|*.xls";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileName = saveFileDialog1.FileName; return true;
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.Message);
                    return false;
                }
            }
            return false;
        }

        private void ShopAddressesDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string Address = string.Empty;
            if (ShopAddressesDataGrid.SelectedRows.Count != 0 && ShopAddressesDataGrid.SelectedRows[0].Cells["Address"].Value != DBNull.Value)
                Address = ShopAddressesDataGrid.SelectedRows[0].Cells["Address"].Value.ToString();
            if (Address.Length == 0)
                return;
            try
            {
                StringBuilder QuerryAddress = new StringBuilder();
                QuerryAddress.Append("http://www.google.com/maps?q=");
                QuerryAddress.Append(Address);
                System.Diagnostics.Process.Start(QuerryAddress.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }

        private void ShopAddressesDataGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            PercentageDataGrid dataGridView = (PercentageDataGrid)sender;
            if (e.ColumnIndex > -1 && e.RowIndex > -1 && dataGridView.Columns[e.ColumnIndex].Name == "Address" && dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Length > 0)
                dataGridView.Cursor = Cursors.Hand;
            else
                dataGridView.Cursor = Cursors.Default;
        }
    }
}
