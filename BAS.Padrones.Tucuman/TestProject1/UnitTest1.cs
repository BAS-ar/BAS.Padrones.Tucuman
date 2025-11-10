using BAS.Padrones.Tucuman;

namespace TestProject1
{
    public class Tests
    {
        Configuracion configuracion;
        Parametros options = new Parametros();
        AcreditanRegistry? acreditanRegistry;
        CoeficienteRegistry? coeficienteRegistry;
        CalculadoraDeAlicuota calculadoraDeAlicuota;
        IClientesRepository clientesRepository;

        [SetUp]
        public void Setup()
        {
            configuracion = new Configuracion();
            configuracion.AlicuotaEspecial = 0.17;
            configuracion.RazonCoeficiente = 0.5;
            configuracion.CoeficientesParaExistentes = false;
            configuracion.CoeficientesParaInexistentes = false;

            options = new Parametros();
            options.ProvinceCode = "914";

            acreditanRegistry = new AcreditanRegistry();
            acreditanRegistry.Cuit = "20364986352";
            acreditanRegistry.Excento = false;
            acreditanRegistry.Porcentaje = 3.5;
            acreditanRegistry.Convenio = Convenio.Local;
            acreditanRegistry.Denominacion = "izzitech s.a.";
            acreditanRegistry.FechaDesde = new DateTime(2025, 10, 01);
            acreditanRegistry.FechaHasta = new DateTime(2025, 10, 31);

            coeficienteRegistry = new CoeficienteRegistry();
            coeficienteRegistry.Fecha = new DateTime(2025, 10, 31);
            coeficienteRegistry.Coeficiente = 1.0;
            coeficienteRegistry.Porcentaje = 3.5;
            coeficienteRegistry.Cuit = "20364986352";
            coeficienteRegistry.Denominacion = "izzitech s.a.";
            coeficienteRegistry.Excento = false;

            clientesRepository = new ClientesRepositoryMock();
            clientesRepository.ObtenerClientesLocales("914");
        }

        [Test]
        public void CorrectStringGeneration()
        {
            var padron = new PadronRegistry(acreditanRegistry, 3.5);

            var result = padron.ToString();
            Assert.That(result, Is.EqualTo("P;01102025;01102025;31102025;20364986352;D;;N;3.50;;"));
        }

        [Test]
        public void ExcentoShouldReturnZeroAliquot()
        {
            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            acreditanRegistry.Excento = true;
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            calculadoraDeAlicuota.CargarCoeficientesRegistry(coeficienteRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(0));
        }

        [Test]
        public void ExistentesFalseInexistentesFalseLocalAlicuotaCompleta()
        {
            configuracion.CoeficientesParaExistentes = false;
            configuracion.CoeficientesParaInexistentes = false;

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(3.5));
        }

