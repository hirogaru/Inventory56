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

        Guid getAdminID(string login);

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

        public Guid getAdminID(string login)
        {
            login = login.Substring(Globals.DomainName.Length + 1); //отрезаем логин от домена
            try
            {
                Guid UsrID = (Db.LogoPasses.First(p => p.Login == login)).UserID;
                return UsrID;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        public string getAdminName(string login)
        {
            var AdminID = getAdminID(login);
            try
            {
                var usrName = (Db.Users.First(p => p.ID_User == AdminID)).Username;
                return usrName;
            }
            catch
            {
                return "Гость";
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
                cache.ID_User = instance.ID_User;
                cache.Username = instance.Username;
                cache.Post = instance.Post;
                cache.Phone = instance.Phone;
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
                DelData backup = new DelData() 
                { 
                    DeletedData = "User" +
                    "|" + instance.ID_User.ToString() +
                    "|" + instance.Username +
                    "|" + instance.Post +
                    "|" + instance.Phone + 
                    "|" + instance.IsAdmin.ToString() 
                };
                Db.DelDatas.InsertOnSubmit(backup); //бэкапим данные

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
                cache.ID_Item = instance.ID_Item;
                cache.Itemname = instance.Itemname;
                cache.Serial = instance.Serial;
                cache.Package = Db.Packages.Single(x => x.ID_Pack == instance.Belongs);
                cache.Place1 = Db.Places.Single(x => x.ID_Place == instance.Place);
                cache.Cast = instance.Cast;
                cache.User = Db.Users.Where(p => p.ID_User == instance.Username).FirstOrDefault();
                cache.Broken = instance.Broken;
                cache.Verifi = instance.Verifi;
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
                DelData backup = new DelData()
                {
                    DeletedData = "Item" +
                    "|" + instance.ID_Item.ToString() +
                    "|" + instance.Itemname +
                    "|" + instance.Serial +
                    "|" + instance.Belongs.ToString() +
                    "|" + instance.Place.ToString() +
                    "|" + instance.Cast.ToString() +
                    "|" + (instance.Username == null ? "null" : instance.Username.ToString()) +
                    "|" + (instance.Broken == null ? "null" : instance.Broken.ToString()) +
                    "|" + (instance.Verifi == null ? "null" : instance.Verifi.ToString())
                };
                Db.DelDatas.InsertOnSubmit(backup); //бэкапим данные

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
                DelData backup = new DelData()
                {
                    DeletedData = "Action" +
                    "|" + instance.ID_Act.ToString() +
                    "|" + instance.Whom.ToString() +
                    "|" + instance.What.ToString() +
                    "|" + instance.When.ToString() +
                    "|" + instance.Todo.ToString() +
                    "|" + instance.Coment +
                    "|" + instance.AdminID.ToString()
                };
                Db.DelDatas.InsertOnSubmit(backup); //бэкапим данные
                
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
                cache.ID_Pack = instance.ID_Pack;
                cache.Name = instance.Name;
                cache.Coment = instance.Coment;
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
                DelData backup = new DelData()
                {
                    DeletedData = "Package" +
                    "|" + instance.ID_Pack.ToString() +
                    "|" + instance.Name +
                    "|" + instance.Coment
                };
                Db.DelDatas.InsertOnSubmit(backup); //бэкапим данные

                Db.Packages.DeleteOnSubmit(instance);
                Db.Packages.Context.SubmitChanges();
                return true;
            }

            return false;
        }
                
    }

    public class TestRepository : IRepository
    {
        public IQueryable<DelData> DelDatas { get; set; }

        public IQueryable<InCharge> InCharges { get; set; }

        public IQueryable<LogoPass> LogoPasses { get; private set; }

        public Guid getAdminID(string login)
        {
            throw new System.NotImplementedException();
        }

        public string getAdminName(string login)
        {
            return "Admin";
        }

        public IQueryable<User> Users { get { return (new List<User> { new User { 
            Username = "TestUser", 
            ID_User = Guid.Parse("14444444-4444-4444-4444-444444444444"), 
            Post ="TestPost" } }).AsQueryable(); } }

        public bool CreateUser(User instance)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateUser(User instance)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveUser(Guid idUser)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<Place> Places { get { return (new List<Place> { new Place { 
            Name = "TestZone", 
            ID_Place = Guid.Parse("14444444-4444-4444-4444-444444444444"),
            Picture = "oblivion.png",
            City = "TestTown" } }).AsQueryable(); } }

        public IQueryable<Item> Items { get { return (new List<Item> { new Item { 
            Itemname = "TestItem", 
            ID_Item = Guid.Parse("14444444-4444-4444-4444-444444444444"), 
            Place1 = new Place { Name = "TestZone", ID_Place = Guid.Parse("14444444-4444-4444-4444-444444444444")},
            Package = new Package { Name = "TestPack", ID_Pack = Guid.Parse("14444444-4444-4444-4444-444444444444")},
            Serial = "TestSerial", 
            Cast = 1 } }).AsQueryable(); } }

        public bool CreateItem(Item instance)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateItem(Item instance)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveItem(Guid idItem)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<sklad56.Models.Action> Actions { get { return (new List<sklad56.Models.Action> { new sklad56.Models.Action{
            ID_Act = Guid.Parse("14444444-4444-4444-4444-444444444444"),
            Item = new Item { Itemname = "TestItem", ID_Item = Guid.Parse("14444444-4444-4444-4444-444444444444") },
            When = DateTime.Now,
            User = new User { Username = "TestUser", ID_User = Guid.Parse("14444444-4444-4444-4444-444444444444") },
            User1 = new User { Username = "TestAdmin" }, 
            Todo = 2,
            Coment = "Test" } }).AsQueryable(); } }

        public bool CreateAct(sklad56.Models.Action instance)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveAct(Guid idAct)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<Package> Packages { get { return (new List<Package> { new Package { 
            Name = "TestPack", 
            ID_Pack = Guid.Parse("14444444-4444-4444-4444-444444444444"),
            Coment = "TestComment" } }).AsQueryable(); } }

        public bool CreatePack(Package instance)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdatePack(Package instance)
        {
            throw new System.NotImplementedException();
        }

        public bool RemovePack(Guid idPack)
        {
            throw new System.NotImplementedException();
        }
    }
}