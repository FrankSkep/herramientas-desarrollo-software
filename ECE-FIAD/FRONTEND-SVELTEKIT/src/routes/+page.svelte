<svelte:head>
	<title>ECE-FIAD — Gestor Clínico</title>
	<meta name="description" content="Gestor clínico ECE-FIAD" />
</svelte:head>

<div class="home">
	<div class="hero">
		<div class="hero__badge">Sistema de Gestión Médica</div>
		<h1 class="hero__title">ECE-FIAD <span>Gestor Clínico</span></h1>
		<p class="hero__subtitle">Administra pacientes, doctores, citas e historias clínicas desde un solo lugar.</p>
	</div>

	<div class="modules-grid">
		<a class="module-card module--blue" href="/pacientes">
			<div class="module-card__icon">
				<svg width="26" height="26" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 00-3-3.87"/><path d="M16 3.13a4 4 0 010 7.75"/></svg>
			</div>
			<div class="module-card__content">
				<h2>Pacientes</h2>
				<p>Registro y gestión del padrón de pacientes</p>
			</div>
			<div class="module-card__arrow">
				<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="5" y1="12" x2="19" y2="12"/><polyline points="12 5 19 12 12 19"/></svg>
			</div>
		</a>

		<a class="module-card module--green" href="/doctores">
			<div class="module-card__icon">
				<svg width="26" height="26" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 7h-9"/><path d="M14 17H5"/><circle cx="17" cy="17" r="3"/><circle cx="7" cy="7" r="3"/></svg>
			</div>
			<div class="module-card__content">
				<h2>Doctores</h2>
				<p>Gestión del equipo médico</p>
			</div>
			<div class="module-card__arrow">
				<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="5" y1="12" x2="19" y2="12"/><polyline points="12 5 19 12 12 19"/></svg>
			</div>
		</a>

		<a class="module-card module--purple" href="/especialidades">
			<div class="module-card__icon">
				<svg width="26" height="26" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="2" y="3" width="20" height="14" rx="2"/><line x1="8" y1="21" x2="16" y2="21"/><line x1="12" y1="17" x2="12" y2="21"/></svg>
			</div>
			<div class="module-card__content">
				<h2>Especialidades</h2>
				<p>Catálogo de especialidades médicas</p>
			</div>
			<div class="module-card__arrow">
				<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="5" y1="12" x2="19" y2="12"/><polyline points="12 5 19 12 12 19"/></svg>
			</div>
		</a>

		<a class="module-card module--amber" href="/citas">
			<div class="module-card__icon">
				<svg width="26" height="26" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
			</div>
			<div class="module-card__content">
				<h2>Citas</h2>
				<p>Agenda y programación de consultas</p>
			</div>
			<div class="module-card__arrow">
				<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="5" y1="12" x2="19" y2="12"/><polyline points="12 5 19 12 12 19"/></svg>
			</div>
		</a>

		<a class="module-card module--sky" href="/historias-clinicas">
			<div class="module-card__icon">
				<svg width="26" height="26" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="12" y1="18" x2="12" y2="12"/><line x1="9" y1="15" x2="15" y2="15"/></svg>
			</div>
			<div class="module-card__content">
				<h2>Historias Clínicas</h2>
				<p>Registros médicos por paciente</p>
			</div>
			<div class="module-card__arrow">
				<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="5" y1="12" x2="19" y2="12"/><polyline points="12 5 19 12 12 19"/></svg>
			</div>
		</a>

		<a class="module-card module--pink" href="/evoluciones">
			<div class="module-card__icon">
				<svg width="26" height="26" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/></svg>
			</div>
			<div class="module-card__content">
				<h2>Evoluciones</h2>
				<p>Seguimiento clínico y diagnósticos</p>
			</div>
			<div class="module-card__arrow">
				<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="5" y1="12" x2="19" y2="12"/><polyline points="12 5 19 12 12 19"/></svg>
			</div>
		</a>
	</div>
</div>

