# ECE-FIAD Frontend

Frontend SvelteKit para la API ECE-FIAD. Incluye CRUD de pacientes, doctores, especialidades, citas, historias clinicas y evoluciones, con validaciones alineadas al backend.

## Requisitos

- Node.js 18+
- API backend en ejecucion

## Configuracion

El frontend usa la variable `PUBLIC_API_BASE_URL` para apuntar al backend. Si no se define, usara rutas relativas (`/api`).

Ejemplo:

```sh
PUBLIC_API_BASE_URL=http://localhost:5000
```

## Desarrollo

```sh
npm install
npm run dev
```

## Pruebas

```sh
npm run test
```

## Build

```sh
npm run build
```

