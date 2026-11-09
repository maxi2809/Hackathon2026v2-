namespace Hackathon2026
{
    /// <summary>
    /// Registro de un dato de evaluación de un proyecto dado por un
    /// jurado: sus tres notas individuales (0 a 10) y quién evaluó.
    /// </summary>
    /// <remarks>
    /// Es un objeto de valor: se entrega a
    /// <see cref="Proyecto.AgregarEvaluacion"/>, que valida y recalcula los
    /// promedios expuestos por el proyecto.
    /// </remarks>
    public class EvaluacionProyecto
    {
        /// <summary>DNI del jurado que emitió las notas.</summary>
        public string DniJurado { get; set; }

        /// <summary>Nota de innovación tecnológica (0 a 10).</summary>
        public double Innovacion { get; set; }

        /// <summary>Nota de viabilidad (0 a 10).</summary>
        public double Viabilidad { get; set; }

        /// <summary>Nota de calidad de presentación (0 a 10).</summary>
        public double Presentacion { get; set; }
    }
}
