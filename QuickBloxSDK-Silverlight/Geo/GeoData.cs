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
using QuickBloxSDK_Silverlight.users;

namespace QuickBloxSDK_Silverlight.Geo
{
    /// <summary>
    /// Объект данного класса - это пушпин на карте.
    /// 
    /// or not correct then identifier is -1
    /// </summary>
    public class GeoData
    {

        #region

        /// <summary>
        /// Создание геодаты
        /// </summary>
        /// <param name="UserId">Идентификатор пользователя которому принадлежит данное местоположение</param>
        /// <param name="Latitude">Географическая широта</param>
        /// <param name="Longitude">Географическая долгота</param>
        /// <param name="Status">Статус. Представляет из себя текстовое поле длинной от 10 до 1000 символов</param>
        public GeoData(int UserId, decimal Latitude, decimal Longitude, string Status)
        {
            if (UserId < 1)
                throw new ArgumentException();

            this.UserId = UserId;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Status = Status;
        }
        /// <summary>
        /// Создание геодаты по схеме
        /// </summary>
        /// <param name="Scheme">XML схема</param>
        public GeoData(string Scheme)
        {
            if (string.IsNullOrEmpty(Scheme))
                throw new Exception("Content error");

            try
            {
                XElement xmlResult = XElement.Parse(Scheme);
                this.Id = int.Parse(xmlResult.Element("id").Value);
                //----
                this.CreatedDate = DateTime.Parse(xmlResult.Element("created-at").Value);
                this.UpdatedDate = DateTime.Parse(xmlResult.Element("updated-at").Value);
                //----
                this.UserId = int.Parse(xmlResult.Element("user-id").Value);
                this.AppId = int.Parse(xmlResult.Element("application-id").Value);
                try
                {
                    this.user = new User(xmlResult.Element("user").ToString());
                }
                catch { }                
                try
                {
                    this.CreatedAtTimestamp = string.IsNullOrEmpty(xmlResult.Element("created-at-timestamp").Value) ? 0 : int.Parse(xmlResult.Element("created-at-timestamp").Value);
                }
                catch
                {
                    this.CreatedAtTimestamp = 0;
                }
                //------
                this.Status = xmlResult.Element("status").Value;
               //------------

                try
                {
                    this.Longitude = decimal.Parse(xmlResult.Element("longitude").Value);
                }
                catch
                {
                    try{
                        this.Longitude = decimal.Parse(xmlResult.Element("longitude").Value.Replace('.', ','));
                    }
                    catch
                    {
 
                    }
                }
                try
                {
                    this.Latitude = decimal.Parse(xmlResult.Element("latitude").Value);
                }
                catch
                {
                    try
                    {
                        this.Latitude = decimal.Parse(xmlResult.Element("latitude").Value.Replace('.', ','));
                    }
                    catch
                    {

                    }
                }
                
                
            }
            catch(Exception ex)
            {
                throw new Exception("Content error");
            }

 
        }
        #endregion
        #region
        /// <summary>
        /// Идентификатор местоположения в базе данных
        /// </summary>
        public int Id
        { get; private set; }

        /// <summary>
        /// Идентификтаор пользователя которому принадлежит местоположение
        /// </summary>
        public int UserId
        { get; set; }

        /// <summary>
        /// Дата создания местоположения
        /// </summary>
        public DateTime CreatedDate
        { get; private set; }

        /// <summary>
        /// Идентификатор приложения которому принадлежит это местоположения
        /// </summary>
        public int AppId
        { get; private set; }

        /// <summary>
        /// Дата редактирования.
        /// Свегда совпадает с датой создания, так как редактирование геодаты невозможно
        /// </summary>
        public DateTime UpdatedDate
        { get; private set; }

        /// <summary>
        /// Географическая широта
        /// </summary>
        public decimal Latitude
        { get; set; }

        /// <summary>
        /// Географическая долгота
        /// </summary>
        public decimal Longitude
        { get; set; }

        /// <summary>
        /// Статус. Любой текст до 1000 символов
        /// </summary>
        public string Status
        { get; set; }

      /// <summary>
      /// Дата создания
      /// </summary>
        public int CreatedAtTimestamp
        { get; private set; }

        /// <summary>
        /// Пользователь которому принадлежит данное местоположение
        /// </summary>
        public User user
        { get; private set; }

        #endregion
    }
}
