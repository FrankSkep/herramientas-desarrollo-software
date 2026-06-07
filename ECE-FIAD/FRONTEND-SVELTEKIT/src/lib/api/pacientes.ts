import { requestJson, normalizeResultado } from './client';
import type { PacienteDTO, CrearPacienteDTO, ActualizarPacienteDTO, ResultadoAccionSinDatos } from '../types';

export const getPacientes = async () => normalizeResultado<PacienteDTO[]>(await requestJson('/api/pacientes'));

export const getPaciente = async (id: number) => normalizeResultado<PacienteDTO>(await requestJson(`/api/pacientes/${id}`));

export const getPacientesActivosSinHistoria = async () =>
	normalizeResultado<PacienteDTO[]>(await requestJson('/api/pacientes/activos-sin-historia'));

export const crearPaciente = async (payload: CrearPacienteDTO) =>
	normalizeResultado<PacienteDTO>(
		await requestJson('/api/pacientes', { method: 'POST', body: JSON.stringify(payload) })
	);

export const actualizarPaciente = async (payload: ActualizarPacienteDTO) =>
	normalizeResultado<PacienteDTO>(
		await requestJson(`/api/pacientes/${payload.Id}`, {
		method: 'PUT',
		body: JSON.stringify(payload)
	})
	);

export const eliminarPaciente = async (id: number) =>
	normalizeResultado<null>(await requestJson(`/api/pacientes/${id}`, { method: 'DELETE' })) as ResultadoAccionSinDatos;

export const existeIdentificacion = (identificacion: string) =>
	requestJson<boolean>(`/api/pacientes/existe-identificacion?identificacion=${encodeURIComponent(identificacion)}`);
