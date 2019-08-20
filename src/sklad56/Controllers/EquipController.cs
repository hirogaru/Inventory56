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

        //Формируем листы страницы регистрации
        private List<SelectListItem> GetPlaceList()
        {
            List<SelectListItem> PlaceList = new List<SelectListItem>();

            foreach(Place x in Repository.Places)
            {
                PlaceList.Add(new SelectListItem(){Text = x.Name, Value = x.ID_Place.ToString()});
            }
            return PlaceList;
        }

        private List<SelectListItem> GetPackageList()
        {
            List<SelectListItem> PackageList = new List<SelectListItem>();

            foreach (Package x in Repository.Packages)
            {
                PackageList.Add(new SelectListItem() { Text = x.Name, Value = x.ID_Pack.ToString() });
            }
            return PackageList;
        }

        private List<SelectListItem> GetCastList()
        {
            List<SelectListItem> CastList = new List<SelectListItem>();

            for (byte i = 0; i <= Enums.castCount; i++)
            {
                CastList.Add(new SelectListItem() { Text = i.ToEnumTyp(), Value = i.ToString() });
            }
            return CastList;
        }

        [HttpGet]
        public ActionResult RegisterItem()
        {
            //Закидываем во вью подготовленные листы
            ViewData["PlaceList"] = GetPlaceList();
            ViewData["PackageList"] = GetPackageList();
            ViewData["CastList"] = GetCastList();

            var newItem = new Item();
            return View(newItem);  //страница регистрации нового предмета
        }

        [HttpPost]
        public ActionResult RegisterItem(Item item)
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
            if (anyItemN && anyItemS)
            {
                ModelState.AddModelError("Serial", "Такой предмет с таким же серийным номером уже существует");
            }

            if (ModelState.IsValid)
            {
                item.ID_Item = Guid.NewGuid();
                Repository.CreateItem(item);
                return RedirectToAction("EquipEditList");
            }

            ViewData["PlaceList"] = GetPlaceList();
            ViewData["PackageList"] = GetPackageList();
            ViewData["CastList"] = GetCastList();
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

        [Authorize(Roles = Globals.editGroup)]
        public ViewResult ReturnList(int page = 1) 
        {
            var sortedItems = Repository.Items.Where(itm => itm.Username != null).OrderBy(name => name.Itemname).AsQueryable(); //создаём список предметов на руках у пользователей
            return View(new PageableData<Item>(sortedItems, page, Globals.itemsPerPage)); //отображаем список
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

    }
}
