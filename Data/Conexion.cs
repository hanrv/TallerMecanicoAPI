using Microsoft.Data.SqlClient;

namespace TallerMecanicoAPI.Data
{
    public class Conexion
    {
        private readonly string _cadenaSql;

        public Conexion(IConfiguration configuration)
        {
            _cadenaSql = configuration.GetConnectionString("CadenaSQL")
                         ?? throw new InvalidOperationException("Cadena 'CadenaSQL' no configurada.");
        }

        public SqlConnection CrearConexion()
        {
            return new SqlConnection(_cadenaSql);
        }
    }
}