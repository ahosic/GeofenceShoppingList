namespace ShoppingListWPApp.Models
{
    /// <summary>
    /// An object which holds information about a Shoppinglistitem.
    /// </summary>
    class ShoppingListItem
    {
        /// <summary>
        /// Gets or Sets the Name of a Shoppinglistitem.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or Sets the AmountAndMeasure of a Shoppinglistitem.
        /// </summary>
        public string AmountAndMeasure { get; set; }

        public ShoppingListItem() { }

        public ShoppingListItem(string _name, string _amountAndMeasure)
        {
            Name = _name;
            AmountAndMeasure = _amountAndMeasure;
        }
    }
}
