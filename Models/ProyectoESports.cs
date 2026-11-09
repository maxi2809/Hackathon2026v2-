namespace Hackathon2026
{
    public class ProyectoESports : Proyecto
    {
        public ProyectoESports(
            string titulo,
            string descripcion,
            string repositorio)
            : base(
                titulo,
                descripcion,
                repositorio,
                CategoriaHackathon.ESports)
        {
        }

        public override double CalcularPuntajeFinal()
        {
            return (Innovacion * 0.40) +
                   (Viabilidad * 0.30) +
                   (Presentacion * 0.30);
        }
    }
}
