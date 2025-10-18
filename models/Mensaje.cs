namespace AgroMarketApi.models
{
    public class Mensaje
    {
        public int Id { get; set; }
        public int Chat_Id { get; set; }
        public int Remitente_Id { get; set; }
        public string Mensaje_Texto { get; set; } = "";
        public DateTime? Fecha_Envio { get; set; }
        public bool Leido { get; set; } = false;
    }
}
