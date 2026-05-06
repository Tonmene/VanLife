using System.ComponentModel.DataAnnotations;

namespace VanLife.Api.Models;

public record VanListItemDto(Guid Id, string Name, VanCategory Category, decimal PricePerDay);

public record VanDetailsDto(
    Guid Id,
    string Name,
    decimal PricePerDay,
    string FullDescription,
    bool Availability,
    int NumberAvailable
);

public record SellerVanDetailsDto(
    Guid Id,
    string Name,
    VanCategory Category,
    string FullDescription,
    bool Visibility,
    decimal Pricing,
    List<string> Photos
);

public class PaginationQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int Skip { get; set; } = 0;
}

public class VanQuery : PaginationQuery
{
    public VanCategory? Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public Guid? SellerId { get; set; }
    public bool? IsVisible { get; set; }
}

public class ReviewQuery : PaginationQuery
{
    public Guid UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ReviewType? ReviewType { get; set; }
}

public class TransactionQuery : PaginationQuery
{
    public Guid? SellerId { get; set; }
    public int? Days { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}

public record PagedResult<T>(IEnumerable<T> Items, int Total, int Page, int PageSize, int Skip);

public class SignUpRequest
{
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public string ConfirmPassword { get; set; } = string.Empty;
    [Required] public UserRole Role { get; set; }
}

public class LoginRequest
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public UserRole Role { get; set; }
}

public class ForgotPasswordRequest
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string NewPassword { get; set; } = string.Empty;
    [Required] public string ConfirmPassword { get; set; } = string.Empty;
}

