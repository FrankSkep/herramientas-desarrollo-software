using System.ComponentModel.DataAnnotations;
using Entity.Shared;

namespace Entity.DTO;

public class HistorialMedicoDTO
{
    public int IdHistorial { get; set; }

    [Required(ErrorMessage = "El paciente es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un paciente válido")]
    public int IdPaciente { get; set; }

    [Required(ErrorMessage = "El doctor es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un doctor válido")]
    public int IdDoctor { get; set; }

    [Required(ErrorMessage = "La fecha de consulta es requerida")]
    [DataType(DataType.Date)]
    [FechaPasadaOActual(ErrorMessage = "La fecha de consulta no puede ser futura")]
    public DateTime FechaConsulta { get; set; }

    [Required(ErrorMessage = "El diagnóstico es requerido")]
    public string Diagnostico { get; set; } = string.Empty;

    public string? Tratamiento { get; set; }

    public string? Medicamentos { get; set; }

    public string? Notas { get; set; }

    // Propiedades de navegación (solo lectura)
    public string? NombrePaciente { get; set; }
    public string? NombreDoctor { get; set; }
}