using System;
using LottoCommon;

namespace LottoWebService.ViewModels
{
    [Serializable]
    public class LottoEntityViewModel : LottoEntity
    {
    }
    [Serializable]
    public class LottoNumberViewModel : LottoNumber
    {
    }
    [Serializable]
    public class LottoNumbersListViewModel : LottoNumbers
    {
        public LottoNumbersListViewModel(LottoEntity entity) : base(entity)
        {
        }
    }
}