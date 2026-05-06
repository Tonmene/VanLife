using Microsoft.AspNetCore.Mvc;
using VanLife.Api.Models;
using VanLife.Api.Services;

namespace VanLife.Api.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController(ReviewService reviewService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUserReviews([FromQuery] ReviewQuery query)
    {
        return Ok(await reviewService.GetUserReviews(query));
    }
}

