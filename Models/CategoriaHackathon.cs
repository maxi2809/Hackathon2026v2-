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
}
