using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Hackathon2026
{
    /// <summary>
    /// Capa de entrada (UI): centraliza la lectura de datos desde la
    /// consola. Todos los métodos "Leer..." permiten hasta
    /// <see cref="MaximoIntentos"/> ingresos; si en ninguno obtienen un valor
    /// válido, lanzan <see cref="IntentosAgotadosException"/> para abortar el
    /// flujo actual y regresar al menú principal con el aviso correspondiente.
    /// Si el valor es válido, se devuelve sin que el resto del programa
    /// necesite revalidarlo.
    /// </summary>
    public static class ValidadorConsola
    {
        /// <summary>
        /// Cantidad máxima de intentos por ingreso antes de abortar el flujo.
        /// </summary>
        public const int MaximoIntentos = 3;

        /// <summary>
        /// Pide un texto cualquiera con hasta 3 intentos mientras esté vacío o
        /// con sólo espacios. Devuelve el texto sin espacios en los extremos.
        /// </summary>
        public static string LeerTexto(string mensaje)
        {
            for (int intento = 1; intento <= MaximoIntentos; intento++)
            {
                Console.Write(mensaje);
                string valor = Console.ReadLine() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(valor))
                    return valor.Trim();

                Console.WriteLine("Texto inválido.");
            }

            throw new IntentosAgotadosException();
        }

        /// <summary>
        /// Pide un nombre propio (sin vacío y compuesto sólo por letras y
        /// espacios) con hasta 3 intentos. Lanza
        /// <see cref="IntentosAgotadosException"/> si fallan tres.
        /// </summary>
        public static string LeerNombre(string mensaje)
        {
            for (int intento = 1; intento <= MaximoIntentos; intento++)
            {
                Console.Write(mensaje);
                string nombre = Console.ReadLine() ?? string.Empty;

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

            throw new IntentosAgotadosException();
        }

        /// <summary>
        /// Pide un DNI (sólo dígitos, entre 7 y 8 caracteres) con hasta
        /// 3 intentos. Se devuelve como string para no perder ceros a la
        /// izquierda. Lanza <see cref="IntentosAgotadosException"/> si
        /// fallan tres.
        /// </summary>
        public static string LeerDni(string mensaje)
        {
            for (int intento = 1; intento <= MaximoIntentos; intento++)
            {
                Console.Write(mensaje);
                string dni = (Console.ReadLine() ?? string.Empty).Trim();

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

            throw new IntentosAgotadosException();
        }

        /// <summary>
        /// Pide un correo con formato básico "algo@algo.algo" con hasta 3
        /// intentos (validación por expresión regular). Lanza
        /// <see cref="IntentosAgotadosException"/> si fallan tres.
        /// </summary>
        public static string LeerEmail(string mensaje)
        {
            for (int intento = 1; intento <= MaximoIntentos; intento++)
            {
                Console.Write(mensaje);
                string email = (Console.ReadLine() ?? string.Empty).Trim();

                if (Regex.IsMatch(
                    email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    return email;
                }

                Console.WriteLine("Email inválido.");
            }

            throw new IntentosAgotadosException();
        }

        /// <summary>
        /// Pide un número entero dentro del rango [minimo, maximo] con hasta
        /// 3 intentos; se usa para las opciones del menú y otros números.
        /// Lanza <see cref="IntentosAgotadosException"/> si fallan tres.
        /// </summary>
        public static int LeerEntero(
            string mensaje,
            int minimo,
            int maximo)
        {
            for (int intento = 1; intento <= MaximoIntentos; intento++)
            {
                Console.Write(mensaje);

                int valor;

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

            throw new IntentosAgotadosException();
        }

        /// <summary>
        /// Pide un número decimal dentro del rango [minimo, maximo] con hasta
        /// 3 intentos. Acepta coma o punto como separador decimal
        /// (cualquier cultura). Lanza
        /// <see cref="IntentosAgotadosException"/> si fallan tres.
        /// </summary>
        public static double LeerDouble(
            string mensaje,
            double minimo,
            double maximo)
        {
            for (int intento = 1; intento <= MaximoIntentos; intento++)
            {
                Console.Write(mensaje);

                string entrada = (Console.ReadLine() ?? string.Empty)
                    .Trim()
                    .Replace(' ', '.')
                    .Replace(',', '.');

                double valor;

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

            throw new IntentosAgotadosException();
        }

        /// <summary>
        /// Muestra las opciones de categoría y devuelve la elegida con hasta
        /// 3 intentos (siempre es un valor válido de
        /// <see cref="CategoriaHackathon"/>).
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
        /// Pide un texto (sin vacío y sin dígitos) con hasta 3 intentos
        /// (para nombres de equipo, provincias, etc.). Lanza
        /// <see cref="IntentosAgotadosException"/> si fallan tres.
        /// </summary>
        public static string LeerTextoSinNumeros(string mensaje)
        {
            for (int intento = 1; intento <= MaximoIntentos; intento++)
            {
                Console.Write(mensaje);
                string valor = Console.ReadLine() ?? string.Empty;

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

            throw new IntentosAgotadosException();
        }
    }
}
