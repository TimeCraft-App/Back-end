namespace TimeCraft.Domain.Entities
{
    public class BaseEntity
    {
        public virtual int Id { get; set; }
       
        public bool Deleted { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public DateTime UpdatedOn { get; set;}
    }
}
