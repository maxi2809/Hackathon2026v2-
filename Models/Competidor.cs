using System;

namespace Hackathon2026
{
    public class Competidor : ParticipanteHackathon
    {
        public string NombreEquipo { get; set; }
        public CategoriaHackathon Categoria { get; set; }
        public string LenguajePrincipal { get; set; }
        public Proyecto ProyectoAsignado { get; private set; }
        public MentorJurado MentorAsignado { get; private set; }

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

        public override double CalcularMetricaBase()
        {
            if (ProyectoAsignado == null)
                return 0;

            return ProyectoAsignado.CalcularPuntajeFinal();
        }
    }
}
