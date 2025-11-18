using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Domain;

namespace ProyectoFinal.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Estudiante> Estudiantes { get; set; } = default!;
        public DbSet<Termino> Terminos { get; set; } = default!;
        public DbSet<Curso> Cursos { get; set; } = default!;
        public DbSet<Evaluacion> Evaluaciones { get; set; } = default!;
        public DbSet<DocenteCurso> DocenteCursos { get; set; } = default!;
        public DbSet<ActivityLog> ActivityLogs { get; set; } = default!;

        public DbSet<IdentityUserRole<string>> UserRoles { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Curso: Código único
            builder.Entity<Curso>()
                .HasIndex(c => c.Codigo)
                .IsUnique();

            // Estudiante: matrícula, identificación y correo únicos
            builder.Entity<Estudiante>()
                .HasIndex(e => e.CodigoMatricula)
                .IsUnique();

            builder.Entity<Estudiante>()
                .HasIndex(e => e.Identificacion)
                .IsUnique();

            builder.Entity<Estudiante>()
                .HasIndex(e => e.Correo)
                .IsUnique();

            // Relación Curso -> Termino (deja Cascade por defecto si quieres)
            builder.Entity<Curso>()
                .HasOne(c => c.Termino)
                .WithMany(t => t.Cursos)
                .HasForeignKey(c => c.TerminoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Evaluacion -> Estudiante
            builder.Entity<Evaluacion>()
                .HasOne(e => e.Estudiante)
                .WithMany(est => est.Evaluaciones)
                .HasForeignKey(e => e.EstudianteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Evaluacion -> Curso
            builder.Entity<Evaluacion>()
                .HasOne(e => e.Curso)
                .WithMany(c => c.Evaluaciones)
                .HasForeignKey(e => e.CursoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Evaluacion -> Termino
            // AQUÍ rompemos la cascada para evitar multiple cascade paths
            builder.Entity<Evaluacion>()
                .HasOne(e => e.Termino)
                .WithMany(t => t.Evaluaciones)
                .HasForeignKey(e => e.TerminoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Evaluacion.NotaFinal: precisión 5,2
            builder.Entity<Evaluacion>()
                .Property(e => e.NotaFinal)
                .HasPrecision(5, 2);

            // Relación DocenteCurso → Curso
            builder.Entity<DocenteCurso>()
                .HasOne(dc => dc.Curso)
                .WithMany(c => c.Docentes)
                .HasForeignKey(dc => dc.CursoId);
        }
    }
}
