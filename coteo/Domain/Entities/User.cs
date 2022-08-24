using coteo.Domain.Enum;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace coteo.Domain.Entities
{
    public class User : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(30)")]
        public string Role { get; set; }

        public string? OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
        public string? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        public List<Order> MyOrders { get; set; } = new();
        public List<Order> IssuedToMeOrders { get; set; } = new();

        public string GetShortName()
        {
            string[] words = FullName.Split(' ');
            if (words.Length < 2 || words.Length > 3)
            {
                return "";
            }
            string shortName = words[0];
            for (int i = 1; i < words.Length; i++)
            {
                shortName += $" {words[i][0]}.";
            }
            return shortName;
        }
    }
}