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
using QuickBloxSDK_Silverlight.Core;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text;

namespace QuickBloxSDK_Silverlight.PushNotification
{
    /// <summary>
    /// Класс для работы Push Notification сервиса QuickBLox
    /// </summary>
    public class PushNotificationService
    {
        /// <summary>
        /// Главный делегат
        /// </summary>
        /// <param name="Args"></param>
        public delegate void PushNotificationServiceHandler(PushNotificationEventArgs Args);

        /// <summary>
        /// Событие связанное с ответом сервера на запрос.
        /// </summary>
        public event PushNotificationServiceHandler PushNotificationServiceEvent;

        /// <summary>
        /// Альтернативный адрес сервера QuickBlox
        /// </summary>
        public string CustomServerAddr
        { get; set; }

        /// <summary>
        /// Подключиноли приложение к серверу
        /// </summary>
        public bool IsOnline
        {
            get;
            set;
        }

        /// <summary>
        /// Идентификтаор владельца приложения
        /// </summary>
        public int OwnerId
        { get; set; }

        /// <summary>
        /// Контекст подключения к серверу
        /// </summary>
        private ConnectionContext Сontext;
        /// <summary>
        /// Подключает подсервис Push Notification  к серверу
        /// </summary>
        /// <param name="context"></param>
        public PushNotificationService(ConnectionContext context)
        {
            Сontext = context;
            this.Сontext.RequestResult +=new ConnectionContext.Main((Result result)=>{

                if (
                    !(result.ServerName + result.ControllerName).Contains(Helper.PartToServerName(Part.push_tokens, this.CustomServerAddr))  &&
                    !(result.ServerName + result.ControllerName).Contains(Helper.PartToServerName(Part.events, this.CustomServerAddr))  &&
                    !(result.ServerName + result.ControllerName).Contains(Helper.PartToServerName(Part.subscriptions, this.CustomServerAddr))
                    )
                    return;

                switch (result.Verbs)
                {
                    case AcceptVerbs.GET:
                        {
                            if (result.ControllerName.Contains("subscriptions"))
                            {
                                this.GetSubscriptions_Response(result);
                                return;
                            }
                            break;
                        }
                    case AcceptVerbs.DELETE:
                        {
                            if (result.ControllerName.Contains("push_tokens"))
                            {
                               this.DeletePushToken_Response(result);
                                return;
                            }
                            if (result.ControllerName.Contains("subscriptions"))
                            {
                                this.DeleteSubscribe_Response(result);
                                return;
                            }
                            break;
                        }
                    case AcceptVerbs.PUT:
                        {
                           
                            break;
                        }
                    case AcceptVerbs.POST:
                        {
                            if (result.ControllerName.Contains("push_tokens"))
                            {
                                this.CreatePushToken_Response(result);
                                return;
                            }
                            if (result.ControllerName.Contains("subscriptions"))
                            {
                                this.CreateSubscription_Response(result);
                                return;
                            }
                            if (result.ControllerName.Contains("events"))
                            {
                                this.CreateEvent_Response(result);
                                return;
                            }
                            break;
                        }
                }

            });
        }

        public string DeviceId
        { get; set; }

