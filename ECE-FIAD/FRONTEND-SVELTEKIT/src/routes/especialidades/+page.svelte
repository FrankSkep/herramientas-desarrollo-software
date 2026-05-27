<script lang="ts">
	import { onMount } from 'svelte';
	import { api } from '$lib/api';
	import { toast } from '$lib/toast';
	import { actualizarEspecialidadSchema, crearEspecialidadSchema } from '$lib/validation/schemas';
	import { mapZodErrors } from '$lib/validation/utils';
	import type { EspecialidadDTO } from '$lib/types';
	import { defaultBooleanOptions, findLabel } from '$lib/types';
	import ConfirmModal from '$lib/components/ConfirmModal.svelte';
	import SkeletonLoader from '$lib/components/SkeletonLoader.svelte';

	let especialidades: EspecialidadDTO[] = $state([]);
	let loading = $state(true);
	let editing = $state(false);
	let submitting = $state(false);
	let fieldErrors: Record<string, string> = $state({});
	let searchQuery = $state('');
	let confirmOpen = $state(false);
	let confirmLoading = $state(false);
	let pendingDeleteId = $state<number | null>(null);

	let form = $state({ Id: 0, Nombre: '', Descripcion: '', Activo: true });

	const resetForm = () => {
		form = { Id: 0, Nombre: '', Descripcion: '', Activo: true };
		editing = false;
		fieldErrors = {};
	};

	const loadData = async () => {
		loading = true;
		const resultado = await api.getEspecialidades();
		if (resultado.Exitoso && resultado.Datos) {
			especialidades = resultado.Datos;
		} else {
			toast.error(resultado.Mensaje || 'Error al cargar especialidades');
		}
		loading = false;
	};

	const editarEspecialidad = (e: EspecialidadDTO) => {
		editing = true;
		fieldErrors = {};
		form = { Id: e.Id, Nombre: e.Nombre, Descripcion: e.Descripcion ?? '', Activo: e.Activo };
		window.scrollTo({ top: 0, behavior: 'smooth' });
	};

	const solicitarEliminar = (id: number) => { pendingDeleteId = id; confirmOpen = true; };

	const confirmarEliminar = async () => {
		if (!pendingDeleteId) return;
		confirmLoading = true;
		const resultado = await api.eliminarEspecialidad(pendingDeleteId);
		confirmLoading = false;
		confirmOpen = false;
		pendingDeleteId = null;
		if (resultado.Exitoso) {
			toast.success('Especialidad eliminada');
			await loadData();
			resetForm();
		} else {
			toast.error(resultado.Mensaje || 'Error al eliminar');
		}
	};

	const submit = async () => {
		fieldErrors = {};
		submitting = true;
		try {
			const validation = (editing ? actualizarEspecialidadSchema : crearEspecialidadSchema).safeParse({
				Id: form.Id, Nombre: form.Nombre, Descripcion: form.Descripcion, Activo: form.Activo
			});
			if (!validation.success) { fieldErrors = mapZodErrors(validation.error); return; }

			const payload = { Nombre: form.Nombre, Descripcion: form.Descripcion, Activo: form.Activo };
			const resultado = editing
				? await api.actualizarEspecialidad({ ...payload, Id: form.Id })
				: await api.crearEspecialidad(payload);

			if (resultado.Exitoso) {
				toast.success(editing ? 'Especialidad actualizada' : 'Especialidad creada');
				await loadData();
				resetForm();
			} else {
				toast.error(resultado.Mensaje || 'Error al guardar');
			}
		} finally {
			submitting = false;
		}
	};

	let filtered = $derived(
		especialidades.filter((e) => {
			if (!searchQuery) return true;
			const q = searchQuery.toLowerCase();
			return e.Nombre.toLowerCase().includes(q) || (e.Descripcion ?? '').toLowerCase().includes(q);
		})
	);

	onMount(loadData);
</script>

<svelte:head><title>Especialidades — ECE-FIAD</title></svelte:head>

<ConfirmModal
	open={confirmOpen}
	title="Eliminar especialidad"
	message="¿Deseas eliminar esta especialidad? Esta acción no se puede deshacer."
	confirmLabel="Sí, eliminar"
	danger={true}
	loading={confirmLoading}
	onconfirm={confirmarEliminar}
	oncancel={() => { confirmOpen = false; pendingDeleteId = null; }}
/>

