using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using todoStewar.Common.Models;
using todoStewar.Common.Responses;
using todoStewar.Function.Entities;

namespace todoStewar.Function.Functions
{
    public static class TodoAPI
    {
        [FunctionName(nameof(CreateTodo))]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo/add")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("Received a new Todo");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);

            if (string.IsNullOrEmpty(todo?.taskDescription))
            {
                return new BadRequestObjectResult(new Response
                {
                    isSuccess = false,
                    message = "The request is missing the task description"
                });
            }

            TodoEntity todoEntity = new TodoEntity
            {
                createdTime = DateTime.UtcNow,
                ETag = "*",
                isCompleted = false,
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                taskDescription = todo.taskDescription
            };

            TableOperation addOperation = TableOperation.Insert(todoEntity);
            await todoTable.ExecuteAsync(addOperation);

            string message = "New Todo added in the table";

            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                isSuccess = true,
                message = message,
                result = todoEntity
            }); ;
        }
    }
}
