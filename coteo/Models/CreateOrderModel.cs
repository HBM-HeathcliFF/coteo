namespace coteo.Models
{
    public class CreateOrderModel
    {
        public string CreatedById { get; set; }
        public string OrderName { get; set; }
        public string ExecutorFullName { get; set; }
        public string Deadline { get; set; }
        public string Text { get; set; }
    }
}