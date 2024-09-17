using adminpanel.Data;
using Microsoft.AspNetCore.Identity;

namespace adminpanel.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime LastLogin { get; set; }
        public DateTime Registration { get; set; }
        public SD.Status Status { get; set; }
    }
}
