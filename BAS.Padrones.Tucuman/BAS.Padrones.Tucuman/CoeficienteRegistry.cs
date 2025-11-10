using System.Globalization;

namespace BAS.Padrones.Tucuman
{
    internal class CoeficienteRegistry
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
                Porcentaje = Double.Parse(porcentaje, CultureInfo.InvariantCulture);
            }
        }

        public void ParseCoeficiente(string coeficiente)
        {
            var substring = coeficiente.Substring(16, 6);

            if (substring == "-.----")
            {
                Coeficiente = null;
            }
            else
            {
                Coeficiente = Double.Parse(substring, CultureInfo.InvariantCulture);
            }
        }
    }
}
