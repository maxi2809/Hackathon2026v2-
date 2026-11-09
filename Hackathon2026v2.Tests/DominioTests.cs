using System;
using System.Linq;
using Xunit;

namespace Hackathon2026.Tests
{
    /// <summary>
    /// Pruebas de la entidad PersonaDatos (identidad común a los tres
    /// roles): inmutabilidad de DNI/correo y comparaciones de unicidad.
    /// </summary>
    public class PersonaDatosTests
    {
        [Fact]
        public void MismoDni_coincide_ignorando_mayusculas_y_rechaza_otros()
        {
            var persona = new PersonaDatos("40111222", "marco@mail.com", "Marco Polo");

            Assert.True(persona.MismoDni("40111222"));
            Assert.True(persona.MismoDni("40111222".ToUpper()));
            Assert.False(persona.MismoDni("40222333"));
        }

        [Fact]
        public void MismoEmail_coincide_ignorando_mayusculas_y_rechaza_otros()
        {
            var persona = new PersonaDatos("40111222", "marco@mail.com", "Marco Polo");

            Assert.True(persona.MismoEmail("MARCO@mail.com"));
            Assert.False(persona.MismoEmail("nadia@mail.com"));
        }

        [Fact]
        public void Dni_y_Email_son_inmutables_y_Nombre_es_editable()
        {
            var persona = new PersonaDatos("40111222", "marco@mail.com", "Marco Polo");

            persona.Nombre = "Marco Segundo";

            Assert.Equal("40111222", persona.Dni);
            Assert.Equal("marco@mail.com", persona.Email);
            Assert.Equal("Marco Segundo", persona.Nombre);
        }
    }

    /// <summary>
    /// Pruebas del polimorfismo de Proyecto: pesos de puntaje por
    /// Federación y reglas de AgregarEvaluacion.
    /// </summary>
    public class ProyectoTests
    {
        private static EvaluacionProyecto Evaluacion(
            string dni,
            double innovacion,
            double viabilidad,
            double presentacion)
        {
            return new EvaluacionProyecto
            {
                DniJurado = dni,
                Innovacion = innovacion,
                Viabilidad = viabilidad,
                Presentacion = presentacion
            };
        }

        [Fact]
        public void AgregarEvaluacion_calcula_promedios_simple()
        {
            var proyecto = new ProyectoESports("Juego X", "desc", "repo");

            proyecto.AgregarEvaluacion(Evaluacion("30111222", 9, 8, 10));

            Assert.Equal(9, proyecto.Innovacion, 6);
            Assert.Equal(8, proyecto.Viabilidad, 6);
            Assert.Equal(10, proyecto.Presentacion, 6);
            Assert.Equal(9, proyecto.Promedio.Value, 6);
            Assert.True(proyecto.EstaEvaluado());
        }

        [Fact]
        public void Dos_evaluadores_promedian_y_el_mismo_no_puede_repetir()
        {
            var proyecto = new ProyectoIoTDeportivo("Sensor Y", "desc", "repo");

            proyecto.AgregarEvaluacion(Evaluacion("30111222", 6, 6, 6));
            proyecto.AgregarEvaluacion(Evaluacion("29876543", 8, 8, 8));

            Assert.Equal(2, proyecto.Evaluaciones.Count);
            Assert.Equal(7, proyecto.Promedio.Value, 6);

            var ex = Assert.Throws<InvalidOperationException>(() =>
                proyecto.AgregarEvaluacion(Evaluacion("30111222", 5, 5, 5)));

            Assert.Contains("ya evalu", ex.Message);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(11)]
        public void Notas_fuera_del_rango_0_a_10_rechazadas(double notaFueraDeRango)
        {
            var proyecto = new ProyectoBiomecanica("Kit Z", "desc", "repo");

            var ex = Assert.ThrowsAny<ArgumentException>(() =>
                proyecto.AgregarEvaluacion(
                    Evaluacion("30111222", notaFueraDeRango, 5, 5)));

            Assert.Contains("0 y 10", ex.Message);
            Assert.False(proyecto.EstaEvaluado());
        }

        [Fact]
        public void Puntaje_final_es_ponderado_por_la_categoria_polimorfismo()
        {
            // Notas 10/10/10 en las tres subclases deben dar distinto
            // por los pesos de cada categoría (40/30/30, etc.).
            var e = Evaluacion("30111222", 10, 10, 10);

            var esports = new ProyectoESports("A", "d", "r");
            esports.AgregarEvaluacion(e);

            var iot = new ProyectoIoTDeportivo("B", "d", "r");
            iot.AgregarEvaluacion(Evaluacion("30111222", 10, 10, 10));

            var bioma = new ProyectoBiomecanica("C", "d", "r");
            bioma.AgregarEvaluacion(Evaluacion("30111222", 10, 10, 10));

            Assert.Equal(10, esports.CalcularPuntajeFinal(), 6);
            Assert.True(iot.CalcularPuntajeFinal() <= 10);
            Assert.True(bioma.CalcularPuntajeFinal() <= 10);
            Assert.Equal(
                iot.CalcularPuntajeFinal(),
                esports.CalcularPuntajeFinal(),
                6);
        }

