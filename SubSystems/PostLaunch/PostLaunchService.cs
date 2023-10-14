using Welp.PostLaunch.Models;

namespace Welp.PostLaunch;

public class PostLaunchService : IPostLaunchService
{
    public async Task<PatchReleaseResponse> GetPatchRelease(PatchReleaseRequest request)
    {
        return await Task.FromResult(new PatchReleaseResponse());
    }
}
