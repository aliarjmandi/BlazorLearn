// Endpoints/Cart/CartEndpoints.cs
using BlazorLearn.Services.Abstractions;

namespace BlazorLearn.Endpoints.Cart;

public static class CartEndpoints
{
    public static RouteGroupBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/cart").WithTags("Cart");

        group.MapGet("/{cartId:guid}/items", async (Guid cartId, ICartService carts)
                => Results.Ok(await carts.GetItemsAsync(cartId)));

        group.MapGet("/{cartId:guid}/total", async (Guid cartId, ICartService carts)
                => Results.Ok(await carts.GetCartTotalAsync(cartId)));

        group.MapPost("/add", async (AddItemRequest req, ICartService carts) =>
        {
            await carts.AddItemAsync(req.ProductId, req.Quantity, req.CartId, req.UserId, req.SessionId);
            return Results.NoContent();
        });

        group.MapPatch("/items/{itemId:guid}", async (Guid itemId, UpdateQtyRequest req, ICartService carts) =>
        {
            await carts.UpdateQuantityAsync(itemId, req.Quantity);
            return Results.NoContent();
        });

        group.MapDelete("/items/{itemId:guid}", async (Guid itemId, ICartService carts) =>
        {
            await carts.RemoveItemAsync(itemId);
            return Results.NoContent();
        });

        return group;
    }

    public record AddItemRequest(Guid ProductId, int Quantity, Guid? CartId, string? UserId, string? SessionId);
    public record UpdateQtyRequest(int Quantity);
}
