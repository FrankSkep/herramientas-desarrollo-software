namespace Data.IRepository;

using Entity.Models;

public interface IPaciente
{
    // Operaciones básicas CRUD
    Task<Paciente> InsertarPacienteAsync(Paciente paciente);
    Task<bool> BorrarPacienteAsync(int id);
    Task<Paciente> ModificarPacienteAsync(Paciente paciente);
    Task<IEnumerable<Paciente>> ObtenerTodosLosPacientesAsync();
    Task<Paciente> ObtenerPacientePorIdAsync(int id);
    
    // =========================
// CONSULTAS ESPECIFICAS
// =========================

// Buscar por nombre (termino)
    Task<IEnumerable<Paciente>> BuscarPacientePorNombreAsync(string termino);

// Buscar por rango de fecha de ingreso
    Task<IEnumerable<Paciente>> ObtenerPacientePorFechaIngresoAsync(
        DateOnly fechaInicio,
        DateOnly fechaFin);

// Validar email existente (para evitar duplicados)
    Task<bool> ExisteEmailDePacienteAsync(
        string email,
        int? idExcluir = null);

// Validar telefono existente
    Task<bool> ExisteTelefonoDePacienteAsync(
        string telefono,
        int? idExcluir = null);

// Obtener paciente incluyendo relaciones (Appointments + MedicalFile)
    Task<Paciente?> ObtenerPacienteConRelacionesAsync(int id);

// Contar pacientes registrados
    Task<int> ContarPacientesAsync();
}