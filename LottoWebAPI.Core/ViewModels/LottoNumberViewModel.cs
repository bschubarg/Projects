using System;
using LottoCommon;

namespace LottoWebService.ViewModels
{
    [Serializable]
    public class LottoEntityViewModel : LottoUrlStateName
    {
    }
    [Serializable]
    public class LottoGameEntityViewModel : LottoGame
    {
    }   
    [Serializable]
    public class LottoNumberViewModel : LottoNumber
    {
    }
    [Serializable]
    public class LottoNumbersListViewModel : LottoNumbers
    {
        public LottoNumbersListViewModel(LottoNumber entity) : base(entity)
        {
        }
    }
    [Serializable]
    public class LottoEntityListViewModel : LottoEntities
    {
        public LottoEntityListViewModel(LottoEntity entity) : base(entity)
        {
        }
    }
}