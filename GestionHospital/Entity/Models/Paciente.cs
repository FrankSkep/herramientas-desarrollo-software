using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Entidad Paciente
/// </summary>
[Table("Pacientes")]
public class Paciente
{
    [Key]
    public int IdPaciente { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public DateTime FechaNacimiento { get; set; }

    [Required]
    [MaxLength(10)]
    public string Genero { get; set; } = string.Empty;

    [MaxLength(15)]
    public string? Telefono { get; set; }

    [MaxLength(255)]
    public string? Direccion { get; set; }

    [MaxLength(5)]
    public string? TipoSangre { get; set; }

    [Required]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    // Navegación
    public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();
    public virtual ICollection<HistorialMedico> HistorialesMedicos { get; set; } = new List<HistorialMedico>();
}