using System.ComponentModel.DataAnnotations;

namespace Entity.DTO;

public class DepartamentoDTO
{
    public int IdDepartamento { get; set; }

    [Required(ErrorMessage = "El nombre del departamento es requerido")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public DateTime? FechaCreacion { get; set; }
}