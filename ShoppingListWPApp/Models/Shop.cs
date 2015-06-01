using Windows.Devices.Geolocation;

namespace ShoppingListWPApp.Models
{
    class Shop
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public double Radius { get; set; }
        public BasicGeoposition? Location { get; set; }

        public Shop()
        {
        }

        public Shop(string name, string address, double radius, BasicGeoposition? location)
        {
            Name = name;
            Address = address;
            Radius = radius;
            Location = location;
        }
    }
}
