namespace Hackathon2026
{
    /// <summary>
    /// Clase base (ABSTRACCIÓN): modela lo común de cualquier persona que
    /// participa del hackathon. La identificación (DNI, email, nombre) es
    /// común a los tres roles, por lo que vive en clase aparte
    /// (<see cref="PersonaDatos"/>) y se compone acá; cada subclase
    /// termina de completar los campos propios de su rol.
    /// </summary>
    /// <remarks>
    /// ENCAPSULAMIENTO: el objeto común <see cref="Persona"/> se asigna al
    /// momento del alta y el resto del programa lo accede por propiedades
    /// de sólo lectura (Dni, Email, Nombre): no se puede cambiar la
    /// identidad de un participante por detrás.
    /// No se puede instanciar directamente porque es abstracta.
    /// </remarks>
    public abstract class ParticipanteHackathon
    {
        /// <summary>
        /// Datos comunes de la persona (DNI, email, nombre): se construye
        /// una vez en el alta (<c>Program.SolicitarPersonaUnica</c>) y se
        /// comparte con todos los roles.
        /// </summary>
        public PersonaDatos Persona { get; set; }

        public string IdInscripcion { get; set; }
        public string Telefono { get; set; }
        public string ProvinciaPais { get; set; }
        public string Rol { get; set; }

        /// <summary>Acceso de sólo lectura al DNI de la persona común.</summary>
        public string Dni
        {
            get
            {
                return Persona != null ? Persona.Dni : null;
            }
        }

        /// <summary>Acceso de sólo lectura al email de la persona común.</summary>
        public string Email
        {
            get
            {
                return Persona != null ? Persona.Email : null;
            }
        }

        /// <summary>Acceso de sólo lectura al nombre de la persona común.</summary>
        public string Nombre
        {
            get
            {
                return Persona != null ? Persona.Nombre : null;
            }
        }

        /// <summary>
        /// Contrato abstracto (polimorfismo): cada subclase define su propia
        /// métrica. En <see cref="MentorJurado"/> son los proyectos
        /// evaluados; en <see cref="Competidor"/>, el puntaje de su
        /// proyecto.
        /// </summary>
        public abstract double CalcularMetricaBase();
    }
}
