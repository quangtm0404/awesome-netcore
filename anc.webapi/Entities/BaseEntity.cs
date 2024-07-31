namespace anc.webapi.Entities;
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsDeleted { get; set; } = false;
    public DateTime CreationDate { get; set; } = DateTime.Now;
}