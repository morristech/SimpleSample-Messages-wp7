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

namespace QuickBloxSDK_Silverlight.Content
{
    public class BlobObjectAccess
    {
        public BlobObjectAccess(string XML)
        {
            this.Parse(XML);
        }

        #region

        public uint BlobId
        { get; set; }

        public DateTime Expires
        { get; set; }

        /// <summary>
        /// Идентификатор объекта в базе
        /// </summary>
        public uint Id
        { get; set; }

        public string ObjectAccessType
        { get; set; }


        public string Params
        { get; set; }

        #endregion


        private void Parse(string xml)
        {
            try
            {
                XElement xmlResult = XElement.Parse(xml);
                this.Id = uint.Parse(xmlResult.Element("id").Value);
                this.BlobId = uint.Parse(xmlResult.Element("blob-id").Value);
                //----
                this.Expires = DateTime.Parse(xmlResult.Element("expires").Value);
                this.Params = xmlResult.Element("params").Value;
                this.ObjectAccessType = xmlResult.Element("object-access-type").Value;
            }
            catch
            {
            }
        }


    }
}
