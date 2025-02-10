using BCrypt.Net;

public static class BCryptService
{
    private static int WorkFactor = 12; // Por defecto

    public static void SetWorkFactor(int factor)
    {
        WorkFactor = factor;
    }

    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }
    public static bool VerificarPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
