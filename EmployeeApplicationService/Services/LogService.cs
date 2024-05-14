using EmployeeApplicationService.Interfaces;
using Serilog;

namespace EmployeeApplicationService.Services
{
    public class LogService: ILogService
    {
        public void Information( string methodName, string message)
        {
            Log.Information("\nMethod Name: {@methodName} \nMessage: {@message}\n",  methodName, message);
        }

        public void Error(string methodName, string message)
        {
            Log.Error("\nMethod Name: {@methodName} \nError: {@message}\n", methodName, message);
        }
    }
}
