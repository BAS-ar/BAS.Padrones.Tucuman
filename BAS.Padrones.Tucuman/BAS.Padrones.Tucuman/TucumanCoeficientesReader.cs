using System.Globalization;
using System.Text;

namespace BAS.Padrones.Tucuman
{
    internal class TucumanCoeficientesReader
    {
        // This file seems similar to Acreditan but it doesn't seem to have the columns size problem.

        string _filePath = "";

        public TucumanCoeficientesReader(string filePath)
        {
            _filePath = filePath;
        }

        public List<CoeficienteRegistry> GetRegistries()
        {
            var coeficientesFileStream = new FileStream(_filePath, FileMode.Open);
            List<CoeficienteRegistry> padron = new();

            using (TextReader reader = new StreamReader(coeficientesFileStream, Encoding.UTF8))
            {
                string? line = "";
                reader.ReadLine(); // We skip the column's names
                while ((line = reader.ReadLine()) != null)
                {
                    var registry = new CoeficienteRegistry()
                    {
                        Cuit = line.Substring(0, 13).TrimEnd(),
                        Excento = line.Substring(13, 3).TrimEnd() == "E",
                        Fecha = DateTime.ParseExact(line.Substring(24, 6).TrimEnd(), "yyyyMM", CultureInfo.InvariantCulture),
                        Denominacion = line.Substring(32, 150).TrimEnd()
                    };

                    registry.ParseCoeficiente(line);
                    registry.ParsePorcentaje(line);

                    padron.Add(registry);
                }
            }

            return padron;
        }


    }
}
