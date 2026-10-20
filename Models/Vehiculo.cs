namespace TallerMecanicoAPI.Models
{
    public class Vehiculo
    {
        public string Placa { get; set; } = "";
        public string Marca { get; set; } = "";
        public int? Anio { get; set; }
    }

    public class VehiculoPatch
    {
        public string? Marca { get; set; }
        public int? Anio { get; set; }
    }
}