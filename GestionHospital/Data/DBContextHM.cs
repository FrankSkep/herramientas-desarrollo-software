using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class DBContextHM : DbContext
{
    public DBContextHM(DbContextOptions<DBContextHM> options) : base(options) { }

    // DbSets
    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<Doctor> Doctores { get; set; }
    public DbSet<Cita> Citas { get; set; }
    public DbSet<Departamento> Departamentos { get; set; }
    public DbSet<HistorialMedico> HistorialesMedicos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ============================================
        // CONFIGURACIÓN DE PACIENTE
        // ============================================
        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.ToTable("Pacientes");
            entity.HasKey(e => e.IdPaciente);

            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.FechaNacimiento)
                .IsRequired();

            entity.Property(e => e.Genero)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.Telefono)
                .HasMaxLength(15);

            entity.Property(e => e.Direccion)
                .HasMaxLength(255);

            entity.Property(e => e.TipoSangre)
                .HasMaxLength(5);

            entity.Property(e => e.FechaRegistro)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Relaciones: Paciente tiene muchas Citas
            entity.HasMany(p => p.Citas)
                .WithOne(c => c.Paciente)
                .HasForeignKey(c => c.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            // Relaciones: Paciente tiene muchos Historiales Médicos
            entity.HasMany(p => p.HistorialesMedicos)
                .WithOne(h => h.Paciente)
                .HasForeignKey(h => h.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ============================================
        // CONFIGURACIÓN DE DEPARTAMENTO
        // ============================================
        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.ToTable("Departamentos");
            entity.HasKey(e => e.IdDepartamento);

            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            // Índice único para el nombre del departamento
            entity.HasIndex(e => e.Nombre)
                .IsUnique()
                .HasDatabaseName("IX_Departamentos_Nombre_Unique");

            entity.Property(e => e.Descripcion)
                .HasColumnType("text");

            entity.Property(e => e.FechaCreacion)
                .HasColumnType("date");

            // Relaciones: Departamento tiene muchos Doctores
            entity.HasMany(d => d.Doctores)
                .WithOne(doc => doc.Departamento)
                .HasForeignKey(doc => doc.IdDepartamento)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ============================================
        // CONFIGURACIÓN DE DOCTOR
        // ============================================
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctores");
            entity.HasKey(e => e.IdDoctor);

            entity.Property(e => e.IdDepartamento)
                .IsRequired();

            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Especialidad)
                .HasMaxLength(100);

            entity.Property(e => e.Telefono)
                .HasMaxLength(15);

            entity.Property(e => e.Email)
                .HasMaxLength(100);

            entity.Property(e => e.FeechaContratacion)
                .HasColumnType("date");

            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValue(true);

            // Relación: Doctor pertenece a un Departamento
            entity.HasOne(d => d.Departamento)
                .WithMany(dep => dep.Doctores)
                .HasForeignKey(d => d.IdDepartamento)
                .OnDelete(DeleteBehavior.Restrict);

            // Relaciones: Doctor tiene muchas Citas
            entity.HasMany(d => d.Citas)
                .WithOne(c => c.Doctor)
                .HasForeignKey(c => c.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);

            // Relaciones: Doctor tiene muchos Historiales Médicos
            entity.HasMany(d => d.HistorialesMedicos)
                .WithOne(h => h.Doctor)
                .HasForeignKey(h => h.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ============================================
        // CONFIGURACIÓN DE CITA
        // ============================================
        modelBuilder.Entity<Cita>(entity =>
        {
            entity.ToTable("Citas");
            entity.HasKey(e => e.IdCita);

            entity.Property(e => e.IdPaciente)
                .IsRequired();

            entity.Property(e => e.IdDoctor)
                .IsRequired();

            entity.Property(e => e.FechaCita)
                .IsRequired()
                .HasColumnType("datetime");

            entity.Property(e => e.Motivo)
                .HasColumnType("text");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Programada");

            entity.Property(e => e.FechaRegistro)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Relación: Cita pertenece a un Paciente
            entity.HasOne(c => c.Paciente)
                .WithMany(p => p.Citas)
                .HasForeignKey(c => c.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación: Cita pertenece a un Doctor
            entity.HasOne(c => c.Doctor)
                .WithMany(d => d.Citas)
                .HasForeignKey(c => c.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice compuesto para búsquedas eficientes
            entity.HasIndex(e => new { e.IdPaciente, e.FechaCita })
                .HasDatabaseName("IX_Citas_Paciente_Fecha");

            entity.HasIndex(e => new { e.IdDoctor, e.FechaCita })
                .HasDatabaseName("IX_Citas_Doctor_Fecha");
        });

        // ============================================
        // CONFIGURACIÓN DE HISTORIAL MÉDICO
        // ============================================
        modelBuilder.Entity<HistorialMedico>(entity =>
        {
            entity.ToTable("Historial_Medico");
            entity.HasKey(e => e.IdHistorial);

            entity.Property(e => e.IdPaciente)
                .IsRequired();

            entity.Property(e => e.IdDoctor)
                .IsRequired();

            entity.Property(e => e.FechaConsulta)
                .IsRequired()
                .HasColumnType("datetime");

            entity.Property(e => e.Diagnostico)
                .HasColumnType("text");

            entity.Property(e => e.Tratamiento)
                .HasColumnType("text");

            entity.Property(e => e.Medicamentos)
                .HasColumnType("text");

            entity.Property(e => e.Notas)
                .HasColumnType("text");

            // Relación: HistorialMedico pertenece a un Paciente
            entity.HasOne(h => h.Paciente)
                .WithMany(p => p.HistorialesMedicos)
                .HasForeignKey(h => h.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación: HistorialMedico pertenece a un Doctor
            entity.HasOne(h => h.Doctor)
                .WithMany(d => d.HistorialesMedicos)
                .HasForeignKey(h => h.IdDoctor)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice para búsquedas por paciente y fecha
            entity.HasIndex(e => new { e.IdPaciente, e.FechaConsulta })
                .HasDatabaseName("IX_Historial_Paciente_Fecha");

            // Índice para búsquedas por doctor y fecha
            entity.HasIndex(e => new { e.IdDoctor, e.FechaConsulta })
                .HasDatabaseName("IX_Historial_Doctor_Fecha");
        });
    }
}