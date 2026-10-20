namespace TallerMecanicoApi.Models
{
    public class Vehiculo
    {
        public string Placa { get; set; } = "";
        public string Marca { get; set; } = "";
        public int Anio { get; set; }
    }

    // Clase para el PATCH: solo se envian los datos que se quieren cambiar
    public class VehiculoPatch
    {
        public string? Marca { get; set; }
        public int? Anio { get; set; }
    }
}