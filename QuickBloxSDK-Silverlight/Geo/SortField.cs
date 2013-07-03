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
    /// Сортировать по полю
    /// </summary>
    public enum SortField
    {
        /// <summary>
        /// По дате создания
        /// </summary>
        Date,
        /// <summary>
        /// По географицеской широте
        /// </summary>
        Latitude,
        /// <summary>
        /// По географической долготе
        /// </summary>
        Longitude,
        /// <summary>
        /// По расстоянию
        /// </summary>
        Distance
    }
}
