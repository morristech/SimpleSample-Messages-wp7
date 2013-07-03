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
using QuickBloxSDK_Silverlight.users;
using QuickBloxSDK_Silverlight.Core;
using QuickBloxSDK_Silverlight.Geo;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using QuickBloxSDK_Silverlight.owners;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using QuickBloxSDK_Silverlight.Places;
using QuickBloxSDK_Silverlight.Content;
using QuickBloxSDK_Silverlight.PushNotification;

/// <summary>
/// Библиотека классов для взаимодействия с сервисами QuickBlox
/// 
/// </summary>
namespace QuickBloxSDK_Silverlight
{
    /// <summary>
    ///Главный класс фрейморка.
    ///Этот класс является реализацией шаблона "Фасад".
    ///Данный класс предаставляет позможность работать со всеми сервисами QuickBLox через него.
    ///Здесь происходит настройка соеденения с сервером и выбор его режима работы.
    /// </summary>
    public class QuickBlox: IQuickBlox
    {
       
        private ConnectionContext Сontext;

        /// <summary>
        /// Подсервис QuickBlox.
        /// Через него осуществляется работа с геоданными и текущим местоположением,
        /// а так же местоположением других клиентов сервиса.
        /// </summary>
        public GeoService geoService
        {
            get;
            set;
        }


        public ContentService contentService
        {
            get;
            set;
        }

        /// <summary>
        /// Подсервис QuickBlox.
        /// Отвечает за работу с объектами типа "owner".
        /// </summary>
        public OwnersService ownerService
        {
            get;
            set;
        }

        /// <summary>
        ///Подсервис QuickBlox.
        ///Отвечает за работу с пользователями сервиса QuickBlox.
        ///Создание, редактирование, удаление и выборки.
        /// </summary>
        public UserService userService
        { get; set; }

        /// <summary>
        ///Подсервис QuickBlox.
        ///Отвечает за работу с местами отмеченными на карте.
        ///Используется соместно с геосервисом.
        /// </summary>
        public PlaceService placeService
        { get; set; }


        public PushNotificationService pushNotificationService
        { get; set; }

        

        /// <summary>
        /// Создает и настраивает подключение к серверу QuickBlox.
        /// </summary>
        /// <param name="AppId">Идентификатор приложениея (Берется в панели администрирования сервиса)</param>
        /// <param name="OwnerId">Идентификатор владельца (Берется в панели администрирования сервиса)</param>
        /// <param name="AuthenticationKey">Открытый ключ (Берется в панели администрирования сервиса)</param>
        /// <param name="AuthenticationSecret">Закрытый ключ (Берется в панели администрирования сервиса)</param>
        /// <param name="CustomServerAddr">
        /// Адрес сервера QuickBlox.
        /// Если это полу пустое, то сервси будет использовать адресс сервера по умолчанию. 
        /// Рекоминдуется использовать именно сервер по умолчанию.
        /// Если используется серсив не по умолчанию, то существует два способа задать необходимый сервер:
        /// 1. Задать маской в стиле test.*.test.com
        /// 1. Задать только адрес домена test.com
        /// </param>
        /// <param name="IsUseSSL">Использовать соединение по защищенному каналу</param>
        public QuickBlox(int AppId, int OwnerId , string AuthenticationKey, string AuthenticationSecret, string CustomServerAddr, bool IsUseSSL, string DeviceId)
        {
            this._DeviceId = DeviceId;
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            this._ApplicationId = AppId;
            this.IsOnline = false;
            this._AuthenticationKey = string.IsNullOrEmpty(AuthenticationKey) ? string.Empty : AuthenticationKey;
            this._AuthenticationSecret = string.IsNullOrEmpty(AuthenticationSecret) ? string.Empty : AuthenticationSecret;
            this.Сontext = new ConnectionContext(AppId, CustomServerAddr, IsUseSSL);
            this.BackgroundConnection = new ConnectionContext(AppId, CustomServerAddr, IsUseSSL);
            this.BackgroundConnection.RequestResult += new ConnectionContext.Main(BackgroundConnection_RequestResult);
            this.geoService = new GeoService(this.Сontext);
            this.userService = new UserService(this.Сontext);
            this.ownerService = new OwnersService(this.Сontext);
            this.placeService = new PlaceService(this.Сontext);
            this.contentService = new ContentService(this.Сontext);
            this.pushNotificationService = new PushNotificationService(this.Сontext);
            this.pushNotificationService.DeviceId = DeviceId;
            this.placeService.CustomServerAddr = CustomServerAddr;
            this.pushNotificationService.CustomServerAddr = CustomServerAddr;
            this.geoService.CustomServerAddr = CustomServerAddr;
            this.userService.CustomServerAddr = CustomServerAddr;
            this.ownerService.CustomServerAddr = CustomServerAddr;
            this.contentService.CustomServerAddr = CustomServerAddr;
            this.userService.user = this.QBUser;
            this.userService.IsOnline = this.IsOnline;
            this.contentService.IsOnline = this.IsOnline;
            this.pushNotificationService.IsOnline = this.IsOnline;
            this.contentService.user = this.QBUser;
            this.OwnerId = OwnerId;
            this.userService.OwnerId = this.OwnerId;
            this.IsQBUserLoaded = false;
            this.IsQBUsersLoaded = false;
            this.ApplicationLogOn(0);
            this.GetBgRequest();
            this.contentService.session = this.session;
            
        }

