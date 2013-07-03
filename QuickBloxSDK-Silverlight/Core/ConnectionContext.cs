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
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

namespace QuickBloxSDK_Silverlight.Core
{
    /// <summary>
    ///  Контекст подключения.
    ///  Через этот класс осуществляется обмен информацией с сервером.
    ///  
    /// </summary>
    public class ConnectionContext
    {

        #region Конструкторы

        /// <summary>
        /// Создание подключения
        /// </summary>
        /// <param name="ApplicationId">Идентификатор приложения</param>
        /// <param name="CustomServerAddr">Адрес сервера</param>
        /// <param name="IsUseSSL">Использовать или нет защищенное соединение</param>
        public ConnectionContext(int ApplicationId, string CustomServerAddr, bool IsUseSSL)
        {
            this.ApplicationId = ApplicationId;
            this.Form = new List<FormElement>();
            this.Cookie = new CookieContainer();
            this.Files = new List<File>();
            this.CustomServerAddr = CustomServerAddr;
            this.IsUseSSL = IsUseSSL;
        }
        #endregion


        #region Поля

        private string CustomServerAddr;
        public string PrioritetServerName;
        private bool IsUseSSL;

        /// <summary>
        /// Адрес сервера к которому производится подключение
        /// </summary>
        public string CurrentServerAdr
        {  get; private set; }

        /// <summary>
        ///Идентифкатор приложения
        /// </summary>
        public int ApplicationId
        { get; set; }

        /// <summary>
        /// Имя пользовтеля под которым производится подключение
        /// </summary>
        public User Username
        {
            get;
            set;
        }

        
        private Part currentPart;

        /// <summary>
        /// Текущий раздел сервиса к которому выполняются запросы
        /// </summary>
        public Part CurrentPart
        {
            get
            {
                return currentPart;
            }
            set
            {
                this.currentPart = value;
                this.CurrentServerAdr = Helper.PartToServerName(value, this.CustomServerAddr);
            }
        }

        /// <summary>
        /// Identification client line
        /// Example: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)
        /// </summary>
        public string AgentName
        { get; set; }

        /// <summary>
        /// Content which client can accept
        /// Example: Accept:text/html, application/xhtml+xml, */*
        /// </summary>
        public string Accept
        { get; set; }

        
        /// <summary>
        /// Заголовки запроса
        /// </summary>
        public Header[] Headers
        {
            get;
            private set;
        }

        private CookieContainer Cookie; 

        /// <summary>
        /// Главный делегат запроса
        /// </summary>
        /// <param name="result">Ответ от сервера</param>
        public delegate void Main(Result result);

        private Main _RequestResult;

        /// <summary>
        /// Событие возникающие в результате ответа от сервера
        /// </summary>
        public event Main RequestResult
        {
            add
            {
                this._RequestResult += value;
            }
            remove
            {
                this._RequestResult -= value;
            }
        }

        /// <summary>
        /// Аутентификационный билет 
        /// </summary>
        public string Ticket
        {
            get;
            set;
        }

        #endregion

        #region Form
       
        private List<FormElement> Form;

       /// <summary>
       /// Добавить новое поле формы в запрос к серверу
       /// </summary>
       /// <param name="key">Нзавание параметра формы</param>
       /// <param name="value">Значение</param>
        public void Add(string key, string value)
        {
            if (this.Form == null)
                this.Form = new List<FormElement>();

            if (string.IsNullOrEmpty(key))
                return;

            this.Form.Add(new FormElement { key =key, value = value });
        }

        /// <summary>
        /// Удалить все поля формы
        /// </summary>
        public void Clear()
        {
            if (this.Form == null)
                this.Form = new List<FormElement>();

            this.Form.Clear();
        }

