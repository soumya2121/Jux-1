using System;

namespace DatingAppWebApi.Dto
{
    public class PhotoDetailsDto
    {
       public int Id {get;set;}
       public string Url { get; set; }
       public string Description { get; set; }
       public DateTime DateAdded { get; set; }
       public bool IsMain{get;set;}
    }
}