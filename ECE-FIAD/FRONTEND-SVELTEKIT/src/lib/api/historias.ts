import { requestJson, normalizeResultado } from './client';
import type { HistoriaClinicaDTO, CrearHistoriaDTO, ActualizarHistoriaDTO, ResultadoAccionSinDatos } from '../types';

export const getHistorias = async () => normalizeResultado<HistoriaClinicaDTO[]>(await requestJson('/api/historias-clinicas'));

export const getHistoriasActivas = async () =>
	normalizeResultado<HistoriaClinicaDTO[]>(await requestJson('/api/historias-clinicas/activas'));

export const crearHistoria = async (payload: CrearHistoriaDTO) =>
	normalizeResultado<HistoriaClinicaDTO>(
		await requestJson('/api/historias-clinicas', { method: 'POST', body: JSON.stringify(payload) })
	);

export const actualizarHistoria = async (payload: ActualizarHistoriaDTO) =>
	normalizeResultado<HistoriaClinicaDTO>(
		await requestJson(`/api/historias-clinicas/${payload.Id}`, {
		method: 'PUT',
		body: JSON.stringify(payload)
	})
	);

export const eliminarHistoria = async (id: number) =>
	normalizeResultado<null>(await requestJson(`/api/historias-clinicas/${id}`, { method: 'DELETE' })) as ResultadoAccionSinDatos;

export const existeHistoriaActiva = (idPaciente: number, idHistoria?: number) => {
	const query = new URLSearchParams();
	query.set('idPaciente', String(idPaciente));
	if (idHistoria) query.set('idHistoria', String(idHistoria));
	return requestJson<boolean>(`/api/historias-clinicas/existe-activa?${query.toString()}`);
};
