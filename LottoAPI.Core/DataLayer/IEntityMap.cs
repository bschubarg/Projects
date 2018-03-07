using Microsoft.EntityFrameworkCore;

namespace LottoAPI.Core.DataLayer
{
    public interface IEntityMap
    {
        void Map(ModelBuilder modelBuilder);
    }
}
