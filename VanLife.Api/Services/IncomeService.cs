using Microsoft.EntityFrameworkCore;
using VanLife.Api.Data;
using VanLife.Api.Models;

namespace VanLife.Api.Services;

public class IncomeService(AppDbContext db)
{
    public async Task<object> GetYearlyGraph(int year, Guid? sellerId = null)
    {
        var query = db.Transactions.Where(t => t.Date.Year == year);
        if (sellerId.HasValue)
        {
            query = query.Where(t => t.SellerId == sellerId.Value);
        }

        var transactions = await query.ToListAsync();

        var monthly = Enumerable.Range(1, 12)
            .Select(month =>
            {
                var monthTransactions = transactions.Where(t => t.Date.Month == month).ToList();
                return new
                {
                    month,
                    transactionCount = monthTransactions.Count,
                    totalValue = monthTransactions.Sum(t => t.Price)
                };
            });

        return new { year, monthly };
    }

    public async Task<PagedResult<object>> GetTransactions(TransactionQuery query)
    {
        var transactions = db.Transactions.AsQueryable();

        if (query.SellerId.HasValue)
        {
            transactions = transactions.Where(t => t.SellerId == query.SellerId.Value);
        }

        if (query.Days.HasValue)
        {
            var from = DateTime.UtcNow.AddDays(-query.Days.Value);
            transactions = transactions.Where(t => t.Date >= from);
        }

        if (query.StartDate.HasValue)
        {
            transactions = transactions.Where(t => t.Date >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            transactions = transactions.Where(t => t.Date <= query.EndDate.Value);
        }

        if (query.MinPrice.HasValue)
        {
            transactions = transactions.Where(t => t.Price >= query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
        {
            transactions = transactions.Where(t => t.Price <= query.MaxPrice.Value);
        }

        var total = await transactions.CountAsync();
        var safePage = Math.Max(1, query.Page);
        var safePageSize = Math.Clamp(query.PageSize, 1, 100);
        var pageSkip = (safePage - 1) * safePageSize;
        var skip = Math.Max(query.Skip, pageSkip);

        var list = await transactions
            .OrderByDescending(t => t.Date)
            .Skip(skip)
            .Take(safePageSize)
            .Select(t => new { t.Id, t.SellerId, t.VanId, t.Price, t.Date })
            .ToListAsync();

        return new PagedResult<object>(list.Cast<object>(), total, safePage, safePageSize, skip);
    }
}

