using coteo.Areas.Identity.Data;
using coteo.Domain;
using coteo.Domain.Repositories.Abstract;
using coteo.Domain.Repositories.EntityFramework;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddTransient<IUsersRepository, EFUsersRepository>();
builder.Services.AddTransient<IOrganizationsRepository, EFOrganizationsRepository>();
builder.Services.AddTransient<IDepartmentsRepository, EFDepartmentsRepository>();
builder.Services.AddTransient<IOrdersRepository, EFOrdersRepository>();
builder.Services.AddTransient<DataManager>();

builder.Services.AddHostedService<TimedHostedService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();