        /// <summary>
        /// Удалить конкретное поле формы
        /// </summary>
        /// <param name="key">Form field name</param>
        public void Delete(string key)
        {
            if (this.Form == null)
            {
                this.Form = new List<FormElement>();
                return;
            }

            if (string.IsNullOrEmpty(key))
                return;

            try
            {
                FormElement element = null;
                foreach (var t in this.Form)
                    if (t.key == key)
                        element = t;

                if (element != null)
                    this.Form.Remove(element);
            }
            catch
            { }
        }
        /// <summary>
        /// Сгенерировать форму
        /// </summary>
        /// <param name="IsPost">Query type</param>
        /// <returns></returns>
        private string RenderForm(bool IsPost)
        {
            if (this.Form == null)
                return string.Empty;

            /*if (this.Form.Count < 1)
                return string.Empty;*/

            StringBuilder result = new StringBuilder();
            result.Append(IsPost?string.Empty:"?");
            foreach (var t in this.Form)
            {
                result.Append(t.ToString());
                result.Append("&");
            }

            if (!string.IsNullOrEmpty(this.Ticket))
            {
                result.Append("token");
                result.Append("=");
                result.Append(this.Ticket);
            }

            string formresult = result.ToString();
            if (formresult[result.ToString().Length - 1] == '&')
                formresult = formresult.Remove(formresult.Length - 1);



            return formresult;
        }
        #endregion
      
        #region

       /// <summary>
       /// Выполнить асинхронный запрос к серверу
       /// </summary>
       /// <param name="ControllerName">Controller name</param>
       /// <param name="method">Query type</param>
       public void SendAsyncRequest(string ControllerName, AcceptVerbs method)
        {

            StringBuilder urlMaker = new StringBuilder();

            if (string.IsNullOrEmpty(this.PrioritetServerName))
            {
                if (string.IsNullOrEmpty(this.Ticket))
                    if (this.currentPart != Part.Admin)
                        return;

                urlMaker.Append(this.IsUseSSL ? "https://" : "http://");
                urlMaker.Append(this.CurrentServerAdr);
              //  urlMaker.Append("/");
                urlMaker.Append(ControllerName);
            }
            else
            {
                urlMaker.Append(this.PrioritetServerName);
            }

            HttpWebRequest request;

            if (method == AcceptVerbs.GET || method == AcceptVerbs.DELETE)
                request = WebRequest.CreateHttp(new Uri(urlMaker.ToString() + this.RenderForm(false)));
            else 
                request = WebRequest.CreateHttp(new Uri(urlMaker.ToString()));

           
            request.Method = Helper.AcceptVerbsToString(method);
            request.AllowAutoRedirect = false;

            if (request == null)
                return;

            try
            {
                request.Headers["Referer"] = "-";
                request.Headers["Accept-Language"] = "en;q=0.8";
                request.Headers["Cache-Control"] = "no-cache";
                request.Headers["Accept-Encoding"] = "windows-1251,utf-8;q=0.7,*;q=0.7";
                request.Accept = string.IsNullOrEmpty(this.Accept) ? "*/*;q=0.1" : this.Accept;
                request.UserAgent = string.IsNullOrEmpty(this.AgentName) ? "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)" : string.Empty;
            }
            catch
            { }
                request.CookieContainer = this.Cookie;
            switch (method)
            {
                case AcceptVerbs.GET:
                    {
                        IAsyncResult asyncResult = (IAsyncResult)request.BeginGetResponse(new AsyncCallback(ResponseCallback), request);
                        this.PrioritetServerName = string.Empty;
                        this.Clear();
                        this.ClearFiles();
                        break;
                    }
                case AcceptVerbs.POST:
                    {
                        request.ContentType = "application/x-www-form-urlencoded";

                        if (this.Files != null)
                            if (this.Files.Count > 0)
                                request.ContentType = String.Concat("multipart/form-data; boundary=", BoundaryString);

                        request.BeginGetRequestStream(new AsyncCallback(CreateRequestStreamCallback), request);
                       
                        break;
                    }
                case AcceptVerbs.PUT:
                    {
                        request.ContentType = "application/x-www-form-urlencoded";

                        if (this.Files != null)
                            if (this.Files.Count > 0)
                                request.ContentType = String.Concat("multipart/form-data; boundary=", BoundaryString);

                        request.BeginGetRequestStream(new AsyncCallback(CreateRequestStreamCallback), request);
                       
                        break;
                    }
                case AcceptVerbs.DELETE:
                    {
                        request.BeginGetResponse(new AsyncCallback(ResponseCallback), request);
                        this.ClearFiles();
                        this.Clear();
                        break;
                    }
                case AcceptVerbs.HEAD:
                    {
                        break;
                    }
                case AcceptVerbs.OPTIONS:
                    {
                        break;
                    }
                case AcceptVerbs.TRACE:
                    {
                        break;
                    }
            }
        }



