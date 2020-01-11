using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DattingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static int CalculateAge(this DateTime dt)
        {
            var age = DateTime.Today.Year - dt.Year;
            if (dt.AddYears(age) > DateTime.Today)
                age--;
            return age;
        }

        public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            PaginationHeader pHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            var camelCase = new JsonSerializerSettings();
            camelCase.ContractResolver = new CamelCasePropertyNamesContractResolver(); // Cuz Angular works with CamelCase
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(pHeader, camelCase));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}