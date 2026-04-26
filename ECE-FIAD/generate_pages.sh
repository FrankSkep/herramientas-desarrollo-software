#!/bin/bash
DIR="Presentacion/Components/Pages/ECE/Especialidades"
cat << 'FILE' > $DIR/ListaEspecialidades.razor.cs
using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Especialidades;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;
using CurrieTechnologies.Razor.SweetAlert2;
namespace Presentacion.Components.Pages.ECE.Especialidades
{
    public partial class ListaEspecialidades : ComponentBase
    {
        [Inject] private IEspecialidadService serviciosEspecialidad { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private SweetAlertService Swal { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;
        protected List<EspecialidadDTO>? especialidades;
        protected override async Task OnInitializedAsync()
        {
            await CargarEspecialidades();
        }
        protected async Task CargarEspecialidades()
        {
            var resultado = await serviciosEspecialidad.ObtenerTodosAsync();
            if (resultado.Exitoso && resultado.Datos != null)
            {
                especialidades = resultado.Datos.ToList();
            }
            else
            {
                await Toastr.MsgError("Error al cargar especialidades: " + resultado.Mensaje);
                especialidades = new List<EspecialidadDTO>();
            }
        }
        protected void NavegarACrear() => Navigation.NavigateTo("/crear-especialidad");
        protected void VerDetalles(int id) => Navigation.NavigateTo($"/detalles-especialidad/{id}");
        protected void Editar(int id) => Navigation.NavigateTo($"/editar-especialidad/{id}");
        protected async Task ConfirmarEliminar(int id, string nombre)
        {
            var result = await Swal.FireAsync(new SweetAlertOptions
            {
                Title = "Estás seguro?",
                Text = $"La especialidad '{nombre}' será eliminada",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Sí, eliminar",
                CancelButtonText = "Cancelar"
            });
            if (!string.IsNullOrEmpty(result.Value))
            {
                var resultado = await serviciosEspecialidad.EliminarAsync(id);
                if (resultado.Exitoso)
                {
                    await Toastr.MsgExito($"Especialidad eliminada correctamente.");
                    await CargarEspecialidades();
                    StateHasChanged();
                }
                else
                {
                    await Swal.FireAsync("Advertencia", resultado.Mensaje, SweetAlertIcon.Warning);
                }
            }
        }
    }
}
FILE
cat << 'FILE' > $DIR/CrearEspecialidad.razor
@page "/crear-especialidad"
@using Aplicacion.DTOs.Especialidades
@using Microsoft.AspNetCore.Components.Forms
<h3>Nueva Especialidad</h3>
<EditForm Model="crearEspecialidadDTO" OnValidSubmit="GrabarEspecialidad">
    <FluentValidationValidator />
    <ValidationSummary />
    <div class="mb-3">
        <label>Nombre</label>
        <InputText class="form-control" @bind-Value="crearEspecialidadDTO.Nombre" />
        <ValidationMessage For="@(() => crearEspecialidadDTO.Nombre)" />
    </div>
    <div class="mb-3">
        <label>Descripción</label>
        <InputTextArea class="form-control" @bind-Value="crearEspecialidadDTO.Descripcion" rows="4" />
        <ValidationMessage For="@(() => crearEspecialidadDTO.Descripcion)" />
    </div>
    <button type="submit" class="btn btn-primary">Guardar Especialidad</button>
    <button type="button" class="btn btn-secondary" @onclick="Cancelar">Cancelar</button>
</EditForm>
FILE
cat << 'FILE' > $DIR/CrearEspecialidad.razor.cs
using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Especialidades;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;
namespace Presentacion.Components.Pages.ECE.Especialidades
{
    public partial class CrearEspecialidad : ComponentBase
    {
        [Inject] private IEspecialidadService serviciosEspecialidad { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;
        protected CrearEspecialidadDTO crearEspecialidadDTO { get; set; } = new();
        protected async Task GrabarEspecialidad()
        {
            var resultado = await serviciosEspecialidad.CrearAsync(crearEspecialidadDTO);
            if (resultado.Exitoso)
            {
                await Toastr.MsgExito("Especialidad creada con éxito.");
                Navigation.NavigateTo("/especialidades");
            }
            else
            {
                await Toastr.MsgError("Error al crear la especialidad: " + resultado.Mensaje);
            }
        }
        protected void Cancelar()
        {
            Navigation.NavigateTo("/especialidades");
        }
    }
}
FILE
cat << 'FILE' > $DIR/DetalleEspecialidad.razor
@page "/detalles-especialidad/{id:int}"
@if (especialidad == null)
{
    <p><em>Cargando...</em></p>
}
else
{
    <div class="card mb-4 shadow" style="width: 28rem;">
        <div class="card-header bg-primary text-white text-center">
            <h5>@especialidad.Nombre</h5>
        </div>
        <div class="card-body">
            <p><strong>Descripción:</strong> @(string.IsNullOrEmpty(especialidad.Descripcion) ? "Sin descripción" : especialidad.Descripcion)</p>
            <p><strong>Estado:</strong> @(especialidad.Activo ? "Activo" : "Inactivo")</p>
        </div>
        <div class="card-footer d-flex justify-content-between">
            <button class="btn btn-secondary" @onclick="Volver">Volver al listado</button>
            <button class="btn btn-warning" @onclick="Editar">Editar especialidad</button>
        </div>
    </div>
}
FILE
cat << 'FILE' > $DIR/DetalleEspecialidad.razor.cs
using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Especialidades;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;
namespace Presentacion.Components.Pages.ECE.Especialidades
{
    public partial class DetalleEspecialidad : ComponentBase
    {
        [Parameter] public int id { get; set; }
        [Inject] private IEspecialidadService serviciosEspecialidad { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;
        protected EspecialidadDTO? especialidad;
        protected override async Task OnInitializedAsync()
        {
            await CargarEspecialidad();
        }
        protected async Task CargarEspecialidad()
        {
            var resultado = await serviciosEspecialidad.ObtenerPorIdAsync(id);
            if (resultado.Exitoso && resultado.Datos != null)
            {
                especialidad = resultado.Datos;
            }
            else
            {
                await Toastr.MsgError("Especialidad no encontrada.");
                Navigation.NavigateTo("/especialidades");
            }
        }
        protected void Volver() => Navigation.NavigateTo("/especialidades");
        protected void Editar() => Navigation.NavigateTo($"/editar-especialidad/{id}");
    }
}
FILE
cat << 'FILE' > $DIR/EditarEspecialidad.razor
@page "/editar-especialidad/{id:int}"
@using Microsoft.AspNetCore.Components.Forms
<h3>Editar Especialidad (ID: @id)</h3>
@if (especialidadEditar == null)
{
    <p><em>Cargando...</em></p>
}
else
{
    <EditForm Model="especialidadEditar" OnValidSubmit="GrabarEspecialidad">
        <FluentValidationValidator />
        <ValidationSummary />
        <div class="mb-3">
            <label>Nombre</label>
            <InputText class="form-control" @bind-Value="especialidadEditar.Nombre" />
            <ValidationMessage For="@(() => especialidadEditar.Nombre)" />
        </div>
        <div class="mb-3">
            <label>Descripción</label>
            <InputTextArea class="form-control" @bind-Value="especialidadEditar.Descripcion" rows="4" />
            <ValidationMessage For="@(() => especialidadEditar.Descripcion)" />
        </div>
        <button type="submit" class="btn btn-primary">Guardar Especialidad</button>
        <button type="button" class="btn btn-secondary" @onclick="Cancelar">Cancelar</button>
    </EditForm>
}
FILE
cat << 'FILE' > $DIR/EditarEspecialidad.razor.cs
using Microsoft.AspNetCore.Components;
using Aplicacion.DTOs.Especialidades;
using Aplicacion.Servicios.Interfaces;
using Presentacion.Servicios;
namespace Presentacion.Components.Pages.ECE.Especialidades
{
    public partial class EditarEspecialidad : ComponentBase
    {
        [Parameter] public int id { get; set; }
        [Inject] private IEspecialidadService serviciosEspecialidad { get; set; } = null!;
        [Inject] private IToastrService Toastr { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;
        protected EspecialidadDTO? especialidadOriginal;
        protected ActualizarEspecialidadDTO? especialidadEditar;
        protected override async Task OnInitializedAsync()
        {
            var resultado = await serviciosEspecialidad.ObtenerPorIdAsync(id);
            if (resultado.Exitoso && resultado.Datos != null)
            {
                especialidadOriginal = resultado.Datos;
                especialidadEditar = new ActualizarEspecialidadDTO
                {
                    Id = especialidadOriginal.Id,
                    Nombre = especialidadOriginal.Nombre,
                    Descripcion = especialidadOriginal.Descripcion
                };
            }
            else
            {
                await Toastr.MsgError("Especialidad no encontrada.");
                Navigation.NavigateTo("/especialidades");
            }
        }
        protected async Task GrabarEspecialidad()
        {
            if (especialidadEditar != null)
            {
                var resultado = await serviciosEspecialidad.ActualizarAsync(id, especialidadEditar);
                if (resultado.Exitoso)
                {
                    await Toastr.MsgExito("Especialidad actualizada con éxito.");
                    Navigation.NavigateTo("/especialidades");
                }
                else
                {
                    await Toastr.MsgError("Error al actualizar: " + resultado.Mensaje);
                }
            }
        }
        protected void Cancelar()
        {
            Navigation.NavigateTo("/especialidades");
        }
    }
}
FILE
chmod +x generate_pages.sh
./generate_pages.sh
