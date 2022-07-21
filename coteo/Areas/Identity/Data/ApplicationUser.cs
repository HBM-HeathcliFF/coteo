using coteo.Domain.Entities;
using coteo.Domain.Enum;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace coteo.Areas.Identity.Data;

public class ApplicationUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(150)")]
    public string FullName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(30)")]
    public UserRole Role { get; set; }

    public string? OrganizationId { get; set; }
    [ForeignKey("OrganizationId")]
    public Organization Organization { get; set; }
    public string? DepartmentId { get; set; }
    [ForeignKey("DepartmentId")]
    public Department Department { get; set; }

    public List<Order> MyOrders { get; set; } = new();
    public List<Order> IssuedToMeOrders { get; set; } = new();
}