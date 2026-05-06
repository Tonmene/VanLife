using Microsoft.EntityFrameworkCore;
using VanLife.Api.Data;
using VanLife.Api.Models;

namespace VanLife.Api.Services;

public class DashboardService(AppDbContext db, VanService vanService, IncomeService incomeService)
{
    public async Task<object> GetSellerSummary(Guid sellerId, TransactionQuery query, VanQuery vanQuery)
    {
        vanQuery.SellerId = sellerId;
        var vans = await vanService.GetAll(vanQuery);

        query.SellerId = sellerId;
        var transactions = await incomeService.GetTransactions(query);
        var totalIncome = await db.Transactions
            .Where(t => t.SellerId == sellerId)
            .SumAsync(t => (decimal?)t.Price) ?? 0m;

        return new { sellerId, totalIncome, vans, transactions };
    }
}

