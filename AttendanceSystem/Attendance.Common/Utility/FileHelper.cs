namespace Attendance.Common.Utility
{
    public static class FileHelper
    {
        public static string GetEmailTemplateDirectory(string emailTemplate)
        {
            var path = new string[] { FindSolutionPath(), "Attendance.Service" , "HtmlEmails", emailTemplate };
            return Path.Combine(path);
        }

        private static string FindSolutionPath()
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;

            while (currentPath != null)
            {
                string[] solutionFiles = Directory.GetFiles(currentPath, "*.sln");
                if (solutionFiles.Length > 0)
                {
                    return currentPath;
                }

                currentPath = Directory.GetParent(currentPath)?.FullName;
            }

            return null;
        }
    }
}
