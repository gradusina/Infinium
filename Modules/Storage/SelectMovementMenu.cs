using Infinium.Store;

using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class SelectMovementMenu : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        int FormEvent = 0;

        MainStoreManager StorageManager;
        NewMovementParameters Parameters;
        Form MainForm = null;

        public SelectMovementMenu(Form tMainForm, MainStoreManager tStorageManager, ref NewMovementParameters tParameters)
        {
            StorageManager = tStorageManager;
            Parameters = tParameters;
            MainForm = tMainForm;
            InitializeComponent();
        }

        private void OKReportButton_Click(object sender, EventArgs e)
        {
            if (StoreAllocFromDataGrid.SelectedRows.Count != 0 && StoreAllocFromDataGrid.SelectedRows[0].Cells["StoreAllocID"].Value != DBNull.Value)
                Parameters.SellerStoreAllocID = Convert.ToInt32(StoreAllocFromDataGrid.SelectedRows[0].Cells["StoreAllocID"].Value);
            if (StoreAllocToDataGrid.SelectedRows.Count != 0 && StoreAllocToDataGrid.SelectedRows[0].Cells["StoreAllocID"].Value != DBNull.Value)
                Parameters.RecipientStoreAllocID = Convert.ToInt32(StoreAllocToDataGrid.SelectedRows[0].Cells["StoreAllocID"].Value);

            if (Parameters.SellerStoreAllocID == 1 || Parameters.SellerStoreAllocID == 3 || Parameters.SellerStoreAllocID == 10)
            {
                Parameters.FactoryID = 1;
            }
            if (Parameters.SellerStoreAllocID == 2 || Parameters.SellerStoreAllocID == 4 || Parameters.SellerStoreAllocID == 11)
            {
                Parameters.FactoryID = 2;
            }
            if (Parameters.RecipientStoreAllocID == 3 || Parameters.RecipientStoreAllocID == 4)
            {
                if (SectorsComboBox.SelectedItem != null)
                    Parameters.RecipientSectorID = Convert.ToInt32(((DataRowView)SectorsComboBox.SelectedItem).Row["SectorID"]);
            }
            else
                Parameters.RecipientSectorID = 0;

            if (UnknownPersonButton.Checked)
            {
                Parameters.PersonID = 0;
                if (PersonTextBox.Text.Length > 0)
                {
                    Parameters.PersonName = PersonTextBox.Text;
                }
            }
            else
            {
                if (PersonsComboBox.SelectedItem != null)
                {
                    Parameters.PersonID = Convert.ToInt32(((DataRowView)PersonsComboBox.SelectedItem).Row["UserID"]);
                    Parameters.PersonName = ((DataRowView)PersonsComboBox.SelectedItem).Row["ShortName"].ToString();
                }
            }
            Parameters.StoreKeeperID = Security.CurrentUserID;
            Parameters.DateTime = MovementDateTimePicker.Value;
            if (ClientsDataGrid.Enabled)
            {
                string From = string.Empty;
                if (ClientsDataGrid.SelectedRows.Count != 0 && ClientsDataGrid.SelectedRows[0].Cells["From"].Value != DBNull.Value)
                    From = ClientsDataGrid.SelectedRows[0].Cells["From"].Value.ToString();
                if (ClientsDataGrid.SelectedRows.Count != 0 && ClientsDataGrid.SelectedRows[0].Cells["ClientName"].Value != DBNull.Value)
                    Parameters.ClientName = ClientsDataGrid.SelectedRows[0].Cells["ClientName"].Value.ToString();
                if (From == "Storage")
                {
                    if (ClientsDataGrid.SelectedRows.Count != 0 && ClientsDataGrid.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                        Parameters.SellerID = Convert.ToInt32(ClientsDataGrid.SelectedRows[0].Cells["ClientID"].Value);
                }
                if (From == "Marketing")
                {
                    if (ClientsDataGrid.SelectedRows.Count != 0 && ClientsDataGrid.SelectedRows[0].Cells["ClientID"].Value != DBNull.Value)
                        Parameters.ClientID = Convert.ToInt32(ClientsDataGrid.SelectedRows[0].Cells["ClientID"].Value);
                }
            }
            Parameters.Notes = NotesRichTextBox.Text;

            Parameters.OKPress = true;
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void CancelReportButton_Click(object sender, EventArgs e)
        {
            Parameters.OKPress = false;
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

        private void Initialize()
        {
            ClientsDataGrid.DataSource = StorageManager.ClientsList;
            ClientsDataGrid.Columns["From"].Visible = false;
            ClientsDataGrid.Columns["ClientID"].Visible = false;
            ClientsDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsDataGrid.Columns["ClientName"].MinimumWidth = 150;

            StoreAllocFromDataGrid.DataSource = StorageManager.StoreAllocFromList;
            StoreAllocFromDataGrid.Columns["StoreAllocID"].Visible = false;
            StoreAllocFromDataGrid.Columns["FactoryID"].Visible = false;
            StoreAllocFromDataGrid.Columns["StoreAlloc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            StoreAllocFromDataGrid.Columns["StoreAlloc"].MinimumWidth = 150;

            StoreAllocToDataGrid.DataSource = StorageManager.StoreAllocToList;
            StoreAllocToDataGrid.Columns["StoreAllocID"].Visible = false;
            StoreAllocToDataGrid.Columns["FactoryID"].Visible = false;
            StoreAllocToDataGrid.Columns["StoreAlloc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            StoreAllocToDataGrid.Columns["StoreAlloc"].MinimumWidth = 150;

            SectorsComboBox.DataSource = StorageManager.SectorsList;
            SectorsComboBox.DisplayMember = "Sector";
            SectorsComboBox.ValueMember = "SectorID";

            PersonsComboBox.DataSource = StorageManager.UsersList;
            PersonsComboBox.DisplayMember = "ShortName";
            PersonsComboBox.ValueMember = "UserID";

        }

        private void GroupsGridSettings()
        {

        }

        private void MovementReportMenu_Load(object sender, EventArgs e)
        {
            Initialize();
        }

        private void KnownPersonButton_CheckedChanged(object sender, EventArgs e)
        {
            UnknownPersonButton.Checked = !KnownPersonButton.Checked;
            PersonsComboBox.Enabled = KnownPersonButton.Checked;
            PersonTextBox.Enabled = !KnownPersonButton.Checked;

            if (KnownPersonButton.Checked)
                PersonsComboBox.Focus();
        }

        private void UnknownPersonButton_CheckedChanged(object sender, EventArgs e)
        {
            KnownPersonButton.Checked = !UnknownPersonButton.Checked;
            PersonsComboBox.Enabled = !UnknownPersonButton.Checked;
            PersonTextBox.Enabled = UnknownPersonButton.Checked;

            if (UnknownPersonButton.Checked)
                PersonTextBox.Focus();
        }

        private void StoreAllocToDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (StoreAllocToDataGrid.SelectedRows.Count != 0 && StoreAllocToDataGrid.SelectedRows[0].Cells["StoreAllocID"].Value != DBNull.Value)
                Parameters.RecipientStoreAllocID = Convert.ToInt32(StoreAllocToDataGrid.SelectedRows[0].Cells["StoreAllocID"].Value);
            if (Parameters.RecipientStoreAllocID != 3 && Parameters.RecipientStoreAllocID != 4)
            {
                SectorsComboBox.Enabled = false;
                Parameters.RecipientSectorID = 0;
            }
            else
            {
                int FactoryID = 0;
                //if (SectorsComboBox.SelectedItem != null)
                //    FactoryID = Convert.ToInt32(((DataRowView)SectorsComboBox.SelectedItem).Row["FactoryID"]);

                if (StoreAllocToDataGrid.SelectedRows.Count != 0 && StoreAllocToDataGrid.SelectedRows[0].Cells["FactoryID"].Value != DBNull.Value)
                    FactoryID = Convert.ToInt32(StoreAllocToDataGrid.SelectedRows[0].Cells["FactoryID"].Value);
                if (SectorsComboBox.SelectedItem != null)
                    Parameters.RecipientSectorID = Convert.ToInt32(((DataRowView)SectorsComboBox.SelectedItem).Row["SectorID"]);
                StorageManager.FilterSectors(FactoryID);
                //Parameters.RecipientSectorID = GetRecipientSectorID();
                SectorsComboBox.Enabled = true;
            }

            if (Parameters.RecipientStoreAllocID == 12 || Parameters.RecipientStoreAllocID == 13)
            {
                ClientsDataGrid.Enabled = true;
            }
            else
            {
                ClientsDataGrid.Enabled = false;
            }
        }
    }
}
