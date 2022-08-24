using coteo.Domain.Entities;
using coteo.Domain.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace coteo.Domain.Repositories.EntityFramework
{
    public class EFUsersRepository : IUsersRepository
    {
        private readonly AppDbContext _context;

        public EFUsersRepository(AppDbContext context)
        {
            _context = context;
        }

        public void DeleteUser(string id)
        {
            _context.Users.Remove(new User() { Id = id });
        }

        public User? GetUserById(string id)
        {
            return _context.Users
                .Include(u => u.MyOrders)
                .Include(u => u.IssuedToMeOrders)
                .ToList()
                .FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<User> GetUsers()
        {
            return _context.Users;
        }

        public void SaveUser(User entity)
        {
            if (entity.Id == default)
                _context.Entry(entity).State = EntityState.Added;
            else
                _context.Entry(entity).State = EntityState.Modified;
        }
    }
}