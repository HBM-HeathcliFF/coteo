using coteo.Domain.Entities;
using coteo.Domain.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace coteo.Domain.Repositories.EntityFramework
{
    public class EFOrganizationsRepository : IOrganizationsRepository
    {
        private readonly AppDbContext _context;

        public EFOrganizationsRepository(AppDbContext context)
        {
            _context = context;
        }

        public void DeleteOrganization(string id)
        {
            _context.Organizations.Remove(new Organization() { Id = id });
        }

        public Organization? GetOrganizationById(string id)
        {
            return _context.Organizations
                .Include(u => u.Departments)
                .ToList()
                .FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<Organization> GetOrganizations()
        {
            return _context.Organizations;
        }

        public void SaveOrganization(Organization entity)
        {
            if (entity.Id == default)
            {
                entity.Id = Guid.NewGuid().ToString();
                _context.Entry(entity).State = EntityState.Added;
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
        }
    }
}