using System;

namespace Hackathon2026
{
    /// <summary>
    /// Un participante competidor que forma parte de un equipo y solicita
    /// (o recibe) la inscripción de un proyecto en su categoría.
    /// </summary>
    /// <remarks>
    /// ENCAPSULAMIENTO: <see cref="ProyectoAsignado"/> y
    /// <see cref="MentorAsignado"/> son de sólo lectura desde afuera. La
    /// única forma de cambiarlos son los métodos <see cref="AsignarProyecto"/>
    /// y <see cref="AsignarMentor"/>, que validan antes de asignar (no se
    /// permite doble asignación ni proyectos/mentores de otra categoría).
    /// Así el objeto nunca queda en un estado inválido.
    /// </remarks>
    public class Competidor : ParticipanteHackathon
    {
        public string NombreEquipo { get; set; }
        public CategoriaHackathon Categoria { get; set; }
        public string LenguajePrincipal { get; set; }

        /// <summary>
        /// proyecto inscripto a nombre de este competidor; null si todavía
        /// no tiene uno. Sólo modificable dentro de la clase.
        /// </summary>
        public Proyecto ProyectoAsignado { get; private set; }

        /// <summary>
        /// mentor que acompaña al equipo; null hasta recibir la asignación.
        /// Sólo modificable dentro de la clase.
        /// </summary>
        public MentorJurado MentorAsignado { get; private set; }

        /// <summary>
        /// Asigna un proyecto a este competidor.
        /// </summary>
        /// <exception cref="ArgumentNullException">proyecto es null.</exception>
        /// <exception cref="InvalidOperationException">
        /// El competidor ya tiene un proyecto, o el proyecto pertenece a
        /// otra categoría.
        /// </exception>
        public void AsignarProyecto(Proyecto proyecto)
        {
            if (proyecto == null)
                throw new ArgumentNullException(nameof(proyecto));

            if (ProyectoAsignado != null)
                throw new InvalidOperationException(
                    "El competidor ya tiene un proyecto asignado.");

            if (proyecto.Categoria != Categoria)
                throw new InvalidOperationException(
                    "El proyecto no pertenece a la categoría del competidor.");

            ProyectoAsignado = proyecto;
        }

        /// <summary>
        /// Asigna el mentor que acompañará a este competidor.
        /// </summary>
        /// <exception cref="ArgumentNullException">mentor es null.</exception>
        /// <exception cref="InvalidOperationException">
        /// El competidor ya tiene mentor, o el mentor no pertenece a la
        /// categoría del competidor.
        /// </exception>
        /// <remarks>
        /// Además incrementa la carga del mentor
        /// (<see cref="MentorJurado.IncrementarEquiposAsignados"/>): el
        /// encapsulamiento hace que el estado del mentor sólo cambie por
        /// acciones válidas.
        /// </remarks>
        public void AsignarMentor(MentorJurado mentor)
        {
            if (mentor == null)
                throw new ArgumentNullException(nameof(mentor));

            if (MentorAsignado != null)
                throw new InvalidOperationException(
                    "El competidor ya tiene un mentor asignado.");

            if (!mentor.CategoriaAsignada.HasValue ||
                mentor.CategoriaAsignada.Value != Categoria)
            {
                throw new InvalidOperationException(
                    "El mentor no pertenece a la categoría del competidor.");
            }

            MentorAsignado = mentor;
            mentor.IncrementarEquiposAsignados();
        }

        /// <summary>
        /// Polimorfismo: la métrica de un competidor es el puntaje final de
        /// su proyecto (0 si todavía no tiene proyecto).
        /// </summary>
        public override double CalcularMetricaBase()
        {
            if (ProyectoAsignado == null)
                return 0;

            return ProyectoAsignado.CalcularPuntajeFinal();
        }
    }
}
