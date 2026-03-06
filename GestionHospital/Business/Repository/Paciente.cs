namespace Business.Repository;

using Business.IRepository;
using Data.IRepository;
using Microsoft.Extensions.Logging;
using PacienteModel = Entity.Models.Paciente;

public class PacienteNegocio : INPaciente
{
    private readonly IPaciente _repositorioPacientes;
    private readonly ILogger<PacienteNegocio> _logger;

    public PacienteNegocio(IPaciente repositorioPacientes, ILogger<PacienteNegocio> logger)
    {
        _repositorioPacientes = repositorioPacientes;
        _logger = logger;
    }

    // Crear nuevo paciente con validaciones de negocio
    public async Task<PacienteModel> InsertarPacienteAsync(PacienteModel paciente)
    {
        try
        {
            _logger.LogInformation("Iniciando proceso de creación de paciente");

            ValidarPacienteNuevo(paciente);
            AplicarReglasNegocioCreacion(paciente);

            var resultado = await _repositorioPacientes.InsertarPacienteAsync(paciente);

            _logger.LogInformation("Paciente creado exitosamente en capa negocio ID: {IdPaciente}", resultado.IdPaciente);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en capa negocio al crear paciente");
            throw;
        }
    }

    // Eliminar paciente con validaciones de negocio
    public async Task<bool> BorrarPacienteAsync(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando proceso de eliminación de paciente ID: {IdPaciente}", id);

            var paciente = await _repositorioPacientes.ObtenerPacientePorIdAsync(id);
            if (paciente == null)
            {
                _logger.LogWarning("Paciente no encontrado para eliminar ID: {IdPaciente}", id);
                return false;
            }

            var resultado = await _repositorioPacientes.BorrarPacienteAsync(id);

            _logger.LogInformation("Paciente eliminado exitosamente en capa negocio ID: {IdPaciente}", id);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en capa negocio al eliminar paciente ID: {IdPaciente}", id);
            throw;
        }
    }

    // Actualizar paciente con validaciones de negocio
    public async Task<PacienteModel> ModificarPacienteAsync(PacienteModel paciente)
    {
        try
        {
            _logger.LogInformation("Iniciando proceso de actualización de paciente ID: {IdPaciente}", paciente.IdPaciente);

            ValidarPacienteExistente(paciente);
            AplicarReglasNegocioActualizacion(paciente);

            var resultado = await _repositorioPacientes.ModificarPacienteAsync(paciente);

            _logger.LogInformation("Paciente actualizado exitosamente en capa negocio ID: {IdPaciente}", resultado.IdPaciente);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en capa negocio al actualizar paciente ID: {IdPaciente}", paciente.IdPaciente);
            throw;
        }
    }

    // Obtener todos los pacientes
    public async Task<IEnumerable<PacienteModel>> ObtenerTodosLosPacientesAsync()
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los pacientes desde capa negocio");
            return await _repositorioPacientes.ObtenerTodosLosPacientesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en capa negocio al obtener todos los pacientes");
            throw;
        }
    }

    // Obtener paciente por ID
    public async Task<PacienteModel?> ObtenerPacientePorIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo paciente por ID desde capa negocio: {IdPaciente}", id);
            return await _repositorioPacientes.ObtenerPacientePorIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en capa negocio al obtener paciente ID: {IdPaciente}", id);
            throw;
        }
    }

    // Buscar pacientes por nombre (mínimo 2 caracteres)
    public async Task<IEnumerable<PacienteModel>> BuscarPacientesPorNombreAsync(string termino)
    {
        try
        {
            _logger.LogInformation("Buscando pacientes con término: {Termino}", termino);

            if (string.IsNullOrWhiteSpace(termino) || termino.Length < 2)
                return new List<PacienteModel>();

            return await _repositorioPacientes.BuscarPacientePorNombreAsync(termino);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en capa negocio al buscar pacientes con término: {Termino}", termino);
            throw;
        }
    }

    // Obtener pacientes por rango de fechas de ingreso
    public async Task<IEnumerable<PacienteModel>> ObtenerPacientesPorRangoFechasAsync(DateOnly fechaInicio, DateOnly fechaFin)
    {
        try
        {
            _logger.LogInformation("Obteniendo pacientes por rango de fechas: {FechaInicio} - {FechaFin}", fechaInicio, fechaFin);

            if (fechaInicio > fechaFin)
                throw new ArgumentException("La fecha de inicio no puede ser mayor que la fecha fin.");

            return await _repositorioPacientes.ObtenerPacientePorFechaIngresoAsync(fechaInicio, fechaFin);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en capa negocio al obtener pacientes por rango de fechas");
            throw;
        }
    }

    // Obtener paciente con historial completo (citas e historiales médicos)
    public async Task<PacienteModel?> ObtenerPacienteConHistorialCompletoAsync(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo paciente con historial completo ID: {IdPaciente}", id);
            return await _repositorioPacientes.ObtenerPacienteConRelacionesAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en capa negocio al obtener paciente con historial ID: {IdPaciente}", id);
            throw;
        }
    }

    // Validar teléfono único
    public async Task<bool> ValidarTelefonoUnicoAsync(string telefono, int? idPacienteExcluir = null)
    {
        try
        {
            return await _repositorioPacientes.ExisteTelefonoDePacienteAsync(telefono, idPacienteExcluir);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en capa negocio al validar teléfono único: {Telefono}", telefono);
            throw;
        }
    }

    // Contar total de pacientes
    public async Task<int> ContarTotalPacientesAsync()
    {
        try
        {
            return await _repositorioPacientes.ContarPacientesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en capa negocio al contar pacientes");
            throw;
        }
    }

    #region Métodos Privados de Validación y Reglas de Negocio

    private void ValidarPacienteNuevo(PacienteModel paciente)
    {
        if (string.IsNullOrWhiteSpace(paciente.Nombre))
            throw new ArgumentException("El nombre del paciente es obligatorio.");

        if (string.IsNullOrWhiteSpace(paciente.Genero))
            throw new ArgumentException("El género del paciente es obligatorio.");

        if (paciente.FechaNacimiento == default)
            throw new ArgumentException("La fecha de nacimiento es obligatoria.");

        // Validar que la edad sea razonable
        var edad = CalcularEdad(paciente.FechaNacimiento);
        if (edad < 0 || edad > 120)
            throw new ArgumentException("La fecha de nacimiento no es válida.");

        // Validar formato de teléfono si se proporciona
        if (!string.IsNullOrWhiteSpace(paciente.Telefono) && !EsTelefonoValido(paciente.Telefono))
            throw new ArgumentException("El formato del teléfono no es válido (mínimo 10 dígitos numéricos).");
    }

    private void ValidarPacienteExistente(PacienteModel paciente)
    {
        if (paciente.IdPaciente <= 0)
            throw new ArgumentException("ID de paciente no válido.");

        ValidarPacienteNuevo(paciente);
    }

    private void AplicarReglasNegocioCreacion(PacienteModel paciente)
    {
        // Formatear nombre (primera letra de cada palabra en mayúscula)
        paciente.Nombre = FormatearNombre(paciente.Nombre);

        // Normalizar dirección si existe
        if (!string.IsNullOrWhiteSpace(paciente.Direccion))
            paciente.Direccion = paciente.Direccion.Trim();

        // Normalizar tipo de sangre a mayúsculas
        if (!string.IsNullOrWhiteSpace(paciente.TipoSangre))
            paciente.TipoSangre = paciente.TipoSangre.Trim().ToUpper();

        // Establecer fecha de registro si no viene
        if (paciente.FechaRegistro == default)
            paciente.FechaRegistro = DateTime.Today;
    }

    private void AplicarReglasNegocioActualizacion(PacienteModel paciente)
    {
        AplicarReglasNegocioCreacion(paciente);
    }

    private bool EsTelefonoValido(string telefono)
    {
        return !string.IsNullOrWhiteSpace(telefono) &&
               telefono.All(char.IsDigit) &&
               telefono.Length >= 10;
    }

    private int CalcularEdad(DateTime fechaNacimiento)
    {
        var hoy = DateTime.Today;
        var edad = hoy.Year - fechaNacimiento.Year;
        if (fechaNacimiento.Date > hoy.AddYears(-edad))
            edad--;
        return edad;
    }

    private string FormatearNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return nombre;

        nombre = nombre.Trim().ToLower();
        var palabras = nombre.Split(' ');

        for (int i = 0; i < palabras.Length; i++)
        {
            if (palabras[i].Length > 0)
                palabras[i] = char.ToUpper(palabras[i][0]) + palabras[i].Substring(1);
        }

        return string.Join(' ', palabras);
    }

    #endregion
}