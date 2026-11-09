using System;
using System.Collections.Generic;
using System.Linq;

namespace Hackathon2026
{
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
}
