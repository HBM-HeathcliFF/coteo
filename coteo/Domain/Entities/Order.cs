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
        public User Creator { get; set; }

        public string ExecutorId { get; set; }
        [ForeignKey("ExecutorId")]
        public User Executor { get; set; }


        public void SetStatus(OrderStatus newStatus)
        {
            if (Status != OrderStatus.Canceled &&
                Status != OrderStatus.Completed &&
                Status != OrderStatus.CompletedNotOnTime)
            {
                if (newStatus == OrderStatus.Completed)
                {
                    Performed = DateTime.Now;
                }

                if (Status == OrderStatus.NotOnTime)
                {
                    if (newStatus == OrderStatus.Completed)
                    {
                        Status = OrderStatus.CompletedNotOnTime;
                    }
                    else if (newStatus == OrderStatus.Canceled)
                    {
                        Status = newStatus;
                    }
                }
                else
                {
                    Status = newStatus;
                }
            }
        }
    }
}