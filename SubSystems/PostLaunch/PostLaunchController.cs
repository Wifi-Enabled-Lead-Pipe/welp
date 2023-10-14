using Microsoft.AspNetCore.Mvc;
using Welp.PostLaunch.Models;

namespace Welp.PostLaunch;

[Route("post-launch")]
public class PostLaunchController : Controller, IPostLaunchService
{
    private readonly IPostLaunchService postLaunchService;

    public PostLaunchController(IPostLaunchService postLaunchService)
    {
        this.postLaunchService = postLaunchService;
    }

    /// <summary>
    /// Patch Releases: Addresses bug fixes and improvements.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<PatchReleaseResponse> GetPatchRelease(PatchReleaseRequest request) =>
        await postLaunchService.GetPatchRelease(request);
}
