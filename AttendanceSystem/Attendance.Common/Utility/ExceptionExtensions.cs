﻿namespace Attendance.Common.Utility
{
    public static class ExceptionExtensions
    {
        public static string GetMessage(this Exception ex)
        {
            if (ex == null)
                return null;

            if (ex.InnerException != null)
            {
                return ex.Message + ": " + ex.InnerException.Message;
            }

            return ex.Message;
        }
    }
}
