using System;
using System.Web.Mvc;
using sklad56.Models;
using System.Web.Security;

namespace sklad56.Controllers
{
    public class AccountController : BaseController
    {
        //
        // GET: /Account/
        
        // Этот атрибут позволяет просматривать страничку /Account только авторизованным пользователям
        // если при обращении к ней вы не авторизованы - у вас запросят логин/пароль авторизаии
        [Authorize(Roles = Globals.editGroup)]
        public ViewResult Index()
        {
            return View();
        }

        //Экшн сгенерит футер с именем админа или покажет гостевой доступ
        [ChildActionOnly]
        public ActionResult Footer()
        {
            var userName = new PartialUserInfo();

            try 
            { userName.Name = Repository.getAdminName(User.Identity.Name); }
            catch (Exception)
            { userName.Name = "Гость"; }

            return PartialView("Footer", userName);  //отображаем информацию о пользователе в футер
        }

    }
}
