using EmployeeApplicationService.DTOs;

namespace EmployeeApplicationService.Models
{
    public class UpdateApplicationform
    {
        public string? ProgramTitle { get; set; }
        public string? ProgramDescription { get; set; }
        public List<Question>? Questions { get; set; }
    }
}
