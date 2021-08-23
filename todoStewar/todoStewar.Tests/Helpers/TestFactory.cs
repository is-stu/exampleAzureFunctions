using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.IO;
using todoStewar.Common.Models;
using todoStewar.Function.Entities;

namespace todoStewar.Tests.Helpers
{
    public class TestFactory
    {
        public static TodoEntity GetTodoEntity()
        {
            return new TodoEntity
            {
                ETag = "*",
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                createdTime = DateTime.UtcNow,
                isCompleted = false,
                taskDescription = "Kill the humans"
            };
        }


        public static DefaultHttpRequest CreateHttpRequest(Guid todoId, Common.Models.Todo todoRequest)
        {
            string request = JsonConvert.SerializeObject(todoRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromSting(request),
                Path = $"{todoId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid todoId)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"{todoId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Common.Models.Todo todoRequest)
        {
            string request = JsonConvert.SerializeObject(todoRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromSting(request),
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static Todo GetTodoRequest()
        {
            return new Todo
            {
                createdTime = DateTime.UtcNow,
                isCompleted = false,
                taskDescription = "try to whatever"
            };
        }

        public static Stream GenerateStreamFromSting(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writter = new StreamWriter(stream);
            writter.Write(stringToConvert);
            writter.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null) {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else {
                logger = NullLoggerFactory.Instance.CreateLogger("Null loger");
            }

            return logger;
             
        }


    }
}