<style>
	.home { animation: fadeUp 0.4s ease; }

	@keyframes fadeUp {
		from { opacity:0; transform:translateY(12px); }
		to   { opacity:1; transform:translateY(0); }
	}

	.hero {
		text-align: center;
		padding: 3rem 1rem 2.5rem;
	}

	.hero__badge {
		display: inline-flex;
		align-items: center;
		padding: 0.3rem 0.9rem;
		border-radius: 999px;
		background: #eff6ff;
		color: #2563eb;
		font-size: 0.8rem;
		font-weight: 700;
		letter-spacing: 0.05em;
		text-transform: uppercase;
		margin-bottom: 1rem;
		border: 1px solid #bfdbfe;
	}

	.hero__title {
		font-size: clamp(2rem, 5vw, 3rem);
		font-weight: 800;
		color: #0f172a;
		margin: 0 0 0.75rem;
		letter-spacing: -0.04em;
		line-height: 1.1;
	}

	.hero__title span {
		background: linear-gradient(135deg, #2563eb, #7c3aed);
		-webkit-background-clip: text;
		-webkit-text-fill-color: transparent;
		background-clip: text;
	}

	.hero__subtitle {
		font-size: 1.05rem;
		color: #64748b;
		max-width: 480px;
		margin: 0 auto;
		line-height: 1.6;
	}

	.modules-grid {
		display: grid;
		grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
		gap: 1rem;
	}

	.module-card {
		display: flex;
		align-items: center;
		gap: 1rem;
		padding: 1.25rem 1.25rem 1.25rem 1.25rem;
		background: #fff;
		border: 1.5px solid #e2e8f0;
		border-radius: 1rem;
		text-decoration: none;
		color: inherit;
		transition: all 0.2s cubic-bezier(0.34, 1.2, 0.64, 1);
		box-shadow: 0 1px 3px rgba(0,0,0,0.04);
		position: relative;
		overflow: hidden;
	}

	.module-card::before {
		content: '';
		position: absolute;
		inset: 0;
		opacity: 0;
		transition: opacity 0.2s;
	}

	.module-card:hover {
		transform: translateY(-3px);
		box-shadow: 0 12px 32px rgba(0,0,0,0.1);
		text-decoration: none;
	}

	.module-card:hover .module-card__arrow {
		transform: translateX(4px);
		opacity: 1;
	}

	.module-card__icon {
		width: 52px;
		height: 52px;
		border-radius: 14px;
		display: grid;
		place-items: center;
		flex-shrink: 0;
		transition: transform 0.2s;
	}

	.module-card:hover .module-card__icon {
		transform: scale(1.08);
	}

	.module-card__content { flex: 1; }

	.module-card__content h2 {
		font-size: 1rem;
		font-weight: 700;
		color: #0f172a;
		margin: 0 0 0.2rem;
		letter-spacing: -0.01em;
	}

	.module-card__content p {
		font-size: 0.825rem;
		color: #64748b;
		margin: 0;
		line-height: 1.4;
	}

	.module-card__arrow {
		flex-shrink: 0;
		opacity: 0.3;
		transition: all 0.2s;
		color: #94a3b8;
	}

	/* Color variants */
	.module--blue .module-card__icon   { background: #eff6ff; color: #2563eb; }
	.module--blue:hover                 { border-color: #bfdbfe; }

	.module--green .module-card__icon  { background: #f0fdf4; color: #16a34a; }
	.module--green:hover                { border-color: #bbf7d0; }

	.module--purple .module-card__icon { background: #faf5ff; color: #7c3aed; }
	.module--purple:hover               { border-color: #ddd6fe; }

	.module--amber .module-card__icon  { background: #fffbeb; color: #d97706; }
	.module--amber:hover                { border-color: #fde68a; }

	.module--sky .module-card__icon    { background: #f0f9ff; color: #0284c7; }
	.module--sky:hover                  { border-color: #bae6fd; }

	.module--pink .module-card__icon   { background: #fdf2f8; color: #be185d; }
	.module--pink:hover                 { border-color: #fbcfe8; }
</style>
