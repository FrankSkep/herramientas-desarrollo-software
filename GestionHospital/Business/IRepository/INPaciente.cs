namespace Business.IRepository;

using Entity.Models;

public interface INPaciente
{
    // CRUD
    Task<Paciente> InsertarPacienteAsync(Paciente paciente);
    Task<bool> BorrarPacienteAsync(int id);
    Task<Paciente> ModificarPacienteAsync(Paciente paciente);
    Task<IEnumerable<Paciente>> ObtenerTodosLosPacientesAsync();
    Task<Paciente?> ObtenerPacientePorIdAsync(int id);

    // Consultas específicas
    Task<IEnumerable<Paciente>> BuscarPacientesPorNombreAsync(string termino);
    Task<IEnumerable<Paciente>> ObtenerPacientesPorRangoFechasAsync(DateOnly fechaInicio, DateOnly fechaFin);
    Task<Paciente?> ObtenerPacienteConHistorialCompletoAsync(int id);

    // Validaciones
    Task<bool> ValidarTelefonoUnicoAsync(string telefono, int? idPacienteExcluir = null);

    // Estadísticas
    Task<int> ContarTotalPacientesAsync();
}