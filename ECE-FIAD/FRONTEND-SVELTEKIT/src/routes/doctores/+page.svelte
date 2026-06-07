<script lang="ts">
	import { onMount } from 'svelte';
	import { api } from '$lib/api';
	import { toast } from '$lib/toast';
	import { toDateInputValue } from '$lib/date';
	import { actualizarDoctorSchema, crearDoctorSchema } from '$lib/validation/schemas';
	import { mapZodErrors } from '$lib/validation/utils';
	import type { DoctorDTO, EspecialidadDTO } from '$lib/types';
	import { defaultBooleanOptions, findLabel } from '$lib/types';
	import ConfirmModal from '$lib/components/ConfirmModal.svelte';
	import SkeletonLoader from '$lib/components/SkeletonLoader.svelte';

	let doctores: DoctorDTO[] = $state([]);
	let especialidades: EspecialidadDTO[] = $state([]);
	let loading = $state(true);
	let editing = $state(false);
	let submitting = $state(false);
	let fieldErrors: Record<string, string> = $state({});
	let searchQuery = $state('');
	let confirmOpen = $state(false);
	let confirmLoading = $state(false);
	let pendingDeleteId = $state<number | null>(null);

	let form = $state({
		Id: 0, Nombre: '', Descripcion: '', IdEspecialidad: 0,
		Telefono: '', Email: '', FechaContratacion: toDateInputValue(new Date()), Activo: true
	});

	const resetForm = () => {
		form = { Id: 0, Nombre: '', Descripcion: '', IdEspecialidad: especialidades[0]?.Id ?? 0,
			Telefono: '', Email: '', FechaContratacion: toDateInputValue(new Date()), Activo: true };
		editing = false;
		fieldErrors = {};
	};

	const loadData = async () => {
		loading = true;
		const [dr, er] = await Promise.all([api.getDoctores(), api.getEspecialidades()]);
		if (dr.Exitoso && dr.Datos) doctores = dr.Datos;
		else toast.error(dr.Mensaje || 'Error al cargar doctores');
		if (er.Exitoso && er.Datos) especialidades = er.Datos;
		if (!editing) form.IdEspecialidad = especialidades[0]?.Id ?? 0;
		loading = false;
	};

	const editarDoctor = (d: DoctorDTO) => {
		editing = true; fieldErrors = {};
		form = { Id: d.Id, Nombre: d.Nombre, Descripcion: d.Descripcion, IdEspecialidad: d.IdEspecialidad,
			Telefono: d.Telefono, Email: d.Email, FechaContratacion: toDateInputValue(d.FechaContratacion), Activo: d.Activo };
		window.scrollTo({ top: 0, behavior: 'smooth' });
	};

	const solicitarEliminar = (id: number) => { pendingDeleteId = id; confirmOpen = true; };

	const confirmarEliminar = async () => {
		if (!pendingDeleteId) return;
		confirmLoading = true;
		const resultado = await api.eliminarDoctor(pendingDeleteId);
		confirmLoading = false; confirmOpen = false; pendingDeleteId = null;
		if (resultado.Exitoso) {
			toast.success('Doctor eliminado');
			await loadData(); resetForm();
		} else toast.error(resultado.Mensaje || 'Error al eliminar');
	};

	const submit = async () => {
		fieldErrors = {}; submitting = true;
		try {
			const validation = (editing ? actualizarDoctorSchema : crearDoctorSchema).safeParse({
				Id: form.Id, Nombre: form.Nombre, Descripcion: form.Descripcion,
				IdEspecialidad: Number(form.IdEspecialidad), Telefono: form.Telefono,
				Email: form.Email, FechaContratacion: form.FechaContratacion, Activo: form.Activo
			});
			if (!validation.success) { fieldErrors = mapZodErrors(validation.error); return; }

			const payload = { Nombre: form.Nombre, Descripcion: form.Descripcion,
				IdEspecialidad: Number(form.IdEspecialidad), Telefono: form.Telefono,
				Email: form.Email, FechaContratacion: form.FechaContratacion, Activo: form.Activo };
			const resultado = editing
				? await api.actualizarDoctor({ ...payload, Id: form.Id })
				: await api.crearDoctor(payload);

			if (resultado.Exitoso) {
				toast.success(editing ? 'Doctor actualizado' : 'Doctor creado');
				await loadData(); resetForm();
			} else {
				toast.error(resultado.Mensaje || 'Error al guardar');
				(resultado.Errores ?? []).forEach((e) => toast.error(e));
			}
		} finally { submitting = false; }
	};

	let filtered = $derived(
		doctores.filter((d) => {
			if (!searchQuery) return true;
			const q = searchQuery.toLowerCase();
			return d.Nombre.toLowerCase().includes(q) || d.Email.toLowerCase().includes(q)
				|| d.NombreEspecialidad.toLowerCase().includes(q);
		})
	);

	onMount(loadData);
