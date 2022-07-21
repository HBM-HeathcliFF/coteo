using System.ComponentModel.DataAnnotations;

namespace coteo.Domain.Enum
{
    public enum OrderStatus
    {
        [Display(Name = "В работе")]
        InWork = 0,
        [Display(Name = "Отложено")]
        Postponed = 1,
        [Display(Name = "Просрочено")]
        NotOnTime = 2,
        [Display(Name = "Отменено")]
        Canceled = 3,
        [Display(Name = "Выполнено")]
        Completed = 4,
        [Display(Name = "Выполнено не в срок")]
        CompletedNotOnTime = 5
    }
}