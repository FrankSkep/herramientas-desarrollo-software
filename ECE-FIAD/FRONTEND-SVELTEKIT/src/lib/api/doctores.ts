import { requestJson, normalizeResultado } from './client';
import type { DoctorDTO, CrearDoctorDTO, ActualizarDoctorDTO, ResultadoAccionSinDatos } from '../types';

export const getDoctores = async () => normalizeResultado<DoctorDTO[]>(await requestJson('/api/doctores'));

export const crearDoctor = async (payload: CrearDoctorDTO) =>
	normalizeResultado<DoctorDTO>(
		await requestJson('/api/doctores', { method: 'POST', body: JSON.stringify(payload) })
	);

export const actualizarDoctor = async (payload: ActualizarDoctorDTO) =>
	normalizeResultado<DoctorDTO>(
		await requestJson(`/api/doctores/${payload.Id}`, {
		method: 'PUT',
		body: JSON.stringify(payload)
	})
	);

export const eliminarDoctor = async (id: number) =>
	normalizeResultado<null>(await requestJson(`/api/doctores/${id}`, { method: 'DELETE' })) as ResultadoAccionSinDatos;
