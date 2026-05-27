<script lang="ts">
	import { onMount } from 'svelte';
	import { api } from '$lib/api';
	import { toast } from '$lib/toast';
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
	import ConfirmModal from '$lib/components/ConfirmModal.svelte';
	import SkeletonLoader from '$lib/components/SkeletonLoader.svelte';

	let pacientes: PacienteDTO[] = $state([]);
	let loading = $state(true);
	let editing = $state(false);
	let submitting = $state(false);
	let fieldErrors: Record<string, string> = $state({});
	let searchQuery = $state('');

	// Confirm modal state
	let confirmOpen = $state(false);
	let confirmLoading = $state(false);
	let pendingDeleteId = $state<number | null>(null);

	let form = $state({
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
	});

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
	};

	const loadPacientes = async () => {
		loading = true;
		const resultado = await api.getPacientes();
		if (resultado.Exitoso && resultado.Datos) {
			pacientes = resultado.Datos;
		} else {
			toast.error(resultado.Mensaje || 'Error al cargar pacientes');
		}
		loading = false;
	};

	const editarPaciente = (paciente: PacienteDTO) => {
		editing = true;
		fieldErrors = {};
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
		window.scrollTo({ top: 0, behavior: 'smooth' });
	};

	const solicitarEliminar = (id: number) => {
		pendingDeleteId = id;
		confirmOpen = true;
	};

	const confirmarEliminar = async () => {
		if (!pendingDeleteId) return;
		confirmLoading = true;
		const resultado = await api.eliminarPaciente(pendingDeleteId);
		confirmLoading = false;
		confirmOpen = false;
		pendingDeleteId = null;
		if (resultado.Exitoso) {
			toast.success('Paciente eliminado correctamente');
			await loadPacientes();
			resetForm();
		} else {
			toast.error(resultado.Mensaje || 'Error al eliminar');
		}
	};

	const submit = async () => {
		fieldErrors = {};
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
				if (resultado.Exitoso) {
					toast.success('Paciente actualizado correctamente');
					await loadPacientes();
					resetForm();
				} else {
					toast.error(resultado.Mensaje || 'Error al actualizar');
					(resultado.Errores ?? []).forEach((e) => toast.error(e));
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
				fieldErrors = { NumeroDocumento: 'La identificación ya está registrada.' };
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
			if (resultado.Exitoso) {
				toast.success('Paciente creado correctamente');
				await loadPacientes();
				resetForm();
			} else {
				toast.error(resultado.Mensaje || 'Error al crear');
				(resultado.Errores ?? []).forEach((e) => toast.error(e));
			}
		} finally {
			submitting = false;
		}
	};

	let filteredPacientes = $derived(
		pacientes.filter((p) => {
			if (!searchQuery) return true;
			const q = searchQuery.toLowerCase();
			return (
				p.Nombres.toLowerCase().includes(q) ||
				p.Apellidos.toLowerCase().includes(q) ||
				p.NumeroDocumento.toLowerCase().includes(q) ||
				p.Email.toLowerCase().includes(q)
			);
		})
	);

	const estadoBadge = (activo: boolean) =>
		activo ? 'badge badge--active' : 'badge badge--inactive';

	onMount(loadPacientes);
</script>

<svelte:head><title>Pacientes — ECE-FIAD</title></svelte:head>

<ConfirmModal
	open={confirmOpen}
	title="Eliminar paciente"
	message="¿Estás seguro de que deseas eliminar este paciente? Esta acción no se puede deshacer."
	confirmLabel="Sí, eliminar"
	danger={true}
	loading={confirmLoading}
	onconfirm={confirmarEliminar}
	oncancel={() => { confirmOpen = false; pendingDeleteId = null; }}
/>

<div class="page-header-bar">
	<div class="page-header-info">
		<div class="page-icon">
			<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 00-3-3.87"/><path d="M16 3.13a4 4 0 010 7.75"/></svg>
		</div>
		<div>
			<h1 class="page-title">Pacientes</h1>
			<p class="page-subtitle">Gestión del padrón de pacientes</p>
		</div>
	</div>
</div>

