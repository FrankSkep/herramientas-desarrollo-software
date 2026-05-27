<script lang="ts">
	import { page } from '$app/state';

	const navItems = [
		{ href: '/', label: 'Inicio', icon: `<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2z"/><polyline points="9 22 9 12 15 12 15 22"/></svg>` },
		{ href: '/pacientes', label: 'Pacientes', icon: `<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 00-3-3.87"/><path d="M16 3.13a4 4 0 010 7.75"/></svg>` },
		{ href: '/doctores', label: 'Doctores', icon: `<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 7h-9"/><path d="M14 17H5"/><circle cx="17" cy="17" r="3"/><circle cx="7" cy="7" r="3"/></svg>` },
		{ href: '/especialidades', label: 'Especialidades', icon: `<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="2" y="3" width="20" height="14" rx="2"/><line x1="8" y1="21" x2="16" y2="21"/><line x1="12" y1="17" x2="12" y2="21"/></svg>` },
		{ href: '/citas', label: 'Citas', icon: `<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>` },
		{ href: '/historias-clinicas', label: 'Historias', icon: `<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="12" y1="18" x2="12" y2="12"/><line x1="9" y1="15" x2="15" y2="15"/></svg>` },
		{ href: '/evoluciones', label: 'Evoluciones', icon: `<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/></svg>` },
	];

	let menuOpen = $state(false);
</script>

<header class="app-header">
	<div class="brand">
		<div class="brand-mark">
			<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
				<path d="M22 12h-4l-3 9L9 3l-3 9H2"/>
			</svg>
		</div>
		<div>
			<div class="brand-title">ECE-FIAD</div>
			<div class="brand-subtitle">Gestor clínico</div>
		</div>
	</div>

	<nav class="nav" class:nav--open={menuOpen}>
		{#each navItems as item}
			<a
				class="nav-link"
				class:active={item.href === '/' ? page.url.pathname === '/' : page.url.pathname.startsWith(item.href)}
				href={item.href}
				onclick={() => menuOpen = false}
			>
				{@html item.icon}
				{item.label}
			</a>
		{/each}
	</nav>

	<button class="menu-toggle" onclick={() => menuOpen = !menuOpen} aria-label="Menú">
		{#if menuOpen}
			<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
		{:else}
			<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="3" y1="12" x2="21" y2="12"/><line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="18" x2="21" y2="18"/></svg>
		{/if}
	</button>
</header>

<style>
	.app-header {
		display: flex;
		align-items: center;
		justify-content: space-between;
		padding: 0.75rem 1.5rem;
		background: #ffffff;
		border-bottom: 1px solid #e2e8f0;
		position: sticky;
		top: 0;
		z-index: 100;
		box-shadow: 0 1px 3px rgba(0,0,0,0.05);
	}

	.brand {
		display: flex;
		align-items: center;
		gap: 0.75rem;
		flex-shrink: 0;
	}

	.brand-mark {
		width: 38px;
		height: 38px;
		border-radius: 10px;
		background: linear-gradient(135deg, #2563eb, #1d4ed8);
		color: #fff;
		display: grid;
		place-items: center;
		box-shadow: 0 4px 10px rgba(37,99,235,0.35);
	}

	.brand-title {
		font-weight: 800;
		font-size: 0.95rem;
		color: #0f172a;
		letter-spacing: -0.02em;
	}

	.brand-subtitle {
		font-size: 0.75rem;
		color: #94a3b8;
		font-weight: 500;
	}

	.nav {
		display: flex;
		flex-wrap: wrap;
		gap: 0.25rem;
		align-items: center;
	}

	.nav-link {
		display: flex;
		align-items: center;
		gap: 0.4rem;
		padding: 0.45rem 0.75rem;
		border-radius: 0.5rem;
		font-size: 0.85rem;
		font-weight: 600;
		color: #475569;
		transition: all 0.15s;
		text-decoration: none;
	}

	.nav-link:hover {
		background: #f1f5f9;
		color: #0f172a;
		text-decoration: none;
	}

	.nav-link.active {
		background: #eff6ff;
		color: #2563eb;
	}

	.menu-toggle {
		display: none;
		background: none;
		border: 1.5px solid #e2e8f0;
		border-radius: 0.5rem;
		padding: 0.4rem;
		cursor: pointer;
		color: #475569;
	}

	@media (max-width: 900px) {
		.menu-toggle { display: flex; }

		.nav {
			display: none;
			position: absolute;
			top: 100%;
			left: 0;
			right: 0;
			background: white;
			border-bottom: 1px solid #e2e8f0;
			padding: 0.75rem 1.5rem;
			flex-direction: column;
			align-items: flex-start;
			box-shadow: 0 8px 24px rgba(0,0,0,0.08);
		}

		.nav--open { display: flex; }

		.nav-link { width: 100%; }
	}
</style>
