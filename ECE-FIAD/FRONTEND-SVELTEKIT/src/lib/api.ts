import * as pacientes from './api/pacientes';
import * as doctores from './api/doctores';
import * as especialidades from './api/especialidades';
import * as historias from './api/historias';
import * as evoluciones from './api/evoluciones';
import * as citas from './api/citas';

export const api = {
	...pacientes,
	...doctores,
	...especialidades,
	...historias,
	...evoluciones,
	...citas
};
