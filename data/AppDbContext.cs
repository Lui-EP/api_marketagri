using Microsoft.EntityFrameworkCore;
using AgroMarketApi.models;

namespace AgroMarketApi.data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Interes>  Intereses => Set<Interes>();
        public DbSet<Chat>     Chats => Set<Chat>();
        public DbSet<Mensaje>  Mensajes => Set<Mensaje>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔠 Convierte todas las tablas y columnas a minúsculas
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName()!.ToLower());

                foreach (var property in entity.GetProperties())
                    property.SetColumnName(property.GetColumnName()!.ToLower());
                    modelBuilder.Entity<Producto>().Property(p => p.Activo)
    .HasColumnName("activo")
    .HasDefaultValue(true);

            }

            // 🔗 Mapear tablas específicas (por si acaso)
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<Producto>().ToTable("productos");
            modelBuilder.Entity<Interes>().ToTable("intereses");
            modelBuilder.Entity<Chat>().ToTable("chats");
            modelBuilder.Entity<Mensaje>().ToTable("mensajes");

            // 🧩 Mapear columnas personalizadas
            modelBuilder.Entity<Usuario>().Property(p => p.Fecha_Registro).HasColumnName("fecha_registro");
            modelBuilder.Entity<Usuario>().Property(p => p.Ultima_Conexion).HasColumnName("ultima_conexion");

            modelBuilder.Entity<Producto>().Property(p => p.Productor_Id).HasColumnName("productor_id");
            modelBuilder.Entity<Producto>().Property(p => p.Fecha_Publicacion).HasColumnName("fecha_publicacion");

            modelBuilder.Entity<Interes>().Property(p => p.Producto_Id).HasColumnName("producto_id");
            modelBuilder.Entity<Interes>().Property(p => p.Empresa_Id).HasColumnName("empresa_id");
            modelBuilder.Entity<Interes>().Property(p => p.Fecha_Interes).HasColumnName("fecha_interes");

            modelBuilder.Entity<Chat>().Property(p => p.Producto_Id).HasColumnName("producto_id");
            modelBuilder.Entity<Chat>().Property(p => p.Productor_Id).HasColumnName("productor_id");
            modelBuilder.Entity<Chat>().Property(p => p.Empresa_Id).HasColumnName("empresa_id");
            modelBuilder.Entity<Chat>().Property(p => p.Fecha_Creacion).HasColumnName("fecha_creacion");

            modelBuilder.Entity<Mensaje>().Property(p => p.Chat_Id).HasColumnName("chat_id");
            modelBuilder.Entity<Mensaje>().Property(p => p.Remitente_Id).HasColumnName("remitente_id");
            modelBuilder.Entity<Mensaje>().Property(p => p.Mensaje_Texto).HasColumnName("mensaje");
            modelBuilder.Entity<Mensaje>().Property(p => p.Fecha_Envio).HasColumnName("fecha_envio");

            base.OnModelCreating(modelBuilder);
        }
    }
}
