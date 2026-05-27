<script lang="ts">
	import { onMount } from 'svelte';
	import { api } from '$lib/api';
	import { toast } from '$lib/toast';
	import { toDateInputValue } from '$lib/date';
	import { actualizarHistoriaSchema, crearHistoriaSchema } from '$lib/validation/schemas';
	import { mapZodErrors } from '$lib/validation/utils';
	import type { HistoriaClinicaDTO, PacienteDTO } from '$lib/types';
	import { defaultBooleanOptions, findLabel } from '$lib/types';
	import ConfirmModal from '$lib/components/ConfirmModal.svelte';
	import SkeletonLoader from '$lib/components/SkeletonLoader.svelte';

	let historias: HistoriaClinicaDTO[] = $state([]);
	let pacientes: PacienteDTO[] = $state([]);
	let pacientesSinHistoria: PacienteDTO[] = $state([]);
	let loading = $state(true);
	let editing = $state(false);
	let submitting = $state(false);
	let fieldErrors: Record<string, string> = $state({});
	let searchQuery = $state('');
	let confirmOpen = $state(false);
	let confirmLoading = $state(false);
	let pendingDeleteId = $state<number | null>(null);

	let form = $state({
		Id: 0, IdPaciente: 0, FechaApertura: toDateInputValue(new Date()),
		Alergias: '', AntecedentesFamiliares: '', AntecedentesPersonales: '', Activo: true
	});

	const resetForm = () => {
		form = { Id: 0, IdPaciente: pacientesSinHistoria[0]?.Id ?? 0,
			FechaApertura: toDateInputValue(new Date()),
			Alergias: '', AntecedentesFamiliares: '', AntecedentesPersonales: '', Activo: true };
		editing = false; fieldErrors = {};
	};

	const loadData = async () => {
		loading = true;
		const [hr, pr, sr] = await Promise.all([api.getHistorias(), api.getPacientes(), api.getPacientesActivosSinHistoria()]);
		if (hr.Exitoso && hr.Datos) historias = hr.Datos;
		else toast.error(hr.Mensaje || 'Error al cargar historias');
		if (pr.Exitoso && pr.Datos) pacientes = pr.Datos;
		if (sr.Exitoso && sr.Datos) pacientesSinHistoria = sr.Datos;
		if (!editing) form.IdPaciente = pacientesSinHistoria[0]?.Id ?? 0;
		loading = false;
	};

	const editarHistoria = (h: HistoriaClinicaDTO) => {
		editing = true; fieldErrors = {};
		form = { Id: h.Id, IdPaciente: h.IdPaciente, FechaApertura: toDateInputValue(h.FechaApertura),
			Alergias: h.Alergias ?? '', AntecedentesFamiliares: h.AntecedentesFamiliares ?? '',
			AntecedentesPersonales: h.AntecedentesPersonales ?? '', Activo: h.Activo };
		window.scrollTo({ top: 0, behavior: 'smooth' });
	};

	const solicitarEliminar = (id: number) => { pendingDeleteId = id; confirmOpen = true; };

	const confirmarEliminar = async () => {
		if (!pendingDeleteId) return;
		confirmLoading = true;
		const resultado = await api.eliminarHistoria(pendingDeleteId);
		confirmLoading = false; confirmOpen = false; pendingDeleteId = null;
		if (resultado.Exitoso) {
			toast.success('Historia clínica eliminada');
			await loadData(); resetForm();
		} else toast.error(resultado.Mensaje || 'Error al eliminar');
	};

	const submit = async () => {
		fieldErrors = {}; submitting = true;
		try {
			const validation = (editing ? actualizarHistoriaSchema : crearHistoriaSchema).safeParse({
				Id: form.Id, IdPaciente: Number(form.IdPaciente), FechaApertura: form.FechaApertura,
				Alergias: form.Alergias, AntecedentesFamiliares: form.AntecedentesFamiliares,
				AntecedentesPersonales: form.AntecedentesPersonales, Activo: form.Activo
			});
			if (!validation.success) { fieldErrors = mapZodErrors(validation.error); return; }

			if (form.Activo) {
				const existe = await api.existeHistoriaActiva(Number(form.IdPaciente), editing ? form.Id : undefined);
				if (existe) {
					fieldErrors = { IdPaciente: 'El paciente ya tiene una historia clínica activa.' };
					toast.warning('El paciente ya tiene una historia clínica activa');
					return;
				}
			}

			const payload = { IdPaciente: Number(form.IdPaciente), FechaApertura: form.FechaApertura,
				Alergias: form.Alergias, AntecedentesFamiliares: form.AntecedentesFamiliares,
				AntecedentesPersonales: form.AntecedentesPersonales, Activo: form.Activo };
			const resultado = editing
				? await api.actualizarHistoria({ ...payload, Id: form.Id })
				: await api.crearHistoria(payload);

			if (resultado.Exitoso) {
				toast.success(editing ? 'Historia clínica actualizada' : 'Historia clínica creada');
				await loadData(); resetForm();
			} else {
				toast.error(resultado.Mensaje || 'Error al guardar');
				(resultado.Errores ?? []).forEach((e) => toast.error(e));
			}
		} finally { submitting = false; }
	};

	let pacientesDisponibles = $derived(editing ? pacientes : pacientesSinHistoria);
	let filtered = $derived(
		historias.filter((h) => {
			if (!searchQuery) return true;
			return h.NombrePaciente.toLowerCase().includes(searchQuery.toLowerCase());
		})
	);

	const formatDate = (d: string) => {
		try { return new Intl.DateTimeFormat('es-EC', { dateStyle: 'medium' }).format(new Date(d)); }
		catch { return d; }
	};

	onMount(loadData);
