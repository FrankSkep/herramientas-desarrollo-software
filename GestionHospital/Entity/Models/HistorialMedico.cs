namespace Entity.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Historial_Medico")]
public class HistorialMedico
{
    [Key]
    public int IdHistorial { get; set; }

    [Required]
    public int IdPaciente { get; set; }

    [Required]
    public int IdDoctor { get; set; }

    [Required]
    public DateTime FechaConsulta { get; set; }

    public string? Diagnostico { get; set; }

    public string? Tratamiento { get; set; }

    public string? Medicamentos { get; set; }

    public string? Notas { get; set; }

    // Navegación
    [ForeignKey("IdPaciente")]
    public virtual Paciente Paciente { get; set; } = null!;

    [ForeignKey("IdDoctor")]
    public virtual Doctor Doctor { get; set; } = null!;
}