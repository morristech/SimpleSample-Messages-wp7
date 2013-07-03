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
    public class PostResponse
    {

        public PostResponse(string XML)
        {
            this.Parse(XML);
        }


        public string Location
        { get; set; }


        public string Bucket
        { get; set; }


        public string Key
        { get; set; }


        public string ETag
        { get; set; }



        private void Parse(string xml)
        {
            try
            {
                XElement xmlResult = XElement.Parse(xml);
                this.Location = xmlResult.Element("Location").Value;
                this.Bucket = xmlResult.Element("Bucket").Value;
                this.Key = xmlResult.Element("Key").Value;
                this.ETag = xmlResult.Element("ETag").Value;
            }
            catch
            {
            }
        }

    }
}
