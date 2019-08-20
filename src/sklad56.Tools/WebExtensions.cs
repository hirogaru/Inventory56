using System;
using System.Text;
using System.Web.Mvc;

namespace sklad56.Tools
{
    public static class WebExtensions
    {
        //Преобразует многострочные данные
        public static MvcHtmlString NlToBr(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return new MvcHtmlString(string.Empty);
            }
            return new MvcHtmlString(source.Replace(Environment.NewLine, "<br />"));
        }

        //Урезает строку до максимального допустимого количества символов, и ставит «…» после
        public static string Teaser(this string content, int length, string more = "...")
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return string.Empty;
            }

            if (content.Length < length)
            {
                return content;
            }

            return content.Substring(0, length) + more;
        }

        //Подставляет одно из слов в определении для 1, 2 или 5
        public static string CountWord(this int count, string first, string second, string five)
        {
            if (count % 10 == 1 && (int)(count / 10) != 1)
            {
                return first;
            }
            if (count % 10 > 1 && count % 10 < 5 && ((int)(count / 10) % 10) != 1)
            {
                return second;
            }
            return five;
        }

        //Переводит номер действия в строковое представление
        public static string ToEnumAct(this byte num)
        {
            string rez="";
            switch (num)
            {
                case 1:
                    rez = "Взял";
                    break;
                case 2:
                    rez = "Вернул";
                    break;
                case 3:
                    rez = "Сломал";
                    break;
                case 4:
                    rez = "Потерял";
                    break;
                case 5:
                    rez = "Передал";
                    break;
            }
            return rez;
        }

        //переводит номер типа в название типа оборудования
        public static string ToEnumTyp(this byte num)
        {
            string rez = "";
            switch (num)
            {
                case 0:
                    rez = "Всё подряд";
                    break;
                case 1:
                    rez = "Общая группа";
                    break;
                case 2:
                    rez = "Приёмная станция";
                    break;
                case 3:
                    rez = "Ноутбук";
                    break;
                case 4:
                    rez = "АФС";
                    break;
                case 5:
                    rez = "Питание";
                    break;
            }
            return rez;
        }

        //Helper пагинатора, который даст нам возможность пролистывать постранично таблицу формируя список страниц.
        public static MvcHtmlString PageLinks(this HtmlHelper html, int currentPage, int totalPages, Func<int, string> pageUrl)
        {
            StringBuilder builder = new StringBuilder();

            //Вывести Prev
            var prevBuilder = new TagBuilder("a");
            prevBuilder.InnerHtml = "&laquo;";
            if (currentPage == 1) //Сделать активным если надо
            {
                prevBuilder.MergeAttribute("href", "#");
                builder.AppendLine("<li class=\"active\">" + prevBuilder.ToString() + "</li>");
            }
            else
            {
                prevBuilder.MergeAttribute("href", pageUrl.Invoke(currentPage - 1));
                builder.AppendLine("<li>" + prevBuilder.ToString() + "</li>");
            }

            for (int i = 1; i <= totalPages; i++) //По порядку страницы
            {
                if (((i <= 3) || (i > (totalPages - 3))) || ((i > (currentPage - 2)) && (i < (currentPage + 2)))) //Условие что выводим только необходимые номера
                {
                    var subBuilder = new TagBuilder("a");
                    subBuilder.InnerHtml = i.ToString();
                    if (i == currentPage)
                    {
                        subBuilder.MergeAttribute("href", "#");
                        builder.AppendLine("<li class=\"active\">" + subBuilder.ToString() + "</li>");  //Вывести активной ссылку текущей страницы 
                    }
                    else
                    {
                        subBuilder.MergeAttribute("href", pageUrl.Invoke(i));
                        builder.AppendLine("<li>" + subBuilder.ToString() + "</li>");
                    }
                }
                else if ((i == 4) && (currentPage > 5)) //Троеточие первое
                {
                    builder.AppendLine("<li class=\"disabled\"> <a href=\"#\">...</a> </li>");
                }
                else if ((i == (totalPages - 3)) && (currentPage < (totalPages - 4))) //Троеточие второе
                {
                    builder.AppendLine("<li class=\"disabled\"> <a href=\"#\">...</a> </li>");
                }
            }
            //Вывести Next 
            var nextBuilder = new TagBuilder("a");
            nextBuilder.InnerHtml = "&raquo;";
            if ((currentPage == totalPages) || (totalPages == 0)) //Сделать активной если надо
            {
                nextBuilder.MergeAttribute("href", "#");
                builder.AppendLine("<li class=\"active\">" + nextBuilder.ToString() + "</li>");
            }
            else
            {
                nextBuilder.MergeAttribute("href", pageUrl.Invoke(currentPage + 1));
                builder.AppendLine("<li>" + nextBuilder.ToString() + "</li>");
            }
            return new MvcHtmlString("<ul>" + builder.ToString() + "</ul>"); //Заключить всё в ul и вывести как MvcHtmlString
        }
    }
}
