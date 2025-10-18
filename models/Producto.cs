using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMarketApi.models
{
    [Table("productos")]
    public class Producto
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("volumen")]
        public decimal Volumen { get; set; }

        [Column("ubicacion")]
        public string? Ubicacion { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        // 🔴 Este campo faltaba y lo pide el controlador
        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Nombres que ya mapeaste en AppDbContext
        [Column("productor_id")]
        public int Productor_Id { get; set; }

        [Column("fecha_publicacion")]
        public DateTime Fecha_Publicacion { get; set; } = DateTime.UtcNow;
    }
}
