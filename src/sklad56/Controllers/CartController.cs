using System;
using System.Linq;
using System.Web.Mvc;
using sklad56.Models;
using System.Collections.Generic;

namespace sklad56.Controllers
{
    public class CartController : BaseController
    {
        // GET: Cart
        [Authorize(Roles = Globals.editGroup)]
        public ViewResult Index(string returnUrl)
        {
            List<SelectListItem> UserList = new List<SelectListItem>();

            foreach(User x in Repository.Users)
            {
                UserList.Add(new SelectListItem(){Text = x.Username});
            }

            return View(new CartIndexViewModel //отображаем корзину
            {
                Cart = GetCart(),
                ReturnUrl = returnUrl,
                Users = UserList
            });
        }

        public RedirectToRouteResult AddToCart(Guid ItemID, string returnUrl)
        {
            Item item = Repository.Items
                .FirstOrDefault(g => g.ID_Item == ItemID); //ищем предмет в базе

            if (item != null)
            {
                GetCart().AddItem(item, 1); //если нашли - добавляем в корзину
            }
            return RedirectToAction("Index", new { returnUrl }); //перенаправляем на корзину
        }

        public RedirectToRouteResult RemoveFromCart(Guid ItemID, string returnUrl) //удаление из корзины выглядит аналогично добавлению
        {
            Item item = Repository.Items
                .FirstOrDefault(g => g.ID_Item == ItemID);

            if (item != null)
            {
                GetCart().RemoveLine(item);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public Cart GetCart()
        {
            Cart cart = (Cart)Session["Cart"]; //достаём корзину из сессии
            if (cart == null) //если пустая - заводим новую
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
    }
}