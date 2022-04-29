using DivarDataAccess.Database;
using DivarDataAccess.Database.Domain;
using Microsoft.EntityFrameworkCore;

namespace DivarDataAccess.Repositories
{
    public class RequestRepository : IDisposable
    {
        private readonly DivarDbContext db;

        public RequestRepository()
        {
            db = new DivarDbContext();
        }
        public void Dispose()
        {
            db.Dispose();
        }
        


        public async Task Insert(Request model)
        {
            db.Entry(model).State = EntityState.Added;
            await db.SaveChangesAsync();
        }
        public async Task Update(Request selected)
        {
            db.Update(selected);
            await db.SaveChangesAsync();
        }
        public async Task<List<Request>> GetActiveList()
        {
            return await db.Requests.Where(w => w.IsActive).ToListAsync();
        }
        public async Task UpdateMany(List<Request> requests)
        {
            db.UpdateRange(requests);
            await db.SaveChangesAsync();
        }
    }
}
