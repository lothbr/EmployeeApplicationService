using System.ComponentModel.DataAnnotations;

namespace EmployeeApplicationService.DTOs
{
    public class ApplicationData
    {
        [Required]
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? ProgramTitle { get; set; }
        public string? ProgramDescription { get; set; }
        public Profile? Profile { get; set; }
        public List<Question>? Questions { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