       private void CreateRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            using (Stream postStream = webRequest.EndGetRequestStream(asynchronousResult))
            {
                byte[] byteArray = null;

                if (this.Files.Count > 0)
                    byteArray = this.RenderPost();
                else
                    byteArray = Encoding.UTF8.GetBytes(this.RenderForm(true));


                postStream.Write(byteArray, 0, byteArray.Length);
            }
            this.PrioritetServerName = string.Empty;
            this.Clear();
            this.ClearFiles();
            webRequest.BeginGetResponse(new AsyncCallback(ResponseCallback), webRequest);
           // allDone.WaitOne();
            
        }

       private void ResponseCallback(IAsyncResult result)
        {
            
            HttpWebRequest requestResult = result.AsyncState as HttpWebRequest;
            AcceptVerbs v = Helper.StringToAcceptVerbs(requestResult.Method);
            string ServerName = requestResult.RequestUri.Host;
            string ControllerName = requestResult.RequestUri.AbsolutePath;
            string URI = requestResult.RequestUri.OriginalString;
 
            try
            {
                if (requestResult == null) return;
                using (WebResponse response = requestResult.EndGetResponse(result))
                {
                    #region Обработка ответа
                    if (response == null)
                        return;
                    //Переписываем хеадер
                    List<Header> headersList = new List<Header>();
                    for (int i = 0; i < response.Headers.Count; ++i)
                        headersList.Add(new Header { Name = response.Headers.AllKeys[i], Value = response.Headers[response.Headers.AllKeys[i]] });

                    try
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {

                            if (this._RequestResult != null)
                                this.CallResult(null, reader, string.Empty, Helper.HeaderToStatus(headersList.ToArray()), v, ServerName, ControllerName, URI);
                        }
                    }
                    catch (Exception ex)
                    {
                        //this.CallResult(null, null, ex.Message, Status.StreamError);
                    }
                    #endregion
                    response.Close();
                }
            }
            #region Обработка исключений
            
