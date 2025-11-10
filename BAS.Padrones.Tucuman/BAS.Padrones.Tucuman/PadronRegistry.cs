using System.Globalization;
using System.Text;

namespace BAS.Padrones.Tucuman
{
    internal class PadronRegistry
    {
        public Regimen Regimen { get; set; }

        public DateTime FechaPublicacion { get; set; }

        public DateTime FechaDesde { get; set; }

        public DateTime FechaHasta { get; set; }

        public string? Cuit { get; set; }

        public Convenio Convenio { get; set; }

        public AltaBaja? AltaBaja { get; set; }

        public bool? Actualizado { get; set; }

        public double? Alicuota { get; set; }

        public int? Grupo { get; set; }

        public PadronRegistry()
        {
                
        }

        public PadronRegistry(AcreditanRegistry acreditanRegistry, double alicuota)
        {
            Regimen = Regimen.Percepcion;
            FechaPublicacion = acreditanRegistry.FechaDesde;
            FechaDesde = acreditanRegistry.FechaDesde;
            FechaHasta = acreditanRegistry.FechaHasta;
            Cuit = acreditanRegistry.Cuit;
            Convenio = acreditanRegistry.Convenio;
            AltaBaja = null;
            Actualizado = false;
            Alicuota = alicuota;
            Grupo = null;
        }

        public PadronRegistry(CoeficienteRegistry coeficienteRegistry, double alicuota)
        {
            Regimen = Regimen.Retencion;
            FechaPublicacion = coeficienteRegistry.Fecha;
            Cuit = coeficienteRegistry.Cuit;
            AltaBaja = null;
            Actualizado = false;
            Alicuota = alicuota;
            Grupo = null;
        }

        public override string ToString()
        {
            // Padron Bs.As.
            // +----+-----------------------------+------+----------+-------+-------+---------------------------------------------------------------+
            // | Nº | Campo                       | Tipo | Longitud | Desde | Hasta | Descripción                                                   |
            // +----+-----------------------------+------+----------+-------+-------+---------------------------------------------------------------+
            // |  1 | Régimen                     | A    | 1        | 1     | 1     | R o P según se trate del régimen de Retención o Percepción    |
            // |  2 | ";"                         | A    | 1        | 2     | 2     | Separador                                                     |
            // |  3 | Fecha de Publicación        | N    | 8        | 3     | 10    | Formato DDMMAAAA                                              |
            // |  4 | ";"                         | A    | 1        | 11    | 11    | Separador                                                     |
            // |  5 | Fecha de vigencia Desde     | N    | 8        | 12    | 19    | Formato DDMMAAAA                                              |
            // |  6 | ";"                         | A    | 1        | 20    | 20    | Separador                                                     |
            // |  7 | Fecha de vigencia Hasta     | N    | 8        | 21    | 28    | Formato DDMMAAAA                                              |
            // |  8 | ";"                         | A    | 1        | 29    | 29    | Separador                                                     |
            // |  9 | Numero CUIT                 | N    | 11       | 30    | 40    | CUIT del sujeto                                               |
            // | 10 | ";"                         | A    | 1        | 41    | 41    | Separador                                                     |
            // | 11 | Tipo Contribuyente Insc     | A    | 1        | 42    | 42    | "C" Convenio Multilateral - "D" Directo Pcia de Bs.As.        |
            // | 12 | ";"                         | A    | 1        | 43    | 43    | Separador                                                     |
            // | 13 | Marca Alta - Baja - Sujeto  | A    | 1        | 44    | 44    | S o N - "S" indica alta, "B" indica baja                      |
            // | 14 | ";"                         | A    | 1        | 45    | 45    | Separador                                                     |
            // | 15 | Marca cambio alícuota       | A    | 1        | 46    | 46    | S o N - indica si hubo cambio de alícuota                     |
            // | 16 | ";"                         | A    | 1        | 47    | 47    | Separador                                                     |
            // | 17 | Alícuota                    | A    | 4        | 48    | 51    | Formato 9,99                                                  |
            // | 18 | ";"                         | A    | 1        | 52    | 52    | Separador                                                     |
            // | 19 | Nro Grupo                   | N    | 2        | 53    | 54    |                                                               |
            // | 20 | ";"                         | A    | 1        | 55    | 55    | Separador                                                     |
            // +----+-----------------------------+------+----------+-------+-------+---------------------------------------------------------------+

            var sb = new StringBuilder();
            sb.Append($"{(Regimen == Regimen.Retencion? "R" : "P")}");
            sb.Append(";");
            sb.Append($"{FechaPublicacion.ToString("ddMMyyyy")}"); // Yes... this format. nc.
            sb.Append(";");
            sb.Append($"{FechaDesde.ToString("ddMMyyyy")}");
            sb.Append(";");
            sb.Append($"{FechaHasta.ToString("ddMMyyyy")}");
            sb.Append(";");
            sb.Append($"{Cuit}");
            sb.Append(";");
            sb.Append($"{(Convenio == Convenio.Multilateral ? "C" : "D")}");
            sb.Append(";");

            if(AltaBaja.HasValue && AltaBaja == Tucuman.AltaBaja.Alta)
            {
                sb.Append($"S");
            }

            if (AltaBaja.HasValue && AltaBaja == Tucuman.AltaBaja.Baja)
            {
                sb.Append($"B");
            }

            if (!AltaBaja.HasValue || AltaBaja == Tucuman.AltaBaja.None)
            {
                sb.Append($"");
            }

            sb.Append(";");

            if (Actualizado.HasValue && Actualizado.Value)
            {
                sb.Append("S");
            }
            else
            {
                sb.Append("N");
            }
            
            sb.Append(";");
            sb.Append($"{(Alicuota?? Alicuota!.Value).ToString("N2", CultureInfo.InvariantCulture)}");
            sb.Append(";");
            sb.Append($"{Grupo}");
            sb.Append(";");

            return sb.ToString();
        }
    }
}
