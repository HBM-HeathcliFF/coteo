using Microsoft.AspNetCore.Identity;

namespace coteo
{
    public class SeedData
    {
        public static async void CreateRoles(IServiceProvider provider)
        {
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var roleName in RoleNames.AllRoles)
            {
                var role = roleManager.FindByNameAsync(roleName).Result;

                if (role == null)
                {
                    IdentityResult result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                }
            }
        }

        public static class RoleNames
        {
            public const string User = "User";
            public const string Employee = "Employee";
            public const string Leader = "Leader";
            public const string Creator = "Creator";

            public static IEnumerable<string> AllRoles
            {
                get
                {
                    yield return User;
                    yield return Employee;
                    yield return Leader;
                    yield return Creator;
                }  
            }
        }
    }
}