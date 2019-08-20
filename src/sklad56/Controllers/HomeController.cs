using System;
using System.Web.Mvc;
using sklad56.Models;

namespace sklad56.Controllers
{
    //Базовый контроллер, от него наследуются все контроллеры, которым требуется работа с SQL репозиторием
    public abstract class BaseController : Controller  
    {
        public IRepository Repository
        {
            get { return new TestRepository(); }
            //get { return Service<IRepository>.GetInstance(); }
        }
    }

    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        //[Authorize] - нет необходимости
        public ViewResult Index()
        {
            return View();
        }

        //Представления для редактирующих
        [Authorize(Roles = Globals.editGroup)]
        public ViewResult Editor()
        {
            return View();
        }

        [Authorize(Roles = Globals.editGroup)]
        public ViewResult Movements()
        {
            return View();
        }
    }

}
