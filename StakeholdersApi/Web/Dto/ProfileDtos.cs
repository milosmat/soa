namespace StakeholdersApi.Web.Dto;
public class ProfileResponse
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public string? Motto { get; set; }
}
public class ProfileUpdateRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public string? Motto { get; set; }
}
