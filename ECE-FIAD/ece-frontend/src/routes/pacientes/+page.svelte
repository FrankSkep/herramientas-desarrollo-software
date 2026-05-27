<script lang="ts">
	import { onMount } from 'svelte';
	import { api } from '$lib/api';
	import { toDateInputValue } from '$lib/date';
	import {
		generoOptions,
		grupoSanguineoOptions,
		tipoDocumentoOptions,
		findLabel,
		defaultBooleanOptions
	} from '$lib/types';
	import type { PacienteDTO } from '$lib/types';
	import { actualizarPacienteSchema, crearPacienteSchema } from '$lib/validation/schemas';
	import { mapZodErrors } from '$lib/validation/utils';

	let pacientes: PacienteDTO[] = [];
	let loading = false;
	let editing = false;
	let submitting = false;
	let fieldErrors: Record<string, string> = {};
	let serverMessage = '';
	let serverErrors: string[] = [];

	let form = {
		Id: 0,
		Nombres: '',
		Apellidos: '',
		NumeroDocumento: '',
		TipoDocumento: 1,
		Telefono: '',
		Email: '',
		FechaNacimiento: toDateInputValue(new Date()),
		Genero: 1,
		GrupoSanguineo: 1,
		Direccion: ''
	};

	const resetForm = () => {
		form = {
			Id: 0,
			Nombres: '',
			Apellidos: '',
			NumeroDocumento: '',
			TipoDocumento: 1,
			Telefono: '',
			Email: '',
			FechaNacimiento: toDateInputValue(new Date()),
			Genero: 1,
			GrupoSanguineo: 1,
			Direccion: ''
		};
		editing = false;
		fieldErrors = {};
		serverMessage = '';
		serverErrors = [];
	};

	const loadPacientes = async () => {
		loading = true;
		const resultado = await api.getPacientes();
		if (resultado.Exitoso && resultado.Datos) {
			pacientes = resultado.Datos;
		} else {
			serverMessage = resultado.Mensaje;
			serverErrors = resultado.Errores ?? [];
		}
		loading = false;
	};

	const editarPaciente = (paciente: PacienteDTO) => {
		editing = true;
		fieldErrors = {};
		serverMessage = '';
		serverErrors = [];
		form = {
			Id: paciente.Id,
			Nombres: paciente.Nombres,
			Apellidos: paciente.Apellidos,
			NumeroDocumento: paciente.NumeroDocumento,
			TipoDocumento: paciente.TipoDocumento,
			Telefono: paciente.Telefono ?? '',
			Email: paciente.Email,
			FechaNacimiento: toDateInputValue(paciente.FechaNacimiento),
			Genero: paciente.Genero,
			GrupoSanguineo: paciente.GrupoSanguineo,
			Direccion: paciente.Direccion ?? ''
		};
	};

	const eliminarPaciente = async (id: number) => {
		if (!confirm('Deseas eliminar este paciente?')) return;
		const resultado = await api.eliminarPaciente(id);
		serverMessage = resultado.Mensaje;
		serverErrors = resultado.Errores ?? [];
		if (resultado.Exitoso) {
			await loadPacientes();
			resetForm();
		}
	};

	const submit = async () => {
		fieldErrors = {};
		serverMessage = '';
		serverErrors = [];
		submitting = true;

		try {
			if (editing) {
				const validation = actualizarPacienteSchema.safeParse({
					Id: form.Id,
					Nombres: form.Nombres,
					Apellidos: form.Apellidos,
					Email: form.Email
				});
				if (!validation.success) {
					fieldErrors = mapZodErrors(validation.error);
					return;
				}
				const resultado = await api.actualizarPaciente({
					Id: form.Id,
					Nombres: form.Nombres,
					Apellidos: form.Apellidos,
					Telefono: form.Telefono,
					Email: form.Email,
					Direccion: form.Direccion
				});
				serverMessage = resultado.Mensaje;
				serverErrors = resultado.Errores ?? [];
				if (resultado.Exitoso) {
					await loadPacientes();
					resetForm();
				}
				return;
			}

			const validation = crearPacienteSchema.safeParse({
				Nombres: form.Nombres,
				Apellidos: form.Apellidos,
				NumeroDocumento: form.NumeroDocumento,
				TipoDocumento: form.TipoDocumento,
				Telefono: form.Telefono,
				Email: form.Email,
				FechaNacimiento: form.FechaNacimiento,
				Genero: form.Genero,
				GrupoSanguineo: form.GrupoSanguineo,
				Direccion: form.Direccion
			});
			if (!validation.success) {
				fieldErrors = mapZodErrors(validation.error);
				return;
			}

			const existe = await api.existeIdentificacion(form.NumeroDocumento);
			if (existe) {
				fieldErrors = { NumeroDocumento: 'La identificacion ya existe.' };
				return;
			}

			const resultado = await api.crearPaciente({
				Nombres: form.Nombres,
				Apellidos: form.Apellidos,
				NumeroDocumento: form.NumeroDocumento,
				TipoDocumento: form.TipoDocumento,
				Telefono: form.Telefono,
				Email: form.Email,
				FechaNacimiento: form.FechaNacimiento,
				Genero: form.Genero,
				GrupoSanguineo: form.GrupoSanguineo,
				Direccion: form.Direccion
			});
			serverMessage = resultado.Mensaje;
			serverErrors = resultado.Errores ?? [];
			if (resultado.Exitoso) {
				await loadPacientes();
				resetForm();
			}
		} finally {
			submitting = false;
		}
	};

	onMount(loadPacientes);
