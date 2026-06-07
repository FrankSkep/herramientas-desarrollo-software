<script lang="ts">
	import { onMount, tick } from 'svelte';
	import { api } from '$lib/api';
	import { toast } from '$lib/toast';
	import { toDateInputValue } from '$lib/date';
	import { crearEvolucionSchema } from '$lib/validation/schemas';
	import { mapZodErrors } from '$lib/validation/utils';
	import type { EvolucionDTO, HistoriaClinicaDTO, DoctorDTO } from '$lib/types';
	import SkeletonLoader from '$lib/components/SkeletonLoader.svelte';

	let historias: HistoriaClinicaDTO[] = $state([]);
	let doctores: DoctorDTO[] = $state([]);
	let evolucionesDelPaciente: EvolucionDTO[] = $state([]);
	let selectedHistoriaId = $state<number>(0);

	let loading = $state(true);
	let loadingEvoluciones = $state(false);
	
	let formVisible = $state(false);
	let submitting = $state(false);
	let fieldErrors: Record<string, string> = $state({});

	let form = $state({
		IdHistoriaClinica: 0, IdDoctor: 0,
		Fecha: toDateInputValue(new Date()), Diagnostico: '', Tratamiento: '', Notas: '', Activo: true
	});

	const loadData = async () => {
		loading = true;
		const [hr, dr] = await Promise.all([api.getHistoriasActivas(), api.getDoctores()]);
		if (hr.Exitoso && hr.Datos) historias = hr.Datos;
		else toast.error(hr.Mensaje || 'Error al cargar pacientes activos');
		
		if (dr.Exitoso && dr.Datos) doctores = dr.Datos;
		loading = false;
	};

	const onSelectPaciente = async () => {
		if (selectedHistoriaId === 0) {
			evolucionesDelPaciente = [];
			formVisible = false;
			return;
		}
		loadingEvoluciones = true;
		formVisible = false;
		const nr = await api.getEvolucionesPorHistoria(selectedHistoriaId);
		if (nr.Exitoso && nr.Datos) {
			evolucionesDelPaciente = nr.Datos.sort((a,b) => new Date(b.Fecha).getTime() - new Date(a.Fecha).getTime());
		} else {
			toast.error(nr.Mensaje || 'Error al cargar evoluciones');
		}
		loadingEvoluciones = false;
	};

	const abrirFormulario = () => {
		formVisible = true;
		fieldErrors = {};
		form = { 
			IdHistoriaClinica: selectedHistoriaId, 
			IdDoctor: doctores[0]?.Id ?? 0,
			Fecha: toDateInputValue(new Date()), 
			Diagnostico: '', 
			Tratamiento: '', 
			Notas: '', 
			Activo: true 
		};
	};

	const submitEvolucion = async () => {
		fieldErrors = {}; submitting = true;
		try {
			const validation = crearEvolucionSchema.safeParse({
				IdHistoriaClinica: Number(form.IdHistoriaClinica),
				IdDoctor: Number(form.IdDoctor), Fecha: form.Fecha,
				Diagnostico: form.Diagnostico, Tratamiento: form.Tratamiento,
				Notas: form.Notas, Activo: form.Activo
			});
			if (!validation.success) { fieldErrors = mapZodErrors(validation.error); return; }

			const payload = { IdHistoriaClinica: Number(form.IdHistoriaClinica), IdDoctor: Number(form.IdDoctor),
				Fecha: form.Fecha, Diagnostico: form.Diagnostico, Tratamiento: form.Tratamiento,
				Notas: form.Notas, Activo: form.Activo };
			
			const resultado = await api.crearEvolucion(payload);

			if (resultado.Exitoso) {
				toast.success('Evolución registrada');
				formVisible = false;
				await onSelectPaciente();
			} else {
				toast.error(resultado.Mensaje || 'Error al guardar');
				(resultado.Errores ?? []).forEach((e) => toast.error(e));
			}
		} finally { submitting = false; }
	};

	const formatDate = (d: string) => {
		try { return new Intl.DateTimeFormat('es-EC', { dateStyle: 'medium' }).format(new Date(d)); }
		catch { return d; }
	};

	onMount(loadData);
</script>

<svelte:head><title>Historial Clínico — ECE-FIAD</title></svelte:head>

<div class="page-header-bar">
	<div class="page-header-info">
		<div class="page-icon" style="background: linear-gradient(135deg, #10b981, #059669);">
			<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="12" y1="18" x2="12" y2="12"/><line x1="9" y1="15" x2="15" y2="15"/></svg>
		</div>
		<div>
			<h1 class="page-title">Historial Clínico</h1>
			<p class="page-subtitle">Consulta de pacientes y sus evoluciones</p>
		</div>
	</div>
</div>

