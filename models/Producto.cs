using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroMarketApi.models
{
    [Table("productos")]
    public class Producto
    {
        [Key]
        [Column("id")] // 👈 fuerza a usar minúsculas en la columna
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

        [Column("productorid")]
        public int ProductorId { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
