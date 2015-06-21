using Windows.Devices.Geolocation;

namespace ShoppingListWPApp.Models
{
    /// <summary>
    /// An object which holds information about a Shop.
    /// </summary>
    public class Shop
    {
        /// <summary>
        /// Gets the ID of a Shop.
        /// </summary>
        public string ID { get; private set; }
        /// <summary>
        /// Gets or Sets the Name of a Shop.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or Sets the Address of a Shop.
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Gets or Sets the Radius of a Shop.
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// Gets or Sets the Geographical Position (Longitude, Latitude, Altitude) of a Shop.
        /// </summary>
        public BasicGeoposition? Location { get; set; }

        public Shop(string id, string name, string address, double radius, BasicGeoposition? location)
        {
            ID = id;
            Name = name;
            Address = address;
            Radius = radius;
            Location = location;
        }

        /// <summary>
        /// Gets a formatted representation of the Shop containing the Name and Address of the Shop.
        /// 
        /// If the Address property is null, only the Name of the Shop will be returned.
        /// </summary>
        /// <returns>A formatted representation of the Shop.</returns>
        public override string ToString()
        {
            // Check, if Address property is empty or null
            if (Address.Trim().Equals(string.Empty) || Address == null)
            {
                return Name;
            }

            return Name + ", " + Address;
        }
    }
}
