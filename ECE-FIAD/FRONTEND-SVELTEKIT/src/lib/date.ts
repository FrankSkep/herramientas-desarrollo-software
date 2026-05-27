const pad = (value: number) => String(value).padStart(2, '0');

export const toDateInputValue = (value?: string | Date | null) => {
	if (!value) return '';
	const date = value instanceof Date ? value : new Date(value);
	return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
};

export const toDateTimeLocalValue = (value?: string | Date | null) => {
	if (!value) return '';
	const date = value instanceof Date ? value : new Date(value);
	return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(
		date.getHours()
	)}:${pad(date.getMinutes())}`;
};

export const normalizeDateTimeLocal = (value: string) => {
	if (!value) return value;
	return value.length === 16 ? `${value}:00` : value;
};

