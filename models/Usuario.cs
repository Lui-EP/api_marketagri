namespace AgroMarketApi.models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = "";      // 'productor' | 'empresa'
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";  // plano para demo
        public string? Ubicacion { get; set; }
        public string? Telefono { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime? Fecha_Registro { get; set; }
        public DateTime? Ultima_Conexion { get; set; }
    }
}
