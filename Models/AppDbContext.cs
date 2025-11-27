using Microsoft.EntityFrameworkCore;
using Hotel.Models;

namespace Hotel.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) {}

        // DbSets
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Huesped> Huespedes { get; set; }
        public DbSet<TipoHabitacion> TiposHabitacion { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Habitacion> Habitaciones { get; set; }
        public DbSet<HabitacionServicio> HabitacionServicios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<TemporadaPrecio> TemporadasPrecio { get; set; }
        public DbSet<TemporadaHabitacionPrecio> TemporadaHabitacionPrecios { get; set; }
        public DbSet<HabitacionFoto> HabitacionFotos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Rol);

                entity.Property(e => e.CreadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ActualizadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasMany(e => e.ReservasCreadas)
                    .WithOne(r => r.UsuarioCreador)
                    .HasForeignKey(r => r.CreadoPor)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.ReservasCanceladas)
                    .WithOne(r => r.UsuarioCancelador)
                    .HasForeignKey(r => r.CanceladoPor)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            
            modelBuilder.Entity<Huesped>(entity =>
            {
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.NumeroDocumento).IsUnique();

                entity.Property(e => e.CreadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ActualizadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            
            modelBuilder.Entity<TipoHabitacion>(entity =>
            {
                entity.HasIndex(e => e.Nombre).IsUnique();

                entity.Property(e => e.CreadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

           
            modelBuilder.Entity<Servicio>(entity =>
            {
                entity.HasIndex(e => e.Nombre).IsUnique();

                entity.Property(e => e.CreadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            
            modelBuilder.Entity<Habitacion>(entity =>
            {
                entity.HasIndex(e => e.NumeroHabitacion).IsUnique();
                entity.HasIndex(e => e.TipoHabitacionId);
                entity.HasIndex(e => e.Estado);

                entity.Property(e => e.Estado)
                    .HasDefaultValue("disponible");

                entity.Property(e => e.CreadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ActualizadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.TipoHabitacion)
                    .WithMany(t => t.Habitaciones)
                    .HasForeignKey(e => e.TipoHabitacionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            
            modelBuilder.Entity<HabitacionServicio>(entity =>
            {
                entity.HasKey(hs => new { hs.HabitacionId, hs.ServicioId });

                entity.HasOne(hs => hs.Habitacion)
                    .WithMany(h => h.HabitacionServicios)
                    .HasForeignKey(hs => hs.HabitacionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(hs => hs.Servicio)
                    .WithMany(s => s.HabitacionServicios)
                    .HasForeignKey(hs => hs.ServicioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            
            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.HasIndex(e => e.HabitacionId);
                entity.HasIndex(e => e.HuespedId);
                entity.HasIndex(e => e.Estado);
                entity.HasIndex(e => new { e.FechaEntrada, e.FechaSalida });

                entity.Property(e => e.Estado)
                    .HasDefaultValue("pendiente");

                entity.Property(e => e.NumeroHuespedes)
                    .HasDefaultValue((short)1);

                entity.Property(e => e.CreadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ActualizadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.NumeroNoches)
                    .HasComputedColumnSql("DATEDIFF(DAY, fecha_entrada, fecha_salida)", stored: true);


                entity.HasOne(e => e.Habitacion)
                    .WithMany(h => h.Reservas)
                    .HasForeignKey(e => e.HabitacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Huesped)
                    .WithMany(h => h.Reservas)
                    .HasForeignKey(e => e.HuespedId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Reservas_FechaSalida", "fecha_salida > fecha_entrada");
                    t.HasCheckConstraint("CK_Reservas_NumeroHuespedes", "numero_huespedes > 0");
                    t.HasCheckConstraint("CK_Reservas_PrecioPorNoche", "precio_por_noche > 0");
                    t.HasCheckConstraint("CK_Reservas_PrecioTotal", "precio_total > 0");
                });
            });

            
            modelBuilder.Entity<Pago>(entity =>
            {
                entity.HasIndex(e => e.ReservaId);
                entity.HasIndex(e => e.Estado);

                entity.Property(e => e.Estado)
                    .HasDefaultValue("pendiente");

                entity.Property(e => e.PagadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Reserva)
                    .WithMany(r => r.Pagos)
                    .HasForeignKey(e => e.ReservaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.UsuarioProcesador)
                    .WithMany(u => u.PagosProcesados)
                    .HasForeignKey(e => e.ProcesadoPor)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Pagos_Monto", "monto > 0");
                });
            });

            
            modelBuilder.Entity<TemporadaPrecio>(entity =>
            {
                entity.HasIndex(e => new { e.FechaInicio, e.FechaFin });

                entity.Property(e => e.FactorMultiplicador)
                    .HasDefaultValue(1.00m);

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ActualizadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Temporadas_FechaFin", "fecha_fin >= fecha_inicio");
                    t.HasCheckConstraint("CK_Temporadas_Factor", "factor_multiplicador > 0");
                });
            });

            
            modelBuilder.Entity<TemporadaHabitacionPrecio>(entity =>
            {
                entity.HasIndex(e => e.HabitacionId);
                entity.HasIndex(e => e.TemporadaId);
                entity.HasIndex(e => new { e.TemporadaId, e.HabitacionId }).IsUnique();

                entity.Property(e => e.CreadoEn)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Temporada)
                    .WithMany(t => t.HabitacionPrecios)
                    .HasForeignKey(e => e.TemporadaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Habitacion)
                    .WithMany(h => h.TemporadaPrecios)
                    .HasForeignKey(e => e.HabitacionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_TemporadaPrecios_Precio", "precio_override > 0");
                });
            });
        }
    }
}
