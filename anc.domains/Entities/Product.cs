namespace anc.domains.Entities;
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; } = 0;
}