namespace Hackathon2026
{
    public abstract class ParticipanteHackathon
    {
        public string IdInscripcion { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string ProvinciaPais { get; set; }
        public string Rol { get; set; }

        public abstract double CalcularMetricaBase();
    }
}
