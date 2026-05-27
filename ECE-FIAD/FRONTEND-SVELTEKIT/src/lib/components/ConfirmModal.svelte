<script lang="ts">
	type Props = {
		open: boolean;
		title?: string;
		message: string;
		confirmLabel?: string;
		cancelLabel?: string;
		danger?: boolean;
		loading?: boolean;
		onconfirm: () => void;
		oncancel: () => void;
	};

	let {
		open,
		title = 'Confirmar acción',
		message,
		confirmLabel = 'Confirmar',
		cancelLabel = 'Cancelar',
		danger = false,
		loading = false,
		onconfirm,
		oncancel
	}: Props = $props();

	function handleKeydown(e: KeyboardEvent) {
		if (e.key === 'Escape' && open) oncancel();
	}
</script>

<svelte:window onkeydown={handleKeydown} />

{#if open}
	<!-- svelte-ignore a11y_click_events_have_key_events -->
	<!-- svelte-ignore a11y_no_static_element_interactions -->
	<div class="modal-overlay" onclick={oncancel}>
		<!-- svelte-ignore a11y_click_events_have_key_events -->
		<!-- svelte-ignore a11y_no_static_element_interactions -->
		<div class="modal" role="dialog" aria-modal="true" onclick={(e) => e.stopPropagation()}>
			<div class="modal__icon {danger ? 'modal__icon--danger' : 'modal__icon--info'}">
				{#if danger}
					<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>
				{:else}
					<svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>
				{/if}
			</div>
			<h3 class="modal__title">{title}</h3>
			<p class="modal__message">{message}</p>
			<div class="modal__actions">
				<button class="btn btn-secondary" onclick={oncancel} disabled={loading}>{cancelLabel}</button>
				<button
					class="btn {danger ? 'btn-danger' : 'btn-primary'}"
					onclick={onconfirm}
					disabled={loading}
				>
					{#if loading}
						<span class="spinner-sm"></span>
					{/if}
					{confirmLabel}
				</button>
			</div>
		</div>
	</div>
{/if}

<style>
	.modal-overlay {
		position: fixed;
		inset: 0;
		background: rgba(15, 23, 42, 0.45);
		backdrop-filter: blur(4px);
		z-index: 1000;
		display: grid;
		place-items: center;
		padding: 1rem;
		animation: fadeIn 0.15s ease;
	}

	@keyframes fadeIn {
		from { opacity: 0; }
		to   { opacity: 1; }
	}

	.modal {
		background: #fff;
		border-radius: 1rem;
		padding: 2rem;
		max-width: 420px;
		width: 100%;
		box-shadow: 0 20px 60px rgba(0,0,0,0.15), 0 4px 16px rgba(0,0,0,0.08);
		text-align: center;
		animation: slideUp 0.2s cubic-bezier(0.34, 1.2, 0.64, 1);
	}

	@keyframes slideUp {
		from { transform: translateY(16px) scale(0.97); opacity: 0; }
		to   { transform: translateY(0) scale(1); opacity: 1; }
	}

	.modal__icon {
		width: 56px;
		height: 56px;
		border-radius: 50%;
		display: grid;
		place-items: center;
		margin: 0 auto 1rem;
	}

	.modal__icon--danger { background: #fee2e2; color: #dc2626; }
	.modal__icon--info   { background: #dbeafe; color: #2563eb; }

	.modal__title {
		font-size: 1.1rem;
		font-weight: 700;
		margin: 0 0 0.5rem;
		color: #0f172a;
	}

	.modal__message {
		color: #64748b;
		margin: 0 0 1.5rem;
		font-size: 0.95rem;
		line-height: 1.5;
	}

	.modal__actions {
		display: flex;
		gap: 0.75rem;
		justify-content: center;
	}

	.spinner-sm {
		display: inline-block;
		width: 14px;
		height: 14px;
		border: 2px solid rgba(255,255,255,0.4);
		border-top-color: white;
		border-radius: 50%;
		animation: spin 0.6s linear infinite;
		margin-right: 0.35rem;
	}

	@keyframes spin {
		to { transform: rotate(360deg); }
	}
</style>
