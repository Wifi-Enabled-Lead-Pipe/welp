namespace Welp.PostLaunch;

public class PostLaunchService : IPostLaunchService 
{ 
    public async Task<PatchReleaseResponse> GetPatchRelease(PatchReleaseResponse request)
    {
        return await Task.FromResult(new PatchReleaseResponse());
    }
}
