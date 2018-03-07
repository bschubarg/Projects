using System;
using System.Collections.Generic;
using LottoAPI.Core.EntityLayer;

namespace LottoAPI.Core.DataLayer
{
    public interface ILottoWebServiceRepository : IDisposable
    {
        #region Properties
        // Properties
        IEnumerable<LottoStateEntity> LottoStates { get; }
        IEnumerable<LottoGameEntity> LottoGames { get; }
        #endregion

        #region Methods
        // Methods
        IEnumerable<LottoNumberEntity> GetLottoNumbers(int? stateId, int? gameId, DateTime? lottoDate);

        LottoStateEntity GetStateIdByName(string stateAbbrev);

        LottoNumberEntity GetLottoNumber(int? id);        

        LottoNumberEntity AddLottoNumber(LottoNumberEntity entity);

        LottoNumberEntity UpdateLottoNumber(int? id, LottoNumberEntity changes);

        LottoNumberEntity DeleteLottoNumber(int? id);
        #endregion
    }
}
