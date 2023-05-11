using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Db
{
    public class BaseModel : IAuditEntity
    {
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedIP { get; set; }


        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedIP { get; set; }

        public int Version { get; set; }
    }

    public class BaseModel<T> : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public T Id { get; set; }
    }

    public class BaseSoftDeletableModel<T> : BaseModel<T>, ISoftDeletableEntity
    {
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
    }
}