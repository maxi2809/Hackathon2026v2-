using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Hackathon2026
{
    /// <summary>
    /// Capa de UI de entrada: centraliza la lectura de datos desde la
    /// consola. Todos los métodos "Leer..." no devuelven el control al
    /// invocador hasta que el usuario ingresa un valor válido, por lo que
    /// el resto del programa no necesita validar lo que entra.
    /// </summary>
    public static class ValidadorConsola
    {
        /// <summary>
        /// Pide un texto cualquiera, repitiendo mientras esté vacío o con
        /// sólo espacios. Devuelve el texto sin espacios en los extremos.
        /// </summary>
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

        /// <summary>
        /// Pide un nombre propio: sin vacío y compuesto sólo por letras y
        /// espacios. Repite el ingreso hasta ser válido.
        /// </summary>
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

        /// <summary>
        /// Pide un DNI: sólo dígitos, entre 7 y 8 caracteres. Se devuelve
        /// como string para no perder ceros a la izquierda.
        /// </summary>
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

        /// <summary>
        /// Pide un correo con formato básico
        /// "algo@algo.algo" (validación por expresión regular).
        /// </summary>
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

        /// <summary>
        /// Pide un teléfono y valida los dígitos que quedan al quitar
        /// formatos (+, guiones, espacios y paréntesis): entre 8 y 15
        /// dígitos. El valor devuelto conserva el formato original.
        /// </summary>
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

        /// <summary>
        /// Pide un número entero dentro del rango [minimo, maximo],
        /// repitiendo hasta obtener un valor válido. Se usa para las
        /// opciones del menú y otros valores numéricos.
        /// </summary>
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

        /// <summary>
        /// Pide un número decimal dentro del rango [minimo, maximo].
        /// Acepta coma o punto como separador decimal (cualquier cultura).
        /// Se usa para las notas de evaluación.
        /// </summary>
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

        /// <summary>
        /// Muestra las opciones de categoría y devuelve la elegida
        /// (siempre es un valor válido de <see cref="CategoriaHackathon"/>).
        /// </summary>
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

        /// <summary>
        /// Pide un texto sin vacío y sin dígitos (para nombres de equipo,
        /// provincias, etc.). Reintenta hasta devolver un valor correcto.
        /// </summary>
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
}
