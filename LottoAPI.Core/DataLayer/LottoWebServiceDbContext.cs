using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LottoAPI.Core.DataLayer
{
    public class LottoWebServiceDbContext : DbContext
    {
        public string ConnectionString { get; }
        public IEntityMapper EntityMapper { get; }

        public LottoWebServiceDbContext(IOptions<AppSettings> appSettings, IEntityMapper entityMapper)
        {
            ConnectionString = appSettings.Value.ConnectionString;

            EntityMapper = entityMapper;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);

            base.OnConfiguring(optionsBuilder);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EntityMapper.MapEntities(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
    }
}
