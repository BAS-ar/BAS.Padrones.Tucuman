// 2025-10-09
// Iván Sierra
// BAS Software

using BAS.Padrones.Tucuman;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;
//using CommandLine; 

string acreditanFilepath = "";
string coeficientesFilepath = "";
string outputFilepath = "";
string provinceCode = "";

var parser = new Parser(args);
var options = parser.GetOptions(); 

acreditanFilepath = options.AcreditanFilepath ?? "acreditan.txt";
coeficientesFilepath = options.CoeficientesFilepath ?? "coeficientes.txt";
outputFilepath = options.OutputFilepath ?? "output.txt";
provinceCode = options.ProvinceCode ?? "914";

var readerAcreditan = new TucumanAcreditanReader(acreditanFilepath);
var readerCoeficientes = new TucumanCoeficientesReader(coeficientesFilepath);
var outputFile = new StreamWriter(outputFilepath, false, Encoding.UTF8);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var connectionString =
    $"Data Source={configuration["Database:Server"]};" +
    $"Initial Catalog={configuration["Database:Database"]};" +
    $"User Id={configuration["Database:User"]};" +
    $"Password={configuration["Database:Password"]};" +
    $"TrustServerCertificate=True";

// Access to the database. This is extremely slow. Like 700% slower.
var clienteRepository = new ClientesRepository(connectionString);
try
{
    clienteRepository.ObtenerClientesLocales(provinceCode);
}
catch(SqlException sqlEx)
{
    Console.WriteLine($"Ocurrió un error al conectarse a la base de datos. Error: {sqlEx.Message}");
    return;
}

Stopwatch sw = Stopwatch.StartNew();
Console.CursorVisible = false;
Console.WriteLine($"Servidor de base de datos: {configuration["Database:Server"]}");
Console.WriteLine($"Nombre de la Base de datos: {configuration["Database:Database"]}");
Console.WriteLine($"Leyendo archivo acreditan: {acreditanFilepath}");

List<AcreditanRegistry> padron = new();
try
{
    padron = readerAcreditan.GetRegistries();
}
catch (Exception ex)
{
    Console.WriteLine($"Ocurrió un error al abrir el archivo de contribuyentes. Error: {ex.Message}");
    return;
}

Console.WriteLine($"Leyendo archivo coeficientes: {coeficientesFilepath}");

List<CoeficienteRegistry> coeficientes = new();
try
{
    coeficientes = readerCoeficientes.GetRegistries();
}
catch (Exception ex)
{
    Console.WriteLine($"Ocurrió un error al abrir el archivo de coeficientes. Error: {ex.Message}");
    return;
}

Console.WriteLine("Buscando coeficientes sin registros en el padrón...");
// This uses 30% of the time
// var coeficientesSinPadron = coeficientes.Where(c => !padron.Any(p => p.Cuit == c.Cuit)).ToList();
IAsyncEnumerable<CoeficienteRegistry> coeficientesAsync = coeficientes.ToAsyncEnumerable();
var coeficientesSinPadron = await coeficientesAsync.Where(c => !padron.Any(p => p.Cuit == c.Cuit)).ToListAsync();

Console.WriteLine($"Se encontraron {coeficientesSinPadron.Count} coeficientes sin registro en el padrón");
Console.WriteLine("Procesando registros de Acreditan...");

int i = 0;
foreach (var registry in padron)
{
    Console.SetCursorPosition(0, Console.CursorTop);
    i++;
    var coeficiente = coeficientes.SingleOrDefault(c => c.Cuit == registry.Cuit);
    var config = new Configuracion();
    config.CargarDesdeFrameworks(configuration);
    var calculadora = new CalculadoraDeAlicuota(clienteRepository, config, options);
    calculadora.CargarAcreditanRegistry(registry);
    calculadora.CargarCoeficientesRegistry(coeficiente);
    var resultado = calculadora.CalcularAlicuota();

    var bsasRegistry = new PadronRegistry(registry, resultado.Alicuota, resultado.Regimen);

    outputFile.WriteLine(bsasRegistry.ToString());
    Console.Write($"Se han procesado {i} registros de {padron.Count} ({(((double)i / (double)padron.Count) * 100).ToString("N0")}%)");
}

Console.WriteLine();
Console.WriteLine("Procesando registros de Coeficientes sin registros en Acreditan...");
i = 0;
foreach (var registry in coeficientesSinPadron)
{
    Console.SetCursorPosition(0, Console.CursorTop);
    i++;
    var config = new Configuracion();
    config.CargarDesdeFrameworks(configuration);
    var calculadora = new CalculadoraDeAlicuota(clienteRepository, config, options);
    calculadora.CargarCoeficientesRegistry(registry);
    var resultado = calculadora.CalcularAlicuota();

    if (resultado != null)
    {
        var bsasRegistry = new PadronRegistry(registry, resultado.Alicuota, resultado.Regimen);
        outputFile.WriteLine(bsasRegistry.ToString());
    }

    Console.Write($"Se han procesado {i} registros de {coeficientesSinPadron.Count()} ({(((double)i / (double)coeficientesSinPadron.Count()) * 100).ToString("N0")}%)");
}


outputFile.Close();
var fInfo = new FileInfo(outputFilepath);

sw.Stop();

Console.WriteLine();
Console.WriteLine($"Archivo de salida almacenado en {outputFilepath} ({(fInfo.Length/(double)1024).ToString("0")} kiB)");
Console.WriteLine($"Procesado en {sw.Elapsed}");
Console.CursorVisible = true;