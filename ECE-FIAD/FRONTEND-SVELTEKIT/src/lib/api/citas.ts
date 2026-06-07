import { requestJson, normalizeResultado } from './client';
import type { CitaDTO, CrearCitaDTO, ActualizarCitaDTO, ResultadoAccionSinDatos } from '../types';

export const getCitas = async () => normalizeResultado<CitaDTO[]>(await requestJson('/api/citas'));

export const crearCita = async (payload: CrearCitaDTO) =>
	normalizeResultado<CitaDTO>(await requestJson('/api/citas', { method: 'POST', body: JSON.stringify(payload) }));

export const actualizarCita = async (payload: ActualizarCitaDTO) =>
	normalizeResultado<CitaDTO>(
		await requestJson(`/api/citas/${payload.Id}`, {
		method: 'PUT',
		body: JSON.stringify(payload)
	})
	);

export const eliminarCita = async (id: number) =>
	normalizeResultado<null>(await requestJson(`/api/citas/${id}`, { method: 'DELETE' })) as ResultadoAccionSinDatos;

export const estaDisponible = (idDoctor: number, fechaHora: string, idCita?: number) => {
	const query = new URLSearchParams();
	query.set('idDoctor', String(idDoctor));
	query.set('fechaHora', fechaHora);
	if (idCita) query.set('idCita', String(idCita));
	return requestJson<boolean>(`/api/citas/disponible?${query.toString()}`);
};