</script>

<svelte:head><title>Doctores — ECE-FIAD</title></svelte:head>

<ConfirmModal
	open={confirmOpen}
	title="Eliminar doctor"
	message="¿Deseas eliminar este doctor? Esta acción no se puede deshacer."
	confirmLabel="Sí, eliminar"
	danger={true}
	loading={confirmLoading}
	onconfirm={confirmarEliminar}
	oncancel={() => { confirmOpen = false; pendingDeleteId = null; }}
/>

<div class="page-header-bar">
	<div class="page-header-info">
		<div class="page-icon" style="background: linear-gradient(135deg, #059669, #10b981);">
			<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 7h-9"/><path d="M14 17H5"/><circle cx="17" cy="17" r="3"/><circle cx="7" cy="7" r="3"/></svg>
		</div>
		<div>
			<h1 class="page-title">Doctores</h1>
			<p class="page-subtitle">Gestión del equipo médico</p>
		</div>
	</div>
</div>

<div class="form-section">
	<div class="form-section__header">
		<span class="form-section__title">{editing ? '✏️ Editar doctor' : '➕ Nuevo doctor'}</span>
		{#if editing}<span class="form-section__badge">Modo edición</span>{/if}
	</div>
	<div class="form-grid">
		<label class="field">
			<span>Nombre *</span>
			<input bind:value={form.Nombre} placeholder="Nombre completo" class:field--error={fieldErrors.Nombre} />
			{#if fieldErrors.Nombre}<span class="error-text">⚠ {fieldErrors.Nombre}</span>{/if}
		</label>
		<label class="field">
			<span>Especialidad *</span>
			<select bind:value={form.IdEspecialidad} class:field--error={fieldErrors.IdEspecialidad}>
				{#each especialidades as esp}
					<option value={esp.Id}>{esp.Nombre}</option>
				{/each}
			</select>
			{#if fieldErrors.IdEspecialidad}<span class="error-text">⚠ {fieldErrors.IdEspecialidad}</span>{/if}
		</label>
		<label class="field">
			<span>Email *</span>
			<input type="email" bind:value={form.Email} placeholder="doctor@hospital.com" class:field--error={fieldErrors.Email} />
			{#if fieldErrors.Email}<span class="error-text">⚠ {fieldErrors.Email}</span>{/if}
		</label>
		<label class="field">
			<span>Teléfono</span>
			<input bind:value={form.Telefono} placeholder="+593 999 000 000" />
		</label>
		<label class="field">
			<span>Fecha contratación</span>
			<input type="date" bind:value={form.FechaContratacion} class:field--error={fieldErrors.FechaContratacion} />
			{#if fieldErrors.FechaContratacion}<span class="error-text">⚠ {fieldErrors.FechaContratacion}</span>{/if}
		</label>
		<label class="field">
			<span>Estado</span>
			<select bind:value={form.Activo}>
				{#each defaultBooleanOptions as option}
					<option value={option.value}>{option.label}</option>
				{/each}
			</select>
		</label>
		<label class="field" style="grid-column: 1 / -1;">
			<span>Descripción / Perfil</span>
			<textarea bind:value={form.Descripcion} placeholder="Especialización, años de experiencia, etc."></textarea>
			{#if fieldErrors.Descripcion}<span class="error-text">⚠ {fieldErrors.Descripcion}</span>{/if}
		</label>
	</div>
	<div class="actions">
		<button class="btn btn-primary" onclick={submit} disabled={submitting}
			style="background: linear-gradient(135deg,#059669,#10b981); border-color:transparent;">
			{#if submitting}<span class="spinner"></span>{/if}
			{submitting ? 'Guardando...' : editing ? 'Guardar cambios' : 'Crear doctor'}
		</button>
		{#if editing}
			<button class="btn btn-secondary" onclick={resetForm} disabled={submitting}>Cancelar edición</button>
		{/if}
	</div>
</div>

<div class="card">
	<div class="section-header">
		<div>
			<h2 class="section-title">Equipo médico</h2>
			{#if !loading}<span class="record-count">{filtered.length} de {doctores.length} registros</span>{/if}
		</div>
		<div class="search-box">
			<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="#94a3b8" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
			<input class="search-input" bind:value={searchQuery} placeholder="Buscar doctor..." />
		</div>
	</div>

	{#if loading}
		<SkeletonLoader rows={5} cols={4} />
	{:else}
		<div class="table-container">
			<table class="table">
				<thead>
					<tr>
						<th>Doctor</th>
						<th>Especialidad</th>
						<th>Email</th>
						<th>Estado</th>
						<th>Acciones</th>
					</tr>
				</thead>
				<tbody>
					{#if filtered.length === 0}
						<tr><td colspan="5">
							<div class="empty-state">
								<div class="empty-state__icon">
									<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M20 7h-9"/><path d="M14 17H5"/><circle cx="17" cy="17" r="3"/><circle cx="7" cy="7" r="3"/></svg>
								</div>
								<div class="empty-state__title">No hay doctores registrados</div>
							</div>
						</td></tr>
					{:else}
						{#each filtered as doc}
							<tr>
								<td>
									<div style="display:flex;align-items:center;gap:0.65rem;">
										<div class="avatar" style="background:linear-gradient(135deg,#d1fae5,#a7f3d0);color:#065f46;">{doc.Nombre.split(' ').map((n: string) => n[0]).slice(0,2).join('')}</div>
										<div>
											<div style="font-weight:600;color:#0f172a;font-size:0.875rem;">Dr. {doc.Nombre}</div>
											{#if doc.Telefono}<div style="font-size:0.775rem;color:#94a3b8;">{doc.Telefono}</div>{/if}
										</div>
									</div>
								</td>
								<td>
									<span class="esp-pill">{doc.NombreEspecialidad}</span>
								</td>
								<td style="font-size:0.875rem;color:#475569;">{doc.Email}</td>
								<td><span class={doc.Activo ? 'badge badge--active' : 'badge badge--inactive'}>{findLabel(defaultBooleanOptions, doc.Activo)}</span></td>
								<td>
									<div class="table-actions">
										<button class="btn btn-sm btn-secondary" onclick={() => editarDoctor(doc)}>
											<svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>
											Editar
										</button>
										<button class="btn btn-sm btn-danger" aria-label="Eliminar" title="Eliminar" onclick={() => solicitarEliminar(doc.Id)}>
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
	.page-header-bar { display:flex; align-items:center; justify-content:space-between; margin-bottom:1.5rem; flex-wrap:wrap; gap:1rem; }
	.page-header-info { display:flex; align-items:center; gap:1rem; }
	.page-icon { width:50px; height:50px; border-radius:14px; display:grid; place-items:center; box-shadow:0 4px 14px rgba(0,0,0,0.15); flex-shrink:0; }
	.page-title { font-size:1.6rem; font-weight:800; color:#0f172a; margin:0; letter-spacing:-0.03em; }
	.page-subtitle { font-size:0.85rem; color:#64748b; margin:0.2rem 0 0; }
	.search-box { display:flex; align-items:center; gap:0.5rem; background:#f8fafc; border:1.5px solid #e2e8f0; border-radius:0.6rem; padding:0.45rem 0.85rem; transition:border-color 0.15s,box-shadow 0.15s; }
	.search-box:focus-within { border-color:#059669; box-shadow:0 0 0 3px rgba(5,150,105,0.1); background:#fff; }
	.search-input { border:none; background:none; outline:none; font-size:0.875rem; color:#0f172a; width:200px; }
	.avatar { width:34px; height:34px; border-radius:50%; font-size:0.72rem; font-weight:700; display:grid; place-items:center; flex-shrink:0; text-transform:uppercase; }
	.esp-pill { display:inline-flex; align-items:center; padding:0.2rem 0.65rem; border-radius:0.35rem; background:#f0fdf4; color:#15803d; font-size:0.78rem; font-weight:600; }
</style>
