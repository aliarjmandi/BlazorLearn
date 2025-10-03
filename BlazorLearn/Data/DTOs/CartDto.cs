// Data/DTOs/CartDto.cs
using System;

namespace BlazorLearn.Data.DTOs;

public sealed class CartDto
{
    public Guid Id { get; set; }
    public Guid? PersonId { get; set; }          // nullable برای مهمان
    public string? UserId { get; set; }          // AspNetUsers.Id (nullable)
    public string? SessionId { get; set; }       // برای مهمان (nullable)
    public byte Status { get; set; }             // 0=Open, 1=Ordered, 2=Abandoned...
    public DateTime CreatedAt { get; set; }
}

public sealed class CartItemDto
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public string ProductSlug { get; set; } = "";
    public string? FirstImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }       // قیمت زمان افزودن
    public DateTime AddedAt { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;
}

/*
public sealed class CartItemDto
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
}
*/
// برای نوشتن (ایجاد/ویرایش) آیتم
public sealed class CartItemWriteDto
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}