</script>

<section class="card">
	<h1 class="text-2xl font-semibold">Pacientes</h1>

	<div class="card">
		<h2 class="text-xl font-semibold">{editing ? 'Editar paciente' : 'Crear paciente'}</h2>
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
				<span>Nombres</span>
				<input bind:value={form.Nombres} placeholder="Nombres" />
				{#if fieldErrors.Nombres}<span class="error-text">{fieldErrors.Nombres}</span>{/if}
			</label>
			<label class="field">
				<span>Apellidos</span>
				<input bind:value={form.Apellidos} placeholder="Apellidos" />
				{#if fieldErrors.Apellidos}<span class="error-text">{fieldErrors.Apellidos}</span>{/if}
			</label>
			<label class="field">
				<span>Numero documento</span>
				<input bind:value={form.NumeroDocumento} placeholder="Identificacion" disabled={editing} />
				{#if fieldErrors.NumeroDocumento}<span class="error-text">{fieldErrors.NumeroDocumento}</span>{/if}
			</label>
			<label class="field">
				<span>Tipo documento</span>
				<select bind:value={form.TipoDocumento} disabled={editing}>
					{#each tipoDocumentoOptions as option}
						<option value={option.value}>{option.label}</option>
					{/each}
				</select>
				{#if fieldErrors.TipoDocumento}<span class="error-text">{fieldErrors.TipoDocumento}</span>{/if}
			</label>
			<label class="field">
				<span>Telefono</span>
				<input bind:value={form.Telefono} placeholder="Telefono" />
				{#if fieldErrors.Telefono}<span class="error-text">{fieldErrors.Telefono}</span>{/if}
			</label>
			<label class="field">
				<span>Email</span>
				<input type="email" bind:value={form.Email} placeholder="correo@dominio.com" />
				{#if fieldErrors.Email}<span class="error-text">{fieldErrors.Email}</span>{/if}
			</label>
			<label class="field">
				<span>Fecha nacimiento</span>
				<input type="date" bind:value={form.FechaNacimiento} disabled={editing} />
				{#if fieldErrors.FechaNacimiento}<span class="error-text">{fieldErrors.FechaNacimiento}</span>{/if}
			</label>
			<label class="field">
				<span>Genero</span>
				<select bind:value={form.Genero} disabled={editing}>
					{#each generoOptions as option}
						<option value={option.value}>{option.label}</option>
					{/each}
				</select>
				{#if fieldErrors.Genero}<span class="error-text">{fieldErrors.Genero}</span>{/if}
			</label>
			<label class="field">
				<span>Grupo sanguineo</span>
				<select bind:value={form.GrupoSanguineo} disabled={editing}>
					{#each grupoSanguineoOptions as option}
						<option value={option.value}>{option.label}</option>
					{/each}
				</select>
				{#if fieldErrors.GrupoSanguineo}<span class="error-text">{fieldErrors.GrupoSanguineo}</span>{/if}
			</label>
			<label class="field">
				<span>Direccion</span>
				<input bind:value={form.Direccion} placeholder="Direccion" />
			</label>
		</div>
		<div class="actions">
			<button class="btn btn-primary" on:click={submit} disabled={submitting}>
				{submitting ? 'Guardando...' : editing ? 'Guardar cambios' : 'Crear paciente'}
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
					<th>Documento</th>
					<th>Email</th>
					<th>Genero</th>
					<th>Grupo sanguineo</th>
					<th>Estado</th>
					<th>Acciones</th>
				</tr>
			</thead>
			<tbody>
				{#if pacientes.length === 0}
					<tr>
						<td colspan="7">Sin registros.</td>
					</tr>
				{:else}
					{#each pacientes as paciente}
						<tr>
							<td>{paciente.Nombres} {paciente.Apellidos}</td>
							<td>{paciente.NumeroDocumento}</td>
							<td>{paciente.Email}</td>
							<td>{findLabel(generoOptions, paciente.Genero)}</td>
							<td>{findLabel(grupoSanguineoOptions, paciente.GrupoSanguineo)}</td>
							<td><span class="badge">{findLabel(defaultBooleanOptions, paciente.Activo)}</span></td>
							<td class="space-x-2">
								<button class="btn btn-secondary" on:click={() => editarPaciente(paciente)}>Editar</button>
								<button class="btn btn-danger" on:click={() => eliminarPaciente(paciente.Id)}>Eliminar</button>
							</td>
						</tr>
					{/each}
				{/if}
			</tbody>
		</table>
	{/if}
</section>

