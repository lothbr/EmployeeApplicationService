namespace EmployeeApplicationService.Interfaces
{
    public interface ILogService
    {
		void Information( string methodName, string message);
        void Error(string methodName, string message);
    }
}
