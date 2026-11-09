using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hackathon2026
{
    internal class Program
    {
        private static int ultimoIdMentorJurado;
        private static int ultimoIdCompetidor;

        private static readonly List<Competidor> competidores =
            new List<Competidor>();

        private static readonly List<MentorJurado> mentores =
            new List<MentorJurado>();

        private static readonly List<Proyecto> proyectos =
            new List<Proyecto>();

        private const string RutaDatos = PersistenciaJson.RutaPorDefecto;
        private static string avisoCarga;

        private static void Main(string[] args)
        {
            CargarDatosIniciales();

            int opcion;

            do
            {
                if (!Console.IsOutputRedirected)
                    Console.Clear();

                Console.WriteLine("==================================");
                Console.WriteLine("         HACKATHON 2026");
                Console.WriteLine("==================================");
                Console.WriteLine("    1 - Registro Competidor");
                Console.WriteLine("    2 - Registro Mentor/Jurado");
                Console.WriteLine("    3 - Registro Proyecto");
                Console.WriteLine("    4 - Listado Equipos y Proyectos");
                Console.WriteLine("    5 - Asignación Mentores");
                Console.WriteLine("    6 - Evaluación Proyectos");
                Console.WriteLine("    7 - Ranking");
                Console.WriteLine("    8 - Salir");

                if (avisoCarga != null)
                {
                    Console.WriteLine();
                    Console.WriteLine(avisoCarga);
                    avisoCarga = null;
                }

                opcion = ValidadorConsola.LeerEntero(
                    "\nSeleccionar una Opción: ",
                    1,
                    8);

                if (!Console.IsOutputRedirected)
                    Console.Clear();

                try
                {
                    switch (opcion)
                    {
                        case 1:
                            RegistrarCompetidor();
                            break;

                        case 2:
                            RegistrarMentor();
                            break;

                        case 3:
                            RegistrarProyecto();
                            break;

                        case 4:
                            Listar();
                            break;

                        case 5:
                            AsignarMentores();
                            break;

                        case 6:
                            Evaluar();
                            break;

                        case 7:
                            MostrarRanking();
                            break;
                    }
                }
                // FIX: las operaciones de dominio (asignar/evaluar) pueden
                // lanzar excepciones de validación; se muestran como mensaje
                // de consola en lugar de interrumpir el programa.
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error: " + ex.Message);
                }

                if (opcion != 8)
                {
                    // Guarda después de cada operación que modifica datos
                    // (4 y 7 son consultas, no necesitan grabar). Así hay
                    // datos disponibles para la próxima ejecución o prueba.
                    if (opcion != 4 && opcion != 7)
                        PersistirDatos();

                    Console.WriteLine("\nPulse una tecla...");

                    // FIX: permite probar con entrada redirigida sin excepción.
                    if (Console.IsInputRedirected)
                        Console.ReadLine();
                    else
                        Console.ReadKey(true);
                }
                else
                {
                    PersistirDatos();
                    Console.WriteLine();
                    Console.WriteLine(
                        "Datos guardados en: " + RutaDatos);
                }
            }
            while (opcion != 8);
        }

        private static string ObtenerIdCompetidor()
        {
            ultimoIdCompetidor++;
            return "P-" + ultimoIdCompetidor.ToString("D3");
        }

        private static string ObtenerIdMentorJurado()
        {
            ultimoIdMentorJurado++;
            return "M-" + ultimoIdMentorJurado.ToString("D3");
        }

        // FIX: la verificación de duplicados estaba repetida (y con el
        // paréntesis faltante) en dos lugares. Ahora es un único método.
        private static bool ExisteDni(string dni)
        {
            return competidores.Any(c =>
                       c.Dni != null &&
                       c.Dni.Equals(
                           dni,
                           StringComparison.OrdinalIgnoreCase)) ||
                   mentores.Any(m =>
                       m.Dni != null &&
                       m.Dni.Equals(
                           dni,
                           StringComparison.OrdinalIgnoreCase));
        }

        private static bool ExisteEmail(string email)
        {
            return competidores.Any(c =>
                       c.Email != null &&
                       c.Email.Equals(
                           email,
                           StringComparison.OrdinalIgnoreCase)) ||
                   mentores.Any(m =>
                       m.Email != null &&
                       m.Email.Equals(
                           email,
                           StringComparison.OrdinalIgnoreCase));
        }

        private static void RegistrarCompetidor()
        {
            Console.WriteLine("=== REGISTRO DE COMPETIDOR ===");

            string dni = ValidadorConsola.LeerDni("DNI: ");

            if (ExisteDni(dni))
            {
                Console.WriteLine(
                    "Ya existe una persona registrada con ese DNI.");
                return;
            }

            string email = ValidadorConsola.LeerEmail("Email: ");

            if (ExisteEmail(email))
            {
                Console.WriteLine(
                    "Ya existe una persona registrada con ese email.");
                return;
            }

            Competidor competidor = new Competidor
            {
                IdInscripcion = ObtenerIdCompetidor(),
                Dni = dni,
                Nombre = ValidadorConsola.LeerNombre(
                    "Nombre y apellido: "),
                Email = email,
                Telefono = ValidadorConsola.LeerTelefono(
                    "Teléfono: "),
                ProvinciaPais = ValidadorConsola.LeerTextoSinNumeros(
                    "Provincia o país: "),
                Rol = ValidadorConsola.LeerTextoSinNumeros(
                    "Rol dentro del equipo: "),
                NombreEquipo = ValidadorConsola.LeerTextoSinNumeros(
                    "Nombre del equipo: "),
                Categoria = ValidadorConsola.LeerCategoria(),
                LenguajePrincipal = ValidadorConsola.LeerTextoSinNumeros(
                    "Lenguaje principal: ")
            };

            competidores.Add(competidor);

            Console.WriteLine();
            Console.WriteLine(
                "Competidor registrado correctamente.");
            Console.WriteLine(
                "ID de inscripción: " +
                competidor.IdInscripcion);
        }

        private static void RegistrarMentor()
        {
            Console.WriteLine("=== REGISTRO DE MENTOR/JURADO ===");

            string dni = ValidadorConsola.LeerDni("DNI: ");

            if (ExisteDni(dni))
            {
                Console.WriteLine(
                    "Ya existe una persona registrada con ese DNI.");
                return;
            }

            int tipoSeleccionado = ValidadorConsola.LeerEntero(
                "Tipo (1 - Mentor, 2 - Jurado): ",
                1,
                2);

            string email = ValidadorConsola.LeerEmail("Email: ");

            if (ExisteEmail(email))
            {
                Console.WriteLine(
                    "Ya existe una persona registrada con ese email.");
                return;
            }

            // FIX: también los jurados tienen categoría asignada, para poder
            // limitar sobre qué proyectos pueden evaluar.
            MentorJurado mentor = new MentorJurado
            {
                IdInscripcion = ObtenerIdMentorJurado(),
                Dni = dni,
                Nombre = ValidadorConsola.LeerNombre(
                    "Nombre y apellido: "),
                Email = email,
                Telefono = ValidadorConsola.LeerTelefono(
                    "Teléfono: "),
                ProvinciaPais = ValidadorConsola.LeerTexto(
                    "Provincia o país: "),
                Rol = tipoSeleccionado == 1
                    ? "Mentor"
                    : "Jurado",
                Tipo = (TipoParticipante)tipoSeleccionado,
                Experiencia = ValidadorConsola.LeerEntero(
                    "Años de experiencia: ",
                    1,
                    50),
                Institucion = ValidadorConsola.LeerTexto(
                    "Institución: "),
                CategoriaAsignada = ValidadorConsola.LeerCategoria()
            };

            mentores.Add(mentor);

            Console.WriteLine();
            Console.WriteLine(
                "Registro realizado correctamente.");
            Console.WriteLine(
                "Tipo: " + mentor.Rol);
            Console.WriteLine(
                "ID de inscripción: " +
                mentor.IdInscripcion);
        }

        private static void RegistrarProyecto()
        {
            Console.WriteLine("=== REGISTRO DE PROYECTO ===");

            List<Competidor> disponibles =
                competidores
                    .Where(c => c.ProyectoAsignado == null)
                    .ToList();

            if (disponibles.Count == 0)
            {
                Console.WriteLine(
                    "No hay competidores sin proyecto.");
                return;
            }

            string titulo = ValidadorConsola.LeerTexto(
                "Título del proyecto: ");

            string descripcion = ValidadorConsola.LeerTexto(
                "Descripción del proyecto: ");

            string repositorio = ValidadorConsola.LeerTexto(
                "URL del repositorio: ");

            CategoriaHackathon categoria =
                ValidadorConsola.LeerCategoria();

            Proyecto proyecto;

            switch (categoria)
            {
                case CategoriaHackathon.ESports:
                    proyecto = new ProyectoESports(
                        titulo,
                        descripcion,
                        repositorio);
                    break;

                case CategoriaHackathon.IoTDeportivo:
                    proyecto = new ProyectoIoTDeportivo(
                        titulo,
                        descripcion,
                        repositorio);
                    break;

                case CategoriaHackathon.Biomecanica:
                    proyecto = new ProyectoBiomecanica(
                        titulo,
                        descripcion,
                        repositorio);
                    break;

                default:
                    Console.WriteLine("Categoría inválida.");
                    return;
            }

            List<Competidor> disponiblesPorCategoria =
                disponibles
                    .Where(c => c.Categoria == categoria)
                    .ToList();

            if (disponiblesPorCategoria.Count == 0)
            {
                Console.WriteLine(
                    "No hay competidores disponibles para esa categoría.");
                return;
            }

            Console.WriteLine("\nCompetidores disponibles:");

            for (int i = 0; i < disponiblesPorCategoria.Count; i++)
            {
                Console.WriteLine(
                    (i + 1) + " - " +
                    disponiblesPorCategoria[i].Nombre +
                    " | Equipo: " +
                    disponiblesPorCategoria[i].NombreEquipo);
            }

            int seleccion = ValidadorConsola.LeerEntero(
                "Seleccione el competidor: ",
                1,
                disponiblesPorCategoria.Count);

            Competidor competidorSeleccionado =
                disponiblesPorCategoria[seleccion - 1];

            competidorSeleccionado.AsignarProyecto(proyecto);
            proyectos.Add(proyecto);

            Console.WriteLine();
            Console.WriteLine(
                "Proyecto registrado y asignado correctamente.");
        }

        private static void Listar()
        {
            Console.WriteLine(
                "=== LISTADO DE EQUIPOS Y PROYECTOS ===");

            if (competidores.Count == 0)
            {
                Console.WriteLine("No hay competidores registrados.");
            }
            else
            {
                foreach (Competidor competidor in competidores)
                {
                    string proyecto =
                        competidor.ProyectoAsignado == null
                            ? "Sin proyecto"
                            : competidor.ProyectoAsignado.Titulo;

                    string mentor =
                        competidor.MentorAsignado == null
                            ? "Sin mentor"
                            : competidor.MentorAsignado.Nombre;

                    Console.WriteLine(
                        "Competidor: " +
                        competidor.Nombre +
                        " | Equipo: " +
                        competidor.NombreEquipo +
                        " | Proyecto: " +
                        proyecto +
                        " | Mentor: " +
                        mentor);
                }
            }

            Console.WriteLine();

            if (mentores.Count == 0)
            {
                Console.WriteLine("No hay mentores ni jurados registrados.");
            }
            else
            {
                foreach (MentorJurado mentor in mentores)
                {
                    string categoria = mentor.CategoriaAsignada.HasValue
                        ? CategoriaHelper.Nombre(
                            mentor.CategoriaAsignada.Value)
                        : "No aplica";

                    Console.WriteLine(
                        "Nombre: " +
                        mentor.Nombre +
                        " | Tipo: " +
                        mentor.Tipo +
                        " | Categoría: " +
                        categoria +
                        " | Proyectos evaluados: " +
                        mentor.ProyectosEvaluados);
                }
            }

            Console.WriteLine();

            if (proyectos.Count == 0)
            {
                Console.WriteLine("No hay proyectos registrados.");
            }
            else
            {
                foreach (Proyecto proyecto in proyectos)
                {
                    // FIX: no se muestra "0.00" para un proyecto sin
                    // evaluaciones; se muestra "Pendiente".
                    string promedio =
                        proyecto.Promedio.HasValue
                            ? proyecto.Promedio.Value.ToString("0.00")
                            : "Pendiente";

                    string puntaje =
                        proyecto.EstaEvaluado()
                            ? proyecto.CalcularPuntajeFinal()
                                .ToString("0.00")
                            : "Pendiente";

                    Console.WriteLine(
                        "Proyecto: " +
                        proyecto.Titulo +
                        " | Categoría: " +
                        CategoriaHelper.Nombre(
                            proyecto.Categoria) +
                        " | Evaluaciones: " +
                        proyecto.Evaluaciones.Count +
                        " | Promedio: " +
                        promedio +
                        " | Puntaje: " +
                        puntaje);
                }
            }
        }

        private static void AsignarMentores()
        {
            Console.WriteLine("=== ASIGNACIÓN DE MENTORES ===");

            if (competidores.Count == 0)
            {
                Console.WriteLine("No hay competidores registrados.");
                return;
            }

            foreach (Competidor competidor in competidores)
            {
                if (competidor.ProyectoAsignado == null)
                {
                    Console.WriteLine(
                        "El competidor " +
                        competidor.Nombre +
                        " no tiene un proyecto asignado.");

                    continue;
                }

                if (competidor.MentorAsignado != null)
                {
                    Console.WriteLine(
                        "El competidor " +
                        competidor.Nombre +
                        " ya tiene mentor asignado.");

                    continue;
                }

                // FIX: se elige al mentor MENOS cargado y la asignación
                // incrementa su contador, de modo que la carga se reparte
                // en vez de caer siempre sobre el mismo.
                MentorJurado mentorDisponible =
                    mentores
                        .Where(m =>
                            m.Tipo == TipoParticipante.Mentor &&
                            m.CategoriaAsignada.HasValue &&
                            m.CategoriaAsignada.Value ==
                                competidor.Categoria)
                        .OrderBy(m => m.EquiposAsignados)
                        .ThenBy(m => m.Nombre)
                        .FirstOrDefault();

                if (mentorDisponible == null)
                {
                    Console.WriteLine(
                        "No hay mentores disponibles para " +
                        competidor.Nombre);

                    continue;
                }

                competidor.AsignarMentor(mentorDisponible);

                Console.WriteLine(
                    "Se asignó el mentor " +
                    mentorDisponible.Nombre +
                    " al competidor " +
                    competidor.Nombre);
            }
        }

        private static void Evaluar()
        {
            Console.WriteLine("=== EVALUACIÓN DE PROYECTOS ===");

            string dniJurado =
                ValidadorConsola.LeerDni("DNI del jurado: ");

            MentorJurado jurado =
                mentores.FirstOrDefault(
                    m =>
                        m.Tipo == TipoParticipante.Jurado &&
                        m.Dni != null &&
                        m.Dni.Equals(
                            dniJurado,
                            StringComparison.OrdinalIgnoreCase));

            if (jurado == null)
            {
                Console.WriteLine(
                    "El DNI no corresponde a un jurado registrado.");

                return;
            }

            // FIX: el jurado solo puede evaluar proyectos de SU categoría.
            if (!jurado.CategoriaAsignada.HasValue)
            {
                Console.WriteLine(
                    "El jurado no tiene categoría asignada.");

                return;
            }

            List<Competidor> elegibles =
                competidores
                    .Where(c =>
                        c.ProyectoAsignado != null &&
                        c.ProyectoAsignado.Categoria ==
                            jurado.CategoriaAsignada.Value)
                    .ToList();

            if (elegibles.Count == 0)
            {
                Console.WriteLine(
                    "No hay proyectos para evaluar en la categoría " +
                    CategoriaHelper.Nombre(
                        jurado.CategoriaAsignada.Value) +
                    ".");

                return;
            }

            Console.WriteLine(
                "\nProyectos de la categoría " +
                CategoriaHelper.Nombre(
                    jurado.CategoriaAsignada.Value) +
                ":");

            for (int i = 0; i < elegibles.Count; i++)
            {
                Competidor competidor = elegibles[i];
                Proyecto proyecto = competidor.ProyectoAsignado;

                string estado;

                if (!proyecto.EstaEvaluado())
                {
                    estado = "Pendiente de evaluación";
                }
                else
                {
                    estado =
                        "Evaluado por " +
                        proyecto.Evaluaciones.Count +
                        " jurado(s). Promedio: " +
                        proyecto.Promedio.Value.ToString("0.00");

                    if (proyecto.YaEvaluadoPor(dniJurado))
                        estado += " (incluye su evaluación)";
                }

                Console.WriteLine(
                    (i + 1) + " - " +
                    proyecto.Titulo +
                    " | Competidor: " +
                    competidor.Nombre +
                    " | " +
                    estado);
            }

            // FIX: cada jurado puede evaluar todos los proyectos aún si otro
            // jurado ya lo hizo; la nota final es el promedio de todos.
            List<Competidor> pendientes =
                elegibles
                    .Where(c =>
                        !c.ProyectoAsignado.YaEvaluadoPor(dniJurado))
                    .ToList();

            if (pendientes.Count == 0)
            {
                Console.WriteLine(
                    "\nYa evaluó todos los proyectos de su categoría.");
                return;
            }

            int seleccion = ValidadorConsola.LeerEntero(
                "\nSeleccione el proyecto a evaluar: ",
                1,
                pendientes.Count);

            Proyecto proyectoSeleccionado =
                pendientes[seleccion - 1].ProyectoAsignado;

            Console.WriteLine(
                "\nEvaluando proyecto: " +
                proyectoSeleccionado.Titulo);

            double innovacion = ValidadorConsola.LeerDouble(
                "Innovación (0-10): ",
                0,
                10);

            double viabilidad = ValidadorConsola.LeerDouble(
                "Viabilidad (0-10): ",
                0,
                10);

            double presentacion = ValidadorConsola.LeerDouble(
                "Presentación (0-10): ",
                0,
                10);

            proyectoSeleccionado.AgregarEvaluacion(
                new EvaluacionProyecto
                {
                    DniJurado = jurado.Dni,
                    Innovacion = innovacion,
                    Viabilidad = viabilidad,
                    Presentacion = presentacion
                });

            jurado.IncrementarProyectosEvaluados();

            Console.WriteLine();
            Console.WriteLine(
                "Proyecto evaluado correctamente.");
            Console.WriteLine(
                "Promedio registrado: " +
                proyectoSeleccionado.Promedio.Value.ToString("0.00"));
        }

        private static void MostrarRanking()
        {
            if (proyectos.Count == 0)
            {
                Console.WriteLine("No hay proyectos registrados.");
                return;
            }

            // FIX: solo rankea proyectos efectivamente evaluados; antes los
            // pendientes entraban con puntaje 0. Empates comparten posición.
            List<Proyecto> ranking =
                proyectos
                    .Where(p => p.EstaEvaluado())
                    .OrderByDescending(
                        p => p.CalcularPuntajeFinal())
                    .ToList();

            if (ranking.Count == 0)
            {
                Console.WriteLine(
                    "Ningún proyecto fue evaluado todavía.");
                return;
            }

            Console.WriteLine("=== RANKING DE PROYECTOS ===");

            int posicion = 1;
            double puntajeAnterior = double.NaN;

            for (int i = 0; i < ranking.Count; i++)
            {
                Proyecto proyecto = ranking[i];
                double puntaje = proyecto.CalcularPuntajeFinal();

                if (i > 0 &&
                    Math.Round(puntaje, 2) !=
                    Math.Round(puntajeAnterior, 2))
                {
                    posicion = i + 1;
                }

                Console.WriteLine(
                    posicion + ". " +
                    proyecto.Titulo +
                    " - Puntaje: " +
                    puntaje.ToString("0.00") +
                    " (" +
                    proyecto.Evaluaciones.Count +
                    " evaluacion(es))");

                puntajeAnterior = puntaje;
            }
        }

        // ===== Persistencia =====

        private static void CargarDatosIniciales()
        {
            try
            {
                if (!File.Exists(RutaDatos))
                    return; // primera ejecución: no hay archivo

                DatosHackathonDto datos = PersistenciaJson.Cargar(
                    RutaDatos);

                ReconstruirEstado(datos);

                avisoCarga =
                    "Datos cargados: " +
                    competidores.Count +
                    " competidores, " +
                    mentores.Count +
                    " mentores/jurados, " +
                    proyectos.Count +
                    " proyectos.";
            }
            catch (Exception ex)
            {
                avisoCarga =
                    "Aviso: no se pudo cargar '" +
                    RutaDatos +
                    "' (" +
                    ex.Message +
                    "). Se inicia con datos vacíos.";
            }
        }

        private static void PersistirDatos()
        {
            try
            {
                PersistenciaJson.Guardar(
                    RutaDatos,
                    competidores,
                    mentores,
                    proyectos,
                    ultimoIdCompetidor,
                    ultimoIdMentorJurado);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Aviso: no se pudieron guardar los datos (" +
                    ex.Message +
                    ").");
            }
        }

        // Reconstruye el estado del dominio a partir del DTO. Se reusan los
        // métodos reales del dominio (AsignarProyecto, AsignarMentor,
        // AgregarEvaluación), de modo que los contadores y las validaciones
        // se reproducen de forma idéntica al flujo normal del programa.
        private static void ReconstruirEstado(DatosHackathonDto datos)
        {
            ultimoIdCompetidor = datos.UltimoIdCompetidor;
            ultimoIdMentorJurado = datos.UltimoIdMentorJurado;

            foreach (MentorJuradoDto dto in datos.MentoresJurados)
            {
                mentores.Add(new MentorJurado
                {
                    IdInscripcion = dto.IdInscripcion,
                    Dni = dto.Dni,
                    Nombre = dto.Nombre,
                    Email = dto.Email,
                    Telefono = dto.Telefono,
                    ProvinciaPais = dto.ProvinciaPais,
                    Rol = dto.Rol,
                    Tipo = (TipoParticipante)dto.Tipo,
                    Experiencia = dto.Experiencia,
                    Institucion = dto.Institucion,
                    CategoriaAsignada = dto.CategoriaAsignada.HasValue
                        ? (CategoriaHackathon)dto.CategoriaAsignada.Value
                        : (CategoriaHackathon?)null
                });
            }

            foreach (CompetidorDto dto in datos.Competidores)
            {
                competidores.Add(new Competidor
                {
                    IdInscripcion = dto.IdInscripcion,
                    Dni = dto.Dni,
                    Nombre = dto.Nombre,
                    Email = dto.Email,
                    Telefono = dto.Telefono,
                    ProvinciaPais = dto.ProvinciaPais,
                    Rol = dto.Rol,
                    NombreEquipo = dto.NombreEquipo,
                    Categoria = (CategoriaHackathon)dto.Categoria,
                    LenguajePrincipal = dto.LenguajePrincipal
                });
            }

            foreach (ProyectoDto dto in datos.Proyectos)
            {
                Competidor dueno =
                    competidores.FirstOrDefault(
                        c =>
                            c.Dni != null &&
                            c.Dni.Equals(
                                dto.DniCompetidor,
                                StringComparison.OrdinalIgnoreCase));

                if (dueno == null)
                {
                    Console.WriteLine(
                        "Aviso: proyecto '" +
                        dto.Titulo +
                        "' omitido: su competidor no fue encontrado.");

                    continue;
                }

                Proyecto proyecto;

                switch (dto.Tipo)
                {
                    case nameof(ProyectoESports):
                        proyecto = new ProyectoESports(
                            dto.Titulo,
                            dto.Descripcion,
                            dto.Repositorio);
                        break;

                    case nameof(ProyectoIoTDeportivo):
                        proyecto = new ProyectoIoTDeportivo(
                            dto.Titulo,
                            dto.Descripcion,
                            dto.Repositorio);
                        break;

                    case nameof(ProyectoBiomecanica):
                        proyecto = new ProyectoBiomecanica(
                            dto.Titulo,
                            dto.Descripcion,
                            dto.Repositorio);
                        break;

                    default:
                        Console.WriteLine(
                            "Aviso: proyecto '" +
                            dto.Titulo +
                            "' omitido: tipo desconocido '" +
                            dto.Tipo +
                            "'.");
                        continue;
                }

                dueno.AsignarProyecto(proyecto);
                proyectos.Add(proyecto);

                foreach (EvaluacionDto evaluacion in dto.Evaluaciones)
                {
                    proyecto.AgregarEvaluacion(
                        new EvaluacionProyecto
                        {
                            DniJurado = evaluacion.DniJurado,
                            Innovacion = evaluacion.Innovacion,
                            Viabilidad = evaluacion.Viabilidad,
                            Presentacion = evaluacion.Presentacion
                        });

                    MentorJurado jurado =
                        mentores.FirstOrDefault(
                            m =>
                                m.Tipo == TipoParticipante.Jurado &&
                                m.Dni != null &&
                                m.Dni.Equals(
                                    evaluacion.DniJurado,
                                    StringComparison.OrdinalIgnoreCase));

                    if (jurado != null)
                        jurado.IncrementarProyectosEvaluados();
                }
            }

            foreach (CompetidorDto dto in datos.Competidores)
            {
                if (string.IsNullOrEmpty(dto.DniMentor))
                    continue;

                Competidor competidor =
                    competidores.FirstOrDefault(
                        c =>
                            c.Dni != null &&
                            c.Dni.Equals(
                                dto.Dni,
                                StringComparison.OrdinalIgnoreCase));

                MentorJurado mentor =
                    mentores.FirstOrDefault(
                        m =>
                            m.Dni != null &&
                            m.Dni.Equals(
                                dto.DniMentor,
                                StringComparison.OrdinalIgnoreCase));

                if (competidor != null && mentor != null)
                    competidor.AsignarMentor(mentor);
            }
        }
    }
}
