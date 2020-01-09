using System;

namespace lazyape.Models
{
    public class Task
    {
        public enum TaskType
        {
            NORMAL,
            AUTO
        }
        
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Title { get; set; }
        
        //Foreign key
        public string UserId { get; set; } 
        //Navigation property
        public ApplicationUser User { get; set; }
        
        //Auto Generator var
        public TaskType Type { get; set; }
    }
}