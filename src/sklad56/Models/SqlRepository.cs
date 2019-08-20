using System;
using System.Collections.Generic;
using System.Linq;

namespace sklad56.Models
{
    public interface IRepository
    {
        /// <summary>
        /// Доступ к данным, а также методы для создания, изменения и удаления этих данных.
        /// </summary>

        IQueryable<InCharge> InCharges { get; }

        IQueryable<DelData> DelDatas { get; }

        #region LogoPass

        IQueryable<LogoPass> LogoPasses { get; }

        string getAdminName(string login);

        #endregion 

        #region User

        IQueryable<User> Users { get; }

        bool CreateUser(User instance);

        bool UpdateUser(User instance);

        bool RemoveUser(System.Guid idUser);

        #endregion 
        
        #region Place

        IQueryable<Place> Places { get; }

        #endregion 

        #region Item

        IQueryable<Item> Items { get; }

        bool CreateItem(Item instance);

        bool UpdateItem(Item instance);

        bool RemoveItem(System.Guid idItem);

        #endregion 
        
        #region Action

        IQueryable<Action> Actions { get; }

        bool CreateAct(Action instance);

        bool RemoveAct(System.Guid idAct);

        #endregion 

        #region Package

        IQueryable<Package> Packages { get; }

        bool CreatePack(Package instance);

        bool UpdatePack(Package instance);

        bool RemovePack(System.Guid idPack);

        #endregion 

    }

    public class SqlRepository : IRepository
    {
        /// <summary>
        /// Реализация доступа ко всем таблицам, создание, удаление и изменение
        /// </summary>
        
        private readonly DataClassDataContext _context = new DataClassDataContext();
        public DataClassDataContext Db { get { return _context; } }

        public IQueryable<InCharge> InCharges
        {
            get
            {
                return Db.InCharges;
            }
        }

        public IQueryable<DelData> DelDatas
        {
            get
            {
                return Db.DelDatas;
            }
        }
        
        public IQueryable<LogoPass> LogoPasses
        {
            get
            {
                return Db.LogoPasses;
            }
        }

        public string getAdminName(string login)
        {
            login = login.Substring(Globals.DomainName.Length + 1); //отрезаем логин от домена
            try
            {
                System.Guid UsrID = (Db.LogoPasses.First(p => p.Login == login)).UserID;
                return (Db.Users.First(p => p.ID_User == UsrID)).Username;
            }
            catch
            {
                return "Пользователь";
            }
        }

        public IQueryable<User> Users
        {
            get
            {
                return Db.Users;
            }
        }

        public bool CreateUser(User instance)
        {
            if (instance.ID_User != Guid.Empty)
            {
                Db.Users.InsertOnSubmit(instance);
                Db.Users.Context.SubmitChanges();
                return true;
            }

            return false;
        }

        public bool UpdateUser(User instance)
        {
            User cache = Db.Users.Where(p => p.ID_User == instance.ID_User).FirstOrDefault();
            if (cache != null)
            {
                //TODO : Update fields for User
                Db.Users.Context.SubmitChanges();
                return true;
            }

            return false;
        }

        public bool RemoveUser(System.Guid idUser)
        {
            User instance = Db.Users.Where(p => p.ID_User == idUser).FirstOrDefault();
            if (instance != null)
            {
                Db.Users.DeleteOnSubmit(instance);
                Db.Users.Context.SubmitChanges();
                return true;
            }

            return false;
        }

        public IQueryable<Item> Items
        {
            get
            {
                return Db.Items;
            }
        }

        public bool CreateItem(Item instance)
        {
            if (instance.ID_Item != Guid.Empty)
            {
                Db.Items.InsertOnSubmit(instance);
                Db.Items.Context.SubmitChanges();
                return true;
            }

            return false;
        }

        public bool UpdateItem(Item instance)
        {
            Item cache = Db.Items.Where(p => p.ID_Item == instance.ID_Item).FirstOrDefault();
            if (cache != null)
            {
                //TODO : Update fields for Item
                Db.Items.Context.SubmitChanges();
                return true;
            }

            return false;
        }

        public bool RemoveItem(System.Guid idItem)
        {
            Item instance = Db.Items.Where(p => p.ID_Item == idItem).FirstOrDefault();
            if (instance != null)
            {
                Db.Items.DeleteOnSubmit(instance);
                Db.Items.Context.SubmitChanges();
                return true;
            }

            return false;
        }

        public IQueryable<Place> Places
        {
            get
            {
                return Db.Places;
            }
        }
        
        public IQueryable<Action> Actions
        {
            get
            {
                return Db.Actions;
            }
        }

        public bool CreateAct(Action instance)
        {
            if (instance.ID_Act != Guid.Empty)
            {
                Db.Actions.InsertOnSubmit(instance);
                Db.Actions.Context.SubmitChanges();
                return true;
            }

            return false;
        }

        public bool RemoveAct(System.Guid idAct)
        {
            Action instance = Db.Actions.Where(p => p.ID_Act == idAct).FirstOrDefault();
            if (instance != null)
            {
                Db.Actions.DeleteOnSubmit(instance);
                Db.Actions.Context.SubmitChanges();
                return true;
            }

            return false;
        }

        public IQueryable<Package> Packages
        {
            get
            {
                return Db.Packages;
            }
        }

        public bool CreatePack(Package instance)
        {
            if (instance.ID_Pack !=  Guid.Empty)
            {
                Db.Packages.InsertOnSubmit(instance);
                Db.Packages.Context.SubmitChanges();
                return true;
            }

            return false;
        }

        public bool UpdatePack(Package instance)
        {
            Package cache = Db.Packages.Where(p => p.ID_Pack == instance.ID_Pack).FirstOrDefault();
            if (cache != null)
            {
                //TODO : Update fields for Package
                Db.Packages.Context.SubmitChanges();
                return true;
            }

            return false;
        }

        public bool RemovePack(System.Guid idPack)
        {
            Package instance = Db.Packages.Where(p => p.ID_Pack == idPack).FirstOrDefault();
            if (instance != null)
            {
                Db.Packages.DeleteOnSubmit(instance);
                Db.Packages.Context.SubmitChanges();
                return true;
            }

            return false;
        }
                
    }
}