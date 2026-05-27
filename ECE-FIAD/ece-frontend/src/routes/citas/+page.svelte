<script lang="ts">
	import { onMount } from 'svelte';
	import { api } from '$lib/api';
	import { toDateTimeLocalValue, normalizeDateTimeLocal } from '$lib/date';
	import { actualizarCitaSchema, crearCitaSchema } from '$lib/validation/schemas';
	import { mapZodErrors } from '$lib/validation/utils';
	import type { CitaDTO, DoctorDTO, PacienteDTO } from '$lib/types';
	import { estadoCitaOptions, findLabel } from '$lib/types';

	let citas: CitaDTO[] = [];
	let pacientes: PacienteDTO[] = [];
	let doctores: DoctorDTO[] = [];
	let loading = false;
	let editing = false;
	let submitting = false;
	let fieldErrors: Record<string, string> = {};
	let serverMessage = '';
	let serverErrors: string[] = [];

	let form = {
		Id: 0,
		IdPaciente: 0,
		IdDoctor: 0,
		FechaHora: toDateTimeLocalValue(new Date()),
		Motivo: '',
		Notas: '',
		Estado: 1
	};

	const resetForm = () => {
		form = {
			Id: 0,
			IdPaciente: pacientes[0]?.Id ?? 0,
			IdDoctor: doctores[0]?.Id ?? 0,
			FechaHora: toDateTimeLocalValue(new Date()),
			Motivo: '',
			Notas: '',
			Estado: 1
		};
		editing = false;
		fieldErrors = {};
		serverMessage = '';
		serverErrors = [];
	};

	const loadData = async () => {
		loading = true;
		const [citasResult, pacientesResult, doctoresResult] = await Promise.all([
			api.getCitas(),
			api.getPacientes(),
			api.getDoctores()
		]);
		if (citasResult.Exitoso && citasResult.Datos) {
			citas = citasResult.Datos;
		} else {
			serverMessage = citasResult.Mensaje;
			serverErrors = citasResult.Errores ?? [];
		}
		if (pacientesResult.Exitoso && pacientesResult.Datos) {
			pacientes = pacientesResult.Datos;
		}
		if (doctoresResult.Exitoso && doctoresResult.Datos) {
			doctores = doctoresResult.Datos;
		}
		if (!editing) {
			form.IdPaciente = pacientes[0]?.Id ?? 0;
			form.IdDoctor = doctores[0]?.Id ?? 0;
		}
		loading = false;
	};

	const editarCita = (cita: CitaDTO) => {
		editing = true;
		fieldErrors = {};
		serverMessage = '';
		serverErrors = [];
		form = {
			Id: cita.Id,
			IdPaciente: cita.IdPaciente,
			IdDoctor: cita.IdDoctor,
			FechaHora: toDateTimeLocalValue(cita.FechaHora),
			Motivo: cita.Motivo,
			Notas: cita.Notas ?? '',
			Estado: cita.Estado
		};
	};

	const eliminarCita = async (id: number) => {
		if (!confirm('Deseas eliminar esta cita?')) return;
		const resultado = await api.eliminarCita(id);
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
			const validation = (editing ? actualizarCitaSchema : crearCitaSchema).safeParse({
				Id: form.Id,
				IdPaciente: Number(form.IdPaciente),
				IdDoctor: Number(form.IdDoctor),
				FechaHora: form.FechaHora,
				Motivo: form.Motivo,
				Notas: form.Notas,
				Estado: Number(form.Estado)
			});
			if (!validation.success) {
				fieldErrors = mapZodErrors(validation.error);
				return;
			}

			const fechaHora = normalizeDateTimeLocal(form.FechaHora);
			const disponible = await api.estaDisponible(Number(form.IdDoctor), fechaHora, editing ? form.Id : undefined);
			if (!disponible) {
				fieldErrors = { FechaHora: 'El doctor no esta disponible en ese horario.' };
				return;
			}

			const payload = {
				IdPaciente: Number(form.IdPaciente),
				IdDoctor: Number(form.IdDoctor),
				FechaHora: fechaHora,
				Motivo: form.Motivo,
				Notas: form.Notas,
				Estado: Number(form.Estado)
			};

			const resultado = editing
				? await api.actualizarCita({ ...payload, Id: form.Id })
				: await api.crearCita(payload);

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
	<h1 class="text-2xl font-semibold">Citas</h1>

	<div class="card">
		<h2 class="text-xl font-semibold">{editing ? 'Editar cita' : 'Crear cita'}</h2>
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
				<span>Paciente</span>
				<select bind:value={form.IdPaciente}>
					{#each pacientes as paciente}
						<option value={paciente.Id}>{paciente.Nombres} {paciente.Apellidos}</option>
					{/each}
				</select>
				{#if fieldErrors.IdPaciente}<span class="error-text">{fieldErrors.IdPaciente}</span>{/if}
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
				<span>Fecha y hora</span>
				<input type="datetime-local" bind:value={form.FechaHora} />
				{#if fieldErrors.FechaHora}<span class="error-text">{fieldErrors.FechaHora}</span>{/if}
			</label>
			<label class="field">
				<span>Estado</span>
				<select bind:value={form.Estado}>
					{#each estadoCitaOptions as option}
						<option value={option.value}>{option.label}</option>
					{/each}
				</select>
				{#if fieldErrors.Estado}<span class="error-text">{fieldErrors.Estado}</span>{/if}
			</label>
			<label class="field" style="grid-column: span 2;">
				<span>Motivo</span>
				<textarea bind:value={form.Motivo}></textarea>
				{#if fieldErrors.Motivo}<span class="error-text">{fieldErrors.Motivo}</span>{/if}
			</label>
			<label class="field" style="grid-column: span 2;">
				<span>Notas</span>
				<textarea bind:value={form.Notas}></textarea>
				{#if fieldErrors.Notas}<span class="error-text">{fieldErrors.Notas}</span>{/if}
			</label>
		</div>
		<div class="actions">
			<button class="btn btn-primary" on:click={submit} disabled={submitting}>
				{submitting ? 'Guardando...' : editing ? 'Guardar cambios' : 'Crear cita'}
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
				{#if citas.length === 0}
					<tr>
						<td colspan="5">Sin registros.</td>
					</tr>
				{:else}
					{#each citas as cita}
						<tr>
							<td>{cita.NombrePaciente}</td>
							<td>{cita.NombreDoctor}</td>
							<td>{toDateTimeLocalValue(cita.FechaHora)}</td>
							<td><span class="badge">{findLabel(estadoCitaOptions, cita.Estado)}</span></td>
							<td>
								<button class="btn btn-secondary" on:click={() => editarCita(cita)}>Editar</button>
								<button class="btn btn-danger" on:click={() => eliminarCita(cita.Id)}>Eliminar</button>
							</td>
						</tr>
					{/each}
				{/if}
			</tbody>
		</table>
	{/if}
</section>

