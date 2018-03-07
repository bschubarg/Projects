using LottoCommon;
using Microsoft.EntityFrameworkCore;
using LottoAPI.Core.EntityLayer;


namespace LottoAPI.Core.DataLayer
{
    public class LottoNumberEntityMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<LottoNumberEntity>();

            entity.ToTable("LottoNumber", "dbo");

            entity.HasKey(p => new { Id = p.HashCode });

            entity.Property(p => p.HashCode).UseSqlServerIdentityColumn();
        }
    }

    public class LottoStateEntityMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<LottoStateEntity>();

            entity.ToTable("LottoUrlStateName", "dbo");

            entity.HasKey(p => new {Id = p.StateID});

            entity.Property(p => p.StateID).UseSqlServerIdentityColumn();
        }

    }
    public class LottoStateGameEntityMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<LottoStateGameEntity>();

            entity.ToTable("LottoStateGame", "dbo");

            entity.HasKey(p => new { Id = p.ID });

            entity.Property(p => p.ID).UseSqlServerIdentityColumn();
        }
    }
    public class LottoGameEntityMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<LottoGameEntity>();

            entity.ToTable("LottoGame", "dbo");

            entity.HasKey(p => new { Id = p.GameID });

            entity.Property(p => p.GameID).UseSqlServerIdentityColumn();
        }
    }
}
