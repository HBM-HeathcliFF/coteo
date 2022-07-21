using coteo.Domain.Entities;

namespace coteo.Domain.Repositories.Abstract
{
    public interface IDepartmentsRepository
    {
        IQueryable<Department> GetDepartments();
        Department? GetDepartmentById(string id);
        void SaveDepartment(Department entity);
        void DeleteDepartment(string id);
    }
}