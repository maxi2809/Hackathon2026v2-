using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hackathon2026
{
  
    /*Contiene el menú principal y todos los
    flujos de consola (registrador, listado, asignaciones, evaluación
    y ranking), además de la orquestación de carga/persistencia.*/
   
  
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

        
        /*Punto de entrada: muestra el menú principal en un bucle y 
         despacha cada opción a su flujo.*/
       
        private static void Main(string[] args)
        {
            CargarDatosIniciales();

            int opcion = 0;

            do
            {
                if (!Console.IsOutputRedirected)
                    Console.Clear();

                Console.WriteLine("==================================");
                Console.WriteLine("         HACKATHON 2026");
                Console.WriteLine("==================================");
                Console.WriteLine("    1 - Registro");
                Console.WriteLine("    2 - Listar Proyectos");
                Console.WriteLine("    3 - Asignar Mentor");
                Console.WriteLine("    4 - Evaluar Proyecto");
                Console.WriteLine("    5 - Ranking");

                // 6 salida para cerrar
                Console.WriteLine("    6 - Salir");

                if (avisoCarga != null)
                {
                    Console.WriteLine();
                    Console.WriteLine(avisoCarga);
                    avisoCarga = null;
                }

                try
                {
                    opcion = ValidadorConsola.LeerEntero(
                        "\nSeleccionar una Opción: ",
                        1,
                        6);
                }
                /*Máximo 3 intentos; si se agotan, se muestra el aviso y 
                 se repite el bucle (el usuario ya está en el menú principal).*/
                catch (IntentosAgotadosException)
                {
                    Console.WriteLine();
                    Console.WriteLine(IntentosAgotadosException.Mensaje);
                    continue;
                }

                if (!Console.IsOutputRedirected)
                    Console.Clear();

                try
                {
                    switch (opcion)
                    {
                        case 1:
                            Registrar();
                            break;

                        case 2:
                            Listar();
                            break;

                        case 3:
                            AsignarMentores();
                            break;

                        case 4:
                            Evaluar();
                            break;

                        case 5:
                            MostrarRanking();
                            break;
                    }
                }
                /* Si se usaron los 3 intentos de un ingreso, se avisa y la
                 vuelve al menú principal (el flujo se aborta) */
                catch (IntentosAgotadosException)
                {
                    Console.WriteLine();
                    Console.WriteLine(IntentosAgotadosException.Mensaje);
                }
                // Las operaciones de dominio (asignar/evaluar) también pueden
                // lanzar excepciones de validación; se muestran como mensaje
                // de consola sin interrumpir el programa.
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error: " + ex.Message);
                }

                if (opcion != 6)
                {
                    // Guarda después de cada operación que modifica datos
                    // (2 y 5 son consultas, no necesitan grabar). Así hay
                    // datos disponibles para la próxima ejecución o prueba.
                    if (opcion != 2 && opcion != 5)
                        PersistirDatos();

                    Console.WriteLine("\nPulse una tecla...");

                    // Permite probar con entrada redirigida sin excepción.
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
            while (opcion != 6);
        }

        
        /// Genera el siguiente ID de inscripción para competidores
        /// ("P-001", "P-002", ...) 
       
        private static string ObtenerIdCompetidor()
        {
            ultimoIdCompetidor++;
            return "P-" + ultimoIdCompetidor.ToString("D3");
        }

        
        /// Genera el siguiente ID de inscripción de mentor o jurado
        /// ("M-001", ... para mentores; "J-001", ... para jurados)
       
      
        private static string ObtenerIdMentorJurado(TipoParticipante tipo)
        {
            ultimoIdMentorJurado++;

            string prefijo = tipo == TipoParticipante.Jurado
                ? "J-"
                : "M-";

            return prefijo + ultimoIdMentorJurado.ToString("D3");
        }

        //Verificación de duplicados único método.
        /// Indica si ya existe una persona (competidor o mentor/jurado)
        /// registrada con el DNI dado. Se usa para rechazar registros
        /// duplicados al dar de alta a una persona.
      
        private static bool ExisteDni(string dni)
        {
            return competidores.Any(c =>
                       c.Persona != null &&
                       c.Persona.MismoDni(dni)) ||
                   mentores.Any(m =>
                       m.Persona != null &&
                       m.Persona.MismoDni(dni));
        }

        /// Indica si ya existe una persona registrada con el email dado
       
        private static bool ExisteEmail(string email)
        {
            return competidores.Any(c =>
                       c.Persona != null &&
                       c.Persona.MismoEmail(email)) ||
                   mentores.Any(m =>
                       m.Persona != null &&
                       m.Persona.MismoEmail(email));
        }

       
    
        private static PersonaDatos SolicitarPersonaUnica()
        {
            for (int intento = 1; intento <= ValidadorConsola.MaximoIntentos; intento++)
            {
                string dni = ValidadorConsola.LeerDni("DNI: ");

                if (ExisteDni(dni))
                {
                    Console.WriteLine(
                        "Ya existe una persona registrada con ese DNI." +
                        " (intento " +
                        intento +
                        " de " +
                        ValidadorConsola.MaximoIntentos +
                        ")");
                    continue;
                }

                string nombre = ValidadorConsola.LeerNombre(
                    "Nombre y apellido: ");

                string correo = ValidadorConsola.LeerEmail("Correo: ");

                if (ExisteEmail(correo))
                {
                    Console.WriteLine(
                        "Ya existe una persona registrada con ese correo." +
                        " (intento " +
                        intento +
                        " de " +
                        ValidadorConsola.MaximoIntentos +
                        ")");
                    continue;
                }

                return new PersonaDatos(dni, correo, nombre);
            }

            throw new IntentosAgotadosException();
        }

    
        /// Flujo de la opción 1, pide seleccionar qué proceso se desea realizar y 
        /// delelega en el registro correspondiente (participante o mentor/jurado).
       
        private static void Registrar()
        {
            Console.WriteLine("=== REGISTRO ===");

            int proceso = ValidadorConsola.LeerEntero(
                "Seleccionar qué proceso desea realizar" +
                " (1 - Participante, 2 - Mentor/Jurado): ",
                1,
                2);

            if (proceso == 1)
                RegistrarParticipante();
            else
                RegistrarMentorJurado();
        }

        
        
       
        private static void RegistrarParticipante()
        {
            Console.WriteLine("=== REGISTRO DE PARTICIPANTE ===");

            // Parte común (participante, mentor y jurado comparten esto),
            // con verificación de duplicados y hasta 3 intentos.
            PersonaDatos datosPersona = SolicitarPersonaUnica();

            // Campos específicos del rol: competencia (ID Inscripción,
            // Provincia/País y Rol pertenecen a la clase base según la
            // consigna; la lista completa de Competidor lleva además
            // Lenguaje Principal).
            string provinciaPais = ValidadorConsola.LeerTextoSinNumeros(
                "Provincia o país: ");

            string rol = ValidadorConsola.LeerTextoSinNumeros(
                "Rol dentro del equipo: ");

            // Registro del Proyecto para la competencia (integrado, como
            // pide el diagrama de flujo).
            string nombreEquipo = ValidadorConsola.LeerTextoSinNumeros(
                "Nombre del equipo: ");

            CategoriaHackathon categoria =
                ValidadorConsola.LeerCategoria();

            // Lenguaje Principal es un campo del Competidor (consigna).
            string lenguaje = ValidadorConsola.LeerTextoSinNumeros(
                "Lenguaje principal: ");

            string tituloProyecto = ValidadorConsola.LeerTexto(
                "Nombre del proyecto: ");

            // Título, Descripción y Repositorio son los campos del
            // Proyecto según la consigna.
            string descripcion = ValidadorConsola.LeerTexto(
                "Descripción del proyecto: ");

            string repositorio = ValidadorConsola.LeerTexto(
                "URL del repositorio: ");

            Competidor competidor = new Competidor
            {
                Persona = datosPersona,
                IdInscripcion = ObtenerIdCompetidor(),
                ProvinciaPais = provinciaPais,
                Rol = rol,
                NombreEquipo = nombreEquipo,
                Categoria = categoria,
                LenguajePrincipal = lenguaje
            };

            // Crea la subclase concreta según la categoría elegida.
            Proyecto proyecto;

            switch (categoria)
            {
                case CategoriaHackathon.ESports:
                    proyecto = new ProyectoESports(
                        tituloProyecto,
                        descripcion,
                        repositorio);
                    break;

                case CategoriaHackathon.IoTDeportivo:
                    proyecto = new ProyectoIoTDeportivo(
                        tituloProyecto,
                        descripcion,
                        repositorio);
                    break;

                default: // CategoriaHackathon.Biomecanica
                    proyecto = new ProyectoBiomecanica(
                        tituloProyecto,
                        descripcion,
                        repositorio);
                    break;
            }

            // Asignación por los métodos del dominio (valida y enlaza).
            competidor.AsignarProyecto(proyecto);
            proyectos.Add(proyecto);
            competidores.Add(competidor);

            Console.WriteLine();
            Console.WriteLine(
                "El registro se realizó correctamente.");
            Console.WriteLine(
                "Proyecto: " + tituloProyecto);
            Console.WriteLine(
                "Categoría: " + CategoriaHelper.Nombre(categoria));
            Console.WriteLine(
                "ID Participante: " + competidor.IdInscripcion);
        }

        
        /// Registro de MENTOR/JURADO: primero el tipo, luego la parte
        /// común DNI, nombre y apellido, correo, con verificación de duplicados) y 
        /// después los campos del rol: categoría (SÓLO mentores: el jurado no tiene
        /// categoría porque evalúa proyectos de todas),
        /// años de experiencia e institución
      
        private static void RegistrarMentorJurado()
        {
            Console.WriteLine("=== REGISTRO DE MENTOR/JURADO ===");

            int tipoSeleccionado = ValidadorConsola.LeerEntero(
                "Tipo (1 - Mentor, 2 - Jurado): ",
                1,
                2);

            bool esMentor = tipoSeleccionado == 1;

            // Parte común (participante, mentor y jurado comparten esto),
            // con verificación de duplicados y hasta 3 intentos.
            PersonaDatos datosPersona = SolicitarPersonaUnica();

            // La categoría SÓLO se pide para mentores.
            CategoriaHackathon? categoriaAsignada = esMentor
                ? ValidadorConsola.LeerCategoria()
                : (CategoriaHackathon?)null;

            string expertise = ValidadorConsola.LeerTexto(
                "Expertise (área de especialidad): ");

            int experiencia = ValidadorConsola.LeerEntero(
                "Años de experiencia: ",
                1,
                50);

            string institucion = ValidadorConsola.LeerTexto(
                "Institución: ");

            MentorJurado mentor = new MentorJurado
            {
                Persona = datosPersona,
                IdInscripcion = ObtenerIdMentorJurado(
                    (TipoParticipante)tipoSeleccionado),
                Rol = esMentor ? "Mentor" : "Jurado",
                Tipo = (TipoParticipante)tipoSeleccionado,
                Expertise = expertise,
                Experiencia = experiencia,
                Institucion = institucion,
                CategoriaAsignada = categoriaAsignada
            };

            mentores.Add(mentor);

            Console.WriteLine();
            Console.WriteLine(
                "El registro se realizó correctamente.");
            Console.WriteLine(
                (esMentor ? "ID Mentor: " : "ID Jurado: ") +
                mentor.IdInscripcion);
        }

        
        /// Flujo de la opción 2: consulta. Muestra tres listados:
        /// participantes, mentores/jurados y proyectos con su estado de
        /// evaluación (promedio y puntaje, o "Pendiente").
     
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
                    // Los jurados no tienen categoría asignada: evalúan
                    // proyectos de todas las categorías.
                    string categoria = mentor.CategoriaAsignada.HasValue
                        ? CategoriaHelper.Nombre(
                            mentor.CategoriaAsignada.Value)
                        : "Todas";

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

      
        /// Flujo de la opción 3: reparte mentores entre los competidores que
        /// tienen proyecto. Para cada competidor sin mentor elige, de su
        /// categoría, al mentor MENOS cargado (balanceo) y lo asigna
        /// mediante el método del dominio, que valida e incrementa la
        /// carga del mentor.
       
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

                //Se elige al mentor MENOS cargado y la asignación
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

      
        /// Flujo de la opción 4: un mentor o un jurado (identificado por DNI)
        /// evalúa proyectos. Reglas:
        /// - Jurado: evalúa TODOS los proyectos que quiera (todas las
        ///   categorías; no tiene categoría asignada).
        /// - Mentor: sólo proyectos de su categoría y NO el proyecto del
        ///   equipo que acompaña (sólo los de otros equipos).
        /// Toma las tres notas (0-10), las registra en el proyecto (que
        /// recalcula promedios) y suma la evaluación al evaluador.
      
        private static void Evaluar()
        {
            Console.WriteLine("=== EVALUACIÓN DE PROYECTOS ===");

            string dniEvaluador =
                ValidadorConsola.LeerDni(
                    "DNI (mentor o jurado): ");

            // Puede ser mentor o jurado: ambos evalúan proyectos.
            MentorJurado evaluador =
                mentores.FirstOrDefault(
                    m =>
                        m.Dni != null &&
                        m.Dni.Equals(
                            dniEvaluador,
                            StringComparison.OrdinalIgnoreCase));

            if (evaluador == null)
            {
                Console.WriteLine(
                    "El DNI no corresponde a un mentor o jurado registrado.");

                return;
            }

            bool esJurado =
                evaluador.Tipo == TipoParticipante.Jurado;

            // Reglas de quién puede evaluar qué:
            // - Jurado: todos los proyectos que quiera (sin límite por
            //   categoría; no tiene categoría asignada).
            // - Mentor: proyectos de SU categoría, excluyendo el del
            //   equipo que acompaña (sólo los de otros equipos).
            List<Competidor> elegibles;

            if (esJurado)
            {
                elegibles =
                    competidores
                        .Where(c => c.ProyectoAsignado != null)
                        .ToList();

                Console.WriteLine(
                    "\nTodos los proyectos inscriptos" +
                    " (jurado: no está limitado por categoría):");
            }
            else
            {
                // (Mentor) Regla: sólo puede actuar dentro de su categoría.
                if (!evaluador.CategoriaAsignada.HasValue)
                {
                    Console.WriteLine(
                        "El mentor no tiene categoría asignada.");

                    return;
                }

                elegibles =
                    competidores
                        .Where(c =>
                            c.ProyectoAsignado != null &&
                            c.ProyectoAsignado.Categoria ==
                                evaluador.CategoriaAsignada.Value)
                        .ToList();

                // Un mentor no evalúa el proyecto del equipo que acompaña:
                // sólo proyectos de otros equipos.
                elegibles =
                    elegibles
                        .Where(c => c.MentorAsignado != evaluador)
                        .ToList();

                Console.WriteLine(
                    "\nProyectos de la categoría " +
                        CategoriaHelper.Nombre(
                            evaluador.CategoriaAsignada.Value) +
                        " (excluye los de su propio equipo):");
            }

            if (elegibles.Count == 0)
            {
                if (esJurado)
                {
                    Console.WriteLine(
                        "No hay proyectos inscriptos todavía.");
                }
                else
                {
                    Console.WriteLine(
                        "No hay proyectos elegibles para evaluar" +
                        " en la categoría " +
                        CategoriaHelper.Nombre(
                            evaluador.CategoriaAsignada.Value) +
                        " (distinto de sus propios equipos).");
                }

                return;
            }

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
                        " evaluador(es). Promedio: " +
                        proyecto.Promedio.Value.ToString("0.00");

                    if (proyecto.YaEvaluadoPor(dniEvaluador))
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

            // Cada evaluador puede evaluar todos los proyectos de su
            // categoría (cada uno una sola vez por proyecto); la nota
            // final es el promedio de todos los evaluadores.
            List<Competidor> pendientes =
                elegibles
                    .Where(c =>
                        !c.ProyectoAsignado.YaEvaluadoPor(dniEvaluador))
                    .ToList();

            if (pendientes.Count == 0)
            {
                Console.WriteLine(
                    "\nYa evaluó todos los proyectos que tenía disponibles.");
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
                    DniJurado = dniEvaluador,
                    Innovacion = innovacion,
                    Viabilidad = viabilidad,
                    Presentacion = presentacion
                });

            evaluador.IncrementarProyectosEvaluados();

            Console.WriteLine();
            Console.WriteLine(
                "Proyecto evaluado correctamente.");
            Console.WriteLine(
                "Promedio registrado: " +
                proyectoSeleccionado.Promedio.Value.ToString("0.00"));
        }

       
        /// Flujo de la opción 5: consulta. Ordena los proyectos por
        /// puntaje final (respetando el tipo concreto de cada proyecto
        /// con sus pesos de categoría) y los muestra de mayor a menor
        /// con posiciones (los empates comparten posición); los
        /// proyectos sin evaluación se muestran al final como
        /// "PENDIENTE", tal como pide el diagrama de flujo.
        
        private static void MostrarRanking()
        {
            if (proyectos.Count == 0)
            {
                Console.WriteLine("No hay proyectos registrados.");
                return;
            }

            List<Proyecto> evaluados =
                proyectos
                    .Where(p => p.EstaEvaluado())
                    .OrderByDescending(
                        p => p.CalcularPuntajeFinal())
                    .ToList();

            // Los proyectos sin evaluaciones entran al ranking pero con
            // la marca PENDIENTE (no compiten por una posición).
            List<Proyecto> pendientes =
                proyectos
                    .Where(p => !p.EstaEvaluado())
                    .ToList();

            Console.WriteLine("=== RANKING DE PROYECTOS ===");

            int posicion = 1;
            double puntajeAnterior = double.NaN;

            for (int i = 0; i < evaluados.Count; i++)
            {
                Proyecto proyecto = evaluados[i];
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

            foreach (Proyecto proyecto in pendientes)
            {
                Console.WriteLine(
                    "- " +
                    proyecto.Titulo +
                    " - PENDIENTE (sin evaluaciones)");
            }
        }

        /// Al inicio del programa: si existe el archivo de datos, carga el
        /// estado y lo reconstruye; si falla, arranca con listas vacías y
        /// deja el aviso para mostrar en el primer menú.
        
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

        private static void ReconstruirEstado(DatosHackathonDto datos)
        {
            competidores.Clear();
            mentores.Clear();
            proyectos.Clear();

            ultimoIdCompetidor = datos.UltimoIdCompetidor;
            ultimoIdMentorJurado = datos.UltimoIdMentorJurado;

            foreach (MentorJuradoDto dto in datos.MentoresJurados)
            {
                mentores.Add(new MentorJurado
                {
                    Persona = new PersonaDatos(dto.Dni, dto.Email, dto.Nombre),
                    IdInscripcion = dto.IdInscripcion,
                    ProvinciaPais = dto.ProvinciaPais,
                    Rol = dto.Rol,
                    Tipo = (TipoParticipante)dto.Tipo,
                    Expertise = dto.Expertise,
                    Experiencia = dto.Experiencia,
                    Institucion = dto.Institucion,
                    CategoriaAsignada = dto.CategoriaAsignada.HasValue
                        ? (CategoriaHackathon?)dto.CategoriaAsignada.Value
                        : null
                });
            }

            foreach (CompetidorDto dto in datos.Competidores)
            {
                competidores.Add(new Competidor
                {
                    Persona = new PersonaDatos(dto.Dni, dto.Email, dto.Nombre),
                    IdInscripcion = dto.IdInscripcion,
                    ProvinciaPais = dto.ProvinciaPais,
                    Rol = dto.Rol,
                    NombreEquipo = dto.NombreEquipo,
                    Categoria = (CategoriaHackathon)dto.Categoria,
                    LenguajePrincipal = dto.LenguajePrincipal
                });
            }

            foreach (ProyectoDto dto in datos.Proyectos)
            {
                Proyecto proyecto;

                switch (dto.Tipo)
                {
                    case nameof(ProyectoESports):
                        proyecto = new ProyectoESports(
                            dto.Titulo, dto.Descripcion, dto.Repositorio);
                        break;

                    case nameof(ProyectoIoTDeportivo):
                        proyecto = new ProyectoIoTDeportivo(
                            dto.Titulo, dto.Descripcion, dto.Repositorio);
                        break;

                    default:
                        proyecto = new ProyectoBiomecanica(
                            dto.Titulo, dto.Descripcion, dto.Repositorio);
                        break;
                }
                proyectos.Add(proyecto);

                Competidor competidor = competidores.FirstOrDefault(
                    c => c.Dni == dto.DniCompetidor);

                if (competidor != null)
                    competidor.AsignarProyecto(proyecto);

                foreach (EvaluacionDto evaluacion in dto.Evaluaciones)
                {
                    proyecto.AgregarEvaluacion(new EvaluacionProyecto
                    {
                        DniJurado = evaluacion.DniJurado,
                        Innovacion = evaluacion.Innovacion,
                        Viabilidad = evaluacion.Viabilidad,
                        Presentacion = evaluacion.Presentacion
                    });

                    MentorJurado jurado = mentores.FirstOrDefault(
                        m => m.Dni == evaluacion.DniJurado);

                    if (jurado != null)
                        jurado.IncrementarProyectosEvaluados();
                }
            }

            foreach (CompetidorDto dto in datos.Competidores)
            {
                if (dto.DniMentor == null)
                    continue;

                Competidor competidor = competidores.FirstOrDefault(
                    c => c.Dni == dto.Dni);
                MentorJurado mentor = mentores.FirstOrDefault(
                    m => m.Dni == dto.DniMentor);

                if (competidor != null && mentor != null)
                    competidor.AsignarMentor(mentor);
            }
        }
         /// Delega en la capa de persistencia el guardado del estado
        /// completo (con los contadores de ID). Silencioso salvo error:
        /// se llama tras cada operación modificadora y al salir
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
    }
    }

                    