namespace EmployeeApplicationService.DTOs
{
    public class MultipleChoice
    {
        public List<string>? Choice { get; set; }
        public string? Others { get; set; }
        public string? MaxNumberofChoice { get; set; }
        public List<string>? ChoiceSelected { get; set; }
    }
}
