namespace Aplicacion.Abstracciones;

public interface IUnitOfWork : IDisposable
{
    IPacienteRepositorio Pacientes { get; }
    IEspecialidadRepositorio Especialidades { get; }
    Task<int> GuardarCambiosAsync();
}