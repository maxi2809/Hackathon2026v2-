namespace Hackathon2026
{
    /// <summary>
    /// Subclase de <see cref="Proyecto"/> para la categoría IoT Deportivo.
    /// </summary>
    /// <remarks>
    /// ABSTRACCIÓN/POLIMORFISMO: hereda todo el comportamiento general de
    /// <see cref="Proyecto"/> y sólo aporta el dato diferencial: los pesos
    /// de su categoría en el puntaje final (50% innovación, 30%
    /// viabilidad, 20% presentación).
    /// </remarks>
    public class ProyectoIoTDeportivo : Proyecto
    {
        /// <summary>
        /// Crea el proyecto y fija la categoría IoTDeportivo de una vez,
        /// aprovechando la herencia del constructor protegido de la base.
        /// </summary>
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

        /// <summary>
        /// Puntaje final ponderado de un proyecto de IoT Deportivo.
        /// Implementa: <see cref="Proyecto.CalcularPuntajeFinal"/>.
        /// </summary>
        public override double CalcularPuntajeFinal()
        {
            return (Innovacion * 0.50) +
                   (Viabilidad * 0.30) +
                   (Presentacion * 0.20);
        }
    }
}
