<script lang="ts">
	import { toast } from '$lib/toast';

	const icons = {
		success: `<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M20 6L9 17l-5-5"/></svg>`,
		error: `<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="15" y1="9" x2="9" y2="15"/><line x1="9" y1="9" x2="15" y2="15"/></svg>`,
		info: `<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>`,
		warning: `<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>`
	};
</script>

<div class="toaster" aria-live="polite" aria-atomic="false">
	{#each $toast as t (t.id)}
		<div class="toast toast--{t.type}" role="alert">
			<span class="toast__icon">{@html icons[t.type]}</span>
			<span class="toast__msg">{t.message}</span>
			<button class="toast__close" onclick={() => toast.remove(t.id)} aria-label="Cerrar">
				<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
			</button>
		</div>
	{/each}
</div>

<style>
	.toaster {
		position: fixed;
		bottom: 1.5rem;
		right: 1.5rem;
		z-index: 9999;
		display: flex;
		flex-direction: column;
		gap: 0.6rem;
		max-width: 380px;
		width: 100%;
		pointer-events: none;
	}

	.toast {
		display: flex;
		align-items: center;
		gap: 0.65rem;
		padding: 0.75rem 1rem;
		border-radius: 0.65rem;
		font-weight: 500;
		font-size: 0.9rem;
		box-shadow: 0 8px 24px rgba(0,0,0,0.12), 0 2px 8px rgba(0,0,0,0.08);
		pointer-events: all;
		animation: slideIn 0.25s cubic-bezier(0.34, 1.56, 0.64, 1) forwards;
		border: 1px solid transparent;
	}

	@keyframes slideIn {
		from { transform: translateX(120%); opacity: 0; }
		to   { transform: translateX(0);   opacity: 1; }
	}

	.toast--success { background: #f0fdf4; color: #15803d; border-color: #bbf7d0; }
	.toast--error   { background: #fef2f2; color: #b91c1c; border-color: #fecaca; }
	.toast--info    { background: #eff6ff; color: #1d4ed8; border-color: #bfdbfe; }
	.toast--warning { background: #fffbeb; color: #b45309; border-color: #fde68a; }

	.toast__icon { flex-shrink: 0; display: flex; }
	.toast__msg  { flex: 1; line-height: 1.4; }

	.toast__close {
		flex-shrink: 0;
		display: flex;
		align-items: center;
		justify-content: center;
		background: none;
		border: none;
		cursor: pointer;
		padding: 0.15rem;
		border-radius: 0.25rem;
		opacity: 0.6;
		transition: opacity 0.15s;
		color: inherit;
	}
	.toast__close:hover { opacity: 1; }
</style>
