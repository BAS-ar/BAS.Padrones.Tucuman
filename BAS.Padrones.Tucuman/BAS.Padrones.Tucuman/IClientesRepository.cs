namespace BAS.Padrones.Tucuman
{
    public interface IClientesRepository
    {
        public void ObtenerClientesLocales(string provinciaCode);

        public bool EsLocalUsarCache(string cuit);
    }
}
