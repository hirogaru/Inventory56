using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using sklad56.Models;
using sklad56.Tools;
using System.Web.Security;
using System.Runtime.InteropServices;

namespace sklad56.Controllers
{

    public class EquipController : BaseController
    {
        //
        // GET: /Equip/

        public ViewResult Index()
        {
            return View();
        }

        //Формируем листы для страниц регистрации
        private IEnumerable<SelectListItem> PlaceList
        {
            get
            {
                foreach (Place x in Repository.Places)
                {
                    yield return new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.ID_Place.ToString()
                    };
                }

            }
        }

        private IEnumerable<SelectListItem> PackageList
        {
            get
            {
                foreach (Package x in Repository.Packages)
                {
                    yield return new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.ID_Pack.ToString()
                    };
                }
            }
        }

        private List<SelectListItem> GetUserList()
        {
            List<SelectListItem> UserList = new List<SelectListItem>();

            foreach (User x in Repository.Users)
            {
                UserList.Add(new SelectListItem() { Text = x.Username });
            }
            return UserList;
        }

        private IEnumerable<SelectListItem> CastList
        {
            get
            {
                for (byte i = 0; i <= Enums.castCount; i++)
                {
                    yield return new SelectListItem()
                    {
                        Text = i.ToEnumTyp(),
                        Value = i.ToString()
                    };
                }
            }
        }

        [Authorize(Roles = Globals.editGroup)]
        public ActionResult EditItem(Guid ItemID)
        {
            Item editItem = Repository.Items //ищем предмет в базе
                .FirstOrDefault(g => g.ID_Item == ItemID);

            if (editItem != null)
            {
                //Закидываем во вью подготовленные листы
                ViewData["PlaceList"] = PlaceList;
                ViewData["PackageList"] = PackageList;
                ViewData["CastList"] = CastList;

                ViewBag.Edit = true;
                return View("RegisterItem", editItem); //страница редактирования предмета
            }
            return RedirectToAction("EquipEditList");
        }

        [Authorize(Roles = Globals.editGroup)]
        public ActionResult RegisterItem()
        {
            ViewBag.Edit = false;
            //Закидываем во вью подготовленные листы
            ViewData["PlaceList"] = PlaceList;
            ViewData["PackageList"] = PackageList;
            ViewData["CastList"] = CastList;

            var newItem = new Item();
            return View(newItem);  //страница регистрации нового предмета
        }

        [HttpPost]
        public ActionResult RegisterItem(Item item, bool Edit = false)
        {

            if (string.IsNullOrWhiteSpace(item.Itemname))
            {
                ModelState.AddModelError("Itemname", "Поле не может быть пустым!");
            }

            if (string.IsNullOrWhiteSpace(item.Serial))
            {
                ModelState.AddModelError("Serial", "Поле не может быть пустым!");
            }

            var anyItemN = Repository.Items.Any(p => string.Compare(p.Itemname, item.Itemname) == 0);
            var anyItemS = Repository.Items.Any(p => string.Compare(p.Serial, item.Serial) == 0);
            //проверяем по совпадении имени и серийника, но можно заменить на другие проверки
            if (anyItemN && anyItemS && !Edit)
            {
                ModelState.AddModelError("Serial", "Такой предмет с таким же серийным номером уже существует");
            }

            if (ModelState.IsValid)
            {
                if (Edit == false)
                {
                    item.ID_Item = Guid.NewGuid();
                    Repository.CreateItem(item);
                }
                else Repository.UpdateItem(item);
                return RedirectToAction("EquipEditList");
            }

            ViewData["PlaceList"] = PlaceList;
            ViewData["PackageList"] = PackageList;
            ViewData["CastList"] = CastList;

            ViewBag.Edit = Edit;
            return View(item);  //станица регистрации с инвалидными полями
        }

        public ActionResult EquipItem(Guid? ItemID)
        {
            Item item = Repository.Items //ищем предмет в базе
                .FirstOrDefault(g => g.ID_Item == ItemID);

            if (item != null)
            {
                return View(item); //отображаем карточку предмета
            }
            return RedirectToAction("EquipList"); //если предмет не найден - отображаем список предметов
        }

        [ValidateInput(false)]  //по аналогии с UserController, но с сортировкой по типам
        public ActionResult EquipList(int page = 1, int sorted = 0, string searchString = null)
        {
            //Выводим итемы по их типу (если sorted = 0 то выводим всё подряд)
            var typeItems = sorted != 0 ? Repository.Items.Where(itm => itm.Cast == sorted).OrderBy(name => name.Itemname) : Repository.Items.OrderBy(name => name.Itemname);

            ViewBag.Search = searchString;
            ViewBag.Sorted = sorted;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var list = SearchEngine<Item>.Search(searchString, typeItems).AsQueryable();
                var data = new PageableData<Item>(list, page, Globals.itemsPerPage);
                return View(data); //выводим результаты поиска
            }
            else
            {
                var data = new PageableData<Item>(typeItems, page, Globals.itemsPerPage);
                return View(data); //отображаем список предметов
            }
        }

        [Authorize(Roles = Globals.editGroup)]
        public ActionResult EquipEditList(int page = 1, int sorted = 0)
        {
            //сортируем итемы по их типу (если sorted = 0 то выводим всё подряд)
            var typeItems = sorted != 0 ? Repository.Items.Where(itm => itm.Cast == sorted).OrderBy(name => name.Itemname) : Repository.Items.OrderBy(name => name.Itemname);
            ViewBag.Sorted = sorted;
            ViewBag.castCount = Enums.castCount;

            return View(new PageableData<Item>(typeItems, page, Globals.itemsPerPage));//Выводим список предметов на редактирование
        }

        [ChildActionOnly]
        public ActionResult UserEquipList(Guid UserId)
        {
            //TODO : переписать под JOIN
            var data = Repository.Items.Where(x => x.Username == UserId).ToList();
            List<EquipListViewModel> datoz = new List<EquipListViewModel>();

            foreach (var x in data)
            {
                var act = Repository.Actions.ToList().LastOrDefault(g => (g.What == x.ID_Item) && (g.Todo == 1)); //ищем последнее действие принятия, связанное с предметом

                if (act != null) datoz.Add(new EquipListViewModel { item = x, date = act.When });
                else datoz.Add(new EquipListViewModel { item = x, date = null });
            }
            return PartialView(datoz); //список предметов, закреплённых за пользователем
        }

        [Authorize(Roles = Globals.editGroup)]
        public RedirectResult DeleteItem(Guid ItemID, string returnUrl)
        {
            Item item = Repository.Items //ищем предмет в базе
                .FirstOrDefault(g => g.ID_Item == ItemID);
            if (item == null) return Redirect(returnUrl); //если предмет не найден, то завершаем метод

            item.User = null;

            Repository.RemoveItem(ItemID); //удаляем предмет из базы
            return Redirect(returnUrl);
        }

        [Authorize(Roles = Globals.editGroup)]
        public ActionResult EditVeryfi(Guid ItemID, string returnUrl)
        {
            Item item = Repository.Items //ищем предмет в базе
                .FirstOrDefault(g => g.ID_Item == ItemID);

            DateTime dateV = item.Verifi != null ? (DateTime)item.Verifi : DateTime.Now;

            return View(new EditVeryfiViewModel()
            {
                returnUrl = returnUrl,
                Day = dateV.Day,
                Month = dateV.Month,
                Year = dateV.Year,
                ItemID = ItemID,
                Itemname = item.Itemname
            }); //Показываем дату поверки предмета для редактирования
        }

        [HttpPost]
        public RedirectResult EditVeryfi(EditVeryfiViewModel mdl)
        {
            Item item = Repository.Items //ищем предмет в базе
                .FirstOrDefault(g => g.ID_Item == mdl.ItemID);

            item.Verifi = new DateTime(mdl.Year, mdl.Month, mdl.Day);
            Repository.UpdateItem(item); //устанавливаем новую дату поверки

            return Redirect(mdl.returnUrl);
        }

        [Authorize(Roles = Globals.editGroup)]
        public RedirectResult DeleteVeryfi(Guid ItemID, string returnUrl)
        {
            Item item = Repository.Items //ищем предмет в базе
                .FirstOrDefault(g => g.ID_Item == ItemID);
            item.Verifi = null;
            Repository.UpdateItem(item); //удаляем дату поверки
            return Redirect(returnUrl);
        }

        /// <summary>
        /// Методы для движения ТМЦ по складу
        /// </summary>
        [Authorize(Roles = Globals.editGroup)]
        public RedirectResult ReturnItem(Guid ItemID, string returnUrl, bool broke = false, bool lose = false) //возвращение предмета на склад
        {
            Item itm = Repository.Items.First(g => g.ID_Item == ItemID);

            if ((itm != null) && (itm.Username != null))
            {
                var act = new Models.Action() //действие зарегистрировали
                {
                    ID_Act = Guid.NewGuid(),
                    Whom = (Guid)itm.Username,
                    What = itm.ID_Item,
                    When = DateTime.Now,
                    Todo = (byte)Enums.Todo.Return,
                    AdminID = Repository.getAdminID(User.Identity.Name),
                    Coment = ""
                };
                if (broke) act.Todo = (byte)Enums.Todo.breaked;
                if (lose) act.Todo = (byte)Enums.Todo.lost;
                Repository.CreateAct(act); //оставить запись об возвращении предмета

                itm.User = null; //убрать пользователя
                if (lose) itm.Place1 = Repository.Places.Single(x => x.ID_Place.ToString() == "44444444-4444-4444-4444-444444444444"); //потерять предмет если надо
                if (broke) itm.Broken = true; //а если надо сломать - то сломать)
                Repository.UpdateItem(itm);
            }
            return Redirect(returnUrl);
        }

        [Authorize(Roles = Globals.editGroup)]
        public ActionResult ReturnList(int page = 1)
        {
            var sortedItems = Repository.Items.Where(itm => itm.Username != null).OrderBy(name => name.Itemname).AsQueryable(); //создаём список предметов на руках у пользователей
            return View(new PageableData<Item>(sortedItems, page, Globals.itemsPerPage)); //отображаем список
        }

        [Authorize(Roles = Globals.editGroup)]
        public ActionResult HandOn(Guid? ItemID)
        {
            var itm = Repository.Items.FirstOrDefault(x => x.ID_Item == ItemID);
            if (itm != null)
            {
                var mdl = new HandOnViewModel()
                            {
                                users = GetUserList(),
                                item = itm
                            };

                return View(mdl); //отображаем страницу передачи предмета
            }

            return RedirectToAction("ReturnList");
        }

        [HttpPost]
        public ActionResult HandOn(HandOnViewModel result, Guid ItemID)
        {
            result.item = Repository.Items.FirstOrDefault(x => x.ID_Item == ItemID);
            if (result.item == null)
            {
                return RedirectToAction("ReturnList");
            }

            var usr = Repository.Users.FirstOrDefault(g => g.Username == result.user);
            if (usr == null) //проверяем, есть ли такой пользователь
            {
                ModelState.AddModelError("NotFound", "Такой пользователь не найден");
                result.users = GetUserList();
                return View(result);
            }

            var act = new Models.Action() //действие зарегистрировали
            {
                ID_Act = Guid.NewGuid(),
                Whom = (Guid)result.item.Username,
                What = result.item.ID_Item,
                When = DateTime.Now,
                Todo = (byte)Enums.Todo.Handed,
                AdminID = Repository.getAdminID(User.Identity.Name),
                Coment = "На руки " + usr.Username
            };
            Repository.CreateAct(act); //оставить запись о передаче предмета

            act = new Models.Action()
            {
                ID_Act = Guid.NewGuid(),
                Whom = usr.ID_User,
                What = result.item.ID_Item,
                When = DateTime.Now,
                Todo = (byte)Enums.Todo.InUse,
                AdminID = Repository.getAdminID(User.Identity.Name),
                Coment = "Взял у " + result.item.User.Username
            };
            Repository.CreateAct(act); //запись о том, кто предмет взял

            result.item.User = usr;  //назначаем нового пользователя предмету
            Repository.UpdateItem(result.item);

            return RedirectToAction("LogList", "Logs");
        }
    }
}
