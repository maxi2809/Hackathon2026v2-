namespace Hackathon2026
{
    /// <summary>
    /// Persona adulta/invitada del hackathon en un determinado
    /// <see cref="TipoParticipante"/>: mentor de equipos o jurado evaluador.
    /// </summary>
    /// <remarks>
    /// ENCAPSULAMIENTO: los contadores de carga (EquiposAsignados y
    /// ProyectosEvaluados) tienen setter privado; nadie puede dejarlos en
    /// un valor arbitrario, sólo incrementarlos mediante los métodos
    /// públicos Incrementar...(), que son la única vía de modificación.
    /// </remarks>
    public class MentorJurado : ParticipanteHackathon
    {
        public TipoParticipante Tipo { get; set; }
        public int Experiencia { get; set; }
        public string Institucion { get; set; }

        /// <summary>
        /// Cantidad de proyectos que este jurado evaluó
        /// (sólo lectura desde afuera; se aumenta con
        /// <see cref="IncrementarProyectosEvaluados"/>).
        /// </summary>
        public int ProyectosEvaluados { get; private set; }

        /// <summary>
        /// más equipos/competidores que tiene asignados en este momento
        /// (sólo lectura desde afuera; se aumenta con
        /// <see cref="IncrementarEquiposAsignados"/>). Sirve para balancear
        /// la carga entre mentores.
        /// </summary>
        public int EquiposAsignados { get; private set; }

        /// <summary>
        /// Categoría en la que actúa; también condiciona a qué proyectos
        /// puede evaluar un jurado.
        /// </summary>
        public CategoriaHackathon? CategoriaAsignada { get; set; }

        /// <summary>
        /// Registra que este mentor recibió un equipo más.
        /// A lo sumo se hace desde <see cref="Competidor.AsignarMentor"/>.
        /// </summary>
        public void IncrementarEquiposAsignados()
        {
            EquiposAsignados++;
        }

        /// <summary>
        /// Registra una evaluación más hecha por este jurado.
        /// A lo sumo se hace desde <c>Program.Evaluar</c>.
        /// </summary>
        public void IncrementarProyectosEvaluados()
        {
            ProyectosEvaluados++;
        }

        /// <summary>
        /// Polimorfismo: la métrica de un mentor/jurado es la cantidad de
        /// proyectos que ya evaluó.
        /// </summary>
        public override double CalcularMetricaBase()
        {
            return ProyectosEvaluados;
        }
    }
}
