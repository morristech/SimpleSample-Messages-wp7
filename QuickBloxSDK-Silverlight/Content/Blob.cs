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
    public class Blob
    {
        public Blob(string XML)
        {
            this.Parse(XML);
        }

        #region Fields

        public string BlobExtendedStatus
        { get; set; }


        public uint BlobOwnerId
        { get; set; }


        public BlobStatus BStatus
        { get; set; }


        public string ContentType
        { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreatedAt
        { get; set; }


        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime? LastReadAccessTs
        { get; set; }


        public DateTime? SetCompletedAt
        { get; set; }


        public DateTime UpdatedAt
        { get; set; }

        /// <summary>
        /// Идентификатор
        /// </summary>
        public uint Id
        { get; set; }

        /// <summary>
        /// Время жизни блоба
        /// </summary>
        public int Lifetime
        { get; set; }

        /// <summary>
        /// Название блоба
        /// </summary>
        public string Name
        { get; set; }


        /// <summary>
        /// Доступен
        /// </summary>
        public bool IsPublic
        { get; set; }

        /// <summary>
        /// Колличество скачиваний
        /// </summary>
        public uint RefCount
        { get; set; }

        /// <summary>
        /// Размер файла
        /// </summary>
        public uint Size
        { get; set; }

        /// <summary>
        /// Теги
        /// </summary>
        public string Tags
        { get; set; }


        public string UID
        { get; set; }

        public BlobObjectAccess BOA
        { get; set; }

        #endregion


        #region

        private void Parse(string xml)
        {
            try
            {
                XElement xmlResult = XElement.Parse(xml);
                this.Id = uint.Parse(xmlResult.Element("id").Value);
                this.BlobOwnerId = uint.Parse(xmlResult.Element("blob-owner-id").Value);
                //----
                this.CreatedAt = DateTime.Parse(xmlResult.Element("created-at").Value);
                this.UpdatedAt = DateTime.Parse(xmlResult.Element("updated-at").Value);
                //----
                this.LastReadAccessTs = (string.IsNullOrEmpty(xmlResult.Element("last-read-access-ts").Value) ? (DateTime?)null : DateTime.Parse(xmlResult.Element("last-read-access-ts").Value));
                this.SetCompletedAt = (string.IsNullOrEmpty(xmlResult.Element("set-completed-at").Value) ? (DateTime?)null : DateTime.Parse(xmlResult.Element("set-completed-at").Value));
                //----
                this.Lifetime = string.IsNullOrEmpty(xmlResult.Element("lifetime").Value) ? 0 : int.Parse(xmlResult.Element("lifetime").Value);
                this.RefCount = string.IsNullOrEmpty(xmlResult.Element("ref-count").Value) ? 0 : uint.Parse(xmlResult.Element("ref-count").Value);
                this.Size = string.IsNullOrEmpty(xmlResult.Element("size").Value) ? 0 : uint.Parse(xmlResult.Element("size").Value);
                //-----
                this.IsPublic = xmlResult.Element("public").Value == "true" ? true : false;
                //-----
                this.ContentType = xmlResult.Element("content-type").Value;
                this.BlobExtendedStatus = xmlResult.Element("blob-extended-status").Value;
                this.Name = xmlResult.Element("name").Value;
                this.Tags = xmlResult.Element("tags").Value;
                //-----
                this.BOA = new BlobObjectAccess(xmlResult.Element("blob-object-access").Value);
                //----
                switch (xmlResult.Element("blob-status").Value)
                {
                    case "Complete":
                        {
                            this.BStatus = BlobStatus.Complete;
                            break;
                        }
                    case "Locked":
                        {
                            this.BStatus = BlobStatus.Locked;
                            break;
                        }
                    case "New":
                        {
                            this.BStatus = BlobStatus.New;
                            break;
                        }
                }

                this.UID = xmlResult.Element("uid").Value;


            }

            catch (Exception ex)
            {
               
            }

        }

        #endregion
    }
}
