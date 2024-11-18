using System.Security.Cryptography;
using System.Text;

namespace UrlShortener.Api;

public static class HashService
{
    public static string Get8LengthHash(string input)
    {
        using HashAlgorithm algorithm = SHA256.Create();
        var byteHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        var base64 = Convert.ToBase64String(byteHash).Replace('+', '-').Replace('/', '_');
        return base64[..8];
    }
}