using Dapper;
using WebApi_Solangel_Medina.Data;
using WebApi_Solangel_Medina.Dtos;
using WebApi_Solangel_Medina.Models;

namespace WebApi_Solangel_Medina.Services
{
    public class SolicitudService
    {
        private readonly DataContext _context;

        public SolicitudService(DataContext context)
        {
            _context = context;
        }

        
        public async Task<IEnumerable<Solicitud>> ObtenerTodos()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Solicitud>(
                "SELECT * FROM Solicitudes ORDER BY Id DESC"
            );
        }

        
        public async Task<Solicitud?> ObtenerPorId(int id)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Solicitud>(
                "SELECT * FROM Solicitudes WHERE Id = @Id",
                new { Id = id }
            );
        }

        
        public async Task<IEnumerable<DetalleSoli>> ObtenerDetalles(int id)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<DetalleSoli>(
                @"SELECT * FROM DetallesSolicitud
              WHERE SolicitudId = @Id",
                new { Id = id }
            );
        }

        
        public async Task<int> Crear(CrearSolicitud dto)
        {
            ValidarDetalles(dto);

            decimal total = dto.Detalles.Sum(
                d => d.Cantidad * d.PrecioUnitario
            );

            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                string sqlSolicitud = @"
                INSERT INTO Solicitudes
                (DepartamentoId, SolicitanteId,
                 Justificacion, Total)
                OUTPUT INSERTED.Id
                VALUES
                (@DepartamentoId, @SolicitanteId,
                 @Justificacion, @Total)";

                int solicitudId =
                    await connection.ExecuteScalarAsync<int>(
                        sqlSolicitud,
                        new
                        {
                            dto.DepartamentoId,
                            dto.SolicitanteId,
                            dto.Justificacion,
                            Total = total
                        },
                        transaction
                    );

                string sqlDetalle = @"
                INSERT INTO DetallesSolicitud
                (SolicitudId, Descripcion,
                 Cantidad, PrecioUnitario)
                VALUES
                (@SolicitudId, @Descripcion,
                 @Cantidad, @PrecioUnitario)";

                foreach (var detalle in dto.Detalles)
                {
                    await connection.ExecuteAsync(
                        sqlDetalle,
                        new
                        {
                            SolicitudId = solicitudId,
                            detalle.Descripcion,
                            detalle.Cantidad,
                            detalle.PrecioUnitario
                        },
                        transaction
                    );
                }

                transaction.Commit();
                return solicitudId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        
        public async Task<bool> Actualizar(
            int id, CrearSolicitud dto)
        {
            ValidarDetalles(dto);

            decimal total = dto.Detalles.Sum(
                d => d.Cantidad * d.PrecioUnitario
            );

            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                int filas = await connection.ExecuteAsync(
                    @"UPDATE Solicitudes
                  SET DepartamentoId = @DepartamentoId,
                      SolicitanteId = @SolicitanteId,
                      Justificacion = @Justificacion,
                      Total = @Total
                  WHERE Id = @Id
                    AND Estado = 'Borrador'",
                    new
                    {
                        Id = id,
                        dto.DepartamentoId,
                        dto.SolicitanteId,
                        dto.Justificacion,
                        Total = total
                    },
                    transaction
                );

                if (filas == 0)
                {
                    transaction.Rollback();
                    return false;
                }

                await connection.ExecuteAsync(
                    @"DELETE FROM DetallesSolicitud
                  WHERE SolicitudId = @Id",
                    new { Id = id },
                    transaction
                );

                foreach (var detalle in dto.Detalles)
                {
                    await connection.ExecuteAsync(
                        @"INSERT INTO DetallesSolicitud
                      (SolicitudId, Descripcion,
                       Cantidad, PrecioUnitario)
                      VALUES
                      (@SolicitudId, @Descripcion,
                       @Cantidad, @PrecioUnitario)",
                        new
                        {
                            SolicitudId = id,
                            detalle.Descripcion,
                            detalle.Cantidad,
                            detalle.PrecioUnitario
                        },
                        transaction
                    );
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        
        public async Task<bool> Eliminar(int id)
        {
            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var estado =
                    await connection.QueryFirstOrDefaultAsync<string>(
                        @"SELECT Estado
                      FROM Solicitudes WITH (UPDLOCK, HOLDLOCK)
                      WHERE Id = @Id",
                        new { Id = id },
                        transaction
                    );

                if (estado != "Borrador")
                {
                    transaction.Rollback();
                    return false;
                }

                await connection.ExecuteAsync(
                    @"DELETE FROM Cotizaciones
                  WHERE SolicitudId = @Id",
                    new { Id = id },
                    transaction
                );

                await connection.ExecuteAsync(
                    @"DELETE FROM DetallesSolicitud
                  WHERE SolicitudId = @Id",
                    new { Id = id },
                    transaction
                );

                int filas = await connection.ExecuteAsync(
                    @"DELETE FROM Solicitudes
                  WHERE Id = @Id AND Estado = 'Borrador'",
                    new { Id = id },
                    transaction
                );

                transaction.Commit();
                return filas > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        
        public async Task<bool> EnviarRevision(int id)
        {
            using var connection = _context.CreateConnection();

            int filas = await connection.ExecuteAsync(
                @"UPDATE Solicitudes
              SET Estado = 'EnRevision'
              WHERE Id = @Id
                AND Estado = 'Borrador'",
                new { Id = id }
            );

            return filas > 0;
        }

       
        public async Task<bool> Aprobar(int id, string etapa)
        {
            string? columna = etapa.ToLowerInvariant() switch
            {
                "departamento" => "AprobacionDepartamento",
                "finanzas" => "AprobacionFinanzas",
                "gerencia" => "AprobacionGerencia",
                _ => null
            };

            if (columna == null)
                return false;

            using var connection = _context.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var solicitud =
                    await connection.QueryFirstOrDefaultAsync<Solicitud>(
                        @"SELECT *
                      FROM Solicitudes WITH (UPDLOCK, HOLDLOCK)
                      WHERE Id = @Id",
                        new { Id = id },
                        transaction
                    );

                if (solicitud == null ||
                    solicitud.Estado != "EnRevision")
                {
                    transaction.Rollback();
                    return false;
                }

                
                if (columna == "AprobacionFinanzas" &&
                    solicitud.Total <= 500)
                {
                    transaction.Rollback();
                    return false;
                }

                if (columna == "AprobacionGerencia" &&
                    solicitud.Total <= 3000)
                {
                    transaction.Rollback();
                    return false;
                }

                
                if (columna == "AprobacionFinanzas" &&
                    solicitud.AprobacionDepartamento != "Aprobado")
                {
                    transaction.Rollback();
                    return false;
                }

                if (columna == "AprobacionGerencia" &&
                    (solicitud.AprobacionDepartamento != "Aprobado" ||
                     solicitud.AprobacionFinanzas != "Aprobado"))
                {
                    transaction.Rollback();
                    return false;
                }

                string estadoEtapa = columna switch
                {
                    "AprobacionDepartamento" =>
                        solicitud.AprobacionDepartamento,
                    "AprobacionFinanzas" =>
                        solicitud.AprobacionFinanzas,
                    _ => solicitud.AprobacionGerencia
                };

                if (estadoEtapa != "Pendiente")
                {
                    transaction.Rollback();
                    return false;
                }

                
                string sql = $@"
                UPDATE Solicitudes
                SET {columna} = 'Aprobado'
                WHERE Id = @Id";

                await connection.ExecuteAsync(
                    sql,
                    new { Id = id },
                    transaction
                );

                
                bool completada = columna switch
                {
                    "AprobacionDepartamento" =>
                        solicitud.Total <= 500,

                    "AprobacionFinanzas" =>
                        solicitud.Total <= 3000,

                    "AprobacionGerencia" => true,

                    _ => false
                };

                if (completada)
                {
                    await connection.ExecuteAsync(
                        @"UPDATE Solicitudes
                      SET Estado = 'Aprobada'
                      WHERE Id = @Id",
                        new { Id = id },
                        transaction
                    );
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        
        public async Task<bool> Rechazar(int id)
        {
            using var connection = _context.CreateConnection();

            int filas = await connection.ExecuteAsync(
                @"UPDATE Solicitudes
              SET Estado = 'Rechazada'
              WHERE Id = @Id
                AND Estado = 'EnRevision'",
                new { Id = id }
            );

            return filas > 0;
        }

        
        private static void ValidarDetalles(CrearSolicitud dto)
        {
            if (dto.Detalles == null || dto.Detalles.Count == 0)
                throw new ArgumentException(
                    "Debe agregar al menos un detalle"
                );

            if (dto.Detalles.Any(
                d => d.Cantidad <= 0 ||
                     d.PrecioUnitario < 0 ||
                     string.IsNullOrWhiteSpace(d.Descripcion)))
            {
                throw new ArgumentException(
                    "Los detalles contienen valores inválidos"
                );
            }
        }
    }
}
