using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Hackathon2026
{
    // Persistencia en JSON de todo el estado del sistema (listas + contadores
    // de IDs). Guardando tras cada operación, basta con salir del programa
    // para restaurarlo en la próxima ejecución.
    public static class PersistenciaJson
    {
        public const string RutaPorDefecto = "datos-hackathon.json";

        private static readonly JsonSerializerOptions opciones =
            new JsonSerializerOptions
            {
                WriteIndented = true,
                // Muestra acentos como caracteres reales en el archivo,
                // para que sea legible a la vista.
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

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

            foreach (Proyecto proyecto in proyectos)
            {
                // Persistencia: el "dueño" del proyecto se resuelve al
                // reconstruir buscando al competidor por DNI.
                Competidor dueno =
                    competidores.FirstOrDefault(
                        c => c.ProyectoAsignado == proyecto);

                if (dueno == null)
                    continue;

                ProyectoDto dto = new ProyectoDto
                {
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

        public static DatosHackathonDto Cargar(string ruta)
        {
            string json = File.ReadAllText(ruta);

            return JsonSerializer.Deserialize<DatosHackathonDto>(
                json,
                opciones);
        }
    }
}
