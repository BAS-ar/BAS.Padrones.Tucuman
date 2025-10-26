using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAS.Padrones.Tucuman
{
    internal class ClientesRepositoryMock : IClientesRepository
    {
        private List<string> _clientesLocalesCache;
        private int _clientesLocalesCount;

        public bool EsLocal(string cuit, string provinciaCode)
        {
            if (cuit == "20364986352" && provinciaCode == "914")
            {
                return true;
            }
            return false;
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

        public void ObtenerClientesLocales(string provinciaCode)
        {
            _clientesLocalesCache = new List<string>();
            _clientesLocalesCache.Add("20364986352");
        }

        public int ObtenerClientesLocalesCount()
        {
            return _clientesLocalesCount;
        }
    }
}
