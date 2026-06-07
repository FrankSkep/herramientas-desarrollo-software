import { requestJson, normalizeResultado } from './client';
import type { EspecialidadDTO, CrearEspecialidadDTO, ActualizarEspecialidadDTO, ResultadoAccionSinDatos } from '../types';

export const getEspecialidades = async () => normalizeResultado<EspecialidadDTO[]>(await requestJson('/api/especialidades'));

export const crearEspecialidad = async (payload: CrearEspecialidadDTO) =>
	normalizeResultado<EspecialidadDTO>(
		await requestJson('/api/especialidades', { method: 'POST', body: JSON.stringify(payload) })
	);

export const actualizarEspecialidad = async (payload: ActualizarEspecialidadDTO) =>
	normalizeResultado<EspecialidadDTO>(
		await requestJson(`/api/especialidades/${payload.Id}`, {
		method: 'PUT',
		body: JSON.stringify(payload)
	})
	);

export const eliminarEspecialidad = async (id: number) =>
	normalizeResultado<null>(await requestJson(`/api/especialidades/${id}`, { method: 'DELETE' })) as ResultadoAccionSinDatos;
