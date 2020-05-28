using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DatingAppWebApi.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response,string message)
        {
            response.Headers.Add("Application-Error",message);
            response.Headers.Add("Access-Control-Expose-Headers","Application-Error");
            response.Headers.Add("Access-Allow-Control-Origin","*");
        }
        public static void AddPaginationHeader(this HttpResponse response, int currentPage,int itemsPerPage, int totalPage, int totalItems)
        {
            var paginationHeader= new PaginationHeader(currentPage,itemsPerPage,totalItems,totalPage);
            var camelCaseFormatter= new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver= new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("paginationHeader", JsonConvert.SerializeObject(paginationHeader,camelCaseFormatter));
            response.Headers.Add("Access-Control-Expose-Headers","paginationHeader");
        }
        public static int CalculateAge(this DateTime datetime)
        {
            int age= DateTime.Now.Year - datetime.Year;
            if(datetime.AddYears(age) > DateTime.Now)
            {
               age--;
            }
            return age;
        }
    }
}