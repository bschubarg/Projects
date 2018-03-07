using System;
using System.Collections.Generic;

namespace LottoWebServiceAPI.Responses
{
    public interface IListModelResponse<TModel> : IResponse
    {
        Int32 StateId { get; set; }

        Int32 GameId { get; set; }

        DateTime LottoDate { get; set; }

        IEnumerable<TModel> Model { get; set; }
    }    
}
