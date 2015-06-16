﻿using Windows.Devices.Geolocation;

namespace ShoppingListWPApp.Models
{
    /// <summary>
    /// An object which holds information about a Shop.
    /// </summary>
    class Shop
    {
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

        public Shop(string name, string address, double radius, BasicGeoposition? location)
        {
            Name = name;
            Address = address;
            Radius = radius;
            Location = location;
        }
    }
}
