import { writable } from 'svelte/store';

export type ToastType = 'success' | 'error' | 'info' | 'warning';

export type Toast = {
	id: string;
	type: ToastType;
	message: string;
	duration?: number;
};

function createToastStore() {
	const { subscribe, update } = writable<Toast[]>([]);

	const add = (message: string, type: ToastType = 'info', duration = 4000) => {
		const id = Math.random().toString(36).slice(2);
		update((toasts) => [...toasts, { id, type, message, duration }]);
		if (duration > 0) {
			setTimeout(() => remove(id), duration);
		}
		return id;
	};

	const remove = (id: string) => {
		update((toasts) => toasts.filter((t) => t.id !== id));
	};

	return {
		subscribe,
		success: (msg: string, duration?: number) => add(msg, 'success', duration),
		error: (msg: string, duration?: number) => add(msg, 'error', duration ?? 6000),
		info: (msg: string, duration?: number) => add(msg, 'info', duration),
		warning: (msg: string, duration?: number) => add(msg, 'warning', duration),
		remove
	};
}

export const toast = createToastStore();
