namespace Welp.UserManagement.Models;

public class PatchReleaseRequest {  }

public record PatchReleaseResponse
{
    public string BugFixes { get; set; } = string.Empty;
    public string Improvements { get; set; } = string.Empty;
}
