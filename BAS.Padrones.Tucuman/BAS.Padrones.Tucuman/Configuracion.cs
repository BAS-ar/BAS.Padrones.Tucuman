using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace BAS.Padrones.Tucuman
{
    public class Configuracion
    {
        public double CoeficienteCorreccion = 0.5;
        public double AlicuotaEspecial = 0.17;
        public bool CoeficientesParaExistentes = false;
        public bool CoeficientesParaInexistentes = false;

        public void CargarDesdeFramework(IConfiguration configuration)
        {
            if (HasMoreThanTwoDecimals(configuration.GetSection("Alicuota especial").Get<string>()))
            {
                throw new Exception($"No se permiten más de dos espacios decimales en el valor de la alícuota. Valor actual: {AlicuotaEspecial}");
            }

            CoeficienteCorreccion = SanitizeDouble(configuration.GetSection("Coeficiente correccion").Get<string>());
            AlicuotaEspecial = SanitizeDouble(configuration.GetSection("Alicuota especial").Get<string>());

            if (configuration.GetSection("Evaluar coeficientes para existentes en padron").Exists())
            {
                CoeficientesParaExistentes = configuration.GetSection("Evaluar coeficientes para existentes en padron").Get<bool>();
            }
            else
            {
                Console.WriteLine("No existe la clave \"Evaluar coeficientes para existentes en padrón\" configurando en false");
                CoeficientesParaExistentes = false;
            }

            if (configuration.GetSection("Evaluar coeficientes para inexistentes en padron").Exists())
            {
                CoeficientesParaInexistentes = configuration.GetSection("Evaluar coeficientes para inexistentes en padron").Get<bool>();
            }
            else
            {
                Console.WriteLine("No existe la clave \"Evaluar coeficientes para inexistentes en padrón\" configurando en false");
                CoeficientesParaInexistentes = false;
            }

            if (CoeficienteCorreccion <= 0)
            {
                throw new Exception($"El valor del coeficiente correccion es cero o menor que cero");
            }

            if (AlicuotaEspecial <= 0)
            {
                throw new Exception($"El valor la alicuota especial es cero o menor que cero");
            }
        }

        public double SanitizeDouble(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0.0;

            value = value.Replace(',', '.');
            return Double.Parse(value, CultureInfo.InvariantCulture);
        }


        public bool HasMoreThanTwoDecimals(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception($"Valor de alícuota especial nulo.");

            value = value.Replace(',', '.');
            var parts = value.Split('.');

            // invalid case
            if (parts.Length > 2)
                return true;

            if (parts.Length == 1)
                return false;

            return parts[1].Length > 2;
        }
    }
}
