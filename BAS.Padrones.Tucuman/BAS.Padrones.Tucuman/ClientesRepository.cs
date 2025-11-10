using Microsoft.Data.SqlClient;

namespace BAS.Padrones.Tucuman
{
    internal class ClientesRepository : IClientesRepository
    {
        private string _connectionString;
        private List<string> _clientesLocalesCache;

        public ClientesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ObtenerClientesLocales(string provinciaCode)
        {
            _clientesLocalesCache = new List<string>();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "SP_PADRONES_ESLOCAL";

                SqlParameter provinciaParameter = command.Parameters.Add("@codprv", System.Data.SqlDbType.Char);
                provinciaParameter.Value = provinciaCode;

                SqlParameter returnValue = command.Parameters.Add("@RETURN_VALUE", System.Data.SqlDbType.Int);
                returnValue.Direction = System.Data.ParameterDirection.ReturnValue;

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if(!reader.IsDBNull(0))
                        {
                            _clientesLocalesCache.Add(reader.GetString(0));
                        }
                    }
                }
            }
        }

        public bool EsLocalUsarCache(string cuit)
        {
            var esLocal = _clientesLocalesCache.Any(c => c.CompareTo(cuit) == 0);
            return esLocal;
        }
    }
}
