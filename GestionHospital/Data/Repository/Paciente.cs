namespace Data.Repository;

using Microsoft.Extensions.Logging;
using Data.IRepository;
using Microsoft.EntityFrameworkCore;
using PacienteModel = Entity.Models.Paciente;

public class PacienteRepository : IPaciente
{
    private readonly DBContextHM _context;
    private readonly ILogger<PacienteRepository> _logger;

    public PacienteRepository(DBContextHM context, ILogger<PacienteRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Crear nuevo paciente
    public async Task<PacienteModel> InsertarPacienteAsync(PacienteModel paciente)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Validar que no exista teléfono duplicado
            if (!string.IsNullOrEmpty(paciente.Telefono) &&
                await ExisteTelefonoDePacienteAsync(paciente.Telefono))
            {
                throw new InvalidOperationException($"Ya existe un paciente con el teléfono {paciente.Telefono}");
            }

            // Establecer fecha de registro si no viene
            if (paciente.FechaRegistro == default)
            {
                paciente.FechaRegistro = DateTime.Today;
            }

            await _context.Pacientes.AddAsync(paciente);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Paciente creado exitosamente con ID: {IdPaciente}", paciente.IdPaciente);
            return paciente;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error al crear paciente");
            throw;
        }
    }

    // Eliminar paciente
    public async Task<bool> BorrarPacienteAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var paciente = await _context.Pacientes
                .Include(p => p.Citas)
                .Include(p => p.HistorialesMedicos)
                .FirstOrDefaultAsync(p => p.IdPaciente == id);

            if (paciente == null)
            {
                _logger.LogWarning("Intento de eliminar paciente inexistente ID: {IdPaciente}", id);
                return false;
            }

            // Verificar si tiene citas o historiales médicos
            if (paciente.Citas != null && paciente.Citas.Any())
            {
                throw new InvalidOperationException("No se puede eliminar el paciente porque tiene citas asociadas.");
            }

            if (paciente.HistorialesMedicos != null && paciente.HistorialesMedicos.Any())
            {
                throw new InvalidOperationException("No se puede eliminar el paciente porque tiene historiales médicos asociados.");
            }

            _context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Paciente eliminado exitosamente con ID: {IdPaciente}", id);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error al eliminar paciente con ID: {IdPaciente}", id);
            throw;
        }
    }

    // Actualizar paciente existente
    public async Task<PacienteModel> ModificarPacienteAsync(PacienteModel paciente)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var pacienteExistente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.IdPaciente == paciente.IdPaciente);

            if (pacienteExistente == null)
            {
                throw new KeyNotFoundException($"No se encontró paciente con ID: {paciente.IdPaciente}");
            }

            // Validar teléfono único (excluyendo el actual)
            if (!string.IsNullOrEmpty(paciente.Telefono) &&
                await ExisteTelefonoDePacienteAsync(paciente.Telefono, paciente.IdPaciente))
            {
                throw new InvalidOperationException($"Ya existe otro paciente con el teléfono {paciente.Telefono}");
            }

            // Actualizar propiedades
            _context.Entry(pacienteExistente).CurrentValues.SetValues(paciente);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Paciente actualizado exitosamente con ID: {IdPaciente}", paciente.IdPaciente);
            return pacienteExistente;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error al actualizar paciente con ID: {IdPaciente}", paciente.IdPaciente);
            throw;
        }
    }

    // Obtener todos los pacientes
    public async Task<IEnumerable<PacienteModel>> ObtenerTodosLosPacientesAsync()
    {
        try
        {
            return await _context.Pacientes
                .OrderBy(p => p.Nombre)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los pacientes");
            throw;
        }
    }

    // Obtener paciente por ID
    public async Task<PacienteModel> ObtenerPacientePorIdAsync(int id)
    {
        try
        {
            return await _context.Pacientes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdPaciente == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener paciente con ID: {IdPaciente}", id);
            throw;
        }
    }

    // Buscar pacientes por nombre (búsqueda parcial)
    public async Task<IEnumerable<PacienteModel>> BuscarPacientePorNombreAsync(string termino)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(termino))
                return await ObtenerTodosLosPacientesAsync();

            termino = termino.Trim().ToLower();

            return await _context.Pacientes
                .Where(p => p.Nombre.ToLower().Contains(termino))
                .OrderBy(p => p.Nombre)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar pacientes con término: {Termino}", termino);
            throw;
        }
    }

    // Obtener pacientes por rango de fecha de ingreso
    public async Task<IEnumerable<PacienteModel>> ObtenerPacientePorFechaIngresoAsync(DateOnly fechaInicio, DateOnly fechaFin)
    {
        try
        {
            var inicio = fechaInicio.ToDateTime(TimeOnly.MinValue);
            var fin = fechaFin.ToDateTime(TimeOnly.MaxValue);

            return await _context.Pacientes
                .Where(p => p.FechaRegistro >= inicio && p.FechaRegistro <= fin)
                .OrderBy(p => p.FechaRegistro)
                .ThenBy(p => p.Nombre)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pacientes por fecha de ingreso");
            throw;
        }
    }

    // Verificar si existe email (no disponible en el modelo actual)
    public Task<bool> ExisteEmailDePacienteAsync(string email, int? idExcluir = null)
    {
        // El modelo Paciente no cuenta con campo Email
        throw new NotSupportedException("El modelo Paciente no tiene campo Email.");
    }

    // Verificar si existe teléfono
    public async Task<bool> ExisteTelefonoDePacienteAsync(string telefono, int? idExcluir = null)
    {
        try
        {
            var query = _context.Pacientes.Where(p => p.Telefono == telefono);

            if (idExcluir.HasValue)
            {
                query = query.Where(p => p.IdPaciente != idExcluir.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de teléfono: {Telefono}", telefono);
            throw;
        }
    }

    // Obtener paciente con todas sus relaciones (citas e historiales médicos)
    public async Task<PacienteModel?> ObtenerPacienteConRelacionesAsync(int id)
    {
        try
        {
            return await _context.Pacientes
                .Include(p => p.Citas)
                .Include(p => p.HistorialesMedicos)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdPaciente == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener paciente con relaciones ID: {IdPaciente}", id);
            throw;
        }
    }

    // Contar total de pacientes
    public async Task<int> ContarPacientesAsync()
    {
        try
        {
            return await _context.Pacientes.CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al contar total de pacientes");
            throw;
        }
    }
}