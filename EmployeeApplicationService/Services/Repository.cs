
using Azure.Core;
using EmployeeApplicationService.Data;
using EmployeeApplicationService.DTOs;
using EmployeeApplicationService.Interfaces;
using EmployeeApplicationService.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;
using System.Reflection.Metadata;




namespace EmployeeApplicationService.Services
{
    public class Repository:IRepository
    {
        private readonly ILogService _logService;
        private readonly IConfigService _configService;
        private readonly ServiceContext _data;
        private readonly CosmosClient _cosmosClientService;
        public Repository(ILogService logService, IConfigService configService)
        {
            _logService = logService;
            _configService = configService;
            _data = new ServiceContext(_configService);
            _cosmosClientService = new CosmosClient(_configService.GetCosmosEndpoint(), _configService.GetCosmoskey());
        }

        public async Task<Response> CreateAppQuestions(CreateQuestionRequest? applicationData)
        {
            string MethodName = "CreateAppQuestions";
            var createAppResponse = new Response();
            try
            {
                var createReq = new ApplicationData()
                {
                    DateCreated = DateTime.Now,
                    ProgramDescription = applicationData?.ProgramDescription,
                    ProgramTitle = applicationData?.ProgramTitle,
                };
                createReq.Questions = applicationData?.Questions;

                _logService.Information(MethodName, $"About to App Details Insert ---> {JsonConvert.SerializeObject(createReq)}");
                
                _data.Applications.Add(createReq);
                var res = await _data.SaveChangesAsync();
                if (res > 0)
                {
                    createAppResponse.ResponseCode = "00";
                    createAppResponse.ResponseMessage = "Inserted Successfully";
                }
                else
                {
                    createAppResponse.ResponseCode = "01";
                    createAppResponse.ResponseMessage = "Something Went wrong while inserting";
                }
                _logService.Information(MethodName, $"Insert Response ---> {JsonConvert.SerializeObject(createAppResponse)}");
            }
            catch (Exception ex)
            {
                _logService.Error(MethodName, ex.Message);
            }
            return createAppResponse;
        }

        public async Task<Response> DeleteApplication(string appId)
        {
            var MethodName = "DeleteApplication";
            var response = new Response();
            try
            {
                //GET APPLICATION FORM 
                var formcreated = await GetSingleForm(appId);
                if (formcreated == null)
                {
                    response.ResponseCode = "01";
                    response.ResponseMessage = "Unable to Complete Request, No Form Matches your Application Id";
                    return response;
                }

                _logService.Information(MethodName, $"Delete Created Form --->");
                var container = _cosmosClientService.GetContainer(_configService.GetCosmosDbName(), "ApplicationData");
               
                var updateresult = await container.DeleteItemAsync<ApplicationData>(formcreated.Id, new PartitionKey($"{appId}"));
                response.ResponseCode = "00";
                response.ResponseMessage = "Success";
            }
            catch(CosmosException ex)
            {
                if(ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Unable to Locate Form";
                }
                _logService.Error(MethodName, ex.Message);
            }
            return response;
        }

