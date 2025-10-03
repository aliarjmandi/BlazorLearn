using BlazorLearn.Data.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorLearn.Services.Abstractions
{
    /// <summary>
    /// سرویس مدیریت سبد خرید (مهمان + کاربر لاگین کرده)
    /// </summary>
    public interface ICartService
    {
        /// <summary>
        /// بر اساس userId یا sessionId یک سبد «باز» برمی‌گرداند؛ اگر نبود می‌سازد و Id آن را می‌دهد.
        /// </summary>
        Task<Guid> GetOrCreateOpenCartAsync(string? userId, string? sessionId);

        /// <summary>آیتم‌های سبد</summary>
        Task<IEnumerable<CartItemDto>> GetItemsAsync(Guid cartId);

        /// <summary>تعداد کل اقلام (جمع Quantityها) در سبد</summary>
        Task<int> GetCountAsync(Guid cartId);

        /// <summary>
        /// اضافه‌کردن یا افزایش تعداد یک محصول در سبد.
        /// اگر cartId ندارید، userId / sessionId بدهید تا خودش سبد را پیدا/ایجاد کند.
        /// </summary>
        Task AddItemAsync(Guid productId, int quantity = 1, Guid? cartId = null, string? userId = null, string? sessionId = null);

        /// <summary>تغییر تعداد یک آیتم سبد (اگر مقدار <= 0 شود آیتم حذف می‌گردد)</summary>
        Task UpdateQuantityAsync(Guid itemId, int quantity);

        /// <summary>حذف آیتم از سبد</summary>
        Task RemoveItemAsync(Guid itemId);

        /// <summary>حذف تمام آیتم‌ها</summary>
        Task ClearAsync(Guid cartId);

        /// <summary>
        /// ادغام سبد مهمان (بر اساس sessionId) با سبد کاربر (بر اساس userId) پس از لاگین.
        /// </summary>
        Task MergeGuestCartAsync(string sessionId, string userId);
        
        // 👇 متد جدید برای جمع کل
        Task<decimal> GetCartTotalAsync(Guid cartId);
    }

}
