<script lang="ts">
	import { onMount } from 'svelte';
	import { api } from '$lib/api';
	import { toDateInputValue } from '$lib/date';
	import { actualizarHistoriaSchema, crearHistoriaSchema } from '$lib/validation/schemas';
	import { mapZodErrors } from '$lib/validation/utils';
	import type { HistoriaClinicaDTO, PacienteDTO } from '$lib/types';
	import { defaultBooleanOptions, findLabel } from '$lib/types';

	let historias: HistoriaClinicaDTO[] = [];
	let pacientes: PacienteDTO[] = [];
	let pacientesSinHistoria: PacienteDTO[] = [];
	let loading = false;
	let editing = false;
	let submitting = false;
	let fieldErrors: Record<string, string> = {};
	let serverMessage = '';
	let serverErrors: string[] = [];

	let form = {
		Id: 0,
		IdPaciente: 0,
		FechaApertura: toDateInputValue(new Date()),
		Alergias: '',
		AntecedentesFamiliares: '',
		AntecedentesPersonales: '',
		Activo: true
	};

	const resetForm = () => {
		form = {
			Id: 0,
			IdPaciente: pacientesSinHistoria[0]?.Id ?? 0,
			FechaApertura: toDateInputValue(new Date()),
			Alergias: '',
			AntecedentesFamiliares: '',
			AntecedentesPersonales: '',
			Activo: true
		};
		editing = false;
		fieldErrors = {};
		serverMessage = '';
		serverErrors = [];
	};

	const loadData = async () => {
		loading = true;
		const [historiasResult, pacientesResult, sinHistoriaResult] = await Promise.all([
			api.getHistorias(),
			api.getPacientes(),
			api.getPacientesActivosSinHistoria()
		]);
		if (historiasResult.Exitoso && historiasResult.Datos) {
			historias = historiasResult.Datos;
		} else {
			serverMessage = historiasResult.Mensaje;
			serverErrors = historiasResult.Errores ?? [];
		}
		if (pacientesResult.Exitoso && pacientesResult.Datos) {
			pacientes = pacientesResult.Datos;
		}
		if (sinHistoriaResult.Exitoso && sinHistoriaResult.Datos) {
			pacientesSinHistoria = sinHistoriaResult.Datos;
		}
		if (!editing) {
			form.IdPaciente = pacientesSinHistoria[0]?.Id ?? 0;
		}
		loading = false;
	};

	const editarHistoria = (historia: HistoriaClinicaDTO) => {
		editing = true;
		fieldErrors = {};
		serverMessage = '';
		serverErrors = [];
		form = {
			Id: historia.Id,
			IdPaciente: historia.IdPaciente,
			FechaApertura: toDateInputValue(historia.FechaApertura),
			Alergias: historia.Alergias ?? '',
			AntecedentesFamiliares: historia.AntecedentesFamiliares ?? '',
			AntecedentesPersonales: historia.AntecedentesPersonales ?? '',
			Activo: historia.Activo
		};
	};

	const eliminarHistoria = async (id: number) => {
		if (!confirm('Deseas eliminar esta historia clinica?')) return;
		const resultado = await api.eliminarHistoria(id);
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
			const validation = (editing ? actualizarHistoriaSchema : crearHistoriaSchema).safeParse({
				Id: form.Id,
				IdPaciente: Number(form.IdPaciente),
				FechaApertura: form.FechaApertura,
				Alergias: form.Alergias,
				AntecedentesFamiliares: form.AntecedentesFamiliares,
				AntecedentesPersonales: form.AntecedentesPersonales,
				Activo: form.Activo
			});
			if (!validation.success) {
				fieldErrors = mapZodErrors(validation.error);
				return;
			}

			if (form.Activo) {
				const existe = await api.existeHistoriaActiva(Number(form.IdPaciente), editing ? form.Id : undefined);
				if (existe) {
					fieldErrors = { IdPaciente: 'El paciente ya tiene una historia clinica activa.' };
					return;
				}
			}

			const payload = {
				IdPaciente: Number(form.IdPaciente),
				FechaApertura: form.FechaApertura,
				Alergias: form.Alergias,
				AntecedentesFamiliares: form.AntecedentesFamiliares,
				AntecedentesPersonales: form.AntecedentesPersonales,
				Activo: form.Activo
			};

			const resultado = editing
				? await api.actualizarHistoria({ ...payload, Id: form.Id })
				: await api.crearHistoria(payload);

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

	$: pacientesDisponibles = editing ? pacientes : pacientesSinHistoria;

	onMount(loadData);
</script>

<section class="card">
	<h1 class="text-2xl font-semibold">Historias clinicas</h1>

	<div class="card">
		<h2 class="text-xl font-semibold">{editing ? 'Editar historia' : 'Crear historia'}</h2>
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
					{#each pacientesDisponibles as paciente}
						<option value={paciente.Id}>{paciente.Nombres} {paciente.Apellidos}</option>
					{/each}
				</select>
				{#if fieldErrors.IdPaciente}<span class="error-text">{fieldErrors.IdPaciente}</span>{/if}
			</label>
			<label class="field">
				<span>Fecha apertura</span>
				<input type="date" bind:value={form.FechaApertura} />
				{#if fieldErrors.FechaApertura}<span class="error-text">{fieldErrors.FechaApertura}</span>{/if}
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
				<span>Alergias</span>
				<textarea bind:value={form.Alergias}></textarea>
				{#if fieldErrors.Alergias}<span class="error-text">{fieldErrors.Alergias}</span>{/if}
			</label>
			<label class="field" style="grid-column: span 2;">
				<span>Antecedentes familiares</span>
				<textarea bind:value={form.AntecedentesFamiliares}></textarea>
				{#if fieldErrors.AntecedentesFamiliares}<span class="error-text">{fieldErrors.AntecedentesFamiliares}</span>{/if}
			</label>
			<label class="field" style="grid-column: span 2;">
				<span>Antecedentes personales</span>
				<textarea bind:value={form.AntecedentesPersonales}></textarea>
				{#if fieldErrors.AntecedentesPersonales}<span class="error-text">{fieldErrors.AntecedentesPersonales}</span>{/if}
			</label>
		</div>
		<div class="actions">
			<button class="btn btn-primary" on:click={submit} disabled={submitting}>
				{submitting ? 'Guardando...' : editing ? 'Guardar cambios' : 'Crear historia'}
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
					<th>Fecha apertura</th>
					<th>Estado</th>
					<th>Acciones</th>
				</tr>
			</thead>
			<tbody>
				{#if historias.length === 0}
					<tr>
						<td colspan="4">Sin registros.</td>
					</tr>
				{:else}
					{#each historias as historia}
						<tr>
							<td>{historia.NombrePaciente}</td>
							<td>{toDateInputValue(historia.FechaApertura)}</td>
							<td><span class="badge">{findLabel(defaultBooleanOptions, historia.Activo)}</span></td>
							<td>
								<button class="btn btn-secondary" on:click={() => editarHistoria(historia)}>Editar</button>
								<button class="btn btn-danger" on:click={() => eliminarHistoria(historia.Id)}>Eliminar</button>
							</td>
						</tr>
					{/each}
				{/if}
			</tbody>
		</table>
	{/if}
</section>

