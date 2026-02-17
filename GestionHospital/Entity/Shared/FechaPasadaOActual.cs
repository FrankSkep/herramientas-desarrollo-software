using System.ComponentModel.DataAnnotations;

namespace Entity.Shared;

public class FechaPasadaOActualAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null) return true;

        if (value is DateTime fecha)
        {
            return fecha.Date <= DateTime.Now.Date;
        }

        return false;
    }
}