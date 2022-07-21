using coteo.Domain.Entities;
using coteo.Domain.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace coteo.Domain.Repositories.EntityFramework
{
    public class EFDepartmentsRepository : IDepartmentsRepository
    {
        private readonly AppDbContext _context;

        public EFDepartmentsRepository(AppDbContext context)
        {
            _context = context;
        }

        public void DeleteDepartment(string id)
        {
            _context.Departments.Remove(new Department() { Id = id });
        }

        public Department? GetDepartmentById(string id)
        {
            return _context.Departments.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<Department> GetDepartments()
        {
            return _context.Departments;
        }

        public void SaveDepartment(Department entity)
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