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
    /// Ответ от сервера
    /// </summary>
    public class Result : MessageBase
    {
        /// <summary>
        /// Непостредственно ответ. В текстовом виде
        /// </summary>
        public string Content
        { get; set; }

        /// <summary>
        /// Название контроллера на сервере куда было отправлено сообщение
        /// </summary>
        public string ControllerName
        { get; set; }

        /// <summary>
        /// Тип запроса к серверу
        /// </summary>
        public AcceptVerbs Verbs
        { get; set; }

        /// <summary>
        /// Полная адресная строка запроса
        /// </summary>
        public string URI
        { get; set; }

        /// <summary>
        /// URI сервера
        /// </summary>
        public string ServerName
        { get; set; }

        /// <summary>
        /// Статус ответа. 
        /// </summary>
        public Status ResultStatus
        { get; set; }

        
    }
}
