namespace Welp.PostLaunch;

public interface IPostLaunchService 
{ 
    Task<PatchReleaseResponse> GetPatchRelease(PatchReleaseRequest request);
}

