using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
            
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;        // if it is 0, then nothing was saved (bool false). if it is greater than 0, then AT LEAST SOMETHING was saved to the database (bool true)
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;      // it tells our EF tracker that smth has changed with our passed user entity (we don't save anything at this point ! ! !)
        }
    }
}