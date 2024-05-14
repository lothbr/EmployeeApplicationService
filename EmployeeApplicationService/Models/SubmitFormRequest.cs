using EmployeeApplicationService.DTOs;

namespace EmployeeApplicationService.Models
{
    public class SubmitFormRequest
    {
        public string? ProgramTitle { get; set; }
        public string? ProgramDescription { get; set; }
        public Profile? Profile { get; set; }
        public List<Question>? Questions { get; set; }
    }
}
