using System.Collections.Generic;

namespace Hackathon2026
{
    // DTOs de persistencia (Data Transfer Object): copias planas de los
    // objetos de dominio. Son la versión "serializable" del estado.
    //
    // (ABSTRACCIÓN: la capa de persistencia sólo ve los objetos como
    // datos planos, sin referencias entre ellos; las relaciones se
    // representan por DNI.)
    //
    // (ENCAPSULAMIENTO de las invariantes del dominio: al reconstruir el
    // estado NO se asignan los campos directamente, sino que se reutilizan
    // los métodos del dominio como AsignarProyecto/AsignarMentor/
    // AgregarEvaluacion, que son la única vía de cambio válida.)

    /// <summary>
    /// Contenedor del estado completo del sistema, listo para serializar.
    /// incluye los contadores de IDs para que no se repitan después de
    /// recargar.
    /// </summary>
    public class DatosHackathonDto
    {
        /// <summary>
        /// Último identificador de inscripción emitido para competidores
        /// ("P-nnn") al momento de guardar.
        /// </summary>
        public int UltimoIdCompetidor { get; set; }

        /// <summary>
        /// Último identificador de inscripción emitido para
        /// mentores/jurados ("M-nnn") al momento de guardar.
        /// </summary>
        public int UltimoIdMentorJurado { get; set; }

        /// <summary>Todos los competidores registrados.</summary>
        public List<CompetidorDto> Competidores { get; set; }
            = new List<CompetidorDto>();

        /// <summary>Todos los mentores y jurados registrados.</summary>
        public List<MentorJuradoDto> MentoresJurados { get; set; }
            = new List<MentorJuradoDto>();

        /// <summary>Todos los proyectos inscriptos.</summary>
        public List<ProyectoDto> Proyectos { get; set; }
            = new List<ProyectoDto>();
    }

    /// <summary>
    /// Versión serializable de <see cref="Competidor"/>. La relación con
    /// su mentor se guarda como <see cref="DniMentor"/> (referencia por
    /// DNI, no objeto).
    /// </summary>
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

        /// <summary>
        /// DNI del mentor asignado, o null si no tiene mentor todavía.
        /// </summary>
        public string DniMentor { get; set; }
    }

    /// <summary>
    /// Versión serializable de <see cref="MentorJurado"/>. Los contadores
    /// de carga no se guardan: al reconstruir el estado se vuelven a
    /// derivar con los métodos del dominio (cada AsignarMentor incrementa
    /// EquiposAsignados y cada AgregarEvaluacion suma al jurado).
    /// </summary>
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
        public string Expertise { get; set; }
        public int Experiencia { get; set; }
        public string Institucion { get; set; }

        /// <summary>
        /// Categoría asignada como número (o null si no se conoce);
        /// coincide con <see cref="CategoriaHackathon"/>.
        /// </summary>
        public int? CategoriaAsignada { get; set; }
    }

    /// <summary>
    /// Versión serializable de <see cref="Proyecto"/>.
    /// </summary>
    public class ProyectoDto
    {
        /// <summary>
        /// Nombre de la clase concreta (ProyectoESports,
        /// ProyectoIoTDeportivo, o ProyectoBiomecanica) para poder
        /// reconstruir la subclase polimórfica al cargar.
        /// </summary>
        public string Tipo { get; set; }

        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Repositorio { get; set; }

        /// <summary>DNI del competidor dueño del proyecto.</summary>
        public string DniCompetidor { get; set; }

        /// <summary>Evaluaciones registradas (una por jurado).</summary>
        public List<EvaluacionDto> Evaluaciones { get; set; }
            = new List<EvaluacionDto>();
    }

    /// <summary>
    /// Versión serializable de <see cref="EvaluacionProyecto"/>.
    /// </summary>
    public class EvaluacionDto
    {
        public string DniJurado { get; set; }
        public double Innovacion { get; set; }
        public double Viabilidad { get; set; }
        public double Presentacion { get; set; }
    }
}
