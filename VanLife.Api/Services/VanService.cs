using Microsoft.EntityFrameworkCore;
using VanLife.Api.Data;
using VanLife.Api.Models;

namespace VanLife.Api.Services;

public class VanService(AppDbContext db)
{
    public async Task<PagedResult<VanListItemDto>> GetAll(VanQuery query)
    {
        var vans = db.Vans.AsQueryable();

        if (query.Category.HasValue)
        {
            vans = vans.Where(v => v.Category == query.Category.Value);
        }

        if (query.MinPrice.HasValue)
        {
            vans = vans.Where(v => v.PricePerDay >= query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
        {
            vans = vans.Where(v => v.PricePerDay <= query.MaxPrice.Value);
        }

        if (query.SellerId.HasValue)
        {
            vans = vans.Where(v => v.SellerId == query.SellerId.Value);
        }

        if (query.IsVisible.HasValue)
        {
            vans = vans.Where(v => v.IsVisible == query.IsVisible.Value);
        }

        var total = await vans.CountAsync();
        var safePage = Math.Max(1, query.Page);
        var safePageSize = Math.Clamp(query.PageSize, 1, 100);
        var pageSkip = (safePage - 1) * safePageSize;
        var skip = Math.Max(query.Skip, pageSkip);

        var items = await vans
            .OrderBy(v => v.Name)
            .Skip(skip)
            .Take(safePageSize)
            .Select(v => new VanListItemDto(v.Id, v.Name, v.Category, v.PricePerDay))
            .ToListAsync();

        return new PagedResult<VanListItemDto>(items, total, safePage, safePageSize, skip);
    }

    public async Task<VanDetailsDto?> GetById(Guid id)
    {
        var van = await db.Vans.FirstOrDefaultAsync(v => v.Id == id);
        return van is null
            ? null
            : new VanDetailsDto(van.Id, van.Name, van.PricePerDay, van.FullDescription, van.IsAvailable, van.NumberAvailable);
    }

    public async Task<SellerVanDetailsDto?> GetSellerVan(Guid id)
    {
        var van = await db.Vans.Include(v => v.Photos).FirstOrDefaultAsync(v => v.Id == id);
        return van is null
            ? null
            : new SellerVanDetailsDto(van.Id, van.Name, van.Category, van.FullDescription, van.IsVisible, van.PricePerDay, van.Photos.Select(x => x.Url).ToList());
    }

    public async Task<object> RentVan(Guid vanId, int days)
    {
        var van = await db.Vans.FirstOrDefaultAsync(v => v.Id == vanId);
        if (van is null)
        {
            return new { success = false, message = "Van not found." };
        }

        if (!van.IsAvailable || van.NumberAvailable < 1)
        {
            return new { success = false, message = "Van is not currently available." };
        }

        if (days < 1)
        {
            return new { success = false, message = "Days must be at least 1." };
        }

        var totalPrice = van.PricePerDay * days;
        van.NumberAvailable -= 1;
        van.IsAvailable = van.NumberAvailable > 0;

        db.Transactions.Add(new Transaction
        {
            Id = Guid.NewGuid(),
            SellerId = van.SellerId,
            VanId = van.Id,
            Price = totalPrice,
            Date = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        return new
        {
            success = true,
            message = "Van rented successfully.",
            vanId,
            days,
            totalPrice
        };
    }
}

