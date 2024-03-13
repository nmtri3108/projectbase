namespace Attendance.Common.Constants
{
    public class AppSettings
    {
        public static string ConnectionStrings { get; private set; }
        public static string Secret { get; set; }
        public static string CORS { get; set; }

        // refresh token time to live (in days), inactive tokens are
        // automatically deleted from the database after this time
        public static int RefreshTokenTTL { get; set; }

        public static string Mail { get; set; }
        public static string DisplayName { get; set; }
        public static string Password { get; set; }
        public static string Host { get; set; }
        public static int Port { get; set; }
    }
}
