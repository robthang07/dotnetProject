using System;

namespace lazyape.Models
{
    public class Setting
    {
        public int Id { get; set; } 
        
        public bool DarkMode { get; set; }
        
        //This is default for whole week. When the user is available.
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; } 
        
        //The time interval calender view will show normally 
        public DateTime VisibleTimeTo { get; set; }
        public DateTime VisibleTimeFrom { get; set; }
        //User connection 
        //Foreign key
        public string UserId { get; set; } 
        //Navigation property
        public ApplicationUser User { get; set; }
    }
}