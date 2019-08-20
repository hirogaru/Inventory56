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

        public ActionResult PackageList(Guid? PackID, int page = 1)
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

        public ActionResult PlaceList(Guid? PlaceID, int page = 1)
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
        
        public ViewResult Packs()
        {
            var packs = Repository.Packages.OrderBy(pck => pck.Name).ToList();
            return View(packs);  //список комплектов
        }

        public ViewResult Places()
        {
            var places = Repository.Places.OrderBy(plc => plc.Name).ToList();
            return View(places);  //список мест
        }
        
        [Authorize(Roles = Globals.editGroup)]
        public ActionResult EditPack(Guid? PackID)
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
        public ViewResult Register()
        {
            ViewBag.Edit = false;
            var newPack = new Package();
            return View(newPack);  //страница регистрации нового комплекта
        }

        [HttpPost]
        public ActionResult Register(Package pack, bool Edit = false)
        {
            var anyPack = Repository.Packages.Any(p => string.Compare(p.Name, pack.Name) == 0);
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
        public ViewResult EditList() 
        {
            var packs = Repository.Packages.OrderBy(pck => pck.Name).ToList();
            return View(packs);  //список комплектов для редактирования
        }
        
        [Authorize(Roles = Globals.editGroup)]
        public RedirectResult DeletePack(Guid PackID, string returnUrl)
        {
            Package pack = Repository.Packages //ищем комплект в базе
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

        [Authorize(Roles = Globals.editGroup)]
        public ViewResult EditPlaceList()
        {
            var places = Repository.Places.OrderBy(plc => plc.Name).ToList();
            return View(places);  //список мест для редактирования
        }

        [Authorize(Roles = Globals.editGroup)]
        public ViewResult RegisterPlace()
        {
            ViewBag.Edit = false;
            var newPlace = new Place();
            return View(newPlace);  //страница регистрации нового места
        }

        [Authorize(Roles = Globals.editGroup)]
        public ActionResult EditPlace(Guid? PlaceID)
        {
            Place editPlace = Repository.Places //ищем местo в базе
                .FirstOrDefault(g => g.ID_Place == PlaceID);

            if (editPlace != null)
            {
                ViewBag.Edit = true;
                return View("RegisterPlace", editPlace); //страница редактирования места
            }
            return RedirectToAction("EditPlaceList");
        }

        [HttpPost]
        public ActionResult RegisterPlace(Place place, bool Edit = false)
        {
            var anyPlace = Repository.Places.Any(p => string.Compare(p.Name, place.Name) == 0);
            if (anyPlace && !Edit)
            {
                ModelState.AddModelError("Name", "Место с таким именем уже существует");
            }

            if (string.IsNullOrWhiteSpace(place.Name))
            {
                ModelState.AddModelError("Name", "Поле не может быть пустым!");
            }

            if (string.IsNullOrWhiteSpace(place.City))
            {
                ModelState.AddModelError("City", "Поле не может быть пустым!");
            }

            if (ModelState.IsValid)
            {
                if (Edit == false)
                {
                    place.ID_Place = Guid.NewGuid();
                    Repository.CreatePlace(place);
                }
                else Repository.UpdatePlace(place);
                return RedirectToAction("EditPlaceList");
            }

            ViewBag.Edit = Edit;
            return View(place);  //станица регистрации с инвалидными полями 
        }

        [Authorize(Roles = Globals.editGroup)]
        public RedirectResult DeletePlace(Guid PlaceID, string returnUrl)
        {
            Place place = Repository.Places //ищем место в базе
                .FirstOrDefault(g => g.ID_Place == PlaceID);
            var defPlace = Repository.Places.FirstOrDefault(x => x.ID_Place == Guid.Parse("44444444-4444-4444-4444-444444444444")); //место по умолчанию

            if ((place == null) || (PlaceID == defPlace.ID_Place)) return Redirect(returnUrl); //если место не найдено или оно зарезервировано, то завершаем метод

            var useItems = Repository.Items.Where(x => x.Place == PlaceID).ToList(); //список предметов на месте
            foreach (Item itm in useItems)
            {
                itm.Place1 = defPlace;
                Repository.UpdateItem(itm); //отвязываем предметы от места
            }

            Repository.RemovePlace(PlaceID); //удаляем место из базы
            return Redirect(returnUrl);
        }
    }
}
