using System.Data.SqlTypes;
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
            using (var sw = new StreamWriter("parsing_coefficients.txt", true))
            {
                sw.WriteLine($"Parseando porcentaje en coeficientes");
                sw.WriteLine($"Line: {line}");

                var porcentaje = line.Substring(line.Length - 5, 5).Trim();
                sw.WriteLine($"Substring: {porcentaje}");
                if (porcentaje == "-----")
                {
                    Porcentaje = null;
                    sw.WriteLine($"Retorno: null");
                }
                else
                {
                    Porcentaje = SanitizeDouble(porcentaje);
                    sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "Retorno: {0}", Porcentaje));
                }
                sw.WriteLine($"---***---");
            }
        }

        public void ParseCoeficiente(string line)
        {
            using (var sw = new StreamWriter("parsing_coefficients.txt", true))
            {
                sw.WriteLine($"Parseando coeficiente en coeficientes");
                sw.WriteLine($"Line: {line}");
                var substring = line.Substring(16, 6);
                sw.WriteLine($"Substring: {substring}");
                if (substring == "-.----" || substring == "-,----")
                {
                    Coeficiente = null;
                    sw.WriteLine($"Retorno: null");
                }
                else
                {
                    Coeficiente = SanitizeDouble(substring);
                    sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "Retorno: {0}", Coeficiente));
                }
                sw.WriteLine($"---***---");
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
