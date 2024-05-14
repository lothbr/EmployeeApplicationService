using EmployeeApplicationService.Interfaces;

namespace EmployeeApplicationService.Services
{

    public class ConfigService:IConfigService
    {
        private IConfiguration _configuration;
        public ConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetCosmosDbName()
        {
            return _configuration.GetConnectionString("CosmosDB");
        }

        public string GetCosmosEndpoint()
        {
            return _configuration.GetConnectionString("CosmosEndpoint");
        }

        public string GetCosmoskey()
        {
            return _configuration.GetConnectionString("CosmosKey");
        }

        public string GetQuestionTypes()
        {
            return _configuration.GetSection("QuestionType").Value;
        }
    }
}
