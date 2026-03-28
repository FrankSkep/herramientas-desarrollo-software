using Dominio.Entidades.Base;
using Dominio.Entidades.Citas;
using Dominio.Entidades.HistoriasClinicas;
using Dominio.Enumeraciones;

namespace Dominio.Entidades.Pacientes;

public class Paciente : EntidadBase
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NombreCompleto => $"{Nombres} {Apellidos}";
    public string NumeroDeExpediente { get; set; } = string.Empty;
    public DateTime FechaDeNacimiento { get; set; }
    public Genero Genero { get; set; }
    public TipoDeDocumento TipoDeDocumento { get; set; }
    public string NumeroDeDocumento { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Alergias { get; set; } = string.Empty;
    public string AntecedentesMedicos { get; set; } = string.Empty;
    public GrupoSanguineo GrupoSanguineo { get; set; }
    public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();
    public virtual HistoriaClinica? HistoriaClinica { get; set; }

    public void MarcarComoEliminado()
    {
        Eliminado = true;
        FechaDeEliminacion = DateTime.UtcNow;
        Activo = false;
    }

    public void Reactivar()
    {
        Eliminado = false;
        FechaDeEliminacion = null;
        Activo = true;
        FechaDeModificacion = DateTime.UtcNow;
    }
}