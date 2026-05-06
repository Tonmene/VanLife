using Microsoft.AspNetCore.Mvc;
using VanLife.Api.Models;
using VanLife.Api.Services;

namespace VanLife.Api.Controllers;

[ApiController]
[Route("api/vans")]
public class VansController(VanService vanService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] VanQuery query)
    {
        var vans = await vanService.GetAll(query);
        return Ok(vans);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOne(Guid id)
    {
        var van = await vanService.GetById(id);
        return van is null ? NotFound(new { message = "Van not found." }) : Ok(van);
    }

    [HttpGet("{id:guid}/management")]
    public async Task<IActionResult> GetSellerVanDetails(Guid id)
    {
        var van = await vanService.GetSellerVan(id);
        return van is null ? NotFound(new { message = "Van not found." }) : Ok(van);
    }

    [HttpPost("{id:guid}/rent")]
    public async Task<IActionResult> Rent(Guid id, [FromQuery] int days = 1)
    {
        var result = await vanService.RentVan(id, days);
        return Ok(result);
    }
}

