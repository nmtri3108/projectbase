using Attendance.Common.Constants;

namespace Attendance.Common.Exceptions
{
    public class AppException : Exception
    {
        public StringEnum.AppErrorCode ErrorCode { get; set; }

        public AppException(string message, StringEnum.AppErrorCode errorCode = StringEnum.AppErrorCode.Error) :
            base(message)
        {
            ErrorCode = errorCode;
        }

        public AppException(string message, Exception ex,
            StringEnum.AppErrorCode errorCode = StringEnum.AppErrorCode.Error) : base(message, ex)
        {
            ErrorCode = errorCode;
        }
    }
}
