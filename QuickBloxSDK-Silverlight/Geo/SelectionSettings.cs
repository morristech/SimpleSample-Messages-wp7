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
    /// (Устаревшее)
    /// Параметры выборки.
    /// </summary>
    public class SelectionSettings
    {
        /// <summary>
        /// Размер страници
        /// </summary>
        public int PageSize
        { get; set; }

        /// <summary>
        /// Выбрать последнюю запись
        /// </summary>
        public bool IsLastOnly
        { get; set; }

        /// <summary>
        ///Сортировать по типу
        /// </summary>
        public SortType sortType
        { get; set; }


        /// <summary>
        /// Сортировать по полю
        /// </summary>
        public SortField sortField
        { get; set; }

    }
}