        private string _Username, _Password, _AuthenticationSecret, _AuthenticationKey, _CustomServerAddr, _DeviceId;

        private int _UserId, _GeoUserId, _ApplicationId;

        /// <summary>
        ///Идентификатор устройства
        /// </summary>
        public string DeviceId
        {
            get
            {
                return this._DeviceId;
            }
            set
            {
                this._DeviceId = value;
            }
        }

        /// <summary>
        /// Текущая географическая широта.
        /// Это поле необходимо использовать когда фреймфорк работает вместе с GPS навигатором.
        /// Сюда следует заносить текущее значение.
        /// Именно это поле используют подсервисы для получения координаты в реальном времени.
        /// </summary>
        public decimal Latitude
        {get; set;}
        /// <summary>
        /// Текущая географическая долгота.
        /// Это поле необходимо использовать когда фреймфорк работает вместе с GPS навигатором.
        /// Сюда следует заносить текущее значение.
        /// Именно это поле используют подсервисы для получения координаты в реальном времени.
        /// </summary>
        public decimal Longitude
        { get; set; }

        /// <summary>
        /// Идентифкатор владельца приложения.
        /// Ипользуется для подключения к серверу.
        /// Так же под нему выбиравется текущий объект "owner" приложения.
        /// </summary>
        public int OwnerId
        { get; private set; }

        /// <summary>
        /// Загружен ли пользователь прилоежния.
        /// Бывает прилоежние олицетварено с конкретным пользователем из QuickBlox User Service и выступает от лица этого польователя.
        /// Это происходит когда пользователь залогинется,
        /// при этом система загружает этого пользователя.
        /// 
        /// </summary>
        public bool IsQBUserLoaded
        { get; set; }

        /// <summary>
        /// Пользователь под которым выполнен вход.
        /// Если прилоежение не олицетварено с конкретным пользователем то это поле NULL.
        /// Если пользователь выполнил вход в систему то приложение работает от лица этого самого пользователя.
        /// При этом происходит загрузка пользователя.
        /// </summary>
        public User QBUser
        { get; set; }

        
        /// <summary>
        ///Загружен ли спосок пользователей.
        ///Приложение может работать в фоновом режиме.
        ///В таком режиме работы происходит автоматическая синхронизация пользователей.
        ///Другими словами система сама выбирает пользователей и отслеживает у них изменения.
        /// </summary>
        public bool IsQBUsersLoaded
        { get; set; }


