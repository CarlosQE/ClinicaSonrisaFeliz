using System.Security.Cryptography;

namespace ClinicaDental.Web.Services.Seguridad
{
    /// <summary>
    /// Hash de contraseñas con PBKDF2-HMAC-SHA256 (recomendado por OWASP), sal aleatoria de 16 bytes
    /// y 100 000 iteraciones. Formato guardado: "PBKDF2$iteraciones$salBase64$hashBase64".
    /// La verificación usa comparación en tiempo constante para evitar ataques de tiempo.
    /// </summary>
    public static class HasherContrasena
    {
        private const int Iteraciones = 100_000;
        private const int TamanoSal = 16;
        private const int TamanoHash = 32;
        private static readonly HashAlgorithmName Algoritmo = HashAlgorithmName.SHA256;

        public static string Hashear(string contrasena)
        {
            var sal = RandomNumberGenerator.GetBytes(TamanoSal);
            var hash = Rfc2898DeriveBytes.Pbkdf2(contrasena, sal, Iteraciones, Algoritmo, TamanoHash);
            return $"PBKDF2${Iteraciones}${Convert.ToBase64String(sal)}${Convert.ToBase64String(hash)}";
        }

        public static bool Verificar(string contrasena, string hashGuardado)
        {
            var partes = hashGuardado.Split('$');
            if (partes.Length != 4 || partes[0] != "PBKDF2" || !int.TryParse(partes[1], out var iteraciones)) return false;

            try
            {
                var sal = Convert.FromBase64String(partes[2]);
                var esperado = Convert.FromBase64String(partes[3]);
                var calculado = Rfc2898DeriveBytes.Pbkdf2(contrasena, sal, iteraciones, Algoritmo, esperado.Length);
                return CryptographicOperations.FixedTimeEquals(calculado, esperado);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
