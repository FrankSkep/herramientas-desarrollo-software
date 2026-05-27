<script lang="ts">
	import { onMount } from 'svelte';
	import { api } from '$lib/api';
	import { toDateInputValue } from '$lib/date';
	import { actualizarDoctorSchema, crearDoctorSchema } from '$lib/validation/schemas';
	import { mapZodErrors } from '$lib/validation/utils';
	import type { DoctorDTO, EspecialidadDTO } from '$lib/types';
	import { defaultBooleanOptions, findLabel } from '$lib/types';

	let doctores: DoctorDTO[] = [];
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
		IdEspecialidad: 0,
		Telefono: '',
		Email: '',
		FechaContratacion: toDateInputValue(new Date()),
		Activo: true
	};

	const resetForm = () => {
		form = {
			Id: 0,
			Nombre: '',
			Descripcion: '',
			IdEspecialidad: especialidades[0]?.Id ?? 0,
			Telefono: '',
			Email: '',
			FechaContratacion: toDateInputValue(new Date()),
			Activo: true
		};
		editing = false;
		fieldErrors = {};
		serverMessage = '';
		serverErrors = [];
	};

	const loadData = async () => {
		loading = true;
		const [doctoresResult, especialidadesResult] = await Promise.all([
			api.getDoctores(),
			api.getEspecialidades()
		]);
		if (doctoresResult.Exitoso && doctoresResult.Datos) {
			doctores = doctoresResult.Datos;
		} else {
			serverMessage = doctoresResult.Mensaje;
			serverErrors = doctoresResult.Errores ?? [];
		}
		if (especialidadesResult.Exitoso && especialidadesResult.Datos) {
			especialidades = especialidadesResult.Datos;
		}
		if (!editing) {
			form.IdEspecialidad = especialidades[0]?.Id ?? 0;
		}
		loading = false;
	};

	const editarDoctor = (doctor: DoctorDTO) => {
		editing = true;
		fieldErrors = {};
		serverMessage = '';
		serverErrors = [];
		form = {
			Id: doctor.Id,
			Nombre: doctor.Nombre,
			Descripcion: doctor.Descripcion,
			IdEspecialidad: doctor.IdEspecialidad,
			Telefono: doctor.Telefono,
			Email: doctor.Email,
			FechaContratacion: toDateInputValue(doctor.FechaContratacion),
			Activo: doctor.Activo
		};
	};

	const eliminarDoctor = async (id: number) => {
		if (!confirm('Deseas eliminar este doctor?')) return;
		const resultado = await api.eliminarDoctor(id);
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
			const validation = (editing ? actualizarDoctorSchema : crearDoctorSchema).safeParse({
				Id: form.Id,
				Nombre: form.Nombre,
				Descripcion: form.Descripcion,
				IdEspecialidad: Number(form.IdEspecialidad),
				Telefono: form.Telefono,
				Email: form.Email,
				FechaContratacion: form.FechaContratacion,
				Activo: form.Activo
			});
			if (!validation.success) {
				fieldErrors = mapZodErrors(validation.error);
				return;
			}

			const payload = {
				Nombre: form.Nombre,
				Descripcion: form.Descripcion,
				IdEspecialidad: Number(form.IdEspecialidad),
				Telefono: form.Telefono,
				Email: form.Email,
				FechaContratacion: form.FechaContratacion,
				Activo: form.Activo
			};

			const resultado = editing
				? await api.actualizarDoctor({ ...payload, Id: form.Id })
				: await api.crearDoctor(payload);

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
	<h1 class="text-2xl font-semibold">Doctores</h1>

	<div class="card">
		<h2 class="text-xl font-semibold">{editing ? 'Editar doctor' : 'Crear doctor'}</h2>
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
				<span>Especialidad</span>
				<select bind:value={form.IdEspecialidad}>
					{#each especialidades as especialidad}
						<option value={especialidad.Id}>{especialidad.Nombre}</option>
					{/each}
				</select>
				{#if fieldErrors.IdEspecialidad}<span class="error-text">{fieldErrors.IdEspecialidad}</span>{/if}
			</label>
			<label class="field">
				<span>Email</span>
				<input type="email" bind:value={form.Email} />
				{#if fieldErrors.Email}<span class="error-text">{fieldErrors.Email}</span>{/if}
			</label>
			<label class="field">
				<span>Telefono</span>
				<input bind:value={form.Telefono} />
			</label>
			<label class="field">
				<span>Fecha contratacion</span>
				<input type="date" bind:value={form.FechaContratacion} />
				{#if fieldErrors.FechaContratacion}<span class="error-text">{fieldErrors.FechaContratacion}</span>{/if}
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
				{submitting ? 'Guardando...' : editing ? 'Guardar cambios' : 'Crear doctor'}
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
					<th>Especialidad</th>
					<th>Email</th>
					<th>Estado</th>
					<th>Acciones</th>
				</tr>
			</thead>
			<tbody>
				{#if doctores.length === 0}
					<tr>
						<td colspan="5">Sin registros.</td>
					</tr>
				{:else}
					{#each doctores as doctor}
						<tr>
							<td>{doctor.Nombre}</td>
							<td>{doctor.NombreEspecialidad}</td>
							<td>{doctor.Email}</td>
							<td><span class="badge">{findLabel(defaultBooleanOptions, doctor.Activo)}</span></td>
							<td>
								<button class="btn btn-secondary" on:click={() => editarDoctor(doctor)}>Editar</button>
								<button class="btn btn-danger" on:click={() => eliminarDoctor(doctor.Id)}>Eliminar</button>
							</td>
						</tr>
					{/each}
				{/if}
			</tbody>
		</table>
	{/if}
</section>

