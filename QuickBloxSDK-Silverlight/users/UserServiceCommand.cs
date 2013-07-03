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

namespace QuickBloxSDK_Silverlight.users
{
    /// <summary>
    /// Команды которые может выполнять сервис для работы с пользователями
    /// </summary>
    public enum UserServiceCommand
    {
        /// <summary>
        /// Получение конкретного пользователя по идентификатору
        /// </summary>
        GetUser,
        /// <summary>
        /// Получиение всех пользователей для прилоежения
        /// </summary>
        GetUsers,
        /// <summary>
        /// Получение пользователя по внешнему идентификатору
        /// </summary>
        GetUserByExternalId,
        /// <summary>
        /// Получение пользователя под адресу электронной почты
        /// </summary>
        GetUserByEmail,
        /// <summary>
        /// Добавление пользователя
        /// </summary>
        AddUser,
        /// <summary>
        /// Удаление пользователя
        /// </summary>
        DeleteUser,
        /// <summary>
        /// Редактирование пользователя
        /// </summary>
        EditUser,
        /// <summary>
        /// Задание новго пароля
        /// </summary>
        SetNewPassword,
        /// <summary>
        /// Подтверждения адреса электронной почты
        /// </summary>
        EmailVerification,
        /// <summary>
        /// Аутентифкация пользователя
        /// </summary>
        Authenticate,
        /// <summary>
        /// Пинг
        /// </summary>
        Identify,
        /// <summary>
        /// Выход из системы пользователя
        /// </summary>
        Logout,
        /// <summary>
        /// Сброс пароля по электронной почты
        /// </summary>
        Resetmypasswordbyemail,
        /// <summary>
        /// Сброс пароля
        /// </summary>
        Resetpassword,
        /// <summary>
        /// Получение пользователей по идентификатору овнера
        /// </summary>
        GetUsersByOwner

    }
}
