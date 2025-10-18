namespace AgroMarketApi.models
{
    public class Chat
    {
        public int Id { get; set; }
        public int Producto_Id { get; set; }
        public int Productor_Id { get; set; }
        public int Empresa_Id { get; set; }
        public DateTime? Fecha_Creacion { get; set; }
    }
}
