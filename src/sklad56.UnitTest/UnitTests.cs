using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using sklad56;
using sklad56.Controllers;
using sklad56.Models;
using System.Linq;
using System.Collections.Generic;

namespace sklad56.UnitTest
{
    /// <summary>
    /// Чтобы не прибегать к помощи различных DependencyResolver'ов достаточно в BaseController раскоментить один аксесор и закоментить другой
    /// </summary>
    
    [TestClass]
    public class EquipControllerTest
    {
        [TestMethod]
        public void EquipIndexTest()
        {
            EquipController controller = new EquipController();
            ViewResult view = controller.Index();
            Assert.AreEqual("", view.ViewName); //проверяем по имени результата
        }

        [TestMethod]
        public void EquipListTest()
        {
            EquipController controller = new EquipController();
            ViewResult result = controller.EquipList(page: 1, sorted: 2, searchString: "бор", itemsPerPage: 50);
            Assert.AreEqual("", result.ViewName); //проверяем по имени результата
            Assert.AreEqual(2, result.ViewBag.Sorted);//проверяем вьюбэг
            Assert.AreEqual("бор", result.ViewBag.Search);
            Assert.AreEqual(50, result.ViewBag.ItemsPage);
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(PageableData<Item>)); //проверяем по типу модели
        }

        [TestMethod]
        public void EquipItemViewTest()
        {
            EquipController controller = new EquipController();
            Guid ItemID = Guid.Parse("14444444-4444-4444-4444-444444444444");
            ActionResult result = controller.EquipItem(ItemID);
            Assert.IsInstanceOfType(result, typeof(ViewResult)); //проверяем, результ это вью?
            
            ViewResult view = result as ViewResult;
            Assert.AreEqual("", view.ViewName); //проверяем по имени результата
            Assert.IsInstanceOfType(view.ViewData.Model, typeof(Item)); //проверяем по типу модели
        }

