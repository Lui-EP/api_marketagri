namespace AgroMarketApi.models
{
    public class Producto
    {
        public int Id { get; set; }
        public int Productor_Id { get; set; }
        public string Nombre { get; set; } = "";
        public decimal Precio { get; set; }
        public decimal Volumen { get; set; }     // toneladas
        public string? Ubicacion { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? Fecha_Publicacion { get; set; }
        public bool Activo { get; set; } = true;
    }
}
