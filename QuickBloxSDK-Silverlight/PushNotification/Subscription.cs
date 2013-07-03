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

namespace QuickBloxSDK_Silverlight.PushNotification
{
    public class Subscription
    {

        public Subscription(string Xml)
        {
            this.Parse(Xml);

        }
       
        /// <summary>
        /// Уникальный идентификатор элемента
        /// </summary>
        public uint Id
        { get; private set; }

        /// <summary>
        /// Зарезервированный канал
        /// </summary>
        public string NotificationChannel
        { get; set; }


        /// <summary>
        /// Идентификатор устройства
        /// </summary>
        public string DeviceId
        { get; set; }



        /// <summary>
        /// Тип устройсва
        /// </summary>
        public string DevicePlatform
        { get; set; }

   

       

        #region
        /// <summary>
        /// Converts object into string
        /// </summary>
        /// <returns>Title</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(this.NotificationChannel) ? string.Empty : NotificationChannel;
        }

        /// <summary>
        /// Parse user that came from server
        /// </summary>
        /// <param name="xml"></param>
        private void Parse(string xml)
        {
            try
            {
                XElement xmlResult = XElement.Parse(xml);
                this.Id = uint.Parse(xmlResult.Element("id").Value);
                this.NotificationChannel = xmlResult.Element("notification-channel").Element("name").Value;
                this.DeviceId = xmlResult.Element("device").Element("udid").Value;
                this.DevicePlatform = xmlResult.Element("device").Element("platform").Element("name").Value;
            }
            
            catch
            {}
                
        }
        #endregion

    }
}
