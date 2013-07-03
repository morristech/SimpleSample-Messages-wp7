using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace QuickBloxSDK_Silverlight.Geo
{
    /// <summary>
    /// Command ID that was done at present time.
    /// </summary>
    public enum GeoServiceCommand
    {

        /// <summary>
        /// Add location
        /// </summary>
        AddGeoLocation,
        /// <summary>
        /// Get location
        /// </summary>
        GetGeoLocation,
        /// <summary>
        /// Get all locations for application 
        /// </summary>
        GetGeoLocationsForApp,
        /// <summary>
        /// Get all locations for user 
        /// </summary>
        GetGeoLocationsForUser
    }
}
