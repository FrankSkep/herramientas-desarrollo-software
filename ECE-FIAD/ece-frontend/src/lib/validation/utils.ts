import type { ZodError } from 'zod';

export const mapZodErrors = (error: ZodError) => {
	const fieldErrors: Record<string, string> = {};
	for (const issue of error.issues) {
		const key = issue.path.join('.') || 'form';
		if (!fieldErrors[key]) {
			fieldErrors[key] = issue.message;
		}
	}
	return fieldErrors;
};

