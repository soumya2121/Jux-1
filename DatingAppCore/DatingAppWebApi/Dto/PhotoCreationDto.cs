using System;
using Microsoft.AspNetCore.Http;

namespace DatingAppWebApi.Dto
{
    public class PhotoCreationDto
    {
        public PhotoCreationDto()
        {
            this.DateAdded= DateTime.Now;
        }
       public int Id {get;set;}
       public string Url { get; set; }
       public string Description { get; set; }
       public DateTime DateAdded { get; set; }
       public IFormFile File{get;set;}
       public string  PublicId { get; set; }
    }
}