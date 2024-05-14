using EmployeeApplicationService.DTOs;

namespace EmployeeApplicationService.Models
{
    public class QuestionResponse<T>
    {
        public string? QuestionContent { get; set; }
        public string? QuestionType { get; set; }
        public T? Answers { get; set; }

    }
}
