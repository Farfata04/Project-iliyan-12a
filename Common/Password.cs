using System.Text;

namespace WebChat.Common;

public abstract class Password
{
    public static string Hash(string password)
    {
        return Convert.ToBase64String(Encoding.Default.GetBytes(password));
    }

    public static string Generate()
    {
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890+_-!@#$%";
        int charactersLength = characters.Length;
        
        string password = "";
        for (int i = 0; i < 6; ++i) {
            password += characters[(int)Math.Floor(Random.Shared.NextDouble() * charactersLength)];
        }

        return password;
    }
}