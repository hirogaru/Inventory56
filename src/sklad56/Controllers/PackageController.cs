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

        [HttpGet]
        public ActionResult Register()
        {
            var newPack = new Package();
            return View(newPack);  //страница регистрации нового комплекта
        }

        [HttpPost]
        public ActionResult Register(Package pack)
        {
            var anyPack = Repository.Users.Any(p => string.Compare(p.Username, pack.Name) == 0);
            if (anyPack)
            {
                ModelState.AddModelError("Name", "Комплект с таким именем уже существует");
            }

            if (string.IsNullOrWhiteSpace(pack.Name))
            {
                ModelState.AddModelError("Name", "Поле не может быть пустым!");
            }

            if (ModelState.IsValid)
            {
                pack.ID_Pack = Guid.NewGuid();
                Repository.CreatePack(pack);
                return RedirectToAction("EditList");
            }
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
    }
}
