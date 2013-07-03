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
using QuickBloxSDK_Silverlight.Core;

namespace QuickBloxSDK_Silverlight.Geo
{
    /// <summary>
    /// Ответ от сервера касающийся сервиса геолокаций
    /// </summary>
    public class GeoServiceEventArgs
    {
        /// <summary>
        /// Возвращаемый объект
        /// </summary>
        public object result
        { get; set; }

        /// <summary>
        /// Тип возвращаемого объекта
        /// </summary>
       public Type t
        { get; set; }

        /// <summary>
        /// Статус ответа от сервера
        /// </summary>
       public Status status
       { get; set; }

        /// <summary>
        /// Текущая команда, которая была выполнена
        /// </summary>
       public GeoServiceCommand currentCommand
       { get; set; }

        /// <summary>
        /// Сообщение об ошибки. Зависит от статуса
        /// </summary>
       public string errorMessage
       { get; set; }
    }
}
