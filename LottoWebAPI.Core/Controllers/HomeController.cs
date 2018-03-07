// Lotto Numbers Service and Application.
// Open source.  This is a project to demonstrate 
// the various technologies to use when gathering data
// and publishing data by various means.  I created
// this project only for my personal use.  Any alterations
// by others is welcomed.
// 
// I do not pretend to be an expert on these technologies
// but rather a demonstration of my approach to satisfy
// certain requirements.
// 
// Acknowledgments: https://github.com/rubicon-oss/LicenseHeaderManager/wiki - License Header Snippet
//					https://www.codeproject.com/Articles/1041115/Webscraping-with-Csharp - ScrapySharp
// 
// Copyright (c) 2016 William Schubarg

using Microsoft.AspNetCore.Mvc;

namespace LottoWebService.Controllers
{
    [Route("api/[controller]/[action]")]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View("index");
        }

        [HttpGet]        
        public ActionResult Help()
        {
            return View();
        }
    }
}
