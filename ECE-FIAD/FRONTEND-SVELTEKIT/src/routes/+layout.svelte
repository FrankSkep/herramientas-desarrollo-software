<script lang="ts">
	import Header from './Header.svelte';
	import Toaster from '$lib/components/Toaster.svelte';
	import './layout.css';

	let { children } = $props();
</script>

<div class="app">
	<Header />
	<main class="container">{@render children()}</main>
</div>
<Toaster />

<style>
	.app {
		display: flex;
		flex-direction: column;
		min-height: 100vh;
	}

	main {
		flex: 1;
	}
</style>
