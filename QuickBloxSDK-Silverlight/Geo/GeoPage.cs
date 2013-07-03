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
    /// Страница местоположений.
    /// Объект хранит местоположения пришедшие от сервера а так же информацию касающуюся общего колличества местоположений.
    /// Используется для постраничной навигации.
    /// </summary>
    public class GeoPage
    {

        public GeoPage()
        {
            
        }


        /// <summary>
        /// Устаревшее.
        /// Используется для совметимости.
        /// </summary>
        /// <param name="PageCount">Общее колличесвто страниц.</param>
        /// <param name="geoData">Масси геолокаций</param>
        public GeoPage(int PageCount, GeoData[] geoData)
        {
            this.GeoLocations = geoData;
            this.PageCount = PageCount;
        }

        /// <summary>
        /// Колличество локаций всего для этого приложения
        /// </summary>
        public int TotalEntries
        { get; set; }

        /// <summary>
        /// Колличество записей на странице
        /// </summary>
        public int LocationsOnPage
        { get; set; }

        /// <summary>
        /// Текущая страница
        /// </summary>
        public int CurrentPage
        { get; set; }

        /// <summary>
        /// Total pages (устаревшее, оставлено для совместимости)
        /// </summary>
        public int PageCount
        { get; set; }

        /// <summary>
        /// Массив местоположений
        /// </summary>
        public GeoData[] GeoLocations
        { get; set; }
    }
}
