using LottoCommon;
using LottoWebService.ViewModels;

namespace LottoWebService.Extensions
{
    public static class LottoNumberViewModelMapper
    {
        public static LottoNumberViewModel ToViewModel(this LottoNumber entity) => entity == null ? null : new LottoNumberViewModel
        {
            StateID = entity.StateID,
            Number = entity.Number,
            GameID = entity.GameID,
            LottoDate = entity.LottoDate
        };

        public static LottoEntityViewModel ToViewModel(this LottoUrlStateName entity) => entity == null ? null : new LottoEntityViewModel
        {
            StateID = entity.StateID,
            Name = entity.Name,
            StateProvinceCode = entity.StateProvinceCode,
            Enable = entity.Enable
        };

        public static LottoGameEntityViewModel ToViewModel(this LottoGame entity) => entity == null ? null : new LottoGameEntityViewModel
        {
            GameID = entity.GameID,
            Game = entity.Game
        };

        public static LottoNumber ToEntity(this LottoNumberViewModel viewModel) => viewModel == null ? null : new LottoNumber
        {
            StateID = viewModel.StateID,
            Number = viewModel.Number,
            GameID = viewModel.GameID,
            LottoDate = viewModel.LottoDate            
        };

        public static LottoEntity ToEntity(this LottoEntityViewModel viewModel) => viewModel == null ? null : new LottoEntity
        {
            StateID = viewModel.StateID,
            State = viewModel.Name,
            LottoUrl = viewModel.StateProvinceCode            
        };
    }
}