<div class="card" style="margin-bottom: 2rem;">
	<div class="section-header">
		<h2 class="section-title">Seleccionar Paciente (solo activos)</h2>
	</div>
	
	{#if loading}
		<SkeletonLoader rows={1} cols={1} />
	{:else}
		<div class="form-grid" style="margin-top: 1rem;">
			<label class="field">
				<select bind:value={selectedHistoriaId} onchange={onSelectPaciente} class="filter-select">
					<option value={0}>-- Seleccione un paciente --</option>
					{#each historias as h}
						<option value={h.Id}>{h.NombrePaciente}</option>
					{/each}
				</select>
			</label>
		</div>
	{/if}
</div>

{#if selectedHistoriaId !== 0}
	<div class="page-header-bar" style="margin-top: 2rem;">
		<h2 class="page-title">Evoluciones del Paciente</h2>
		{#if !formVisible}
			<button class="btn btn-primary" onclick={abrirFormulario}>+ Agregar evolución</button>
		{/if}
	</div>

	{#if formVisible}
		<div class="form-section">
			<div class="form-section__header">
				<span class="form-section__title">📝 Nueva Evolución</span>
			</div>
			<div class="form-grid">
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
					<span>Fecha *</span>
					<input type="date" bind:value={form.Fecha} class:field--error={fieldErrors.Fecha} />
					{#if fieldErrors.Fecha}<span class="error-text">⚠ {fieldErrors.Fecha}</span>{/if}
				</label>
				<label class="field" style="grid-column: 1 / -1;">
					<span>Diagnóstico *</span>
					<textarea bind:value={form.Diagnostico} class:field--error={fieldErrors.Diagnostico}></textarea>
					{#if fieldErrors.Diagnostico}<span class="error-text">⚠ {fieldErrors.Diagnostico}</span>{/if}
				</label>
				<label class="field" style="grid-column: 1 / -1;">
					<span>Tratamiento *</span>
					<textarea bind:value={form.Tratamiento} class:field--error={fieldErrors.Tratamiento}></textarea>
					{#if fieldErrors.Tratamiento}<span class="error-text">⚠ {fieldErrors.Tratamiento}</span>{/if}
				</label>
			</div>
			<div class="actions">
				<button class="btn btn-primary" onclick={submitEvolucion} disabled={submitting}>
					{#if submitting}<span class="spinner"></span>{/if}
					Guardar Evolución
				</button>
				<button class="btn btn-secondary" onclick={() => formVisible = false} disabled={submitting}>Cancelar</button>
			</div>
		</div>
	{/if}

	<div class="card" style="margin-top: 1.5rem;">
		{#if loadingEvoluciones}
			<SkeletonLoader rows={4} cols={3} />
		{:else}
			<div class="table-container">
				<table class="table">
					<thead>
						<tr>
							<th>Fecha</th>
							<th>Doctor / Especialidad</th>
							<th>Diagnóstico</th>
							<th>Tratamiento</th>
						</tr>
					</thead>
					<tbody>
						{#if evolucionesDelPaciente.length === 0}
							<tr><td colspan="4">
								<div class="empty-state">
									<div class="empty-state__title">No hay evoluciones registradas para este paciente</div>
								</div>
							</td></tr>
						{:else}
							{#each evolucionesDelPaciente as evo}
								<tr>
									<td>
										<div class="fecha-cell">
											{formatDate(evo.Fecha)}
										</div>
									</td>
									<td>
										<div style="font-weight:600;font-size:0.875rem;">Dr. {evo.NombreDoctor}</div>
										<div style="font-size:0.75rem;color:#64748b;">{evo.NombreEspecialidad}</div>
									</td>
									<td><div class="truncate" style="max-width: 250px;">{evo.Diagnostico}</div></td>
									<td><div class="truncate" style="max-width: 250px;">{evo.Tratamiento}</div></td>
								</tr>
							{/each}
						{/if}
					</tbody>
				</table>
			</div>
		{/if}
	</div>
{/if}

<style>
	.page-header-bar { display:flex; align-items:center; justify-content:space-between; margin-bottom:1.5rem; flex-wrap:wrap; gap:1rem; }
	.page-header-info { display:flex; align-items:center; gap:1rem; }
	.page-icon { width:50px; height:50px; border-radius:14px; display:grid; place-items:center; box-shadow:0 4px 14px rgba(0,0,0,0.15); flex-shrink:0; }
	.page-title { font-size:1.6rem; font-weight:800; color:#0f172a; margin:0; letter-spacing:-0.03em; }
	.page-subtitle { font-size:0.85rem; color:#64748b; margin:0.2rem 0 0; }
	.filter-select { padding:0.5rem 0.75rem; border:1.5px solid #e2e8f0; border-radius:0.6rem; font-size:0.875rem; width: 100%; max-width: 400px; }
	.fecha-cell { display:flex; align-items:center; gap:0.4rem; font-size:0.875rem; color:#334155; white-space:nowrap; }
	.truncate { overflow:hidden; text-overflow:ellipsis; white-space:nowrap; }
</style>
