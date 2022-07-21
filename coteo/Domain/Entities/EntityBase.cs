using System.ComponentModel.DataAnnotations.Schema;

namespace coteo.Domain.Entities
{
    public abstract class EntityBase
    {
        public string Id { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Name { get; set; }
    }
}