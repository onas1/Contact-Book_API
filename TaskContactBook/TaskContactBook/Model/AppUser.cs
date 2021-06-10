using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskContactBook
{
    public class AppUser: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string  Gender { get; set; }
        public string  Image { get; set; }
        public string Street { get; set; }
        public string FacebookLink { get; set; }
        public string LinkedinLink { get; set; }
        public string TwitterLink { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        

    }
}
