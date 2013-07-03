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

namespace QuickBloxSDK_Silverlight.Core
{
    /// <summary>
    ///Тип запросов к серверу
    /// </summary>
    public enum AcceptVerbs
    {

        OPTIONS,
        /// <summary>
        /// Получить ресурс
        /// </summary>
        GET,
        /// <summary>
        /// Получить заголовок
        /// </summary>
        HEAD,
        /// <summary>
        /// Создать ресурс
        /// </summary>
        POST,
        /// <summary>
        /// Изменить ресурс
        /// </summary>
        PUT,
        /// <summary>
        /// Удалить ресурс
        /// </summary>
        DELETE,
        TRACE
    }
}
