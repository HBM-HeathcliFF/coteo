using coteo.Domain.Repositories.Abstract;

namespace coteo.Domain
{
    public class DataManager
    {
        private readonly AppDbContext _context;

        public IUsersRepository Users { get; set; }
        public IOrganizationsRepository Organizations { get; set; }
        public IDepartmentsRepository Departments { get; set; }
        public IOrdersRepository Orders { get; set; }

        public DataManager(
            IUsersRepository users,
            IOrganizationsRepository organizations,
            IDepartmentsRepository departments,
            IOrdersRepository orders,
            AppDbContext context)
        {
            Users = users;
            Organizations = organizations;
            Departments = departments;
            Orders = orders;
            _context = context;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}