        private void CreatePushToken_Response(Result result)
        {
            if (PushNotificationServiceEvent != null)
            {
                if (result.ResultStatus == Status.OK)
                {
                    try
                    {

                        this.PushNotificationServiceEvent(new PushNotificationEventArgs
                        {
                            result = new PushToken(result.Content),
                            t = typeof(PushToken),
                            status = result.ResultStatus,
                            currentCommand = PushNotificationCommand.CreatePushToken,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex) // ошибка распарсивания
                    {
                        this.PushNotificationServiceEvent(new PushNotificationEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = PushNotificationCommand.CreatePushToken,
                            errorMessage = ex.Message
                        });
                    }
                }
                // нет контента из за ошибок
                else if (result.ResultStatus == Status.StreamError
                    || result.ResultStatus == Status.TimeoutError
                    || result.ResultStatus == Status.UnknownError
                    || result.ResultStatus == Status.NotFoundError
                    || result.ResultStatus == Status.AccessDenied
                    || result.ResultStatus == Status.ConnectionError
                    || result.ResultStatus == Status.NotAcceptable
                    || result.ResultStatus == Status.AuthenticationError)
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = PushNotificationCommand.CreatePushToken,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = PushNotificationCommand.CreatePushToken,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = PushNotificationCommand.CreatePushToken,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        public void CreatePushToken(string ChanelName)
        {
            this.Сontext.CurrentPart = Part.push_tokens;

            this.Сontext.Add("push_token[environment]", "production");
            this.Сontext.Add("push_token[client_identification_sequence]", ChanelName);

            this.Сontext.SendAsyncRequest(".xml", AcceptVerbs.POST);
        }

        private void CreateSubscription_Response(Result result)
        {
            if (PushNotificationServiceEvent != null)
            {
                if (result.ResultStatus == Status.OK)
                {
                    try
                    {
                        List<Subscription> raccoon = new List<Subscription>();

                        XElement xml = XElement.Parse(result.Content);
                        foreach (var t in xml.Descendants("subscription"))
                            raccoon.Add(new Subscription(t.ToString()));

                        this.PushNotificationServiceEvent(new PushNotificationEventArgs
                        {
                            result = raccoon.ToArray(),
                            t = typeof(Subscription[]),
                            status = result.ResultStatus,
                            currentCommand = PushNotificationCommand.CreateSubscriptions,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex)
                    {
                        this.PushNotificationServiceEvent(new PushNotificationEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = PushNotificationCommand.CreateSubscriptions,
                            errorMessage = ex.Message
                        });
                    }
                }
                // нет контента из за ошибок
                else if (result.ResultStatus == Status.StreamError
                    || result.ResultStatus == Status.TimeoutError
                    || result.ResultStatus == Status.UnknownError
                    || result.ResultStatus == Status.NotFoundError
                    || result.ResultStatus == Status.AccessDenied
                    || result.ResultStatus == Status.NotAcceptable
                    || result.ResultStatus == Status.ConnectionError
                    || result.ResultStatus == Status.AuthenticationError)
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = PushNotificationCommand.CreateSubscriptions,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = PushNotificationCommand.CreateSubscriptions,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = PushNotificationCommand.CreateSubscriptions,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        public void CreateSubscription(string Url, NotificationType Type)
        {
            this.Сontext.CurrentPart = Part.subscriptions;
            string t;

            switch (Type)
            {
                case NotificationType.apns:
                    {
                        t = "apns";
                        break;
                    }
                case NotificationType.c2dm:
                    {
                        t = "c2dm";
                        break;
                    }
                case NotificationType.email:
                    {
                        t = "email";
                        break;
                    }
                case NotificationType.mpns:
                    {
                        t = "mpns";
                        break;
                    }
                case NotificationType.pull:
                    {
                        t = "pull";
                        break;
                    }
                default:
                    {
                        t = string.Empty;
                        break;
                    }
            }

            this.Сontext.Add("notification_channels", t);
            this.Сontext.Add("url", Url);

            this.Сontext.SendAsyncRequest(".xml", AcceptVerbs.POST);
        }

        private void GetSubscriptions_Response(Result result)
        {
            if (PushNotificationServiceEvent != null)
            {
                if (result.ResultStatus == Status.OK)
                {
                    try
                    {

                        List<Subscription> raccoon = new List<Subscription>();

                        XElement xml = XElement.Parse(result.Content);
                        foreach (var t in xml.Descendants("subscription"))
                            raccoon.Add(new Subscription(t.ToString()));

                        this.PushNotificationServiceEvent(new PushNotificationEventArgs
                        {
                            result = raccoon.ToArray(),
                            t = typeof(Subscription[]),
                            status = result.ResultStatus,
                            currentCommand = PushNotificationCommand.GetSubscriptions,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex)
                    {
                        this.PushNotificationServiceEvent(new PushNotificationEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = PushNotificationCommand.GetSubscriptions,
                            errorMessage = ex.Message
                        });
                    }
                }
                // нет контента из за ошибок
                else if (result.ResultStatus == Status.StreamError
                    || result.ResultStatus == Status.TimeoutError
                    || result.ResultStatus == Status.UnknownError
                    || result.ResultStatus == Status.NotFoundError
                    || result.ResultStatus == Status.AccessDenied
                    || result.ResultStatus == Status.NotAcceptable
                    || result.ResultStatus == Status.ConnectionError
                    || result.ResultStatus == Status.AuthenticationError)
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = PushNotificationCommand.GetSubscriptions,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = PushNotificationCommand.GetSubscriptions,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = PushNotificationCommand.GetSubscriptions,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        public void GetSubscriptions()
        {
            this.Сontext.CurrentPart = Part.subscriptions;
            this.Сontext.SendAsyncRequest(".xml", AcceptVerbs.GET);
        }

        private void DeletePushToken_Response(Result result)
        {
            if (PushNotificationServiceEvent != null)
            {
                if (result.ResultStatus == Status.OK)
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                        {
                            result = null,
                            t = null,
                            status = result.ResultStatus,
                            currentCommand = PushNotificationCommand.DeletePushToken,
                            errorMessage = result.ErrorMessage
                        });
                   
                }
                // нет контента из за ошибок
                else if (result.ResultStatus == Status.StreamError
                    || result.ResultStatus == Status.TimeoutError
                    || result.ResultStatus == Status.UnknownError
                    || result.ResultStatus == Status.NotFoundError
                    || result.ResultStatus == Status.AccessDenied
                    || result.ResultStatus == Status.NotAcceptable
                    || result.ResultStatus == Status.ConnectionError
                    || result.ResultStatus == Status.AuthenticationError)
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = PushNotificationCommand.DeletePushToken,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = PushNotificationCommand.DeletePushToken,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = PushNotificationCommand.DeletePushToken,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        public void DeletePushToken(uint id)
        {
            this.Сontext.CurrentPart = Part.push_tokens;
            this.Сontext.SendAsyncRequest("/" + id.ToString(), AcceptVerbs.DELETE);
        }

        private void DeleteSubscribe_Response(Result result)
        {
            if (PushNotificationServiceEvent != null)
            {
                if (result.ResultStatus == Status.OK)
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = PushNotificationCommand.DeleteSubscribe,
                        errorMessage = result.ErrorMessage
                    });

                }
                // нет контента из за ошибок
                else if (result.ResultStatus == Status.StreamError
                    || result.ResultStatus == Status.TimeoutError
                    || result.ResultStatus == Status.UnknownError
                    || result.ResultStatus == Status.NotFoundError
                    || result.ResultStatus == Status.AccessDenied
                    || result.ResultStatus == Status.NotAcceptable
                    || result.ResultStatus == Status.ConnectionError
                    || result.ResultStatus == Status.AuthenticationError)
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = PushNotificationCommand.DeleteSubscribe,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = PushNotificationCommand.DeleteSubscribe,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = PushNotificationCommand.DeleteSubscribe,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        public void DeleteSubscribe(uint id)
        {
            this.Сontext.CurrentPart = Part.subscriptions;
            this.Сontext.SendAsyncRequest("/" + id.ToString(), AcceptVerbs.DELETE);
        }

        private void CreateEvent_Response(Result result)
        {
            if (PushNotificationServiceEvent != null)
            {
                if (result.ResultStatus == Status.OK)
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = PushNotificationCommand.CreateEvent,
                        errorMessage = result.ErrorMessage
                    });

                }
                // нет контента из за ошибок
                else if (result.ResultStatus == Status.StreamError
                    || result.ResultStatus == Status.TimeoutError
                    || result.ResultStatus == Status.UnknownError
                    || result.ResultStatus == Status.NotFoundError
                    || result.ResultStatus == Status.AccessDenied
                    || result.ResultStatus == Status.NotAcceptable
                    || result.ResultStatus == Status.ConnectionError
                    || result.ResultStatus == Status.AuthenticationError)
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = PushNotificationCommand.CreateEvent,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = PushNotificationCommand.CreateEvent,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.PushNotificationServiceEvent(new PushNotificationEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = PushNotificationCommand.CreateEvent,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        public void CreateEvent(int[] UserIds, string Message)
        {
            try
            {
                this.Сontext.CurrentPart = Part.events;
                this.Сontext.Add("event[environment]", "production");
                this.Сontext.Add("event[push_type]", "mpns");
                this.Сontext.Add("event[notification_type]", "push");
                if(UserIds != null)
                    if (UserIds.Length > 1)
                    {
                        this.Сontext.Add("event[user_ids]", string.Join(",", UserIds.Select(t => t.ToString()).ToArray()));
                    }
                    else if (UserIds.Length == 1)
                    {
                        this.Сontext.Add("event[user_ids]", UserIds[0].ToString());
                    }
                    else if (UserIds.Length == 0)
                    {
                        this.Сontext.Add("event[user_ids]", "0");
                    }
                this.Сontext.Add("event[message]", Message);
                 this.Сontext.SendAsyncRequest(".xml", AcceptVerbs.POST);
            }
            catch
            { }
        }

    }
}
