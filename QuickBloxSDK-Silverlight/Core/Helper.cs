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
    /// Служебный класс.
    /// Выполняет исключительно вспомогательные операции.
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Переводит тип запроса в строку
        /// </summary>
        /// <param name="acceptVerbs">Тп\ип запроса</param>
        /// <returns>Тип запроса в строковом виде</returns>
        public static string AcceptVerbsToString(AcceptVerbs acceptVerbs)
        {
            switch (acceptVerbs)
            {
                case AcceptVerbs.DELETE:
                    {
                        return "DELETE";
                       
                    }
                case AcceptVerbs.GET:
                    {
                        return "GET";

                    }
                case AcceptVerbs.HEAD:
                    {
                        return "HEAD";

                    }
                case AcceptVerbs.OPTIONS:
                    {
                        return "OPTIONS";

                    }
                case AcceptVerbs.POST:
                    {
                        return "POST";

                    }
                case AcceptVerbs.PUT:
                    {
                        return "PUT";
                    }
                case AcceptVerbs.TRACE:
                    {
                        return "TRACE";
                    }
            }
            return null;
        }

        /// <summary>
        /// Переводит текстовое значение заголовка в AcceptVerbs
        /// </summary>
        /// <param name="acceptVerbs"></param>
        /// <returns></returns>
        public static AcceptVerbs StringToAcceptVerbs( string acceptVerbs)
        {
            switch (acceptVerbs)
            {
                case "DELETE":
                    {
                        return AcceptVerbs.DELETE;

                    }
                case "GET":
                    {
                        return AcceptVerbs.GET;

                    }
                case "HEAD":
                    {
                        return AcceptVerbs.HEAD;

                    }
                case "OPTIONS":
                    {
                        return AcceptVerbs.OPTIONS;

                    }
                case "POST":
                    {
                        return AcceptVerbs.POST;

                    }
                case "PUT":
                    {
                        return AcceptVerbs.PUT;
                    }
                case "TRACE":
                    {
                        return AcceptVerbs.TRACE ;
                    }
            }
            return AcceptVerbs.GET;
        }


        /// <summary>
        /// Переводит раздел в URI
        /// </summary>
        /// <param name="part">Раздел котороего нужно получить адрес сервера</param>
        /// <returns>URI сервера</returns>
        public static string PartToServerName(Part part, string CustomServerAddr)
        {
            switch (part)
            {
                case Part.blobs:
                    {
                        if (string.IsNullOrEmpty(CustomServerAddr))
                            return "api.quickblox.com";
                        else if (CustomServerAddr.Contains("*"))
                            return CustomServerAddr.Replace("*", "blobs2");
                        else
                            return "blobs2." + CustomServerAddr;
                    }
                case Part.push_tokens:
                    {
                        if (string.IsNullOrEmpty(CustomServerAddr))
                            return "api.quickblox.com/push_tokens";
                        else if (CustomServerAddr.Contains("*"))
                            return CustomServerAddr.Replace("*", "location");
                        else
                            return "location." + CustomServerAddr;
                    }
                case Part.subscriptions:
                    {
                        if (string.IsNullOrEmpty(CustomServerAddr))
                            return "api.quickblox.com/subscriptions";
                        else if (CustomServerAddr.Contains("*"))
                            return CustomServerAddr.Replace("*", "location");
                        else
                            return "location." + CustomServerAddr;
                    }
                case Part.events:
                    {
                        if (string.IsNullOrEmpty(CustomServerAddr))
                            return "api.quickblox.com/events";
                        else if (CustomServerAddr.Contains("*"))
                            return CustomServerAddr.Replace("*", "location");
                        else
                            return "location." + CustomServerAddr;
                    }
                case Part.geopos:
                    {
                        if (string.IsNullOrEmpty(CustomServerAddr))
                            return "api.quickblox.com/geodata";
                        else if (CustomServerAddr.Contains("*"))
                            return CustomServerAddr.Replace("*", "location");
                        else
                            return "location." + CustomServerAddr;
                    }
                case Part.places:
                    {
                        if (string.IsNullOrEmpty(CustomServerAddr))
                            return "api.quickblox.com/places";
                        else if (CustomServerAddr.Contains("*"))
                            return CustomServerAddr.Replace("*", "location");
                        else
                            return "location." + CustomServerAddr;
                    }
                case Part.messaging:
                    {
                        if (string.IsNullOrEmpty(CustomServerAddr))
                            return "api.quickblox.com";
                        else if (CustomServerAddr.Contains("*"))
                            return CustomServerAddr.Replace("*", "messages");
                        else
                            return "messages." + CustomServerAddr;
                    }
                case Part.ratings:
                    {
                        return "";
                    }
                case Part.users:
                    {
                        if (string.IsNullOrEmpty(CustomServerAddr))
                            return "api.quickblox.com/users";
                        else if (CustomServerAddr.Contains("*"))
                            return CustomServerAddr.Replace("*", "users");
                        else
                            return "users." + CustomServerAddr;
                    }
                case Part.Admin:
                    {
                        if (string.IsNullOrEmpty(CustomServerAddr))
                            return "admin.quickblox.com";
                        else if (CustomServerAddr.Contains("*"))
                            return CustomServerAddr.Replace("*", "admin");
                        else
                            return "admin." + CustomServerAddr;
                    }
            }
            return null;
        }

        /// <summary>
        /// Полчает на входе заголовки ответа от сервера и выбирает из них статус
        /// </summary>
        /// <param name="headers">Заголовки ответа</param>
        /// <returns>Статус</returns>
        public static Status HeaderToStatus(Header[] headers)
        {
            try
            {
                string statusCode = null;
                foreach (var t in headers)
                    if (t.Name == "Status")
                        statusCode = t.Value;

                return Helper.StringToStatus(statusCode);

            }
            catch
            {
                return Status.none;
            }
           
        }

        /// <summary>
        /// Переводит строку в статус
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static Status StringToStatus(string statusCode)
        {
            if (string.IsNullOrEmpty(statusCode))
                return Status.none;

            if (statusCode.IndexOf("20") != -1 )
                return Status.OK;

            if (statusCode.IndexOf("404") != -1 || statusCode.IndexOf("NotFound") != -1)
                return Status.NotFoundError;

            if (statusCode.IndexOf("401") != -1 || statusCode.IndexOf("Unauthorized") != -1)
                return Status.Unauthorized;

            if (statusCode.IndexOf("401") != -1)
                return Status.AuthenticationError;

            if (statusCode.IndexOf("405") != -1 || statusCode.IndexOf("MethodNotAllowed") != -1)
                return Status.MethodNotAllowed;

            if (statusCode.IndexOf("406") != -1 || statusCode.IndexOf("NotAcceptable") != -1)
                return Status.NotAcceptable;
            
            if (statusCode.IndexOf("403") != -1 )
                return Status.AccessDenied;

            if (statusCode.IndexOf("422") != -1)
                return Status.ValidationError;


            return Status.none;
        }      

    }
}
