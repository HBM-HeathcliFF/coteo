using coteo;
using coteo.Domain;
using coteo.Domain.Entities;
using coteo.Domain.Repositories.Abstract;
using coteo.Domain.Repositories.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(config =>
{
    config.Cookie.Name = "Identity.Cookie";
    config.LoginPath = "/Account/Login";
    config.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddTransient<IUsersRepository, EFUsersRepository>();
builder.Services.AddTransient<IOrganizationsRepository, EFOrganizationsRepository>();
builder.Services.AddTransient<IDepartmentsRepository, EFDepartmentsRepository>();
builder.Services.AddTransient<IOrdersRepository, EFOrdersRepository>();
builder.Services.AddTransient<DataManager>();

builder.Services.AddHostedService<TimedHostedService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

using (var serviceScope = app.Services.CreateScope())
{
    SeedData.CreateRoles(serviceScope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();