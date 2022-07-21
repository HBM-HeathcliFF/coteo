using System.ComponentModel.DataAnnotations;

namespace coteo.Domain.Enum
{
    public enum UserRole
    {
        [Display(Name = "Пользователь")]
        User = 0,
        [Display(Name = "Сотрудник")]
        Employee = 1,
        [Display(Name = "Руководитель")]
        Leader = 2,
        [Display(Name = "Создатель")]
        Creator = 3
    }
}