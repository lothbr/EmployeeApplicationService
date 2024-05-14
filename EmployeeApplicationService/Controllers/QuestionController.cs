using EmployeeApplicationService.DTOs;
using EmployeeApplicationService.Interfaces;
using EmployeeApplicationService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;

namespace EmployeeApplicationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
		private readonly ILogService _logService;
        private readonly IConfigService _configService;
        private readonly IRepository _repositories;
        public QuestionController(ILogService logService, IConfigService configService, IRepository repository)
        {
            _logService = logService;
            _configService = configService;
            _repositories = repository;
        }

        private bool stopcheck = false;
        private string stopMessage = string.Empty;

        [HttpGet("GetQuestionSchemas_By_Type/{questionType}")]
        public ActionResult GetQuestionsByType(string questionType)
        {
            string MethodName = "GetQuestionsByType";
            var response = new QuestionResponse<dynamic>();

            if (string.IsNullOrEmpty(questionType))
            {
                stopcheck = true;
                stopMessage = "Invalid QuestionType";
            }
            _logService.Information(MethodName, $"QuestionType Received is {questionType}");
            if(!stopcheck)
            {
                try
                {
                    switch (questionType.ToLower())
                    {
                        case "paragraph":
                            response = new QuestionResponse<dynamic>()
                            {
                                QuestionContent = "Enter your Question here",
                                QuestionType = QuestionTypes.Paragraph.GetDisplayName(),
                                Answers = new Paragraph() { Value = "Enter the Answer here" },
                            };
                            break;
                        case "yesno":
                            response = new QuestionResponse<dynamic>()
                            {
                                QuestionContent = "Enter your Question here",
                                QuestionType = QuestionTypes.YesNo.GetDisplayName(),
                                Answers = new Yes_No() { Value = false },
                            };
                            break;
                        case "dropdown":
                            response = new QuestionResponse<dynamic>()
                            {
                                QuestionContent = "Enter your Question here",
                                QuestionType = QuestionTypes.Dropdown.GetDisplayName(),
                                Answers = new Dropdown() { Choice = [], Others = "enter others value here" },
                            };
                            break;
                        case "multiplechoice":
                            response = new QuestionResponse<dynamic>()
                            {
                                QuestionContent = "Enter your Question here",
                                QuestionType = QuestionTypes.MultipleChoice.GetDisplayName(),
                                Answers = new MultipleChoice() { Choice = [], Others = "enter others value here", MaxNumberofChoice = "Enter Max number of Choice" },
                            };
                            break;
                        case "date":
                            response = new QuestionResponse<dynamic>()
                            {
                                QuestionContent = "Enter your Question here",
                                QuestionType = QuestionTypes.Date.GetDisplayName(),
                                Answers = new Date(),
                            };
                            break;
                        case "number":
                            response = new QuestionResponse<dynamic>()
                            {
                                QuestionContent = "Enter your Question here",
                                QuestionType = QuestionTypes.Number.GetDisplayName(),
                                Answers = new Number(),
                            };
                            break;
                        default:
                            return BadRequest("Invalid QuestionType");
                    }
                    _logService.Information(MethodName, $"Response ----> {JsonConvert.SerializeObject(response)}");
                }
                catch (Exception ex)
                {
                    _logService.Error(MethodName, ex.Message);
                    
                }
            }
            return Ok(response);
        }

        [HttpPost("CreateQuestions")]
        public async Task<ActionResult> PostQuestions(QuestionRequests requests) 
        {
            string MethodName = "PostQuestions";
            var allowedQuestionType = _configService.GetQuestionTypes().ToLower().Split(',');
            if (!allowedQuestionType.Any(e=> e.Equals(requests?.QuestionType?.ToLower())))
            {
                stopcheck = true;
                stopMessage = "Invalid QuestionType";
            }

            if (!stopcheck)
            {
                _logService.Information(MethodName, $"Request Received---> {JsonConvert.SerializeObject(requests)}");

                var response = await _repositories.InsertQuestion(requests);
                if (response.ResponseCode== "00")
                {
                    return Ok(response);
                }
            }
            return BadRequest(stopMessage);
        }

        [HttpGet("GetCreatedQuestion_By_QuestionTypes/{questionType}")]
        public ActionResult GetCreatedquestions (string questionType)
        {
            string MethodName = "GetCreatedquestions";
            var allowedQuestionType = _configService.GetQuestionTypes().ToLower().Split(',');
            if (!allowedQuestionType.Any(e => e.Equals(questionType.ToLower())))
            {
                stopcheck = true;
                stopMessage = "Invalid QuestionType";
            }
            if (!stopcheck)
            {
                _logService.Information(MethodName, $"QuestionType Received---> {JsonConvert.SerializeObject(questionType)}");
                 var details = _repositories.GetQuestionsCreated(questionType);
                if (details != null)
                {
                    return Ok(details);
                }
                return Ok("No Record at the Moment");
            }
            return BadRequest(stopMessage);
        }
    }
}