        /// <summary>
        /// Список пользователей QuickBLox доступных для текущего приложения.
        /// Используется если приложение работает в фоновом режиме.
        /// Система может работать в фономом режиме. При котором самостоятельно выбиаются 
        /// пользователи системы доступные для этого прилоежения. Так же происходит потсоянная синхронизация
        /// для того что бы выявить изменения которые произошли с пользователями.
        /// </summary>
        public User[] QBUsers
        { get; set; }

        /// <summary>
        ///Происходит ли загрузка геодат.
        ///
        /// </summary>
        public bool IsGeoDataLoad
        {
            get;
            set;
        }

        /// <summary>
        /// Список геодат доступных для конкретного приложения.
        /// Используется исли приложение работает в фоновом режиме.
        /// </summary>
        public GeoData[] GeoData
        {
            get;
            set;
        }
        
        /// <summary>
        /// Имя пользователя.
        /// Используется исли приложение работает от имени пользователя.
        /// </summary>
        public string Username
        {
            get
            {
                return this._Username;
            }
            set
            {
               this._Username = value;
            }
        }

        /// <summary>
        /// Идентифкатор пользователя.
        /// Используется если приложение работает от имени пользователя.
        /// </summary>
        public int UserId
        {
            get
            {
                return this._UserId;
            }
            set
            {
                this._UserId = value;
            }
        }
        /// <summary>
        /// Пароль пользователя.
        /// Используется если необходимо выполнить вход от имени пользователя.
        /// </summary>
        public string Password
        {
            get
            {
                return this._Password;
            }
            set
            {
                this._Password = value;
            }
        }

        /// <summary>
        /// Идентифкатор прилоежния.
        /// Используется для подключения к серверу.
        /// </summary>
        public int ApplicationId
        {
            get
            {
                return this._ApplicationId;
            }
            set
            {
                this._ApplicationId = value;
            }
        }

        /// <summary>
        /// Секретный ключ.
        /// Используется для подключения к серверу.
        /// </summary>
        public string AuthenticationSecret
        {
            get
            {
                return this._AuthenticationSecret;
            }
            set
            {
                this._AuthenticationSecret = value;
            }
        }
        /// <summary>
        /// Открытый ключ.
        /// Используется для подключения к серверу.
        /// </summary>
        public string AuthenticationKey
        {
            get
            {
                return this._AuthenticationKey;
            }
            set
            {
                this._AuthenticationKey = value;
            }
        }

        /// <summary>
        /// Вход от лица конкретного пользователя.
        /// Приложение может работать от лица конкретного пользователя.
        /// В таком режиме открываются дополнительные возможности по работе с сервисами.
        /// Многие оперции могут быть выполнены только тогда когда приложение работает в режиме
        /// олицетворения.
        /// Для этого метода должны быть не пустыми:
        /// Username
        /// OwnerId
        /// Password
        /// </summary>
        public void LogOn()
        {
            this.userService.Authenticate(this.Username, this.OwnerId, this.Password);
        }

        /// <summary>
        ///Приложение перестает работать в режиме олицетворения.
        /// </summary>
        public void LogOff()
        {
            this.userService.Logout();
        }

        /// <summary>
        /// При длительном бездейсвии сервер определяет текущее подключение как неактивное и разрывает соединение.
        /// Для этого раз в 10 минут необходимо выполнить хотябы одну операцию.
        /// Если действия никакие не требуются но подключение  с сервером должна быть постоянной то
        /// необходимо использовать этот метод раз в десять минут.
        /// </summary>
        public void Ping()
        {
            this.userService.Identify();
        }


       /// <summary>
       /// Поключено ли приложение к серверу или нет.
       /// </summary>
        public bool IsOnline
        {
            get;
            private set;
        }


        #region DataUpdate

       /// <summary>
       /// Интервал пинга.
       /// Свойство используется если прилоежние работает в фоновом режиме.
       /// </summary>
       public int PingInterval
        {
            get
            {
                if (timer != null)
                    return  (int)timer.Interval.TotalSeconds;
                else
                    return -1;
            }
            set
            {
                if (timer != null)
                {
                    if (value < 1)
                    {
                        timer.Interval = new TimeSpan(0, 0, 1);
                        this.interval = 1;
                    }
                    else
                    {
                        timer.Interval = new TimeSpan(0, 0, value);
                        this.interval = value;
                    }
                }
            }

        }


