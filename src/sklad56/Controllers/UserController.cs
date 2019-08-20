using System;
using System.Linq;
using System.Web.Mvc;
using sklad56.Models;
using System.Collections.Generic;

namespace sklad56.Controllers
{
    public class UserController : BaseController
    {
        //
        // GET: /User/

        public ViewResult Index()
        {
            return View();
        }

        public ActionResult InCharge()
        {
            var list = new List<UserInCharge>();
            var data = Repository.InCharges.ToList();
            var notes = Repository.Items.Where(itm => itm.Cast == (byte)Enums.Cast.Notebook); //отобрали ноутбуки из всего барахла

            foreach (var x in data)
            {
                var usr = Repository.Users.ToList().FirstOrDefault(g => g.Username == x.Name); //ищем пользователя по имени в базе
                var itm = notes.ToList().FirstOrDefault(g => g.Serial.Substring(g.Serial.IndexOf("(") + 1, 4) == x.Number);//ищем ноутбук по номеру комплекта (числа в скобках)
                list.Add(new UserInCharge 
                { 
                    item = x.Number, user = x.Name, 
                    itemID = itm == null ? null : itm.ID_Item.ToString(),
                    userID = usr == null ? null : usr.ID_User.ToString(),
                }); //собираем список
            }
            return View(list); //отображаем список ответственных
        }

        public ActionResult UserCard(Guid? UserID, int Log = 0)
        {
            User User = Repository.Users //ищем пользователя в базе
                .FirstOrDefault(g => g.ID_User == UserID);

            ViewBag.Log = Log;

            if (User != null)
            {
                return View(User); //отображаем карточку пользователя
            }
            return RedirectToAction("UserList"); //если пользователь не найден - отображаем список пользователей
        }
        
        [ValidateInput(false)]  //Аттрибут отключает проверку (чтоб не возникало HttpRequestValidationException)
        public ActionResult UserList(int page = 1, string searchString = null)
        {
            ViewBag.Search = searchString;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var list = SearchEngine<User>.Search(searchString, Repository.Users.OrderBy(name => name.Username)).AsQueryable();
                var data = new PageableData<User>(list, page, Globals.itemsPerPage);
                return View(data); //выводим результаты поиска
            }
            else
            {
                var data = new PageableData<User>(Repository.Users.OrderBy(name => name.Username), page, Globals.itemsPerPage);
                return View(data);  //выводим список пользователей
            }
        }

        [Authorize(Roles = Globals.editGroup)]
        public ActionResult UserEditList(int page = 1) //список пользователей для редактирования
        {
            return View(new PageableData<User>(Repository.Users.OrderBy(name => name.Username), page, Globals.itemsPerPage));
        }

        [Authorize(Roles = Globals.editGroup)]
        public ActionResult EditUser(Guid UserID)
        {
            User editUser = Repository.Users //ищем пользователя в базе
                .FirstOrDefault(g => g.ID_User == UserID);

            if (editUser != null)
            {
                ViewBag.Edit = true;
                return View("RegisterUser", editUser); //страница редактирования пользователя
            }
            return RedirectToAction("EquipEditList");
        }
        
        [Authorize(Roles = Globals.editGroup)]
        public ActionResult RegisterUser()
        {
            ViewBag.Edit = false;
            var newUser = new User();
            return View(newUser);  //страница регистрации нового пользователя
        }

        [HttpPost]
        public ActionResult RegisterUser(User user, bool Edit = false)
        {
            var anyUser = Repository.Users.Any(p => string.Compare(p.Username, user.Username) == 0);
            if (anyUser && !Edit)
            {
                ModelState.AddModelError("Username", "Пользователь с таким именем уже существует");
            }

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                ModelState.AddModelError("Username", "Поле не может быть пустым!");
            }

            if (ModelState.IsValid)
            {
                if (Edit == false)
                {
                    user.ID_User = Guid.NewGuid();
                    Repository.CreateUser(user);
                }
                else Repository.UpdateUser(user);
                return RedirectToAction("UserEditList");
            }

            ViewBag.Edit = Edit;
            return View(user);  //станица регистрации с инвалидными полями 
        }

        [Authorize(Roles = Globals.editGroup)]
        public RedirectResult DeleteUser(Guid UserID, string returnUrl)
        {
            User User = Repository.Users //ищем пользователя в базе
                .FirstOrDefault(g => g.ID_User == UserID);
            if ((User == null) || (User.IsAdmin)) return Redirect(returnUrl); //если пользователь не найден или он - админ, то завершаем метод

            var useItems = Repository.Items.Where(x => x.Username == UserID).ToList();
            foreach (Item itm in useItems)
            {
                itm.User = null;
                Repository.UpdateItem(itm); //отвязываем предметы от пользователя
            }

            Repository.RemoveUser(UserID); //удаляем пользователя из базы
            return Redirect(returnUrl);
        }
    }
}
