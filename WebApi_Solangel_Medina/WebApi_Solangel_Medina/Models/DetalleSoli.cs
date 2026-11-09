namespace WebApi_Solangel_Medina.Models
{
    public class DetalleSoli
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public string Descripcion { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
