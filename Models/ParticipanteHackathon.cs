namespace Hackathon2026
{
    /// <summary>
    /// Clase base (ABSTRACCIÓN): modela lo común de cualquier persona que
    /// participa del hackathon (datos de identificación y contacto) sin
    /// definir cómo se comporta un rol concreto.
    /// </summary>
    /// <remarks>
    /// ABSTRACCIÓN: sólo se declara "qué" existe (propiedades comunes y un
    /// contrato "CalcularMetricaBase"), no "cómo" se calcula: eso lo
    /// resuelve cada clase derivada, que depende del rol.
    /// No se puede instanciar directamente porque es abstracta.
    /// </remarks>
    public abstract class ParticipanteHackathon
    {
        public string IdInscripcion { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string ProvinciaPais { get; set; }
        public string Rol { get; set; }

        /// <summary>
        /// Contrato abstracto (polimorfismo): cada subclase define su propia
        /// métrica. En <see cref="MentorJurado"/> son los proyectos
        /// evaluados; en <see cref="Competidor"/>, el puntaje de su
        /// proyecto.
        /// </summary>
        public abstract double CalcularMetricaBase();
    }
}