<div class="page-header-bar">
	<div class="page-header-info">
		<div class="page-icon" style="background: linear-gradient(135deg, #7c3aed, #8b5cf6);">
			<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="2" y="3" width="20" height="14" rx="2"/><line x1="8" y1="21" x2="16" y2="21"/><line x1="12" y1="17" x2="12" y2="21"/></svg>
		</div>
		<div>
			<h1 class="page-title">Especialidades</h1>
			<p class="page-subtitle">Catálogo de especialidades médicas</p>
		</div>
	</div>
</div>

<div class="form-section">
	<div class="form-section__header">
		<span class="form-section__title">{editing ? '✏️ Editar especialidad' : '➕ Nueva especialidad'}</span>
		{#if editing}<span class="form-section__badge">Modo edición</span>{/if}
	</div>
	<div class="form-grid">
		<label class="field">
			<span>Nombre *</span>
			<input bind:value={form.Nombre} placeholder="Ej. Cardiología" class:field--error={fieldErrors.Nombre} />
			{#if fieldErrors.Nombre}<span class="error-text">⚠ {fieldErrors.Nombre}</span>{/if}
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
			<span>Descripción</span>
			<textarea bind:value={form.Descripcion} placeholder="Descripción de la especialidad..."></textarea>
			{#if fieldErrors.Descripcion}<span class="error-text">⚠ {fieldErrors.Descripcion}</span>{/if}
		</label>
	</div>
	<div class="actions">
		<button class="btn btn-primary" onclick={submit} disabled={submitting}>
			{#if submitting}<span class="spinner"></span>{/if}
			{submitting ? 'Guardando...' : editing ? 'Guardar cambios' : 'Crear especialidad'}
		</button>
		{#if editing}
			<button class="btn btn-secondary" onclick={resetForm} disabled={submitting}>Cancelar edición</button>
		{/if}
	</div>
</div>

<div class="card">
	<div class="section-header">
		<div>
			<h2 class="section-title">Listado de especialidades</h2>
			{#if !loading}<span class="record-count">{filtered.length} de {especialidades.length} registros</span>{/if}
		</div>
		<div class="search-box">
			<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="#94a3b8" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
			<input class="search-input" bind:value={searchQuery} placeholder="Buscar..." />
		</div>
	</div>

	{#if loading}
		<SkeletonLoader rows={5} cols={4} />
	{:else}
		<div class="table-container">
			<table class="table">
				<thead>
					<tr>
						<th>Especialidad</th>
						<th>Descripción</th>
						<th>Médicos</th>
						<th>Estado</th>
						<th>Acciones</th>
					</tr>
				</thead>
				<tbody>
					{#if filtered.length === 0}
						<tr><td colspan="5">
							<div class="empty-state">
								<div class="empty-state__icon">
									<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><rect x="2" y="3" width="20" height="14" rx="2"/></svg>
								</div>
								<div class="empty-state__title">No hay especialidades registradas</div>
							</div>
						</td></tr>
					{:else}
						{#each filtered as esp}
							<tr>
								<td>
									<div style="display:flex;align-items:center;gap:0.6rem;">
										<div class="esp-dot" style="background: hsl({(esp.Id * 47) % 360}, 65%, 55%)"></div>
										<span style="font-weight:600;color:#0f172a;">{esp.Nombre}</span>
									</div>
								</td>
								<td style="color:#64748b;font-size:0.875rem;">{esp.Descripcion || '—'}</td>
								<td>
									<span class="medicos-badge">{esp.CantidadMedicos} médico{esp.CantidadMedicos !== 1 ? 's' : ''}</span>
								</td>
								<td><span class={esp.Activo ? 'badge badge--active' : 'badge badge--inactive'}>{findLabel(defaultBooleanOptions, esp.Activo)}</span></td>
								<td>
									<div class="table-actions">
										<button class="btn btn-sm btn-secondary" onclick={() => editarEspecialidad(esp)}>
											<svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>
											Editar
										</button>
										<button class="btn btn-sm btn-danger" onclick={() => solicitarEliminar(esp.Id)}>
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
	.search-box:focus-within { border-color:#7c3aed; box-shadow:0 0 0 3px rgba(124,58,237,0.1); background:#fff; }
	.search-input { border:none; background:none; outline:none; font-size:0.875rem; color:#0f172a; width:200px; }
	.esp-dot { width:10px; height:10px; border-radius:50%; flex-shrink:0; }
	.medicos-badge { display:inline-flex; align-items:center; padding:0.2rem 0.6rem; border-radius:0.35rem; background:#eff6ff; color:#2563eb; font-size:0.775rem; font-weight:600; }
</style>
