using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingListWPApp.Models
{
    class ShoppingList
    {
        public string ListName { get; set; }
        public Shop Shop { get; set; }

        public ObservableCollection<ShoppingListItem> Items { get; set; }

        public ShoppingList() { }

        public ShoppingList(string _name, Shop _shop)
        {
            ListName = _name;
            Shop = _shop;
            Items = new ObservableCollection<ShoppingListItem>();
        }

        public void AddItem(ShoppingListItem shListItem){
            Items.Add(shListItem);
        }
    }
}
