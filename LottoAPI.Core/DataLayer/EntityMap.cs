using Microsoft.EntityFrameworkCore;

namespace LottoAPI.Core.DataLayer
{
    public class EntityMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            //var entity = modelBuilder.Entity<Product>();

            //entity.ToTable("Product", "Production");

            //entity.HasKey(p => new { p.ProductID });

            //entity.Property(p => p.ProductID).UseSqlServerIdentityColumn();
        }
    }
}
