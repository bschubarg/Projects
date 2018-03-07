using System.Collections.Generic;

namespace LottoAPI.Core.DataLayer
{
    public class LottoWebServiceEntityMapper : EntityMapper
    {
        public LottoWebServiceEntityMapper()
        {
            Mappings = new List<IEntityMap>()
            {
                new LottoNumberEntityMap(),
                new LottoStateEntityMap(),
                new LottoStateGameEntityMap(),
                new LottoGameEntityMap()
            };
        }
    }
}
