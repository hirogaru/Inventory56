using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace sklad56.Models
{
    public class Cart
    {
        /// <summary>
        /// класс, представляющий из себя модель корзины
        /// </summary>
        private List<CartLine> lineCollection = new List<CartLine>();

        public void AddItem(Item _item, int _quantity)
        {
            CartLine line = lineCollection //ищем, есть ли в корзине уже такой предмет
                .Where(g => g.item.ID_Item == _item.ID_Item)
                .FirstOrDefault();

            if (line == null)
            {
                lineCollection.Add(new CartLine //если нет - добавляем
                {
                    item = _item,
                    quantity = _quantity
                });
            }
            else
            {
                //Пока ничего не делаем, кол-во для каждой ячейки предметов будет реализовано позже
                //line.quantity += _quantity; //если есть - увеличиваем его число
            }
        }

        public void RemoveLine(Item _item)
        {
            lineCollection.RemoveAll(l => l.item.ID_Item == _item.ID_Item); //удаляем ячейку со всем количеством предметов
        }

        public int ComputeTotalValue()
        {
            return lineCollection.Sum(e => e.quantity); //рассчитываем общее кол-во предметов

        }

        public void Clear()
        {
            lineCollection.Clear(); //очищаем корзину
        }

        public IEnumerable<CartLine> Lines
        {
            get { return lineCollection; } //формируем список для отображения
        }
    }

    public class CartLine
    {
        //ячейка корзины с предметом и его количеством
        
        public Item item { get; set; }

        public int quantity { get; set; }
    }

    public class CartIndexViewModel
    {
        //модель для представления корзины
        
        public Cart Cart { get; set; }

        public string ReturnUrl { get; set; }

        public string Coment { get; set; }

        public string User { get; set; }

        public string Place { get; set; }

        public List<SelectListItem> Places { get; set; }

        public List<SelectListItem> Users { get; set; }
    }
}