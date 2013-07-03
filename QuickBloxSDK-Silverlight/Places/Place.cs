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

//
namespace QuickBloxSDK_Silverlight.Places
{
    /// <summary>
    /// Класс представляет собой место отмеченное на карте
    /// </summary>
    public class Place
    {
        #region Конструкторы
        public Place()
        { }

        public Place(int PhotoId, int GeoDataId, string PlaceTitle)
        {
            this.PhotoId = PhotoId;
            this.GeoDataId = GeoDataId;
            this.Title = PlaceTitle;
        }

        public Place(string Xml)
        {
            this.Parse(Xml);

        }
        #endregion
        #region filds
        /// <summary>
        /// Уникальный идентификатор элемента
        /// </summary>
        public int Id
        { get; private set; }

        /// <summary>
        /// Адресс данного местоположения
        /// </summary>
        public string Adrress
        { get; set; }


        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title
        { get; set; }

        /// <summary>
        /// Дата создания объекта
        /// </summary>
        public DateTime CreatedDate
        { get; private set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description
        { get; set; }

        /// <summary>
        /// Идентификаор местоположения к которому привязано место
        /// </summary>
        public int GeoDataId
        { get; set; }

        /// <summary>
        /// Идентификатор картинки привязанной к этому месту
        /// </summary>
        public int PhotoId
        { get; set; }

        /// <summary>
        /// Дата редактирование места
        /// </summary>
        public DateTime? UpdatedDate
        {
            get;
            private set;
        }

        /// <summary>
        /// Широта
        /// </summary>
        public decimal Latitude
        { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        public decimal Longitude
        { get; set; }

        #endregion
        #region
        /// <summary>
        /// Converts object into string
        /// </summary>
        /// <returns>Title</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Title) ? string.Empty : Title;
        }

        /// <summary>
        /// Parse user that came from server
        /// </summary>
        /// <param name="xml"></param>
        private void Parse(string xml)
        {
            try
            {
                XElement xmlResult = XElement.Parse(xml);
                this.Id = int.Parse(xmlResult.Element("id").Value);
                //----
                this.CreatedDate = DateTime.Parse(xmlResult.Element("created-at").Value);
                this.UpdatedDate = DateTime.Parse(xmlResult.Element("updated-at").Value);
                //----
                this.GeoDataId = string.IsNullOrEmpty(xmlResult.Element("geo-data-id").Value) ? 0 : int.Parse(xmlResult.Element("geo-data-id").Value);
                this.PhotoId = string.IsNullOrEmpty(xmlResult.Element("photo-id").Value) ? 0 : int.Parse(xmlResult.Element("photo-id").Value);

                this.Description = xmlResult.Element("description").Value;
                this.Title = xmlResult.Element("title").Value;
                this.Adrress = xmlResult.Element("address").Value;

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
            
            catch (Exception ex)
            {
                this.Id = -1;
                
            }
                
        }
        #endregion

    }
}
