#!/bin/bash
cat << 'FILE' > Infraestructura/Repositorios/EspecialidadRepositorio.cs
namespace Infraestructura.Repositorios;
using Aplicacion.Abstracciones;
using Dominio.Entidades.Especialidades;
using Infraestructura.Data;
using Microsoft.EntityFrameworkcs;
public class EspecialidadRepositorio : RepositorioGenerico<Especialidad>, IEspecialidadRepositorio
{
    public EspecialidadRepositorio(ContextoECE contexto) : base(contexto)
    {
    }
}
FILE
sed -i 's/public IPacienteRepositorio Pacientes { get; }/public IPacienteRepositorio Pacientes { get; }\n    public IEspecialidadRepositorio Especialidades { get; }/' Infraestructura/Repositorios/UnitOfWork.cs
sed -i 's/Pacientes = new PacienteRepositorio(_contexto);/Pacientes = new PacienteRepositorio(_contexto);\n        Especialidades = new EspecialidadRepositorio(_contexto);/' Infraestructura/Repositorios/UnitOfWork.cs
chmod +x generate_service_and_mapping.sh
./generate_service_and_mapping.sh
