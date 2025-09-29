using System.Security.Cryptography;

namespace UserManagementWebapp.Helpers
{
    public class PasswordHasher
    {
        private static readonly int SaltSize = 256 / 8;
        private static readonly int HashSize = 256 / 8;
        private static readonly int Iterations = 1000_000;
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;
        public static byte[] GenHashedPassword(string password, byte[] salt)
        {
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            return Rfc2898DeriveBytes.Pbkdf2(passwordBytes, salt, Iterations, HashAlgorithm, HashSize);
        }

        public static byte[] GenSalt()
        {
            var salt = new byte[SaltSize];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}
