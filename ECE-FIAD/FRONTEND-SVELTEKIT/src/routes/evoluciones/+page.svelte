<script lang="ts">
	import { onMount } from 'svelte';
	import { api } from '$lib/api';
	import { toast } from '$lib/toast';
	import { toDateInputValue } from '$lib/date';
	import { actualizarEvolucionSchema, crearEvolucionSchema } from '$lib/validation/schemas';
	import { mapZodErrors } from '$lib/validation/utils';
	import type { DoctorDTO, EvolucionDTO, HistoriaClinicaDTO } from '$lib/types';
	import { defaultBooleanOptions, findLabel } from '$lib/types';
	import ConfirmModal from '$lib/components/ConfirmModal.svelte';
	import SkeletonLoader from '$lib/components/SkeletonLoader.svelte';

	let evoluciones: EvolucionDTO[] = $state([]);
	let doctores: DoctorDTO[] = $state([]);
	let historias: HistoriaClinicaDTO[] = $state([]);
	let loading = $state(true);
	let editing = $state(false);
	let submitting = $state(false);
	let fieldErrors: Record<string, string> = $state({});
	let searchQuery = $state('');
	let confirmOpen = $state(false);
	let confirmLoading = $state(false);
	let pendingDeleteId = $state<number | null>(null);

	let form = $state({
		Id: 0, IdHistoriaClinica: 0, IdDoctor: 0,
		Fecha: toDateInputValue(new Date()), Diagnostico: '', Tratamiento: '', Notas: '', Activo: true
	});

	const resetForm = () => {
		form = { Id: 0, IdHistoriaClinica: historias[0]?.Id ?? 0, IdDoctor: doctores[0]?.Id ?? 0,
			Fecha: toDateInputValue(new Date()), Diagnostico: '', Tratamiento: '', Notas: '', Activo: true };
		editing = false; fieldErrors = {};
	};

	const loadData = async () => {
		loading = true;
		const [er, dr, hr] = await Promise.all([api.getEvoluciones(), api.getDoctores(), api.getHistoriasActivas()]);
		if (er.Exitoso && er.Datos) evoluciones = er.Datos;
		else toast.error(er.Mensaje || 'Error al cargar evoluciones');
		if (dr.Exitoso && dr.Datos) doctores = dr.Datos;
		if (hr.Exitoso && hr.Datos) historias = hr.Datos;
		if (!editing) { form.IdDoctor = doctores[0]?.Id ?? 0; form.IdHistoriaClinica = historias[0]?.Id ?? 0; }
		loading = false;
	};

	const editarEvolucion = (e: EvolucionDTO) => {
		editing = true; fieldErrors = {};
		form = { Id: e.Id, IdHistoriaClinica: e.IdHistoriaClinica, IdDoctor: e.IdDoctor,
			Fecha: toDateInputValue(e.Fecha), Diagnostico: e.Diagnostico,
			Tratamiento: e.Tratamiento, Notas: e.Notas ?? '', Activo: e.Activo };
		window.scrollTo({ top: 0, behavior: 'smooth' });
	};

	const solicitarEliminar = (id: number) => { pendingDeleteId = id; confirmOpen = true; };

	const confirmarEliminar = async () => {
		if (!pendingDeleteId) return;
		confirmLoading = true;
		const resultado = await api.eliminarEvolucion(pendingDeleteId);
		confirmLoading = false; confirmOpen = false; pendingDeleteId = null;
		if (resultado.Exitoso) {
			toast.success('Evolución eliminada');
			await loadData(); resetForm();
		} else toast.error(resultado.Mensaje || 'Error al eliminar');
	};

	const submit = async () => {
		fieldErrors = {}; submitting = true;
		try {
			const validation = (editing ? actualizarEvolucionSchema : crearEvolucionSchema).safeParse({
				Id: form.Id, IdHistoriaClinica: Number(form.IdHistoriaClinica),
				IdDoctor: Number(form.IdDoctor), Fecha: form.Fecha,
				Diagnostico: form.Diagnostico, Tratamiento: form.Tratamiento,
				Notas: form.Notas, Activo: form.Activo
			});
			if (!validation.success) { fieldErrors = mapZodErrors(validation.error); return; }

			const payload = { IdHistoriaClinica: Number(form.IdHistoriaClinica), IdDoctor: Number(form.IdDoctor),
				Fecha: form.Fecha, Diagnostico: form.Diagnostico, Tratamiento: form.Tratamiento,
				Notas: form.Notas, Activo: form.Activo };
			const resultado = editing
				? await api.actualizarEvolucion({ ...payload, Id: form.Id })
				: await api.crearEvolucion(payload);

			if (resultado.Exitoso) {
				toast.success(editing ? 'Evolución actualizada' : 'Evolución registrada');
				await loadData(); resetForm();
			} else {
				toast.error(resultado.Mensaje || 'Error al guardar');
				(resultado.Errores ?? []).forEach((e) => toast.error(e));
			}
		} finally { submitting = false; }
	};

	let filtered = $derived(
		evoluciones.filter((e) => {
			if (!searchQuery) return true;
			const q = searchQuery.toLowerCase();
			return e.NombrePaciente.toLowerCase().includes(q) || e.NombreDoctor.toLowerCase().includes(q)
				|| e.Diagnostico.toLowerCase().includes(q);
		})
	);

	const formatDate = (d: string) => {
		try { return new Intl.DateTimeFormat('es-EC', { dateStyle: 'medium' }).format(new Date(d)); }
		catch { return d; }
	};

	onMount(loadData);
</script>

<svelte:head><title>Evoluciones — ECE-FIAD</title></svelte:head>

