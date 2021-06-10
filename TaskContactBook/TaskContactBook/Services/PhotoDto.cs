using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskContactBook.Services
{
    public class PhotoDto
    {
        public IFormFile PhotoFile{ get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
