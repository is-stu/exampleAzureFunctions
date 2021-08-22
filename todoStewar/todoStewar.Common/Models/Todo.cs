using System;

namespace todoStewar.Common.Models
{
    public class Todo
    {
        public DateTime reatedTime { get; set; }

        public string taskDescription { get; set; }

        public bool isCompleted { get; set; }
    }
}
