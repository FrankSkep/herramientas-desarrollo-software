#!/bin/bash
mkdir -p Aplicacion/DTOs/Especialidades
mkdir -p Aplicacion/Servicios/Interfaces
mkdir -p Aplicacion/Servicios/Implementaciones
mkdir -p Aplicacion/Validaciones/Especialidades
mkdir -p Presentacion/Components/Pages/ECE/Especialidades
cat << 'CS' > Aplicacion/DTOs/Especialidades/EspecialidadDTO.cs
namespace Aplicacion.DTOs.Especialidades;
public class EspecialidadDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public int CantidadMedicos { get; set; }
}
CS
cat << 'CS' > Aplicacion/DTOs/Especialidades/CrearEspecialidadDTO.cs
namespace Aplicacion.DTOs.Especialidades;
public class CrearEspecialidadDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
CS
cat << 'CS' > Aplicacion/DTOs/Especialidades/ActualizarEspecialidadDTO.cs
namespace Aplicacion.DTOs.Especialidades;
public class ActualizarEspecialidadDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}
CS
cat << 'CS' > Aplicacion/Validaciones/Especialidades/CrearEspecialidadValidator.cs
using FluentValidation;
using Aplicacion.DTOs.Especialidades;
namespace Aplicacion.Validaciones.Especialidades;
public class CrearEspecialidadValidator : AbstractValidator<CrearEspecialidadDTO>
{
    public CrearEspecialidadValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre es requerido").MaximumLength(100).WithMessage("Máximo 100 caracteres");
        RuleFor(x => x.Descripcion).MaximumLength(500).WithMessage("Máximo 500 caracteres");
    }
}
CS
cat << 'CS' > Aplicacion/Validaciones/Especialidades/ActualizarEspecialidadValidator.cs
using FluentValidation;
using Aplicacion.DTOs.Especialidades;
namespace Aplicacion.Validaciones.Especialidades;
public class ActualizarEspecialidadValidator : AbstractValidator<ActualizarEspecialidadDTO>
{
    public ActualizarEspecialidadValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre es requerido").MaximumLength(100).WithMessage("Máximo 100 caracteres");
        RuleFor(x => x.Descripcion).MaximumLength(500).WithMessage("Máximo 500 caracteres");
    }
}
CS
cat << 'CS' > Aplicacion/Servicios/Interfaces/IEspecialidadService.cs
using Aplicacion.DTOs.Especialidades;
using Aplicacion.Helpers;
namespace Aplicacion.Servicios.Interfaces;
public interface IEspecialidadService
{
    Task<ResultadoAccion<IEnumerable<EspecialidadDTO>>> ObtenerTodosAsync();
    Task<ResultadoAccion<EspecialidadDTO>> ObtenerPorIdAsync(int id);
    Task<ResultadoAccion<EspecialidadDTO>> CrearAsync(CrearEspecialidadDTO dto);
    Task<ResultadoAccion<EspecialidadDTO>> ActualizarAsync(int id, ActualizarEspecialidadDTO dto);
    Task<ResultadoAccion<bool>> EliminarAsync(int id);
}
CS
