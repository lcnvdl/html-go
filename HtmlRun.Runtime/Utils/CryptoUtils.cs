using System.Security.Cryptography;
using System.Text;

namespace HtmlRun.Runtime.Utils;

public static class CryptoUtils
{
  public static string HashWithSHA256(string value)
  {
    using var hash = SHA256.Create();
    var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
    return Convert.ToHexString(byteArray);
  }
}