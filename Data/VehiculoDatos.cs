using Microsoft.Data.SqlClient;
using System.Data;
using TallerMecanicoAPI.Models;

namespace TallerMecanicoAPI.Data
{
    public class VehiculoDatos
    {
        private readonly Conexion _conexion;

        public VehiculoDatos(Conexion conexion)
        {
            _conexion = conexion;
        }

        public async Task<List<Vehiculo>> ListarAsync()
        {
            var lista = new List<Vehiculo>();

            using (var cn = _conexion.CrearConexion())
            {
                const string query = "SELECT Placa, Marca, Anio FROM Vehiculos";
                using (var cmd = new SqlCommand(query, cn))
                {
                    cmd.CommandType = CommandType.Text;
                    await cn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new Vehiculo
                            {
                                Placa = reader.GetString("Placa"),
                                Marca = reader.GetString("Marca"),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Anio"))
                                       ? null
                                       : reader.GetInt32("Anio")
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<Vehiculo?> ObtenerPorPlacaAsync(string placa)
        {
            using (var cn = _conexion.CrearConexion())
            {
                const string query = "SELECT Placa, Marca, Anio FROM Vehiculos WHERE Placa = @Placa";
                using (var cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@Placa", placa);
                    await cn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Vehiculo
                            {
                                Placa = reader.GetString("Placa"),
                                Marca = reader.GetString("Marca"),
                                Anio = reader.IsDBNull(reader.GetOrdinal("Anio"))
                                       ? null
                                       : reader.GetInt32("Anio")
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> RegistrarAsync(Vehiculo vehiculo)
        {
            using (var cn = _conexion.CrearConexion())
            {
                const string query = @"INSERT INTO Vehiculos (Placa, Marca, Anio) 
                                       VALUES (@Placa, @Marca, @Anio)";
                using (var cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@Placa", vehiculo.Placa);
                    cmd.Parameters.AddWithValue("@Marca", vehiculo.Marca);
                    cmd.Parameters.AddWithValue("@Anio", (object?)vehiculo.Anio ?? DBNull.Value);

                    await cn.OpenAsync();
                    int filas = await cmd.ExecuteNonQueryAsync();
                    return filas > 0;
                }
            }
        }

        public async Task<bool> ModificarAsync(string placa, Vehiculo cambios)
        {
            var clausulas = new List<string>();
            var parametros = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(cambios.Marca))
            {
                clausulas.Add("Marca = @Marca");
                parametros.Add(new SqlParameter("@Marca", cambios.Marca));
            }

            if (cambios.Anio.HasValue)
            {
                clausulas.Add("Anio = @Anio");
                parametros.Add(new SqlParameter("@Anio", cambios.Anio.Value));
            }

            if (clausulas.Count == 0) return false;

            string query = $"UPDATE Vehiculos SET {string.Join(", ", clausulas)} WHERE Placa = @Placa";

            using (var cn = _conexion.CrearConexion())
            {
                using (var cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@Placa", placa);
                    cmd.Parameters.AddRange(parametros.ToArray());

                    await cn.OpenAsync();
                    int filas = await cmd.ExecuteNonQueryAsync();
                    return filas > 0;
                }
            }
        }

        public async Task<bool> EliminarAsync(string placa)
        {
            using (var cn = _conexion.CrearConexion())
            {
                const string query = "DELETE FROM Vehiculos WHERE Placa = @Placa";
                using (var cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@Placa", placa);

                    await cn.OpenAsync();
                    int filas = await cmd.ExecuteNonQueryAsync();
                    return filas > 0;
                }
            }
        }
    }
}