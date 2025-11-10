// using CommandLine;
namespace BAS.Padrones.Tucuman
{
    public class Parametros
    {
        // [Option('a', "acreditan", Required = true, HelpText = "Ruta al archivo de padrón Acreditan")]
        public string? AcreditanFilepath { get; set; }

        
        // [Option('c', "coeficientes", Required = true, HelpText = "Ruta al archivo de Coeficientes")]
        public string? CoeficientesFilepath { get; set; }


        // [Option('o', "output", Required = true, HelpText = "Ruta al archivo de salida")]
        public string? OutputFilepath { get; set; }


        // [Option('p', "provincia", Required = true, HelpText = "Codigo de provincia")]
        public string? ProvinceCode { get; set; }
    }
}
