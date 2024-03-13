namespace Attendance.Common.Constants
{
    public static class StringEnum
    {
        public static string ToValue(this Enum thisEnum)
        {
            string output = null;
            var type = thisEnum.GetType();

            var fieldInfo = type.GetField(thisEnum.ToString());
            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(StringValue), false) as StringValue[];
                if (attrs != null && attrs.Length > 0)
                {
                    output = attrs[0].Value;
                }
            }

            return output;
        }

        public static T GetValueFromStringValue<T>(string value) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(StringValue)) is StringValue attribute)
                {
                    if (attribute.Value == value)
                    {
                        return (T)field.GetValue(null);
                    }
                    else
                    {
                        if (field.Name == value)
                        {
                            return (T)field.GetValue(null);
                        }
                    }
                }
            }

            throw new ArgumentException("Not Found", value);
        }

        private class StringValue : Attribute
        {
            public StringValue(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }

        public enum AppErrorCode
        {
            Error,
            Warning,
            Info
        }

        public enum EmailExceptionType
        {
            [StringValue("Info")] Info,
            [StringValue("Warning")] Warning,
            [StringValue("Error")] Error,
        }

        public enum Roles
        {
            [StringValue("GeneralEmployee")] GeneralEmployee,
            [StringValue("Administrator")] Administrator,
            [StringValue("Manager")] Manager
        }

        public enum Sex
        {
           Male,
           Female,
           Other
        }

        public enum Departments
        {
            [StringValue("Dev")] Dev,
            [StringValue("QA")] QA,
            [StringValue("Manager")] Manager
        }

        public enum Bands
        {
            A1,
            A2,
            A3,
            A4,
            A5,
            A6,
            A7,
            A8
        }

        public enum ManagerTypes
        {
            DepartmentManager,
            GeneralManager
        }

        public enum AttendanceRecordType
        {
            Arrival = 1,
            Leave = 2,
        }

        public enum EmployeeTypes
        {
           Dev,
           QA,
           Manager
        }
    }
}
