using Microsoft.EntityFrameworkCore;
using VanLife.Api.Data;
using VanLife.Api.Models;

namespace VanLife.Api.Services;

public class ReviewService(AppDbContext db)
{
    public async Task<object> GetUserReviews(ReviewQuery query)
    {
        var reviews = db.Reviews.Where(r => r.TargetUserId == query.UserId);

        if (query.StartDate.HasValue)
        {
            reviews = reviews.Where(r => r.Date >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            reviews = reviews.Where(r => r.Date <= query.EndDate.Value);
        }

        var filtered = await reviews.ToListAsync();
        var total = filtered.Count;

        if (query.ReviewType.HasValue)
        {
            filtered = filtered.Where(r => r.Type == query.ReviewType.Value).ToList();
        }

        var specificCount = filtered.Count;
        var percentage = total == 0 ? 0 : (double)specificCount / total * 100;
        var safePage = Math.Max(1, query.Page);
        var safePageSize = Math.Clamp(query.PageSize, 1, 100);
        var pageSkip = (safePage - 1) * safePageSize;
        var skip = Math.Max(query.Skip, pageSkip);

        var pageItems = filtered
            .OrderByDescending(r => r.Date)
            .Skip(skip)
            .Take(safePageSize)
            .ToList();

        return new
        {
            totalReviews = total,
            specificReviews = specificCount,
            reviewPercentage = Math.Round(percentage, 2),
            page = safePage,
            pageSize = safePageSize,
            skip,
            items = pageItems.Select(r => new { r.Id, r.Date, r.Type, r.Stars, r.Comment })
        };
    }
}