</script>

<svelte:head><title>Historias Clínicas — ECE-FIAD</title></svelte:head>

<ConfirmModal
	open={confirmOpen}
	title="Eliminar historia clínica"
	message="¿Deseas eliminar esta historia clínica? Esta acción no se puede deshacer."
	confirmLabel="Sí, eliminar"
	danger={true}
	loading={confirmLoading}
	onconfirm={confirmarEliminar}
	oncancel={() => { confirmOpen = false; pendingDeleteId = null; }}
/>

<div class="page-header-bar">
	<div class="page-header-info">
		<div class="page-icon" style="background: linear-gradient(135deg, #0284c7, #38bdf8);">
			<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="12" y1="18" x2="12" y2="12"/><line x1="9" y1="15" x2="15" y2="15"/></svg>
		</div>
		<div>
			<h1 class="page-title">Historias Clínicas</h1>
			<p class="page-subtitle">Registros médicos por paciente</p>
		</div>
	</div>
</div>

<div class="form-section">
	<div class="form-section__header">
		<span class="form-section__title">{editing ? '✏️ Editar historia' : '📋 Nueva historia clínica'}</span>
		{#if editing}<span class="form-section__badge">Modo edición</span>{/if}
	</div>
	<div class="form-grid">
		<label class="field">
			<span>Paciente *</span>
			<select bind:value={form.IdPaciente} class:field--error={fieldErrors.IdPaciente}>
				{#each pacientesDisponibles as p}
					<option value={p.Id}>{p.Nombres} {p.Apellidos}</option>
				{/each}
			</select>
			{#if fieldErrors.IdPaciente}<span class="error-text">⚠ {fieldErrors.IdPaciente}</span>{/if}
		</label>
		<label class="field">
			<span>Fecha apertura</span>
			<input type="date" bind:value={form.FechaApertura} class:field--error={fieldErrors.FechaApertura} />
			{#if fieldErrors.FechaApertura}<span class="error-text">⚠ {fieldErrors.FechaApertura}</span>{/if}
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
			<span>Alergias</span>
			<textarea bind:value={form.Alergias} placeholder="Alergias conocidas del paciente..."></textarea>
		</label>
		<label class="field" style="grid-column: 1 / -1;">
			<span>Antecedentes familiares</span>
			<textarea bind:value={form.AntecedentesFamiliares} placeholder="Enfermedades hereditarias, antecedentes familiares relevantes..."></textarea>
		</label>
		<label class="field" style="grid-column: 1 / -1;">
			<span>Antecedentes personales</span>
			<textarea bind:value={form.AntecedentesPersonales} placeholder="Enfermedades previas, cirugías, medicación actual..."></textarea>
		</label>
	</div>
	<div class="actions">
		<button class="btn btn-primary" onclick={submit} disabled={submitting}
			style="background: linear-gradient(135deg,#0284c7,#38bdf8); border-color:transparent;">
			{#if submitting}<span class="spinner"></span>{/if}
			{submitting ? 'Guardando...' : editing ? 'Guardar cambios' : 'Crear historia'}
		</button>
		{#if editing}
			<button class="btn btn-secondary" onclick={resetForm} disabled={submitting}>Cancelar edición</button>
		{/if}
	</div>
</div>

<div class="card">
	<div class="section-header">
		<div>
			<h2 class="section-title">Historias registradas</h2>
			{#if !loading}<span class="record-count">{filtered.length} de {historias.length} registros</span>{/if}
		</div>
		<div class="search-box">
			<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="#94a3b8" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
			<input class="search-input" bind:value={searchQuery} placeholder="Buscar paciente..." />
		</div>
	</div>

	{#if loading}
		<SkeletonLoader rows={5} cols={3} />
	{:else}
		<div class="table-container">
			<table class="table">
				<thead>
					<tr>
						<th>Paciente</th>
						<th>Fecha apertura</th>
						<th>Alergias</th>
						<th>Estado</th>
						<th>Acciones</th>
					</tr>
				</thead>
				<tbody>
					{#if filtered.length === 0}
						<tr><td colspan="5">
							<div class="empty-state">
								<div class="empty-state__icon">
									<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z"/><polyline points="14 2 14 8 20 8"/></svg>
								</div>
								<div class="empty-state__title">No hay historias clínicas registradas</div>
							</div>
						</td></tr>
					{:else}
						{#each filtered as h}
							<tr>
								<td>
									<div style="font-weight:600;color:#0f172a;font-size:0.875rem;">{h.NombrePaciente}</div>
								</td>
								<td style="font-size:0.875rem;color:#475569;">{formatDate(h.FechaApertura)}</td>
								<td style="font-size:0.85rem;color:#64748b;max-width:200px;">
									{#if h.Alergias}
										<span class="alergia-pill">{h.Alergias.slice(0, 40)}{h.Alergias.length > 40 ? '…' : ''}</span>
									{:else}
										<span style="color:#cbd5e1;">—</span>
									{/if}
								</td>
								<td><span class={h.Activo ? 'badge badge--active' : 'badge badge--inactive'}>{findLabel(defaultBooleanOptions, h.Activo)}</span></td>
								<td>
									<div class="table-actions">
										<button class="btn btn-sm btn-secondary" onclick={() => editarHistoria(h)}>
											<svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>
											Editar
										</button>
										<button class="btn btn-sm btn-danger" onclick={() => solicitarEliminar(h.Id)}>
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
	.search-box:focus-within { border-color:#0284c7; box-shadow:0 0 0 3px rgba(2,132,199,0.1); background:#fff; }
	.search-input { border:none; background:none; outline:none; font-size:0.875rem; color:#0f172a; width:200px; }
	.alergia-pill { display:inline-flex; align-items:center; padding:0.15rem 0.5rem; border-radius:0.35rem; background:#fef9c3; color:#a16207; font-size:0.775rem; font-weight:500; }
</style>
