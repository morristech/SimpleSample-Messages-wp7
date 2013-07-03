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

namespace QuickBloxSDK_Silverlight.Places
{
    /// <summary>
    /// Класс предастовляет возможность работать с сервисами "Места" в QuickBlox
    /// </summary>
    public class PlaceService
    {
        /// <summary>
        /// Главный делегат
        /// </summary>
        /// <param name="Args"></param>
        public delegate void PlaceServiceHandler(PlaceServiceEventArgs Args);

        /// <summary>
        /// Событие связанное с ответом сервера на запрос.
        /// </summary>
        public event PlaceServiceHandler PlaceServiceEvent;

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
        /// Подключает подсервис "Места" к серверу
        /// </summary>
        /// <param name="context"></param>
        public PlaceService(ConnectionContext context)
        {
            Сontext = context;
            this.Сontext.RequestResult +=new ConnectionContext.Main((Result result)=>{

              if (!(result.ServerName + result.ControllerName).Contains(Helper.PartToServerName(Part.places, this.CustomServerAddr)))
                    return;

                switch (result.Verbs)
                {
                    case AcceptVerbs.GET:
                        {
                            if (result.ControllerName.Contains("/places.xml"))
                            {
                                this.GetPlacesForApp_Response(result);
                                return;
                            }
                            if (result.ControllerName.Contains("/places/") && result.ControllerName.Contains(".xml"))
                            {
                                this.GetPlace_Response(result);
                                return;
                            }
                            break;
                        }
                    case AcceptVerbs.DELETE:
                        {
                            if (result.ControllerName.Contains("/places/"))
                            {
                                this.DeletePlace_Response(result);
                                return;
                            }
                            break;
                        }
                    case AcceptVerbs.PUT:
                        {
                            if (result.ControllerName.Contains("/places/"))
                            {
                                this.EditPlace_Response(result);
                                return;
                            }
                            break;
                        }
                    case AcceptVerbs.POST:
                        {
                            if (result.ControllerName.Contains("/places.xml"))
                            {
                                this.AddPlace_Response(result);
                                return;
                            }
                            break;
                        }
                }

            });
        }





