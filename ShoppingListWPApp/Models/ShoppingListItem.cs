namespace ShoppingListWPApp.Models
{
    class ShoppingListItem
    {
        public string Name { get; set; }
        public string AmountAndMeasure { get; set; }

        public ShoppingListItem() { }

        public ShoppingListItem(string _name, string _amountAndMeasure)
        {
            Name = _name;
            AmountAndMeasure = _amountAndMeasure;
        }
    }
}
