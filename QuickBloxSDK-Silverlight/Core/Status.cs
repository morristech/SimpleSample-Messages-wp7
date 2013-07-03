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

namespace QuickBloxSDK_Silverlight.Core
{
    /// <summary>
    /// Статус выполненой операции
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Операция выполнена успешно
        /// </summary>
        OK,
        /// <summary>
        /// При отправки формы некоторые поля не прошли валидацию на сервере
        /// </summary>
        ValidationError,
        /// <summary>
        ///  Запрашиваемый ресурс не был найден
        /// </summary>
        NotFoundError,

        /// <summary>
        /// Команда не может быть выполнена
        /// </summary>
        MethodNotAllowed,
        /// <summary>
        /// Connection error to the server(incorrect options)
        /// or network problems
        /// </summary>
        ConnectionError,
        /// <summary>
        /// Timeout error
        /// </summary>
        TimeoutError,
        /// <summary>
        /// Authentification error
        /// </summary>
        AuthenticationError,
        /// <summary>
        /// Problems with output stream
        /// </summary>
        StreamError,
        /// <summary>
        /// Доступ запрещен или нет прав для просмотре ресурса или выполнения команды
        /// </summary>
        AccessDenied,
        /// <summary>
        /// Неизвестная ошибка
        /// </summary>
        UnknownError,
        /// <summary>
        /// (Богдан - переведи на русский и потом нормально обратно))) Has occured nothing or the result doesn't approach under the known description
        /// </summary>
        none,
        /// <summary>
        /// Контент который пришол от сервера не может быть правильно обработан
        /// </summary>
        ContentError,
        /// <summary>
        /// Пустой ответ от сервера
        /// </summary>
        NullContent,
        NotAcceptable,
        Unauthorized

    }
}
