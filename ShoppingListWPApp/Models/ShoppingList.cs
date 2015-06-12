using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingListWPApp.Models
{
    class ShoppingList
    {
        public string Name { get; set; }

        public ShoppingList() { }

        public ShoppingList(string _name)
        {
            Name = _name;
        }
    }
}
