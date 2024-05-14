using EmployeeApplicationService.DTOs;
using System.Text.Json.Serialization;

namespace EmployeeApplicationService.Models
{
    public class CreateQuestionRequest
    {
        public string? ProgramTitle { get; set; }
        public string? ProgramDescription { get; set; }
        public List<Question>? Questions { get; set; }
        
    }
}
