using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Infinium.Modules.Editors
{
    public class LabelParamsManager
    {
        public BindingSource ParamsBS;

        DataTable ParamsDT;

        SqlCommandBuilder ParamsCB;

        SqlDataAdapter ParamsDA;

        public LabelParamsManager()
        {
            ParamsDT = new DataTable();
            ParamsBS = new BindingSource();

            string SelectCommand = @"SELECT * FROM LabelParams";
            ParamsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString);
            ParamsCB = new SqlCommandBuilder(ParamsDA);
            ParamsDA.Fill(ParamsDT);

            ParamsBS.DataSource = ParamsDT;
        }

        public void Save()
        {
            ParamsDA.Update(ParamsDT);
            ParamsDT.Clear();
            ParamsDA.Fill(ParamsDT);
        }
    }
}
