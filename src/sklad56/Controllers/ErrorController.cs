using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace sklad56.Controllers
{
    public class ErrorController : Controller //Контроллер ошибок
    {
        //
        // GET: /Error/

        public ViewResult Error()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return View(); //Отображаем простейшую страницу ошибки
        }

        public ViewResult NotFoundPage()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View(); //Отображаем ошибку 404
        }
    }
}
