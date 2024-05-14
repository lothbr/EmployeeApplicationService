using EmployeeApplicationService.DTOs;
using EmployeeApplicationService.Models;

namespace EmployeeApplicationService.Interfaces
{
    public interface IRepository
    {
		public  Task<Response> InsertQuestion(QuestionRequests questionRequests);
        public List<Question> GetQuestionsCreated(string questionId);
        public Response UpdateQuestion(QuestionRequests questionRequests);
        public Task<Response> CreateAppQuestions(CreateQuestionRequest? applicationData);
        public Task<List<ApplicationData>> GetAllCreatedForms();
        public Task<ApplicationData> GetSingleForm(string applicationId);
        public bool FindAppByID(string appId);
        Task<Response> UpdateApplication(UpdateApplicationform request, string id );
    }
}
