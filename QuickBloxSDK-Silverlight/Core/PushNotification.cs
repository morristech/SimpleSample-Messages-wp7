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
using System.Text;

namespace QuickBloxSDK_Silverlight.Core
{
    public class PushNotification
    {

        public PushNotification(PushNotificationType type, string title, string text, string page)
        {
            this.NotificationType = type;
            this.Title = title;
            this.Text = text;
            this.PageName = page;
        }

        public PushNotificationType NotificationType
        { get; private set; }


        public string Title
        { get; private set; }

        public string Text
        { get; private set; }

        public string PageName
        { get; private set; }

        private string Render()
        {
           /* StringBuilder result = new StringBuilder("<?xml version='1.0' encoding='utf-8'?>");
            result.Append("<wp:Notification xmlns:wp='WPNotification'>");
                result.Append("<wp:" + PushNotification.PushNotificationTypeToString(this.NotificationType) + "");
                    result.Append("<wp:Text1>");
                        result.Append(this.Title);
                    result.Append("</wp:Text1>");
                    result.Append("<wp:Text2>");
                        result.Append(this.Text);
                    result.Append("</wp:Text2>");
                    result.Append("<wp:Param>");
                        result.Append(this.PageName);
                    result.Append("</wp:Param>");
                result.Append("</wp:" + PushNotification.PushNotificationTypeToString(this.NotificationType) + ">");
            result.Append("</wp:Notification>");
            return result.ToString();*/


            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<wp:Notification xmlns:wp=\"WPNotification\">" +
                   "<wp:Toast>" +
                        "<wp:Text1>" + this.Title + "</wp:Text1>" +
                        "<wp:Text2>" + this.Text + "</wp:Text2>" +
                        "<wp:Param>/MainPage.xaml?NavigatedFrom=Toast Notification</wp:Param>" +
                   "</wp:Toast> " +
                "</wp:Notification>";
                
                

        }


        public override string ToString()
        {
            return this.Render();
        }

        public string ToBase64String()
        {
            return this.ToBase64(this.Render()).Replace("+", "%2B");
        }

        public static string PushNotificationTypeToString(PushNotificationType type)
        {
            switch (type)
            {
                case PushNotificationType.Raw:
                    {
                        return "Raw";
                    }
                case PushNotificationType.Tile:
                    {
                        return "Tile";
                    }
                case PushNotificationType.Toast:
                    {
                        return "Toast";
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }

        private string ToBase64(string str)
        {
            return System.Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(str));
        }

        private string FromBase64(string str)
        {
            byte[] tr = System.Convert.FromBase64String(str);
            // string result = System.Text.UTF8Encoding.UTF8.GetString(tr);
            UTF8Encoding encoder = new UTF8Encoding();
            string result = encoder.GetString(tr, 0, tr.Length);
            return result;

        }



    }
}
