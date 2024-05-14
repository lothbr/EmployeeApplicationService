using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApplicationService.DTOs
{
    public class Question
    {
        public string QuestionId { get; set; } = Guid.NewGuid().ToString();
        public string? QuestionContent { get; set; }
        public string? QuestionType { get; set; }
        public Dropdown? dropdowns { get; set; }
        public MultipleChoice? multipleChoices { get; set; }
        public Date? date { get; set; }
        public Number? Number { get; set; }
        public Yes_No? yes { get; set; }
        public Paragraph? paragraph { get; set; }


    }
}
