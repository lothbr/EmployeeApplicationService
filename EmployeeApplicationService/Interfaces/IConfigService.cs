namespace EmployeeApplicationService.Interfaces
{
    public interface IConfigService
    {
		public string GetCosmosEndpoint();
        public string GetCosmoskey();
        public string GetCosmosDbName();
        public string GetQuestionTypes();
    }
}
