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
    /// Класс описывает сеанс подключения к серверу.
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Идентификатор приложения в базе данных
        /// </summary>
        public int ApplicationId
        { get; private set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedDate
        { get; private set; }

        /// <summary>
        /// Идентификатор устройства
        /// </summary>
        public int? DeviceId
        { get; private set; }

        /// <summary>
        /// Идентификатор сессии в базе данных
        /// </summary>
        public int Id
        { get; private set; }


        /// <summary>
        /// Случайное число
        /// </summary>
        public int Nonce
        { get; private set; }

        /// <summary>
        /// Маркер идентифицирующий сеанс подключения (кука короче тока не кука)
        /// </summary>
        public string Token
        { get; private set; }

        /// <summary>
        /// Время генерации запроса в формате unix timestamp.
        /// </summary>
        public int TS
        { get; private set; }

        /// <summary>
        /// Лата обновления сеанса
        /// </summary>
        public DateTime UpdatedTime
        { get; private set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int? UserId
        { get; private set; }

        /// <summary>
        /// Создаёт объект по xml схеме
        /// </summary>
        /// <param name="Scheme">Схема</param>
        /// <exception cref="Exception">
        /// Не верная схема или не возможно распарсить XML
        /// </exception>
        public Session(string Scheme)
        {
            if (string.IsNullOrEmpty(Scheme))
                throw new Exception("Scheme not valid");

            try
            {
                XElement xmlResult = XElement.Parse(Scheme);
                this.Id = int.Parse(xmlResult.Element("id").Value);
                this.ApplicationId = int.Parse(xmlResult.Element("application-id").Value);
                this.Nonce = int.Parse(xmlResult.Element("nonce").Value);
                this.TS = int.Parse(xmlResult.Element("ts").Value);
                //----
                this.CreatedDate = DateTime.Parse(xmlResult.Element("created-at").Value);
                this.UpdatedTime = DateTime.Parse(xmlResult.Element("updated-at").Value);
                this.DeviceId = string.IsNullOrEmpty(xmlResult.Element("device-id").Value) ? (int?)null : int.Parse(xmlResult.Element("device-id").Value);
                this.UserId = string.IsNullOrEmpty(xmlResult.Element("user-id").Value) ? (int?)null : int.Parse(xmlResult.Element("user-id").Value);
                this.Token = xmlResult.Element("token").Value;
            }
            catch
            {
                throw new Exception("Scheme not valid");
            }
        }


    }
}
