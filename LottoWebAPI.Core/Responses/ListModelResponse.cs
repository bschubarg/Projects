using System;
using System.Collections.Generic;

namespace LottoWebServiceAPI.Responses
{
    public class ListModelResponse<TModel> : IListModelResponse<TModel>
    {
        public String Message { get; set; }

        public Boolean DidError { get; set; }

        public String ErrorMessage { get; set; }

        public Int32 StateId { get; set; }        

        public Int32 GameId { get; set; }

        public DateTime LottoDate { get; set; }

        public IEnumerable<TModel> Model { get; set; }
    }    
}
