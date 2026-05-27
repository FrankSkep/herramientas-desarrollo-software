<script lang="ts">
	import { onMount } from 'svelte';
	import { api } from '$lib/api';
	import { actualizarEspecialidadSchema, crearEspecialidadSchema } from '$lib/validation/schemas';
	import { mapZodErrors } from '$lib/validation/utils';
	import type { EspecialidadDTO } from '$lib/types';
	import { defaultBooleanOptions, findLabel } from '$lib/types';

	let especialidades: EspecialidadDTO[] = [];
	let loading = false;
	let editing = false;
	let submitting = false;
	let fieldErrors: Record<string, string> = {};
	let serverMessage = '';
	let serverErrors: string[] = [];

	let form = {
		Id: 0,
		Nombre: '',
		Descripcion: '',
		Activo: true
	};

	const resetForm = () => {
		form = { Id: 0, Nombre: '', Descripcion: '', Activo: true };
		editing = false;
		fieldErrors = {};
		serverMessage = '';
		serverErrors = [];
	};

	const loadData = async () => {
		loading = true;
		const resultado = await api.getEspecialidades();
		if (resultado.Exitoso && resultado.Datos) {
			especialidades = resultado.Datos;
		} else {
			serverMessage = resultado.Mensaje;
			serverErrors = resultado.Errores ?? [];
		}
		loading = false;
	};

	const editarEspecialidad = (especialidad: EspecialidadDTO) => {
		editing = true;
		fieldErrors = {};
		serverMessage = '';
		serverErrors = [];
		form = {
			Id: especialidad.Id,
			Nombre: especialidad.Nombre,
			Descripcion: especialidad.Descripcion ?? '',
			Activo: especialidad.Activo
		};
	};

	const eliminarEspecialidad = async (id: number) => {
		if (!confirm('Deseas eliminar esta especialidad?')) return;
		const resultado = await api.eliminarEspecialidad(id);
		serverMessage = resultado.Mensaje;
		serverErrors = resultado.Errores ?? [];
		if (resultado.Exitoso) {
			await loadData();
			resetForm();
		}
	};

	const submit = async () => {
		fieldErrors = {};
		serverMessage = '';
		serverErrors = [];
		submitting = true;

		try {
			const validation = (editing ? actualizarEspecialidadSchema : crearEspecialidadSchema).safeParse({
				Id: form.Id,
				Nombre: form.Nombre,
				Descripcion: form.Descripcion,
				Activo: form.Activo
			});
			if (!validation.success) {
				fieldErrors = mapZodErrors(validation.error);
				return;
			}

			const payload = {
				Nombre: form.Nombre,
				Descripcion: form.Descripcion,
				Activo: form.Activo
			};

			const resultado = editing
				? await api.actualizarEspecialidad({ ...payload, Id: form.Id })
				: await api.crearEspecialidad(payload);

			serverMessage = resultado.Mensaje;
			serverErrors = resultado.Errores ?? [];
			if (resultado.Exitoso) {
				await loadData();
				resetForm();
			}
		} finally {
			submitting = false;
		}
	};

	onMount(loadData);
</script>

<section class="card">
	<h1 class="text-2xl font-semibold">Especialidades</h1>

	<div class="card">
		<h2 class="text-xl font-semibold">{editing ? 'Editar especialidad' : 'Crear especialidad'}</h2>
		{#if loading}
			<div class="alert alert-info">Cargando datos...</div>
		{/if}
		{#if serverMessage}
			<div class={`alert ${serverErrors.length ? 'alert-error' : 'alert-success'}`}>
				{serverMessage}
			</div>
		{/if}
		{#if serverErrors.length}
			<ul class="mt-2">
				{#each serverErrors as err}
					<li class="error-text">{err}</li>
				{/each}
			</ul>
		{/if}
		<div class="form-grid mt-4">
			<label class="field">
				<span>Nombre</span>
				<input bind:value={form.Nombre} />
				{#if fieldErrors.Nombre}<span class="error-text">{fieldErrors.Nombre}</span>{/if}
			</label>
			<label class="field">
				<span>Estado</span>
				<select bind:value={form.Activo}>
					{#each defaultBooleanOptions as option}
						<option value={option.value}>{option.label}</option>
					{/each}
				</select>
			</label>
			<label class="field" style="grid-column: span 2;">
				<span>Descripcion</span>
				<textarea bind:value={form.Descripcion}></textarea>
				{#if fieldErrors.Descripcion}<span class="error-text">{fieldErrors.Descripcion}</span>{/if}
			</label>
		</div>
		<div class="actions">
			<button class="btn btn-primary" on:click={submit} disabled={submitting}>
				{submitting ? 'Guardando...' : editing ? 'Guardar cambios' : 'Crear especialidad'}
			</button>
			{#if editing}
				<button class="btn btn-secondary" on:click={resetForm} disabled={submitting}>Cancelar</button>
			{/if}
		</div>
	</div>
</section>

<section class="card">
	<h2 class="text-xl font-semibold">Listado</h2>
	{#if loading}
		<p>Cargando...</p>
	{:else}
		<table class="table">
			<thead>
				<tr>
					<th>Nombre</th>
					<th>Descripcion</th>
					<th>Medicos</th>
					<th>Estado</th>
					<th>Acciones</th>
				</tr>
			</thead>
			<tbody>
				{#if especialidades.length === 0}
					<tr>
						<td colspan="5">Sin registros.</td>
					</tr>
				{:else}
					{#each especialidades as especialidad}
						<tr>
							<td>{especialidad.Nombre}</td>
							<td>{especialidad.Descripcion}</td>
							<td>{especialidad.CantidadMedicos}</td>
							<td><span class="badge">{findLabel(defaultBooleanOptions, especialidad.Activo)}</span></td>
							<td>
								<button class="btn btn-secondary" on:click={() => editarEspecialidad(especialidad)}>Editar</button>
								<button class="btn btn-danger" on:click={() => eliminarEspecialidad(especialidad.Id)}>Eliminar</button>
							</td>
						</tr>
					{/each}
				{/if}
			</tbody>
		</table>
	{/if}
</section>

