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
    public class PushToken
    {
        /// <summary>
        /// Идентификатор PushToken  в базе данных
        /// </summary>
        public uint Id
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Environment
        {
            get;
            set;
        }

        /// <summary>
        /// Зарегистрированный канал
        /// </summary>
        public string ClientIdentificationSequence
        {
            get;
            set;
        }


        public PushToken(string Xml)
        {
            this.Parse(Xml);

        }
        #region Методы 
         
        /// <summary>
        /// Converts object into string
        /// </summary>
        /// <returns>User name</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(this.ClientIdentificationSequence) ? string.Empty : ClientIdentificationSequence;
        }

        /// <summary>
        /// Parse user that came from server
        /// </summary>
        /// <param name="xml"></param>
        private void Parse(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                throw new Exception("Content error");
            try
            {
                XElement xmlResult = XElement.Parse(xml);
                this.Id = uint.Parse(xmlResult.Element("id").Value);
                this.Environment = xmlResult.Element("environment").Value;
                this.ClientIdentificationSequence = xmlResult.Element("client-identification-sequence").Value;
            }
            catch
            {
                throw new Exception("Content error");
            }
        }
        #endregion

    }
}
