using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hackathon2026
{
    public enum CategoriaHackathon
    {
        ESports = 1,
        IoTDeportivo = 2,
        Biomecanica = 3
    }

    public enum TipoParticipante
    {
        Mentor = 1,
        Jurado = 2
    }

    public static class CategoriaHelper
    {
        public static string Nombre(CategoriaHackathon categoria)
        {
            switch (categoria)
            {
                case CategoriaHackathon.ESports:
                    return "E-Sports";

                case CategoriaHackathon.IoTDeportivo:
                    return "IoT Deportivo";

                case CategoriaHackathon.Biomecanica:
                    return "Biomecánica";

                default:
                    return "Categoría desconocida";
            }
        }
    }

    public static class ValidadorConsola
    {
        public static string LeerTexto(string mensaje)
        {
            string valor;

            do
            {
                Console.Write(mensaje);
                valor = Console.ReadLine() ?? string.Empty;
            }
            while (string.IsNullOrWhiteSpace(valor));

            return valor.Trim();
        }

        public static string LeerNombre(string mensaje)
        {
            string nombre;

            do
            {
                Console.Write(mensaje);
                nombre = Console.ReadLine() ?? string.Empty;

                bool valido = !string.IsNullOrWhiteSpace(nombre);

                foreach (char caracter in nombre)
                {
                    if (!char.IsLetter(caracter) && caracter != ' ')
                    {
                        valido = false;
                        break;
                    }
                }

                if (valido)
                    return nombre.Trim();

                Console.WriteLine("Nombre inválido.");
            }
            while (true);
        }

        public static string LeerDni(string mensaje)
        {
            string dni;

            do
            {
                Console.Write(mensaje);
                dni = (Console.ReadLine() ?? string.Empty).Trim();

                long numero;

                if (long.TryParse(dni, out numero) &&
                    dni.Length >= 7 &&
                    dni.Length <= 8)
                {
                    return dni;
                }

                Console.WriteLine(
                    "DNI inválido. Debe contener entre 7 y 8 números.");
            }
            while (true);
        }

        public static string LeerEmail(string mensaje)
        {
            string email;

            do
            {
                Console.Write(mensaje);
                email = (Console.ReadLine() ?? string.Empty).Trim();

                if (Regex.IsMatch(
                    email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    return email;
                }

                Console.WriteLine("Email inválido.");
            }
            while (true);
        }

        public static string LeerTelefono(string mensaje)
        {
            string telefono;

            do
            {
                Console.Write(mensaje);

                telefono = Console.ReadLine() ?? string.Empty;

                string limpio = telefono
                    .Replace("+", "")
                    .Replace("-", "")
                    .Replace(" ", "")
                    .Replace("(", "")
                    .Replace(")", "");

                long numero;

                if (long.TryParse(limpio, out numero) &&
                    limpio.Length >= 8 &&
                    limpio.Length <= 15)
                {
                    return telefono;
                }

                Console.WriteLine("Teléfono inválido.");
            }
            while (true);
        }

        public static int LeerEntero(
            string mensaje,
            int minimo,
            int maximo)
        {
            int valor;

            do
            {
                Console.Write(mensaje);

                if (int.TryParse(
                    Console.ReadLine(),
                    out valor) &&
                    valor >= minimo &&
                    valor <= maximo)
                {
                    return valor;
                }

                Console.WriteLine("Valor inválido.");
            }
            while (true);
        }

        // FIX: acepta coma o punto como separador decimal,
        // independente de la cultura de la máquina.
        public static double LeerDouble(
            string mensaje,
            double minimo,
            double maximo)
        {
            double valor;

            do
            {
                Console.Write(mensaje);

                string entrada = (Console.ReadLine() ?? string.Empty)
                    .Trim()
                    .Replace(' ', '.')
                    .Replace(',', '.');

                if (double.TryParse(
                        entrada,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out valor) &&
                    valor >= minimo &&
                    valor <= maximo)
                {
                    return valor;
                }

                Console.WriteLine("Número inválido.");
            }
            while (true);
        }

        public static CategoriaHackathon LeerCategoria()
        {
            Console.WriteLine();
            Console.WriteLine("1 - E-Sports");
            Console.WriteLine("2 - IoT Deportivo");
            Console.WriteLine("3 - Biomecánica");

            int opcion = LeerEntero(
                "Categoría: ",
                1,
                3);

            return (CategoriaHackathon)opcion;
        }

        public static string LeerTextoSinNumeros(string mensaje)
        {
            string valor;

            do
            {
                Console.Write(mensaje);
                valor = Console.ReadLine() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(valor))
                {
                    bool contieneNumero = false;

                    foreach (char caracter in valor)
                    {
                        if (char.IsDigit(caracter))
                        {
                            contieneNumero = true;
                            break;
                        }
                    }

                    if (!contieneNumero)
                        return valor.Trim();
                }

                Console.WriteLine(
                    "El valor no puede estar vacío ni contener números.");
            }
            while (true);
        }
    }

    public abstract class ParticipanteHackathon
    {
        public string IdInscripcion { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string ProvinciaPais { get; set; }
        public string Rol { get; set; }

        public abstract double CalcularMetricaBase();
    }

    public class MentorJurado : ParticipanteHackathon
    {
        public TipoParticipante Tipo { get; set; }
        public string Expertise { get; set; }
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

    public class EvaluacionProyecto
    {
        public string DniJurado { get; set; }
        public double Innovacion { get; set; }
        public double Viabilidad { get; set; }
        public double Presentacion { get; set; }
    }

    public abstract class Proyecto
    {
        private readonly List<EvaluacionProyecto> evaluaciones =
            new List<EvaluacionProyecto>();

        public string Titulo { get; private set; }
        public string Descripcion { get; private set; }
        public string Repositorio { get; private set; }
        public CategoriaHackathon Categoria { get; private set; }

        public double Innovacion { get; private set; }
        public double Viabilidad { get; private set; }
        public double Presentacion { get; private set; }
        public double? Promedio { get; private set; }

        public IReadOnlyList<EvaluacionProyecto> Evaluaciones
        {
            get
            {
                return evaluaciones.AsReadOnly();
            }
        }

        protected Proyecto(
            string titulo,
            string descripcion,
            string repositorio,
            CategoriaHackathon categoria)
        {
            Titulo = titulo;
            Descripcion = descripcion;
            Repositorio = repositorio;
            Categoria = categoria;
        }

        public void AgregarEvaluacion(EvaluacionProyecto evaluacion)
        {
            if (evaluacion == null)
                throw new ArgumentNullException(nameof(evaluacion));

            if (evaluacion.Innovacion < 0 ||
                evaluacion.Innovacion > 10 ||
                evaluacion.Viabilidad < 0 ||
                evaluacion.Viabilidad > 10 ||
                evaluacion.Presentacion < 0 ||
                evaluacion.Presentacion > 10)
            {
                throw new ArgumentException(
                    "Las notas deben estar entre 0 y 10.");
            }

            if (YaEvaluadoPor(evaluacion.DniJurado))
            {
                throw new InvalidOperationException(
                    "El jurado ya evaluó este proyecto.");
            }

            evaluaciones.Add(evaluacion);

            Innovacion = evaluaciones.Average(e => e.Innovacion);
            Viabilidad = evaluaciones.Average(e => e.Viabilidad);
            Presentacion = evaluaciones.Average(e => e.Presentacion);

            Promedio = evaluaciones.Average(e =>
                (e.Innovacion + e.Viabilidad + e.Presentacion) / 3);
        }

        public bool YaEvaluadoPor(string dniJurado)
        {
            return evaluaciones.Any(e =>
                e.DniJurado != null &&
                e.DniJurado.Equals(
                    dniJurado,
                    StringComparison.OrdinalIgnoreCase));
        }

        public bool EstaEvaluado()
        {
            return evaluaciones.Count > 0;
        }

        public abstract double CalcularPuntajeFinal();
    }

    public class ProyectoESports : Proyecto
    {
        public ProyectoESports(
            string titulo,
            string descripcion,
            string repositorio)
            : base(
                titulo,
                descripcion,
                repositorio,
                CategoriaHackathon.ESports)
        {
        }

        public override double CalcularPuntajeFinal()
        {
            return (Innovacion * 0.40) +
                   (Viabilidad * 0.30) +
                   (Presentacion * 0.30);
        }
    }

    public class ProyectoIoTDeportivo : Proyecto
    {
        public ProyectoIoTDeportivo(
            string titulo,
            string descripcion,
            string repositorio)
            : base(
                titulo,
                descripcion,
                repositorio,
                CategoriaHackathon.IoTDeportivo)
        {
        }

        public override double CalcularPuntajeFinal()
        {
            return (Innovacion * 0.50) +
                   (Viabilidad * 0.30) +
                   (Presentacion * 0.20);
        }
    }

    public class ProyectoBiomecanica : Proyecto
    {
        public ProyectoBiomecanica(
            string titulo,
            string descripcion,
            string repositorio)
            : base(
                titulo,
                descripcion,
                repositorio,
                CategoriaHackathon.Biomecanica)
        {
        }

        public override double CalcularPuntajeFinal()
        {
            return (Innovacion * 0.30) +
                   (Viabilidad * 0.50) +
                   (Presentacion * 0.20);
        }
    }

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

        private static void Main(string[] args)
        {
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

                opcion = ValidadorConsola.LeerEntero(
                    "\nSeleccionar una Opción: ",
                    1,
                    8);

                if (!Console.IsOutputRedirected)
                    Console.Clear();

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

                if (opcion != 8)
                {
                    Console.WriteLine("\nPulse una tecla...");

                    // FIX: permite probar con entrada redirigida sin excepción.
                    if (Console.IsInputRedirected)
                        Console.ReadLine();
                    else
                        Console.ReadKey(true);
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
    }
}
