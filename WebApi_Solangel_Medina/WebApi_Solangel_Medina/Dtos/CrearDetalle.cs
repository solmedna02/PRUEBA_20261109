using System.ComponentModel.DataAnnotations;

namespace WebApi_Solangel_Medina.Dtos
{
    public class CrearDetalle
    {
        [Required]
        public string Descripcion { get; set; } = "";

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PrecioUnitario { get; set; }
    }
}
