using System;
using System.Linq;
using System.Web.Mvc;
using sklad56.Models;

namespace sklad56.Controllers
{
    //Базовый контроллер, от него наследуются все контроллеры, которым требуется работа с SQL репозиторием
    public abstract class BaseController : Controller  
    {
        public IRepository Repository
        {
            //get { return new TestRepository(); }
            get { return Service<IRepository>.GetInstance(); }
        }
    }

    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ViewResult Index()
        {
            var text = Repository.Miscs.FirstOrDefault(x => x.Key == "Start_Page_Text") ?? new Misc(); //ищем в базе текст для главной страницы
            var header = Repository.Miscs.FirstOrDefault(x => x.Key == "Start_Page_Header") ?? new Misc(); //ищем в базе заголовок

            return View(new IndexModelView(header.Value, text.Value));
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
