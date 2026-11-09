namespace Hackathon2026
{
    public class MentorJurado : ParticipanteHackathon
    {
        public TipoParticipante Tipo { get; set; }
        public int Experiencia { get; set; }
        public string Institucion { get; set; }
        public int ProyectosEvaluados { get; private set; }
        public int EquiposAsignados { get; private set; }
        public CategoriaHackathon? CategoriaAsignada { get; set; }

        public void IncrementarEquiposAsignados()
        {
            EquiposAsignados++;
        }

        public void IncrementarProyectosEvaluados()
        {
            ProyectosEvaluados++;
        }

        public override double CalcularMetricaBase()
        {
            return ProyectosEvaluados;
        }
    }
}
