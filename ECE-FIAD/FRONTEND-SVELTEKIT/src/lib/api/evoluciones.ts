import { requestJson, normalizeResultado } from './client';
import type { EvolucionDTO, CrearEvolucionDTO, ActualizarEvolucionDTO, ResultadoAccionSinDatos } from '../types';

export const getEvoluciones = async () => normalizeResultado<EvolucionDTO[]>(await requestJson('/api/evoluciones'));

export const getEvolucionesPorHistoria = async (idHistoria: number) =>
	normalizeResultado<EvolucionDTO[]>(await requestJson(`/api/evoluciones/por-historia/${idHistoria}`));

export const crearEvolucion = async (payload: CrearEvolucionDTO) =>
	normalizeResultado<EvolucionDTO>(
		await requestJson('/api/evoluciones', { method: 'POST', body: JSON.stringify(payload) })
	);

export const actualizarEvolucion = async (payload: ActualizarEvolucionDTO) =>
	normalizeResultado<EvolucionDTO>(
		await requestJson(`/api/evoluciones/${payload.Id}`, {
		method: 'PUT',
		body: JSON.stringify(payload)
	})
	);

export const eliminarEvolucion = async (id: number) =>
	normalizeResultado<null>(await requestJson(`/api/evoluciones/${id}`, { method: 'DELETE' })) as ResultadoAccionSinDatos;