<ConfirmModal
	open={confirmOpen}
	title="Eliminar evolución"
	message="¿Deseas eliminar esta evolución? Esta acción no se puede deshacer."
	confirmLabel="Sí, eliminar"
	danger={true}
	loading={confirmLoading}
	onconfirm={confirmarEliminar}
	oncancel={() => { confirmOpen = false; pendingDeleteId = null; }}
/>

<div class="page-header-bar">
	<div class="page-header-info">
		<div class="page-icon" style="background: linear-gradient(135deg, #be185d, #ec4899);">
			<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/></svg>
		</div>
		<div>
			<h1 class="page-title">Evoluciones</h1>
			<p class="page-subtitle">Seguimiento clínico y diagnósticos</p>
		</div>
	</div>
</div>

<div class="form-section">
	<div class="form-section__header">
		<span class="form-section__title">{editing ? '✏️ Editar evolución' : '🩺 Registrar evolución'}</span>
		{#if editing}<span class="form-section__badge">Modo edición</span>{/if}
	</div>
	<div class="form-grid">
		<label class="field">
			<span>Historia clínica *</span>
			<select bind:value={form.IdHistoriaClinica} class:field--error={fieldErrors.IdHistoriaClinica}>
				{#each historias as h}
					<option value={h.Id}>#{h.Id} — {h.NombrePaciente}</option>
				{/each}
			</select>
			{#if fieldErrors.IdHistoriaClinica}<span class="error-text">⚠ {fieldErrors.IdHistoriaClinica}</span>{/if}
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
			<span>Fecha</span>
			<input type="date" bind:value={form.Fecha} class:field--error={fieldErrors.Fecha} />
			{#if fieldErrors.Fecha}<span class="error-text">⚠ {fieldErrors.Fecha}</span>{/if}
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
			<span>Diagnóstico *</span>
			<textarea bind:value={form.Diagnostico} placeholder="Descripción del diagnóstico..." class:field--error={fieldErrors.Diagnostico}></textarea>
			{#if fieldErrors.Diagnostico}<span class="error-text">⚠ {fieldErrors.Diagnostico}</span>{/if}
		</label>
		<label class="field" style="grid-column: 1 / -1;">
			<span>Tratamiento *</span>
			<textarea bind:value={form.Tratamiento} placeholder="Descripción del tratamiento indicado..." class:field--error={fieldErrors.Tratamiento}></textarea>
			{#if fieldErrors.Tratamiento}<span class="error-text">⚠ {fieldErrors.Tratamiento}</span>{/if}
		</label>
		<label class="field" style="grid-column: 1 / -1;">
			<span>Notas adicionales</span>
			<textarea bind:value={form.Notas} placeholder="Observaciones, indicaciones de seguimiento..."></textarea>
		</label>
	</div>
	<div class="actions">
		<button class="btn btn-primary" onclick={submit} disabled={submitting}
			style="background: linear-gradient(135deg,#be185d,#ec4899); border-color:transparent;">
			{#if submitting}<span class="spinner"></span>{/if}
			{submitting ? 'Guardando...' : editing ? 'Guardar cambios' : 'Registrar evolución'}
		</button>
		{#if editing}
			<button class="btn btn-secondary" onclick={resetForm} disabled={submitting}>Cancelar edición</button>
		{/if}
	</div>
</div>

<div class="card">
	<div class="section-header">
		<div>
			<h2 class="section-title">Registro de evoluciones</h2>
			{#if !loading}<span class="record-count">{filtered.length} de {evoluciones.length} registros</span>{/if}
		</div>
		<div class="search-box">
			<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="#94a3b8" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
			<input class="search-input" bind:value={searchQuery} placeholder="Buscar evolución..." />
		</div>
	</div>

	{#if loading}
		<SkeletonLoader rows={5} cols={4} />
	{:else}
		<div class="table-container">
			<table class="table">
				<thead>
					<tr>
						<th>Paciente</th>
						<th>Doctor / Especialidad</th>
						<th>Fecha</th>
						<th>Diagnóstico</th>
						<th>Estado</th>
						<th>Acciones</th>
					</tr>
				</thead>
				<tbody>
					{#if filtered.length === 0}
						<tr><td colspan="6">
							<div class="empty-state">
								<div class="empty-state__icon">
									<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/></svg>
								</div>
								<div class="empty-state__title">No hay evoluciones registradas</div>
							</div>
						</td></tr>
					{:else}
						{#each filtered as ev}
							<tr>
								<td>
									<div style="font-weight:600;color:#0f172a;font-size:0.875rem;">{ev.NombrePaciente}</div>
								</td>
								<td>
									<div style="font-weight:500;font-size:0.875rem;">Dr. {ev.NombreDoctor}</div>
									<div style="font-size:0.775rem;color:#94a3b8;">{ev.NombreEspecialidad}</div>
								</td>
								<td style="font-size:0.875rem;color:#475569;white-space:nowrap;">{formatDate(ev.Fecha)}</td>
								<td style="font-size:0.85rem;color:#64748b;max-width:200px;">
									<div class="truncate">{ev.Diagnostico}</div>
								</td>
								<td><span class={ev.Activo ? 'badge badge--active' : 'badge badge--inactive'}>{findLabel(defaultBooleanOptions, ev.Activo)}</span></td>
								<td>
									<div class="table-actions">
										<button class="btn btn-sm btn-secondary" onclick={() => editarEvolucion(ev)}>
											<svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>
											Editar
										</button>
										<button class="btn btn-sm btn-danger" onclick={() => solicitarEliminar(ev.Id)}>
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
	.search-box:focus-within { border-color:#be185d; box-shadow:0 0 0 3px rgba(190,24,93,0.1); background:#fff; }
	.search-input { border:none; background:none; outline:none; font-size:0.875rem; color:#0f172a; width:200px; }
	.truncate { overflow:hidden; text-overflow:ellipsis; white-space:nowrap; }
</style>
