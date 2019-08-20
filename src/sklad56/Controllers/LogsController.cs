using System;
using System.Linq;
using System.Web.Mvc;
using sklad56.Models;

namespace sklad56.Controllers
{
    public class LogsController : BaseController
    {
        //
        // GET: /Logs/

        public ViewResult Index()
        {
            return View();
        }

        public ViewResult LogList(int page = 1, int sort = 0)
        {
            ViewBag.sort = sort;
            IQueryable<Models.Action> actions;
            switch (sort)
            {
                case 1:
                    actions = Repository.Actions.OrderBy(name => name.User.Username);
                    break;
                case 2:
                    actions = Repository.Actions.OrderBy(name => name.Item.Itemname);
                    break;
                case 3:
                    actions = Repository.Actions.OrderBy(name => name.Todo);
                    break;
                case 4:
                    actions = Repository.Actions.OrderBy(name => name.When);
                    break;
                default:
                    actions = Repository.Actions.OrderByDescending(name => name.When);
                    break;
            }
            var data = new PageableData<Models.Action>(actions, page, Globals.itemsPerPage);
            return View(data);  //выводим лог список 
        }

        [ChildActionOnly]
        public ActionResult itemLog(Guid ItemID)
        {
            var actions = Repository.Actions.Where( x => x.What == ItemID).OrderByDescending(name => name.When).ToList();
            return PartialView(actions); //выводим все действия связанные с предметом
        }

        [ChildActionOnly]
        public ActionResult UserLog(Guid UserID)
        {
            var actions = Repository.Actions.Where(x => x.Whom == UserID).OrderByDescending(name => name.When).ToList();
            return PartialView(actions); //выводим все действия конкретного пользователя
        }

        [Authorize(Roles = Globals.editGroup)]
        public RedirectToRouteResult DeleteAction(Guid ActID)
        {
            Repository.RemoveAct(ActID); //удаляем действие из базы
            return RedirectToAction("LogList");
        }
    }
}
