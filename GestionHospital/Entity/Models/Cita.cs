namespace Entity.Models;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Citas")]
public class Cita
{
    [Key]
    public int IdCita { get; set; }

    [Required]
    public int IdPaciente { get; set; }

    [Required]
    public int IdDoctor { get; set; }

    [Required]
    public DateTime FechaCita { get; set; }

    public string? Motivo { get; set; }

    [MaxLength(20)]
    public string? Estado { get; set; }

    [Required]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    // Navegación
    [ForeignKey("IdPaciente")]
    public virtual Paciente Paciente { get; set; } = null!;

    [ForeignKey("IdDoctor")]
    public virtual Doctor Doctor { get; set; } = null!;
}