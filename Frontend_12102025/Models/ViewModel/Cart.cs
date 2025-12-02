using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend_12102025.Models.ViewModel
{
    //Cho phep object luu vao Session
    //Luu duoi dang byte array
    [Serializable]
    public class Cart
    {
        private List<CartItem> items = new List<CartItem>();
        //IEnumerable de chi doc
        public IEnumerable<CartItem> Items => items;

        // Them san pham vao gio
        public void AddItem(int bookEditionId, string coverImage, string title,string authorName, string isbn, decimal unitPrice,int quantity, int stock)
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
                // Khong duoc qua stock
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

        // Xoa san pham khoi gio
        public void RemoveItem(int bookEditionId)
        {
            items.RemoveAll(i => i.BookEditionId == bookEditionId);
        }

        // Tong tien
        public decimal TotalValue()
        {
            return items.Sum(i => i.TotalPrice);
        }

        // Tong luong san pham
        public int TotalQuantity()
        {
            return items.Sum(i => i.Quantity);
        }

        // Clear Cart
        public void Clear()
        {
            items.Clear();
        }

        // Update sl san pham
        public void UpdateQuantity(int bookEditionId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.BookEditionId == bookEditionId);
            if (item != null)
            {
                // K qua stock
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

        // Ktra coi co san pham chua
        public bool HasItem(int bookEditionId)
        {
            return items.Any(i => i.BookEditionId == bookEditionId);
        }

        // Lay so luong san pham trong gio
        public int GetQuantity(int bookEditionId)
        {
            var item = items.FirstOrDefault(i => i.BookEditionId == bookEditionId);
            return item?.Quantity ?? 0;
        }
    }
}