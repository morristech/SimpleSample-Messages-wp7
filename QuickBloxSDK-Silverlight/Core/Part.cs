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
    /// Разделы сервиса QuickBlox
    /// </summary>
    public enum Part
    {
        /// <summary>
        /// Пользоваетли и владельци
        /// </summary>
        users,
        /// <summary>
        /// Гео раздел
        /// </summary>
        geopos,
        /// <summary>
        /// Сообщения пользователю
        /// </summary>
        messaging,
        /// <summary>
        /// Чат на джабере
        /// </summary>
        chat,
        /// <summary>
        /// Контент / файлы
        /// </summary>
        blobs,
        /// <summary>
        /// Рейтнги и результаты
        /// </summary>
        ratings,
        places,
        push_tokens,
        subscriptions,
        events,
        /// <summary>
        /// Авторизация приложения
        /// </summary>
        Admin
    }
}
