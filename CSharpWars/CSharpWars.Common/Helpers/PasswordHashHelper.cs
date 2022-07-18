using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace CSharpWars.Helpers;

public interface IPasswordHashHelper
{
    (string salt, string hashed) CalculateHash(string password, string salt = null);
}

public class PasswordHashHelper : IPasswordHashHelper
{
    private readonly Random _random = new Random();

    public (string salt, string hashed) CalculateHash(string password, string salt = null)
    {
        if (salt == null)
        {
            var saltData = new byte[16];
            _random.NextBytes(saltData);

            salt = Convert.ToBase64String(saltData);
        }

        var sha1 = KeyDerivation.Pbkdf2(password: password, salt: Convert.FromBase64String(salt), prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 32);
        var hashed = Convert.ToBase64String(sha1);

        return (salt, hashed);
    }
}