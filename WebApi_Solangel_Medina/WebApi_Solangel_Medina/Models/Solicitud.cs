namespace WebApi_Solangel_Medina.Models
{
    public class Solicitud
    {
        public int Id { get; set; }
        public int DepartamentoId { get; set; }
        public int SolicitanteId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Justificacion { get; set; } = "";
        public string Estado { get; set; } = "Borrador";
        public decimal Total { get; set; }

        public string AprobacionDepartamento { get; set; } = "Pendiente";
        public string AprobacionFinanzas { get; set; } = "Pendiente";
        public string AprobacionGerencia { get; set; } = "Pendiente";
    }
}