<!-- FORM CARD -->
<div class="form-section">
	<div class="form-section__header">
		<span class="form-section__title">{editing ? '✏️ Editar paciente' : '➕ Nuevo paciente'}</span>
		{#if editing}
			<span class="form-section__badge">Modo edición</span>
		{/if}
	</div>

	<div class="form-grid">
		<label class="field">
			<span>Nombres *</span>
			<input bind:value={form.Nombres} placeholder="Ej. Juan Carlos" class:field--error={fieldErrors.Nombres} />
			{#if fieldErrors.Nombres}<span class="error-text">⚠ {fieldErrors.Nombres}</span>{/if}
		</label>
		<label class="field">
			<span>Apellidos *</span>
			<input bind:value={form.Apellidos} placeholder="Ej. García López" class:field--error={fieldErrors.Apellidos} />
			{#if fieldErrors.Apellidos}<span class="error-text">⚠ {fieldErrors.Apellidos}</span>{/if}
		</label>
		<label class="field">
			<span>Tipo documento</span>
			<select bind:value={form.TipoDocumento} disabled={editing}>
				{#each tipoDocumentoOptions as option}
					<option value={option.value}>{option.label}</option>
				{/each}
			</select>
		</label>
		<label class="field">
			<span>Número documento *</span>
			<input bind:value={form.NumeroDocumento} placeholder="Número de identificación" disabled={editing} class:field--error={fieldErrors.NumeroDocumento} />
			{#if fieldErrors.NumeroDocumento}<span class="error-text">⚠ {fieldErrors.NumeroDocumento}</span>{/if}
		</label>
		<label class="field">
			<span>Teléfono</span>
			<input bind:value={form.Telefono} placeholder="+593 999 000 000" />
		</label>
		<label class="field">
			<span>Email *</span>
			<input type="email" bind:value={form.Email} placeholder="correo@dominio.com" class:field--error={fieldErrors.Email} />
			{#if fieldErrors.Email}<span class="error-text">⚠ {fieldErrors.Email}</span>{/if}
		</label>
		<label class="field">
			<span>Fecha nacimiento</span>
			<input type="date" bind:value={form.FechaNacimiento} disabled={editing} />
		</label>
		<label class="field">
			<span>Género</span>
			<select bind:value={form.Genero} disabled={editing}>
				{#each generoOptions as option}
					<option value={option.value}>{option.label}</option>
				{/each}
			</select>
		</label>
		<label class="field">
			<span>Grupo sanguíneo</span>
			<select bind:value={form.GrupoSanguineo} disabled={editing}>
				{#each grupoSanguineoOptions as option}
					<option value={option.value}>{option.label}</option>
				{/each}
			</select>
		</label>
		<label class="field">
			<span>Dirección</span>
			<input bind:value={form.Direccion} placeholder="Calle, ciudad" />
		</label>
	</div>

	<div class="actions">
		<button class="btn btn-primary" onclick={submit} disabled={submitting}>
			{#if submitting}<span class="spinner"></span>{/if}
			{submitting ? 'Guardando...' : editing ? 'Guardar cambios' : 'Crear paciente'}
		</button>
		{#if editing}
			<button class="btn btn-secondary" onclick={resetForm} disabled={submitting}>Cancelar edición</button>
		{/if}
	</div>
</div>

<!-- TABLE CARD -->
<div class="card">
	<div class="section-header">
		<div>
			<h2 class="section-title">Listado de pacientes</h2>
			{#if !loading}
				<span class="record-count">{filteredPacientes.length} de {pacientes.length} registros</span>
			{/if}
		</div>
		<div class="search-box">
			<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="#94a3b8" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
			<input class="search-input" bind:value={searchQuery} placeholder="Buscar paciente..." />
		</div>
	</div>

	{#if loading}
		<SkeletonLoader rows={6} cols={5} />
	{:else}
		<div class="table-container">
			<table class="table">
				<thead>
					<tr>
						<th>Nombre</th>
						<th>Documento</th>
						<th>Email</th>
						<th>Género</th>
						<th>Grupo</th>
						<th>Estado</th>
						<th>Acciones</th>
					</tr>
				</thead>
				<tbody>
					{#if filteredPacientes.length === 0}
						<tr>
							<td colspan="7">
								<div class="empty-state">
									<div class="empty-state__icon">
										<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2"/><circle cx="9" cy="7" r="4"/></svg>
									</div>
									<div class="empty-state__title">{searchQuery ? 'Sin resultados para esa búsqueda' : 'No hay pacientes registrados'}</div>
								</div>
							</td>
						</tr>
					{:else}
						{#each filteredPacientes as paciente}
							<tr>
								<td>
									<div class="patient-name">
										<div class="avatar">{paciente.Nombres[0]}{paciente.Apellidos[0]}</div>
										<div>
											<div class="font-name">{paciente.Nombres} {paciente.Apellidos}</div>
										</div>
									</div>
								</td>
								<td class="mono">{paciente.NumeroDocumento}</td>
								<td>{paciente.Email}</td>
								<td>{findLabel(generoOptions, paciente.Genero)}</td>
								<td><span class="blood-badge">{findLabel(grupoSanguineoOptions, paciente.GrupoSanguineo)}</span></td>
								<td><span class={estadoBadge(paciente.Activo)}>{findLabel(defaultBooleanOptions, paciente.Activo)}</span></td>
								<td>
									<div class="table-actions">
										<button class="btn btn-sm btn-secondary" onclick={() => editarPaciente(paciente)} title="Editar">
											<svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>
											Editar
										</button>
										<button class="btn btn-sm btn-danger" onclick={() => solicitarEliminar(paciente.Id)} title="Eliminar">
											<svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 01-2 2H7a2 2 0 01-2-2V6m3 0V4a1 1 0 011-1h4a1 1 0 011 1v2"/></svg>
										</button>
									</div>
								</td>
							</tr>
						{/each}
					{/if}
				</tbody>
			</table>
		</div>
	{/if}
</div>

<style>
	.page-header-bar {
		display: flex;
		align-items: center;
		justify-content: space-between;
		margin-bottom: 1.5rem;
		flex-wrap: wrap;
		gap: 1rem;
	}

	.page-header-info {
		display: flex;
		align-items: center;
		gap: 1rem;
	}

	.page-icon {
		width: 50px;
		height: 50px;
		border-radius: 14px;
		background: linear-gradient(135deg, #2563eb, #3b82f6);
		display: grid;
		place-items: center;
		box-shadow: 0 4px 14px rgba(37,99,235,0.3);
		flex-shrink: 0;
	}

	.page-title {
		font-size: 1.6rem;
		font-weight: 800;
		color: #0f172a;
		margin: 0;
		letter-spacing: -0.03em;
	}

	.page-subtitle {
		font-size: 0.85rem;
		color: #64748b;
		margin: 0.2rem 0 0;
	}

	.search-box {
		display: flex;
		align-items: center;
		gap: 0.5rem;
		background: #f8fafc;
		border: 1.5px solid #e2e8f0;
		border-radius: 0.6rem;
		padding: 0.45rem 0.85rem;
		transition: border-color 0.15s, box-shadow 0.15s;
	}

	.search-box:focus-within {
		border-color: #2563eb;
		box-shadow: 0 0 0 3px rgba(37,99,235,0.1);
		background: #fff;
	}

	.search-input {
		border: none;
		background: none;
		outline: none;
		font-size: 0.875rem;
		color: #0f172a;
		width: 200px;
	}

	.patient-name {
		display: flex;
		align-items: center;
		gap: 0.65rem;
	}

	.avatar {
		width: 34px;
		height: 34px;
		border-radius: 50%;
		background: linear-gradient(135deg, #dbeafe, #bfdbfe);
		color: #1d4ed8;
		font-size: 0.75rem;
		font-weight: 700;
		display: grid;
		place-items: center;
		flex-shrink: 0;
		text-transform: uppercase;
	}

	.font-name {
		font-weight: 600;
		color: #0f172a;
		font-size: 0.875rem;
	}

	.mono {
		font-family: 'JetBrains Mono', 'Fira Code', monospace;
		font-size: 0.825rem;
		color: #475569;
	}

	.blood-badge {
		display: inline-flex;
		align-items: center;
		padding: 0.15rem 0.5rem;
		border-radius: 0.35rem;
		background: #fef2f2;
		color: #b91c1c;
		font-size: 0.775rem;
		font-weight: 700;
	}
</style>