        public bool FindAppByID(string appId)
        {
            var content = _data.Applications.Where(e=> e.Id == appId);
            if (content.Count() >0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<ApplicationData>> GetAllCreatedForms()
        {
            var forms = new List<ApplicationData>();
            try
            {
                _logService.Information("GetAllCreatedForms", $"Getting all Created Forms --->");
                var container = _cosmosClientService.GetContainer(_configService.GetCosmosDbName(), "ApplicationData");
                var query = new QueryDefinition("SELECT * FROM c");
                var result = container.GetItemQueryIterator<ApplicationData>(query);
                while (result.HasMoreResults)
                {
                    var formcontents = await result.ReadNextAsync();
                    foreach (var item in formcontents)
                    {
                        forms.Add(item);
                    };
                }
               
               
            }
            catch (Exception ex)
            {
                _logService.Error("GetAllCreatedForms", ex.Message);
            }
            return forms;
            
        }

        public List<Question> GetQuestionsCreated(string questiontype)
        {
            var questionlist = new List<Question>();
            var Questions=  _data.Applications.Select(e => e.Questions).ToList();
            foreach (var item in Questions)
            {
                if (item.Any(e=> e.QuestionType.ToLower() == questiontype.ToLower()))
                {
                    foreach (var ques in item)
                    {
                        questionlist.Add(ques);
                    }
                }
            }
            return questionlist;
        }

        public async Task<ApplicationData> GetSingleForm(string applicationId)
        {
            var form = new ApplicationData();
            try
            {
                _logService.Information("GetSingleForm", $"Getting Single Forms --->");
                var container = _cosmosClientService.GetContainer(_configService.GetCosmosDbName(), "ApplicationData");
                var query = new QueryDefinition($"SELECT * FROM c where c.Id = \"{applicationId}\"");
                var result = container.GetItemQueryIterator<ApplicationData>(query);
                while (result.HasMoreResults)
                {
                    var formcontents = await result.ReadNextAsync();
                    foreach (var item in formcontents)
                    {
                        form = (ApplicationData?) item;
                    };
                }
            }
            catch (Exception ex)
            {
                _logService.Error("GetSingleForm", ex.Message);
                
            }

            return form;
        }

        public async Task<Response> InsertQuestion(QuestionRequests questionRequests)
        {
            string MethodName = "InsertQuestion";
            var questionResponse = new Response();
            try
            {
                var input = new Question();
                input.QuestionContent = questionRequests.QuestionContent;
                input.QuestionType = questionRequests.QuestionType;

                if (input?.QuestionType?.ToLower() == "dropdown" )
                {
                    input.dropdowns = JsonConvert.DeserializeObject<Dropdown>(questionRequests?.Answers?.ToString());
                }
                else if(input?.QuestionType?.ToLower() == "multiplechoice")
                {
                    input.multipleChoices = JsonConvert.DeserializeObject<MultipleChoice>(questionRequests?.Answers?.ToString());
                }
                else if (input?.QuestionType?.ToLower() == "date")
                {
                    input.date = JsonConvert.DeserializeObject<Date>(questionRequests?.Answers?.ToString());
                }
                else if (input?.QuestionType?.ToLower() == "number")
                {
                    input.Number = JsonConvert.DeserializeObject<Number>(questionRequests?.Answers?.ToString());
                }
                else if (input?.QuestionType?.ToLower() == "yesno")
                {
                    input.yes = JsonConvert.DeserializeObject<Yes_No>(questionRequests?.Answers?.ToString());
                }
                else
                {
                    input.paragraph = JsonConvert.DeserializeObject<Paragraph>(questionRequests?.Answers?.ToString());
                }
                
                _logService.Information(MethodName, $"About to Insert ---> {JsonConvert.SerializeObject(input)}");
                _data.Questions.Add(input);
                var res = await _data.SaveChangesAsync();
                if (res>0)
                {
                    questionResponse.ResponseCode = "00";
                    questionResponse.ResponseMessage = "Inserted Successfully";
                }
                _logService.Information(MethodName, $"Insert Response ---> {JsonConvert.SerializeObject(questionResponse)}");
               
            }
            catch (Exception ex)
            {
                _logService.Error(MethodName, ex.Message);
            }
            return questionResponse;
        }

        public async Task<Response> SubmitApplication(ApplicationData applicationData, string ID)
        {
            var methodName = "SubmitApplication";
            var response = new Response();
            try
            {
                _logService.Information(methodName, $"About to Submit Application with Id {ID} --> {JsonConvert.SerializeObject(applicationData)}");
                var form = await _data.Applications.Where(e => e.Id == ID).FirstOrDefaultAsync();
                if (form != null)
                {
                    form.Profile= applicationData.Profile;
                    form.Questions= applicationData.Questions;
                    form.DateCreated = applicationData.DateCreated;
                    form.DateAnswered = applicationData.DateAnswered;
                    form.DateModified = applicationData.DateModified;
                    form.ProgramDescription= applicationData.ProgramDescription;
                    form.ProgramTitle = applicationData.ProgramTitle;
                    var res = await _data.SaveChangesAsync(); 
                    if(res>0)
                    {
                        response.ResponseCode = "00";
                        response.ResponseMessage = "Success";
                    }
                    else
                    {
                        response.ResponseCode = "01";
                        response.ResponseMessage = "Something Went wrong while Submitting";
                    }
                }
                else
                {

                    response.ResponseCode = "03";
                    response.ResponseMessage = "Unable to Locate Record";
                }


                
            }
            catch (Exception ex)
            {
                response.ResponseCode = "02";
                response.ResponseMessage = "Something Went Wrong";
                _logService.Error(methodName, ex.Message);
            }
            _logService.Information(methodName, $"Operation Respone {ID} --> {JsonConvert.SerializeObject(response)}");
            return response;
        }

        public async Task<Response> UpdateApplication(UpdateApplicationform request, string ID)
        {
            var MethodName = "UpdateApplication";
            var response = new Response();
            try
            {
                _logService.Information(MethodName, $"About to Update Application with Id {ID} --> {JsonConvert.SerializeObject(request)}");
                var form = await _data.Applications.Where(e => e.Id == ID).FirstOrDefaultAsync();
                if (form != null)
                {
                    form.DateModified = DateTime.Now;
                    form.ProgramDescription = request.ProgramDescription;
                    form.Questions = request.Questions;
                    form.ProgramTitle = request.ProgramTitle;
                    var res = await _data.SaveChangesAsync();
                    if (res > 0)
                    {
                        response.ResponseCode = "00";
                        response.ResponseMessage = "Success";
                    }
                    else
                    {
                        response.ResponseCode = "01";
                        response.ResponseMessage = "Something Went wrong while Submitting";
                    }
                }
                else
                {
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Unable to Locate Form";
                }
               
                
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    response.ResponseCode = "02";
                    response.ResponseMessage = "Unable to Locate Form";
                }
                _logService.Error(MethodName, ex.Message);
            }
            _logService.Information(MethodName, $"Operation Respone {ID} --> {JsonConvert.SerializeObject(response)}");

            return response;
        }

        public Response UpdateQuestion(QuestionRequests questionRequests)
        {
            throw new NotImplementedException();
        }
    }
}
