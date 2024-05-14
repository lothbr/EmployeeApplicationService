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
    }
}
