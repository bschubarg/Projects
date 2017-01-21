using LottoWebService.Core.EntityLayer;
using LottoWebService.ViewModels;

namespace LottoWebService.Extensions
{
    public static class LottoNumberViewModelMapper
    {
        public static LottoNumberViewModel ToViewModel(this LottoNumberEntity entity)
        {
            return entity == null ? null : new LottoNumberViewModel
            {
                ProductID = entity.ProductID,
                ProductName = entity.Name,
                ProductNumber = entity.ProductNumber
            };
        }

        public static LottoNumber ToEntity(this LottoNumberViewModel viewModel)
        {
            return viewModel == null ? null : new Product
            {
                Name = viewModel.ProductName,
                ProductNumber = viewModel.ProductNumber
            };
        }
    }
}