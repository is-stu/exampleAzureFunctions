using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace todoStewar.Function.Entities
{
    public class TodoEntity : TableEntity
    {
        public DateTime createdTime { get; set; }

        public string taskDescription { get; set; }

        public bool isCompleted { get; set; }
    }
}