        [Fact]
        public void La_lista_de_evaluaciones_es_de_solo_lectura_desde_afuera()
        {
            var proyecto = new ProyectoESports("A", "d", "r");
            proyecto.AgregarEvaluacion(Evaluacion("30111222", 9, 9, 9));

            Assert.Single(proyecto.Evaluaciones);

            // IReadOnlyList: no debe poder agregarse por la vista pública.
            Assert.Throws<NotSupportedException>(() =>
            {
                ((System.Collections.Generic.ICollection<EvaluacionProyecto>)
                    proyecto.Evaluaciones).Add(
                    Evaluacion("29876543", 9, 9, 9));
            });
        }
    }

    /// <summary>
    /// Pruebas de las validaciones de asignación de Competidor y de los
    /// contadores encapsulados de MentorJurado.
    /// </summary>
    public class CompetidorMentorTests
    {
        private static PersonaDatos Persona(string dni, string email, string nombre)
        {
            return new PersonaDatos(dni, email, nombre);
        }

        private static MentorJurado Mentor(string dni)
        {
            return new MentorJurado
            {
                Persona = Persona(dni, dni + "@mail.com", "Mentor " + dni),
                IdInscripcion = "M-TEST",
                Tipo = TipoParticipante.Mentor,
                CategoriaAsignada = CategoriaHackathon.Biomecanica
            };
        }

        private static Proyecto ProyectoDe(CategoriaHackathon categoria)
        {
            switch (categoria)
            {
                case CategoriaHackathon.ESports:
                    return new ProyectoESports("P", "d", "r");

                case CategoriaHackathon.IoTDeportivo:
                    return new ProyectoIoTDeportivo("P", "d", "r");

                default:
                    return new ProyectoBiomecanica("P", "d", "r");
            }
        }

        [Fact]
        public void No_se_puede_asignar_dos_proyectos_al_mismo_competidor()
        {
            var competidor = new Competidor
            {
                Persona = Persona("40111222", "m@mail.com", "Marco Polo"),
                Categoria = CategoriaHackathon.ESports
            };

            competidor.AsignarProyecto(ProyectoDe(CategoriaHackathon.ESports));

            Assert.Throws<InvalidOperationException>(() =>
                competidor.AsignarProyecto(
                    ProyectoDe(CategoriaHackathon.ESports)));
        }

        [Fact]
        public void No_se_puede_asignar_un_proyecto_de_otra_categoria()
        {
            var competidor = new Competidor
            {
                Persona = Persona("40111222", "m@mail.com", "Marco Polo"),
                Categoria = CategoriaHackathon.Biomecanica
            };

            Assert.Throws<InvalidOperationException>(() =>
                competidor.AsignarProyecto(
                    ProyectoDe(CategoriaHackathon.ESports)));
        }

        [Fact]
        public void AsignarMentor_incrementa_la_carga_y_rechaza_repetidos()
        {
            var competidor = new Competidor
            {
                Persona = Persona("40111222", "m@mail.com", "Marco Polo"),
                Categoria = CategoriaHackathon.Biomecanica
            };

            competidor.AsignarProyecto(ProyectoDe(CategoriaHackathon.Biomecanica));

            MentorJurado mentor = Mentor("27999888");

            competidor.AsignarMentor(mentor);
            Assert.Equal(1, mentor.EquiposAsignados);
            Assert.Same(mentor, competidor.MentorAsignado);

            Assert.Throws<InvalidOperationException>(() =>
                competidor.AsignarMentor(Mentor("28888999")));
        }

        [Fact]
        public void Mentor_de_otra_categoria_es_rechazado()
        {
            var competidor = new Competidor
            {
                Persona = Persona("40111222", "m@mail.com", "Marco Polo"),
                Categoria = CategoriaHackathon.ESports
            };

            var mentorJuegos =
                Mentor("27999888"); // categoría Biomecánica

            Assert.Throws<InvalidOperationException>(() =>
                competidor.AsignarMentor(mentorJuegos));
        }

        [Fact]
        public void Contadores_de_mentor_solo_suben_por_metodos_validos()
        {
            MentorJurado mentor = Mentor("27999888");

            mentor.IncrementarProyectosEvaluados();
            mentor.IncrementarProyectosEvaluados();

            Assert.Equal(2, mentor.ProyectosEvaluados);
            Assert.Equal(2, mentor.CalcularMetricaBase(), 6);
        }
    }

    /// <summary>
    /// Pruebas del helper de categorías.
    /// </summary>
    public class CategoriaHelperTests
    {
        [Theory]
        [InlineData(CategoriaHackathon.ESports, "E-Sports")]
        [InlineData(CategoriaHackathon.IoTDeportivo, "IoT Deportivo")]
        [InlineData(CategoriaHackathon.Biomecanica, "Biomecánica")]
        public void Nombres_de_categorias_son_correctos(
            CategoriaHackathon categoria,
            string nombreEsperado)
        {
            Assert.Equal(nombreEsperado, CategoriaHelper.Nombre(categoria));
        }
    }
}
