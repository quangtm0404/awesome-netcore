namespace anc.webapi.Entities;
public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNo { get; set; } = string.Empty;
}