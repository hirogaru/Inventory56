using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace sklad56.Models
{
    public class PageableData<T> where T : class
    {
        /// <summary>
        /// Модель для отображения определенного количества данных на текущей странице
        /// </summary>

        protected static int ItemPerPageDefault = 20; //По умолчанию количество выводимых значений на странице 

        public IEnumerable<T> List { get; set; } //список элементов

        public int PageNo { get; set; } //текущий номер страницы

        public int CountPage { get; set; } //кол-во страниц

        public int ItemPerPage { get; set; } //ячеек на странице

        public PageableData(IQueryable<T> queryableSet, int page, int itemPerPage = 0)
        {
           
            ItemPerPage = itemPerPage == 0 ? ItemPerPageDefault : itemPerPage;
            
            PageNo = page;
            var count = queryableSet.Count();

            CountPage = (int)decimal.Remainder(count, itemPerPage) == 0 ? count / itemPerPage : count / itemPerPage + 1; //Вычисляем кол-во страниц
            List = queryableSet.Skip((PageNo - 1) * itemPerPage).Take(itemPerPage); //Используя PageNo, набираем страницу значений
        }
    }

    public class SearchEngine<T> where T : class
    {
        /// <summary>
        /// Методы по очистке строки запроса, убирающие теги и убираем разделители типа [,], {,}, (,)
        /// и метод Search, который принимает значения IQueryable, и строку поиска, а возвращает данные по поиску
        /// </summary>

        private static string StripHtml(string html)
        {
            Regex RegexStripHtml = new Regex("<[^>]*>", RegexOptions.Compiled);
            return string.IsNullOrWhiteSpace(html) ? string.Empty : RegexStripHtml.Replace(html, string.Empty).Trim(); //очищаем строку от html тегов
        }

        private static string CleanContent(string content, bool removeHtml)
        {
            if (removeHtml)
            {
                content = StripHtml(content);
            }

            content =
                content.Replace("\\", string.Empty).
                Replace("|", string.Empty).
                Replace("(", string.Empty).
                Replace(")", string.Empty).
                Replace("[", string.Empty).
                Replace("]", string.Empty).
                Replace("*", string.Empty).
                Replace("?", string.Empty).
                Replace("}", string.Empty).
                Replace("{", string.Empty).
                Replace("^", string.Empty).
                Replace("+", string.Empty);  //убираем элементы regex

            var words = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries); //избавляемся от разделителей
            var sb = new StringBuilder();
            foreach (var word in
                words.Select(t => t.ToLowerInvariant().Trim()).Where(word => word.Length > 1))
            {
                sb.AppendFormat("{0} ", word);
            }

            return sb.ToString();
        }

        public static IEnumerable<T> Search(string searchString, IQueryable<T> source) 
        {
            var term = CleanContent(searchString.ToLowerInvariant().Trim(), true); //Очищаем строку запроса
            var terms = term.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); //Создаём массив искомых слов
            var regex = string.Format(CultureInfo.InvariantCulture, "({0})", string.Join("|", terms)); //Создаем regex для поиска
            
            //TODO : переписать под dynamic чтоб убрать нахер этот пиздец с рефлексией
            string fieldName="";
            var myType = source.ElementType;

            if (myType == typeof(User)) fieldName = "_Username";
            if (myType == typeof(Item)) fieldName = "_Itemname";

            var field = myType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var entry in source) //Просматриваем весь список, переданный через IQueryable
            {
                var rank = 0;
                
                var value = (string)field.GetValue(entry);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    rank += Regex.Matches(value.ToLowerInvariant(), regex).Count;
                }
                if (rank > 0) //Если есть совпадение, то выносим его в IEnumerable 
                {
                    yield return entry;
                }
            }
        }
    }

    public class Enums
    {
        //класс для перечисления возможных действий, совершённых пользователем и типов оборудования

        public enum Todo : byte
        {
            InUse = 1,  //Взял
            Return,     //Вернул
            breaked,    //Сломал
            lost,       //Потерял
            Handed      //Передал
        }

        public enum Cast: byte 
        {
            NoType=1,	//Общая группа
            Station,	//Приёмная станция
            Notebook,	//Ноутбук
            AFS,		//АФС
            Power       //Питание
        }

        public static byte castCount = 5; //кол-во возможных типов оборудования
    }

    public class EquipListViewModel 
    { 
        //класс для отображения предметов с датами их принятия

        public Item item { get; set;}

        public DateTime? date { get; set; }
    }

    public class PackageViewModel
    {
        //модель отображения списка предметов в комплекте

        public Package pack { get; set; }

        public PageableData<Item> items { get; set; }
    }

    public class PlaceViewModel
    {
        //модель отображения списка предметов в данном месте

        public Place place { get; set; }

        public PageableData<Item> items { get; set; }
    }

    public class UserInCharge
    {
        //модель отображения списка ответственных

        public string user { get; set; }

        public string item { get; set; }

        public string userID { get; set; }

        public string itemID { get; set; }
    }

    public class HandOnViewModel
    {
        //модель отображения Пунктa «передать»

        public Item item { get; set; }

        public string user { get; set; }

        public List<SelectListItem> users { get; set; }
    }

    public class EditVeryfiViewModel
    {
        //модель отображения редактирования поверки

        public string returnUrl { get; set; }

        public Guid ItemID { get; set; }

        public string Itemname { get; set; }

        public int Day { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public IEnumerable<SelectListItem> DaySelectList
        {
            get
            {
                for (int i = 1; i < 32; i++)
                {
                    yield return new SelectListItem
                    {
                        Value = i.ToString(),
                        Text = i.ToString(),
                        Selected = Day == i
                    };
                }
            }
        }

        public IEnumerable<SelectListItem> MonthSelectList
        {
            get
            {
                for (int i = 1; i < 13; i++)
                {
                    yield return new SelectListItem
                    {
                        Value = i.ToString(),
                        Text = new DateTime(2000, i, 1).ToString("MMMM"),
                        Selected = Month == i
                    };
                }
            }
        }

        public IEnumerable<SelectListItem> YearSelectList
        {
            get
            {
                for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 5; i++)
                {
                    yield return new SelectListItem
                    {
                        Value = i.ToString(),
                        Text = i.ToString(),
                        Selected = Year == i
                    };
                }
            }
        }

    }
}