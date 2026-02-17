using System.ComponentModel.DataAnnotations;
using Entity.Shared;

namespace Entity.DTO;

public class CitaDTO
{
    public int IdCita { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un paciente existente")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un paciente existente")]
    public int IdPaciente { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un doctor existente")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un doctor existente")]
    public int IdDoctor { get; set; }

    [Required(ErrorMessage = "La fecha de la cita es requerida")]
    [FechaFutura(ErrorMessage = "La fecha de la cita debe ser una fecha futura")]
    public DateTime FechaCita { get; set; }

    public string? Motivo { get; set; }

    [RegularExpression("^(Programada|Completada|Cancelada)$", ErrorMessage = "Estado debe ser: Programada, Completada o Cancelada")]
    public string? Estado { get; set; } = "Programada";

    public DateTime FechaRegistro { get; set; }

    // Propiedades de navegación (solo lectura)
    public string? NombrePaciente { get; set; }
    public string? NombreDoctor { get; set; }
}