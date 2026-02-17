namespace Entity.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Entidad Doctor
/// </summary>
[Table("Doctores")]
public class Doctor
{
    [Key]
    public int IdDoctor { get; set; }

    [Required]
    public int IdDepartamento { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Especialidad { get; set; }

    [MaxLength(15)]
    public string? Telefono { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    public DateTime? FeechaContratacion { get; set; }

    [Required]
    public bool Activo { get; set; } = true;

    // Navegación
    [ForeignKey("IdDepartamento")]
    public virtual Departamento Departamento { get; set; } = null!;
    public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();
    public virtual ICollection<HistorialMedico> HistorialesMedicos { get; set; } = new List<HistorialMedico>();
}
