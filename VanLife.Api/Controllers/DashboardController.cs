using Microsoft.AspNetCore.Mvc;
using VanLife.Api.Models;
using VanLife.Api.Services;

namespace VanLife.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(DashboardService dashboardService, IncomeService incomeService) : ControllerBase
{
    [HttpGet("sellers/{sellerId:guid}")]
    public async Task<IActionResult> GetSellerSummary(
        Guid sellerId,
        [FromQuery] VanQuery vanQuery,
        [FromQuery] TransactionQuery transactionQuery)
    {
        var summary = await dashboardService.GetSellerSummary(sellerId, transactionQuery, vanQuery);
        return Ok(summary);
    }

    [HttpGet("income-graph")]
    public async Task<IActionResult> GetIncomeGraph([FromQuery] int year, [FromQuery] Guid? sellerId)
    {
        return Ok(await incomeService.GetYearlyGraph(year, sellerId));
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] TransactionQuery query)
    {
        return Ok(await incomeService.GetTransactions(query));
    }
}
