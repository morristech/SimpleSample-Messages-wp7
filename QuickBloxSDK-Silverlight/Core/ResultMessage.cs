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
using System.Xml.Linq;

namespace QuickBloxSDK_Silverlight.Core
{
    /// <summary>
    /// Сообщение от сервера которое приходит в ответ на смену пароля.
    /// </summary>
    public class ResultMessage
    {
        /// <summary>
        ///  Конструктор. Создает сообщение
        /// </summary>
        /// <param name="IsOk">Successfully/not successfully</param>
        /// <param name="Password">Password</param>
        public ResultMessage(bool IsOk, string Password)
        {
            this.IsOK = IsOk;
            this.Password = Password;
 
        }


        /// <summary>
        /// Распарсивает сообщение от сервера
        /// </summary>
        /// <param name="Scheme">XML scheme</param>
        public ResultMessage(string Scheme)
        {


            XElement xmlResult = XElement.Parse(Scheme);
            this.IsOK = bool.Parse(xmlResult.Element("result").Value);
            

            try
            {
                this.Password = xmlResult.Element("password").Value;
            }
            catch
            {
 
            }

        }

        /// <summary>
        /// Successfully/not successfully
        /// </summary>
        public bool IsOK
        { get; private set; }

        /// <summary>
        /// Changed password
        /// </summary>
        public string Password
        { get; private set; }
    }
}
