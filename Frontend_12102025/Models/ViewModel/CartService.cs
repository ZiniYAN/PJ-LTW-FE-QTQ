using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend_12102025.Models.ViewModel
{
    public class CartService
    {
        //Refer den Session object
        private readonly HttpSessionStateBase session;
        //Ten key la Cart, lay tu session
        private const string CartSessionKey = "Cart";

        public CartService(HttpSessionStateBase session)
        {
            this.session = session;
        }

        // Lấy giỏ hàng từ Session
        public Cart GetCart()
        {
            var cart = (Cart)session[CartSessionKey];
            if (cart == null)
            {
                cart = new Cart();
                session[CartSessionKey] = cart;
            }
            return cart;
        }

        // Xóa giỏ hàng
        public void ClearCart()
        {
            session[CartSessionKey] = null;
        }

        // Lưu giỏ hàng vào Session
        public void SaveCart(Cart cart)
        {
            session[CartSessionKey] = cart;
        }

        // Lấy số lượng sách trong giỏ (hiển thị trên icon giỏ hàng)
        public int GetCartItemCount()
        {
            var cart = GetCart();
            return cart.TotalQuantity();
        }

        // Lấy tổng giá trị giỏ hàng
        public decimal GetCartTotal()
        {
            var cart = GetCart();
            return cart.TotalValue();
        }
    }
}