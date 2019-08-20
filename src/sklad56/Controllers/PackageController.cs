using System;
using System.Linq;
using System.Web.Mvc;
using sklad56.Models;

namespace sklad56.Controllers
{
    public class PackageController : BaseController
    {
        //
        // GET: /Package/

        public ViewResult Index()
        {
            return View();
        }

        public ActionResult PackageList(Guid PackID, int page = 1)
        {
            var Pack = Repository.Packages.FirstOrDefault(x => x.ID_Pack == PackID);

            if (Pack != null)
            {
                var list = Repository.Items.Where(item => item.Belongs == PackID);

                var data = new PackageViewModel
                {
                    items = new PageableData<Item>(list.OrderBy(name => name.Itemname), page, Globals.itemsPerPage),
                    pack = Pack
                };

                return View(data);  //выводим список предметов, состоящих в комплекте
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = Globals.editGroup)]
        public ActionResult EditPack(Guid PackID)
        {
            Package editPack = Repository.Packages //ищем комплект в базе
                .FirstOrDefault(g => g.ID_Pack == PackID);

            if (editPack != null)
            {
                ViewBag.Edit = true;
                return View("Register", editPack); //страница редактирования комплекта
            }
            return RedirectToAction("EditList");
        }

        [Authorize(Roles = Globals.editGroup)]
        public ActionResult Register()
        {
            ViewBag.Edit = false;
            var newPack = new Package();
            return View(newPack);  //страница регистрации нового комплекта
        }

        [HttpPost]
        public ActionResult Register(Package pack, bool Edit = false)
        {
            var anyPack = Repository.Users.Any(p => string.Compare(p.Username, pack.Name) == 0);
            if (anyPack && !Edit)
            {
                ModelState.AddModelError("Name", "Комплект с таким именем уже существует");
            }

            if (string.IsNullOrWhiteSpace(pack.Name))
            {
                ModelState.AddModelError("Name", "Поле не может быть пустым!");
            }

            if (ModelState.IsValid)
            {
                if (Edit == false)
                {
                    pack.ID_Pack = Guid.NewGuid();
                    Repository.CreatePack(pack);
                }
                else Repository.UpdatePack(pack);
                return RedirectToAction("EditList");
            }

            ViewBag.Edit = Edit;
            return View(pack);  //станица регистрации с инвалидными полями 
        }

        [Authorize(Roles = Globals.editGroup)]
        public ActionResult EditList() 
        {
            var packs = Repository.Packages.ToList();
            return View(packs);  //список комплектов для редактирования
        }

        public ActionResult PlaceList(Guid PlaceID, int page = 1)
        {
            var Place = Repository.Places.FirstOrDefault(x => x.ID_Place == PlaceID);

            if (Place != null)
            {
                var list = Repository.Items.Where(item => item.Place == PlaceID);

                var data = new PlaceViewModel
                {
                    items = new PageableData<Item>(list.OrderBy(name => name.Itemname), page, Globals.itemsPerPage),
                    place = Place
                };

                return View(data);  //выводим список предметов, привязанных к данному месту
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = Globals.editGroup)]
        public RedirectResult DeletePack(Guid PackID, string returnUrl)
        {
            Package pack = Repository.Packages //ищем пользователя в базе
                .FirstOrDefault(g => g.ID_Pack == PackID);
            var defPack = Repository.Packages.FirstOrDefault(x => x.ID_Pack == Guid.Parse("11111111-1111-1111-1111-111111111111") ); //комплект по умолчанию

            if ((pack == null) || (PackID == defPack.ID_Pack)) return Redirect(returnUrl); //если комплект не найден или он зарезервирован, то завершаем метод

            var useItems = Repository.Items.Where(x => x.Belongs == PackID).ToList(); //список предметов в комплекте
            foreach (Item itm in useItems)
            {
                itm.Package = defPack;
                Repository.UpdateItem(itm); //отвязываем предметы от комплекта
            }

            Repository.RemovePack(PackID); //удаляем комплект из базы
            return Redirect(returnUrl);
        }
    }
}
