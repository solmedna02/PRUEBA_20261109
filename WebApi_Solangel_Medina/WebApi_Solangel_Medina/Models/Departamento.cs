namespace WebApi_Solangel_Medina.Models
{
    public class Departamento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public int? ResponsableId { get; set; }
        public decimal PresupuestoAnual { get; set; }
    }
}
