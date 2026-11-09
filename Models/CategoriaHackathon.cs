// Enumeraciones del dominio: definen el conjunto cerrado de valores válidos
// que usan otras clases (categorías del hackathon y roles de mentoría).

namespace Hackathon2026
{
    /// <summary>
    /// Categoría/track en la que compite un equipo con su proyecto.
    /// Los valores numéricos (1..3) se muestran en el menú de selección.
    /// </summary>
    public enum CategoriaHackathon
    {
        ESports = 1,
        IoTDeportivo = 2,
        Biomecanica = 3
    }

    /// <summary>
    /// Rol que cumple un <see cref="MentorJurado"/>: mentor de un equipo
    /// o jurado que evalúa proyectos.
    /// </summary>
    public enum TipoParticipante
    {
        Mentor = 1,
        Jurado = 2
    }

    /// <summary>
    /// Helper con la conversión de la enumeración
    /// <see cref="CategoriaHackathon"/> a texto legible para el usuario.
    /// (ABSTRACCIÓN: oculta la relación número <-> nombre de la categoría):
    /// el resto del programa sólo pide el nombre y no sabe de números.
    /// </summary>
    public static class CategoriaHelper
    {
        /// <summary>
        /// Devuelve el nombre para mostrar de la categoría
        /// (con acentos y espacios), o "Categoría desconocida" si
        /// no corresponde a ningún valor de la enumeración.
        /// </summary>
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
}
