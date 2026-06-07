import { env } from '$env/dynamic/public';
import type { ResultadoAccion } from '../types';

const baseUrl = (env.PUBLIC_API_BASE_URL || '').replace(/\/$/, '');
export const buildUrl = (path: string) => `${baseUrl}${path.startsWith('/') ? path : `/${path}`}`;

export const requestJson = async <T>(path: string, init?: RequestInit): Promise<T> => {
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

export const normalizeResultado = <T>(value: any): ResultadoAccion<T> => {
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
