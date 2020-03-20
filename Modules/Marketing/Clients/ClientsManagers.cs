using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Infinium.Modules.Marketing.Clients
{

    public class ClientsManagers
    {
        public BindingSource ManagersBS;
        public BindingSource UsersBS;

        DataTable ManagersDT;
        DataTable UsersDT;

        SqlCommandBuilder ManagersCB;
        SqlCommandBuilder UsersСB;

        SqlDataAdapter ManagersDA;
        SqlDataAdapter UsersDA;

        public ClientsManagers()
        {
            ManagersDT = new DataTable();
            ManagersBS = new BindingSource();

            UsersDT = new DataTable();
            UsersBS = new BindingSource();

            string SelectCommand = @"SELECT ManagerID, Name, ShortName, Password, UserID, Phone, Email, Skype FROM ClientsManagers ORDER BY Name";
            ManagersDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString);
            ManagersCB = new SqlCommandBuilder(ManagersDA);
            ManagersDA.Fill(ManagersDT);

            SelectCommand = @"SELECT ShortName, UserID, Name, Password FROM Users ORDER BY Name";
            UsersDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString);
            UsersСB = new SqlCommandBuilder(UsersDA);
            UsersDA.Fill(UsersDT);

            ManagersBS.DataSource = ManagersDT;
            UsersBS.DataSource = UsersDT;
        }

        public void AddManager(int UserID, string Name, string ShortName, string Password, string Phone, string Email, string Skype)
        {
            DataRow NewRow = ManagersDT.NewRow();
            NewRow["UserID"] = UserID;
            NewRow["Password"] = Password;
            NewRow["ShortName"] = ShortName;
            NewRow["Name"] = Name;
            NewRow["Phone"] = Phone;
            NewRow["Email"] = Email;
            NewRow["Skype"] = Skype;
            ManagersDT.Rows.Add(NewRow);
        }

        public bool IsUserClientManager(int ManagerID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Clients WHERE ManagerID = " + ManagerID,
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    return DA.Fill(DT) > 0;
                }
            }
        }

        public void RemoveManager()
        {
            if (ManagersBS.Count == 0)
                return;
            int Pos = ManagersBS.Position;
            ManagersBS.RemoveCurrent();
            if (ManagersBS.Count > 0)
                if (Pos >= ManagersBS.Count)
                    ManagersBS.MoveLast();
                else
                    ManagersBS.Position = Pos;
        }

        public void SaveManagers()
        {
            ManagersDA.Update(ManagersDT);
            ManagersDT.Clear();
            ManagersDA.Fill(ManagersDT);
        }
    }
}
