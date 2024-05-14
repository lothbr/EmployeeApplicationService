using EmployeeApplicationService.DTOs;

namespace EmployeeApplicationService.Models
{
    public class SubmitFormRequest
    {
       
        public Profile? Profile { get; set; }
        public List<Question>? Questions { get; set; }
    }
}
