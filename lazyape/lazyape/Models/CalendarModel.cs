using System.Collections.Generic;

namespace lazyape.Models
{
    public class CalendarModel
    {
        private List<Task> Tasks { get; set; }

        public CalendarModel()
        {
            Tasks = new List<Task>();
        }

        public void AddTask(Task task)
        {
            Tasks.Add(task);
        }

        public List<Task> GetTask()
        {
            return Tasks;
        }
    }
}