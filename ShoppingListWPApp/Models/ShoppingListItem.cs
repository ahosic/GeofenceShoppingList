using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingListWPApp.Models
{
    class ShoppingListItem
    {
        public string Name { get; set; }
        public double Amount { get; set; }
        public string Measure { get; set; }

        public ShoppingListItem() { }

        public ShoppingListItem(string _name, double _amount, string _measure)
        {
            Name = _name;
            Amount = _amount;
            Measure = _measure;
        }
    }
}
