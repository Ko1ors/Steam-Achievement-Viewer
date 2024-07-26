using Serilog;
using System.Diagnostics;


namespace Sav.Common.Logs
{
    public static class Log
    {
        public static ILogger Logger
        {
            get
            {
                var methodBase = new StackTrace()?.GetFrame(1)?.GetMethod();
                var callerType = methodBase?.DeclaringType;
                var logger = Serilog.Log.ForContext(callerType ?? typeof(object));
                var sourceContext = methodBase?.DeclaringType?.FullName;
                var memberName = methodBase?.Name;

                return logger
                    .ForContext("SourceContext", sourceContext)
                    .ForContext("MemberName", memberName);
            }
        }
    }
}
