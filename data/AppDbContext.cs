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

        protected override void OnModelCreating(ModelBuilder b)
        {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Convierte automáticamente todo a minúsculas
    foreach (var entity in modelBuilder.Model.GetEntityTypes())
    {
        entity.SetTableName(entity.GetTableName()!.ToLower());

        foreach (var property in entity.GetProperties())
            property.SetColumnName(property.GetColumnName()!.ToLower());
    }

    base.OnModelCreating(modelBuilder);
}

            // Mapear nombres de tablas reales (minúsculas)
            b.Entity<Usuario>().ToTable("usuarios");
            b.Entity<Producto>().ToTable("productos");
            b.Entity<Interes>().ToTable("intereses");
            b.Entity<Chat>().ToTable("chats");
            b.Entity<Mensaje>().ToTable("mensajes");

            // Mapear columnas que tienen guión bajo en DB
            b.Entity<Usuario>().Property(p => p.Fecha_Registro).HasColumnName("fecha_registro");
            b.Entity<Usuario>().Property(p => p.Ultima_Conexion).HasColumnName("ultima_conexion");

            b.Entity<Producto>().Property(p => p.Productor_Id).HasColumnName("productor_id");
            b.Entity<Producto>().Property(p => p.Fecha_Publicacion).HasColumnName("fecha_publicacion");

            b.Entity<Interes>().Property(p => p.Producto_Id).HasColumnName("producto_id");
            b.Entity<Interes>().Property(p => p.Empresa_Id).HasColumnName("empresa_id");
            b.Entity<Interes>().Property(p => p.Fecha_Interes).HasColumnName("fecha_interes");

            b.Entity<Chat>().Property(p => p.Producto_Id).HasColumnName("producto_id");
            b.Entity<Chat>().Property(p => p.Productor_Id).HasColumnName("productor_id");
            b.Entity<Chat>().Property(p => p.Empresa_Id).HasColumnName("empresa_id");
            b.Entity<Chat>().Property(p => p.Fecha_Creacion).HasColumnName("fecha_creacion");

            b.Entity<Mensaje>().Property(p => p.Chat_Id).HasColumnName("chat_id");
            b.Entity<Mensaje>().Property(p => p.Remitente_Id).HasColumnName("remitente_id");
            b.Entity<Mensaje>().Property(p => p.Mensaje_Texto).HasColumnName("mensaje");
            b.Entity<Mensaje>().Property(p => p.Fecha_Envio).HasColumnName("fecha_envio");
        }
    }
}
