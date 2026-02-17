using System.ComponentModel.DataAnnotations;
using Entity.Shared;

namespace Entity.DTO;

public class DoctorDTO
{
    public int IdDoctor { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un departamento válido")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un departamento válido")]
    public int IdDepartamento { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Especialidad { get; set; }

    [MaxLength(15)]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido")]
    [MaxLength(100)]
    public string? Email { get; set; }

    [DataType(DataType.Date)]
    [FechaPasadaOActual(ErrorMessage = "La fecha de contratación no puede ser futura")]
    public DateTime? FeechaContratacion { get; set; }

    public bool Activo { get; set; } = true;

    // Propiedad de navegación (solo lectura)
    public string? NombreDepartamento { get; set; }
}