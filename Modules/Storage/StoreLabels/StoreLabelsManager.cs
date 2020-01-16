using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Infinium.Modules.Storage.StoreLabels
{
    public class StoreLabelsManager
    {
        DataTable dtColors;
        DataTable dtCovers;
        DataTable dtPatina;

        DataTable dtStoreLabels;
        DataTable dtStore;
        BindingSource bsStoreLabels;
        BindingSource bsStore;

        public DataGridViewComboBoxColumn ColorsColumn;
        public DataGridViewComboBoxColumn CoversColumn;
        public DataGridViewComboBoxColumn PatinaColumn;

        public StoreLabelsManager()
        {

        }

        public void Initialize()
        {
            Create();
            CreateCovers();
            Fill();
            Binding();
            CreateGridColumns();
        }

        private void Create()
        {
            dtColors = new DataTable();
            dtPatina = new DataTable();
            dtStoreLabels = new DataTable();
            dtStore = new DataTable();
            bsStoreLabels = new BindingSource();
            bsStore = new BindingSource();
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Colors",
                ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(dtColors);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Patina",
                ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(dtPatina);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM StoreLabels",
                ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(dtStoreLabels);
            }
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 Store.StoreID, Store.Diameter, Store.Capacity," +
                " Store.Thickness, Store.Length, Store.Height, Store.Width, Store.Admission, Store.Weight, StoreItems.ItemName AS StoreItemColumn FROM Store" +
                " INNER JOIN StoreItems ON Store.StoreItemID = StoreItems.StoreItemID" +
                " ORDER BY ItemName",
                ConnectionStrings.StorageConnectionString))
            {
                DA.Fill(dtStore);
            }
        }

        private void Binding()
        {
            bsStoreLabels.DataSource = dtStoreLabels;
            bsStore.DataSource = dtStore;
        }

        private void CreateCovers()
        {
            dtCovers = new DataTable();
            dtCovers.Columns.Add(new DataColumn("CoverID", Type.GetType("System.Int64")));
            dtCovers.Columns.Add(new DataColumn("CoverName", Type.GetType("System.String")));

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT  StoreItemID, ItemName FROM StoreItems" +
                " WHERE StoreSubGroupID IN (SELECT StoreSubGroupID FROM StoreSubGroups WHERE StoreGroupID = 11)" +
                " ORDER BY ItemName",
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    DataRow EmptyRow = dtCovers.NewRow();
                    EmptyRow["CoverID"] = -1;
                    EmptyRow["CoverName"] = "-";
                    dtCovers.Rows.Add(EmptyRow);

                    DataRow ChoiceRow = dtCovers.NewRow();
                    ChoiceRow["CoverID"] = 0;
                    ChoiceRow["CoverName"] = "на выбор";
                    dtCovers.Rows.Add(ChoiceRow);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DataRow NewRow = dtCovers.NewRow();
                        NewRow["CoverID"] = Convert.ToInt64(DT.Rows[i]["StoreItemID"]);
                        NewRow["CoverName"] = DT.Rows[i]["ItemName"].ToString();
                        dtCovers.Rows.Add(NewRow);
                    }
                }
            }
        }

        private void CreateGridColumns()
        {
            ColorsColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ColorsColumn",
                HeaderText = "Цвет",
                DataPropertyName = "ColorID",
                DataSource = dtColors,
                ValueMember = "ColorID",
                DisplayMember = "ColorName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            CoversColumn = new DataGridViewComboBoxColumn()
            {
                Name = "CoversColumn",
                HeaderText = "Облицовка",
                DataPropertyName = "CoverID",
                DataSource = new DataView(dtCovers),
                ValueMember = "CoverID",
                DisplayMember = "CoverName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            PatinaColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PatinaColumn",
                HeaderText = "Патина",
                DataPropertyName = "PatinaID",
                DataSource = dtPatina,
                ValueMember = "PatinaID",
                DisplayMember = "PatinaName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
        }

        public void AddStoreLabel()
        {
            DataRow NewRow = dtStoreLabels.NewRow();
            dtStoreLabels.Rows.Add(NewRow);
        }

        public void CreateGroupLabels(int Count)
        {
            for (int i = 0; i < Count; i++)
            {
                AddStoreLabel();
            }
        }

        public void ClearStoreTable()
        {
            dtStore.Clear();
        }

        public void FilterStore(int StoreID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT StoreItems.ItemName AS StoreItemColumn, Store.StoreID, Store.Diameter, Store.Capacity," +
                " Store.Thickness, Store.Length, Store.Height, Store.Width, Store.Admission, Store.Weight FROM Store" +
                " INNER JOIN StoreItems ON Store.StoreItemID = StoreItems.StoreItemID" +
                " WHERE StoreID = " + StoreID + " ORDER BY ItemName",
                ConnectionStrings.StorageConnectionString))
            {
                dtStore.Clear();
                DA.Fill(dtStore);
            }
        }

        public void SaveStoreLabels()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM StoreLabels",
                ConnectionStrings.StorageConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(dtStoreLabels);
                }
            }
        }

        public void UpdateStoreLabels()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM StoreLabels",
                ConnectionStrings.StorageConnectionString))
            {
                dtStoreLabels.Clear();
                DA.Fill(dtStoreLabels);
            }
        }

        public int[] GetStoreLabels(int Count)
        {
            int[] rows = new int[Count];
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP " + Count + " StoreLabelID FROM StoreLabels ORDER BY StoreLabelID DESC",
                ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    for (int i = 0; i < DT.Rows.Count; i++)
                        rows[i] = Convert.ToInt32(DT.Rows[i]["StoreLabelID"]);
                }
            }
            Array.Sort(rows);
            return rows;
        }

        public bool HasStoreLabels
        {
            get
            {
                return bsStoreLabels.Count > 0;
            }
        }

        public bool HasStore
        {
            get
            {
                return bsStore.Count > 0;
            }
        }

        public BindingSource StoreLabelsList
        {
            get { return bsStoreLabels; }
        }

        public BindingSource StoreList
        {
            get { return bsStore; }
        }

        public int CurrentStoreLabelCount
        {
            get
            {
                if (bsStoreLabels.Count == 0 || ((DataRowView)bsStoreLabels.Current).Row["StoreLabelID"] == DBNull.Value)
                    return -1;
                else
                    return Convert.ToInt32(((DataRowView)bsStoreLabels.Current).Row["StoreLabelID"]);
            }
        }

        public int CurrentStore
        {
            get
            {
                if (bsStoreLabels.Count == 0 || ((DataRowView)bsStoreLabels.Current).Row["StoreID"] == DBNull.Value)
                    return -1;
                else
                    return Convert.ToInt32(((DataRowView)bsStoreLabels.Current).Row["StoreID"]);
            }
        }
    }
}
