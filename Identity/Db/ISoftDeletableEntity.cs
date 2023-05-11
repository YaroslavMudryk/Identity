namespace Identity.Db
{
    public interface ISoftDeletableEntity
    {
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
    }
}