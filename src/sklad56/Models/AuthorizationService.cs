using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sklad56.Models
{
    public class MyAuthorizationService
    {
        /// <summary>
        /// Класс для проверки логина и пароля пользователя
        /// C вводом доменной авторизации стал ненужен
        /// </summary>

        private IRepository _Repository;

        public MyAuthorizationService(IRepository repository)
        {
            _Repository = repository;
        }

        public virtual bool ValidateUser(string username, string password)
        {
            LogoPass retUser = _Repository.LogoPasses.FirstOrDefault(p => string.Compare(p.Login, username, true) == 0 && p.Password == password);
            
            if (retUser != null)
            {
                return true;
            }
            return false;
        }
    }

    public class AuthorizationModel
    {
        //Модель для представления процесса залогинивания (при доменной авторизации не нужна)

        public string Login { get; set; }

        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class PartialUserInfo
    {
        //Инфо для партиал блока

        public string Name { get; set; }
    }
}