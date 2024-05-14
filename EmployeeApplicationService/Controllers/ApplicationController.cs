using EmployeeApplicationService.Interfaces;
using EmployeeApplicationService.Models;
using EmployeeApplicationService.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApplicationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
		private readonly IConfigService _configuration;
        private readonly ILogService _logService;
        private readonly IRepository _repository;
        private  bool stopcheck = false;
        private  string stopMessage = string.Empty;

        public ApplicationController(IConfigService configuration, ILogService logService, IRepository repository)
        {
            _configuration = configuration;
            _logService = logService;
            _repository = repository;
        }

        [HttpPost("CreateApplicationForm")]
        public async Task<ActionResult> CreateApplicationData(CreateQuestionRequest applicationData)
        {
            try
            {
                if (applicationData== null || applicationData.Questions == null)
                {
                    stopcheck = true;
                    stopMessage = "Unable to Create Application No questions Added";
                }

                if (!stopcheck)
                {
                    if (ValidationUtility.IsValidText(applicationData?.ProgramTitle))
                    {
                        stopcheck = true;
                        stopMessage = "Invalid Program Title";
                    }
                    if (ValidationUtility.IsValidText(applicationData?.ProgramDescription))
                    {
                        stopcheck = true;
                        stopMessage = "Invalid Program Description";
                    }
                }
                if (!stopcheck)
                {
                    foreach (var item in applicationData.Questions)
                    {
                        var allowedQuestionType = _configuration.GetQuestionTypes().ToLower().Split(',');
                        if (!allowedQuestionType.Any(e => e.Equals(item?.QuestionType?.ToLower())))
                        {
                            stopcheck = true;
                            stopMessage = "Invalid QuestionType Detected";
                        }
                    }
                }

                if (!stopcheck)
                {
                    var response = await _repository.CreateAppQuestions(applicationData);
                    if (response.ResponseCode=="00")
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }


                return Ok(stopMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.StackTrace);

            }
        }
        [HttpPut("EditApplicationForm")]
        public ActionResult EditAppForm()
        {
            return Ok();
        }

        [HttpPost("SubmitApplicationForm")]
        public ActionResult CreateQuestion(CreateQuestionRequest request)
        {
            return Ok();
        }


        [HttpDelete("DeleteCreated_ApplicationForm")]
        public ActionResult DeleteAppForm()
        {
            return Ok();
        }

        [HttpGet("GetAll_ApplicationForms")]
        public ActionResult GetAllForms(CreateQuestionRequest request)
        {
            return Ok();
        }

        [HttpGet("GetApplicationForm_ByID")]
        public ActionResult GetFormByID(CreateQuestionRequest request)
        {
            return Ok();
        }
    }
}
