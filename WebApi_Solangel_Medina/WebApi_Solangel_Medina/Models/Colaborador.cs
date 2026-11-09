namespace WebApi_Solangel_Medina.Models
{
    public class Colaborador
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Correo { get; set; } = "";
        public int? DepartamentoId { get; set; }
    }
}
