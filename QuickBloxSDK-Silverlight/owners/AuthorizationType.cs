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

namespace QuickBloxSDK_Silverlight.owners
{
    /// <summary>
    /// Authorization type
    /// Shows which fields are needed to authorize a user
    /// </summary>
    public enum AuthorizationType
    {
        /// <summary>
        /// Only by login
        /// </summary>
        login,
        /// <summary>
        /// Login and password
        /// </summary>
        login_password,
        /// <summary>
        /// By device
        /// </summary>
        device,
        /// <summary>
        /// By E-mail address and password
        /// </summary>
        email_password
    }
}
