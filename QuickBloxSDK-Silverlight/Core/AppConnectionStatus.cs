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
    /// Статус подключения приложения к серверу в данный момент
    /// </summary>
    public enum AppConnectionStatus
    {
        /// <summary>
        /// Приложение в сети
        /// Это означает что приложение подключилось к серверу и тепер можно производить дальнейшие операции
        /// До того как приложение не перешло в режим онлай НЕЛЬЗЯ ПРОИЗВОДИТЬ НИКАКИХ операций с сервисом
        /// </summary>
        Online,
        /// <summary>
        /// Приложение отключено от сервера.
        /// Если приложение было в режиме подключения к серверу и потом стало офлайн это может означать что
        /// прилоежение возможно не подключилось из затого что не сомогл пройти аутентификацию.
        /// По это й причине когда приложение находтся в режиме офлайн а находилось до этого в режиме подключения то необходимо
        /// посмотреть ошибки подключения
        /// </summary>
        Offline,
        /// <summary>
        /// Приложение находится в режиме подключения к серверу
        /// </summary>
        Conection

    }
}
