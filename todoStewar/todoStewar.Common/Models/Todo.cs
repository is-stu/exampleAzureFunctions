using System;

namespace todoStewar.Common.Models
{
    public class Todo
    {
        public DateTime createdTime { get; set; }

        public string taskDescription { get; set; }

        public bool isCompleted { get; set; }
    }
}
