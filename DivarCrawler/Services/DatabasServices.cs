using DivarCrawler.Database;
using DivarCrawler.Database.Domain;
using Microsoft.EntityFrameworkCore;

namespace DivarCrawler.Services
{
    public class DatabasServices : IDisposable
    {
        private readonly DivarDbContext db;

        public DatabasServices()
        {
            db = new DivarDbContext();
        }
        public void Dispose()
        {
            db.Dispose();
        }

        public async Task InsertGeneral(IBaseEntity model)
        {
            db.Entry(model).State = EntityState.Added;
            await db.SaveChangesAsync();
        }

        public async Task<List<DivarItem>> GetNotSendDivarItems()
        {
            var q = await db.DivarItems.Where(w => !w.IsSendToTelegram).ToListAsync();
            return q;
        }

        public async Task ChangeToSentStatusDivarItems(List<DivarItem> divarItems)
        {
            divarItems.ForEach(f => f.IsSendToTelegram = true);
            db.UpdateRange(divarItems);
            await db.SaveChangesAsync();
        }

        public async Task InsertManyDivarItem(List<DivarItem> models)
        {
            await db.AddRangeAsync(models);
            await db.SaveChangesAsync();
        }

        public async Task<List<string>> GetExistDivarItems(List<string> tokenLists)
        {
            return await db.DivarItems
                .Where(w => tokenLists.Contains(w.Token))
                .Select(s => s.Token)
                .ToListAsync();
        }

        public async Task<List<Request>> GetActiveRequestList()
        {
            return await db.Requests.Where(w => w.IsActive).ToListAsync();
        }

        public async Task EditDivarItem(Request selected)
        {
            db.Update(selected);
            await db.SaveChangesAsync();
        }

        public async Task UpdateRequests(List<Request> requests)
        {
            db.UpdateRange(requests);
            await db.SaveChangesAsync();
        }
    }
}
