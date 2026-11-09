namespace Hackathon2026
{
    /// <summary>
    /// Subclase de <see cref="Proyecto"/> para la categoría Biomecánica.
    /// </summary>
    /// <remarks>
    /// ABSTRACCIÓN/POLIMORFISMO: hereda todo el comportamiento general de
    /// <see cref="Proyecto"/> y sólo aporta el dato diferencial: los pesos
    /// de su categoría en el puntaje final (30% innovación, 50%
    /// viabilidad, 20% presentación).
    /// </remarks>
    public class ProyectoBiomecanica : Proyecto
    {
        /// <summary>
        /// Crea el proyecto y fija la categoría Biomecanica de una vez,
        /// aprovechando la herencia del constructor protegido de la base.
        /// </summary>
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

        /// <summary>
        /// Puntaje final ponderado de un proyecto de Biomecánica.
        /// Implementa: <see cref="Proyecto.CalcularPuntajeFinal"/>.
        /// </summary>
        public override double CalcularPuntajeFinal()
        {
            return (Innovacion * 0.30) +
                   (Viabilidad * 0.50) +
                   (Presentacion * 0.20);
        }
    }
}
