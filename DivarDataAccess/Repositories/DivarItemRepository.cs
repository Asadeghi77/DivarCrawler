using DivarDataAccess.Database;
using DivarDataAccess.Database.Domain;
using Microsoft.EntityFrameworkCore;

namespace DivarDataAccess.Repositories
{
    public class DivarItemRepository : IDisposable
    {
        private readonly DivarDbContext db;

        public DivarItemRepository()
        {
            db = new DivarDbContext();
        }
        public void Dispose()
        {
            db.Dispose();
        }


        public async Task Insert(DivarItem model)
        {
            db.Entry(model).State = EntityState.Added;
            await db.SaveChangesAsync();
        }
        public async Task InsertMany(List<DivarItem> models)
        {
            await db.AddRangeAsync(models);
            await db.SaveChangesAsync();
        }
        public async Task<List<DivarItem>> GetNotSend()
        {
            return await db.DivarItems.Where(w => !w.IsSendToTelegram)
                .Include(c => c.Request)
                .ToListAsync();
        }
        public async Task ChangeToSentStatus(List<DivarItem> divarItems)
        {
            divarItems.ForEach(f => f.IsSendToTelegram = true);
            db.UpdateRange(divarItems);
            await db.SaveChangesAsync();
        }
        public async Task<List<string>> GetExistItems(List<DivarItem> items)
        {
            var tokenList = items.Select(s => s.Token);
            var data = await db.DivarItems
                .Where(w => tokenList.Contains(w.Token))
                .ToListAsync();

            var result = data.Where(w => items.Any(ww => ww.Token == w.Token && ww.RequestId == w.RequestId))
             .Select(s => s.Token)
             .ToList();

            return result;
        }
    }
}
