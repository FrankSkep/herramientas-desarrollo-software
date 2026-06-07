import * as pacientes from './pacientes';
import * as doctores from './doctores';
import * as especialidades from './especialidades';
import * as historias from './historias';
import * as evoluciones from './evoluciones';
import * as citas from './citas';

export const api = {
	...pacientes,
	...doctores,
	...especialidades,
	...historias,
	...evoluciones,
	...citas
};
