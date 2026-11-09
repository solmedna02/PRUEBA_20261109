using Dapper;
using WebApi_Solangel_Medina.Data;
using WebApi_Solangel_Medina.Models;

namespace WebApi_Solangel_Medina.Services
{
    public class DepartamentoService
    {
        private readonly DataContext _context;

        public DepartamentoService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Departamento>> ObtenerTodos()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Departamento>(
                "SELECT * FROM Departamentos"
            );
        }

        public async Task<Departamento?> ObtenerPorId(int id)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Departamento>(
                "SELECT * FROM Departamentos WHERE Id = @Id",
                new { Id = id }
            );
        }
    }
}
