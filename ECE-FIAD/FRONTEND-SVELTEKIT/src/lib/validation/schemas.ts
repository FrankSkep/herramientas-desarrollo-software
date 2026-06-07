import { z } from 'zod';

const positiveId = (message: string) => z.number().int().gt(0, { message });
const maxString = (max: number, message: string) => z.string().max(max, { message });

const dateFromInput = (messageRequired: string) =>	z.preprocess(
		(value) => (typeof value === 'string' && value ? new Date(value) : value),
		z.date({ required_error: messageRequired })
	);

const dateNotFuture = (message: string) =>	dateFromInput(message).refine((value) => value <= new Date(), { message });

const dateFuture = (message: string) =>	dateFromInput(message).refine((value) => value > new Date(), { message });

export const crearPacienteSchema = z.object({
	Nombres: z.string().min(1, { message: 'Los nombres son obligatorios' }).max(100, {
		message: 'Los nombres no pueden superar 100 caracteres'
	}),
	Apellidos: z.string().min(1, { message: 'Los apellidos son obligatorios' }).max(100, {
		message: 'Los apellidos no pueden superar 100 caracteres'
	}),
	NumeroDocumento: z.string().min(1, { message: 'El numero de identificacion es obligatorio' }).max(20, {
		message: 'El numero de identificacion no puede superar 20 caracteres'
	}),
	TipoDocumento: positiveId('Debe seleccionar un tipo de documento valido.'),
	Telefono: z.string().max(15, { message: 'El telefono no puede superar 15 caracteres' }).optional().or(z.literal('')),
	Email: z.string().email({ message: 'Formato de email incorrecto' }).max(150, { message: 'El email no puede superar 150 caracteres' }),
	FechaNacimiento: dateFromInput('La fecha de nacimiento es obligatoria')
		.refine((value) => value < new Date(), { message: 'La fecha de nacimiento debe ser anterior a hoy' })
		.refine((value) => value > new Date(new Date().setFullYear(new Date().getFullYear() - 120)), {
			message: 'Edad no valida (maximo 120 anos)'
		}),
	Genero: positiveId('Debe seleccionar un genero valido.'),
	GrupoSanguineo: positiveId('Debe seleccionar un grupo sanguineo valido.'),
	Direccion: z.string().optional().or(z.literal(''))
});

export const actualizarPacienteSchema = z.object({
	Id: positiveId('El ID del paciente es obligatorio.'),
	Nombres: z.string().min(1, { message: 'Los nombres son obligatorios' }).max(100, {
		message: 'Los nombres no pueden superar 100 caracteres'
	}),
	Apellidos: z.string().min(1, { message: 'Los apellidos son obligatorios' }).max(100, {
		message: 'Los apellidos no pueden superar 100 caracteres'
	}),
	Email: z.string().email({ message: 'Formato de email incorrecto' }).max(150, { message: 'El email no puede superar 150 caracteres' })
});

export const crearDoctorSchema = z.object({
	Nombre: z.string().min(1, { message: 'El nombre es obligatorio.' }).max(100, {
		message: 'El nombre no puede superar los 100 caracteres.'
	}),
	Descripcion: maxString(500, 'La descripcion no puede superar los 500 caracteres.'),
	Email: z.string().min(1, { message: 'El email es obligatorio.' }).email({ message: 'El email no es valido.' }),
	FechaContratacion: dateNotFuture('La fecha de contratacion no puede ser futura.'),
	IdEspecialidad: positiveId('Debe seleccionar una especialidad.'),
	Telefono: z.string().optional().or(z.literal('')),
	Activo: z.boolean()
});

export const actualizarDoctorSchema = crearDoctorSchema.extend({
	Id: positiveId('El ID del doctor es obligatorio.')
});

export const crearEspecialidadSchema = z.object({
	Nombre: z.string().min(1, { message: 'El nombre es requerido' }).max(100, { message: 'Maximo 100 caracteres' }),
	Descripcion: z.string().max(500, { message: 'Maximo 500 caracteres' }).optional().or(z.literal(''))
});

export const actualizarEspecialidadSchema = crearEspecialidadSchema.extend({
	Id: positiveId('El ID de la especialidad es obligatorio.'),
	Activo: z.boolean()
});

export const crearHistoriaSchema = z.object({
	IdPaciente: positiveId('Debe seleccionar un paciente valido.'),
	FechaApertura: dateNotFuture('La fecha de apertura no puede ser futura.'),
	Alergias: z.string().max(500, { message: 'Las alergias no pueden superar 500 caracteres.' }).optional().or(z.literal('')),
	AntecedentesFamiliares: z
		.string()
		.max(500, { message: 'Los antecedentes familiares no pueden superar 500 caracteres.' })
		.optional()
		.or(z.literal('')),
	AntecedentesPersonales: z
		.string()
		.max(500, { message: 'Los antecedentes personales no pueden superar 500 caracteres.' })
		.optional()
		.or(z.literal('')),
	Activo: z.boolean()
});

export const actualizarHistoriaSchema = crearHistoriaSchema.extend({
	Id: positiveId('El ID de la historia clinica es obligatorio.')
});

export const crearEvolucionSchema = z.object({
	IdHistoriaClinica: positiveId('Debe seleccionar una historia clinica valida.'),
	IdDoctor: positiveId('Debe seleccionar un doctor valido.'),
	Fecha: dateNotFuture('La fecha no puede ser futura.'),
	Diagnostico: z.string().min(1, { message: 'El diagnostico es obligatorio.' }).max(500, {
		message: 'El diagnostico no puede superar 500 caracteres.'
	}),
	Tratamiento: z.string().min(1, { message: 'El tratamiento es obligatorio.' }).max(500, {
		message: 'El tratamiento no puede superar 500 caracteres.'
	}),
	Notas: z.string().max(1000, { message: 'Las notas no pueden superar 1000 caracteres.' }).optional().or(z.literal('')),
	Activo: z.boolean()
});

export const actualizarEvolucionSchema = crearEvolucionSchema.extend({
	Id: positiveId('El Id de la evolucion es obligatorio.')
});

export const crearCitaSchema = z.object({
	IdPaciente: positiveId('Debe seleccionar un paciente valido.'),
	IdDoctor: positiveId('Debe seleccionar un doctor valido.'),
	FechaHora: dateFuture('La fecha y hora deben ser futuras.'),
	Motivo: z.string().min(5, { message: 'El motivo es obligatorio (mínimo 5 caracteres).' }).max(500, {
		message: 'El motivo no puede superar 500 caracteres.'
	}),
	Notas: z.string().max(1000, { message: 'Las notas no pueden superar 1000 caracteres.' }).optional().or(z.literal('')),
	Estado: positiveId('Debe seleccionar un estado valido.')
});

export const actualizarCitaSchema = crearCitaSchema.extend({
	Id: positiveId('El ID de la cita es obligatorio.')
});

