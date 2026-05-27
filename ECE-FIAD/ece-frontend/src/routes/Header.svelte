<script lang="ts">
	import { page } from '$app/state';
</script>

<header class="app-header">
	<div class="brand">
		<div class="brand-mark">ECE</div>
		<div>
			<div class="brand-title">ECE-FIAD</div>
			<div class="brand-subtitle">Gestor clinico</div>
		</div>
	</div>
	<nav class="nav">
		<a class:active={page.url.pathname === '/'} href="/">Inicio</a>
		<a class:active={page.url.pathname.startsWith('/pacientes')} href="/pacientes">Pacientes</a>
		<a class:active={page.url.pathname.startsWith('/doctores')} href="/doctores">Doctores</a>
		<a class:active={page.url.pathname.startsWith('/especialidades')} href="/especialidades">Especialidades</a>
		<a class:active={page.url.pathname.startsWith('/citas')} href="/citas">Citas</a>
		<a class:active={page.url.pathname.startsWith('/historias-clinicas')} href="/historias-clinicas">Historias</a>
		<a class:active={page.url.pathname.startsWith('/evoluciones')} href="/evoluciones">Evoluciones</a>
	</nav>
</header>

<style>
	.app-header {
		display: flex;
		flex-wrap: wrap;
		align-items: center;
		justify-content: space-between;
		gap: 1rem;
		padding: 1rem 1.5rem;
		background: #ffffff;
		border-bottom: 1px solid var(--app-border);
		position: sticky;
		top: 0;
		z-index: 10;
	}

	.brand {
		display: flex;
		align-items: center;
		gap: 0.75rem;
	}

	.brand-mark {
		width: 40px;
		height: 40px;
		border-radius: 12px;
		background: #2563eb;
		color: #fff;
		display: grid;
		place-items: center;
		font-weight: 700;
	}

	.brand-title {
		font-weight: 700;
	}

	.brand-subtitle {
		font-size: 0.85rem;
		color: #64748b;
	}

	.nav {
		display: flex;
		flex-wrap: wrap;
		gap: 0.75rem;
		font-weight: 600;
		font-size: 0.95rem;
	}

	.nav a {
		padding: 0.35rem 0.6rem;
		border-radius: 999px;
		background: transparent;
	}

	.nav a.active {
		background: #e0e7ff;
		color: #1d4ed8;
	}
</style>
