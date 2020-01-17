using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace DattingApp.API.Controllers
{
    //We need view support so we inherit controller and route our angular app.
    public class Fallback : Controller
    {
        public IActionResult Index()
        {
            return PhysicalFile(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html")
                , "text/html");
        }
    }
}