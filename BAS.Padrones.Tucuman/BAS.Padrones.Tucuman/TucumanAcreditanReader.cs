using System.Globalization;
using System.Text;

namespace BAS.Padrones.Tucuman
{
    internal class TucumanAcreditanReader
    {
        // PARSER "Acreditan"
        // File is space separated (yep) BUT i've got a file that has columns/separators of different sizes... so much fun!
        // It seems that columns are separated by 2 spaces. BUT. There are more than 1 space in between data in DENOMINACION too!
        // So, the field that seems to be shrinked sometimes is DENOMINACION by 1 char (or it's end separator got truncated)
        // I will split columns by fixed size. PORCENTAJE will be taken backwards (counting chars from end to start)
        // CUIT EXENTO CONVENIO DESDE HASTA DENOMINACION PORCENTAJE 
        // Chars (followed by 2 spaces afterwards)
        // 11   1      2        8     8     150          3

        string _filePath = "";

        public TucumanAcreditanReader(string filePath)
        {
            _filePath = filePath;
        }

        public List<AcreditanRegistry> GetRegistries()
        {
            var padronFileStream = new FileStream(_filePath, FileMode.Open);
            List<AcreditanRegistry> padron = new();

            using (TextReader reader = new StreamReader(padronFileStream, Encoding.UTF8))
            {

                string? line = "";
                reader.ReadLine(); // We skip the column's names
                while ((line = reader.ReadLine()) != null)
                {
                    var registry = new AcreditanRegistry()
                    {
                        Cuit = line.Substring(0, 13).TrimEnd(),
                        Excento = line.Substring(13, 3).TrimEnd() == "E",
                        Convenio = line.Substring(16, 4).TrimEnd() == "CM" ? Convenio.Multilateral : Convenio.Local,
                        FechaDesde = DateTime.ParseExact(line.Substring(20, 8).TrimEnd(), "yyyyMMdd", CultureInfo.InvariantCulture),
                        FechaHasta = DateTime.ParseExact(line.Substring(30, 8).TrimEnd(), "yyyyMMdd", CultureInfo.InvariantCulture),
                        Denominacion = line.Substring(40, 150).TrimEnd()
                    };

                    registry.ParsePorcentaje(line);

                    padron.Add(registry);
                }
            }

            return padron;
        }
    }
}
