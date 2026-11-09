using System;
using System.Collections.Generic;
using System.Linq;

namespace Hackathon2026
{
    /// <summary>
    /// Clase base (ABSTRACCIÓN): representa un proyecto inscripto en el
    /// hackathon. Guarda los datos generales de todo proyecto (título,
    /// descripción, repositorio, categoría) y las evaluaciones emitidas
    /// por los jurados.
    /// </summary>
    /// <remarks>
    /// ABSTRACCIÓN: el puntaje final cambia según la categoría
    /// (distintos pesos para cada nota), por eso
    /// <see cref="CalcularPuntajeFinal"/> es abstracto: la clase base
    /// declara el comportamiento y cada subclase
    /// (<see cref="ProyectoESports"/>, <see cref="ProyectoIoTDeportivo"/>,
    /// <see cref="ProyectoBiomecanica"/>) define "cómo" se calcula.
    /// El resto del programa lo usa polimórficamente, sin saber qué
    /// subclase está detrás.
    ///
    /// ENCAPSULAMIENTO: la lista real de evaluaciones es privada; a afuera
    /// sólo se ve en modo lectura (<see cref="Evaluaciones"/> como
    /// IReadOnlyList). Las notas expuestas también son de escritura
    /// privada: sólo pueden cambiar mediante <see cref="AgregarEvaluacion"/>,
    /// que valida el rango 0-10, rechaza la duplicación de jurado y
    /// recalcula los promedios. Se garantiza la integridad del estado.
    /// </remarks>
    public abstract class Proyecto
    {
        /// <summary>
        /// Lista real (privada) de evaluaciones recibidas.
        /// ENCAPSULAMIENTO: nadie la modifica de afuera; sólo
        /// <see cref="AgregarEvaluacion"/> la completa.
        /// </summary>
        private readonly List<EvaluacionProyecto> evaluaciones =
            new List<EvaluacionProyecto>();

        public string Titulo { get; private set; }
        public string Descripcion { get; private set; }
        public string Repositorio { get; private set; }
        public CategoriaHackathon Categoria { get; private set; }

        /// <summary>Promedio de innovación entre los jurados (sólo lectura desde afuera; lo recalcula <see cref="AgregarEvaluacion"/>).</summary>
        public double Innovacion { get; private set; }

        /// <summary>Promedio de viabilidad entre los jurados.</summary>
        public double Viabilidad { get; private set; }

        /// <summary>Promedio de presentación entre los jurados.</summary>
        public double Presentacion { get; private set; }

        /// <summary>
        /// Promedio simple de las tres notas (calculado promediando los
        /// promedios por jurado); null mientras no haya evaluaciones.
        /// </summary>
        public double? Promedio { get; private set; }

        /// <summary>
        /// Vistas de sólo lectura de las evaluaciones
        /// (IReadOnlyList: puede recorrerlas, pero no agregar/quitar).
        /// </summary>
        public IReadOnlyList<EvaluacionProyecto> Evaluaciones
        {
            get
            {
                return evaluaciones.AsReadOnly();
            }
        }

        /// <summary>
        /// Constructor protegido: sólo pueden llamarlo las subclases.
        /// Fija los datos invariables del proyecto.
        /// </summary>
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

        /// <summary>
        /// Registra la evaluación de un jurado y recalcula las notas
        /// expuestas (son el promedio entre todos los jurados).
        /// </summary>
        /// <exception cref="ArgumentNullException">evaluacion es null.</exception>
        /// <exception cref="ArgumentException">Alguna nota está fuera del rango 0 a 10.</exception>
        /// <exception cref="InvalidOperationException">
        /// El jurado ya había evaluado este proyecto (un jurado evalúa una
        /// sola vez por proyecto).
        /// </exception>
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

        /// <summary>
        /// Indica si el jurado con el DNI dado ya evaluó este proyecto.
        /// Se usa en el flujo de evaluación para listarle los pendientes.
        /// </summary>
        public bool YaEvaluadoPor(string dniJurado)
        {
            return evaluaciones.Any(e =>
                e.DniJurado != null &&
                e.DniJurado.Equals(
                    dniJurado,
                    StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Indica si el proyecto recibió al menos una evaluación.
        /// Un proyecto no evaluado no participa del ranking.
        /// </summary>
        public bool EstaEvaluado()
        {
            return evaluaciones.Count > 0;
        }

        /// <summary>
        /// Contrato abstracto (polimorfismo): puntaje ponderado con los
        /// pesos propios de la categoría del proyecto.
        /// </summary>
        public abstract double CalcularPuntajeFinal();
    }
}
