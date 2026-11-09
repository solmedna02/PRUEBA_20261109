namespace WebApi_Solangel_Medina.Models
{
    public class Cotizacion
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public string Proveedor { get; set; } = "";
        public DateTime FechaVigencia { get; set; }
        public string? CondicionesPago { get; set; }
    }
}
