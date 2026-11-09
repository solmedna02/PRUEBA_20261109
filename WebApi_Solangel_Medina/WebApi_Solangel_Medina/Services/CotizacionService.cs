using Dapper;
using WebApi_Solangel_Medina.Data;
using WebApi_Solangel_Medina.Models;

namespace WebApi_Solangel_Medina.Services
{
    public class CotizacionService
    {
        private readonly DataContext _context;

        public CotizacionService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cotizacion>> ObtenerTodos()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Cotizacion>(
                "SELECT * FROM Cotizaciones ORDER BY Id DESC"
            );
        }

        public async Task<IEnumerable<Cotizacion>> ObtenerPorSolicitud(
            int solicitudId)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Cotizacion>(
                @"SELECT * FROM Cotizaciones
              WHERE SolicitudId = @SolicitudId",
                new { SolicitudId = solicitudId }
            );
        }

        public async Task<int> Crear(Cotizacion cotizacion)
        {
            using var connection = _context.CreateConnection();

            return await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO Cotizaciones
              (SolicitudId, Proveedor, FechaVigencia, CondicionesPago)
              OUTPUT INSERTED.Id
              VALUES
              (@SolicitudId, @Proveedor,
               @FechaVigencia, @CondicionesPago)",
                cotizacion
            );
        }

        public async Task<bool> Eliminar(int id)
        {
            using var connection = _context.CreateConnection();

            int filas = await connection.ExecuteAsync(
                "DELETE FROM Cotizaciones WHERE Id = @Id",
                new { Id = id }
            );

            return filas > 0;
        }
    }
}
