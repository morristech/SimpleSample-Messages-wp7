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
using System.Collections.Generic;
using QuickBloxSDK_Silverlight.Core;
using System.Xml.Linq;

namespace QuickBloxSDK_Silverlight.Geo
{

    /// <summary>
    /// Класс предоставляющий возможность работать с местоположениями сервиса QuickBlox.
    /// </summary>
    public class GeoService
    {
       /// <summary>
       /// Делегат сервиса
       /// </summary>
       /// <param name="Args">Response data</param>
        public delegate void GeoServiceHandler(GeoServiceEventArgs Args);

        /// <summary>
        /// Событие связанное с ответом от сервера и касающиеся местоположений.
        /// </summary>
        public event GeoServiceHandler GeoServiceEvent;

        /// <summary>
        /// Альтернативный адрес сервера QuickBlox
        /// </summary>
        public string CustomServerAddr
        { get; set; }

         /// <summary>
        /// Контекст подключения
        /// </summary>
        private ConnectionContext Сontext;

        /// <summary>
        /// Создает подключения к сервису.
        /// </summary>
        /// <param name="context"></param>
        public GeoService(ConnectionContext context)
        {
            Сontext = context;
            this.Сontext.RequestResult += new ConnectionContext.Main((Result result) =>
            {

                if (!(result.ServerName + result.ControllerName).Contains(Helper.PartToServerName(Part.geopos, this.CustomServerAddr)))
                    return;

                switch (result.Verbs)
                {
                    case AcceptVerbs.GET:
                        {
                            if (result.ControllerName.Contains("/geodata/find") && result.URI.Contains("app.id") && result.URI.Contains("user.id"))
                            {
                                this.GetGeoLocationsForUser_Response(result);
                                return;
                            }
                            if (result.ControllerName.Contains("/geodata/find") && result.URI.Contains("app.id") && !result.URI.Contains("user.id"))
                            {
                                this.GetGeoLocationsForApp_Response(result);
                                return;
                            }
                            if (result.ControllerName.Contains("/geodata/find") && result.URI.Contains("id") && !result.URI.Contains("user.id") && !result.URI.Contains("app.id"))
                            {
                                this.GetGeoLocation_Response(result);
                                return;
                            }
                            break;
                        }
                    case AcceptVerbs.DELETE:
                        {
                            
                            break;
                        }
                    case AcceptVerbs.PUT:
                        {
                            
                            break;
                        }
                    case AcceptVerbs.POST:
                        {
                            
                            if (result.ControllerName.Contains("/geodata"))
                            {
                                this.AddGeoLocation_Response(result);
                                return;
                            }
                            break;
                        }
                }


            });
        }



