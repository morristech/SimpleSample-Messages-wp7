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

namespace QuickBloxSDK_Silverlight.users
{
    /// <summary>
    /// User
    /// </summary>
    public class User
    {
        #region
        /// <summary>
        /// User ID in database.
        /// </summary>
        public int id
        { get; private set; }

        /// <summary>
        /// User name
        /// </summary>
        public string Username
        { get;  set; }

        public int OwnerId
        { get;  set; }

        /// <summary>
        /// Device ID
        /// </summary>
        public string DeviceId
        { get; set; }

        /// <summary>
        /// Пароль.
        /// Поле используется толлько при создани нового пользователя.
        /// </summary>
        public string Password
        { get; set; }

        /// <summary>
        /// E-mail address
        /// </summary>
        public string Email
        { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime? CreatedDate
        {
            get;
            private set;
        }

        /// <summary>
        /// When the E-mail address was confirmed
        /// </summary>
        public DateTime? EmailVerificationCodeCreatedOn
        {
            get;
            private set;
        }

        /// <summary>
        /// Last access to the server
        /// </summary>
        public DateTime? LastRequestDate
        {
            get;
            private set;
        }

        /// <summary>
        /// Creation date of recovery password key
        /// </summary>
        public DateTime? PasswordResetCodeCreatedDate
        {
            get;
            private set;
        }

        public DateTime? UpdatedDate
        {
            get;
            private set;
        }

        

        public int? ExternalUserId
        { get; set; }

        

        public string TwitterId
        { get; set; }



        public string CryptedPassword
        { get; private set; }

        


        public string EmailNotValidated
        { get; private set; }


        public string EmailVerificationCode
        { get; private set; }


        public string FacebookId
        { get; set; }


        public string FullName
        { get;  set; }


        public string PasswordResetCode
        { get; private set; }

        public string PasswordSalt
        { get; private set; }

        public string PersistenceToken
        { get; private set; }

        public string Phone
        { get; set; }

        public string Website
        { get; set; }

        #endregion
        #region конструкторы
        public User(string Username, int Owner, string Email, string Device)
        {
            if (string.IsNullOrEmpty(Username) || Owner < 0)
                throw new ArgumentException("Argument error");
            this.OwnerId = Owner;
            this.Email = Email;
            this.Username = Username;
            this.DeviceId = Device;
        }

        public User(string Username, int Owner, string Email, string Device, string FacebookId)
        {
            if (string.IsNullOrEmpty(Username) || Owner < 0 || string.IsNullOrEmpty(FacebookId) || string.IsNullOrEmpty(Email))
                throw new ArgumentException("Argument error");
            this.OwnerId = Owner;
            this.Email = Email;
            this.Username = Username;
            this.DeviceId = Device;
            this.FacebookId = FacebookId;
        }


        public User(string Username, int Owner, string Email, string Device, string FacebookId, string TwitterId)
        {
            if (string.IsNullOrEmpty(Username) || Owner < 0 || string.IsNullOrEmpty(FacebookId) || string.IsNullOrEmpty(Email))
                throw new ArgumentException("Argument error");
            this.OwnerId = Owner;
            this.Email = Email;
            this.Username = Username;
            this.DeviceId = Device;
            this.FacebookId = FacebookId;
            this.TwitterId = TwitterId;
        }

        public User(int id, string Username, int Owner, string Email, string Device)
        {
            if (string.IsNullOrEmpty(Username) || id < 0 || Owner < 0)
                throw new ArgumentException("Argument error");

            this.id = id;
            this.OwnerId = Owner;
            this.Email = Email;
            this.Username = Username;
            this.DeviceId = Device;
        }

        public User()
        { }

        public User(string Xml)
        {
            this.Parse(Xml);

        }
        #endregion
        #region Методы 
         
        /// <summary>
        /// Converts object into string
        /// </summary>
        /// <returns>User name</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Username) ? string.Empty : Username;
        }

        /// <summary>
        /// Parse user that came from server
        /// </summary>
        /// <param name="xml"></param>
        private void Parse(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                throw new Exception("Content error");
            try
            {
                XElement xmlResult = XElement.Parse(xml);
                this.id = int.Parse(xmlResult.Element("id").Value);
                this.Username = xmlResult.Element("login").Value;
                //----
                this.CreatedDate = DateTime.Parse(xmlResult.Element("created-at").Value);

                try
                {
                    this.EmailVerificationCodeCreatedOn = (string.IsNullOrEmpty(xmlResult.Element("email-verification-code-created-on").Value) ? (DateTime?)null : DateTime.Parse(xmlResult.Element("email-verification-code-created-on").Value));
                }
                catch { }

                this.UpdatedDate = (string.IsNullOrEmpty(xmlResult.Element("updated-at").Value) ? (DateTime?)null : DateTime.Parse(xmlResult.Element("updated-at").Value));
                try
                {
                    this.LastRequestDate = (string.IsNullOrEmpty(xmlResult.Element("last-request-at").Value) ? (DateTime?)null : DateTime.Parse(xmlResult.Element("last-request-at").Value));
                }
                catch
                { }
                try
                {
                    this.PasswordResetCodeCreatedDate = (string.IsNullOrEmpty(xmlResult.Element("password-reset-code-created-on").Value) ? (DateTime?)null : DateTime.Parse(xmlResult.Element("password-reset-code-created-on").Value));
                }
                catch
                { }
                try
                {
                    this.DeviceId = xmlResult.Element("device-id").Value; // string.IsNullOrEmpty(xmlResult.Element("device-id").Value) ? (int?)null : int.Parse(xmlResult.Element("device-id").Value);
                }
                catch
                { }
                try
                {
                    this.ExternalUserId = string.IsNullOrEmpty(xmlResult.Element("external-user-id").Value) ? (int?)null : int.Parse(xmlResult.Element("external-user-id").Value);
                }
                catch
                { }

                this.OwnerId = string.IsNullOrEmpty(xmlResult.Element("owner-id").Value) ? 0 : int.Parse(xmlResult.Element("owner-id").Value);

                try
                {
                    this.CryptedPassword = xmlResult.Element("crypted-password").Value;
                }
                catch { }
                try
                {
                this.Email = xmlResult.Element("email").Value;
                }
                catch { }
                try
                {
                this.EmailNotValidated = xmlResult.Element("email-not-validated").Value;
                }
                catch { }
                try
                {
                this.EmailVerificationCode = xmlResult.Element("email-verification-code").Value;
                }
                catch { }
                try
                {
                    this.FacebookId = xmlResult.Element("facebook-id").Value;
                }
                catch { }
                try
                {
                this.TwitterId = xmlResult.Element("twitter-id").Value;
                }
                catch { }
                try
                {
                this.FullName = xmlResult.Element("full-name").Value;
                }
                catch { }
                try
                {
                this.PasswordResetCode = xmlResult.Element("password-reset-code").Value;
                }
                catch { }
                try
                {
                this.PasswordSalt = xmlResult.Element("password-salt").Value;
                }
                catch { }
                try
                {
                this.PersistenceToken = xmlResult.Element("persistence-token").Value;
                }
                catch { }
                try
                {
                this.Phone = xmlResult.Element("phone").Value;
                }
                catch { }
                try
                {
                this.Website = xmlResult.Element("website").Value;
                }
                catch { }
                

            }
            catch
            {
                throw new Exception("Content error");
            }
        }
        #endregion
      
    }
}
