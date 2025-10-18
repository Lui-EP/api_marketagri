namespace AgroMarketApi.models
{
    public class Interes
    {
        public int Id { get; set; }
        public int Producto_Id { get; set; }
        public int Empresa_Id { get; set; }
        public string? Notas { get; set; }
        public DateTime? Fecha_Interes { get; set; }
        public bool Activo { get; set; } = true;
    }
}
