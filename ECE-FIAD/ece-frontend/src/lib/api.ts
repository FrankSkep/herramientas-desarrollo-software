import { env } from '$env/dynamic/public';
import type {
	ActualizarCitaDTO,
	ActualizarDoctorDTO,
	ActualizarEspecialidadDTO,
	ActualizarEvolucionDTO,
	ActualizarHistoriaDTO,
	ActualizarPacienteDTO,
	CrearCitaDTO,
	CrearDoctorDTO,
	CrearEspecialidadDTO,
	CrearEvolucionDTO,
	CrearHistoriaDTO,
	CrearPacienteDTO,
	CitaDTO,
	DoctorDTO,
	EspecialidadDTO,
	EvolucionDTO,
	HistoriaClinicaDTO,
	PacienteDTO,
	ResultadoAccion,
	ResultadoAccionSinDatos
} from './types';

const baseUrl = (env.PUBLIC_API_BASE_URL || '').replace(/\/$/, '');

const buildUrl = (path: string) => `${baseUrl}${path.startsWith('/') ? path : `/${path}`}`;

const requestJson = async <T>(path: string, init?: RequestInit): Promise<T> => {
	const response = await fetch(buildUrl(path), {
		headers: {
			'Content-Type': 'application/json',
			...(init?.headers ?? {})
		},
		...init
	});

	const text = await response.text();
	const data = text ? (JSON.parse(text) as T) : ({} as T);
	return data;
};

const isPlainObject = (value: unknown): value is Record<string, unknown> => {
	return Boolean(value) && typeof value === 'object' && !Array.isArray(value);
};

const upperFirstKey = (key: string) => (key ? `${key[0].toUpperCase()}${key.slice(1)}` : key);

const normalizeKeys = <T>(value: T): T => {
	if (Array.isArray(value)) {
		return value.map((item) => normalizeKeys(item)) as T;
	}
	if (!isPlainObject(value)) {
		return value;
	}
	const result: Record<string, unknown> = {};
	for (const [key, item] of Object.entries(value)) {
		const normalizedKey = key[0] === key[0].toUpperCase() ? key : upperFirstKey(key);
		result[normalizedKey] = normalizeKeys(item);
	}
	return result as T;
};

const normalizeResultado = <T>(value: any): ResultadoAccion<T> => {
	if (value && typeof value === 'object') {
		if ('Exitoso' in value) {
			return { ...value, Datos: normalizeKeys(value.Datos) } as ResultadoAccion<T>;
		}
		if ('exitoso' in value) {
			return {
				Exitoso: Boolean(value.exitoso),
				Mensaje: value.mensaje ?? '',
				Datos: normalizeKeys(value.datos),
				Errores: value.errores ?? []
			};
		}
	}
	return normalizeKeys(value) as ResultadoAccion<T>;
};

