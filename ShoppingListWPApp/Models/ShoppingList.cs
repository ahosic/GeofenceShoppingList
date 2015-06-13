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
        public string ShopName { get; set; }

        public ObservableCollection<ShoppingListItem> Items { get; set; }

        public ShoppingList() { }

        public ShoppingList(string _name, string _shopName)
        {
            ListName = _name;
            ShopName = _shopName;
            Items = new ObservableCollection<ShoppingListItem>();
        }

        public void AddItem(ShoppingListItem shListItem){
            Items.Add(shListItem);
        }
    }
}
