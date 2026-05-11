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

    // Seller-only: list seller inventory (includes hidden vans). Requires X-User-Id header with seller id.
    [HttpGet("seller/vans")]
    public async Task<IActionResult> GetSellerInventory([FromQuery] VanQuery query)
    {
        if (!Request.Headers.TryGetValue("X-User-Id", out var sid) || !Guid.TryParse(sid.FirstOrDefault(), out var userId))
        {
            return Unauthorized(new { message = "Seller user id header (X-User-Id) required." });
        }

        var vans = await vanService.GetSellerInventory(userId, query);
        return Ok(vans);
    }

    // Seller-only: create a new van. Requires X-User-Id header with seller id.
    [HttpPost]
    [Route("seller/vans")]
    public async Task<IActionResult> CreateVan([FromBody] CreateVanRequest request)
    {
        if (!Request.Headers.TryGetValue("X-User-Id", out var sid) || !Guid.TryParse(sid.FirstOrDefault(), out var userId))
        {
            return Unauthorized(new { message = "Seller user id header (X-User-Id) required." });
        }

        var result = await vanService.CreateVan(userId, request) as CreateResult;
        if (result is null) return BadRequest(new { message = "Could not create van." });
        return result.Success ? CreatedAtAction(nameof(GetOne), new { id = result.VanId }, result) : BadRequest(result);
    }

    // Seller-only: update a van's details
    [HttpPut("seller/vans/{id:guid}")]
    public async Task<IActionResult> UpdateVan(Guid id, [FromBody] UpdateVanRequest request)
    {
        if (!Request.Headers.TryGetValue("X-User-Id", out var sid) || !Guid.TryParse(sid.FirstOrDefault(), out var userId))
        {
            return Unauthorized(new { message = "Seller user id header (X-User-Id) required." });
        }

        var result = await vanService.UpdateVan(userId, id, request) as OperationResult;
        if (result is null) return BadRequest(new { message = "Could not update van." });
        return result.Success ? Ok(result) : Forbid();
    }

    // Seller-only: toggle availability or set inventory count
    [HttpPatch("seller/vans/{id:guid}/availability")]
    public async Task<IActionResult> UpdateAvailability(Guid id, [FromBody] UpdateAvailabilityRequest request)
    {
        if (!Request.Headers.TryGetValue("X-User-Id", out var sid) || !Guid.TryParse(sid.FirstOrDefault(), out var userId))
        {
            return Unauthorized(new { message = "Seller user id header (X-User-Id) required." });
        }

        var success = await vanService.UpdateAvailability(userId, id, request.IsAvailable, request.NumberAvailable);
        return success ? Ok(new { message = "Availability updated." }) : Forbid();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOne(Guid id)
    {
        var van = await vanService.GetById(id);
        return van is null ? NotFound(new { message = "Van not found." }) : Ok(van);
    }

    // Use the canonical details endpoint GET /api/vans/{id} instead of duplicate query-based routes.

    [HttpGet("{id:guid}/management")]
    public async Task<IActionResult> GetSellerVanDetails(Guid id)
    {
        if (!Request.Headers.TryGetValue("X-User-Id", out var sid) || !Guid.TryParse(sid.FirstOrDefault(), out var userId))
        {
            return Unauthorized(new { message = "Seller user id header (X-User-Id) required." });
        }

        var van = await vanService.GetSellerVan(userId, id);
        return van is null ? Forbid() : Ok(van);
    }

    [HttpPost("{id:guid}/rent")]
    public async Task<IActionResult> Rent(Guid id, [FromBody] RentRequest request)
    {
        if (!Request.Headers.TryGetValue("X-User-Id", out var bid) || !Guid.TryParse(bid.FirstOrDefault(), out var buyerId))
        {
            return Unauthorized(new { success = false, message = "Buyer user id header (X-User-Id) required." });
        }

        // Basic validation: user must be signed up and provide payment token and contact
        if (request.Days < 1)
        {
            return BadRequest(new { success = false, message = "Days must be at least 1." });
        }

        if (string.IsNullOrWhiteSpace(request.PaymentToken))
        {
            return BadRequest(new { success = false, message = "Payment is required before taking the van." });
        }

        if (string.IsNullOrWhiteSpace(request.Contact))
        {
            return BadRequest(new { success = false, message = "Contact information is required." });
        }

        var result = await vanService.RentVan(id, buyerId, request);
        return Ok(result);
    }
}

