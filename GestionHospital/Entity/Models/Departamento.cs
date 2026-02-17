using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

[Table("Departamentos")]
public class Departamento
{
    [Key]
    public int IdDepartamento { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public DateTime? FechaCreacion { get; set; }

    // Navegación
    public virtual ICollection<Doctor> Doctores { get; set; } = new List<Doctor>();
}