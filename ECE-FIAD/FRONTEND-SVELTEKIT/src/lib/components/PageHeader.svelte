<script lang="ts">
	type Props = {
		title: string;
		subtitle?: string;
		icon?: string;
	};
	let { title, subtitle, icon }: Props = $props();
</script>

<div class="page-header">
	{#if icon}
		<div class="page-header__icon">{@html icon}</div>
	{/if}
	<div>
		<h1 class="page-header__title">{title}</h1>
		{#if subtitle}
			<p class="page-header__subtitle">{subtitle}</p>
		{/if}
	</div>
</div>

<style>
	.page-header {
		display: flex;
		align-items: center;
		gap: 1rem;
		margin-bottom: 1.5rem;
	}

	.page-header__icon {
		width: 48px;
		height: 48px;
		border-radius: 12px;
		background: linear-gradient(135deg, #2563eb, #3b82f6);
		color: white;
		display: grid;
		place-items: center;
		flex-shrink: 0;
		box-shadow: 0 4px 12px rgba(37, 99, 235, 0.3);
	}

	.page-header__title {
		font-size: 1.5rem;
		font-weight: 700;
		color: #0f172a;
		margin: 0;
		letter-spacing: -0.02em;
	}

	.page-header__subtitle {
		font-size: 0.875rem;
		color: #64748b;
		margin: 0.2rem 0 0;
	}
</style>
