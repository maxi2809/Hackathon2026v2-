using System.Collections.Generic;

namespace Hackathon2026
{
    // DTOs de persistencia: copias planas de los objetos de dominio, sin
    // referencias cruzadas (se navegan por DNI). Reconstrucción por
    // equivalencia con los objetos reales (así los invariantes del dominio
    // se van a revalidar al reconstruir).
    public class DatosHackathonDto
    {
        public int UltimoIdCompetidor { get; set; }
        public int UltimoIdMentorJurado { get; set; }

        public List<CompetidorDto> Competidores { get; set; }
            = new List<CompetidorDto>();

        public List<MentorJuradoDto> MentoresJurados { get; set; }
            = new List<MentorJuradoDto>();

        public List<ProyectoDto> Proyectos { get; set; }
            = new List<ProyectoDto>();
    }

    public class CompetidorDto
    {
        public string IdInscripcion { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string ProvinciaPais { get; set; }
        public string Rol { get; set; }
        public string NombreEquipo { get; set; }
        public int Categoria { get; set; }
        public string LenguajePrincipal { get; set; }
        public string DniMentor { get; set; }
    }

    public class MentorJuradoDto
    {
        public string IdInscripcion { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string ProvinciaPais { get; set; }
        public string Rol { get; set; }
        public int Tipo { get; set; }
        public int Experiencia { get; set; }
        public string Institucion { get; set; }
        public int? CategoriaAsignada { get; set; }
    }

    public class ProyectoDto
    {
        // Nombre de la clase concreta: ProyectoESports / ProyectoIoTDeportivo / ProyectoBiomecanica.
        public string Tipo { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Repositorio { get; set; }
        public string DniCompetidor { get; set; }
        public List<EvaluacionDto> Evaluaciones { get; set; }
            = new List<EvaluacionDto>();
    }

    public class EvaluacionDto
    {
        public string DniJurado { get; set; }
        public double Innovacion { get; set; }
        public double Viabilidad { get; set; }
        public double Presentacion { get; set; }
    }
}
