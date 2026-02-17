using System.ComponentModel.DataAnnotations;

namespace GestionUsuarios.Models;

/// <summary>
/// Clase UsuarioDTO con validaciones mediante DataAnnotations
/// </summary>
public class UsuarioDTO
{
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
    [Display(Name = "Nombre de Usuario")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La edad es obligatoria")]
    [Range(18, 120, ErrorMessage = "La edad debe estar entre 18 y 120 años")]
    [Display(Name = "Edad")]
    public int Edad { get; set; }
}