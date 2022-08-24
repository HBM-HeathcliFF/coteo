using coteo.Domain.Entities;

namespace coteo.Domain.Repositories.Abstract
{
    public interface IUsersRepository
    {
        IQueryable<User> GetUsers();
        User? GetUserById(string id);
        void SaveUser(User entity);
        void DeleteUser(string id);
    }
}