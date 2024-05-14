﻿
using EmployeeApplicationService.Data;
using EmployeeApplicationService.DTOs;
using EmployeeApplicationService.Interfaces;
using EmployeeApplicationService.Models;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
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
            return  _data.Questions.Where(e=> e.QuestionType.ToLower() == questiontype.ToLower()).ToList();
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

        public async Task<Response> UpdateApplication(UpdateApplicationform request, string ID)
        {
            var MethodName = "UpdateApplication";
            var response = new Response();
            try
            {
                _logService.Information(MethodName, $"About to Update Application with Id {ID} --> {JsonConvert.SerializeObject(request)}");
                var container = _cosmosClientService.GetContainer(_configService.GetCosmosDbName(), "ApplicationData");
                var updaterequest = new ApplicationData()
                {
                    DateModified = DateTime.Now,
                    ProgramDescription = request.ProgramDescription,
                    Questions = request.Questions,
                    ProgramTitle = request.ProgramTitle,
                };
                
                var updateresult = await container.UpsertItemAsync(updaterequest ,new PartitionKey(updaterequest.Id));
                if (updateresult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    response.ResponseCode = "00";
                    response.ResponseMessage = "Success";
                }
                
            }
            catch (Exception ex)
            {
                _logService.Error(MethodName, ex.Message);
                
            }

            return response;
        }

        public Response UpdateQuestion(QuestionRequests questionRequests)
        {
            throw new NotImplementedException();
        }
    }
}