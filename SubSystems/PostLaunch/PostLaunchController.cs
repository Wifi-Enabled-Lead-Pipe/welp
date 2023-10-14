using Microsoft.AspNetCore.Mvc;

namespace Welp.PostLaunch;

[Route("post-launch")]
public class PostLaunchController : Controller, IPostLaunchService
{
    private readonly IPostLaunchService postLaunchService;

    public PostLaunchController(IPostLaunchService postLaunchService)
    {
        this.postLaunchService = postLaunchService;
    }

    [HttpGet]
    public async Task<PatchReleaseResponse> GetPatchRelease(PatchReleaseRequest request) =>
        await postLaunchService.GetPatchRelease(request);

}
