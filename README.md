# EmployeeApplicationService
# Swagger Interface
![image](https://github.com/lothbr/EmployeeApplicationService/assets/39369616/fad5affe-0602-4d55-888f-1661df7f1674)
# Assumptions
1. The Task required that all candidate's information will be spooled from the Database
2. The task required that all candidates' questions and answers be stored in the Database
3. The task required the Backend to follow the sample Figma Example to provision the Payload for Candidate Questions and Answers
4. The task required the Whole CRUD(create, read, update and delete) Operations to be performed.

# Implementations
1. The Application is built on Dot Net Core 8.0 Framework, with robust Dependency Injection, Exception catching with the capability of File Logging. 
2. The Application Provides a schema for creating the questions as provided in the Figma presentation at https://www.figma.com/community/file/1291807396898113201/create-program-design-and-flow
3. Based on the swagger page Above there are two Controllers the Question Controller and the Application Controller
  a. The question Controller handles the schematics of how the question is provided to be utilized in the application Controller
  b. the Application Controller is a robust CRUD operation that allows the front-end team to pass and rely completely on the backend environment to provide each         question.
# Database using Local Cosmos Emulator 
 Application Forms
![image](https://github.com/lothbr/EmployeeApplicationService/assets/39369616/410d8c1e-820a-45e0-83a0-e0fa339980cf)

# Dependency Injection 
1. All services and Interfaces were injected to the controllers.
2. All Models and DTOs were properly managed throughout the application


Happy Viewing 🎉 🎉 

