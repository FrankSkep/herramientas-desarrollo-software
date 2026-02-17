using System.ComponentModel.DataAnnotations;

namespace Entity.Shared;

public class FechaFuturaAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null) return true;

        if (value is DateTime fecha)
        {
            return fecha > DateTime.Now;
        }

        return false;
    }
}