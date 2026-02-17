namespace GestionUsuarios.Models;

/// <summary>
/// Clase básica de Usuario sin validaciones
/// </summary>
public class Usuario
{
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Edad { get; set; }
}