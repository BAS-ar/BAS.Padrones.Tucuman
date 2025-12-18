using System.Globalization;

namespace BAS.Padrones.Tucuman
{
    public class CoeficienteRegistry
    {
        public string? Cuit { get; set; }

        public bool Excento { get; set; }

        public Double? Coeficiente { get; set; }

        public DateTime Fecha { get; set; }

        public string? Denominacion { get; set; }

        public double? Porcentaje { get; set; }

        public override string ToString()
        {
            return $"{Cuit};{Excento};{Coeficiente};{Fecha};{Denominacion};{Porcentaje}";
        }

        public void ParsePorcentaje(string line)
        {
            var porcentaje = line.Substring(line.Length - 5, 5).Trim();

            if (porcentaje == "-----")
            {
                Porcentaje = null;
            }
            else
            {
                Porcentaje = SanitizeDouble(porcentaje);
            }
        }

        public void ParseCoeficiente(string coeficiente)
        {
            var substring = coeficiente.Substring(16, 6);

            if (substring == "-.----" || substring == "-,----")
            {
                Coeficiente = null;
            }
            else
            {
                Coeficiente = SanitizeDouble(substring);
            }
        }

        public double SanitizeDouble(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0.0;

            value = value.Replace(',', '.');
            return Double.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}
