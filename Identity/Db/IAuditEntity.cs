using System.ComponentModel.DataAnnotations;

namespace Identity.Db
{
    public interface IAuditEntity
    {
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public string CreatedIP { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedIP { get; set; }
        public int Version { get; set; }
    }
}