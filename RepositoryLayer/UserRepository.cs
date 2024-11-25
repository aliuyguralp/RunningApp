using Microsoft.EntityFrameworkCore;
using RunningApplicationNew.DataLayer;
using RunningApplicationNew.Entity;

namespace RunningApplicationNew.IRepository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}