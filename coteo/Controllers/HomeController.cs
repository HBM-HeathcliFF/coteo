using coteo.Domain;
using coteo.Domain.Entities;
using coteo.Domain.Enum;
using coteo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using static coteo.SeedData;

namespace coteo.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataManager _dataManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public HomeController(DataManager dataManager, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _dataManager = dataManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        ////////////////////////////////////////////////////////////////////////

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            if (user.Role == RoleNames.User)
            {
                return RedirectToAction("NewOrganization");
            }

            FillViewBag(user);

            return View();
        }

        [HttpGet]
        [Authorize(Roles = $"{RoleNames.Leader},{RoleNames.Creator}")]
        public IActionResult MyOrders()
        {
            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            FillViewBag(user);

            return View(user.MyOrders);
        }

        [HttpGet]
        [Authorize(Roles = $"{RoleNames.Employee},{RoleNames.Leader}")]
        public IActionResult IssuedToMe()
        {
            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            FillViewBag(user);

            return View(user.IssuedToMeOrders);
        }

        [HttpGet]
        [Authorize(Roles = RoleNames.Leader)]
        public IActionResult CreateOrder()
        {
            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            FillViewBag(user);

            return View();
        }

        [HttpGet]
        [Authorize(Roles = RoleNames.Creator)]
        public IActionResult CreateDepartment()
        {
            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            FillViewBag(user);

            return View();
        }

        [HttpGet]
        [Authorize(Roles = $"{RoleNames.Employee},{RoleNames.Leader},{RoleNames.Creator}")]
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

            FillViewBag(user);

            return View(departmentList);
        }

        [HttpGet]
        [Authorize(Roles = RoleNames.Leader)]
        public IActionResult AddEmployee()
        {
            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            FillViewBag(user);

            return View();
        }

        [HttpGet]
        [Authorize(Roles = RoleNames.User)]
        public IActionResult NewOrganization()
        {
            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            FillViewBag(user);

            return View();
        }

        [HttpGet]
        [Authorize(Roles = $"{RoleNames.Employee},{RoleNames.Leader},{RoleNames.Creator}")]
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
            user.Role = RoleNames.User;

            _dataManager.Users.SaveUser(user);

            if (user.Role == RoleNames.Creator)
            {
                var users = _dataManager.Users.GetUsers().Where(x => x.OrganizationId == organization.Id);
                foreach (var usr in users)
                {
                    usr.OrganizationId = null;
                    usr.Role = RoleNames.User;
                    _dataManager.Users.SaveUser(usr);
                }
                _dataManager.Organizations.DeleteOrganization(organization.Id);
            }

            _dataManager.SaveChanges();

            return Redirect("/Home/NewOrganization");
        }

        ////////////////////////////////////////////////////////////////////////

        [HttpPost]
        [Authorize(Roles = RoleNames.User)]
        public async Task<IActionResult> NewOrganization(Organization organization)
        {
            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            if (_dataManager.Organizations.GetOrganizations().FirstOrDefault(x => x.Link == organization.Link) != null)
            {
                await SetRoleAsync(user, user.Role, RoleNames.Employee, true);
                user.Role = RoleNames.Employee;
            }
            else
            {
                organization.Creator = user;
                _dataManager.Organizations.SaveOrganization(organization);
                _dataManager.SaveChanges();

                await SetRoleAsync(user, user.Role, RoleNames.Creator, true);
                user.Role = RoleNames.Creator;
            }

            user.OrganizationId = _dataManager.Organizations.GetOrganizations().First(x => x.Link == organization.Link).Id;
            _dataManager.Users.SaveUser(user);

            _dataManager.SaveChanges();

            return Redirect("/Home/Index");
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Creator)]
        public async Task<IActionResult> CreateDepartment(CreateDepartmentModel model)
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

                    await SetRoleAsync(user, user.Role, RoleNames.Leader, false);
                    user.Role = RoleNames.Leader;
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

            FillViewBag(user);

            return View();
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Leader)]
        public async Task<IActionResult> AddEmployee(AddEmployeeModel model)
        {
            string message;
            var user = _dataManager.Users.GetUsers().FirstOrDefault(x => x.FullName == model.EmployeeFullName);
            var leader = _dataManager.Users.GetUsers().FirstOrDefault(x => x.Id == model.LeaderId);

            if (user != null && leader != null && leader.OrganizationId == user.OrganizationId)
            {
                Department department = _dataManager.Departments.GetDepartments().First(x => x.LeaderId == leader.Id);

                if (user.DepartmentId == null)
                {
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

            FillViewBag(user);

            return View();
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Leader)]
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

            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            FillViewBag(user);

            return View();
        }
        
        [HttpPost]
        [Authorize(Roles = $"{RoleNames.Leader},{RoleNames.Creator}")]
        public IActionResult MyOrders(OrderStatusModel model)
        {
            var order = _dataManager.Orders.GetOrderById(model.Id);
            order.SetStatus(model.Status);

            _dataManager.Orders.SaveOrder(order);
            _dataManager.SaveChanges();

            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            FillViewBag(user);

            return View(user.MyOrders);
        }

        [HttpPost]
        [Authorize(Roles = $"{RoleNames.Employee},{RoleNames.Leader}")]
        public IActionResult IssuedToMe(OrderStatusModel model)
        {
            var order = _dataManager.Orders.GetOrderById(model.Id);
            order.SetStatus(model.Status);

            _dataManager.Orders.SaveOrder(order);
            _dataManager.SaveChanges();

            var user = _dataManager.Users.GetUserById(_userManager.GetUserId(User));

            FillViewBag(user);

            return View(user.IssuedToMeOrders);
        }

        [HttpPost]
        [Authorize]
        public IActionResult GetText(string id)
        {
            ViewBag.Text = _dataManager.Orders.GetOrderById(id).Text;
            return View();
        }

        ////////////////////////////////////////////////////////////////////////

        private void FillViewBag(User user)
        {
            if (user.OrganizationId != null)
            {
                var organization = _dataManager.Organizations.GetOrganizationById(user.OrganizationId);
                ViewBag.OrganizationName = $"\"{organization.Name}\"";
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

        private async Task SetRoleAsync(User user, string prevRole, string currentRole, bool isCurrentUser)
        {
            var identityUser = await _userManager.FindByNameAsync(user.Email);
            await _userManager.RemoveFromRoleAsync(identityUser, prevRole);
            await _userManager.AddToRoleAsync(identityUser, currentRole);
            if (isCurrentUser)
            {
                await _signInManager.SignInAsync(user, false, null);
            }
        }
    }
}