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
        // ----------------------------------------------------------------------------------------
        [FunctionName(nameof(UpdateTodo))]
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/update/{id}")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Update for todo: {id}, received");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);

            // Validate the ID
            TableOperation findOperation = TableOperation.Retrieve<TodoEntity>("TODO", id);
            TableResult findResult = await todoTable.ExecuteAsync(findOperation);

            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    isSuccess = false,
                    message = "Todo doesn't exist"
                });
            }

            //Update todo

            TodoEntity todoEntity = (TodoEntity)findResult.Result;
            todoEntity.isCompleted = todo.isCompleted;
            if (!string.IsNullOrEmpty(todo.taskDescription))
            {
                todoEntity.taskDescription = todo.taskDescription;
            }

            if (string.IsNullOrEmpty(todo?.taskDescription))
            {
                return new BadRequestObjectResult(new Response
                {
                    isSuccess = false,
                    message = "The request is missing the task description"
                });
            }

            TableOperation updateOperation = TableOperation.Replace(todoEntity);
            await todoTable.ExecuteAsync(updateOperation);

            string message = $"Todo: {id}, updated in the table";

            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                isSuccess = true,
                message = message,
                result = todoEntity
            }); ;
        }
        // ----------------------------------------------------------------------------------------
        [FunctionName(nameof(GetTodos))]
        public static async Task<IActionResult> GetTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todos")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("Get all todos saved from the table");

            TableQuery<TodoEntity> query = new TableQuery<TodoEntity>();
            TableQuerySegment<TodoEntity> todos = await todoTable.ExecuteQuerySegmentedAsync(query, null);


            string message = "Retrieving all the todos";

            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                isSuccess = true,
                message = message,
                result = todos
            }); ;
        }
        // ----------------------------------------------------------------------------------------
        [FunctionName(nameof(GetTodoById))]
        public static IActionResult GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req,
            [Table("todo", "TODO", "{id}")] TodoEntity todoEntity,
            string id,
            ILogger log)
        {
            log.LogInformation($"Get the todo with id: {id}");

            if (todoEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    isSuccess = false,
                    message = "Todo doesn't exist"
                });
            }

            string message = $"todo  {id} retrieved";

            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                isSuccess = true,
                message = message,
                result = todoEntity
            }); ;
        }
        // ----------------------------------------------------------------------------------------
        [FunctionName(nameof(DeleteTodo))]
        public static async Task<IActionResult> DeleteTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/delete/{id}")] HttpRequest req,
            [Table("todo", "TODO", "{id}")] TodoEntity todoEntity,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Delete todo with id: {id}");

            if (todoEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    isSuccess = false,
                    message = "Todo doesn't exist"
                });
            }

            await todoTable.ExecuteAsync(TableOperation.Delete(todoEntity));
            string message = $"todo  {id} was deleted";

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
