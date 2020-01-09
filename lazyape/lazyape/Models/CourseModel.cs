using Microsoft.AspNetCore.Identity;

namespace lazyape.Models
{
    public class Course
    {
        //ID of object
        public int Id { get; set; }
        //Name of course
        public string Code { get; set; }
        
        //Foreign key
        public string UserId { get; set; } 
        //Navigation property
        public ApplicationUser User { get; set; }
        
        //Hours done of the course
        public float HoursDone { get; set; }
        
        //Course Active
        public bool Active { get; set; }
    }
}