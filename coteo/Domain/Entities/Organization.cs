using coteo.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace coteo.Domain.Entities
{
    public class Organization : EntityBase
    {
        [Column(TypeName = "nvarchar(100)")]
        public string Link { get; set; }

        public string CreatorId { get; set; }
        [ForeignKey("CreatorId")]
        public ApplicationUser Creator { get; set; }

        public List<Department> Departments { get; set; } = new List<Department>();
    }
}