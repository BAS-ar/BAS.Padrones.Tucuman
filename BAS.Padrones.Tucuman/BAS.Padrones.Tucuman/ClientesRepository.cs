using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAS.Padrones.Tucuman
{
    internal class ClientesRepository : IClientesRepository
    {
        private string _connectionString;
        private int _clientesLocalesCount;
        private List<string> _clientesLocalesCache;

        public ClientesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int ObtenerClientesLocalesCount()
        {
            return _clientesLocalesCount;
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
            if (esLocal)
            {
                _clientesLocalesCount++;
            }
            return esLocal;
        }

        public bool EsLocal(string cuit, string provinciaCode)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "fn_padrones_eslocal";

                SqlParameter cuitParameter = command.Parameters.Add("@cuit", System.Data.SqlDbType.Char);
                cuitParameter.Value = cuit;
                SqlParameter provinciaParameter = command.Parameters.Add("@codprv", System.Data.SqlDbType.Char);
                provinciaParameter.Value = provinciaCode;

                SqlParameter returnValue = command.Parameters.Add("@RETURN_VALUE", System.Data.SqlDbType.Bit);
                returnValue.Direction = System.Data.ParameterDirection.ReturnValue;

                connection.Open();
                command.ExecuteNonQuery();

                if (returnValue != null && (bool)returnValue.Value)
                {
                    _clientesLocalesCount++;
                }

                return returnValue != null && (bool)returnValue.Value;
            }

        }
    }
}
