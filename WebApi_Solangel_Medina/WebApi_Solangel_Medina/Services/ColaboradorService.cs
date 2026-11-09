using Dapper;
using WebApi_Solangel_Medina.Data;
using WebApi_Solangel_Medina.Models;

namespace WebApi_Solangel_Medina.Services
{
    public class ColaboradorService
    {
        private readonly DataContext _context;

        public ColaboradorService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Colaborador>> ObtenerTodos()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Colaborador>(
                "SELECT * FROM Colaboradores"
            );
        }

        public async Task<Colaborador?> ObtenerPorId(int id)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Colaborador>(
                "SELECT * FROM Colaboradores WHERE Id = @Id",
                new { Id = id }
            );
        }
    }
}