        [Test]
        public void ExistentesFalseInexistentesFalseMultilateralAlicuotaPor05()
        {
            configuracion.CoeficientesParaExistentes = false;
            configuracion.CoeficientesParaInexistentes = false;

            acreditanRegistry.Cuit = "2033344455"; // Cuit no local
            acreditanRegistry.Convenio = Convenio.Multilateral;

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(3.5 * 0.5));
        }


        [Test]
        public void ExistentesTrueInexistentesFalseLocalAlicuotaCompleta()
        {
            configuracion.CoeficientesParaExistentes = true;
            configuracion.CoeficientesParaInexistentes = false;

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(3.5));
        }

        [Test]
        public void ExistentesTrueInexistentesFalseMultilateralSedeTucumanAlicuotaPor05()
        {
            configuracion.CoeficientesParaExistentes = true;
            configuracion.CoeficientesParaInexistentes = false;

            acreditanRegistry.Convenio = Convenio.Multilateral;

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(3.5 * 0.5));
        }

        [Test]
        public void ExistentesTrueInexistentesFalseMultilateralSinSedeConvenioIgualACeroAlicuotaEspecial()
        {
            configuracion.CoeficientesParaExistentes = true;
            configuracion.CoeficientesParaInexistentes = false;
            configuracion.AlicuotaEspecial = 1.75;
            

            acreditanRegistry.Cuit = "2033344455"; // Cuit no local
            acreditanRegistry.Convenio = Convenio.Multilateral;
            coeficienteRegistry.Coeficiente = 0;

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(1.75));
        }

        [Test]
        public void ExistentesTrueInexistentesFalseMultilateralSinSedeConvenioMayorACeroAlicuotaCoeficientePorParametrizacion()
        {
            configuracion.CoeficientesParaExistentes = true;
            configuracion.CoeficientesParaInexistentes = false;

            acreditanRegistry.Cuit = "2033344455"; // Cuit no local
            acreditanRegistry.Convenio = Convenio.Multilateral;
            coeficienteRegistry.Coeficiente = 0.75;
            coeficienteRegistry.Porcentaje = 3.5;
            configuracion.RazonCoeficiente = 0.7;

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            calculadoraDeAlicuota.CargarCoeficientesRegistry(coeficienteRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(3.5 * 0.7));
        }

        [Test]
        public void ExistentesFalseInexistentesTrueLocalAlicuotaCompleta()
        {
            configuracion.CoeficientesParaExistentes = false;
            configuracion.CoeficientesParaInexistentes = true;

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(3.5));
        }

        [Test]
        public void ExistentesFalseInexistentesTrueNoEsLocalAlicuotaPor05()
        {
            configuracion.CoeficientesParaExistentes = false;
            configuracion.CoeficientesParaInexistentes = true;

            acreditanRegistry.Cuit = "2033344455"; // Cuit no local
            acreditanRegistry.Convenio = Convenio.Multilateral;

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(3.5 * 0.5));
        }

        [Test]
        public void ExistentesFalseInexistentesTrueEnCoeficientesMayorACeroAlicuotaCoeficientePorParametrizacion()
        {
            configuracion.CoeficientesParaExistentes = false;
            configuracion.CoeficientesParaInexistentes = true;

            acreditanRegistry = null;
            coeficienteRegistry.Coeficiente = 0.75;
            coeficienteRegistry.Porcentaje = 3.5;
            configuracion.RazonCoeficiente = 0.7;

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            calculadoraDeAlicuota.CargarCoeficientesRegistry(coeficienteRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(3.5 * 0.7));
        }

        [Test]
        public void ExistentesFalseInexistentesTrueEnCoeficientesIgualACeroAlicuotaEspecial()
        {
            configuracion.CoeficientesParaExistentes = false;
            configuracion.CoeficientesParaInexistentes = true;
            configuracion.AlicuotaEspecial = 1.75;

            acreditanRegistry.Cuit = "2033344455"; // Cuit no local
            acreditanRegistry.Convenio = Convenio.Multilateral;
            coeficienteRegistry.Coeficiente = 0;
            coeficienteRegistry.Porcentaje = 3.5;
            configuracion.RazonCoeficiente = 0.7;

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(1.75));
        }

        [Test]
        public void ExistentesTrueInexistentesTrueLocalAlicuotaCompleta()
        {
            configuracion.CoeficientesParaExistentes = true;
            configuracion.CoeficientesParaInexistentes = true;

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(3.5));
        }

        [Test]
        public void ExistentesTrueInexistentesTrueNoEsLocalSedeTucumanAlicuotaPor05()
        {
            configuracion.CoeficientesParaExistentes = true;
            configuracion.CoeficientesParaInexistentes = true;

            acreditanRegistry.Convenio = Convenio.Multilateral;

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(3.5 * 0.5));
        }

        [Test]
        public void ExistentesTrueInexistentesTrueNoEsLocalSinSedeTucumanNoEstaEnCoeficientes()
        {
            configuracion.CoeficientesParaExistentes = true;
            configuracion.CoeficientesParaInexistentes = true;
            configuracion.AlicuotaEspecial = 1.75;

            acreditanRegistry.Cuit = "2033344455"; // Cuit no local
            acreditanRegistry.Convenio = Convenio.Multilateral;
            coeficienteRegistry = null;

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(3.5 * 0.5));
        }

        [Test]
        public void ExistentesTrueInexistentesTrueEnCoeficientesIgualACeroAlicuotaEspecial()
        {
            configuracion.CoeficientesParaExistentes = true;
            configuracion.CoeficientesParaInexistentes = true;

            configuracion.AlicuotaEspecial = 1.75;
            coeficienteRegistry.Coeficiente = 0;
            coeficienteRegistry.Porcentaje = 3.5;
            configuracion.RazonCoeficiente = 0.7;

            acreditanRegistry = null;
            coeficienteRegistry.Cuit = "2033344455"; // Cuit no local

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            calculadoraDeAlicuota.CargarCoeficientesRegistry(coeficienteRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(1.75));
        }


        [Test]
        public void ExistentesTrueInexistentesTrueEnCoeficientesIgualACeroAlicuotaEspecialConPadron()
        {
            configuracion.CoeficientesParaExistentes = true;
            configuracion.CoeficientesParaInexistentes = true;

            configuracion.AlicuotaEspecial = 1.75;
            coeficienteRegistry.Coeficiente = 0;
            coeficienteRegistry.Porcentaje = 3.5;
            configuracion.RazonCoeficiente = 0.7;

            coeficienteRegistry.Cuit = "2033344455"; // Cuit no local
            acreditanRegistry.Convenio = Convenio.Multilateral;

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            calculadoraDeAlicuota.CargarCoeficientesRegistry(coeficienteRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(1.75));
        }

        [Test]
        public void ExistentesTrueInexistentesTrueEnCoeficientesMayorACeroAlicuotaCoeficientePorParametrizacion()
        {
            configuracion.CoeficientesParaExistentes = true;
            configuracion.CoeficientesParaInexistentes = true;

            configuracion.AlicuotaEspecial = 1.75;
            coeficienteRegistry.Coeficiente = 0.77;
            coeficienteRegistry.Porcentaje = 3.5;
            configuracion.RazonCoeficiente = 0.7;

            acreditanRegistry = null;
            coeficienteRegistry.Cuit = "2033344455"; // Cuit no local

            calculadoraDeAlicuota = new CalculadoraDeAlicuota(clientesRepository, configuracion, options);
            calculadoraDeAlicuota.CargarAcreditanRegistry(acreditanRegistry);
            calculadoraDeAlicuota.CargarCoeficientesRegistry(coeficienteRegistry);
            var result = calculadoraDeAlicuota.CalcularAlicuota();
            Assert.That(result.Alicuota, Is.EqualTo(3.5 * 0.7));
        }
    }
}
