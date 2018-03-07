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
using LottoAPI.Core.DataLayer;
using LottoWebService.Extensions;
using LottoWebServiceAPI.Responses;
using LottoWebService.ViewModels;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;

namespace LottoWebService.Controllers
{    
    public class LottoValuesController : ApiController
    {
        private readonly ILottoWebServiceRepository _lottoWebServiceRepository;
       
        public LottoValuesController(ILottoWebServiceRepository repository)
        {
            _lottoWebServiceRepository = repository;
        }

        protected override void Dispose(bool disposing)
        {
            _lottoWebServiceRepository?.Dispose();

            base.Dispose(disposing);
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public async Task<IActionResult> GetLottoGames()
        {
            var response = new ListModelResponse<LottoGameEntityViewModel>() as IListModelResponse<LottoGameEntityViewModel>;

            try
            {
                response.Model = await Task.Run(() =>
                {
                    return _lottoWebServiceRepository
                        .LottoGames.Select(item => item.ToViewModel())
                        .ToList();
                });

                response.Message = $"Total of records: {response.Model.Count()}";
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
            }

            return response.ToHttpResponse();
        }
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public async Task<IActionResult> GetLottoStates()
        {
            var response = new ListModelResponse<LottoEntityViewModel>() as IListModelResponse<LottoEntityViewModel>;

            try
            {
                response.Model = await Task.Run(() =>
                {
                    return _lottoWebServiceRepository
                        .LottoStates.Select(item => item.ToViewModel())
                        .ToList();
                });

                response.Message = $"Total of records: {response.Model.Count()}";
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
            }

            return response.ToHttpResponse();
        }



        [Microsoft.AspNetCore.Mvc.HttpGet]
        public async Task<IActionResult> GetLottoNumbersByStateAbbrev(string stateAbbrev)
        {         
            // get the stateId for the associated stateAbbrev
            if (!string.IsNullOrEmpty(stateAbbrev))
                return await GetLottoNumbers(_lottoWebServiceRepository.GetStateIdByName(stateAbbrev).StateID);

            return await GetLottoNumbers();
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public async Task<IActionResult> GetLottoNumbers(int? stateId = null, int? gameId = null, DateTime? lottoDate = null)
        {            
            var response = new ListModelResponse<LottoNumberViewModel>() as IListModelResponse<LottoNumberViewModel>;
            
            try
            {
                if (stateId != null) response.StateId = stateId.Value;
                if (gameId != null) response.GameId = gameId.Value;
                if (lottoDate != null) response.LottoDate = lottoDate.Value;

                response.Model = await Task.Run(() =>
                {
                    return _lottoWebServiceRepository
                        .GetLottoNumbers(response.StateId, response.GameId, response.LottoDate)
                        .Select(item => item.ToViewModel())
                        .ToList();
                });

                response.Message = $"Total of records: {response.Model.Count()}";
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
            }

            return response.ToHttpResponse();
        }                      
    }
}