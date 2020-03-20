using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Infinium.Modules.Marketing.Payments
{
    public class ClientPayments
    {
        public FileManager FM = new FileManager();
        Excel Ex = null;

        PercentageDataGrid ClientsPaymentsDataGrid = null;
        PercentageDataGrid ClientContractDataGrid = null;

        public DataTable ClientPaymentsTable;
        public DataTable ClientSummTable;
        public BindingSource ClientPaymentsBindingSource;

        public DataTable ClientContractTable;
        public DataTable ClientContractSummTable;
        public BindingSource ClientContractBindingSource;

        public DataTable AttachmentsDataTable;

        public DataTable FilterContract;
        public DataTable FinancialDT;
        public DataTable FinancialPaymentDT;

        public decimal CostEUR = 0;
        public decimal CostEURShipping = 0;
        public decimal CostEURPayment = 0;

        public decimal CostUSD = 0;
        public decimal CostUSDShipping = 0;
        public decimal CostUSDPayment = 0;

        public decimal CostRUR = 0;
        public decimal CostRURShipping = 0;
        public decimal CostRURPayment = 0;

        public decimal CostBYR = 0;
        public decimal CostBYRShipping = 0;
        public decimal CostBYRPayment = 0;

        public ClientPayments(ref PercentageDataGrid tClientsPaymentsDataGrid, ref PercentageDataGrid tClientContractDataGrid)
        {
            ClientsPaymentsDataGrid = tClientsPaymentsDataGrid;
            ClientContractDataGrid = tClientContractDataGrid;

            Initialize();
        }

        public ClientPayments()
        {
        }

        public void Initialize()
        {
            CreateAndFill();
            Binding();
            GridSettings();
        }

        public DataTable GetAttachments(string ContractID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientContractDocumentsID, FileSize, FileName, ContractID FROM ClientContractDocuments WHERE ContractID = " + ContractID, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return DT;
                }
            }
        }

        public DataView Filter(string ClientID)
        {
            DataView DataView = new DataView(ClientContractTable)
            {
                RowFilter = "ClientID = " + ClientID
            };
            return DataView;
        }

        public string SaveFile(string FileNameDoc)//temp folder
        {
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");

            string FileName = "";
            int TechCatalogImageID;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FileName, FileSize, ClientContractDocumentsID FROM ClientContractDocuments WHERE FileName = '" + FileNameDoc + "'", ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    FileName = DT.Rows[0]["FileName"].ToString();

                    TechCatalogImageID = Convert.ToInt32(DT.Rows[0]["ClientContractDocumentsID"]);
                    try
                    {
                        FM.DownloadFile(Configs.DocumentsPath + FileManager.GetPath("ClientContracts") + "/" + TechCatalogImageID + ".idf",
                                            tempFolder + "\\" + FileName, Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            return tempFolder + "\\" + FileName;
        }


        public void RemoveAttachmentsDocuments(string ContractId)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContractDocuments WHERE ContractId = " + ContractId, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    foreach (DataRow Row in DT.Rows)
                    {
                        FM.DeleteFile(Configs.DocumentsPath + FileManager.GetPath("ClientContracts") + "/" +
                                      DT.Select("FileName = '" + Row["FileName"].ToString() + "'")[0]["ClientContractDocumentsID"] + ".idf", Configs.FTPType);
                    }
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM ClientContractDocuments WHERE ContractId = " + ContractId, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }

            ReloadAttachments();
        }


        public bool AttachDocuments(DataTable AttachmentsDataTable, string ContractID)
        {
            if (AttachmentsDataTable.Rows.Count == 0)
                return true;

            bool Ok = true;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM ClientContractDocuments", ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        FileInfo fi;

                        fi = new FileInfo(AttachmentsDataTable.Rows[AttachmentsDataTable.Rows.Count - 1]["Path"].ToString());

                        DataRow NewRow = DT.NewRow();
                        NewRow["ContractID"] = ContractID;
                        NewRow["FileName"] = AttachmentsDataTable.Rows[AttachmentsDataTable.Rows.Count - 1]["FileName"];
                        NewRow["FileSize"] = fi.Length;
                        DT.Rows.Add(NewRow);

                        DA.Update(DT);
                    }
                }
            }

            //write to ftp
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContractDocuments WHERE ContractID = " + ContractID, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    FM.UploadFile(AttachmentsDataTable.Rows[AttachmentsDataTable.Rows.Count - 1]["Path"].ToString(), Configs.DocumentsPath +
                                  FileManager.GetPath("ClientContracts") + "/" +
                                  DT.Select("FileName = '" + AttachmentsDataTable.Rows[AttachmentsDataTable.Rows.Count - 1]["FileName"].ToString() + "'")[0]["ClientContractDocumentsID"] + ".idf", Configs.FTPType);


                    DA.Update(DT);
                }
            }

            ReloadAttachments();

            return Ok;
        }

        public void ReloadAttachments()
        {
            AttachmentsDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContractDocuments", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(AttachmentsDataTable);
            }
        }

        public void CreateAndFill()
        {
            ClientPaymentsTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientPaymentsTable);
            }
            ClientSummTable = ClientPaymentsTable.Copy();

            ClientContractTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientContractTable);
                ClientContractTable.Columns.Add("Balance");
            }

            AttachmentsDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientContractDocumentsID, FileSize, FileName, ContractID FROM ClientContractDocuments", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(AttachmentsDataTable);
            }

            FilterContract = new DataTable();

            FinancialDT = new DataTable();
            FinancialDT.Columns.Add("ClientName", Type.GetType("System.String"));
            FinancialDT.Columns.Add("DebtBeginningMonth", Type.GetType("System.String"));
            FinancialDT.Columns.Add("DebtEndMonth", Type.GetType("System.String"));
            FinancialDT.Columns.Add("Shipping", Type.GetType("System.Decimal"));
            FinancialDT.Columns.Add("Payment", Type.GetType("System.Decimal"));

            FinancialPaymentDT = new DataTable();
        }

        private void Binding()
        {
            ClientPaymentsBindingSource = new BindingSource()
            {
                DataSource = ClientPaymentsTable
            };
            ClientsPaymentsDataGrid.DataSource = ClientPaymentsBindingSource;

            ClientContractBindingSource = new BindingSource()
            {
                DataSource = ClientContractTable
            };
            ClientContractDataGrid.DataSource = ClientContractBindingSource;
        }

        public void GridSettings()
        {
            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };

            //------------------------------------------>ClientsPaymentsDataGrid

            ClientsPaymentsDataGrid.Columns["ClientPaymentsID"].Visible = false;
            ClientsPaymentsDataGrid.Columns["CurrencyTypeID"].Visible = false;
            ClientsPaymentsDataGrid.Columns["FactoryID"].Visible = false;
            ClientsPaymentsDataGrid.Columns["ClientID"].Visible = false;
            ClientsPaymentsDataGrid.Columns["ContractId"].Visible = false;
            ClientsPaymentsDataGrid.Columns["TypePayments"].HeaderText = "Тип";
            ClientsPaymentsDataGrid.Columns["DocNumber"].HeaderText = "Номер спецификации";
            ClientsPaymentsDataGrid.Columns["Date"].HeaderText = "Дата";
            ClientsPaymentsDataGrid.Columns["Cost"].HeaderText = "Сумма";

            foreach (DataGridViewColumn Column in ClientsPaymentsDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            DataGridViewComboBoxColumn CurrencyColumn = new DataGridViewComboBoxColumn()
            {
                Name = "CurrencyType",
                DataSource = TableCurrency(),
                DisplayMember = "CurrencyType",
                ValueMember = "CurrencyTypeID",
                DataPropertyName = "CurrencyTypeID",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ClientsPaymentsDataGrid.Columns.Add(CurrencyColumn);
            ClientsPaymentsDataGrid.Columns["CurrencyType"].HeaderText = "Валюта";
            ClientsPaymentsDataGrid.Columns["CurrencyType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsPaymentsDataGrid.Columns["CurrencyType"].MinimumWidth = 60;

            DataGridViewComboBoxColumn ClientContract = new DataGridViewComboBoxColumn()
            {
                Name = "ContractNumber",
                DataSource = ClientContractBindingSource,
                DisplayMember = "ContractNumber",
                ValueMember = "ContractId",
                DataPropertyName = "ContractId",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ClientsPaymentsDataGrid.Columns.Add(ClientContract);
            ClientsPaymentsDataGrid.Columns["ContractNumber"].HeaderText = "Номер договора";
            ClientsPaymentsDataGrid.Columns["ContractNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            DataGridViewComboBoxColumn ClientColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ClientName",
                DataSource = TableClients(),
                DisplayMember = "ClientName",
                ValueMember = "ClientID",
                DataPropertyName = "ClientID",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ClientsPaymentsDataGrid.Columns.Add(ClientColumn);
            ClientsPaymentsDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            ClientsPaymentsDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            DataGridViewComboBoxColumn PaymentsFactoryColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FactoryName",
                DataSource = TableFactory(),
                DisplayMember = "FactoryName",
                ValueMember = "FactoryID",
                DataPropertyName = "FactoryID",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ClientsPaymentsDataGrid.Columns.Add(PaymentsFactoryColumn);
            ClientsPaymentsDataGrid.Columns["FactoryName"].HeaderText = "Участок";
            ClientsPaymentsDataGrid.Columns["FactoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            ClientsPaymentsDataGrid.Columns["TypePayments"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsPaymentsDataGrid.Columns["TypePayments"].MinimumWidth = 100;
            ClientsPaymentsDataGrid.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientsPaymentsDataGrid.Columns["Date"].MinimumWidth = 110;

            ClientsPaymentsDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
            ClientsPaymentsDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;

            ClientsPaymentsDataGrid.Columns["ClientName"].DisplayIndex = 0;
            ClientsPaymentsDataGrid.Columns["ContractNumber"].DisplayIndex = 1;
            ClientsPaymentsDataGrid.Columns["TypePayments"].DisplayIndex = 2;
            ClientsPaymentsDataGrid.Columns["DocNumber"].DisplayIndex = 3;

            //------------------------------------------->ClientContractDataGrid  

            ClientContractDataGrid.Columns["ContractId"].Visible = false;
            ClientContractDataGrid.Columns["CurrencyTypeID"].Visible = false;
            ClientContractDataGrid.Columns["ClientID"].Visible = false;
            ClientContractDataGrid.Columns["FactoryID"].Visible = false;
            ClientContractDataGrid.Columns["ContractNumber"].HeaderText = "Номер договора";
            ClientContractDataGrid.Columns["Balance"].HeaderText = "Остаток";
            ClientContractDataGrid.Columns["StartDateContract"].HeaderText = "Дата заключения";
            ClientContractDataGrid.Columns["EndDateContract"].HeaderText = "Дата окончания";
            ClientContractDataGrid.Columns["Cost"].HeaderText = "Сумма";
            ClientContractDataGrid.Columns["Terminated"].HeaderText = "Расторгнут";

            foreach (DataGridViewColumn Column in ClientContractDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            DataGridViewComboBoxColumn CurrencyColumnContract = new DataGridViewComboBoxColumn()
            {
                Name = "CurrencyType",
                DataSource = TableCurrency(),
                DisplayMember = "CurrencyType",
                ValueMember = "CurrencyTypeID",
                DataPropertyName = "CurrencyTypeID",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ClientContractDataGrid.Columns.Add(CurrencyColumnContract);
            ClientContractDataGrid.Columns["CurrencyType"].HeaderText = "Валюта";
            ClientContractDataGrid.Columns["CurrencyType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientContractDataGrid.Columns["CurrencyType"].MinimumWidth = 60;

            DataGridViewComboBoxColumn ClientColumnContract = new DataGridViewComboBoxColumn()
            {
                Name = "ClientName",
                DataSource = TableClients(),
                DisplayMember = "ClientName",
                ValueMember = "ClientID",
                DataPropertyName = "ClientID",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ClientContractDataGrid.Columns.Add(ClientColumnContract);
            ClientContractDataGrid.Columns["ClientName"].HeaderText = "Клиент";
            ClientContractDataGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            DataGridViewComboBoxColumn ContractFactoryColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FactoryName",
                DataSource = TableFactory(),
                DisplayMember = "FactoryName",
                ValueMember = "FactoryID",
                DataPropertyName = "FactoryID",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ClientContractDataGrid.Columns.Add(ContractFactoryColumn);
            ClientContractDataGrid.Columns["FactoryName"].HeaderText = "Участок";
            ClientContractDataGrid.Columns["FactoryName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            ClientContractDataGrid.Columns["StartDateContract"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientContractDataGrid.Columns["StartDateContract"].MinimumWidth = 110;
            ClientContractDataGrid.Columns["EndDateContract"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientContractDataGrid.Columns["EndDateContract"].MinimumWidth = 110;

            ClientContractDataGrid.Columns["Cost"].DefaultCellStyle.Format = "N";
            ClientContractDataGrid.Columns["Cost"].DefaultCellStyle.FormatProvider = nfi1;
            ClientContractDataGrid.Columns["Balance"].DefaultCellStyle.Format = "N";
            ClientContractDataGrid.Columns["Balance"].DefaultCellStyle.FormatProvider = nfi1;

            ClientContractDataGrid.Columns["ClientName"].DisplayIndex = 0;
            ClientContractDataGrid.Columns["ContractNumber"].DisplayIndex = 1;
            ClientContractDataGrid.Columns["Cost"].DisplayIndex = 2;
            ClientContractDataGrid.Columns["CurrencyType"].DisplayIndex = 3;
            ClientContractDataGrid.Columns["StartDateContract"].DisplayIndex = 4;
            ClientContractDataGrid.Columns["EndDateContract"].DisplayIndex = 5;
            ClientContractDataGrid.Columns["Terminated"].DisplayIndex = 6;
        }

        //private void FinancialReport(DateTime DateFrom)
        //{
        //    string ClientName;
        //    decimal DebtBeginningMonth = 0;
        //    decimal DebtEndMonth = 0;
        //    decimal ShippingCost = 0;
        //    decimal PaymentCost = 0;

        //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE cast (Date as date)>='" + DateFrom.ToString("yyyy-MM-dd") + "'", ConnectionStrings.MarketingReferenceConnectionString))
        //    {
        //        FinancialPaymentDT.Clear();

        //        if (DA.Fill(FinancialPaymentDT) == 0)
        //            return;
        //    }

        //    for (int i = 1; i < FinancialPaymentDT.Rows.Count; i++)
        //    {
        //        ClientName = "";
        //        DebtBeginningMonth = 0;
        //        DebtEndMonth = 0;
        //        ShippingCost = 0;
        //        PaymentCost = 0;


        //    }
        //}

        public void UpdateClientsPaymentsDataGrid()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments", ConnectionStrings.MarketingReferenceConnectionString))
            {
                ClientPaymentsTable.Clear();
                DA.Fill(ClientPaymentsTable);
            }
        }

        public void UpdateClientsPaymentsDataGrid(string ContractId)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE ContractId = " + ContractId, ConnectionStrings.MarketingReferenceConnectionString))
            {
                ClientPaymentsTable.Clear();
                DA.Fill(ClientPaymentsTable);
            }
        }

        public void UpdateClientsContractDataGrid(string ClientID, bool CheckFirmTPS, bool CheckFirmProfil, bool CheckClient)
        {
            if (CheckFirmTPS)
            {
                if (CheckClient)
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract WHERE ClientID = " + ClientID + "and FactoryID = 2", ConnectionStrings.MarketingReferenceConnectionString))
                    {
                        ClientContractTable.Clear();
                        DA.Fill(ClientContractTable);
                    }
                }
                else
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract WHERE FactoryID = 2", ConnectionStrings.MarketingReferenceConnectionString))
                    {
                        ClientContractTable.Clear();
                        DA.Fill(ClientContractTable);
                    }
                }
            }

            if (CheckFirmProfil)
            {
                if (CheckClient)
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract WHERE ClientID = " + ClientID + "and FactoryID = 1", ConnectionStrings.MarketingReferenceConnectionString))
                    {
                        ClientContractTable.Clear();
                        DA.Fill(ClientContractTable);
                    }
                }
                else
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract WHERE FactoryID = 1", ConnectionStrings.MarketingReferenceConnectionString))
                    {
                        ClientContractTable.Clear();
                        DA.Fill(ClientContractTable);
                    }
                }
            }
        }

        public void UpdateClientsPaymentsDataGrid(string ClientID, bool CheckFirmTPS, bool CheckFirmProfil, bool CheckClient)
        {
            if (CheckFirmTPS)
            {
                if (CheckClient)
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE ClientID = " + ClientID + "and FactoryID = 2", ConnectionStrings.MarketingReferenceConnectionString))
                    {
                        ClientPaymentsTable.Clear();
                        DA.Fill(ClientPaymentsTable);
                    }
                }
                else
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE FactoryID = 2", ConnectionStrings.MarketingReferenceConnectionString))
                    {
                        ClientPaymentsTable.Clear();
                        DA.Fill(ClientPaymentsTable);
                    }
                }
            }

            if (CheckFirmProfil)
            {
                if (CheckClient)
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE ClientID = " + ClientID + "and FactoryID = 1", ConnectionStrings.MarketingReferenceConnectionString))
                    {
                        ClientPaymentsTable.Clear();
                        DA.Fill(ClientPaymentsTable);
                    }
                }
                else
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE FactoryID = 1", ConnectionStrings.MarketingReferenceConnectionString))
                    {
                        ClientPaymentsTable.Clear();
                        DA.Fill(ClientPaymentsTable);
                    }
                }
            }
        }

        public void UpdateClientsContractDataGrid()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract", ConnectionStrings.MarketingReferenceConnectionString))
            {
                ClientContractTable.Clear();
                DA.Fill(ClientContractTable);
            }
        }

        public DataTable TableCurrency()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT CurrencyTypeID, CurrencyType FROM CurrencyTypes", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return DT;
                }
            }
        }

        public DataTable TableFactory()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FactoryID, FactoryName FROM Factory", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return DT;
                }
            }
        }

        public DataTable TableClients()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients ORDER BY ClientName", ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return DT;
                }
            }
        }

        public void UpdateTableContractWithAdd(string ClientID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract where ClientID = " + ClientID, ConnectionStrings.MarketingReferenceConnectionString))
            {
                ClientContractTable.Clear();
                DA.Fill(ClientContractTable);
            }
        }

        public void UpdateTableContract()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract", ConnectionStrings.MarketingReferenceConnectionString))
            {
                ClientContractTable.Clear();
                DA.Fill(ClientContractTable);
            }
        }

        public void CheckClientAndPeriod(string ClientID, DateTime DateFrom, DateTime DateTo, bool Check, bool Firm)
        {
            if (Check)
            {
                if (Firm)
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE ClientID = " + ClientID +
                    " AND cast (Date as date)>='" + DateFrom.ToString("yyyy-MM-dd") + "' AND cast (Date as date)<='" + DateTo.ToString("yyyy-MM-dd") + "' and FactoryID = 2", ConnectionStrings.MarketingReferenceConnectionString))
                    {
                        ClientPaymentsTable.Clear();

                        if (DA.Fill(ClientPaymentsTable) == 0)
                            return;
                    }
                }
                else
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE ClientID = " + ClientID +
                    " AND cast (Date as date)>='" + DateFrom.ToString("yyyy-MM-dd") + "' AND cast (Date as date)<='" + DateTo.ToString("yyyy-MM-dd") + "' and FactoryID = 1", ConnectionStrings.MarketingReferenceConnectionString))
                    {
                        ClientPaymentsTable.Clear();

                        if (DA.Fill(ClientPaymentsTable) == 0)
                            return;
                    }
                }
            }
            else
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE cast (Date as date)>='" + DateFrom.ToString("yyyy-MM-dd") + "' AND cast(Date as date)<='" + DateTo.ToString("yyyy-MM-dd") + "'", ConnectionStrings.MarketingReferenceConnectionString))
                {
                    ClientPaymentsTable.Clear();

                    if (DA.Fill(ClientPaymentsTable) == 0)
                        return;
                }
            }
        }

        public void CheckClientContractAndPeriod(string ClientID, DateTime DateFrom, DateTime DateTo, bool Check)
        {
            if (Check)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract WHERE ClientID = " + ClientID +
                " AND cast (StartDateContract as date)>='" + DateFrom.ToString("yyyy-MM-dd") + "' AND cast (StartDateContract as date)<='" + DateTo.ToString("yyyy-MM-dd") + "'", ConnectionStrings.MarketingReferenceConnectionString))
                {
                    ClientContractTable.Clear();

                    if (DA.Fill(ClientContractTable) == 0)
                        return;
                }
            }
            else
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract WHERE cast (StartDateContract as date)>='" + DateFrom.ToString("yyyy-MM-dd") + "' AND cast(StartDateContract as date)<='" + DateTo.ToString("yyyy-MM-dd") + "'", ConnectionStrings.MarketingReferenceConnectionString))
                {
                    ClientContractTable.Clear();

                    if (DA.Fill(ClientContractTable) == 0)
                        return;
                }
            }
        }

        public void SummMoneyEURPayments()
        {
            CostEUR = 0;
            CostEURShipping = 0;
            CostEURPayment = 0;

            foreach (DataRow Row in ClientPaymentsTable.Rows)
            {
                if (Row["CurrencyTypeID"].ToString() == "1")
                    if (Row["TypePayments"].ToString() == "Оплачено")
                    {
                        CostEURPayment += Convert.ToDecimal(Row["Cost"]);
                        CostEUR -= Convert.ToDecimal(Row["Cost"]);
                    }
                    else
                    {
                        CostEURShipping += Convert.ToDecimal(Row["Cost"]);
                        CostEUR += Convert.ToDecimal(Row["Cost"]);
                    }
            }
        }

        public void SummMoneyEURContracts()
        {
            CostEUR = 0;

            foreach (DataRow Row in ClientContractTable.Rows)
            {
                if (Row["CurrencyTypeID"].ToString() == "1")
                    if (Convert.ToBoolean(Row["Terminated"]) == false)
                    {
                        CostEUR += Convert.ToDecimal(Row["Cost"]);
                    }
            }
        }

        public void SummMoneyUSDPayments()
        {
            CostUSD = 0;
            CostUSDShipping = 0;
            CostUSDPayment = 0;

            foreach (DataRow Row in ClientPaymentsTable.Rows)
            {
                if (Row["CurrencyTypeID"].ToString() == "2")
                    if (Row["TypePayments"].ToString() == "Оплачено")
                    {
                        CostUSDPayment += Convert.ToDecimal(Row["Cost"]);
                        CostUSD -= Convert.ToDecimal(Row["Cost"]);
                    }
                    else
                    {
                        CostUSDShipping += Convert.ToDecimal(Row["Cost"]);
                        CostUSD += Convert.ToDecimal(Row["Cost"]);
                    }
            }
        }

        public void SummMoneyUSDContracts()
        {
            CostUSD = 0;

            foreach (DataRow Row in ClientContractTable.Rows)
            {
                if (Row["CurrencyTypeID"].ToString() == "2")
                    if (Convert.ToBoolean(Row["Terminated"]) == false)
                    {
                        CostUSD += Convert.ToDecimal(Row["Cost"]);
                    }
            }
        }

        public void SummMoneyRURPayments()
        {
            CostRUR = 0;
            CostRURShipping = 0;
            CostRURPayment = 0;

            foreach (DataRow Row in ClientPaymentsTable.Rows)
            {
                if (Row["CurrencyTypeID"].ToString() == "3")
                    if (Row["TypePayments"].ToString() == "Оплачено")
                    {
                        CostRURPayment += Convert.ToDecimal(Row["Cost"]);
                        CostRUR -= Convert.ToDecimal(Row["Cost"]);
                    }
                    else
                    {
                        CostRURShipping += Convert.ToDecimal(Row["Cost"]);
                        CostRUR += Convert.ToDecimal(Row["Cost"]);
                    }
            }
        }

        public void SummMoneyRURContracts()
        {
            CostRUR = 0;

            foreach (DataRow Row in ClientContractTable.Rows)
            {
                if (Row["CurrencyTypeID"].ToString() == "3")
                    if (Convert.ToBoolean(Row["Terminated"]) == false)
                    {
                        CostRUR += Convert.ToDecimal(Row["Cost"]);
                    }
            }
        }

        public void SummMoneyBYRPayments()
        {
            CostBYR = 0;
            CostBYRShipping = 0;
            CostBYRPayment = 0;

            foreach (DataRow Row in ClientPaymentsTable.Rows)
            {
                if (Row["CurrencyTypeID"].ToString() == "5")
                    if (Row["TypePayments"].ToString() == "Оплачено")
                    {
                        CostBYRPayment += Convert.ToDecimal(Row["Cost"]);
                        CostBYR -= Convert.ToDecimal(Row["Cost"]);
                    }
                    else
                    {
                        CostBYRShipping += Convert.ToDecimal(Row["Cost"]);
                        CostBYR += Convert.ToDecimal(Row["Cost"]);
                    }
            }
        }

        public void SummMoneyBYRContracts()
        {
            CostBYR = 0;

            foreach (DataRow Row in ClientContractTable.Rows)
            {
                if (Row["CurrencyTypeID"].ToString() == "5")
                    if (Convert.ToBoolean(Row["Terminated"]) == false)
                    {
                        CostBYR += Convert.ToDecimal(Row["Cost"]);
                    }
            }
        }

        public void GetAllPeriodPayments(bool Check, string ClientID)
        {
            if (Check)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE ClientID = " + ClientID + "AND cast (Date as date) IS NOT NULL " +
                                                             "ORDER BY Date ASC", ConnectionStrings.MarketingReferenceConnectionString))
                {
                    ClientPaymentsTable.Clear();
                    DA.Fill(ClientPaymentsTable);
                }


                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE  ClientID = " + ClientID + "AND cast (Date as date) IS NOT NULL " +
                                                             "ORDER BY Date DESC", ConnectionStrings.MarketingReferenceConnectionString))
                {
                    ClientPaymentsTable.Clear();
                    DA.Fill(ClientPaymentsTable);
                }
            }
            else
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE cast (Date as date) IS NOT NULL " +
                                                             "ORDER BY Date ASC", ConnectionStrings.MarketingReferenceConnectionString))
                {
                    ClientPaymentsTable.Clear();
                    DA.Fill(ClientPaymentsTable);
                }


                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE cast (Date as date) IS NOT NULL " +
                                                             "ORDER BY Date DESC", ConnectionStrings.MarketingReferenceConnectionString))
                {
                    ClientPaymentsTable.Clear();
                    DA.Fill(ClientPaymentsTable);
                }
            }
        }

        public void GetAllPeriodContracts(bool Check, string ClientID)
        {
            if (Check)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract WHERE ClientID = " + ClientID + "AND cast (StartDateContract as date) IS NOT NULL " +
                                                             "ORDER BY StartDateContract ASC", ConnectionStrings.MarketingReferenceConnectionString))
                {
                    ClientContractTable.Clear();
                    DA.Fill(ClientContractTable);
                }


                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract WHERE  ClientID = " + ClientID + "AND cast (StartDateContract as date) IS NOT NULL " +
                                                             "ORDER BY StartDateContract DESC", ConnectionStrings.MarketingReferenceConnectionString))
                {
                    ClientContractTable.Clear();
                    DA.Fill(ClientContractTable);
                }
            }
            else
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract WHERE cast (StartDateContract as date) IS NOT NULL " +
                                                             "ORDER BY StartDateContract ASC", ConnectionStrings.MarketingReferenceConnectionString))
                {
                    ClientContractTable.Clear();
                    DA.Fill(ClientContractTable);
                }


                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract WHERE cast (StartDateContract as date) IS NOT NULL " +
                                                             "ORDER BY StartDateContract DESC", ConnectionStrings.MarketingReferenceConnectionString))
                {
                    ClientContractTable.Clear();
                    DA.Fill(ClientContractTable);
                }
            }
        }

        public void AddPayments(string ClientID, bool TypePayments, string DocNumber, DateTime dataTime, string Cost, string CurrencyTypeID, string ContractId, string FactoryID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, TypePayments, DocNumber, Date, Cost"
                    + ", CurrencyTypeID, ContractId, FactoryID FROM ClientPayments", ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DataRow NewRow = DT.NewRow();
                        NewRow["ClientID"] = ClientID;

                        if (TypePayments)
                            NewRow["TypePayments"] = "Отгружено";
                        else
                            NewRow["TypePayments"] = "Оплачено";

                        NewRow["DocNumber"] = DocNumber;
                        NewRow["ContractId"] = ContractId;
                        NewRow["Date"] = dataTime;
                        NewRow["Cost"] = Cost;
                        NewRow["FactoryID"] = FactoryID;
                        NewRow["CurrencyTypeID"] = CurrencyTypeID;
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                    }
                }
            }
        }

        public void AddContracts(string ClientID, string ContractNumber, DateTime StartDateContract, DateTime EndDateContract, string Cost, string CurrencyTypeID, string FactoryID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract", ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DataRow NewRow = DT.NewRow();
                        NewRow["ClientID"] = ClientID;
                        NewRow["ContractNumber"] = ContractNumber;
                        NewRow["StartDateContract"] = StartDateContract;
                        NewRow["EndDateContract"] = EndDateContract;
                        NewRow["Cost"] = Cost;
                        NewRow["FactoryID"] = FactoryID;
                        NewRow["CurrencyTypeID"] = CurrencyTypeID;
                        NewRow["Terminated"] = false;
                        DT.Rows.Add(NewRow);
                        DA.Update(DT);
                    }
                }
            }
        }

        public void DeletePayments(string ClientPaymentsID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE ClientPaymentsID =" + ClientPaymentsID, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        DT.Rows[0].Delete();
                        DA.Update(DT);
                    }
                }
            }
        }

        //public void DeleteContracts(string ContractId)
        //{
        //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract WHERE ContractId =" + ContractId, ConnectionStrings.MarketingReferenceConnectionString))
        //    {
        //        using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
        //        {
        //            using (DataTable DT = new DataTable())
        //            {
        //                DA.Fill(DT);
        //                DT.Rows[0].Delete();
        //                DA.Update(DT);
        //            }
        //        }
        //    }
        //}

        public void CloseContract(string ContractId)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract WHERE ContractId =" + ContractId, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        DT.Rows[0]["Terminated"] = true;
                        DA.Update(DT);
                    }
                }
            }
        }

        public void UpdatePayments(string ClientPaymentsID, bool TypePayments, string DocNumber, DateTime dataTime, string Cost, string CurrencyTypeID, string FactoryID, string ContractId)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientPayments WHERE ClientPaymentsID =" + ClientPaymentsID, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (TypePayments)
                            DT.Rows[0]["TypePayments"] = "Отгружено";
                        else
                            DT.Rows[0]["TypePayments"] = "Оплачено";

                        DT.Rows[0]["DocNumber"] = DocNumber;
                        DT.Rows[0]["Date"] = dataTime;
                        DT.Rows[0]["Cost"] = Cost;
                        DT.Rows[0]["ContractId"] = ContractId;
                        DT.Rows[0]["CurrencyTypeID"] = CurrencyTypeID;
                        DT.Rows[0]["FactoryID"] = FactoryID;

                        DA.Update(DT);
                    }
                }
            }
        }

        public void UpdateContracts(string ContractId, string DocNumber, DateTime StartDateContract, DateTime EndDateContract, string Cost, string CurrencyTypeID, string FactoryID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ClientContract WHERE ContractId =" + ContractId, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DT.Rows[0]["ContractNumber"] = DocNumber;
                        DT.Rows[0]["StartDateContract"] = StartDateContract;
                        DT.Rows[0]["EndDateContract"] = EndDateContract;
                        DT.Rows[0]["Cost"] = Cost;
                        DT.Rows[0]["FactoryID"] = FactoryID;
                        DT.Rows[0]["CurrencyTypeID"] = CurrencyTypeID;

                        DA.Update(DT);
                    }
                }
            }
        }

        //public void GetFilter(string ContractId)
        //{
        //    using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ContractId, Terminated FROM [MarketingReference].[dbo].[ClientContract] where ContractId in (SELECT "
        //           + "ContractId FROM [MarketingReference].[dbo].[ClientPayments] where ContractId = " + ContractId + ") and Terminated = 0", ConnectionStrings.MarketingReferenceConnectionString))
        //    {
        //        FilterContract.Clear();
        //        DA.Fill(FilterContract);
        //    }
        //}

        public void Record()
        {
            decimal BalanceCost, PaymentsCost;
            string Itog;

            NumberFormatInfo nfi1 = new NumberFormatInfo()
            {
                NumberGroupSeparator = " ",
                NumberDecimalDigits = 2,
                NumberDecimalSeparator = ","
            };
            foreach (DataRow Row in ClientContractTable.DefaultView.ToTable().Rows)
            {
                BalanceCost = 0;
                PaymentsCost = 0;

                foreach (DataRow RowCostPayments in ClientPaymentsTable.Select("ContractId = " + Row["ContractId"] + " and TypePayments = 'Отгружено'"))
                {
                    PaymentsCost += Convert.ToInt32(RowCostPayments["Cost"]);
                }

                foreach (DataRow RowCostBalance in ClientContractTable.Select("ContractId = " + Row["ContractId"]))
                {
                    BalanceCost = Convert.ToInt32(RowCostBalance["Cost"]) - PaymentsCost;
                }
                Itog = BalanceCost.ToString("N", nfi1);
                ClientContractTable.Select("ContractId = " + Row["ContractId"])[0]["Balance"] = Itog;
            }
        }

        public void ExportToExcel()
        {
            ClearReport();
            Ex = new Excel();
            Ex.NewDocument(1);

            for (int i = 0; i < ClientsPaymentsDataGrid.Columns.Count; i++)
            {
                Ex.WriteCell(1, ClientsPaymentsDataGrid.Columns[i].HeaderText, 1, i + 1, 12, true);
                Ex.SetHorisontalAlignment(1, 1, i + 1, Excel.AlignHorizontal.xlCenter);
                Ex.SetBorderStyle(1, 1, i + 1, false, true, false, true, Excel.LineStyle.xlContinuous, Excel.BorderWeight.xlThin);
                if (ClientsPaymentsDataGrid.Columns[i].DefaultCellStyle.BackColor == Color.FromArgb(222, 222, 65))
                    for (int j = 0; j <= ClientsPaymentsDataGrid.Rows.Count; j++)
                        Ex.SetColor(1, j + 1, i + 1, Excel.Color.Yellow);
            }

            for (int i = 1; i < ClientsPaymentsDataGrid.Rows.Count + 1; i++)
            {
                Ex.WriteCell(1, ClientsPaymentsDataGrid.Rows[i - 1].Cells[0].Value, i + 1, 1, 12, true);
                Ex.SetBorderStyle(1, i + 1, 1, false, true, false, true, Excel.LineStyle.xlContinuous, Excel.BorderWeight.xlThin);
            }

            for (int i = 1; i < ClientsPaymentsDataGrid.Rows.Count + 1; i++)
            {
                for (int j = 1; j < ClientsPaymentsDataGrid.Columns.Count - 1; j++)
                {
                    if (ClientsPaymentsDataGrid.Rows[i - 1].Cells[j].Value.ToString() != "")
                    {
                        Ex.WriteCell(1, ClientsPaymentsDataGrid.Rows[i - 1].Cells[j].Value.ToString(), i + 1, j + 1, 12, false);
                    }
                    Ex.SetHorisontalAlignment(1, i + 1, j + 1, Excel.AlignHorizontal.xlCenter);
                    Ex.SetBorderStyle(1, i + 1, j + 1, false, true, false, true, Excel.LineStyle.xlContinuous, Excel.BorderWeight.xlThin);
                }
            }

            for (int i = 1; i < ClientsPaymentsDataGrid.Rows.Count + 1; i++)
            {
                if (ClientsPaymentsDataGrid.Rows[i - 1].Cells[ClientsPaymentsDataGrid.Columns.Count - 1].Value.ToString() != "")
                {
                    Ex.WriteCell(1, ClientsPaymentsDataGrid.Rows[i - 1].Cells[ClientsPaymentsDataGrid.Columns.Count - 1].Value.ToString(), i + 1, ClientsPaymentsDataGrid.Columns.Count, 12, true);
                }
                Ex.SetHorisontalAlignment(1, i + 1, ClientsPaymentsDataGrid.Columns.Count, Excel.AlignHorizontal.xlCenter);
                Ex.SetBorderStyle(1, i + 1, ClientsPaymentsDataGrid.Columns.Count, false, true, false, true, Excel.LineStyle.xlContinuous, Excel.BorderWeight.xlThin);
            }
            Ex.AutoFit(1, 1, 1, ClientsPaymentsDataGrid.Rows.Count + 1, 1);
            Ex.Visible = true;
        }

        public void ClearReport()
        {
            if (Ex != null)
            {
                Ex.Dispose();
                Ex = null;
            }
        }
    }
}
