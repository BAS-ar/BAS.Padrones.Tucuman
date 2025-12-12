using Microsoft.Extensions.Configuration;

namespace BAS.Padrones.Tucuman
{
    public class Configuracion
    {
        public double RazonCoeficiente = 0.5;
        public double AlicuotaEspecial = 0.17;
        public bool CoeficientesParaExistentes = false;
        public bool CoeficientesParaInexistentes = false;

        public void CargarDesdeFrameworks(IConfiguration configuration)
        {
            RazonCoeficiente = configuration.GetSection("Coeficiente correccion").Get<double>();
            AlicuotaEspecial = configuration.GetSection("Alicuota especial").Get<double>();
            CoeficientesParaExistentes = configuration.GetSection("Evaluar coeficientes para existentes en padron").Get<bool>();
            CoeficientesParaInexistentes = configuration.GetSection("Evaluar coeficientes para inexistentes en padron").Get<bool>();

            if (AlicuotaEspecial.ToString().Split(',')[1].Length > 2)
            {
                throw new Exception($"No se permiten más de dos espacios decimales en el valor de la alícuota. Valor actual: {AlicuotaEspecial}");
            }
        }
    }
}
