using System;

namespace Hackathon2026
{
    /// <summary>
    /// Datos de persona (ABSTRACCIÓN): los campos que son iguales para
    /// competidor, mentor y jurado (DNI, email y nombre), en una clase
    /// aparte. Al registrar un participante primero se crea esta parte
    /// común y después se "llama" para completar los campos específicos
    /// del rol en la subclase correspondiente.
    /// </summary>
    /// <remarks>
    /// ENCAPSULAMIENTO: DNI y email toman su valor una sola vez en el
    /// constructor (setter privado): son las claves de unicidad del
    /// registro y no pueden mutar por detrás una vez dada de alta la
    /// persona. Las comparaciones (MismoDni/MismoEmail) también quedan
    /// encapsuladas acá, reutilizadas por todas las verificaciones de
    /// duplicados.
    /// </remarks>
    public class PersonaDatos
    {
        /// <summary>DNI de la persona (clave de unicidad, inmutable).</summary>
        public string Dni { get; private set; }

        /// <summary>Email de la persona (clave de unicidad, inmutable).</summary>
        public string Email { get; private set; }

        /// <summary>Nombre y apellido de la persona.</summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Crea y fija los datos comunes de una persona.
        /// </summary>
        public PersonaDatos(string dni, string email, string nombre)
        {
            Dni = dni;
            Email = email;
            Nombre = nombre;
        }

        /// <summary>
        /// Compara con otro DNI sin distinguir mayúsculas (para
        /// las verificaciones de unicidad de registro).
        /// </summary>
        public bool MismoDni(string otroDni)
        {
            return Dni != null &&
                Dni.Equals(otroDni, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Compara con otro email sin distinguir mayúsculas.
        /// </summary>
        public bool MismoEmail(string otroEmail)
        {
            return Email != null &&
                Email.Equals(otroEmail, StringComparison.OrdinalIgnoreCase);
        }
    }
}
