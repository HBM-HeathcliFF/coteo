using coteo.Domain.Entities;

namespace coteo.Domain.Repositories.Abstract
{
    public interface IOrganizationsRepository
    {
        IQueryable<Organization> GetOrganizations();
        Organization GetOrganizationById(string id);
        void SaveOrganization(Organization entity);
        void DeleteOrganization(string id);
    }
}