        private void AddGeoLocation_Response(Result result)
        {
            if (GeoServiceEvent != null)
            {
                if (result.ResultStatus == Status.OK)
                {
                    try
                    {
                        this.GeoServiceEvent(new GeoServiceEventArgs
                        {
                            result = new GeoData(result.Content),
                            t = typeof(GeoData),
                            status = result.ResultStatus,
                            currentCommand = GeoServiceCommand.AddGeoLocation,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex)
                    {
                        this.GeoServiceEvent(new GeoServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = GeoServiceCommand.AddGeoLocation,
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
                    || result.ResultStatus == Status.Unauthorized
                    || result.ResultStatus == Status.ConnectionError
                    || result.ResultStatus == Status.NotAcceptable
                    || result.ResultStatus == Status.AuthenticationError)
                    this.GeoServiceEvent(new GeoServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = GeoServiceCommand.AddGeoLocation,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.GeoServiceEvent(new GeoServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = GeoServiceCommand.AddGeoLocation,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.GeoServiceEvent(new GeoServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = GeoServiceCommand.AddGeoLocation,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        /// <summary>
        /// Создатьновое местоположение
        /// </summary>
        /// <param name="data">Местоположение</param>
        public void AddGeoLocation(GeoData data)
        {
            if (data == null)
                return;


            this.Сontext.CurrentPart = Part.geopos;
           /* this.Сontext.Add("geo_data[user_id]", data.UserId.ToString());
            this.Сontext.Add("geo_data[application_id]", this.Сontext.ApplicationId.ToString());*/

            if (!string.IsNullOrEmpty(data.Status))
                this.Сontext.Add("geo_data[status]", data.Status);


            if (data.Latitude.ToString().Contains(","))
                this.Сontext.Add("geo_data[latitude]", data.Latitude.ToString().Replace(',', '.'));
            else
                this.Сontext.Add("geo_data[latitude]", data.Latitude.ToString());


            if (data.Longitude.ToString().Contains(","))
                this.Сontext.Add("geo_data[longitude]", data.Longitude.ToString().Replace(',', '.'));
            else
                this.Сontext.Add("geo_data[longitude]", data.Longitude.ToString());


            this.Сontext.SendAsyncRequest(".xml", AcceptVerbs.POST);
        }

        private void GetGeoLocation_Response(Result result)
        {
            if (GeoServiceEvent != null) 
            {
                if (result.ResultStatus == Status.OK) 
                {
                    try 
                    {
                        List<GeoData> users = new List<GeoData>();
                        XElement xml = XElement.Parse((string)result.Content);
                        foreach (var t in xml.Descendants("geo-datum"))
                            users.Add(new GeoData(t.ToString()));

                        this.GeoServiceEvent(new GeoServiceEventArgs
                        {
                            result = users.Count > 0 ? users[0] : null,
                            t = typeof(GeoData),
                            status = result.ResultStatus,
                            currentCommand = GeoServiceCommand.GetGeoLocation,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex) // ошибка распарсивания
                    {
                        this.GeoServiceEvent(new GeoServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = GeoServiceCommand.GetGeoLocation,
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
                    || result.ResultStatus == Status.AuthenticationError)
                    this.GeoServiceEvent(new GeoServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = GeoServiceCommand.GetGeoLocation,
                        errorMessage = result.ErrorMessage
                    });

            }
        }

       /// <summary>
       /// Получить местоположение по идентификатору
       /// </summary>
       /// <param name="id">Тдентификатор местополодения в базе данных.</param>
        public void GetGeoLocation(int id)
        {
            this.Сontext.CurrentPart = Part.geopos;
            this.Сontext.Add("id", id.ToString());
            this.Сontext.SendAsyncRequest("/find", AcceptVerbs.GET); // параметры запроса
           
        }

        
        private void GetGeoLocationsForUser_Response(Result result)
        {
            if (GeoServiceEvent != null)
            {
                if (result.ResultStatus == Status.OK)
                {
                    try
                    {
                        GeoPage page = new GeoPage();

                        List<GeoData> users = new List<GeoData>();
                        XElement xml = XElement.Parse((string)result.Content);
                        foreach (var t in xml.Descendants("geo-datum"))
                            users.Add(new GeoData(t.ToString()));

                        page.GeoLocations = users.ToArray();
                        try
                        {
                            page.CurrentPage = int.Parse(xml.Attribute("current_page").Value);
                            page.LocationsOnPage = int.Parse(xml.Attribute("per_page").Value);
                            page.TotalEntries = int.Parse(xml.Attribute("total_entries").Value);
                        }
                        catch { }
                        this.GeoServiceEvent(new GeoServiceEventArgs
                        {
                            result = page,
                            t = typeof(GeoPage),
                            status = result.ResultStatus,
                            currentCommand = GeoServiceCommand.GetGeoLocationsForUser,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex) // ошибка распарсивания
                    {
                        this.GeoServiceEvent(new GeoServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = GeoServiceCommand.GetGeoLocationsForUser,
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
                    this.GeoServiceEvent(new GeoServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = GeoServiceCommand.GetGeoLocationsForUser,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.GeoServiceEvent(new GeoServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = GeoServiceCommand.GetGeoLocationsForUser,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.GeoServiceEvent(new GeoServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = GeoServiceCommand.GetGeoLocationsForUser,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        /// <summary>
        /// (Устаревший)
        /// Получение выборки местоположений для пользователя.
        /// </summary>
        /// <param name="UserId">Идентификатор пользователя в базе данных</param>
        /// <param name="Page">Номер страници</param>
        /// <param name="Settings">Настройки выборки</param>
        public void GetGeoLocationsForUser(int UserId, int Page, SelectionSettings Settings)
        {
            SelectionSettings sett = Settings ?? new SelectionSettings();
            this.Сontext.CurrentPart = Part.geopos;
            this.Сontext.Add("page", Page.ToString());
            this.Сontext.Add("page_size", (sett.PageSize < 10 ? 100 : sett.PageSize).ToString());
            this.Сontext.Add("app.id", this.Сontext.ApplicationId.ToString());

            if(sett.sortType == SortType.SortAsc)
                this.Сontext.Add("sort_asc", "1");
            else
                this.Сontext.Add("sort_by", "1");


            switch (sett.sortField)
            {
                case SortField.Date:
                    {
                        this.Сontext.Add("created_at", "1");
                        break;
                    }
                case SortField.Distance:
                    {
                        this.Сontext.Add("distance", "1");
                        break;
                    }
                case SortField.Latitude:
                    {
                        this.Сontext.Add("latitude", "1");
                        break;
                    }
                case SortField.Longitude:
                    {
                        this.Сontext.Add("longitude", "1");
                        break;
                    }
            }


           if (sett.IsLastOnly)
                this.Сontext.Add("last_only", "1");

            this.Сontext.Add("user.id", UserId.ToString());
            this.Сontext.SendAsyncRequest("/find.xml", AcceptVerbs.GET); // параметры запроса
           
        }

        /// <summary>
        /// Получить местоположения привязанные к конкретному пользователю
        /// </summary>
        /// <param name="UserId">Идентифкатор пользователя в базе пользователей</param>
        /// <param name="CurrentPage">Номер старници</param>
        /// <param name="PerPage">Колличество записей на странице</param>
        public void GetGeoLocationsForUser(int UserId, int CurrentPage, int PerPage)
        {
            this.Сontext.CurrentPart = Part.geopos;
            this.Сontext.Add("page", CurrentPage.ToString());
            this.Сontext.Add("per_page", PerPage.ToString());
            this.Сontext.Add("app.id", this.Сontext.ApplicationId.ToString());
            this.Сontext.Add("user.id", UserId.ToString());
            this.Сontext.SendAsyncRequest("/find.xml", AcceptVerbs.GET); // параметры запроса

        }

        private void GetGeoLocationsForApp_Response(Result result)
        {
            if (GeoServiceEvent != null)
            {
                if (result.ResultStatus == Status.OK)
                {
                    try
                    {
                        GeoPage page = new GeoPage();

                        List<GeoData> users = new List<GeoData>();
                        XElement xml = XElement.Parse((string)result.Content);
                        foreach (var t in xml.Descendants("geo-datum"))
                            users.Add(new GeoData(t.ToString()));

                        page.GeoLocations = users.ToArray();
                        try
                        {
                            page.CurrentPage = int.Parse(xml.Attribute("current_page").Value);
                            page.LocationsOnPage = int.Parse(xml.Attribute("per_page").Value);
                            page.TotalEntries = int.Parse(xml.Attribute("total_entries").Value);
                        }
                        catch {}

                        this.GeoServiceEvent(new GeoServiceEventArgs
                        {
                            result = page,
                            t = typeof(GeoPage),
                            status = result.ResultStatus,
                            currentCommand = GeoServiceCommand.GetGeoLocationsForApp,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex) // ошибка распарсивания
                    {
                        this.GeoServiceEvent(new GeoServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = GeoServiceCommand.GetGeoLocationsForApp,
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
                    this.GeoServiceEvent(new GeoServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = GeoServiceCommand.GetGeoLocationsForApp,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.GeoServiceEvent(new GeoServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = GeoServiceCommand.GetGeoLocationsForApp,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.GeoServiceEvent(new GeoServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = GeoServiceCommand.GetGeoLocationsForApp,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }


        /// <summary>
        /// (Устаревший)
        /// Выборка  метосположений принадлежащих текущему приложению.
        /// </summary>
        /// <param name="Page">Номер страници</param>
        /// <param name="Settings">Параметры выборки</param>
        public void GetGeoLocationsForApp(int Page, SelectionSettings Settings)
        {
            SelectionSettings sett = Settings ?? new SelectionSettings();
            this.Сontext.CurrentPart = Part.geopos;
            this.Сontext.Add("page", Page.ToString());
            this.Сontext.Add("per_page", (sett.PageSize < 10 ? 100 : sett.PageSize).ToString());

            if (sett.sortType == SortType.SortAsc)
                this.Сontext.Add("sort_asc", "1");
            else
                this.Сontext.Add("sort_by", "1");

            this.Сontext.Add("app.id", this.Сontext.ApplicationId.ToString());
            this.Сontext.SendAsyncRequest("/find.xml", AcceptVerbs.GET);

        }

        /// <summary>
        /// Получить геолокации для прилоежния
        /// </summary>
        /// <param name="CurrentPage">Текущая страница</param>
        /// <param name="PerPage">Колличество записей на странице</param>
        public void GetGeoLocationsForApp(int CurrentPage, int PerPage)
        {
            this.Сontext.CurrentPart = Part.geopos;
            this.Сontext.Add("page", CurrentPage.ToString());
            this.Сontext.Add("per_page", PerPage.ToString());
            this.Сontext.Add("app.id", this.Сontext.ApplicationId.ToString());
            this.Сontext.SendAsyncRequest("/find.xml", AcceptVerbs.GET);

        }

        
        
    }
}
