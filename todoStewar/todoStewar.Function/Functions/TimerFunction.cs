using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;
using todoStewar.Function.Entities;

namespace todoStewar.Function.Functions
{
    public static class TimerFunction
    {
        [FunctionName("TimerFunction")]
        public static async Task Run([TimerTrigger("*/1 * * * *")] TimerInfo myTimer, [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable, ILogger log)
        {
            log.LogInformation($"Deleting completed task at: {DateTime.Now}");
            string filter = TableQuery.GenerateFilterConditionForBool("isCompleted", QueryComparisons.Equal, true);
            TableQuery<TodoEntity> query = new TableQuery<TodoEntity>().Where(filter);
            TableQuerySegment<TodoEntity> completedTodos = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            int deleted = 0;
            foreach (TodoEntity completedTodo in completedTodos)
            {
                await todoTable.ExecuteAsync(TableOperation.Delete(completedTodo));
                deleted++;
            }

            log.LogInformation($"Deleted {deleted} todos at: {DateTime.Now}");
        }
    }
}
