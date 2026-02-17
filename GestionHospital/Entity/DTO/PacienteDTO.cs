using System.ComponentModel.DataAnnotations;
using Entity.Shared;

namespace Entity.DTO;

public class PacienteDTO
{
    public int IdPaciente { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [MinLength(3, ErrorMessage = "El nombre debe tener al menos 3 caracteres")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
    [DataType(DataType.Date)]
    [FechaPasada(ErrorMessage = "La fecha de nacimiento no puede ser futura")]
    public DateTime FechaNacimiento { get; set; }

    [Required(ErrorMessage = "El género es requerido")]
    [RegularExpression("^(Masculino|Femenino|Otro)$", ErrorMessage = "Género debe ser: Masculino, Femenino u Otro")]
    public string Genero { get; set; } = string.Empty;

    [RegularExpression(@"^\d{3}-\d{4}-\d{4}$", ErrorMessage = "El formato del teléfono debe ser XXX-XXXX-XXXX")]
    public string? Telefono { get; set; }

    [MaxLength(255)]
    public string? Direccion { get; set; }

    [RegularExpression("^(A\\+|A-|B\\+|B-|O\\+|O-|AB\\+|AB-)$", ErrorMessage = "Tipo de sangre debe ser: A+, A-, B+, B-, O+, O-, AB+, AB-")]
    public string? TipoSangre { get; set; }

    public DateTime FechaRegistro { get; set; }
}