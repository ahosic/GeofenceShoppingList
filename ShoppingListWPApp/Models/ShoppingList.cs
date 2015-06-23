using System.Collections.ObjectModel;

namespace ShoppingListWPApp.Models
{
    /// <summary>
    /// An object which holds information about a Shoppinglist.
    /// </summary>
    class ShoppingList
    {
        /// <summary>
        /// Gets the ID of the Shoppinglist.
        /// </summary>
        public string ID { get; private set; }
        /// <summary>
        /// Gets or Sets the ListName of a Shoppinglist.
        /// </summary>
        public string ListName { get; set; }
        /// <summary>
        /// Gets or Sets the Shop of a Shoppinglist.
        /// </summary>
        public Shop Shop { get; set; }
        /// <summary>
        /// Gets or Sets the Items of the Shoppinglist.
        /// </summary>
        public ObservableCollection<ShoppingListItem> Items { get; set; }

        public ShoppingList(string id, string name, Shop shop)
        {
            ID = id;
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
