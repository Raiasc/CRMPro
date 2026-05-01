using System.Data.SqlClient;





namespace CRMPro.Data
{
    public class SqlBaglanti
    {
       
        private readonly string _connectionString = "Server=DESKTOP-QP8BQMD\\SQLEXPRESS; Database=CrmProDb; Integrated Security=True;";

        public SqlConnection BaglantiAl()
        {
            return new SqlConnection(_connectionString);
        }
    }
}