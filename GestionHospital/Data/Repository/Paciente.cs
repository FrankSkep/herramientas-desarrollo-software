namespace Data.Repository;

using Microsoft.Extensions.Logging;
using Data.IRepository;
using Microsoft.EntityFrameworkCore;
using Entity.Models;

public class Paciente : IPaciente
{
    private readonly DBContextHM _context;
    private readonly ILogger<Paciente> _logger;

    public Paciente(DBContextHM context, ILogger<Paciente> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Entity.Models.Paciente> InsertarPacienteAsync(Entity.Models.Paciente paciente)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // validar que no exista email duplicado
            if (!string.IsNullOrEmpty(paciente.Telefono) && await ExisteTelefonoDePacienteAsync(paciente.Telefono))
            {
                throw new InvalidOperationException("El teléfono no puede estar vacío.");
            }

            if (paciente.FechaRegistro == default)
            {
                paciente.FechaRegistro = DateTime.Today;
            }

            await _context.Pacientes.AddAsync(paciente);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Paciente insertado con ID {IdPaciente}", paciente.IdPaciente);
            return paciente;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError("Error al insertar paciente: {Message}", ex.Message);
            return null;
        }
    }

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
            _logger.LogInformation("Paciente eliminado con ID {IdPaciente}", id);
            return true;
        } catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error al eliminar paciente con ID {IdPaciente}", id);
            return false;
        }
    }

    public async Task<Entity.Models.Paciente> ModificarPacienteAsync(Entity.Models.Paciente paciente)
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var pacienteExistente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.IdPaciente == paciente.IdPaciente);
            if (pacienteExistente is null)
            {
                throw new KeyNotFoundException("Paciente no encontrado");
            }

            _context.Entry(pacienteExistente).CurrentValues.SetValues(paciente);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            _logger.LogInformation("Paciente modificado con ID {IdPaciente}", paciente.IdPaciente);
            return pacienteExistente;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError("Error al modificar paciente con ID {IdPaciente}: {Message}", paciente.IdPaciente, ex.Message);
            throw;
        }
    }

    public Task<IEnumerable<Entity.Models.Paciente>> ObtenerTodosLosPacientesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Entity.Models.Paciente> ObtenerPacientePorIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Entity.Models.Paciente>> BuscarPacientePorNombreAsync(string termino)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Entity.Models.Paciente>> ObtenerPacientePorFechaIngresoAsync(DateOnly fechaInicio, DateOnly fechaFin)
    {
        try
        {
            return await _context.Pacientes
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los pacientes");
            throw;
        }
    }

    public Task<bool> ExisteEmailDePacienteAsync(string email, int? idExcluir = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExisteTelefonoDePacienteAsync(string telefono, int? idExcluir = null)
    {
        throw new NotImplementedException();
    }

    public Task<Entity.Models.Paciente?> ObtenerPacienteConRelacionesAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<int> ContarPacientesAsync()
    {
        throw new NotImplementedException();
    }
}