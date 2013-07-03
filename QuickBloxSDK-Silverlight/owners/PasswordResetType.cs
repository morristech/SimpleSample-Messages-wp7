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
    /// Тип изменения пароля
    /// </summary>
    public enum PasswordResetType
    {
        none,
        email,
        email_code,
        email_code_email
    }
}
