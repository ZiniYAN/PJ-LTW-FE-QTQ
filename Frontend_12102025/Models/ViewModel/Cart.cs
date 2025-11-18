using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend_12102025.Models.ViewModel
{
    [Serializable]
    public class Cart
    {
        private List<CartItem> items = new List<CartItem>();

        public IEnumerable<CartItem> Items => items;

        // Thêm sách vào giỏ
        public void AddItem(int bookEditionId, string coverImage, string title,
                            string authorName, string isbn, decimal unitPrice,
                            int quantity, int stock)
        {
            var existingItem = items.FirstOrDefault(i => i.BookEditionId == bookEditionId);
            if (existingItem == null)
            {
                items.Add(new CartItem
                {
                    BookEditionId = bookEditionId,
                    CoverImage = coverImage,
                    Title = title,
                    AuthorName = authorName,
                    ISBN = isbn,
                    UnitPrice = unitPrice,
                    Quantity = quantity,
                    Stock = stock
                });
            }
            else
            {
                // Kiểm tra không vượt quá tồn kho
                if (existingItem.Quantity + quantity <= stock)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    existingItem.Quantity = stock;
                }
            }
        }

        // Xóa sách khỏi giỏ
        public void RemoveItem(int bookEditionId)
        {
            items.RemoveAll(i => i.BookEditionId == bookEditionId);
        }

        // Tính tổng giá trị giỏ hàng
        public decimal TotalValue()
        {
            return items.Sum(i => i.TotalPrice);
        }

        // Tính tổng số lượng sách trong giỏ
        public int TotalQuantity()
        {
            return items.Sum(i => i.Quantity);
        }

        // Làm trống giỏ hàng
        public void Clear()
        {
            items.Clear();
        }

        // Cập nhật số lượng của sách đã chọn
        public void UpdateQuantity(int bookEditionId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.BookEditionId == bookEditionId);
            if (item != null)
            {
                // Kiểm tra không vượt quá tồn kho
                if (quantity <= item.Stock && quantity > 0)
                {
                    item.Quantity = quantity;
                }
                else if (quantity > item.Stock)
                {
                    item.Quantity = item.Stock;
                }
            }
        }

        // Kiểm tra giỏ hàng có sách này chưa
        public bool HasItem(int bookEditionId)
        {
            return items.Any(i => i.BookEditionId == bookEditionId);
        }

        // Lấy số lượng của một sách trong giỏ
        public int GetQuantity(int bookEditionId)
        {
            var item = items.FirstOrDefault(i => i.BookEditionId == bookEditionId);
            return item?.Quantity ?? 0;
        }
    }
}