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

namespace QuickBloxSDK_Silverlight.Places
{
    /// <summary>
    /// Команды отправляемые к серверу
    /// </summary>
    public enum PlaceServiceCommand
    {
        /// <summary>
        /// Создать место
        /// </summary>
        AddPlace,
        /// <summary>
        /// Получить место по идентификатору
        /// </summary>
        GetPlace,
        /// <summary>
        /// Удалить место по идентификатору
        /// </summary>
        DeletePlace,
        /// <summary>
        /// Редактировать место
        /// </summary>
        EditPlace,
        /// <summary>
        /// Получить все местоположения для приложения
        /// </summary>
        GetPlacesByApp


    }
}
