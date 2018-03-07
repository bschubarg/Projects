using System;
using System.Collections.Generic;
using System.Linq;
using LottoAPI.Core.EntityLayer;

namespace LottoAPI.Core.DataLayer
{
    public class LottoWebServiceRepository : ILottoWebServiceRepository
    {
        private readonly LottoWebServiceDbContext DbContext;
        private Boolean Disposed;

        public LottoWebServiceRepository(LottoWebServiceDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public void Dispose()
        {
            if (Disposed) return;
            if (DbContext == null) return;

            DbContext.Dispose();

            Disposed = true;
        }

        public IEnumerable<LottoNumberEntity> GetLottoNumbers(int? stateId, int? gameId, DateTime? dtLottoDate)
        {
            var query = DbContext.Set<LottoNumberEntity>().AsQueryable();
            
            if (stateId != 0)
                query = query.Where(item => item.StateID.Equals(stateId));
            if (gameId != 0)
                query = query.Where(item => item.GameID.Equals(gameId));
            if (dtLottoDate != null && dtLottoDate.Value.Ticks != 0)
                query = query.Where(item => item.LottoDate.Value.Date.Equals(dtLottoDate.Value.Date));

            return query;
        }

        public LottoNumberEntity GetLottoNumber(int? id) => DbContext.Set<LottoNumberEntity>().FirstOrDefault(item => item.HashCode == id);

        public IEnumerable<LottoStateEntity> LottoStates => DbContext.Set<LottoStateEntity>().AsQueryable();
        public IEnumerable<LottoGameEntity> LottoGames => DbContext.Set<LottoGameEntity>().AsQueryable();

        public LottoStateEntity GetStateIdByName(string stateAbbrev) => DbContext.Set<LottoStateEntity>().FirstOrDefault(item => item.StateProvinceCode == stateAbbrev);

        public LottoNumberEntity AddLottoNumber(LottoNumberEntity entity)
        {
            entity.Number = 0;
            entity.StateID =  1;
            entity.LottoDate = DateTime.Now;
            entity.GameID = 7;
            
            DbContext.Set<LottoNumberEntity>().Add(entity);

            DbContext.SaveChanges();

            return entity;
        }

        public LottoNumberEntity UpdateLottoNumber(int? id, LottoNumberEntity changes)
        {
            var entity = GetLottoNumber(id);

            if (entity != null)
            {
                entity.Number = changes.Number;
                entity.StateID = changes.StateID;
                entity.GameID = changes.GameID;

                DbContext.SaveChanges();
            }

            return entity;
        }

        public LottoNumberEntity DeleteLottoNumber(int? id)
        {
            var entity = GetLottoNumber(id);

            if (entity != null)
            {
                DbContext.Set<LottoNumberEntity>().Remove(entity);

                DbContext.SaveChanges();
            }

            return entity;
        }
    }
}