export const api = {
	getPacientes: async () => normalizeResultado<PacienteDTO[]>(await requestJson('/api/pacientes')),
	getPaciente: async (id: number) => normalizeResultado<PacienteDTO>(await requestJson(`/api/pacientes/${id}`)),
	getPacientesActivosSinHistoria: async () =>
		normalizeResultado<PacienteDTO[]>(await requestJson('/api/pacientes/activos-sin-historia')),
	crearPaciente: async (payload: CrearPacienteDTO) =>
		normalizeResultado<PacienteDTO>(
			await requestJson('/api/pacientes', { method: 'POST', body: JSON.stringify(payload) })
		),
	actualizarPaciente: async (payload: ActualizarPacienteDTO) =>
		normalizeResultado<PacienteDTO>(
			await requestJson(`/api/pacientes/${payload.Id}`, {
			method: 'PUT',
			body: JSON.stringify(payload)
		})
		),
	eliminarPaciente: async (id: number) =>
		normalizeResultado<null>(await requestJson(`/api/pacientes/${id}`, { method: 'DELETE' })) as ResultadoAccionSinDatos,
	existeIdentificacion: (identificacion: string) =>
		requestJson<boolean>(`/api/pacientes/existe-identificacion?identificacion=${encodeURIComponent(identificacion)}`),

	getDoctores: async () => normalizeResultado<DoctorDTO[]>(await requestJson('/api/doctores')),
	crearDoctor: async (payload: CrearDoctorDTO) =>
		normalizeResultado<DoctorDTO>(
			await requestJson('/api/doctores', { method: 'POST', body: JSON.stringify(payload) })
		),
	actualizarDoctor: async (payload: ActualizarDoctorDTO) =>
		normalizeResultado<DoctorDTO>(
			await requestJson(`/api/doctores/${payload.Id}`, {
			method: 'PUT',
			body: JSON.stringify(payload)
		})
		),
	eliminarDoctor: async (id: number) =>
		normalizeResultado<null>(await requestJson(`/api/doctores/${id}`, { method: 'DELETE' })) as ResultadoAccionSinDatos,

	getEspecialidades: async () => normalizeResultado<EspecialidadDTO[]>(await requestJson('/api/especialidades')),
	crearEspecialidad: async (payload: CrearEspecialidadDTO) =>
		normalizeResultado<EspecialidadDTO>(
			await requestJson('/api/especialidades', { method: 'POST', body: JSON.stringify(payload) })
		),
	actualizarEspecialidad: async (payload: ActualizarEspecialidadDTO) =>
		normalizeResultado<EspecialidadDTO>(
			await requestJson(`/api/especialidades/${payload.Id}`, {
			method: 'PUT',
			body: JSON.stringify(payload)
		})
		),
	eliminarEspecialidad: async (id: number) =>
		normalizeResultado<null>(await requestJson(`/api/especialidades/${id}`, { method: 'DELETE' })) as ResultadoAccionSinDatos,

	getHistorias: async () => normalizeResultado<HistoriaClinicaDTO[]>(await requestJson('/api/historias-clinicas')),
	getHistoriasActivas: async () =>
		normalizeResultado<HistoriaClinicaDTO[]>(await requestJson('/api/historias-clinicas/activas')),
	crearHistoria: async (payload: CrearHistoriaDTO) =>
		normalizeResultado<HistoriaClinicaDTO>(
			await requestJson('/api/historias-clinicas', { method: 'POST', body: JSON.stringify(payload) })
		),
	actualizarHistoria: async (payload: ActualizarHistoriaDTO) =>
		normalizeResultado<HistoriaClinicaDTO>(
			await requestJson(`/api/historias-clinicas/${payload.Id}`, {
			method: 'PUT',
			body: JSON.stringify(payload)
		})
		),
	eliminarHistoria: async (id: number) =>
		normalizeResultado<null>(await requestJson(`/api/historias-clinicas/${id}`, { method: 'DELETE' })) as ResultadoAccionSinDatos,
	existeHistoriaActiva: (idPaciente: number, idHistoria?: number) => {
		const query = new URLSearchParams();
		query.set('idPaciente', String(idPaciente));
		if (idHistoria) query.set('idHistoria', String(idHistoria));
		return requestJson<boolean>(`/api/historias-clinicas/existe-activa?${query.toString()}`);
	},

	getEvoluciones: async () => normalizeResultado<EvolucionDTO[]>(await requestJson('/api/evoluciones')),
	getEvolucionesPorHistoria: async (idHistoria: number) =>
		normalizeResultado<EvolucionDTO[]>(await requestJson(`/api/evoluciones/por-historia/${idHistoria}`)),
	crearEvolucion: async (payload: CrearEvolucionDTO) =>
		normalizeResultado<EvolucionDTO>(
			await requestJson('/api/evoluciones', { method: 'POST', body: JSON.stringify(payload) })
		),
	actualizarEvolucion: async (payload: ActualizarEvolucionDTO) =>
		normalizeResultado<EvolucionDTO>(
			await requestJson(`/api/evoluciones/${payload.Id}`, {
			method: 'PUT',
			body: JSON.stringify(payload)
		})
		),
	eliminarEvolucion: async (id: number) =>
		normalizeResultado<null>(await requestJson(`/api/evoluciones/${id}`, { method: 'DELETE' })) as ResultadoAccionSinDatos,

	getCitas: async () => normalizeResultado<CitaDTO[]>(await requestJson('/api/citas')),
	crearCita: async (payload: CrearCitaDTO) =>
		normalizeResultado<CitaDTO>(await requestJson('/api/citas', { method: 'POST', body: JSON.stringify(payload) })),
	actualizarCita: async (payload: ActualizarCitaDTO) =>
		normalizeResultado<CitaDTO>(
			await requestJson(`/api/citas/${payload.Id}`, {
			method: 'PUT',
			body: JSON.stringify(payload)
		})
		),
	eliminarCita: async (id: number) =>
		normalizeResultado<null>(await requestJson(`/api/citas/${id}`, { method: 'DELETE' })) as ResultadoAccionSinDatos,
	estaDisponible: (idDoctor: number, fechaHora: string, idCita?: number) => {
		const query = new URLSearchParams();
		query.set('idDoctor', String(idDoctor));
		query.set('fechaHora', fechaHora);
		if (idCita) query.set('idCita', String(idCita));
		return requestJson<boolean>(`/api/citas/disponible?${query.toString()}`);
	}
};

