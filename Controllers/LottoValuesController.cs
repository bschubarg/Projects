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
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Http;

using LottoWebService.Core.DataLayer;
using LottoWebService.Extensions;
using LottoWebServiceAPI.Responses;
using LottoWebService.ViewModels;

using Microsoft.AspNetCore.Mvc;
// Alias the Microsoft.AspNetCore.Mvc to stop the compile error
// of ambiguous reference between System.Web.Http.GetHtmlAttribute
using Mvc = Microsoft.AspNetCore.Mvc;

namespace LottoWebService.Controllers
{    
    public class LottoValuesController : ApiController
    {
        private ILottoWebServiceRepository LottoWebServiceRepository;

        public LottoValuesController(ILottoWebServiceRepository repository)
        {
            LottoWebServiceRepository = repository;
        }

        protected override void Dispose(Boolean disposing)
        {
            if (LottoWebServiceRepository != null)
            {
                LottoWebServiceRepository.Dispose();
            }

            base.Dispose(disposing);
        }

        // GET Production/Product
        [Mvc.HttpGet]
        [Route("api/GetLottoNumbers")]
        public async Task<IActionResult> GetLottoNumbers(Int32? pageSize = 10, Int32? pageNumber = 1, String name = null)
        {            
            var response = new ListModelResponse<LottoNumberViewModel>() as IListModelResponse<LottoNumberViewModel>;

            try
            {
                response.PageSize = (Int32)pageSize;
                response.PageNumber = (Int32)pageNumber;

                response.Model = await Task.Run(() =>
                {
                    return LottoWebServiceRepository
                        .GetLottoNumbers(response.PageSize, response.PageNumber, name)
                        .Select(item => item.ToViewModel())
                        .ToList();
                });

                response.Message = String.Format("Total of records: {0}", response.Model.Count());
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
            }

            return response.ToHttpResponse();
        }

        [Route("api/LottoValues")]
        // GET api/values
        public IEnumerable<string> Get()
        {            
            return new string[] { "value1", "value2" };
        }
        // 
        public IEnumerable<string> All()
        {
            return new string[] { "value3", "value4" };
        }
        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([Mvc.FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [Mvc.FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}