        /// <summary>
        /// Список ошибок при работе прилоежния в фоновом режиме.
       /// Свойство используется если прилоежние работает в фоновом режиме.
        /// </summary>
       public List<string> UpdateErrorList;



       private System.Windows.Threading.DispatcherTimer timer;
       private int interval = 2;
       private ConnectionContext BackgroundConnection;
       int pageCount = 1;
       private bool LoadPages = false;
       private List<int> LoadedPages = new List<int>();
       private void BackgroundConnection_RequestResult(Result result)
       {
           
                       if (result.ControllerName.Contains("/geodata/find") && result.URI.Contains("app.id") && !result.URI.Contains("user.id"))
                       {
                           if (result.ResultStatus == Status.OK)
                           {
                               try
                               {
                                   List<GeoData> users = new List<GeoData>();
                                   XElement xml = XElement.Parse((string)result.Content);
                                   foreach (var t in xml.Descendants("geo-datum"))
                                   {
                                       try
                                       {
                                           users.Add(new GeoData(t.ToString()));
                                       }
                                       catch { }                                   
                                   }

                                   int cp = int.Parse(result.URI.Split('?')[1].Split('&')[0].Split('=')[1]);

                                   if (!this.LoadedPages.Contains(cp) || cp == pageCount)
                                   {
                                       if (this.GeoData != null)
                                       {
                                           int[] tempGeoDateIdArr = this.GeoData.Select(t => t.Id).ToArray();
                                           this.GeoData = this.GeoData.Union(users.Where(t => t != null).Where(t => !tempGeoDateIdArr.Contains(t.Id)).OrderBy(t => t.CreatedDate)).ToArray();
                                           this.IsGeoDataLoad = true;
                                           this.LoadedPages.Add(cp);
                                       }
                                       else
                                       {
                                           this.GeoData = users.Where(t => t != null).OrderBy(t => t.CreatedDate).ToArray();
                                           this.LoadedPages.Add(cp);
                                       }
                                   }


                                   try
                                   {
                                       try
                                       {
                                           int total_entries = int.Parse(xml.Attribute("total_entries").Value);
                                           int per_page = int.Parse(xml.Attribute("per_page").Value);
                                           pageCount = total_entries / per_page;
                                           if ((total_entries % per_page) > 0)
                                               pageCount++;


                                       }
                                       catch
                                       {
                                           pageCount = 1;
 
                                       }

                                       for (int i = pageCount; i > 0; i--)
                                       {
                                           if (!LoadedPages.Contains(i))
                                           {
                                               this.BackgroundConnection.CurrentPart = Part.geopos;
                                               this.BackgroundConnection.Add("page", i.ToString());
                                               this.BackgroundConnection.Add("per_page", "100");
                                               this.BackgroundConnection.Add("app.id", this.Сontext.ApplicationId.ToString());
                                               this.BackgroundConnection.SendAsyncRequest("/find.xml", AcceptVerbs.GET);
                                           }
                                       }
                                   }
                                   catch
                                   {
                                       pageCount = 1;
                                   }
                                   if (this.BackgroundEvent != null)
                                       this.BackgroundEvent("geodata", GeoData);

                                 
                               }
                               catch (Exception ex)
                               {
                                   this.IsGeoDataLoad = false;
                               }
                           }
                       }
                       
                   
               
                      

                       if (result.ControllerName.Contains("users.xml"))
                       {
                           if (result.ResultStatus == Status.OK)
                           {
                               try
                               {
                                   List<User> users = new List<User>();
                                   XElement xml = XElement.Parse((string)result.Content);
                                   foreach (var t in xml.Descendants("user"))
                                       users.Add(new User(t.ToString()));

                                   this.QBUsers = users.ToArray();
                                  
                                   

                                   this.IsQBUsersLoaded = true;
                                   if(this.BackgroundEvent !=null)
                                       this.BackgroundEvent("users", QBUsers);
                               }
                               catch (Exception ex)
                               {
                                   this.IsQBUsersLoaded = false;
                               }
                           }
                           return;
                       }
                       
                   
             
                       if (result.ControllerName.Contains("auth"))
                       {
                           this.ApplicationLogOn_Response(result);
                       }
                      
                   
           
       }

