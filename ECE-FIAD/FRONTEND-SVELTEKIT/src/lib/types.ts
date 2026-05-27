export type ResultadoAccion<T> = {
	Exitoso: boolean;
	Mensaje: string;
	Datos?: T;
	Errores: string[];
};

export type ResultadoAccionSinDatos = {
	Exitoso: boolean;
	Mensaje: string;
	Errores: string[];
};

export type PacienteDTO = {
	Id: number;
	Nombres: string;
	Apellidos: string;
	NumeroDocumento: string;
	TipoDocumento: number;
	Telefono?: string;
	Email: string;
	FechaNacimiento: string;
	Genero: number;
	GrupoSanguineo: number;
	Direccion?: string;
	Activo: boolean;
};

export type CrearPacienteDTO = {
	Nombres: string;
	Apellidos: string;
	NumeroDocumento: string;
	TipoDocumento: number;
	Telefono?: string;
	Email: string;
	FechaNacimiento: string;
	Genero: number;
	GrupoSanguineo: number;
	Direccion?: string;
};

export type ActualizarPacienteDTO = {
	Id: number;
	Nombres: string;
	Apellidos: string;
	Telefono: string;
	Email: string;
	Direccion: string;
};

export type EspecialidadDTO = {
	Id: number;
	Nombre: string;
	Descripcion?: string;
	Activo: boolean;
	CantidadMedicos: number;
};

export type CrearEspecialidadDTO = {
	Nombre: string;
	Descripcion?: string;
};

export type ActualizarEspecialidadDTO = {
	Id: number;
	Nombre: string;
	Descripcion?: string;
	Activo: boolean;
};

export type DoctorDTO = {
	Id: number;
	Nombre: string;
	Descripcion: string;
	IdEspecialidad: number;
	NombreEspecialidad: string;
	Telefono: string;
	Email: string;
	FechaContratacion: string;
	Activo: boolean;
};

export type CrearDoctorDTO = {
	Nombre: string;
	Descripcion: string;
	IdEspecialidad: number;
	Telefono: string;
	Email: string;
	FechaContratacion: string;
	Activo: boolean;
};

export type ActualizarDoctorDTO = CrearDoctorDTO & {
	Id: number;
};

export type HistoriaClinicaDTO = {
	Id: number;
	IdPaciente: number;
	NombrePaciente: string;
	FechaApertura: string;
	Alergias?: string;
	AntecedentesFamiliares?: string;
	AntecedentesPersonales?: string;
	Activo: boolean;
	Eliminado: boolean;
	FechaDeCreacion: string;
	FechaDeModificacion?: string;
	FechaDeEliminacion?: string;
};

export type CrearHistoriaDTO = {
	IdPaciente: number;
	FechaApertura: string;
	Alergias?: string;
	AntecedentesFamiliares?: string;
	AntecedentesPersonales?: string;
	Activo: boolean;
};

export type ActualizarHistoriaDTO = CrearHistoriaDTO & {
	Id: number;
};

export type EvolucionDTO = {
	Id: number;
	IdHistoriaClinica: number;
	NombrePaciente: string;
	IdDoctor: number;
	NombreDoctor: string;
	NombreEspecialidad: string;
	Fecha: string;
	Diagnostico: string;
	Tratamiento: string;
	Notas?: string;
	Activo: boolean;
	Eliminado: boolean;
	FechaDeCreacion: string;
	FechaDeModificacion?: string;
	FechaDeEliminacion?: string;
};

export type CrearEvolucionDTO = {
	IdHistoriaClinica: number;
	IdDoctor: number;
	Fecha: string;
	Diagnostico: string;
	Tratamiento: string;
	Notas?: string;
	Activo: boolean;
};

export type ActualizarEvolucionDTO = CrearEvolucionDTO & {
	Id: number;
};

export type CitaDTO = {
	Id: number;
	IdPaciente: number;
	NombrePaciente: string;
	IdDoctor: number;
	NombreDoctor: string;
	FechaHora: string;
	Motivo: string;
	Notas?: string;
	Estado: number;
	Activo: boolean;
	Eliminado: boolean;
	FechaDeCreacion: string;
	FechaDeModificacion?: string;
	FechaDeEliminacion?: string;
};

export type CrearCitaDTO = {
	IdPaciente: number;
	IdDoctor: number;
	FechaHora: string;
	Motivo: string;
	Notas?: string;
	Estado: number;
};

export type ActualizarCitaDTO = CrearCitaDTO & {
	Id: number;
};

export const tipoDocumentoOptions = [
	{ value: 1, label: 'Cedula' },
	{ value: 2, label: 'Pasaporte' },
	{ value: 3, label: 'Otro' }
];

export const generoOptions = [
	{ value: 1, label: 'Masculino' },
	{ value: 2, label: 'Femenino' },
	{ value: 3, label: 'Otro' },
	{ value: 4, label: 'Prefiero no decir' }
];

export const grupoSanguineoOptions = [
	{ value: 1, label: 'A+' },
	{ value: 2, label: 'A-' },
	{ value: 3, label: 'B+' },
	{ value: 4, label: 'B-' },
	{ value: 5, label: 'AB+' },
	{ value: 6, label: 'AB-' },
	{ value: 7, label: 'O+' },
	{ value: 8, label: 'O-' }
];

export const estadoCitaOptions = [
	{ value: 1, label: 'Pendiente' },
	{ value: 2, label: 'Confirmada' },
	{ value: 3, label: 'Cancelada' },
	{ value: 4, label: 'Completada' },
	{ value: 5, label: 'No asistio' }
];

export const defaultBooleanOptions = [
	{ value: true, label: 'Activo' },
	{ value: false, label: 'Inactivo' }
];

export const findLabel = (options: { value: number | boolean; label: string }[], value: number | boolean) => {
	return options.find((item) => item.value === value)?.label ?? String(value);
};

