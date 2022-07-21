using coteo.Areas.Identity.Data;
using coteo.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace coteo.Domain.Entities
{
    public class Order : EntityBase
    {
        [Column(TypeName = "nvarchar(2000)")]
        public string Text { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime? Performed { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        public OrderStatus Status { get; set; }

        public string CreatedById { get; set; }
        [ForeignKey("CreatorId")]
        public ApplicationUser Creator { get; set; }

        public string ExecutorId { get; set; }
        [ForeignKey("ExecutorId")]
        public ApplicationUser Executor { get; set; }
    }
}