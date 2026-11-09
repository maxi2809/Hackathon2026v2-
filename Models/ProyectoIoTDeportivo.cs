namespace Hackathon2026
{
    public class ProyectoIoTDeportivo : Proyecto
    {
        public ProyectoIoTDeportivo(
            string titulo,
            string descripcion,
            string repositorio)
            : base(
                titulo,
                descripcion,
                repositorio,
                CategoriaHackathon.IoTDeportivo)
        {
        }

        public override double CalcularPuntajeFinal()
        {
            return (Innovacion * 0.50) +
                   (Viabilidad * 0.30) +
                   (Presentacion * 0.20);
        }
    }
}
