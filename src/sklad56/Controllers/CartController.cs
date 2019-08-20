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
        
        private List<SelectListItem> GetUserList()
        {
            List<SelectListItem> UserList = new List<SelectListItem>();

            foreach (User x in Repository.Users.OrderBy(x => x.Username))
            {
                UserList.Add(new SelectListItem() { Text = x.Username });
            }
            return UserList;
        }

        private List<SelectListItem> GetPlaceList()
        {
            List<SelectListItem> PlaceList = new List<SelectListItem>();

            foreach (Place x in Repository.Places.OrderBy(x => x.Name))
            {
                PlaceList.Add(new SelectListItem() { Text = x.Name });
            }
            return PlaceList;
        }
        
        [Authorize(Roles = Globals.editGroup)]
        public ActionResult Index(string returnUrl)
        {
            if (returnUrl.IsNullOrEmpty()) returnUrl = Url.Action("EquipList", "Equip");

            return View(new CartIndexViewModel //отображаем корзину
            {
                Cart = GetCart(),
                ReturnUrl = returnUrl,
                Users = GetUserList(),
                Places = GetPlaceList()
            });
        }

        [HttpPost]
        public ActionResult Finish(CartIndexViewModel result)
        {
            var cart = GetCart();
            ViewBag.rurl = result.ReturnUrl;
            ViewBag.inf = 0; //сюда записываем результат оформления

            if (cart.Lines.IsNullOrEmpty()) //проверяем пустоту корзины
            {
                ModelState.AddModelError("EmptyCart", "Поле оформления не может быть пустым. Добавьте хотя бы один предмет");
                return View();
            }

            var usr = Repository.Users.ToList().FirstOrDefault(g => g.Username == result.User);
            if (usr == null) //проверяем, есть ли такой пользователь
            {
                ViewBag.rurl = Url.Action("Index");
                ModelState.AddModelError("EmptyCart", "Такой пользователь не найден. Укажите другого пользователя");
                return View();
            }

            var plc = Repository.Places.ToList().FirstOrDefault(g => g.Name == result.Place);
            if (plc == null) //проверяем, есть ли такое место
            {
                ViewBag.rurl = Url.Action("Index");
                ModelState.AddModelError("EmptyCart", "Указанное место в базе не обнаружено, укажите другое место");
                return View();
            }

            foreach (CartLine x in cart.Lines) //проверяем, все ли предметы безхозны
            {
                if (x.item.Username != null)
                {
                    ModelState.AddModelError("EmptyCart", "Некоторые предметы не удалось добавить, поскольку у них уже имеется пользователь");
                    return View();
                }
            }

            if (ModelState.IsValid)
            {
                foreach (CartLine x in cart.Lines)
                {
                    var act = new Models.Action(); //Создаём новое действие

                    act.ID_Act = Guid.NewGuid();
                    act.Whom = usr.ID_User;
                    act.What = x.item.ID_Item;
                    act.When = DateTime.Now;
                    act.Todo = (byte)Enums.Todo.InUse;
                    act.AdminID = Repository.getAdminID(User.Identity.Name);
                    act.Coment = result.Coment; //действие зарегистрировали

                    x.item.User = usr;  //назначаем нового пользователя предмету
                    x.item.Place1 = plc; //переписываем место предмету

                    Repository.UpdateItem(x.item);
                    Repository.CreateAct(act);
                }

                GetCart().Clear();
                ViewBag.inf = 1; //У - успех
                ViewBag.rurl = Url.Action("LogList", "Logs");
                return View();
            }

            return RedirectToAction("Index");//станица оформления(возвращается в случае непредвиденной ошибки)
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