using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace lazyape.Models
{
    public class Token
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        
        public string UserId { get; set; }
        
        public ApplicationUser User { get; set; }
        
    }
    
}