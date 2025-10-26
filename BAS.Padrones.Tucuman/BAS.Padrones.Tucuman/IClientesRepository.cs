using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAS.Padrones.Tucuman
{
    public interface IClientesRepository
    {
        public void ObtenerClientesLocales(string provinciaCode);

        public bool EsLocalUsarCache(string cuit);

        public bool EsLocal(string cuit, string provinciaCode);

        public int ObtenerClientesLocalesCount();
    }
}
