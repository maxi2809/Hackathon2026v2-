using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Hackathon2026
{
    // Persistencia en JSON (SYSTEM.Text.Json) de todo el estado del
    // sistema (personas, proyectos, evaluaciones y contadores de IDs).

    /// <summary>
    /// Capa de persistencia: guarda todo el estado del sistema en un
    /// archivo JSON y lo recupera al iniciar, de modo que el programa
    /// arranca donde quedó o con un conjunto de datos preparado para
    /// pruebas.
    /// </summary>
    /// <remarks>
    /// Usa DTOs (<see cref="DatosHackathonDto"/>) como versión serializable
    /// del estado: los objetos reales del dominio quedan intactos y sus
    /// invariantes no dependen de la serialización.
    /// </remarks>
    public static class PersistenciaJson
    {
        /// <summary>
        /// Archivo por defecto: se crea en la carpeta desde la que se
        /// ejecuta el programa.
        /// </summary>
        public const string RutaPorDefecto = "datos-hackathon.json";

        /// <summary>
        /// Opciones de serialización: indentado (para leerlo a simple
        /// vista) y sin escapar los caracteres no ASCII (los acentos y la
        /// "ñ" aparecen como tales en el archivo).
        /// </summary>
        private static readonly JsonSerializerOptions opciones =
            new JsonSerializerOptions
            {
                WriteIndented = true,
                // Muestra acentos como caracteres reales en el archivo,
                // para que sea legible a la vista.
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

        /// <summary>
        /// Guarda el estado completo en un archivo JSON: convierte los
        /// objetos de dominio a su versión DTO (con las referencias
        /// resueltas por DNI) y serializa ese DTO.
        /// </summary>
        /// <param name="ruta">Archivo destino.</param>
        /// <param name="competidores">Listado completo de competidores.</param>
        /// <param name="mentores">Listado completo de mentores y jurados.</param>
        /// <param name="proyectos">Listado completo de proyectos.</param>
        /// <param name="ultimoIdCompetidor">
        /// Último ID de competidor usado (para poder seguir numerando).
        /// </param>
        /// <param name="ultimoIdMentorJurado">Último ID de mentor/jurado usado.</param>
        /// <returns>La ruta del archivo generado.</returns>
        public static string Guardar(
            string ruta,
            List<Competidor> competidores,
            List<MentorJurado> mentores,
            List<Proyecto> proyectos,
            int ultimoIdCompetidor,
            int ultimoIdMentorJurado)
        {
            DatosHackathonDto datos = new DatosHackathonDto();
            datos.UltimoIdCompetidor = ultimoIdCompetidor;
            datos.UltimoIdMentorJurado = ultimoIdMentorJurado;

            // DTO de cada competidor; el mentor asignado se referencia por DNI.
            foreach (Competidor competidor in competidores)
            {
                datos.Competidores.Add(new CompetidorDto
                {
                    IdInscripcion = competidor.IdInscripcion,
                    Dni = competidor.Dni,
                    Nombre = competidor.Nombre,
                    Email = competidor.Email,
                    Telefono = competidor.Telefono,
                    ProvinciaPais = competidor.ProvinciaPais,
                    Rol = competidor.Rol,
                    NombreEquipo = competidor.NombreEquipo,
                    Categoria = (int)competidor.Categoria,
                    LenguajePrincipal = competidor.LenguajePrincipal,
                    DniMentor = competidor.MentorAsignado != null
                        ? competidor.MentorAsignado.Dni
                        : null
                });
            }

            // DTO de cada mentor/jurado.
            foreach (MentorJurado mentor in mentores)
            {
                datos.MentoresJurados.Add(new MentorJuradoDto
                {
                    IdInscripcion = mentor.IdInscripcion,
                    Dni = mentor.Dni,
                    Nombre = mentor.Nombre,
                    Email = mentor.Email,
                    Telefono = mentor.Telefono,
                    ProvinciaPais = mentor.ProvinciaPais,
                    Rol = mentor.Rol,
                    Tipo = (int)mentor.Tipo,
                    Experiencia = mentor.Experiencia,
                    Institucion = mentor.Institucion,
                    CategoriaAsignada = mentor.CategoriaAsignada.HasValue
                        ? (int)mentor.CategoriaAsignada.Value
                        : (int?)null
                });
            }

            // DTO de cada proyecto. Se busca al competidor dueño del
            // proyecto y se guarda su DNI como referencia (cada proyecto
            // pertenece a un competidor).
            foreach (Proyecto proyecto in proyectos)
            {
                Competidor dueno =
                    competidores.FirstOrDefault(
                        c => c.ProyectoAsignado == proyecto);

                if (dueno == null)
                    continue;

                ProyectoDto dto = new ProyectoDto
                {
                    // Se guarda el nombre de la subclase polimórfica, para
                    // reconstruir el tipo correcto al cargar.
                    Tipo = proyecto.GetType().Name,
                    Titulo = proyecto.Titulo,
                    Descripcion = proyecto.Descripcion,
                    Repositorio = proyecto.Repositorio,
                    DniCompetidor = dueno.Dni
                };

                foreach (EvaluacionProyecto evaluacion in
                    proyecto.Evaluaciones)
                {
                    dto.Evaluaciones.Add(new EvaluacionDto
                    {
                        DniJurado = evaluacion.DniJurado,
                        Innovacion = evaluacion.Innovacion,
                        Viabilidad = evaluacion.Viabilidad,
                        Presentacion = evaluacion.Presentacion
                    });
                }

                datos.Proyectos.Add(dto);
            }

            File.WriteAllText(
                ruta,
                JsonSerializer.Serialize(datos, opciones));

            return ruta;
        }

        /// <summary>
        /// Carga el estado completo desde un archivo JSON y lo devuelve
        /// como DTO. La reconstrucción de los objetos de dominio se hace
        /// en <c>Program.ReconstruirEstado</c>, reutilizando los métodos
        /// del dominio.
        /// </summary>
        public static DatosHackathonDto Cargar(string ruta)
        {
            string json = File.ReadAllText(ruta);

            return JsonSerializer.Deserialize<DatosHackathonDto>(
                json,
                opciones);
        }
    }
}
