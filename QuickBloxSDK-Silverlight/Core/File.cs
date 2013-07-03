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
    public class File
    {
        /// <summary>
        /// Название поля
        /// </summary>
        public string Name
        { get; set; }

        /// <summary>
        /// Название файла
        /// </summary>
        public string FileName
        { get; set; }

        /// <summary>
        /// Mime тип контента
        /// </summary>
        public string ContentType
        { get; set; }

        /// <summary>
        /// Непосредсвенно файл
        /// </summary>
        public byte[] Content
        { get; set; }

    }
}
