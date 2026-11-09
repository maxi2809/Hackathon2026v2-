namespace Hackathon2026
{
    public class ProyectoBiomecanica : Proyecto
    {
        public ProyectoBiomecanica(
            string titulo,
            string descripcion,
            string repositorio)
            : base(
                titulo,
                descripcion,
                repositorio,
                CategoriaHackathon.Biomecanica)
        {
        }

        public override double CalcularPuntajeFinal()
        {
            return (Innovacion * 0.30) +
                   (Viabilidad * 0.50) +
                   (Presentacion * 0.20);
        }
    }
}
