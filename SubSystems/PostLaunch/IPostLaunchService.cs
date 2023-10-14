using Welp.PostLaunch.Models;

namespace Welp.PostLaunch;

public interface IPostLaunchService
{
    Task<PatchReleaseResponse> GetPatchRelease(PatchReleaseRequest request);
}
