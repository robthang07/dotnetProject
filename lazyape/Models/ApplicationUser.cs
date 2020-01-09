using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using lazyape.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace lazyape.Models
{
    public class ApplicationUser : IdentityUser
    {        
        
        //Usage of this class belongs to Setting.cs. Navigation property in db relation.
    }
} 