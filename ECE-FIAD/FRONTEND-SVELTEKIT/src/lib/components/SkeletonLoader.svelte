<script lang="ts">
	type Props = { rows?: number; cols?: number };
	let { rows = 5, cols = 4 }: Props = $props();
</script>

<div class="skeleton-table">
	{#each Array(rows) as _}
		<div class="skeleton-row">
			{#each Array(cols) as _, i}
				<div class="skeleton-cell" style="width: {60 + (i * 13) % 30}%"></div>
			{/each}
		</div>
	{/each}
</div>

<style>
	.skeleton-table {
		display: flex;
		flex-direction: column;
		gap: 0;
	}

	.skeleton-row {
		display: flex;
		gap: 1rem;
		padding: 0.85rem 0.75rem;
		border-bottom: 1px solid #f1f5f9;
		align-items: center;
	}

	.skeleton-row:first-child { border-top: 1px solid #f1f5f9; }

	.skeleton-cell {
		height: 14px;
		background: linear-gradient(90deg, #f1f5f9 25%, #e2e8f0 50%, #f1f5f9 75%);
		background-size: 200% 100%;
		border-radius: 6px;
		animation: shimmer 1.4s infinite;
	}

	@keyframes shimmer {
		0%   { background-position: 200% 0; }
		100% { background-position: -200% 0; }
	}
</style>
