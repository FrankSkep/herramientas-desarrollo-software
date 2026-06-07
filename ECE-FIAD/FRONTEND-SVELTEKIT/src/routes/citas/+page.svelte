<script lang="ts">
	import { onMount } from 'svelte';
	import { api } from '$lib/api';
	import { toast } from '$lib/toast';
	import { toDateTimeLocalValue, normalizeDateTimeLocal } from '$lib/date';
	import { actualizarCitaSchema, crearCitaSchema } from '$lib/validation/schemas';
	import { mapZodErrors } from '$lib/validation/utils';
	import type { CitaDTO, DoctorDTO, PacienteDTO } from '$lib/types';
	import { estadoCitaOptions, findLabel } from '$lib/types';
	import ConfirmModal from '$lib/components/ConfirmModal.svelte';
	import SkeletonLoader from '$lib/components/SkeletonLoader.svelte';

	let citas: CitaDTO[] = $state([]);
	let pacientes: PacienteDTO[] = $state([]);
	let doctores: DoctorDTO[] = $state([]);
	let loading = $state(true);
	let editing = $state(false);
	let submitting = $state(false);
	let formVisible = $state(false);
	let fieldErrors: Record<string, string> = $state({});
	let searchQuery = $state('');
	let filtroEstado = $state('');
	let filtroFecha = $state('');
	let confirmOpen = $state(false);
	let confirmLoading = $state(false);
	let pendingDeleteId = $state<number | null>(null);

	let form = $state({
		Id: 0, IdPaciente: 0, IdDoctor: 0,
		FechaHora: toDateTimeLocalValue(new Date()), Motivo: '', Notas: '', Estado: 1
	});

	const resetForm = () => {
		form = { Id: 0, IdPaciente: pacientes[0]?.Id ?? 0, IdDoctor: doctores[0]?.Id ?? 0,
			FechaHora: toDateTimeLocalValue(new Date()), Motivo: '', Notas: '', Estado: 1 };
		editing = false; fieldErrors = {}; formVisible = false;
	};

	const loadData = async () => {
		loading = true;
		const [cr, pr, dr] = await Promise.all([api.getCitas(), api.getPacientes(), api.getDoctores()]);
		if (cr.Exitoso && cr.Datos) citas = cr.Datos;
		else toast.error(cr.Mensaje || 'Error al cargar citas');
		if (pr.Exitoso && pr.Datos) pacientes = pr.Datos;
		if (dr.Exitoso && dr.Datos) doctores = dr.Datos;
		if (!editing) { form.IdPaciente = pacientes[0]?.Id ?? 0; form.IdDoctor = doctores[0]?.Id ?? 0; }
		loading = false;
	};

	const editarCita = (cita: CitaDTO) => {
		editing = true; fieldErrors = {}; formVisible = true;
		form = { Id: cita.Id, IdPaciente: cita.IdPaciente, IdDoctor: cita.IdDoctor,
			FechaHora: toDateTimeLocalValue(cita.FechaHora), Motivo: cita.Motivo,
			Notas: cita.Notas ?? '', Estado: cita.Estado };
		window.scrollTo({ top: 0, behavior: 'smooth' });
	};

	const solicitarEliminar = (id: number) => { pendingDeleteId = id; confirmOpen = true; };

	const confirmarEliminar = async () => {
		if (!pendingDeleteId) return;
		confirmLoading = true;
		const resultado = await api.eliminarCita(pendingDeleteId);
		confirmLoading = false; confirmOpen = false; pendingDeleteId = null;
		if (resultado.Exitoso) {
			toast.success('Cita eliminada');
			await loadData(); resetForm();
		} else toast.error(resultado.Mensaje || 'Error al eliminar');
	};

	const submit = async () => {
		fieldErrors = {}; submitting = true;
		try {
			const validation = (editing ? actualizarCitaSchema : crearCitaSchema).safeParse({
				Id: form.Id, IdPaciente: Number(form.IdPaciente), IdDoctor: Number(form.IdDoctor),
				FechaHora: form.FechaHora, Motivo: form.Motivo, Notas: form.Notas, Estado: Number(form.Estado)
			});
			if (!validation.success) { fieldErrors = mapZodErrors(validation.error); return; }

			const fechaHora = normalizeDateTimeLocal(form.FechaHora);
			const disponible = await api.estaDisponible(Number(form.IdDoctor), fechaHora, editing ? form.Id : undefined);
			if (!disponible) {
				fieldErrors = { FechaHora: 'El doctor no está disponible en ese horario.' };
				toast.warning('El doctor no está disponible en ese horario');
				return;
			}

			const payload = { IdPaciente: Number(form.IdPaciente), IdDoctor: Number(form.IdDoctor),
				FechaHora: fechaHora, Motivo: form.Motivo, Notas: form.Notas, Estado: Number(form.Estado) };
			const resultado = editing
				? await api.actualizarCita({ ...payload, Id: form.Id })
				: await api.crearCita(payload);

			if (resultado.Exitoso) {
				toast.success(editing ? 'Cita actualizada' : 'Cita agendada correctamente');
				await loadData(); resetForm();
			} else {
				toast.error(resultado.Mensaje || 'Error al guardar');
				(resultado.Errores ?? []).forEach((e) => toast.error(e));
			}
		} finally { submitting = false; }
	};

	const estadoBadgeClass = (estado: number) => {
		const map: Record<number, string> = { 1: 'badge--pending', 2: 'badge--confirmed', 3: 'badge--cancelled', 4: 'badge--completed', 5: 'badge--noshow' };
		return `badge ${map[estado] ?? 'badge--default'}`;
	};

	let filtered = $derived(
		citas.filter((c) => {
			const q = searchQuery.toLowerCase();
			const matchSearch = !searchQuery || c.NombrePaciente.toLowerCase().includes(q) || c.NombreDoctor.toLowerCase().includes(q) || c.Motivo.toLowerCase().includes(q);
			const matchEstado = !filtroEstado || String(c.Estado) === filtroEstado;
			const matchFecha = !filtroFecha || c.FechaHora.startsWith(filtroFecha);
			return matchSearch && matchEstado && matchFecha;
		})
	);

	const formatFecha = (f: string) => {
		try { return new Intl.DateTimeFormat('es-EC', { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(f)); }
		catch { return f; }
	};

	onMount(loadData);
</script>

<svelte:head><title>Citas — ECE-FIAD</title></svelte:head>

<ConfirmModal
	open={confirmOpen}
	title="Cancelar cita"
	message="¿Deseas eliminar esta cita? Esta acción no se puede deshacer."
	confirmLabel="Sí, eliminar"
	danger={true}
	loading={confirmLoading}
	onconfirm={confirmarEliminar}
	oncancel={() => { confirmOpen = false; pendingDeleteId = null; }}
/>

<div class="page-header-bar">
	<div class="page-header-info">
		<div class="page-icon" style="background: linear-gradient(135deg, #d97706, #f59e0b);">
			<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
		</div>
		<div>
			<h1 class="page-title">Citas</h1>
			<p class="page-subtitle">Agenda y programación de consultas</p>
		</div>
	</div>
	<button class="btn btn-primary" onclick={() => { resetForm(); formVisible = true; }}>
		+ Nueva cita
	</button>
</div>

{#if formVisible}
<div class="form-section">
	<div class="form-section__header">
		<span class="form-section__title">{editing ? '✏️ Editar cita' : '📅 Agendar nueva cita'}</span>
		{#if editing}<span class="form-section__badge">Modo edición</span>{/if}
	</div>
	<div class="form-grid">
		<label class="field">
			<span>Paciente *</span>
			<select bind:value={form.IdPaciente} class:field--error={fieldErrors.IdPaciente}>
				{#each pacientes as p}
					<option value={p.Id}>{p.Nombres} {p.Apellidos}</option>
				{/each}
			</select>
			{#if fieldErrors.IdPaciente}<span class="error-text">⚠ {fieldErrors.IdPaciente}</span>{/if}
		</label>
		<label class="field">
			<span>Doctor *</span>
			<select bind:value={form.IdDoctor} class:field--error={fieldErrors.IdDoctor}>
				{#each doctores as d}
					<option value={d.Id}>{d.Nombre}</option>
				{/each}
			</select>
			{#if fieldErrors.IdDoctor}<span class="error-text">⚠ {fieldErrors.IdDoctor}</span>{/if}
		</label>
		<label class="field">
			<span>Fecha y hora *</span>
			<input type="datetime-local" bind:value={form.FechaHora} class:field--error={fieldErrors.FechaHora} />
			{#if fieldErrors.FechaHora}<span class="error-text">⚠ {fieldErrors.FechaHora}</span>{/if}
		</label>
		<label class="field">
			<span>Estado</span>
			<select bind:value={form.Estado}>
				{#each estadoCitaOptions as option}
					<option value={option.value}>{option.label}</option>
				{/each}
			</select>
		</label>
		<label class="field" style="grid-column: 1 / -1;">
			<span>Motivo *</span>
			<textarea bind:value={form.Motivo} placeholder="Motivo de la consulta..." class:field--error={fieldErrors.Motivo}></textarea>
			{#if fieldErrors.Motivo}<span class="error-text">⚠ {fieldErrors.Motivo}</span>{/if}
		</label>
		<label class="field" style="grid-column: 1 / -1;">
			<span>Notas adicionales</span>
			<textarea bind:value={form.Notas} placeholder="Observaciones, instrucciones previas, etc."></textarea>
		</label>
	</div>
	<div class="actions">
		<button class="btn btn-primary" onclick={submit} disabled={submitting}
			style="background: linear-gradient(135deg,#d97706,#f59e0b); border-color:transparent;">
			{#if submitting}<span class="spinner"></span>{/if}
			{submitting ? 'Guardando...' : editing ? 'Guardar cambios' : 'Agendar cita'}
		</button>
		<button class="btn btn-secondary" onclick={resetForm} disabled={submitting}>Cancelar</button>
	</div>
</div>
{/if}

<div class="card">
	<div class="section-header">
		<div>
			<h2 class="section-title">Agenda de citas</h2>
			{#if !loading}<span class="record-count">{filtered.length} de {citas.length} citas</span>{/if}
		</div>
		<div style="display:flex;gap:0.5rem;flex-wrap:wrap;">
			<select class="filter-select" bind:value={filtroEstado}>
				<option value="">Todos los estados</option>
				{#each estadoCitaOptions as opt}
					<option value={String(opt.value)}>{opt.label}</option>
				{/each}
			</select>
			<input type="date" class="filter-select" bind:value={filtroFecha} style="padding: 0.5rem; border: 1px solid #e2e8f0; border-radius: 0.375rem;" />
			<div class="search-box">
				<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="#94a3b8" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
				<input class="search-input" bind:value={searchQuery} placeholder="Buscar cita..." />
			</div>
		</div>
	</div>

	{#if loading}
		<SkeletonLoader rows={6} cols={4} />
	{:else}
		<div class="table-container">
			<table class="table">
				<thead>
					<tr>
						<th>Paciente</th>
						<th>Doctor</th>
						<th>Fecha y hora</th>
						<th>Motivo</th>
						<th>Estado</th>
						<th>Acciones</th>
					</tr>
				</thead>
				<tbody>
					{#if filtered.length === 0}
						<tr><td colspan="6">
							<div class="empty-state">
								<div class="empty-state__icon">
									<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
								</div>
								<div class="empty-state__title">{filtroEstado || searchQuery ? 'Sin citas con esos filtros' : 'No hay citas agendadas'}</div>
							</div>
						</td></tr>
					{:else}
						{#each filtered as cita}
							<tr onclick={() => editarCita(cita)} style="cursor: pointer;" class="clickable-row">
								<td>
									<div style="font-weight:600;color:#0f172a;font-size:0.875rem;">{cita.NombrePaciente}</div>
								</td>
								<td>
									<div style="font-size:0.875rem;color:#475569;">Dr. {cita.NombreDoctor}</div>
								</td>
								<td>
									<div class="fecha-cell">
										<svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="#94a3b8" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
										{formatFecha(cita.FechaHora)}
									</div>
								</td>
								<td style="font-size:0.85rem;color:#64748b;max-width:180px;">
									<div class="truncate">{cita.Motivo}</div>
								</td>
								<td><span class={estadoBadgeClass(cita.Estado)}>{findLabel(estadoCitaOptions, cita.Estado)}</span></td>
								<td>
									<div class="table-actions">
										<button class="btn btn-sm btn-secondary">
											<svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>
											Editar
										</button>
										<button class="btn btn-sm btn-danger" aria-label="Eliminar" title="Eliminar" onclick={(e) => { e.stopPropagation(); solicitarEliminar(cita.Id); }}>
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
	.search-box:focus-within { border-color:#d97706; box-shadow:0 0 0 3px rgba(217,119,6,0.1); background:#fff; }
	.search-input { border:none; background:none; outline:none; font-size:0.875rem; color:#0f172a; width:200px; }
	.filter-select { border:1.5px solid #e2e8f0; border-radius:0.6rem; padding:0.45rem 0.75rem; font-size:0.875rem; background:#f8fafc; outline:none; cursor:pointer; }
	.fecha-cell { display:flex; align-items:center; gap:0.4rem; font-size:0.875rem; color:#334155; white-space:nowrap; }
	.truncate { overflow:hidden; text-overflow:ellipsis; white-space:nowrap; }
	.clickable-row:hover { background-color: #f1f5f9; }
</style>
