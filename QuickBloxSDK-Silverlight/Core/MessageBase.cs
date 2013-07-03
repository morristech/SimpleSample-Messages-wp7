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
    /// Базовый класс сообщений
    /// </summary>
    public class MessageBase : Object
    {
        /// <summary>
        /// Параметр определяет что это сообщение является сообщением об ошибке или нет
        /// </summary>
        public bool IsOK
        { get; set; }

        /// <summary>
        /// Сообщение об ошибки если таковое имеется
        /// </summary>
        public string ErrorMessage
        { get; set; }
    }
}
