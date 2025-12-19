namespace BAS.Padrones.Tucuman
{
    public class CalculadoraDeAlicuota
    {
        IClientesRepository _clientesRepository;
        AcreditanRegistry? _acreditanRegistry;
        CoeficienteRegistry? _coeficienteRegistry;
        Configuracion _configuracion;
        double _coeficienteCorreccion;
        double _alicuotaEspecial;
        bool _coeficientesParaExistentes;
        bool _coeficientesParaInexistentes;

        public CalculadoraDeAlicuota(IClientesRepository clientesRepository, Configuracion configuracion)
        {
            _clientesRepository = clientesRepository;
            _configuracion = configuracion;
            _alicuotaEspecial = _configuracion.AlicuotaEspecial;
            _coeficienteCorreccion = _configuracion.CoeficienteCorreccion;
            _coeficientesParaExistentes = _configuracion.CoeficientesParaExistentes;
            _coeficientesParaInexistentes = _configuracion.CoeficientesParaInexistentes;
        }

        public void CargarAcreditanRegistry(AcreditanRegistry? acreditanRegistry)
        {
            _acreditanRegistry = acreditanRegistry;
        }

        public void CargarCoeficientesRegistry(CoeficienteRegistry? coeficienteRegistry)
        {
            _coeficienteRegistry = coeficienteRegistry;
        }

        public double? CalcularAlicuota()
        {
            if (_acreditanRegistry is null && !_coeficientesParaInexistentes)
            {
                return null;
            }

            if (_acreditanRegistry is null && _coeficientesParaInexistentes)
            {
                if (_coeficienteRegistry!.Coeficiente == 0)
                {
                    return _alicuotaEspecial;
                }

                return _coeficienteRegistry.Porcentaje!.Value * _coeficienteCorreccion;
            }

            if (_acreditanRegistry!.Excento)
            {
                return 0.0;
            }

            if (_acreditanRegistry.Convenio == Convenio.Local)
            {
                return _acreditanRegistry.Porcentaje!.Value;
            }

            if (_acreditanRegistry.Convenio == Convenio.Multilateral)
            {
                if (!_coeficientesParaExistentes)
                {
                    return _acreditanRegistry.Porcentaje!.Value * 0.5;
                }

                if (_coeficienteRegistry == null || _clientesRepository.EsLocalUsarCache(_acreditanRegistry.Cuit!))
                {
                    return _acreditanRegistry.Porcentaje!.Value * 0.5;
                }

                if (_coeficienteRegistry.Coeficiente > 0)
                {
                    return _coeficienteRegistry.Porcentaje!.Value * _coeficienteCorreccion;
                }

                return _alicuotaEspecial;

            }
            return null;
        }
    }
}
