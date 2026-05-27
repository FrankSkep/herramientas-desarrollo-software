<svelte:head>
	<title>ECE-FIAD</title>
	<meta name="description" content="Gestor clinico ECE-FIAD" />
</svelte:head>

<section class="card">
	<h1 class="text-2xl font-semibold">Panel principal</h1>
	<div class="grid gap-4 mt-6 md:grid-cols-2 lg:grid-cols-3">
		<a class="card" href="/pacientes">
			<h2 class="text-lg font-semibold">Pacientes</h2>
		</a>
		<a class="card" href="/doctores">
			<h2 class="text-lg font-semibold">Doctores</h2>
		</a>
		<a class="card" href="/especialidades">
			<h2 class="text-lg font-semibold">Especialidades</h2>
		</a>
		<a class="card" href="/citas">
			<h2 class="text-lg font-semibold">Citas</h2>
		</a>
		<a class="card" href="/historias-clinicas">
			<h2 class="text-lg font-semibold">Historias clinicas</h2>
		</a>
		<a class="card" href="/evoluciones">
			<h2 class="text-lg font-semibold">Evoluciones</h2>
		</a>
	</div>
</section>