        private void GetPlacesForApp_Response(Result result)
        {
            if (PlaceServiceEvent != null)
            {
                if (result.ResultStatus == Status.OK)
                {
                    try
                    {
                        this.PlaceServiceEvent(new PlaceServiceEventArgs
                        {
                            result = new PlacePage(result.Content),
                            t = typeof(PlacePage),
                            status = result.ResultStatus,
                            currentCommand = PlaceServiceCommand.GetPlacesByApp,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex) // ошибка распарсивания
                    {
                        this.PlaceServiceEvent(new PlaceServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = PlaceServiceCommand.GetPlacesByApp,
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
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = PlaceServiceCommand.GetPlacesByApp,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = PlaceServiceCommand.GetPlacesByApp,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = PlaceServiceCommand.GetPlacesByApp,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        /// <summary>
        /// Получить все места для приложения
        /// </summary>
        /// <param name="CurrentPage">Зарпшиваемая старница</param>
        /// <param name="PerPage">Колличество записей на страницу</param>
        public void GetGetPlacesForApp(int CurrentPage, int PerPage)
        {
            this.Сontext.CurrentPart = Part.places;
            this.Сontext.Add("page", CurrentPage.ToString());
            this.Сontext.Add("per_page", PerPage.ToString());
            this.Сontext.Add("app.id", this.Сontext.ApplicationId.ToString());
            this.Сontext.SendAsyncRequest(".xml", AcceptVerbs.GET);

        }

        private void AddPlace_Response(Result result)
        {
            if (PlaceServiceEvent != null) // если привязан обработчик
            {
                if (result.ResultStatus == Status.OK) // если всё хорошо и пришол контент
                {
                    try
                    {

                        this.PlaceServiceEvent(new PlaceServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = result.ResultStatus,
                            currentCommand = PlaceServiceCommand.AddPlace,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex) // ошибка распарсивания
                    {
                        this.PlaceServiceEvent(new PlaceServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = PlaceServiceCommand.AddPlace,
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
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = PlaceServiceCommand.AddPlace,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = PlaceServiceCommand.AddPlace,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = PlaceServiceCommand.AddPlace,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }
        
        /// <summary>
        /// Создать новое место
        /// </summary>
        /// <param name="place"></param>
        public void AddPlace(Place place)
        {
            this.Сontext.CurrentPart = Part.places;

            this.Сontext.Add("place[photo_id]", place.PhotoId.ToString());
            this.Сontext.Add("place[geo_data_id]", place.GeoDataId.ToString());
            this.Сontext.Add("place[title]", place.Title);

            if (!string.IsNullOrEmpty(place.Description))
                this.Сontext.Add("place[description]", place.Description);
            if (!string.IsNullOrEmpty(place.Adrress))
                this.Сontext.Add("place[address]", place.Adrress);

            this.Сontext.SendAsyncRequest(".xml", AcceptVerbs.POST);
        }

        private void GetPlace_Response(Result result)
        {
            if (PlaceServiceEvent != null) // если привязан обработчик
            {
                if (result.ResultStatus == Status.OK) // если всё хорошо и пришол контент
                {
                    try
                    {

                        this.PlaceServiceEvent(new PlaceServiceEventArgs
                        {
                            result = new Place(result.Content),
                            t = typeof(Place),
                            status = result.ResultStatus,
                            currentCommand = PlaceServiceCommand.GetPlace,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex) // ошибка распарсивания
                    {
                        this.PlaceServiceEvent(new PlaceServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = PlaceServiceCommand.GetPlace,
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
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = PlaceServiceCommand.GetPlace,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = PlaceServiceCommand.GetPlace,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = PlaceServiceCommand.GetPlace,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }
        /// <summary>
        /// Получить место под идентификатору
        /// </summary>
        /// <param name="id"></param>
        public void GetPlace(int id)
        {
            this.Сontext.CurrentPart = Part.places;
            this.Сontext.SendAsyncRequest("/"+ id.ToString() +".xml", AcceptVerbs.GET);
        }

        private void DeletePlace_Response(Result result)
        {
            if (PlaceServiceEvent != null) // если привязан обработчик
            {
                if (result.ResultStatus == Status.OK) // если всё хорошо и пришол контент
                {
                    try
                    {

                        this.PlaceServiceEvent(new PlaceServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = result.ResultStatus,
                            currentCommand = PlaceServiceCommand.DeletePlace,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex) // ошибка распарсивания
                    {
                        this.PlaceServiceEvent(new PlaceServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = PlaceServiceCommand.DeletePlace,
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
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = PlaceServiceCommand.DeletePlace,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = PlaceServiceCommand.DeletePlace,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = PlaceServiceCommand.DeletePlace,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        /// <summary>
        /// Удалить место по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор места в базе данных</param>
        public void DeletePlacePlace(int id)
        {
            this.Сontext.CurrentPart = Part.places;
            this.Сontext.SendAsyncRequest("/" + id.ToString(), AcceptVerbs.DELETE);
        }

        private void EditPlace_Response(Result result)
        {
            if (PlaceServiceEvent != null) // если привязан обработчик
            {
                if (result.ResultStatus == Status.OK) // если всё хорошо и пришол контент
                {
                    try
                    {

                        this.PlaceServiceEvent(new PlaceServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = result.ResultStatus,
                            currentCommand = PlaceServiceCommand.EditPlace,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex) // ошибка распарсивания
                    {
                        this.PlaceServiceEvent(new PlaceServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = PlaceServiceCommand.EditPlace,
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
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = PlaceServiceCommand.EditPlace,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = PlaceServiceCommand.EditPlace,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.PlaceServiceEvent(new PlaceServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = PlaceServiceCommand.EditPlace,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        /// <summary>
        /// Редактировать место
        /// </summary>
        /// <param name="place">Редактирование места</param>
        public void EditPlace(Place place)
        {
            this.Сontext.CurrentPart = Part.places;
            this.Сontext.SendAsyncRequest("/" + place.Id.ToString(), AcceptVerbs.PUT);
        }

        /// <summary>
        /// Редактировать место
        /// </summary>
        /// <param name="PlaceId">Идентификатор места которое нужно редактировать</param>
        /// <param name="PhotoId">Новый идентификатор фото</param>
        /// <param name="GeoDataId">Новый идентификтаор геодаты</param>
        public void EditPlace(int PlaceId, int PhotoId, int GeoDataId)
        {
            this.Сontext.CurrentPart = Part.places;
            this.Сontext.Add("place[photo_id]", PhotoId.ToString());
            this.Сontext.Add("place[geo_data_id]", GeoDataId.ToString());
            this.Сontext.SendAsyncRequest("/" + PlaceId.ToString(), AcceptVerbs.PUT);
        }

        /// <summary>
        /// Редактировать место
        /// </summary>
        /// <param name="PlaceId">Идентификатор места которое нужно редактировать</param>
        /// <param name="PhotoId">Новый идентификатор фото</param>
        /// <param name="GeoDataId">Новый идентификтаор геодаты</param>
        /// <param name="Title">Новый заголовок</param>
        public void EditPlace(int PlaceId, int PhotoId, int GeoDataId, string Title)
        {
            this.Сontext.CurrentPart = Part.places;
            this.Сontext.Add("place[photo_id]", PhotoId.ToString());
            this.Сontext.Add("place[geo_data_id]", GeoDataId.ToString());
            this.Сontext.Add("place[title]", Title);
            this.Сontext.SendAsyncRequest("/" + PlaceId.ToString(), AcceptVerbs.PUT);
        }
        /// <summary>
        /// Редактировать место
        /// </summary>
        /// <param name="PlaceId">Идентификатор места которое нужно редактировать</param>
        /// <param name="PhotoId">Новый идентификатор фото</param>
        /// <param name="GeoDataId">Новый идентификтаор геодаты</param>
        /// <param name="Title">Новый заголовок</param>
        /// <param name="Description">НОвое описание</param>
        public void EditPlace(int PlaceId, int PhotoId, int GeoDataId, string Title, string Description)
        {
            this.Сontext.CurrentPart = Part.places;
            this.Сontext.Add("place[photo_id]", PhotoId.ToString());
            this.Сontext.Add("place[geo_data_id]", GeoDataId.ToString());
            this.Сontext.Add("place[title]", Title);
            this.Сontext.Add("place[description]", Description);
            this.Сontext.SendAsyncRequest("/" + PlaceId.ToString(), AcceptVerbs.PUT);
        }
        /// <summary>
        /// Редактировать место
        /// </summary>
        /// <param name="PlaceId">Идентификатор места которое нужно редактировать</param>
        /// <param name="PhotoId">Новый идентификатор фото</param>
        /// <param name="GeoDataId">Новый идентификтаор геодаты</param>
        /// <param name="Title">Новый заголовок</param>
        /// <param name="Description">НОвое описание</param>
        /// <param name="Address">Новый адрес</param>
        public void EditPlace(int PlaceId, int PhotoId, int GeoDataId, string Title, string Description, string Address)
        {
            this.Сontext.CurrentPart = Part.places;
            this.Сontext.Add("place[photo_id]", PhotoId.ToString());
            this.Сontext.Add("place[geo_data_id]", GeoDataId.ToString());
            this.Сontext.Add("place[title]", Title);
            this.Сontext.Add("place[description]", Description);
            this.Сontext.Add("place[address]", Address);
            this.Сontext.SendAsyncRequest("/" + PlaceId.ToString(), AcceptVerbs.PUT);
        }
        /// <summary>
        ///  Редактировать место
        /// </summary>
        /// <param name="PlaceId">Идентификатор места которое нужно редактировать</param>
        /// <param name="Title">Новый заголовок</param>
        /// <param name="Description">НОвое описание</param>
        public void EditPlace(int PlaceId, string Title, string Description)
        {
            this.Сontext.CurrentPart = Part.places;
            this.Сontext.Add("place[title]", Title);
            this.Сontext.Add("place[description]", Description);
            this.Сontext.SendAsyncRequest("/" + PlaceId.ToString(), AcceptVerbs.PUT);
        }
        /// <summary>
        /// Редактировать место
        /// </summary>
        /// <param name="PlaceId">Идентификатор места которое нужно редактировать</param>
        /// <param name="PhotoId">Новый идентификатор фото</param>
        public void EditPlacePhotoIdOnly(int PlaceId, int PhotoId)
        {
            this.Сontext.CurrentPart = Part.places;
            this.Сontext.Add("place[photo_id]", PhotoId.ToString());
            this.Сontext.SendAsyncRequest("/" + PlaceId.ToString(), AcceptVerbs.PUT);
        }
        /// <summary>
        /// Редактировать место
        /// </summary>
        /// <param name="PlaceId">Идентификатор места которое нужно редактировать</param>
        /// <param name="GeoDataId">Новый идентификтаор геодаты</param>
        public void EditPlaceGeoDataIdOnly(int PlaceId, int GeoDataId)
        {
            this.Сontext.CurrentPart = Part.places;
            this.Сontext.Add("place[geo_data_id]", GeoDataId.ToString());
            this.Сontext.SendAsyncRequest("/" + PlaceId.ToString(), AcceptVerbs.PUT);
        }
        /// <summary>
        /// Редактировать место
        /// </summary>
        /// <param name="PlaceId">Идентификатор места которое нужно редактировать</param>
        /// <param name="Title">Новый заголовок</param>
        public void EditPlaceTitleOnly(int PlaceId, string Title)
        {
            this.Сontext.CurrentPart = Part.places;
            this.Сontext.Add("place[title]", Title);
            this.Сontext.SendAsyncRequest("/" + PlaceId.ToString(), AcceptVerbs.PUT);
        }
        /// <summary>
        /// Редактировать место
        /// </summary>
        /// <param name="PlaceId">Идентификатор места которое нужно редактировать</param>
        /// <param name="Description">Новое описание</param>
        public void EditPlaceDescriptionOnly(int PlaceId, string Description)
        {
            this.Сontext.CurrentPart = Part.places;
            this.Сontext.Add("place[description]", Description);
            this.Сontext.SendAsyncRequest("/" + PlaceId.ToString(), AcceptVerbs.PUT);
        }
        /// <summary>
        /// Редактировать место
        /// </summary>
        /// <param name="PlaceId">Идентификатор места которое нужно редактировать</param>
        /// <param name="Address">Новый адрес</param>
        public void EditPlaceAddressOnly(int PlaceId, string Address)
        {
            this.Сontext.CurrentPart = Part.places;
            this.Сontext.Add("place[address]", Address);
            this.Сontext.SendAsyncRequest("/" + PlaceId.ToString(), AcceptVerbs.PUT);
        }

    }
}
