using coteo.Areas.Identity.Data;

namespace coteo.Domain.Repositories.Abstract
{
    public interface IUsersRepository
    {
        IQueryable<ApplicationUser> GetUsers();
        ApplicationUser? GetUserById(string id);
        void SaveUser(ApplicationUser entity);
        void DeleteUser(string id);
    }
}