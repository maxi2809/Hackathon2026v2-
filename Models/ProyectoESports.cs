namespace Hackathon2026
{
    /// <summary>
    /// Subclase de <see cref="Proyecto"/> para la categoría E-Sports.
    /// </summary>
    /// <remarks>
    /// ABSTRACCIÓN/POLIMORFISMO: hereda todo el comportamiento general de
    /// <see cref="Proyecto"/> y sólo aporta el dato diferencial: los pesos
    /// de su categoría en el puntaje final (40% innovación, 30%
    /// viabilidad, 30% presentación).
    /// </remarks>
    public class ProyectoESports : Proyecto
    {
        /// <summary>
        /// Crea el proyecto y fija la categoría ESports de una vez,
        /// aprovechando la herencia del constructor protegido de la base.
        /// </summary>
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

        /// <summary>
        /// Puntaje final ponderado de un proyecto de E-Sports.
        /// Implementa: <see cref="Proyecto.CalcularPuntajeFinal"/>.
        /// </summary>
        public override double CalcularPuntajeFinal()
        {
            return (Innovacion * 0.40) +
                   (Viabilidad * 0.30) +
                   (Presentacion * 0.30);
        }
    }
}
