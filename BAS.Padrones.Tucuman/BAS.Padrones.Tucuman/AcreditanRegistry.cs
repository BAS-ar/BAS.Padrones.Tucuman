using System.Globalization;

namespace BAS.Padrones.Tucuman
{

    public class AcreditanRegistry
    {
        public string? Cuit {  get; set; }
        
        public bool Excento { get; set; }

        public Convenio Convenio { get; set; }

        public DateTime FechaDesde { get; set; }

        public DateTime FechaHasta { get; set; }

        public string? Denominacion { get; set; }

        public double? Porcentaje { get; set; }

        public override string ToString()
        {
            return $"{Cuit};{Excento};{Convenio};{FechaDesde};{Denominacion};{Porcentaje}";
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

        public double SanitizeDouble(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0.0;

            value = value.Replace(',', '.');
            return Double.Parse(value, CultureInfo.InvariantCulture);
        }
    }

}
