using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace Infinium.Modules.Marketing.ClientMessages
{
    public class ConnectClient
    {
        public DataTable ClientDataTable;
        public BindingSource ClientBindingSource;
        SqlDataAdapter ClientDA;
        SqlCommandBuilder ClientCB;

        public DataTable ChatDataTable;
        public BindingSource ChatBindingSource;
        SqlDataAdapter ChatDA;
        SqlCommandBuilder ChatCB;

        public DataGridView ClientGrid, MessagersGrid;
        DataTable UpdateMess;

        public ConnectClient(ref PercentageDataGrid tClientGrid, ref PercentageDataGrid tMessagersGrid)
        {
            ClientGrid = tClientGrid;
            MessagersGrid = tMessagersGrid;
            CreateAndFill();
            UpdateMess = new DataTable();
            UpdateMessagersGrid("0");
            MessagersGrid.DataSource = UpdateMess;
            Binding();
        }

        public void UpdateMessagersGrid(string ClientID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ChatPermission WHERE ClientID = " + ClientID, ConnectionStrings.MarketingReferenceConnectionString))
            {
                UpdateMess.Clear();
                DA.Fill(UpdateMess);
            }
        }

        private void CreateAndFill()
        {
            ClientDataTable = new DataTable();
            ClientDA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients", ConnectionStrings.MarketingReferenceConnectionString);
            ClientCB = new SqlCommandBuilder(ClientDA);
            ClientDA.Fill(ClientDataTable);

            ChatDataTable = new DataTable();
            ChatDA = new SqlDataAdapter("SELECT ClientID, UserID FROM ChatPermission", ConnectionStrings.MarketingReferenceConnectionString);
            ChatCB = new SqlCommandBuilder(ChatDA);
            ChatDA.Fill(ChatDataTable);
        }

        private void Binding()
        {
            ClientBindingSource = new BindingSource() { DataSource = ClientDataTable };
            ChatBindingSource = new BindingSource() { DataSource = ChatDataTable };
        }

        public DataTable TableUsers()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name FROM Users WHERE Fired <> 1  ORDER BY Name", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return DT;
                }
            }
        }

        public void AddChat(string ClientID, string UserID)
        {
            if (UpdateMess.Select("UserId = " + UserID).Count() == 0)
            {
                DataRow NewRow = ChatDataTable.NewRow();

                NewRow["ClientID"] = ClientID;
                NewRow["UserID"] = UserID;
                ChatDataTable.Rows.Add(NewRow);
                ChatDA.Update(ChatDataTable);
            }
        }

        public void DeleteBlogger()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ChatPermission WHERE UserID =" + MessagersGrid.SelectedRows[0].Cells["UserID"].Value.ToString(), ConnectionStrings.MarketingReferenceConnectionString))
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
    }

    public class ConnectClientMessage
    {
        public int CurrentUserID = Security.CurrentUserID;

        public DataTable ClientDataTable;
        public BindingSource ClientBindingSource;
        SqlDataAdapter ClientDA;
        SqlCommandBuilder ClientCB;

        public string CurrentUserName;

        public DataTable SelectedUsersDataTable;
        public BindingSource SelectedUsersBindingSource;
        public BindingSource UsersBindingSource;

        public DataTable MessagesDataTable, UsersDataTable;

        ClientsMessagesDataGrid SelectedUsersGrid = null;
        ClientsDataGrid UsersListDataGrid = null;

        public ConnectClientMessage(ref ClientsMessagesDataGrid tSelectedUsersGrid, ref ClientsDataGrid tUsersListDataGrid)
        {
            UsersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients ORDER BY ClientName ASC", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(UsersDataTable);
            }

            CreateAndFill();
            Binding();

            ClientDataTable.Columns.Add(new DataColumn("OnlineStatus", Type.GetType("System.Boolean")));

            CurrentUserName = Security.GetUserNameByID(CurrentUserID);

            MessagesDataTable = new DataTable();

            SelectedUsersDataTable = new DataTable();
            SelectedUsersDataTable = ClientDataTable.Clone();
            SelectedUsersDataTable.Columns.Add(new DataColumn("UpdatesCount", Type.GetType("System.Int32")));

            SelectedUsersGrid = tSelectedUsersGrid;
            UsersListDataGrid = tUsersListDataGrid;

            SelectedUsersBindingSource = new BindingSource()
            {
                DataSource = SelectedUsersDataTable
            };
            SelectedUsersGrid.DataSource = SelectedUsersBindingSource;

            UsersListDataGrid.DataSource = ClientBindingSource;

            SetSelectedUsersGrid();
            SetUsersGrid();
        }

        private void CreateAndFill()
        {
            ClientDataTable = new DataTable();
            ClientDA = new SqlDataAdapter("SELECT ClientID, ClientName, Online FROM Clients", ConnectionStrings.MarketingReferenceConnectionString);
            ClientCB = new SqlCommandBuilder(ClientDA);
            ClientDA.Fill(ClientDataTable);
        }

        private void Binding()
        {
            ClientBindingSource = new BindingSource() { DataSource = ClientDataTable };
        }

        public void SetSelectedUsersGrid()
        {
            SelectedUsersGrid.AddColumns();

            SelectedUsersGrid.Columns["UpdatesCount"].Visible = false;
            SelectedUsersGrid.Columns["OnlineStatus"].Visible = false;
            SelectedUsersGrid.Columns["CloseColumn"].DisplayIndex = 3;
            SelectedUsersGrid.Columns["OnlineColumn"].DisplayIndex = 0;
            SelectedUsersGrid.Columns["Online"].Visible = false;
            SelectedUsersGrid.sNewMessagesColumnName = "UpdatesCount";
            SelectedUsersGrid.sOnlineStatusColumnName = "OnlineStatus";
        }

        public void SetUsersGrid()
        {
            UsersListDataGrid.AddColumns();
            UsersListDataGrid.sOnlineStatusColumnName = "OnlineStatus";
            UsersListDataGrid.Columns["OnlineColumn"].DisplayIndex = 0;
            UsersListDataGrid.Columns["OnlineStatus"].Visible = false;
            UsersListDataGrid.Columns["Online"].Visible = false;
        }

        public void UpdateList()
        {
            ClientDataTable.Clear();
            ClientDA.Fill(ClientDataTable);
        }

        public void AddUserToSelected(string ClientID)
        {
            if (SelectedUsersDataTable.Select("ClientID = " + ClientID).Count() > 0)
            {
                SelectedUsersBindingSource.Position = SelectedUsersBindingSource.Find("ClientID", ClientID);

                return;
            }

            DataRow[] Row = ClientDataTable.Select("ClientID = " + ClientID);

            SelectedUsersDataTable.ImportRow(Row[0]);

            SelectedUsersBindingSource.Position = SelectedUsersBindingSource.Find("ClientID", ClientID);
        }

        public void RemoveCurrent()
        {
            int Pos = SelectedUsersBindingSource.Position;
            SelectedUsersBindingSource.RemoveCurrent();

            //остается на позиции удаленного
            if (SelectedUsersBindingSource.Count > 0)
                if (Pos >= SelectedUsersBindingSource.Count)
                {
                    SelectedUsersBindingSource.MoveLast();
                    SelectedUsersGrid.Rows[SelectedUsersGrid.Rows.Count - 1].Selected = true;
                }
                else
                    SelectedUsersBindingSource.Position = Pos;

            ((DataTable)SelectedUsersBindingSource.DataSource).AcceptChanges();
        }

        public void ClearCurrentUpdates()
        {
            ((DataRowView)SelectedUsersBindingSource.Current)["UpdatesCount"] = 0;
        }

        public void FillMessages(int SenderID)
        {
            MessagesDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 100 ClientsMessageID, SenderID, RecipientID,SenderTypeID, RecipientTypeID,"
                + " SendDateTime, MessageText, Clients.ClientName AS SenderName, infiniu2_users.dbo.Users.Name AS SenderName2 "
                + " FROM infiniu2_marketingreference.dbo.ClientsMessage"
                + " left JOIN Clients ON Clients.ClientID = infiniu2_marketingreference.dbo.ClientsMessage.SenderID and SenderTypeID = 1"
                + " left JOIN infiniu2_users.dbo.Users ON infiniu2_users.dbo.Users.UserID = infiniu2_marketingreference.dbo.ClientsMessage.SenderID and SenderTypeID = 0"
                + " WHERE (RecipientID = " + SenderID + " AND SenderID = " + CurrentUserID + ") OR (RecipientID = " + CurrentUserID + " AND SenderID = " + SenderID +
                ") ORDER BY SendDateTime DESC", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(MessagesDataTable);
                for (int i = 0; i < MessagesDataTable.Rows.Count; i++)
                {
                    if (MessagesDataTable.Rows[i]["SenderName"] == DBNull.Value)
                        MessagesDataTable.Rows[i]["SenderName"] = MessagesDataTable.Rows[i]["SenderName2"];
                }
                MessagesDataTable.Columns.Remove("SenderName2");
            }
        }

        private string GetUserNameByID(int UserID)
        {
            return UsersDataTable.Select("UserID = " + UserID)[0]["Name"].ToString();
        }

        public bool IsEmptyMessage(string sText)
        {
            if (sText.Length == 0)
                return true;

            int n = 0;

            foreach (char c in sText)
            {
                if (c == '\n' || c == '\r' || c == ' ')
                    n++;
            }

            if (n == sText.Length)
                return true;

            return false;
        }

        public void AddMessage(string sText)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM ClientsMessage", ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        for (int i = 0; i < ClientDataTable.Rows.Count; i++)
                        {
                            int RecipientID = Convert.ToInt32(ClientDataTable.Rows[i]["ClientID"]);
                            DataRow NewRow = DT.NewRow();
                            NewRow["SenderID"] = CurrentUserID;
                            NewRow["RecipientID"] = RecipientID;
                            NewRow["MessageText"] = sText;
                            NewRow["SenderTypeID"] = false;
                            NewRow["RecipientTypeID"] = true;

                            DateTime DateTime = Security.GetCurrentDate();

                            NewRow["SendDateTime"] = DateTime;
                            DT.Rows.Add(NewRow);

                            DA.Update(DT);

                            FillMessages(RecipientID);

                            DataRow[] Row = MessagesDataTable.Select("SendDateTime = '" + DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'");

                            using (SqlDataAdapter sDA = new SqlDataAdapter("SELECT TOP 0 * FROM SubscribesRecords", ConnectionStrings.LightConnectionString))
                            {
                                using (SqlCommandBuilder sCB = new SqlCommandBuilder(sDA))
                                {
                                    using (DataTable sDT = new DataTable())
                                    {
                                        sDA.Fill(sDT);

                                        DataRow sNewRow = sDT.NewRow();
                                        sNewRow["SubscribesItemID"] = 10;
                                        sNewRow["TableItemID"] = Convert.ToInt32(Row[0][0]);
                                        sNewRow["UserID"] = RecipientID;
                                        sNewRow["UserTypeID"] = 2;
                                        sDT.Rows.Add(sNewRow);

                                        sDA.Update(sDT);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddMessage(int RecipientID, string sText)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM ClientsMessage", ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DataRow NewRow = DT.NewRow();
                        NewRow["SenderID"] = CurrentUserID;
                        NewRow["RecipientID"] = RecipientID;
                        NewRow["MessageText"] = sText;
                        NewRow["SenderTypeID"] = false;
                        NewRow["RecipientTypeID"] = true;

                        DateTime DateTime = Security.GetCurrentDate();

                        NewRow["SendDateTime"] = DateTime;
                        DT.Rows.Add(NewRow);

                        DA.Update(DT);

                        FillMessages(RecipientID);

                        DataRow[] Row = MessagesDataTable.Select("SendDateTime = '" + DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'");

                        using (SqlDataAdapter sDA = new SqlDataAdapter("SELECT TOP 0 * FROM SubscribesRecords", ConnectionStrings.LightConnectionString))
                        {
                            using (SqlCommandBuilder sCB = new SqlCommandBuilder(sDA))
                            {
                                using (DataTable sDT = new DataTable())
                                {
                                    sDA.Fill(sDT);

                                    DataRow sNewRow = sDT.NewRow();
                                    sNewRow["SubscribesItemID"] = 10;
                                    sNewRow["TableItemID"] = Convert.ToInt32(Row[0][0]);
                                    sNewRow["UserID"] = RecipientID;
                                    sNewRow["UserTypeID"] = 2;
                                    sDT.Rows.Add(sNewRow);

                                    sDA.Update(sDT);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GetNewMessages()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM infiniu2_marketingreference.dbo.ClientsMessage WHERE infiniu2_marketingreference.dbo.ClientsMessage.ClientsMessageID IN (SELECT TableItemID FROM SubscribesRecords WHERE SubscribesItemID = 10 AND UserTypeID = 0 AND UserID = " + CurrentUserID + ") ORDER BY SendDateTime DESC", ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return;

                    foreach (DataRow Row in DT.Rows)
                    {
                        AddSenderToSelected(Convert.ToInt32(Row["SenderID"]));
                    }
                }
            }
        }

        public void AddSenderToSelected(int ClientID)
        {
            DataRow[] sRow = SelectedUsersDataTable.Select("ClientID = " + ClientID);

            if (sRow.Count() > 0)
            {
                sRow[0]["UpdatesCount"] = 1;
                return;
            }

            DataRow[] Row = ClientDataTable.Select("ClientID = " + ClientID);

            DataRow NewRow = SelectedUsersDataTable.NewRow();
            NewRow["ClientID"] = Row[0]["ClientID"];
            NewRow["ClientName"] = Row[0]["ClientName"];
            NewRow["UpdatesCount"] = 1;
            SelectedUsersDataTable.Rows.Add(NewRow);
        }

        public void CheckOnline()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID FROM Clients WHERE Online = 1", ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    try
                    {
                        DA.Fill(DT);
                    }
                    catch
                    {
                        return;
                    }

                    foreach (DataRow Row in ClientDataTable.Rows)
                    {
                        if (DT.Select("ClientID = " + Row["ClientID"]).Count() > 0)
                            Row["OnlineStatus"] = true;
                        else
                            Row["OnlineStatus"] = false;
                    }

                    foreach (DataRow Row in SelectedUsersDataTable.Rows)
                    {
                        if (DT.Select("ClientID = " + Row["ClientID"]).Count() > 0)
                            Row["OnlineStatus"] = true;
                        else
                            Row["OnlineStatus"] = false;
                    }
                }
            }
        }
    }
}
