﻿using System;
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
using System.Collections.Generic;

namespace QuickBloxSDK_Silverlight.users
{
    public class UserPage
    {

        public UserPage(string XmlScheme)
        {

            this.UsersOnPage = -1;
            this.CurrentPage = -1;
            this.TotalUserCount = -1;
            this.IsPageLoad = false;

            if (string.IsNullOrEmpty(XmlScheme))
                return;

            try
            {
                List<User> users = new List<User>();
                XElement xml = XElement.Parse(XmlScheme);
                foreach (var t in xml.Descendants("user"))
                    users.Add(new User(t.ToString()));

                this.Users = users.ToArray();

                try
                {
                    this.UsersOnPage = int.Parse(xml.Attribute("per_page").Value);
                    this.CurrentPage = int.Parse(xml.Attribute("current_page").Value);
                    this.TotalUserCount = int.Parse(xml.Attribute("total_entries").Value);
                }
                catch
                {

                }

                this.IsPageLoad = true;
            }
            catch
            { }

        }


        /// <summary>
        /// Была ли загружена правильно страница
        /// </summary>
        public bool IsPageLoad
        { get; private set; }

        /// <summary>
        /// Total pages
        /// </summary>
        public int UsersOnPage
        { get; private set; }

        /// <summary>
        /// Текущая страница
        /// </summary>
        public int CurrentPage
        { get; private set; }

        /// <summary>
        /// Колличество пользователей в приожении
        /// </summary>
        public int TotalUserCount
        { get; private set; }

        /// <summary>
        /// Пользователи на странице
        /// </summary>
        public User[] Users
        { get; private set; }

    }
}