            catch (WebException ex) 
            {
                try
                {
                    using (HttpWebResponse ExceptionResponse = ((HttpWebResponse)ex.Response))
                    {
                         using (StreamReader reader = new StreamReader(ExceptionResponse.GetResponseStream()))
                        {
                            this.CallResult(null, reader, ExceptionResponse.StatusCode.ToString(), Helper.StringToStatus(ExceptionResponse.StatusCode.ToString()), v, ServerName, ControllerName, URI);
                         }
                    }
                   
                    ((HttpWebResponse)ex.Response).Close();
                    
       
                }
                catch (Exception e)
                {
                   // this.CallResult(null, null, e.Message, Status.UnknownError);
                    if (((HttpWebResponse)ex.Response) != null)
                    {
                         ((HttpWebResponse)ex.Response).Close();
                        // ((HttpWebResponse)ex.Response).Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
               // this.CallResult(null, null, ex.Message, Status.UnknownError);
                if (requestResult != null)
                {
                    requestResult.Abort();
                    requestResult = null;
                }
            }
            #endregion

      
        }

       private void CallResult(string Content, StreamReader reader, string ErrMessage, Status status, AcceptVerbs v, string ServerName, string ControllerName, string URI)
       {

               string content = string.Empty;
               if (string.IsNullOrEmpty(ErrMessage))
                   ErrMessage = string.Empty;

               //Помойму бредовая конструкци
               if (string.IsNullOrEmpty(Content))
               {
                   if (reader != null)
                   {
                       content = reader.ReadToEnd();
                   }
               }
               else
               {
                   content = Content;
               }


           try
           {
               Delegate[] DelList = this._RequestResult.GetInvocationList();
               Delegate dele = DelList[0];
               
               int counte = DelList.Length;
               var tre = this._RequestResult.Method;

               this._RequestResult(new Result { 
                   IsOK = status == Status.OK ? true : false,
                   Content = content, 
                   ErrorMessage = ErrMessage, 
                   ResultStatus = status, 
                   Verbs = v, 
                   ControllerName = ControllerName,
                   URI = URI,
                   ServerName = ServerName});
             
              //// if(this.IsOnlyOneEventHandler)
              //     this._RequestResult = null;

              // /* if (this._RequestResult != null)
              //     if (this._RequestResult.GetInvocationList().Length >= 1)
              //         this._RequestResult = null;*/
           }
           catch
           {
               
           }
           
           
       }

        #endregion


       #region Fiels

       private List<File> Files;

       public void AddFile(byte[] content, string name, string filename, string contentType)
       {
           if (this.Files == null)
               this.Files = new List<File>();

           this.Files.Add(new File { Content = content, ContentType = contentType, FileName = filename, Name = name });
       }


       public void ClearFiles()
       {
           if (this.Files == null)
               this.Files = new List<File>();

           this.Files.Clear();
       }


       public void DeleteFile(string Name)
       {
           if (this.Files == null)
           {
               this.Files = new List<File>();
               return;
           }

           if (string.IsNullOrEmpty(Name))
               return;

           try
           {
               File element = null;
               foreach (var t in this.Files)
                   if (t.Name == Name)
                       element = t;

               if (element != null)
                   this.Files.Remove(element);
           }
           catch
           { }
       }

       #endregion


       private const string Prefix = "--";
       private const string NewLine = "\r\n";
       private string ms_boundary = string.Empty;

       public String BoundaryString
       {
           get
           {
               if (ms_boundary == string.Empty)
                   ResetBoundaryString();

               return ms_boundary;
           }

       }

       public void ResetBoundaryString()
       {
           ms_boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
       }

       private byte[] RenderPost()
       {
           StringBuilder sb = new StringBuilder();
           List<byte> MainResult = new List<byte>();


           if (this.Form != null)
               foreach (var f in this.Form)
               {
                   sb.Append(Prefix).Append(BoundaryString).Append(NewLine);
                   sb.Append("Content-Disposition: form-data; name=\"" + f.key + "\"");
                   sb.Append(NewLine);
                   sb.Append(NewLine);
                   sb.Append(f.value);
                   sb.Append(NewLine);
               }

           byte[] FormResult = Encoding.UTF8.GetBytes(sb.ToString());

           foreach (var q1 in FormResult)
               MainResult.Add(q1);


           if (this.Files != null)
               foreach (var fl in this.Files)
               {
                   sb = new StringBuilder();
                   sb.Append(Prefix).Append(BoundaryString).Append(NewLine);
                   sb.Append("Content-Disposition: form-data; filename=\"").Append(fl.FileName).Append("\"").Append(NewLine);
                   sb.Append("Content-Type: ").Append(fl.ContentType).Append(NewLine).Append(NewLine);


                   byte[] FHeader = Encoding.UTF8.GetBytes(sb.ToString());
                   byte[] FContent = fl.Content;
                   byte[] FLine = Encoding.UTF8.GetBytes(NewLine);

                   byte[] postData = new byte[FHeader.Length + FContent.Length + FLine.Length];
                   Buffer.BlockCopy(FHeader, 0, postData, 0, FHeader.Length);
                   Buffer.BlockCopy(FContent, 0, postData, FHeader.Length, FContent.Length);
                   Buffer.BlockCopy(FLine, 0, postData, FHeader.Length + FContent.Length, FLine.Length);

                   foreach (var q2 in postData)
                       MainResult.Add(q2);

               }


           byte[] FooterResult = Encoding.UTF8.GetBytes(String.Concat(Prefix, BoundaryString, Prefix, NewLine));

           foreach (var q3 in FooterResult)
               MainResult.Add(q3);


           return MainResult.ToArray();
       }


    }




}
