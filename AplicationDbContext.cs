using AppEquiposBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace AppEquiposBackend
{
    public class AplicationDbContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public AplicationDbContext(DbContextOptions<AplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .HasOne(j => j.Team)
                .WithMany(e => e.Players)
                .HasForeignKey(j => j.TeamId)
                .IsRequired()  // Indicamos que la relación es requerida
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración adicional para asegurarse que Equipo es opcional
            modelBuilder.Entity<Player>()
                .Navigation(j => j.Team)
                .AutoInclude();  // Esto incluirá automáticamente el Equipo en las consultas

            base.OnModelCreating(modelBuilder);
        }
    }
}


//modelBuilder.Entity<Jugador>()
//Aquí indicas que vas a configurar la entidad Jugador. Esto permite especificar detalles sobre su relación con otras entidades.

//HasOne(j => j.Equipo)
//Define que cada Jugador tiene una relación de "uno a uno" o "uno a muchos" con la entidad Equipo. Esto significa que un Jugador está asociado a un único Equipo.

//.WithMany(e => e.Jugadores)
//Define que un Equipo puede tener muchos Jugadores. Esta configuración es necesaria para establecer una relación "uno a muchos" entre las dos entidades.

//.HasForeignKey(j => j.EquipoId)
//Especifica que la clave foránea que conecta estas dos entidades es EquipoId en la tabla Jugador. Esto garantiza que la relación entre ambas tablas quede correctamente definida en la base de datos.

//.OnDelete(DeleteBehavior.Cascade)
//Indica que cuando se elimine un Equipo, automáticamente se eliminarán todos los Jugadores asociados a ese equipo. Este comportamiento evita tener registros huérfanos en la tabla Jugador.

//base.OnModelCreating(modelBuilder)
//Llama al método base para que se procesen configuraciones adicionales en caso de que el DbContext padre (o la implementación por defecto de EF Core) tenga configuraciones generales.

