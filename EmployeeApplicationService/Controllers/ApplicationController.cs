using EmployeeApplicationService.DTOs;
using EmployeeApplicationService.Interfaces;
using EmployeeApplicationService.Models;
using EmployeeApplicationService.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

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
        [HttpPut("EditApplicationForm/{ApplicationId}")]
        public async Task<ActionResult> EditAppForm(string ApplicationId, [FromBody] UpdateApplicationform request )
        {
            if (ValidationUtility.IsValidText(ApplicationId))
            {
                stopcheck = true;
                stopMessage = "Invalid ID";
            }

            if (!stopcheck)
            {
                var getform = _repository.FindAppByID(ApplicationId);
                if (!getform)
                {
                    stopcheck = true;
                    stopMessage = "Unable to Locate Record";
                }
            }

            if (!stopcheck)
            {
                foreach (var item in request?.Questions)
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
                var response = await _repository.UpdateApplication(request, ApplicationId);
                if (response.ResponseCode != "00")
                {
                    return Ok("Unable to Complete Request at the Moment");
                }
                return Ok(response);
            }
            return BadRequest(stopMessage);
        }

        [HttpPost("SubmitApplicationForm")]
        public ActionResult SubmitAppForm(SubmitFormRequest request)
        {
            return Ok();
        }


        [HttpDelete("DeleteCreated_ApplicationForm")]
        public ActionResult DeleteAppForm()
        {
            return Ok();
        }

        [HttpGet("GetAll_CreatedApplicationForms")]
        public async Task<ActionResult> GetAllForms()
        {
            var formcreated = await _repository.GetAllCreatedForms();
            if (formcreated.Count >0)
            {
                return Ok(formcreated);
            }
            return Ok("No Form Available at the Moment");
        }

        [HttpGet("GetApplicationForm_ByID/{ApplicationId}")]
        public async Task<ActionResult> GetFormByID(string ApplicationId)
        {
            if (!ValidationUtility.IsValidText(ApplicationId))
            {
                return Ok(await _repository.GetSingleForm(ApplicationId));
            }
            
            return BadRequest("Invalid Parameter on ApplicationId");
        }
    }
}
