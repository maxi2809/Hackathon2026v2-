using System;

namespace Hackathon2026
{
    /// <summary>
    /// Señala que se usaron los 3 intentos permitidos para un ingreso por
    /// consola sin lograr un valor válido. (ENCAPSULAMIENTO de la regla de
    /// reintentos: cada lector de <see cref="ValidadorConsola"/> lanza esta
    /// excepción en lugar de hacer bucles infinitos.)
    /// </summary>
    /// <remarks>
    /// La excepción asciende hasta el menú principal, que la captura, muestra el
    /// mensaje y regresa al menú sin interrumpir el programa.
    /// </remarks>
    public class IntentosAgotadosException : Exception
    {
        /// <summary>Mensaje a mostrar al usuario cuando se agotan los intentos.</summary>
        public const string Mensaje =
            "Ya se utilizaron los 3 intentos disponibles. Debe volver al menú principal.";

        public IntentosAgotadosException()
            : base(Mensaje)
        {
        }
    }
}
