using coteo.Areas.Identity.Data;
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
            _context.Users.Remove(new ApplicationUser() { Id = id });
        }

        public string? GetShortNameById(string id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                string[] words = user.FullName.Split(' ');
                if (words.Length < 2 || words.Length > 3)
                {
                    return null;
                }
                string shortName = words[0];
                for (int i = 1; i < words.Length; i++)
                {
                    shortName += $" {words[i][0]}.";
                }
                return shortName;
            }
            else
            {
                return null;
            }
            
        }

        public ApplicationUser? GetUserById(string id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<ApplicationUser> GetUsers()
        {
            return _context.Users;
        }

        public void SaveUser(ApplicationUser entity)
        {
            if (entity.Id == default)
                _context.Entry(entity).State = EntityState.Added;
            else
                _context.Entry(entity).State = EntityState.Modified;
        }
    }
}