        /// <summary>
        /// Данный метод переводит работу прилоежения в режим фонового обновления.
        /// </summary>
       public void BackgroundUpdateStart()
        {
            try
            {
                this.UpdateErrorList = new List<string>();
                timer.Interval = new TimeSpan(0, 0, this.interval);
                this.timer.Start();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Прекращения работы приложения в фоновом режиме.
        /// </summary>
       public void BackgroundUpdateStop()
        {
            try
            {
                this.timer.Stop();
            }
            catch
            {

            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.GetBgRequest();
        }

        private void GetBgRequest()
        {
            if (this.AppStatus == AppConnectionStatus.Offline || this.AppStatus == AppConnectionStatus.Conection)
                return;

            this.BackgroundConnection.CurrentPart = Part.users;
            this.BackgroundConnection.Add("current_page", "1");
            this.BackgroundConnection.Add("per_page", "100");
            this.BackgroundConnection.SendAsyncRequest(".xml", AcceptVerbs.GET);


            this.BackgroundConnection.CurrentPart = Part.geopos;
            this.BackgroundConnection.Add("page", this.pageCount.ToString());
            this.BackgroundConnection.Add("per_page", "100");
            this.BackgroundConnection.Add("app.id", this.Сontext.ApplicationId.ToString());
            this.BackgroundConnection.SendAsyncRequest("/find.xml", AcceptVerbs.GET);
          
        }

        /// <summary>
        /// Делегат фонового события
        /// </summary>
        /// <param name="Command">Тип команды</param>
        /// <param name="Result">Объект который посылается событием</param>
        public delegate void BGR(string Command, object Result);

        /// <summary>
        ///Фоновое событие
        /// </summary>
        public event BGR BackgroundEvent;

        #endregion


        #region Application login

        
        /// <summary>
        /// Санс подклюения к серверу
        /// </summary>
        public Session session
        { get; private set; }


        private void ApplicationLogOn_Response(Result result)
        {
            
                if (result.ResultStatus == Status.OK) // если всё хорошо и пришол контент
                {
                    try
                    {

                        Session session = new Session((string)result.Content);
                        this.AppStatus = AppConnectionStatus.Online;
                        this.Ticket = session;
                        this.BackgroundConnection.Ticket = this.Ticket.Token;
                        this.Сontext.Ticket = this.Ticket.Token;

                        this.IsOnline = true;

                        if (this.BackgroundEvent != null)
                            this.BackgroundEvent("Online", session); 

                    }
                    catch (Exception ex) // ошибка распарсивания
                    {
                        this.AppStatus = AppConnectionStatus.Offline;
                        this.AppConnectionErrorList = new string[] { ex.Message };
                        if (this.BackgroundEvent != null)
                            this.BackgroundEvent("Offline", this.AppConnectionErrorList); 
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
                {
                    this.AppStatus = AppConnectionStatus.Offline;
                    this.AppConnectionErrorList = new string[] { result.ErrorMessage };
                    if (this.BackgroundEvent != null)
                        this.BackgroundEvent("Offline", this.AppConnectionErrorList);


                }
                else if (result.ResultStatus == Status.ValidationError)
                {
                    ValidateErrorElement[] errels = ValidateErrorElement.LoadErrorList(result.Content);
  
                    foreach (var t in errels)
                        if (t.ErrorMessage.Contains("Bad timestamp"))
                        {
                            if (this.TimeZones.Length > CurrentTimeZone)
                            {
                                this.ApplicationLogOn(TimeZones[CurrentTimeZone]);
                                this.CurrentTimeZone++;
                                return;
                            }

                            break;
                        }

                    if(errels != null)
                        if(errels.Length > 0)
                            this.AppConnectionErrorList = errels.Select(t => t.ErrorMessage).ToArray();

                    this.AppStatus = AppConnectionStatus.Offline;
                    this.IsOnline = false;
                    if (this.BackgroundEvent != null)
                        this.BackgroundEvent("Offline", this.AppConnectionErrorList);
                    
                }
                else
                {
                    this.AppStatus = AppConnectionStatus.Offline;
                    this.IsOnline = false;
                    if (this.BackgroundEvent != null)
                        this.BackgroundEvent("Offline", this.AppConnectionErrorList);
                }
            
            
        }

        /// <summary>
        /// Текущий идентифкатор сеанса подключнеия
        /// </summary>
        public Session Ticket
        {
            get;
            private set;
        }

        private int CurrentTimeZone = 0;

        private int[] TimeZones = { 0, +1, +2, +3, +4, +5, +6, +7, +8, +9, +10, +11, +12, -1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11, -12 };

        /// <summary>
        /// Статус подключение прилоежения к серверу
        /// </summary>
        public AppConnectionStatus AppStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// Если приложение было в процессе подключения к серверу ну тобиш был статус "конетион"
        /// И потом оно стало офлайн - это может означать что прилоежние не смогло подключится
        /// И данное поле содержит список ошибок подключений
        /// </summary>
        public string[] AppConnectionErrorList
        {
            get;
            private set;
        }



        /// <summary>
        /// Подклчение к серверу
        /// </summary>
        /// <param name="id"></param>
        public void ApplicationLogOn(int TimeZoneOffset)
        {
            this.AppStatus = AppConnectionStatus.Conection;

            if (this.BackgroundEvent != null)
                this.BackgroundEvent("Connection", null);

            Random rdn = new Random();
            int randomResult = rdn.Next(1,1000);
            this.BackgroundConnection.CurrentPart = Part.Admin;
            this.BackgroundConnection.Add("application_id", this.ApplicationId.ToString());
            this.BackgroundConnection.Add("auth_key", this.AuthenticationKey);
            this.BackgroundConnection.Add("nonce", randomResult.ToString());
            
            int ts = (int)((DateTime.Now.AddHours(TimeZoneOffset) - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds); 
            this.BackgroundConnection.Add("timestamp", ts.ToString());
            byte[] key = Encoding.UTF8.GetBytes(this.AuthenticationSecret);

                StringBuilder signature = new StringBuilder();
                signature.Append("application_id");
                signature.Append("=");
                signature.Append(this.ApplicationId.ToString());
                signature.Append("&");
                signature.Append("auth_key");
                signature.Append("=");
                signature.Append(this.AuthenticationKey);
                signature.Append("&");

                
                signature.Append("device[platform]");
                signature.Append("=");
                signature.Append("windows_phone");
                signature.Append("&");

                this.DeviceId = this.DeviceId.Replace('=', '_');
                this.DeviceId = this.DeviceId.Replace('+', '-');
                this.DeviceId = this.DeviceId.Replace('/', '*');

                signature.Append("device[udid]");
                signature.Append("=");
                signature.Append(this.DeviceId);
                signature.Append("&");


                signature.Append("nonce");
                signature.Append("=");
                signature.Append(randomResult);
                signature.Append("&");
                signature.Append("timestamp");
                signature.Append("=");
                signature.Append(ts.ToString());
                
                
                this.BackgroundConnection.Add("signature", this.Encode(signature.ToString(),key));


                this.BackgroundConnection.Add("device[platform]", "windows_phone");
                this.BackgroundConnection.Add("device[udid]", this.DeviceId);


            this.BackgroundConnection.SendAsyncRequest("/auth.xml", AcceptVerbs.POST);
        }

       
        private  string Encode(string input, byte[] key)
        {
           HMACSHA1 myhmacsha1 = new HMACSHA1(key);
           byte[] byteArray = Encoding.UTF8.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            return myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
       }


        #endregion

    }

   

}
