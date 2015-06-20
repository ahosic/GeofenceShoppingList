using System;
using System.Collections.ObjectModel;

namespace ShoppingListWPApp.Models
{
    class ShoppingList
    {
        /// <summary>
        /// Gets the ID of the Shopping list.
        /// </summary>
        public string ID { get; private set; }
        public string ListName { get; set; }
        public Shop Shop { get; set; }

        public ObservableCollection<ShoppingListItem> Items { get; set; }

        public ShoppingList() { }

        public ShoppingList(string name, Shop shop)
        {
            ID = Guid.NewGuid().ToString();
            ListName = name;
            Shop = shop;
            Items = new ObservableCollection<ShoppingListItem>();
        }

        public void AddItem(ShoppingListItem shListItem)
        {
            Items.Add(shListItem);
        }
    }
}
