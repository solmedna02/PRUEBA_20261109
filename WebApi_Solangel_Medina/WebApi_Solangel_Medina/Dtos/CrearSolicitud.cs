using System.ComponentModel.DataAnnotations;

namespace WebApi_Solangel_Medina.Dtos
{
    public class CrearSolicitud
    {
        public int DepartamentoId { get; set; }
        public int SolicitanteId { get; set; }

        [Required]
        public string Justificacion { get; set; } = "";

        [MinLength(1)]
        public List<CrearDetalle> Detalles { get; set; } = new();
    }
}
