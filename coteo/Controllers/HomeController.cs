using coteo.Areas.Identity.Data;
using coteo.Domain;
using coteo.Domain.Entities;
using coteo.Domain.Enum;
using coteo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace coteo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly DataManager _dataManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(DataManager dataManager, UserManager<ApplicationUser> userManager)
        {
            _dataManager = dataManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return GetView<int>(new List<UserRole> { UserRole.User }, "/Home/NewOrganization");
        }

        [HttpGet]
        public IActionResult MyOrders()
        {
            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            return GetView(new List<UserRole> { UserRole.User, UserRole.Employee }, "/Home/Index", user?.MyOrders);
        }

        [HttpGet]
        public IActionResult IssuedToMe()
        {
            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            return GetView(new List<UserRole> { UserRole.User, UserRole.Creator }, "/Home/Index", user?.IssuedToMeOrders);
        }

        [HttpGet]
        public IActionResult CreateOrder()
        {
            return GetView<int>(new List<UserRole> { UserRole.User, UserRole.Employee, UserRole.Creator }, "/Home/Index");
        }

        [HttpGet]
        public IActionResult CreateDepartment()
        {
            return GetView<int>(new List<UserRole> { UserRole.User, UserRole.Employee, UserRole.Leader }, "/Home/Index");
        }

        [HttpGet]
        public IActionResult ListOfDepartments()
        {
            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));
            var organization = _dataManager.Organizations.GetOrganizationById(user.OrganizationId);

            List<DepartmentInfoModel> departmentList = null;

            if (organization != null)
			{
                departmentList = new();
                foreach (var department in organization.Departments)
                {
                    departmentList.Add(new DepartmentInfoModel()
                    {
                        DepartmentName = department.Name,
                        LeaderFullName = _dataManager.Users.GetUserById(department.LeaderId).FullName,
                        EmployeeCount = _dataManager.Users.GetUsers().Where(x => x.DepartmentId == department.Id).Count()
                    });
                }
            }

            return GetView(new List<UserRole> { UserRole.User }, "/Home/NewOrganization", departmentList);
        }

        [HttpGet]
        public IActionResult AddEmployee()
        {
            return GetView<int>(new List<UserRole> { UserRole.User, UserRole.Employee, UserRole.Creator }, "/Home/Index");
        }

        [HttpGet]
        public IActionResult NewOrganization()
        {
            return GetView<int>(new List<UserRole> { UserRole.Employee, UserRole.Leader, UserRole.Creator },
                "/Home/Index");
        }

        [HttpGet]
        public IActionResult LeaveTheOrganization()
        {
            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));
            var organization = _dataManager.Organizations.GetOrganizationById(user.OrganizationId);

            if (organization == null)
            {
                return Redirect("/Home/NewOrganization");
            }
            
            user.OrganizationId = null;
            user.DepartmentId = null;
            user.Role = UserRole.User;

            _dataManager.Users.SaveUser(user);

            if (user.Role == UserRole.Creator)
            {
                var users = _dataManager.Users.GetUsers().Where(x => x.OrganizationId == organization.Id);
                foreach (var usr in users)
                {
                    usr.OrganizationId = null;
                    usr.Role = UserRole.User;
                    _dataManager.Users.SaveUser(usr);
                }
                _dataManager.Organizations.DeleteOrganization(organization.Id);
            }

            _dataManager.SaveChanges();

            return Redirect("/Home/NewOrganization");
        }

        [HttpPost]
        public IActionResult NewOrganization(Organization organization)
        {
            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            if (_dataManager.Organizations.GetOrganizations().FirstOrDefault(x => x.Link == organization.Link) != null)
            {
                user.Role = UserRole.Employee;
            }
            else
            {
                organization.Creator = user;
                _dataManager.Organizations.SaveOrganization(organization);
                _dataManager.SaveChanges();

                user.Role = UserRole.Creator;
            }

            user.OrganizationId = _dataManager.Organizations.GetOrganizations().First(x => x.Link == organization.Link).Id;
            _dataManager.Users.SaveUser(user);

            _dataManager.SaveChanges();

            return Redirect("/Home/Index");
        }

        [HttpPost]
        public IActionResult CreateDepartment(CreateDepartmentModel model)
        {
            string message;
            var user = _dataManager.Users.GetUsers().FirstOrDefault(x => x.FullName == model.LeaderFullName);
            var creator = _dataManager.Users.GetUsers().FirstOrDefault(x => x.Id == model.CreatorId);

            if (user != null &&
                creator != null &&
                user.OrganizationId != null &&
                creator.OrganizationId == user.OrganizationId)
            {
                if (_dataManager.Departments.GetDepartments().FirstOrDefault(x => x.LeaderId == user.Id) == null)
                {
                    Department department = new();
                    department.Name = model.DepartmentName;
                    department.Leader = user;
                    department.LeaderId = user.Id;
                    department.Organization = user.Organization;
                    department.OrganizationId = user.OrganizationId;
                    _dataManager.Departments.SaveDepartment(department);
                    _dataManager.SaveChanges();

                    Department dep = _dataManager.Departments.GetDepartments().First(x => x.LeaderId == user.Id);

                    user.Role = UserRole.Leader;
                    user.DepartmentId = dep.Id;

                    _dataManager.Users.SaveUser(user);
                    _dataManager.SaveChanges();

                    message = "Подразделение создано!";
                }
                else
                {
                    message = "Указанный сотрудник является руководителем другого подразделения!";
                }
            }
            else
            {
                message = "Сотрудников с такими ФИО не найдено!";
            }

            ViewBag.Message = message;
            return GetView<int>(new List<UserRole> { UserRole.User, UserRole.Employee, UserRole.Leader }, "/Home/Index");
        }

        [HttpPost]
        public IActionResult AddEmployee(AddEmployeeModel model)
        {
            string message;
            var user = _dataManager.Users.GetUsers().FirstOrDefault(x => x.FullName == model.EmployeeFullName);
            var leader = _dataManager.Users.GetUsers().FirstOrDefault(x => x.Id == model.LeaderId);

            if (user != null && leader != null && leader.OrganizationId == user.OrganizationId)
            {
                Department department = _dataManager.Departments.GetDepartments().First(x => x.LeaderId == leader.Id);

                if (user.DepartmentId == null)
                {
                    user.Role = UserRole.Employee;
                    user.DepartmentId = department.Id;

                    _dataManager.Users.SaveUser(user);
                    _dataManager.SaveChanges();

                    message = "Сотрудник добавлен в подразделение!";
                }
                else
                {
                    message = "Сотрудник уже состоит в другом подразделении!";
                }
            }
            else
            {
                message = "Сотрудников с такими ФИО не найдено!";
            }

            ViewBag.Message = message;
            return GetView<int>(new List<UserRole> { UserRole.User, UserRole.Employee, UserRole.Creator }, "/Home/Index");
        }

        [HttpPost]
        public IActionResult CreateOrder(CreateOrderModel model)
        {
            string message;
            var executor = _dataManager.Users.GetUsers().FirstOrDefault(x => x.FullName == model.ExecutorFullName);
            var createdById = _dataManager.Users.GetUsers().FirstOrDefault(x => x.Id == model.CreatedById);

            if (executor != null &&
                createdById != null &&
                createdById.OrganizationId == executor.OrganizationId &&
                executor.DepartmentId == createdById.DepartmentId)
            {
                Order order = new();
                order.Name = model.OrderName;
                order.Text = model.Text;
                order.CreationDate = DateTime.Now;
                order.Deadline = DateTime.Parse(model.Deadline);
                order.Status = OrderStatus.InWork;
                order.Creator = createdById;
                order.CreatedById = createdById.Id;
                order.Executor = executor;
                order.ExecutorId = executor.Id;

                _dataManager.Orders.SaveOrder(order);
                _dataManager.SaveChanges();

                message = "Поручение создано!";
            }
            else
            {
                message = "Сотрудников с такими ФИО не найдено!";
            }

            ViewBag.Message = message;
            return GetView<int>(new List<UserRole> { UserRole.User, UserRole.Employee, UserRole.Creator }, "/Home/Index");
        }
        
        [HttpPost]
        public IActionResult MyOrders(OrderStatusModel model)
        {
            var order = _dataManager.Orders.GetOrderById(model.Id);
            order.SetStatus(model.Status);

            _dataManager.Orders.SaveOrder(order);
            _dataManager.SaveChanges();

            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            return GetView(new List<UserRole> { UserRole.User, UserRole.Employee }, "/Home/Index", user.MyOrders);
        }

        [HttpPost]
        public IActionResult IssuedToMe(OrderStatusModel model)
        {
            var order = _dataManager.Orders.GetOrderById(model.Id);
            order.SetStatus(model.Status);

            _dataManager.Orders.SaveOrder(order);
            _dataManager.SaveChanges();

            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            return GetView(new List<UserRole> { UserRole.User, UserRole.Creator }, "/Home/Index", user.IssuedToMeOrders);
        }

        [HttpPost]
        public IActionResult GetText(string id)
        {
            ViewBag.Text = _dataManager.Orders.GetOrderById(id).Text;
            return View();
        }

        private void FillViewBag(ApplicationUser user)
        {
            if (user.OrganizationId != null)
            {
                var organization = _dataManager.Organizations.GetOrganizationById(user.OrganizationId);
                ViewBag.OrganizationName = organization.Name;
                ViewBag.Link = organization.Link;
            }
            else
            {
                ViewBag.Link = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(30);
            }

            ViewBag.UserId = user.Id;
            ViewBag.ShortName = user.GetShortName();
            ViewBag.UserRole = user.Role;
        }

        private IActionResult GetView<T>(List<UserRole> accessDeniedRole, string redirectPage, IEnumerable<T>? model = null)
        {
            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            foreach (var role in accessDeniedRole)
            {
                if (user.Role == role)
                {
                    return Redirect(redirectPage);
                }
            }

            FillViewBag(user);

            if (model == null)
			{
                return View();
            }   
            else
			{
                return View(model);
            }
        }
    }
}