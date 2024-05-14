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
                    return Ok( await _repository.CreateAppQuestions(applicationData));
                    
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

        [HttpPost("SubmitApplicationForm/{ApplicationID}")]
        public async Task<ActionResult> SubmitAppForm(string ApplicationID, SubmitFormRequest request)
        {
            var GetForm = new ApplicationData();
            try
            {
                if (request == null || request.Questions == null)
                {
                    stopcheck = true;
                    stopMessage = "Unable to Create Application No questions Added";
                }

                if (!stopcheck)
                {
                    if (ValidationUtility.IsValidText(ApplicationID))
                    {
                        stopcheck = true;
                        stopMessage = "Invalid ApplicationID";
                    }
                }

                if (!stopcheck)
                {
                    foreach (var item in request.Questions)
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
                    GetForm =await  _repository.GetSingleForm(ApplicationID);
                    if (GetForm == null )
                    {
                        stopcheck = true;
                        stopMessage = "Unable to Find Application Form";
                    }
                }

                if (!stopcheck)
                {
                    return Ok( await _repository.SubmitApplication(new ApplicationData()
                    {
                        Id= GetForm?.Id?.Split('|')[1],
                        ProgramTitle = GetForm?.ProgramTitle, 
                        ProgramDescription = GetForm?.ProgramDescription,
                        DateAnswered = DateTime.Now,
                        Questions= request?.Questions,
                        Profile= request?.Profile,
                        DateCreated= GetForm.DateCreated,
                        DateModified= DateTime.Now,
                    },ApplicationID));

                    
                }
            }
            
             catch (Exception ex)
            {
                return BadRequest(ex.StackTrace);

            }
            return Ok(stopMessage);
        }
           

        [HttpDelete("DeleteCreated_ApplicationForm/{AppFormID}")]
        public async Task<ActionResult> DeleteAppForm(string AppFormID)
        {
            if (!ValidationUtility.IsValidText(AppFormID))
            {
                return Ok(await _repository.DeleteApplication(AppFormID));
            }

            return BadRequest("Invalid Parameter on ApplicationId");
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
