<script lang="ts">
	import { onMount } from 'svelte';
	import { api } from '$lib/api';
	import { toDateInputValue } from '$lib/date';
	import { actualizarEvolucionSchema, crearEvolucionSchema } from '$lib/validation/schemas';
	import { mapZodErrors } from '$lib/validation/utils';
	import type { DoctorDTO, EvolucionDTO, HistoriaClinicaDTO } from '$lib/types';
	import { defaultBooleanOptions, findLabel } from '$lib/types';

	let evoluciones: EvolucionDTO[] = [];
	let doctores: DoctorDTO[] = [];
	let historias: HistoriaClinicaDTO[] = [];
	let loading = false;
	let editing = false;
	let submitting = false;
	let fieldErrors: Record<string, string> = {};
	let serverMessage = '';
	let serverErrors: string[] = [];

	let form = {
		Id: 0,
		IdHistoriaClinica: 0,
		IdDoctor: 0,
		Fecha: toDateInputValue(new Date()),
		Diagnostico: '',
		Tratamiento: '',
		Notas: '',
		Activo: true
	};

	const resetForm = () => {
		form = {
			Id: 0,
			IdHistoriaClinica: historias[0]?.Id ?? 0,
			IdDoctor: doctores[0]?.Id ?? 0,
			Fecha: toDateInputValue(new Date()),
			Diagnostico: '',
			Tratamiento: '',
			Notas: '',
			Activo: true
		};
		editing = false;
		fieldErrors = {};
		serverMessage = '';
		serverErrors = [];
	};

	const loadData = async () => {
		loading = true;
		const [evolucionesResult, doctoresResult, historiasResult] = await Promise.all([
			api.getEvoluciones(),
			api.getDoctores(),
			api.getHistoriasActivas()
		]);
		if (evolucionesResult.Exitoso && evolucionesResult.Datos) {
			evoluciones = evolucionesResult.Datos;
		} else {
			serverMessage = evolucionesResult.Mensaje;
			serverErrors = evolucionesResult.Errores ?? [];
		}
		if (doctoresResult.Exitoso && doctoresResult.Datos) {
			doctores = doctoresResult.Datos;
		}
		if (historiasResult.Exitoso && historiasResult.Datos) {
			historias = historiasResult.Datos;
		}
		if (!editing) {
			form.IdDoctor = doctores[0]?.Id ?? 0;
			form.IdHistoriaClinica = historias[0]?.Id ?? 0;
		}
		loading = false;
	};

	const editarEvolucion = (evolucion: EvolucionDTO) => {
		editing = true;
		fieldErrors = {};
		serverMessage = '';
		serverErrors = [];
		form = {
			Id: evolucion.Id,
			IdHistoriaClinica: evolucion.IdHistoriaClinica,
			IdDoctor: evolucion.IdDoctor,
			Fecha: toDateInputValue(evolucion.Fecha),
			Diagnostico: evolucion.Diagnostico,
			Tratamiento: evolucion.Tratamiento,
			Notas: evolucion.Notas ?? '',
			Activo: evolucion.Activo
		};
	};

	const eliminarEvolucion = async (id: number) => {
		if (!confirm('Deseas eliminar esta evolucion?')) return;
		const resultado = await api.eliminarEvolucion(id);
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
			const validation = (editing ? actualizarEvolucionSchema : crearEvolucionSchema).safeParse({
				Id: form.Id,
				IdHistoriaClinica: Number(form.IdHistoriaClinica),
				IdDoctor: Number(form.IdDoctor),
				Fecha: form.Fecha,
				Diagnostico: form.Diagnostico,
				Tratamiento: form.Tratamiento,
				Notas: form.Notas,
				Activo: form.Activo
			});
			if (!validation.success) {
				fieldErrors = mapZodErrors(validation.error);
				return;
			}

			const payload = {
				IdHistoriaClinica: Number(form.IdHistoriaClinica),
				IdDoctor: Number(form.IdDoctor),
				Fecha: form.Fecha,
				Diagnostico: form.Diagnostico,
				Tratamiento: form.Tratamiento,
				Notas: form.Notas,
				Activo: form.Activo
			};

			const resultado = editing
				? await api.actualizarEvolucion({ ...payload, Id: form.Id })
				: await api.crearEvolucion(payload);

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
	<h1 class="text-2xl font-semibold">Evoluciones</h1>

	<div class="card">
		<h2 class="text-xl font-semibold">{editing ? 'Editar evolucion' : 'Crear evolucion'}</h2>
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
				<span>Historia clinica</span>
				<select bind:value={form.IdHistoriaClinica}>
					{#each historias as historia}
						<option value={historia.Id}>#{historia.Id} - {historia.NombrePaciente}</option>
					{/each}
				</select>
				{#if fieldErrors.IdHistoriaClinica}<span class="error-text">{fieldErrors.IdHistoriaClinica}</span>{/if}
			</label>
			<label class="field">
				<span>Doctor</span>
				<select bind:value={form.IdDoctor}>
					{#each doctores as doctor}
						<option value={doctor.Id}>{doctor.Nombre}</option>
					{/each}
				</select>
				{#if fieldErrors.IdDoctor}<span class="error-text">{fieldErrors.IdDoctor}</span>{/if}
			</label>
			<label class="field">
				<span>Fecha</span>
				<input type="date" bind:value={form.Fecha} />
				{#if fieldErrors.Fecha}<span class="error-text">{fieldErrors.Fecha}</span>{/if}
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
				<span>Diagnostico</span>
				<textarea bind:value={form.Diagnostico}></textarea>
				{#if fieldErrors.Diagnostico}<span class="error-text">{fieldErrors.Diagnostico}</span>{/if}
			</label>
			<label class="field" style="grid-column: span 2;">
				<span>Tratamiento</span>
				<textarea bind:value={form.Tratamiento}></textarea>
				{#if fieldErrors.Tratamiento}<span class="error-text">{fieldErrors.Tratamiento}</span>{/if}
			</label>
			<label class="field" style="grid-column: span 2;">
				<span>Notas</span>
				<textarea bind:value={form.Notas}></textarea>
				{#if fieldErrors.Notas}<span class="error-text">{fieldErrors.Notas}</span>{/if}
			</label>
		</div>
		<div class="actions">
			<button class="btn btn-primary" on:click={submit} disabled={submitting}>
				{submitting ? 'Guardando...' : editing ? 'Guardar cambios' : 'Crear evolucion'}
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
					<th>Paciente</th>
					<th>Doctor</th>
					<th>Fecha</th>
					<th>Estado</th>
					<th>Acciones</th>
				</tr>
			</thead>
			<tbody>
				{#if evoluciones.length === 0}
					<tr>
						<td colspan="5">Sin registros.</td>
					</tr>
				{:else}
					{#each evoluciones as evolucion}
						<tr>
							<td>{evolucion.NombrePaciente}</td>
							<td>{evolucion.NombreDoctor}</td>
							<td>{toDateInputValue(evolucion.Fecha)}</td>
							<td><span class="badge">{findLabel(defaultBooleanOptions, evolucion.Activo)}</span></td>
							<td>
								<button class="btn btn-secondary" on:click={() => editarEvolucion(evolucion)}>Editar</button>
								<button class="btn btn-danger" on:click={() => eliminarEvolucion(evolucion.Id)}>Eliminar</button>
							</td>
						</tr>
					{/each}
				{/if}
			</tbody>
		</table>
	{/if}
</section>

