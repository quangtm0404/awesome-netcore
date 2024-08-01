namespace anc.domains.Entities;
public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public int Quota { get; set; } = 0;
    public int TimeLimit { get; set; } = 60;
}