        [TestMethod]
        public void EquipItemRedirectTest()
        {
            EquipController controller = new EquipController();
            Guid ItemID = Guid.NewGuid();
            ActionResult result = controller.EquipItem(ItemID);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));//проверяем, результ это перенаправление?
            
            RedirectToRouteResult redirect = result as RedirectToRouteResult;
            Assert.AreEqual(redirect.RouteValues["action"], "EquipList"); //проверяем, куда перенаправили
        }

        [TestMethod]
        public void Equip4UserList()
        {
            EquipController controller = new EquipController();
            ActionResult result = controller.UserEquipList(default(Guid));
            Assert.IsInstanceOfType(result, typeof(PartialViewResult)); //проверяем, результ - партиал?

            PartialViewResult view = result as PartialViewResult;
            Assert.AreEqual("", view.ViewName); //проверяем по имени результата
            Assert.IsInstanceOfType(view.ViewData.Model, typeof(List<EquipListViewModel>)); //проверяем по типу модели
        }
    }

    [TestClass]
    public class UserControllerTest
    {
        [TestMethod]
        public void UserIndexTest()
        {
            UserController controller = new UserController();
            ViewResult view = controller.Index();
            Assert.AreEqual("", view.ViewName); //проверяем по имени результата
        }

        [TestMethod]
        public void UserListTest()
        {
            UserController controller = new UserController();
            ViewResult result = controller.UserList(page: 1,sorted: 1, itemsPerPage: 50, searchString: "ов");
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("ов", result.ViewBag.Search); //проверяем вьюбэг
            Assert.AreEqual(1, result.ViewBag.Sorted);
            Assert.AreEqual(50, result.ViewBag.ItemsPage);
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(PageableData<User>)); //проверяем по типу модели
        }

        [TestMethod]
        public void UserCardViewTest()
        {
            UserController controller = new UserController();
            Guid UserID = Guid.Parse("14444444-4444-4444-4444-444444444444");
            ActionResult result = controller.UserCard(UserID,Log: 1);
            Assert.IsInstanceOfType(result, typeof(ViewResult)); //проверяем, результ это вью?
            
            ViewResult view = result as ViewResult;
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(1, view.ViewBag.Log);//проверяем вьюбэг
            Assert.IsInstanceOfType(view.ViewData.Model, typeof(User));
        }

        [TestMethod]
        public void UserCardRedirectTest()
        {
            UserController controller = new UserController();
            Guid UserID = Guid.NewGuid();
            ActionResult result = controller.UserCard(UserID);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            
            RedirectToRouteResult redirect = result as RedirectToRouteResult;
            Assert.AreEqual(redirect.RouteValues["action"], "UserList"); //проверяем, куда перенаправили
        }
    }

    [TestClass]
    public class LogsControllerTest
    {
        [TestMethod]
        public void LogsIndexTest()
        {
            var controller = new LogsController();
            var view = controller.Index();
            Assert.AreEqual("", view.ViewName);
        }

        [TestMethod]
        public void LogListTest()
        {
            var controller = new LogsController();
            var result = controller.LogList(page: 1, sort: 4, itemsPerPage: 50);
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(4, result.ViewBag.sort);
            Assert.AreEqual(50, result.ViewBag.ItemsPage);
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(PageableData<sklad56.Models.Action>));
        }

        [TestMethod]
        public void LogItemsTest()
        {
            var controller = new LogsController();
            var result = controller.itemLog(default(Guid));
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            
            PartialViewResult view = result as PartialViewResult;
            Assert.AreEqual("", view.ViewName);
            Assert.IsInstanceOfType(view.ViewData.Model, typeof(List<sklad56.Models.Action>));
        }

        [TestMethod]
        public void LogUsersTest()
        {
            var controller = new LogsController();
            var result = controller.UserLog(default(Guid));
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            
            PartialViewResult view = result as PartialViewResult;
            Assert.AreEqual("", view.ViewName);
            Assert.IsInstanceOfType(view.ViewData.Model, typeof(List<sklad56.Models.Action>));
        }
    }

    [TestClass]
    public class PackageControllerTest
    {
        /// <summary>
        /// Две аналогичные вьюшки. Проверяем одновременно т.к. поведение идентично(найти запись-сформировать модель-показать вью или показать индекс если запись не найдена)
        /// </summary>
        [TestMethod]
        public void PackageListsViewTest()
        {
            var controller = new PackageController();
            var ID = Guid.Parse("14444444-4444-4444-4444-444444444444"); 
            ActionResult result1 = controller.PackageList(ID, page: 1); //страница комплекта
            Assert.IsInstanceOfType(result1, typeof(ViewResult));
            ViewResult view1 = result1 as ViewResult;
            Assert.AreEqual("", view1.ViewName);
            Assert.IsInstanceOfType(view1.ViewData.Model, typeof(PackageViewModel));

            var result2 = controller.PlaceList(ID, page: 1); //страница места
            Assert.IsInstanceOfType(result2, typeof(ViewResult));
            var view2 = result2 as ViewResult;
            Assert.AreEqual("", view2.ViewName);
            Assert.IsInstanceOfType(view2.ViewData.Model, typeof(PlaceViewModel));
        }

        [TestMethod]
        public void PackageListsRedirectTest()
        {
            var controller = new PackageController();
            var ID = Guid.NewGuid();
            var result1 = controller.PackageList(ID);
            Assert.IsInstanceOfType(result1, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirect1 = result1 as RedirectToRouteResult;
            Assert.AreEqual(redirect1.RouteValues["action"], "Index");

            var result2 = controller.PlaceList(ID);
            Assert.IsInstanceOfType(result2, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirect2 = result2 as RedirectToRouteResult;
            Assert.AreEqual(redirect2.RouteValues["action"], "Index");
        }

        [TestMethod]
        public void PackageIndexTest()
        {
            var controller = new PackageController();
            var view = controller.Index();
            Assert.AreEqual("", view.ViewName);
        }

        /// <summary>
        /// Две одинаковые страницы списков. Поведение одинаково, поэтому проверяются в одном методе
        /// </summary>
        [TestMethod]
        public void PacksTest()
        {
            var controller = new PackageController();
            var view1 = controller.Packs(); //список комплектов
            Assert.AreEqual("", view1.ViewName); 
            Assert.IsInstanceOfType(view1.ViewData.Model, typeof(List<Package>));
            
            var view2 = controller.Places();
            Assert.AreEqual("", view2.ViewName); //список мест
            Assert.IsInstanceOfType(view2.ViewData.Model, typeof(List<Place>));
        }
    }

    [TestClass]
    public class OtherControllerTest
    {
        /// <summary>
        /// чтоб не создавать отдельный класс здесь собраны тесты методов разых контроллеров
        /// </summary>
        
        [TestMethod]
        public void HomeIndexTest()
        {
            var controller = new HomeController();
            var view = controller.Index();
            Assert.AreEqual("", view.ViewName);
            Assert.IsInstanceOfType(view.ViewData.Model, typeof(IndexModelView));
        }

        [TestMethod]
        public void FooterViewTest()
        {
            var controller = new AccountController();
            var result = controller.Footer();
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            PartialViewResult view = result as PartialViewResult;
            Assert.AreEqual("Footer", view.ViewName);
            Assert.IsInstanceOfType(view.ViewData.Model, typeof(PartialUserInfo));
        }
    }
    
    [TestClass]
    public class ModelsTest
    {
        [TestMethod]
        public void CartTest()
        {
            Item item1 = new Item { ID_Item = Guid.NewGuid(), Itemname = "Предмет1" }; //создание нескольких тестовых итемов
            Item item2 = new Item { ID_Item = Guid.NewGuid(), Itemname = "Предмет2" };
            Item item3 = new Item { ID_Item = Guid.NewGuid(), Itemname = "Предмет3" };

            Cart cart = new Cart(); //создание корзины
            cart.AddItem(item1, 1);
            cart.AddItem(item2, 1);
            cart.AddItem(item3, 1);
            cart.RemoveLine(item3); //удаляем один предмет
            List<CartLine> results = cart.Lines.ToList();
            int calc = cart.ComputeTotalValue();

            Assert.AreEqual(results.Count(), 2); //проверяем, что в корзине
            Assert.AreEqual(results[0].item, item1);
            Assert.AreEqual(results[1].item, item2);
            Assert.AreEqual(cart.Lines.Where(c => c.item == item3).Count(), 0); //проверяем, что предмет удалён
            Assert.AreEqual(calc, 2); //проверяем счётчик предметов
        }

        [TestMethod]
        public void PageDataTest()
        {
            var data = new List<Item>();
            for (var i=0; i<6; i++) //создаём тестовый список
            {
                data.Add(new Item {ID_Item = Guid.NewGuid(), Itemname = "Item"+i.ToString() });
            }
            var pd = new PageableData<Item>(data.AsQueryable(), 1, 5);

            Assert.AreEqual(pd.CountPage, 2); //проверяем кол-во страниц
            Assert.AreEqual(pd.List.Count(), 5); //проверяем кол-во элементов на странице
            Assert.AreEqual(pd.List.ToList()[4].Itemname, "Item4"); //проверяем последний элемент списка
        }

        [TestMethod]
        public void SearchTest()
        {
            var data = new List<Item>();
            for (var i = 28; i < 34; i++) //создаём тестовый список
            {
                data.Add(new Item { ID_Item = Guid.NewGuid(), Itemname = "Item" + i.ToString() });
            }
            var list = SearchEngine<Item>.Search("<a>ItEm3", data.AsQueryable()); //проверяем поиск(должно удалять теги и быть нечуствительным к регистру)

            Assert.AreEqual(list.Count(), 4); //кол-во найденых итемов
            Assert.AreEqual(list.ToList()[0].Itemname, "Item30"); //проверяем первый найденый итем
        }
    }
}
